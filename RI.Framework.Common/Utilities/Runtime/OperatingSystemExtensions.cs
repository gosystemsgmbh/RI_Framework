using System;

namespace RI.Framework.Utilities
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="OperatingSystem" /> type.
	/// </summary>
	public static class OperatingSystemExtensions
	{
		#region Static Methods

		/// <summary>
		/// Gets the service pack version.
		/// </summary>
		/// <param name="operatingSystem">The operating system information.</param>
		/// <returns>
		/// The service pack version.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="operatingSystem"/> is null.</exception>
		public static Version GetServicePackVersion (this OperatingSystem operatingSystem)
		{
			if (operatingSystem == null)
			{
				throw new ArgumentNullException(nameof(operatingSystem));
			}

			string servicePack = operatingSystem.ServicePack.Trim().RemoveLineBreaks().Replace(",", ".").Keep(x => char.IsDigit(x) || ( x == '.' ));
			Version servicePackVersion = new Version(servicePack);
			return servicePackVersion;
		}

		/// <summary>
		/// Determines whether an operating system is Windows 10 or newer.
		/// </summary>
		/// <param name="operatingSystem">The operating system information.</param>
		/// <returns>
		/// true if the operating system is Windows 10 or newer, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="operatingSystem"/> is null.</exception>
		public static bool IsWindows10OrNewer (this OperatingSystem operatingSystem)
		{
			if (operatingSystem == null)
			{
				throw new ArgumentNullException(nameof(operatingSystem));
			}

			return ( operatingSystem.Platform == PlatformID.Win32NT ) && ( operatingSystem.Version.Major >= 10 );
		}

		/// <summary>
		/// Determines whether an operating system is Windows 8.
		/// </summary>
		/// <param name="operatingSystem">The operating system information.</param>
		/// <returns>
		/// true if the operating system is Windows 8, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="operatingSystem"/> is null.</exception>
		public static bool IsWindows8 (this OperatingSystem operatingSystem)
		{
			if (operatingSystem == null)
			{
				throw new ArgumentNullException(nameof(operatingSystem));
			}

			return operatingSystem.IsWindows8OrNewer() && ( !operatingSystem.IsWindows10OrNewer() );
		}

		/// <summary>
		/// Determines whether an operating system is Windows 8 or newer.
		/// </summary>
		/// <param name="operatingSystem">The operating system information.</param>
		/// <returns>
		/// true if the operating system is Windows 8 or newer, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="operatingSystem"/> is null.</exception>
		public static bool IsWindows8OrNewer (this OperatingSystem operatingSystem)
		{
			if (operatingSystem == null)
			{
				throw new ArgumentNullException(nameof(operatingSystem));
			}

			return ( operatingSystem.Platform == PlatformID.Win32NT ) && ( ( operatingSystem.Version.Major > 6 ) || ( ( operatingSystem.Version.Major == 6 ) && ( operatingSystem.Version.Minor >= 2 ) ) );
		}

		/// <summary>
		/// Determines whether an operating system is Windows 7.
		/// </summary>
		/// <param name="operatingSystem">The operating system information.</param>
		/// <returns>
		/// true if the operating system is Windows 7, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="operatingSystem"/> is null.</exception>
		public static bool IsWindows7 (this OperatingSystem operatingSystem)
		{
			if (operatingSystem == null)
			{
				throw new ArgumentNullException(nameof(operatingSystem));
			}

			return operatingSystem.IsWindows7OrNewer() && ( !operatingSystem.IsWindows8OrNewer() );
		}

		/// <summary>
		/// Determines whether an operating system is Windows 7 or newer.
		/// </summary>
		/// <param name="operatingSystem">The operating system information.</param>
		/// <returns>
		/// true if the operating system is Windows 7 or newer, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="operatingSystem"/> is null.</exception>
		public static bool IsWindows7OrNewer (this OperatingSystem operatingSystem)
		{
			if (operatingSystem == null)
			{
				throw new ArgumentNullException(nameof(operatingSystem));
			}

			return ( operatingSystem.Platform == PlatformID.Win32NT ) && ( ( operatingSystem.Version.Major > 6 ) || ( ( operatingSystem.Version.Major == 6 ) && ( operatingSystem.Version.Minor >= 1 ) ) );
		}

		/// <summary>
		/// Determines whether an operating system is Windows Vista.
		/// </summary>
		/// <param name="operatingSystem">The operating system information.</param>
		/// <returns>
		/// true if the operating system is Windows Vista, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="operatingSystem"/> is null.</exception>
		public static bool IsWindowsVista (this OperatingSystem operatingSystem)
		{
			if (operatingSystem == null)
			{
				throw new ArgumentNullException(nameof(operatingSystem));
			}

			return operatingSystem.IsWindowsVistaOrNewer() && ( !operatingSystem.IsWindows7OrNewer() );
		}

		/// <summary>
		/// Determines whether an operating system is Windows Vista or newer.
		/// </summary>
		/// <param name="operatingSystem">The operating system information.</param>
		/// <returns>
		/// true if the operating system is Windows Vista or newer, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="operatingSystem"/> is null.</exception>
		public static bool IsWindowsVistaOrNewer (this OperatingSystem operatingSystem)
		{
			if (operatingSystem == null)
			{
				throw new ArgumentNullException(nameof(operatingSystem));
			}

			return ( operatingSystem.Platform == PlatformID.Win32NT ) && ( operatingSystem.Version.Major >= 6 );
		}

		/// <summary>
		/// Determines whether an operating system is Windows XP.
		/// </summary>
		/// <param name="operatingSystem">The operating system information.</param>
		/// <returns>
		/// true if the operating system is Windows XP, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="operatingSystem"/> is null.</exception>
		public static bool IsWindowsXp (this OperatingSystem operatingSystem)
		{
			if (operatingSystem == null)
			{
				throw new ArgumentNullException(nameof(operatingSystem));
			}

			return operatingSystem.IsWindowsXpOrNewer() && ( !operatingSystem.IsWindowsVistaOrNewer() );
		}

		/// <summary>
		/// Determines whether an operating system is Windows XP or newer.
		/// </summary>
		/// <param name="operatingSystem">The operating system information.</param>
		/// <returns>
		/// true if the operating system is Windows XP or newer, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="operatingSystem"/> is null.</exception>
		public static bool IsWindowsXpOrNewer (this OperatingSystem operatingSystem)
		{
			if (operatingSystem == null)
			{
				throw new ArgumentNullException(nameof(operatingSystem));
			}

			return  ( operatingSystem.Platform == PlatformID.Win32NT ) && ( ( operatingSystem.Version.Major > 5 ) || ( ( operatingSystem.Version.Major == 5 ) && ( operatingSystem.Version.Minor >= 1 ) ) );
		}

		#endregion
	}
}
