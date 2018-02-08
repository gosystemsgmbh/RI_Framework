using RI.Framework.Linux.Users;
using RI.Framework.Utilities.Runtime;
using RI.Framework.Windows.Users;




namespace RI.Framework.CrossPlatform.Users
{
	/// <summary>
	///     Provides utilities for working with cross-platform user contexts.
	/// </summary>
	public static class CrossPlatformUser
	{
		#region Static Methods

		/// <summary>
		///     Determines whether the current user has elevated permissions.
		/// </summary>
		/// <returns>
		///     true if the current user has elevated permissions, false otherwise.
		/// </returns>
		public static bool IsCurrentElevated ()
		{
			return RuntimeEnvironment.IsUnixPlatform() ? LinuxUser.IsCurrentRoot() : WindowsUser.IsCurrentAdministrator();
		}

		#endregion
	}
}
