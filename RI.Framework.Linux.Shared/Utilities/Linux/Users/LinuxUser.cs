using Mono.Unix.Native;




namespace RI.Framework.Utilities.Linux.Users
{
	/// <summary>
	///     Provides utilities for working with Linux user contexts.
	/// </summary>
	public static class LinuxUser
	{
		#region Static Methods

		/// <summary>
		///     Determines whether the current user is root.
		/// </summary>
		/// <returns>
		///     true if the current user is root, false otherwise.
		/// </returns>
		public static bool IsCurrentRoot () => Syscall.getuid() == 0;

		#endregion
	}
}
