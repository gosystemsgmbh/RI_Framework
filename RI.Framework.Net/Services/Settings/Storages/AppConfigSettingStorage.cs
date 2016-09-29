using System;
using System.Configuration;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Settings.Storages
{
	/// <summary>
	///     Implements a setting storage which reads from the applications default app.config file.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This setting store is read-only.
	///     </para>
	///     <para>
	///         This setting store internally uses <see cref="ConfigurationManager" />.<see cref="ConfigurationManager.AppSettings" /> to read the applications default configuration, which is usually the app.config file.
	///     </para>
	/// </remarks>
	[Export]
	public sealed class AppConfigSettingStorage : ISettingStorage
	{
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

			if (name.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			return ConfigurationManager.AppSettings[name];
		}

		/// <inheritdoc />
		public bool HasValue (string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			return ConfigurationManager.AppSettings[name] != null;
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
