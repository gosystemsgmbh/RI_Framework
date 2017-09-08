using System;
using System.Collections;
using System.Collections.Generic;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Settings.Storages
{
	/// <summary>
	///     Implements a setting storage which reads from environment variables.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This setting store is read-only.
	///     </para>
	///     <para>
	///         This setting store internally uses <see cref="Environment" />.<see cref="Environment.GetEnvironmentVariable(string)" /> to read from the current processes environment variables.
	///     </para>
	///     <para>
	///         Because environment variables can be global and their names might be ambiguous, a prefix can be specified which is then always appended in front of any name when searching for environment variables.
	///     </para>
	///     <para>
	///         See <see cref="ISettingStorage" /> for more details.
	///     </para>
	/// <note type="important">
	/// <see cref="EnvironmentVariableSettingStorage"/> does not support multiple values for the same setting!
	/// </note>
	/// </remarks>
	[Export]
	public sealed class EnvironmentVariableSettingStorage : ISettingStorage
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="EnvironmentVariableSettingStorage" />.
		/// </summary>
		/// <remarks>
		///     <para>
		///         No prefix will be used.
		///     </para>
		/// </remarks>
		public EnvironmentVariableSettingStorage ()
			: this(null)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="EnvironmentVariableSettingStorage" />.
		/// </summary>
		/// <param name="prefix"> The prefix to be used. </param>
		public EnvironmentVariableSettingStorage (string prefix)
		{
			this.Prefix = prefix ?? string.Empty;
			this.Prefix = this.Prefix.IsEmptyOrWhitespace() ? null : this.Prefix.Trim();
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used prefix.
		/// </summary>
		/// <value>
		///     The used prefix or null if no prefix is used.
		/// </value>
		public string Prefix { get; private set; }

		#endregion




		#region Interface: ISettingStorage

		/// <inheritdoc />
		bool ISettingStorage.IsReadOnly => true;

		/// <inheritdoc />
		bool ISettingStorage.WriteOnlyKnown => false;

		/// <inheritdoc />
		string ISettingStorage.WritePrefixAffinity => null;

		/// <inheritdoc />
		public bool HasValue (Predicate<string> predicate)
		{
			IDictionary variables = Environment.GetEnvironmentVariables();
			foreach (DictionaryEntry entry in variables)
			{
				string name = (string)entry.Key;

				if (!this.Prefix.IsNullOrEmpty())
				{
					if (name.StartsWith(this.Prefix, StringComparison.InvariantCultureIgnoreCase))
					{
						name = name.Substring(this.Prefix.Length);
					}
				}

				if (predicate(name))
				{
					return true;
				}
			}

			return false;
		}

		/// <inheritdoc />
		public void DeleteValues(string name)
		{
			throw new NotSupportedException("Deleting a value from environment variables is not supported.");
		}

		/// <inheritdoc />
		public void DeleteValues(Predicate<string> predicate)
		{
			throw new NotSupportedException("Deleting a value from environment variables is not supported.");
		}

		/// <inheritdoc />
		public List<string> GetValues (string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			List<string> values = new List<string>();
			string value = Environment.GetEnvironmentVariable((this.Prefix ?? string.Empty) + name);
			if (value != null)
			{
				values.Add(value);
			}

			return values;
		}

		/// <inheritdoc />
		public bool HasValue (string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			return Environment.GetEnvironmentVariable((this.Prefix ?? string.Empty) + name) != null;
		}

		/// <inheritdoc />
		public void Load ()
		{
		}

		/// <inheritdoc />
		public void Save ()
		{
			throw new NotSupportedException("Saving to environment variables is not supported.");
		}

		/// <inheritdoc />
		public void SetValues (string name, IEnumerable<string> values)
		{
			throw new NotSupportedException("Setting a value to environment variables is not supported.");
		}

		#endregion
	}
}
