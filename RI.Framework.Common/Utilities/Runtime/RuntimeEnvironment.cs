using System;




namespace RI.Framework.Utilities.Runtime
{
	/// <summary>
	///     Provides various helper functions for the current runtime environment the process is running in.
	/// </summary>
	public static class RuntimeEnvironment
	{
		#region Static Methods

		/// <summary>
		///     Detects whether the current runtime is the Mono runtime.
		/// </summary>
		/// <returns>
		///     true if the current runtime is the Mono runtime, false otherwise.
		/// </returns>
		public static bool IsMonoRuntime ()
		{
			return Type.GetType("Mono.Runtime") != null;
		}

		/// <summary>
		///     Detects whether the current platform is a Unix-like platform (e.g. Linux).
		/// </summary>
		/// <returns>
		///     true if the current platform is a Unix-like platform, false otherwise.
		/// </returns>
		public static bool IsUnixPlatform ()
		{
			int p = (int)Environment.OSVersion.Platform;
			return (p == 4) || (p == 6) || (p == 128);
		}

		#endregion
	}
}
