using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using RI.Framework.Data.Database.Backup;
using RI.Framework.Data.Database.Cleanup;
using RI.Framework.Data.Database.Upgrading;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database
{
	/// <summary>
	/// Implements a single SQLite database processing step.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="SQLiteDatabaseProcessingStep"/> is used by <see cref="SQLiteDatabaseBackupCreator"/>, <see cref="SQLiteDatabaseCleanupProcessor"/>, and <see cref="SQLiteDatabaseVersionUpgrader"/>.
	/// </para>
	/// <para>
	/// By adding sub-steps (<see cref="AddScript(string,SQLiteDatabaseProcessingStepTransactionRequirement)"/>, <see cref="AddBatch(string,SQLiteDatabaseProcessingStepTransactionRequirement)"/>, <see cref="AddBatches(IEnumerable{string},SQLiteDatabaseProcessingStepTransactionRequirement)"/>, <see cref="AddCode(SQLiteDatabaseProcessingStepDelegate,SQLiteDatabaseProcessingStepTransactionRequirement)"/>), a processing step can be heavily customized by using both application code through delegates and SQL scripts.
	/// </para>
	/// <para>
	/// The sub-steps are executed in the order they are added.
	/// </para>
	/// </remarks>
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public class SQLiteDatabaseProcessingStep : ILogSource
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
		/// Creates a new instance of <see cref="SQLiteDatabaseProcessingStep"/>
		/// </summary>
		public SQLiteDatabaseProcessingStep ()
		{
			this.SubSteps = new List<Tuple<SubStepType, SQLiteDatabaseProcessingStepTransactionRequirement, object>>();
		}

		private List<Tuple<SubStepType, SQLiteDatabaseProcessingStepTransactionRequirement, object>> SubSteps { get; }

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
		public bool RequiresTransaction => this.SubSteps.Count == 0 ? false : this.SubSteps.Any(x => x.Item2 == SQLiteDatabaseProcessingStepTransactionRequirement.Required);

		internal void Execute (SQLiteDatabaseManager manager, SQLiteConnection connection, SQLiteTransaction transaction)
		{
			((ILogSource)this).LoggingEnabled = ((ILogSource)manager).LoggingEnabled;
			((ILogSource)this).Logger = ((ILogSource)manager).Logger;

			if (this.SubSteps.Count == 0)
			{
				return;
			}

			if (this.SubSteps.Any(x => x.Item2 == SQLiteDatabaseProcessingStepTransactionRequirement.Required) && this.SubSteps.Any(x => x.Item2 == SQLiteDatabaseProcessingStepTransactionRequirement.Disallowed))
			{
				throw new Exception("Conflicting transaction requirements.");
			}

			List<object> subSteps = new List<object>();
			foreach (Tuple<SubStepType, SQLiteDatabaseProcessingStepTransactionRequirement, object> subStep in this.SubSteps)
			{
				List<string> batches = null;
				SQLiteDatabaseProcessingStepDelegate callback = null;

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

						case SubStepType.Code:
							callback = (SQLiteDatabaseProcessingStepDelegate)subStep.Item3;
							break;
				}

				if (batches != null)
				{
					subSteps.Add(batches);
				}

				if (callback != null)
				{
					subSteps.Add(callback);
				}
			}

			foreach (object subStep in subSteps)
			{
				List<string> batches = subStep as List<string>;
				SQLiteDatabaseProcessingStepDelegate callback = subStep as SQLiteDatabaseProcessingStepDelegate;

				if (batches != null)
				{
					foreach (string batch in batches)
					{
						this.Log(LogLevel.Debug, "Executing SQLite database processing command:{0}{1}", Environment.NewLine, batch);
						using (SQLiteCommand command = transaction == null ? new SQLiteCommand(batch, connection) : new SQLiteCommand(batch, connection, transaction))
						{
							command.ExecuteNonQuery();
						}
					}
				}

				if (callback != null)
				{
					this.Log(LogLevel.Debug, "Executing SQLite database processing callback: {0}.{1}", callback.Method?.DeclaringType?.FullName ?? "[null]", callback.Method?.Name ?? "[null]");
					callback(this, manager, connection, transaction);
				}
			}
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
		/// <see cref="SQLiteDatabaseProcessingStepTransactionRequirement.DontCare"/> is used as the transaction requirement.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="scriptName"/> is null.</exception>
		/// <exception cref="EmptyStringArgumentException"><paramref name="scriptName"/> is an empty string.</exception>
		public void AddScript (string scriptName) => this.AddScript(scriptName, SQLiteDatabaseProcessingStepTransactionRequirement.DontCare);

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
		public void AddScript (string scriptName, SQLiteDatabaseProcessingStepTransactionRequirement transactionRequirement)
		{
			if (scriptName == null)
			{
				throw new ArgumentNullException(nameof(scriptName));
			}

			if (scriptName.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(scriptName));
			}

			this.SubSteps.Add(new Tuple<SubStepType, SQLiteDatabaseProcessingStepTransactionRequirement, object>(SubStepType.Script, transactionRequirement, scriptName));
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
		/// <see cref="SQLiteDatabaseProcessingStepTransactionRequirement.DontCare"/> is used as the transaction requirement.
		/// </para>
		/// <para>
		/// If <paramref name="batch"/> is null or empty, no sub-step is added.
		/// </para>
		/// </remarks>
		public void AddBatch (string batch) => this.AddBatch(batch, SQLiteDatabaseProcessingStepTransactionRequirement.DontCare);

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
		public void AddBatch (string batch, SQLiteDatabaseProcessingStepTransactionRequirement transactionRequirement)
		{
			if (batch == null)
			{
				return;
			}

			if (batch.IsNullOrEmptyOrWhitespace())
			{
				return;
			}

			this.SubSteps.Add(new Tuple<SubStepType, SQLiteDatabaseProcessingStepTransactionRequirement, object>(SubStepType.Batch, transactionRequirement, batch));
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
		/// <see cref="SQLiteDatabaseProcessingStepTransactionRequirement.DontCare"/> is used as the transaction requirement.
		/// </para>
		/// <para>
		/// If <paramref name="batches"/> is null or empty, no sub-step is added.
		/// </para>
		/// <para>
		/// <paramref name="batches"/> is enumerated only once.
		/// </para>
		/// </remarks>
		public void AddBatches (IEnumerable<string> batches) => this.AddBatches(batches, SQLiteDatabaseProcessingStepTransactionRequirement.DontCare);

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
		public void AddBatches (IEnumerable<string> batches, SQLiteDatabaseProcessingStepTransactionRequirement transactionRequirement)
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

			this.SubSteps.Add(new Tuple<SubStepType, SQLiteDatabaseProcessingStepTransactionRequirement, object>(SubStepType.Batches, transactionRequirement, batches));
		}

		/// <summary>
		/// Adds application code as a callback.
		/// </summary>
		/// <param name="callback">The callback which is executed when the sub-step executes.</param>
		/// <remarks>
		/// <para>
		/// <see cref="SQLiteDatabaseProcessingStepTransactionRequirement.DontCare"/> is used as the transaction requirement.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="callback"/> is null.</exception>
		public void AddCode (SQLiteDatabaseProcessingStepDelegate callback) => this.AddCode(callback, SQLiteDatabaseProcessingStepTransactionRequirement.DontCare);

		/// <summary>
		/// Adds application code as a callback.
		/// </summary>
		/// <param name="callback">The callback which is executed when the sub-step executes.</param>
		/// <param name="transactionRequirement">The transaction requirement.</param>
		/// <exception cref="ArgumentNullException"><paramref name="callback"/> is null.</exception>
		public void AddCode (SQLiteDatabaseProcessingStepDelegate callback, SQLiteDatabaseProcessingStepTransactionRequirement transactionRequirement)
		{
			if (callback == null)
			{
				throw new ArgumentNullException(nameof(callback));
			}

			this.SubSteps.Add(new Tuple<SubStepType, SQLiteDatabaseProcessingStepTransactionRequirement, object>(SubStepType.Code, transactionRequirement, callback));
		}

		private enum SubStepType
		{
			Script,

			Batch,

			Batches,

			Code,
		}
	}
}
