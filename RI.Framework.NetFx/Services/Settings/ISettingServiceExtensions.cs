using System;
using System.Collections.Generic;
using System.Security.Cryptography;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Cryptography;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Settings
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="ISettingService" /> type.
	/// </summary>
	public static class ISettingServiceExtensions
	{
		#region Static Methods

		/// <summary>
		///     Gets the first protected setting value.
		/// </summary>
		/// <param name="settingService"> The setting service. </param>
		/// <param name="safeName"> The name of the protected values. </param>
		/// <param name="userScope"> Specifies whether the values should be readable by all users on a machine (false) or only by the user protecting them (true). </param>
		/// <returns>
		///     The setting value or null if the setting is not available.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="settingService" /> or <paramref name="safeName" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="safeName" /> is an empty string. </exception>
		/// <exception cref="CryptographicException"> The value could not be decrypted. This may be cause when trying to decrypt a value which belongs to another user. </exception>
		public static string GetProtectedValue (this ISettingService settingService, string safeName, bool userScope)
		{
			if (settingService == null)
			{
				throw new ArgumentNullException(nameof(settingService));
			}

			if (safeName == null)
			{
				throw new ArgumentNullException(nameof(safeName));
			}

			if (safeName.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(safeName));
			}

			string safeValue = settingService.GetValue<string>(safeName);
			if (safeValue == null)
			{
				return null;
			}

			string unsafeValue = LocalEncryption.Decrypt(userScope, safeValue, safeName);
			return unsafeValue;
		}

		/// <summary>
		///     Gets all protected setting values.
		/// </summary>
		/// <param name="settingService"> The setting service. </param>
		/// <param name="safeName"> The name of the protected values. </param>
		/// <param name="userScope"> Specifies whether the values should be readable by all users on a machine (false) or only by the user protecting them (true). </param>
		/// <returns>
		///     The setting values or an empty list if the setting is not available.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="settingService" /> or <paramref name="safeName" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="safeName" /> is an empty string. </exception>
		/// <exception cref="CryptographicException"> The value could not be decrypted. This may be cause when trying to decrypt a value which belongs to another user. </exception>
		public static List<string> GetProtectedValues (this ISettingService settingService, string safeName, bool userScope)
		{
			if (settingService == null)
			{
				throw new ArgumentNullException(nameof(settingService));
			}

			if (safeName == null)
			{
				throw new ArgumentNullException(nameof(safeName));
			}

			if (safeName.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(safeName));
			}

			List<string> safeValues = settingService.GetValues<string>(safeName);

			List<string> unsafeValues = (from x in safeValues select LocalEncryption.Decrypt(userScope, x, safeName)).ToList();

			return unsafeValues;
		}

		/// <summary>
		///     Protects values by encrypting them and storing under a separate name, deleting the unprotected values.
		/// </summary>
		/// <param name="settingService"> The setting service. </param>
		/// <param name="safeName"> The name of the protected values. </param>
		/// <param name="unsafeName"> The name of the unprotected values. </param>
		/// <param name="userScope"> Specifies whether the values should be readable by all users on a machine (false) or only by the user protecting them (true). </param>
		/// <exception cref="ArgumentNullException"> <paramref name="settingService" />, <paramref name="safeName" />, or <paramref name="unsafeName" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="safeName" /> or <paramref name="unsafeName" /> is an empty string. </exception>
		public static void ProtectValues (this ISettingService settingService, string safeName, string unsafeName, bool userScope)
		{
			if (settingService == null)
			{
				throw new ArgumentNullException(nameof(settingService));
			}

			if (safeName == null)
			{
				throw new ArgumentNullException(nameof(safeName));
			}

			if (safeName.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(safeName));
			}

			if (unsafeName == null)
			{
				throw new ArgumentNullException(nameof(unsafeName));
			}

			if (unsafeName.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(unsafeName));
			}

			List<string> unsafeValues = settingService.GetValues<string>(unsafeName);
			List<string> safeValues = settingService.GetValues<string>(safeName);

			safeValues.AddRange(from x in unsafeValues select LocalEncryption.Encrypt(userScope, x, safeName));

			settingService.SetValues(safeName, safeValues);
			settingService.DeleteValues(unsafeName);

			unsafeValues.Clear();
			safeValues.Clear();
		}

		#endregion
	}
}
