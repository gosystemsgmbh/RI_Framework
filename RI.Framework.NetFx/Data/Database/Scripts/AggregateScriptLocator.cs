using System;
using System.Collections.Generic;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database.Scripts
{
	/// <summary>
	/// Implements a database script locator which combines multiple script locators.
	/// </summary>
	/// <remarks>
	/// <para>
	/// See <see cref="IDatabaseScriptLocator"/> for more details.
	/// </para>
	/// <note type="note">
	/// <see cref="IDatabaseScriptLocator.BatchSeparator"/> is ignored as the values of the individual script locators are used.
	/// </note>
	/// </remarks>
	public sealed class AggregateScriptLocator : IDatabaseScriptLocator
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
		/// Creates a new instance of <see cref="AggregateScriptLocator"/>.
		/// </summary>
		/// <param name="scriptLocators">The sequence of script locators.</param>
		/// <remarks>
		/// <para>
		/// <paramref name="scriptLocators"/> is enumerated only once.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="scriptLocators"/> is null.</exception>
		public AggregateScriptLocator(IEnumerable<IDatabaseScriptLocator> scriptLocators)
		{
			if (scriptLocators == null)
			{
				throw new ArgumentNullException(nameof(scriptLocators));
			}

			this.ScriptLocators = new HashSet<IDatabaseScriptLocator>(scriptLocators);
		}

		/// <summary>
		/// Creates a new instance of <see cref="AggregateScriptLocator"/>.
		/// </summary>
		/// <param name="scriptLocators">The array of script locators.</param>
		/// <exception cref="ArgumentNullException"><paramref name="scriptLocators"/> is null.</exception>
		public AggregateScriptLocator(params IDatabaseScriptLocator[] scriptLocators)
			: this((IEnumerable<IDatabaseScriptLocator>)scriptLocators)
		{
		}

		/// <summary>
		/// Gets the set of used script locators.
		/// </summary>
		/// <value>
		/// The set of used script locators.
		/// </value>
		public HashSet<IDatabaseScriptLocator> ScriptLocators { get; }

		/// <inheritdoc />
		public List<string> GetScriptBatch (IDatabaseManager manager, string name, bool preprocess)
		{
			if (manager == null)
			{
				throw new ArgumentNullException(nameof(manager));
			}

			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			List<string> batches = new List<string>();

			foreach (IDatabaseScriptLocator scriptLocator in this.ScriptLocators)
			{
				scriptLocator.LoggingEnabled = ((ILogSource)this).LoggingEnabled;
				scriptLocator.Logger = ((ILogSource)this).Logger;

				List<string> currentBatches = scriptLocator.GetScriptBatch(manager, name, preprocess);
				if (currentBatches == null)
				{
					return null;
				}

				batches.AddRange(currentBatches);
			}

			return batches;
		}

		/// <inheritdoc />
		string IDatabaseScriptLocator.BatchSeparator { get; set; }
	}
}
