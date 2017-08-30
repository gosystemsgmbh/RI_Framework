using System;
using System.Collections.Generic;
using System.Linq;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database.Scripts
{
	/// <summary>
	/// Implements a base class for database script locators.
	/// </summary>
	/// <remarks>
	/// <para>
	/// It is recommended that database script locator implementations use this base class as it already implements most of the logic which is common to script locators.
	/// </para>
	/// <para>
	/// See <see cref="IDatabaseScriptLocator"/> for more details.
	/// </para>
	/// <para>
	/// The default implementation does the following preprocessing:
	/// Trim all batches, remove empty batches, replace placheholder values.
	/// </para>
	/// </remarks>
	public abstract class DatabaseScriptLocator : IDatabaseScriptLocator
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
		///     The default batch separator.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The default value is <c>GO</c>.
		///     </para>
		/// </remarks>
		public const string DefaultBatchSeparator = "\r\nGO\r\n";

		/// <summary>
		/// Gets the used string comparer used to compare placeholder names for equality.
		/// </summary>
		/// <value>
		/// The used string comparer used to compare placeholder names for equality.
		/// </value>
		public static readonly StringComparerEx PlaceholderNameComparer = StringComparerEx.OrdinalIgnoreCase;


		/// <summary>
		/// Implements default splitting of a script into individual batches.
		/// </summary>
		/// <param name="script">The script to split into individual batches.</param>
		/// <param name="separator">The used batch separator.</param>
		/// <returns>
		/// The list of batches.
		/// </returns>
		public static List<string> SplitBatches (string script, string separator)
		{
			//TODO: Normalize new-lines
			string[] pieces = script.Split(StringSplitOptions.None, separator);
			List<string> batches = new List<string>(pieces);
			batches.RemoveAll(x => x.IsNullOrEmptyOrWhitespace());
			return batches;
		}




		/// <summary>
		/// Creates a new instance of <see cref="DatabaseScriptLocator"/>.
		/// </summary>
		protected DatabaseScriptLocator ()
		{
			this.BatchSeparator = DatabaseScriptLocator.DefaultBatchSeparator;
			this.Placeholders = new Dictionary<string, Func<string, string>>(DatabaseScriptLocator.PlaceholderNameComparer);
		}

		private Dictionary<string, Func<string,string>> Placeholders { get; set; }

		/// <summary>
		/// Sets the resolver for a specified placeholder name.
		/// </summary>
		/// <param name="name">The name of the placeholder.</param>
		/// <param name="resolver">The resolver which is used to resolve the value for the placeholder or null to remove the placeholder.</param>
		/// <remarks>
		/// <para>
		/// If the placeholder resolver is already set, the existing resolver is replaced by <paramref name="resolver"/>.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
		/// <exception cref="EmptyStringArgumentException"><paramref name="name"/> is an empty string.</exception>
		public void SetPlaceholderResolver (string name, Func<string, string> resolver)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			this.Placeholders.Remove(name);

			if (resolver == null)
			{
				return;
			}

			this.Placeholders.Add(name, resolver);
		}

		/// <summary>
		/// Gets the value for a placeholder.
		/// </summary>
		/// <param name="name">The name of the placeholder.</param>
		/// <returns>
		/// The value of the placeholder or null if the value is not found or the placeholder is not defined.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The default implementation uses the resolvers set through <see cref="SetPlaceholderResolver"/>.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
		/// <exception cref="EmptyStringArgumentException"><paramref name="name"/> is an empty string.</exception>
		public virtual string GetPlaceholderValue (string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			if (!this.Placeholders.ContainsKey(name))
			{
				return null;
			}

			return this.Placeholders[name](name);
		}

		/// <inheritdoc />
		public string BatchSeparator { get; set; }




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

			string script = this.LocateAndReadScript(manager, name);
			if (script == null)
			{
				this.Log(LogLevel.Warning, "Script not found: {0}" + name);
				return null;
			}

			List<string> batches;
			if (this.BatchSeparator == null)
			{
				batches = new List<string>();
				batches.Add(script);
			}
			else
			{
				batches = this.SplitBatches(manager, script, this.BatchSeparator);
				if (batches == null)
				{
					this.Log(LogLevel.Warning, "Script is invalid (splitting batches failed): {0}" + name);
					return null;
				}
			}

			if (preprocess)
			{
				batches = new List<string>(batches.Where(x => !x.IsNullOrEmptyOrWhitespace()).Select(x => x.Trim()).Where(x => !x.IsNullOrEmptyOrWhitespace()));

				for (int i1 = 0; i1 < batches.Count; i1++)
				{
					string replaced = this.ReplacePlaceholders(manager, batches[i1], this.Placeholders.Keys.ToList());
					if (replaced == null)
					{
						this.Log(LogLevel.Warning, "Script is invalid (replacing placeholders failed): {0}" + name);
						return null;
					}
					batches[i1] = replaced;
				}

				this.AdditionalPreprocessing(batches);

				batches = new List<string>(batches.Where(x => !x.IsNullOrEmptyOrWhitespace()).Select(x => x.Trim()).Where(x => !x.IsNullOrEmptyOrWhitespace()));
			}

			return batches;
		}

		/// <summary>
		/// Called to locate and read the script with a specified name into a string.
		/// </summary>
		/// <param name="manager">The used database manager representing the database.</param>
		/// <param name="name">The name of the script.</param>
		/// <returns>
		/// The script or null if the script could not be found.
		/// </returns>
		protected abstract string LocateAndReadScript (IDatabaseManager manager, string name);

		/// <summary>
		/// Splits a script into individual batches.
		/// </summary>
		/// <param name="manager">The used database manager representing the database.</param>
		/// <param name="script">The script to split into individual batches.</param>
		/// <param name="separator">The used batch separator, same value as <see cref="BatchSeparator"/>.</param>
		/// <returns>
		/// The list of batches or null if the splitting failed.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The default implementation uses <see cref="SplitBatches(string,string)"/>.
		/// </para>
		/// <note type="note">
		/// <see cref="SplitBatches(IDatabaseManager,string,string)"/> is not called if <see cref="BatchSeparator"/> is null.
		/// </note>
		/// </remarks>
		protected virtual List<string> SplitBatches (IDatabaseManager manager, string script, string separator)
		{
			return DatabaseScriptLocator.SplitBatches(script, separator);
		}

		/// <summary>
		/// Replaces placeholders in a batch.
		/// </summary>
		/// <param name="manager">The used database manager representing the database.</param>
		/// <param name="batch">The current batch in which the placeholders are to be replaced.</param>
		/// <param name="placeholders">The list of defined placeholder names as set by <see cref="SetPlaceholderResolver"/>.</param>
		/// <returns>
		/// The batch with its placeholders replaced or null if the placeholder replacing failed.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The default implementation uses <see cref="GetPlaceholderValue"/> to retrieve the values and then performs a simple search-and-replace on <paramref name="batch"/>.
		/// </para>
		/// <note type="note">
		/// <see cref="ReplacePlaceholders"/> is separately called for each individual batch.
		/// </note>
		/// </remarks>
		protected virtual string ReplacePlaceholders (IDatabaseManager manager, string batch, List<string> placeholders)
		{
			string processed = batch;

			foreach (string placeholder in placeholders)
			{
				string value = this.GetPlaceholderValue(placeholder);
				if (value != null)
				{
					processed = processed.Replace(placeholder, value);
				}
			}

			return processed;
		}

		/// <summary>
		/// Performs additional batch preprocessing, as required by the current <see cref="IDatabaseScriptLocator"/> implementation.
		/// </summary>
		/// <param name="batches">The batches to preprocess.</param>
		/// <remarks>
		/// <para>
		/// The default implementation does nothing.
		/// </para>
		/// </remarks>
		protected virtual void AdditionalPreprocessing (List<string> batches)
		{
		}
	}
}
