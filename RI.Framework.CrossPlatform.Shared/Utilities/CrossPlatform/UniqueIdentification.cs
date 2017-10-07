using System;

using RI.Framework.Utilities.Runtime;




namespace RI.Framework.Utilities.CrossPlatform
{
	/// <summary>
	///     Provides functionality to obtain various unique IDs.
	/// </summary>
	public static class UniqueIdentification
	{
		#region Static Methods

		/// <summary>
		///     Gets an anonymous ID identifying the current network domain.
		/// </summary>
		/// <returns>
		///     The ID as a GUID.
		/// </returns>
		public static Guid GetDomainId ()
		{
			return RuntimeEnvironment.IsUnixPlatform() ? Linux.Cryptography.UniqueIdentification.GetDomainId() : Windows.Cryptography.UniqueIdentification.GetDomainId();
		}

		/// <summary>
		///     Gets an anonymous ID identifying the current machine.
		/// </summary>
		/// <returns>
		///     The ID as a GUID.
		/// </returns>
		public static Guid GetMachineId ()
		{
			return RuntimeEnvironment.IsUnixPlatform() ? Linux.Cryptography.UniqueIdentification.GetMachineId() : Windows.Cryptography.UniqueIdentification.GetMachineId();
		}

		/// <summary>
		///     Gets an anonymous ID identifying the current user.
		/// </summary>
		/// <returns>
		///     The ID as a GUID.
		/// </returns>
		public static Guid GetUserId ()
		{
			return RuntimeEnvironment.IsUnixPlatform() ? Linux.Cryptography.UniqueIdentification.GetUserId() : Windows.Cryptography.UniqueIdentification.GetUserId();
		}

		#endregion
	}
}
