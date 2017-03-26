using System;
using System.Collections.Generic;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Text;




namespace RI.Framework.Services.Settings.Storages
{
	/// <summary>
	///     Implements a setting storage which reads from the command line.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This setting store is read-only.
	///     </para>
	///     <para>
	///         This setting store internally uses <see cref="RI.Framework.Utilities.Text.CommandLine.Parse(string,bool,IEqualityComparer{string})" /> with <see cref="Environment" />.<see cref="Environment.CommandLine" /> to read the current processes command line.
	///         Only parameters, but not literals or the executable, is used for retriving values.
	///     </para>
	///     <para>
	///         If a parameter occurs multiple times in the command line, only the first occurence is used.
	///     </para>
	///     <para>
	///         See <see cref="ISettingStorage" /> for more details.
	///     </para>
	/// </remarks>
	[Export]
	public sealed class CommandLineSettingStorage : ISettingStorage
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="RI.Framework.Services.Settings.Storages.EnvironmentVariableSettingStorage" />.
		/// </summary>
		/// <remarks>
		///     <para>
		///         No prefix will be used.
		///     </para>
		/// </remarks>
		public CommandLineSettingStorage ()
		{
			this.CommandLine = CommandLine.Parse(Environment.CommandLine, true, StringComparerEx.InvariantCultureIgnoreCase);
		}

		#endregion




		#region Instance Properties/Indexer

		private CommandLine CommandLine { get; set; }

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

			if (!this.CommandLine.Parameters.ContainsKey(name))
			{
				return null;
			}

			if (this.CommandLine.Parameters[name].Count == 0)
			{
				return null;
			}

			return this.CommandLine.Parameters[name][0];
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

			if (!this.CommandLine.Parameters.ContainsKey(name))
			{
				return false;
			}

			if (this.CommandLine.Parameters[name].Count == 0)
			{
				return false;
			}

			return true;
		}

		/// <inheritdoc />
		public void Load ()
		{
		}

		/// <inheritdoc />
		public void Save ()
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		public void SetValue (string name, string value)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}
