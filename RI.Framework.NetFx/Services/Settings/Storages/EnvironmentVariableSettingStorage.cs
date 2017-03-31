using System;

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
		public bool IsReadOnly => true;

		/// <inheritdoc />
		public string GetValue (string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			return Environment.GetEnvironmentVariable((this.Prefix ?? string.Empty) + name);
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
		public void SetValue (string name, string value)
		{
			throw new NotSupportedException("Setting a value to environment variables is not supported.");
		}

		#endregion
	}
}
