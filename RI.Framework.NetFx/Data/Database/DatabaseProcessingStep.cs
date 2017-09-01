using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database
{
	/// <summary>
	/// Implements a single database processing step.
	/// </summary>
	/// <typeparam name="TConnection">The database connection type, subclass of <see cref="DbConnection"/>.</typeparam>
	/// <typeparam name="TTransaction">The database transaction type, subclass of <see cref="DbTransaction"/>.</typeparam>
	/// <typeparam name="TConnectionStringBuilder">The connection string builder type, subclass of <see cref="DbConnectionStringBuilder"/>.</typeparam>
	/// <typeparam name="TManager">The type of the database manager.</typeparam>
	/// <typeparam name="TConfiguration">The type of database configuration.</typeparam>
	/// <remarks>
	/// <para>
	/// By adding sub-steps (<see cref="AddScript(string,DatabaseProcessingStepTransactionRequirement)"/>, <see cref="AddBatch(string,DatabaseProcessingStepTransactionRequirement)"/>, <see cref="AddBatches(IEnumerable{string},DatabaseProcessingStepTransactionRequirement)"/>, <see cref="AddCode(DatabaseProcessingStepDelegate{TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration},DatabaseProcessingStepTransactionRequirement)"/>, <see cref="AddCode(DatabaseProcessingStepDelegate,DatabaseProcessingStepTransactionRequirement)"/>), a processing step can be heavily customized by using both application code through delegates and SQL scripts.
	/// </para>
	/// <para>
	/// The sub-steps are executed in the order they are added.
	/// </para>
	/// </remarks>
	public abstract class DatabaseProcessingStep<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> : ILogSource
		where TConnection : DbConnection
		where TTransaction : DbTransaction
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TConnectionStringBuilder, TManager>
		where TConfiguration : class, IDatabaseManagerConfiguration<TConnection, TConnectionStringBuilder, TManager>, new()
	{
		private bool _loggingEnabled;

		/// <inheritdoc />
		bool ILogSource.LoggingEnabled
		{
			get
			{
				return this._loggingEnabled;
			}
			set
			{
				this._loggingEnabled = value;
			}
		}

		private ILogger _logger;

		/// <inheritdoc />
		ILogger ILogSource.Logger
		{
			get
			{
				return this._logger;
			}
			set
			{
				this._logger = value;
			}
		}

		/// <summary>
		/// Creates a new instance of <see cref="DatabaseProcessingStep{TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration}"/>
		/// </summary>
		protected DatabaseProcessingStep()
		{
			this.SubSteps = new List<Tuple<SubStepType, DatabaseProcessingStepTransactionRequirement, object>>();
		}

		private List<Tuple<SubStepType, DatabaseProcessingStepTransactionRequirement, object>> SubSteps { get; }

		/// <summary>
		/// Gets whether this processing step requires a script locator.
		/// </summary>
		/// <value>
		/// true if a script locator is required, false otherwise.
		/// </value>
		public bool RequiresScriptLocator => this.SubSteps.Count == 0 ? false : this.SubSteps.Any(x => x.Item1 == SubStepType.Script);

		/// <summary>
		/// Gets whether this processing step requires a transaction when executed.
		/// </summary>
		/// <value>
		/// true if a transaction is required, false otherwise.
		/// </value>
		public bool RequiresTransaction => this.SubSteps.Count == 0 ? false : this.SubSteps.Any(x => x.Item2 == DatabaseProcessingStepTransactionRequirement.Required);

		/// <summary>
		/// Executes the processing step and all its sub-steps.
		/// </summary>
		/// <param name="manager">The used database manager.</param>
		/// <param name="connection">The used database connection.</param>
		/// <param name="transaction">The used database transaction. Can be null if no transaction is used.</param>
		/// <exception cref="ArgumentNullException"><paramref name="manager"/> or <paramref name="connection"/> is null.</exception>
		public void Execute (TManager manager, TConnection connection, TTransaction transaction)
		{
			if (manager == null)
			{
				throw new ArgumentNullException(nameof(manager));
			}

			if (connection == null)
			{
				throw new ArgumentNullException(nameof(connection));
			}

			((ILogSource)this).LoggingEnabled = ((ILogSource)manager).LoggingEnabled;
			((ILogSource)this).Logger = ((ILogSource)manager).Logger;

			if (this.SubSteps.Count == 0)
			{
				return;
			}

			if (this.SubSteps.Any(x => x.Item2 == DatabaseProcessingStepTransactionRequirement.Required) && this.SubSteps.Any(x => x.Item2 == DatabaseProcessingStepTransactionRequirement.Disallowed))
			{
				throw new Exception("Conflicting transaction requirements.");
			}

			List<object> subSteps = new List<object>();
			foreach (Tuple<SubStepType, DatabaseProcessingStepTransactionRequirement, object> subStep in this.SubSteps)
			{
				List<string> batches = null;
				DatabaseProcessingStepDelegate<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> callbackTyped = null;
				DatabaseProcessingStepDelegate callbackGeneric = null;

				switch (subStep.Item1)
				{
						case SubStepType.Script:
							string scriptName = (string)subStep.Item3;
							batches = manager.GetScriptBatch(scriptName, true);
							if (batches == null)
							{
								throw new Exception("Batch retrieval failed for script: " + scriptName);
							}
							break;

						case SubStepType.Batch:
							batches = new List<string>();
							batches.Add((string)subStep.Item3);
							break;

						case SubStepType.Batches:
							batches = (List<string>)subStep.Item3;
							break;

						case SubStepType.CodeTyped:
							callbackTyped = (DatabaseProcessingStepDelegate<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>)subStep.Item3;
							break;

						case SubStepType.CodeGeneric:
							callbackGeneric = (DatabaseProcessingStepDelegate)subStep.Item3;
							break;
				}

				if (batches != null)
				{
					subSteps.Add(batches);
				}

				if (callbackTyped != null)
				{
					subSteps.Add(callbackTyped);
				}

				if (callbackGeneric != null)
				{
					subSteps.Add(callbackGeneric);
				}
			}

			foreach (object subStep in subSteps)
			{
				List<string> batches = subStep as List<string>;
				DatabaseProcessingStepDelegate<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> callbackTyped = subStep as DatabaseProcessingStepDelegate<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>;
				DatabaseProcessingStepDelegate callbackGeneric = subStep as DatabaseProcessingStepDelegate;

				if (batches != null)
				{
					this.ExecuteBatchesImpl(batches, manager, connection, transaction);
				}

				if (callbackTyped != null)
				{
					this.ExecuteCallbackImpl(callbackTyped, manager, connection, transaction);
				}

				if (callbackGeneric != null)
				{
					this.ExecuteCallbackImpl(callbackGeneric, manager, connection, transaction);
				}
			}
		}

		/// <summary>
		/// Implements the database-specific execution of batches.
		/// </summary>
		/// <param name="batches">The batches to execute.</param>
		/// <param name="manager">The used database manager.</param>
		/// <param name="connection">The used database connection.</param>
		/// <param name="transaction">The used database transaction. Can be null if no transaction is used.</param>
		protected abstract void ExecuteBatchesImpl (List<string> batches, TManager manager, TConnection connection, TTransaction transaction);

		/// <summary>
		/// Implements the database-specific execution of callbacks.
		/// </summary>
		/// <param name="callback">The callback to execute.</param>
		/// <param name="manager">The used database manager.</param>
		/// <param name="connection">The used database connection.</param>
		/// <param name="transaction">The used database transaction. Can be null if no transaction is used.</param>
		/// <remarks>
		/// <para>
		/// The default implementation invokes <paramref name="callback"/>.
		/// </para>
		/// </remarks>
		protected virtual void ExecuteCallbackImpl (DatabaseProcessingStepDelegate<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> callback, TManager manager, TConnection connection, TTransaction transaction)
		{
			this.Log(LogLevel.Debug, "Executing database processing callback: {0}.{1}", callback.Method?.DeclaringType?.FullName ?? "[null]", callback.Method?.Name ?? "[null]");
			callback(this, manager, connection, transaction);
		}

		/// <summary>
		/// Implements the database-specific execution of callbacks.
		/// </summary>
		/// <param name="callback">The callback to execute.</param>
		/// <param name="manager">The used database manager.</param>
		/// <param name="connection">The used database connection.</param>
		/// <param name="transaction">The used database transaction. Can be null if no transaction is used.</param>
		/// <remarks>
		/// <para>
		/// The default implementation invokes <paramref name="callback"/>.
		/// </para>
		/// </remarks>
		protected virtual void ExecuteCallbackImpl(DatabaseProcessingStepDelegate callback, TManager manager, TConnection connection, TTransaction transaction)
		{
			this.Log(LogLevel.Debug, "Executing database processing callback: {0}.{1}", callback.Method?.DeclaringType?.FullName ?? "[null]", callback.Method?.Name ?? "[null]");
			callback(this, manager, connection, transaction);
		}

		/// <summary>
		/// Adds a script using its script name.
		/// </summary>
		/// <param name="scriptName">The name of the script.</param>
		/// <remarks>
		/// <para>
		/// The script is resolved using the script locator provided by the database manager.
		/// </para>
		/// <para>
		/// <see cref="DatabaseProcessingStepTransactionRequirement.DontCare"/> is used as the transaction requirement.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="scriptName"/> is null.</exception>
		/// <exception cref="EmptyStringArgumentException"><paramref name="scriptName"/> is an empty string.</exception>
		public void AddScript (string scriptName) => this.AddScript(scriptName, DatabaseProcessingStepTransactionRequirement.DontCare);

		/// <summary>
		/// Adds a script using its script name.
		/// </summary>
		/// <param name="scriptName">The name of the script.</param>
		/// <param name="transactionRequirement">The transaction requirement.</param>
		/// <remarks>
		/// <para>
		/// The script is resolved using the script locator provided by the database manager.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="scriptName"/> is null.</exception>
		/// <exception cref="EmptyStringArgumentException"><paramref name="scriptName"/> is an empty string.</exception>
		public void AddScript (string scriptName, DatabaseProcessingStepTransactionRequirement transactionRequirement)
		{
			if (scriptName == null)
			{
				throw new ArgumentNullException(nameof(scriptName));
			}

			if (scriptName.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(scriptName));
			}

			this.SubSteps.Add(new Tuple<SubStepType, DatabaseProcessingStepTransactionRequirement, object>(SubStepType.Script, transactionRequirement, scriptName));
		}

		/// <summary>
		/// Adds a single batch as SQL script code.
		/// </summary>
		/// <param name="batch">The SQL script.</param>
		/// <remarks>
		/// <para>
		/// The SQL script is executed as passed by this method, without further processing, as a single command.
		/// </para>
		/// <para>
		/// <see cref="DatabaseProcessingStepTransactionRequirement.DontCare"/> is used as the transaction requirement.
		/// </para>
		/// <para>
		/// If <paramref name="batch"/> is null or empty, no sub-step is added.
		/// </para>
		/// </remarks>
		public void AddBatch (string batch) => this.AddBatch(batch, DatabaseProcessingStepTransactionRequirement.DontCare);

		/// <summary>
		/// Adds a single batch as SQL script code.
		/// </summary>
		/// <param name="batch">The SQL script.</param>
		/// <param name="transactionRequirement">The transaction requirement.</param>
		/// <remarks>
		/// <para>
		/// The SQL script is executed as passed by this method, without further processing, as a single command.
		/// </para>
		/// <para>
		/// If <paramref name="batch"/> is null or empty, no sub-step is added.
		/// </para>
		/// </remarks>
		public void AddBatch (string batch, DatabaseProcessingStepTransactionRequirement transactionRequirement)
		{
			if (batch == null)
			{
				return;
			}

			if (batch.IsNullOrEmptyOrWhitespace())
			{
				return;
			}

			this.SubSteps.Add(new Tuple<SubStepType, DatabaseProcessingStepTransactionRequirement, object>(SubStepType.Batch, transactionRequirement, batch));
		}

		/// <summary>
		/// Adds batches as SQL script code.
		/// </summary>
		/// <param name="batches">The SQL scripts.</param>
		/// <remarks>
		/// <para>
		/// The SQL scripts are executed as passed by this method, without further processing, as a single command per batch.
		/// </para>
		/// <para>
		/// <see cref="DatabaseProcessingStepTransactionRequirement.DontCare"/> is used as the transaction requirement.
		/// </para>
		/// <para>
		/// If <paramref name="batches"/> is null or empty, no sub-step is added.
		/// </para>
		/// <para>
		/// <paramref name="batches"/> is enumerated only once.
		/// </para>
		/// </remarks>
		public void AddBatches (IEnumerable<string> batches) => this.AddBatches(batches, DatabaseProcessingStepTransactionRequirement.DontCare);

		/// <summary>
		/// Adds batches as SQL script code.
		/// </summary>
		/// <param name="batches">The SQL scripts.</param>
		/// <param name="transactionRequirement">The transaction requirement.</param>
		/// <remarks>
		/// <para>
		/// The SQL scripts are executed as passed by this method, without further processing, as a single command per batch.
		/// </para>
		/// <para>
		/// If <paramref name="batches"/> is null or empty, no sub-step is added.
		/// </para>
		/// <para>
		/// <paramref name="batches"/> is enumerated only once.
		/// </para>
		/// </remarks>
		public void AddBatches (IEnumerable<string> batches, DatabaseProcessingStepTransactionRequirement transactionRequirement)
		{
			if (batches == null)
			{
				return;
			}

			List<string> batchList = new List<string>(batches);
			batchList.RemoveAll(x => x.IsNullOrEmptyOrWhitespace());

			if (batchList.Count == 0)
			{
				return;
			}

			this.SubSteps.Add(new Tuple<SubStepType, DatabaseProcessingStepTransactionRequirement, object>(SubStepType.Batches, transactionRequirement, batches));
		}

		/// <summary>
		/// Adds application code as a callback.
		/// </summary>
		/// <param name="callback">The callback which is executed when the sub-step executes.</param>
		/// <remarks>
		/// <para>
		/// <see cref="DatabaseProcessingStepTransactionRequirement.DontCare"/> is used as the transaction requirement.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="callback"/> is null.</exception>
		public void AddCode (DatabaseProcessingStepDelegate<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> callback) => this.AddCode(callback, DatabaseProcessingStepTransactionRequirement.DontCare);

		/// <summary>
		/// Adds application code as a callback.
		/// </summary>
		/// <param name="callback">The callback which is executed when the sub-step executes.</param>
		/// <param name="transactionRequirement">The transaction requirement.</param>
		/// <exception cref="ArgumentNullException"><paramref name="callback"/> is null.</exception>
		public void AddCode (DatabaseProcessingStepDelegate<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> callback, DatabaseProcessingStepTransactionRequirement transactionRequirement)
		{
			if (callback == null)
			{
				throw new ArgumentNullException(nameof(callback));
			}

			this.SubSteps.Add(new Tuple<SubStepType, DatabaseProcessingStepTransactionRequirement, object>(SubStepType.CodeTyped, transactionRequirement, callback));
		}

		/// <summary>
		/// Adds application code as a callback.
		/// </summary>
		/// <param name="callback">The callback which is executed when the sub-step executes.</param>
		/// <remarks>
		/// <para>
		/// <see cref="DatabaseProcessingStepTransactionRequirement.DontCare"/> is used as the transaction requirement.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="callback"/> is null.</exception>
		public void AddCode(DatabaseProcessingStepDelegate callback) => this.AddCode(callback, DatabaseProcessingStepTransactionRequirement.DontCare);

		/// <summary>
		/// Adds application code as a callback.
		/// </summary>
		/// <param name="callback">The callback which is executed when the sub-step executes.</param>
		/// <param name="transactionRequirement">The transaction requirement.</param>
		/// <exception cref="ArgumentNullException"><paramref name="callback"/> is null.</exception>
		public void AddCode(DatabaseProcessingStepDelegate callback, DatabaseProcessingStepTransactionRequirement transactionRequirement)
		{
			if (callback == null)
			{
				throw new ArgumentNullException(nameof(callback));
			}

			this.SubSteps.Add(new Tuple<SubStepType, DatabaseProcessingStepTransactionRequirement, object>(SubStepType.CodeGeneric, transactionRequirement, callback));
		}

		private enum SubStepType
		{
			Script,

			Batch,

			Batches,

			CodeTyped,

			CodeGeneric,
		}
	}
}
