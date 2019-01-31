using System;
using System.Runtime.InteropServices;




namespace RI.Framework.Utilities.Runtime
{
    /// <summary>
    ///     Provides various helper functions for the current runtime environment the process is running in.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
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
			return !RuntimeEnvironment.IsUnityRuntime() && (Type.GetType("Mono.Runtime") != null);
		}

		/// <summary>
		///     Detects whether the current runtime is the .NET Core runtime.
		/// </summary>
		/// <returns>
		///     true if the current runtime is the .NET Core runtime, false otherwise.
		/// </returns>
		public static bool IsNetCoreRuntime ()
		{
			return !RuntimeEnvironment.IsMonoRuntime() && !RuntimeEnvironment.IsUnityRuntime() && RuntimeInformation.FrameworkDescription.Contains("Core", StringComparison.InvariantCultureIgnoreCase);
		}

		/// <summary>
		///     Detects whether the current runtime is the .NET Framework runtime.
		/// </summary>
		/// <returns>
		///     true if the current runtime is the .NET Framework runtime, false otherwise.
		/// </returns>
		public static bool IsNetFxRuntime ()
		{
			return !RuntimeEnvironment.IsMonoRuntime() && !RuntimeEnvironment.IsUnityRuntime() && RuntimeInformation.FrameworkDescription.Contains("Framework", StringComparison.InvariantCultureIgnoreCase);
		}

		/// <summary>
		///     Detects whether the current runtime is the Unity runtime.
		/// </summary>
		/// <returns>
		///     true if the current runtime is the Unity runtime, false otherwise.
		/// </returns>
		public static bool IsUnityRuntime ()
		{
			return Type.GetType("UnityEngine.Networking.UnityWebRequest") != null;
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

		/// <summary>
		///     Detects whether the current platform is a Windows platform (Windows XP or newer).
		/// </summary>
		/// <returns>
		///     true if the current platform is Windows XP or newer, false otherwise.
		/// </returns>
		public static bool IsWindowsPlatform ()
		{
			return Environment.OSVersion.IsWindowsXpOrNewer();
		}

		#endregion
	}
}
