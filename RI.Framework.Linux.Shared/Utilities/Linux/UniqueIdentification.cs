using System;
using System.Security.Cryptography;
using System.Text;

namespace RI.Framework.Utilities.Linux
{
	/// <summary>
	///     Provides functionality to obtain various unique IDs.
	/// </summary>
	public static class UniqueIdentification
	{
		#region Constants

		private const string InnerGuid = "D2810CA2E2B74A1CB859644FD5BE32C2";

		#endregion




		#region Static Methods

		/// <summary>
		///     Gets an anonymous ID identifying the current network domain.
		/// </summary>
		/// <returns>
		///     The ID as a GUID.
		/// </returns>
		/// <exception cref="NotSupportedException"><see cref="GetDomainId"/> is not yet implemented for Linux.</exception>
		public static Guid GetDomainId()
		{
			throw new NotSupportedException(nameof(UniqueIdentification.GetDomainId) + " is not yet implemented for Linux.");
		}

		/// <summary>
		///     Gets an anonymous ID identifying the current machine.
		/// </summary>
		/// <returns>
		///     The ID as a GUID.
		/// </returns>
		public static Guid GetMachineId()
		{
			string cipher = LocalEncryption.Encrypt(false, UniqueIdentification.InnerGuid, null);
			Guid guid = UniqueIdentification.CreateGuidFromString(cipher);
			return guid;
		}

		/// <summary>
		///     Gets an anonymous ID identifying the current user.
		/// </summary>
		/// <returns>
		///     The ID as a GUID.
		/// </returns>
		public static Guid GetUserId()
		{
			string cipher = LocalEncryption.Encrypt(true, UniqueIdentification.InnerGuid, null);
			Guid guid = UniqueIdentification.CreateGuidFromString(cipher);
			return guid;
		}

		private static Guid CreateGuidFromString(string data)
		{
			byte[] guidBytes;
			using (MD5 hasher = MD5.Create())
			{
				guidBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(data));
			}
			Guid guid = new Guid(guidBytes);
			return guid;
		}

		#endregion
	}
}
