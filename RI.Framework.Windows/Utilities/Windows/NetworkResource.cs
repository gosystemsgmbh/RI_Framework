using System;
using System.ComponentModel;
using System.Runtime.InteropServices;




namespace RI.Framework.Utilities.Windows
{
	/// <summary>
	/// Manages Windows network resources.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="NetworkResource"/> can be used to establish and close connections to Windows network resources (e.g. shared drives such as \\server\share\path).
	/// </para>
	/// </remarks>
	public static class NetworkResource
	{
		#region Constants

		private const int ConnectInteractive = 0x8;

		private const int ConnectPrompt = 0x10;

		private const int ResourcetypeAny = 0x0;

		#endregion




		#region Static Methods

		/// <summary>
		/// Closes an existing connection to a Windows network resource.
		/// </summary>
		/// <param name="resource">The path of the resource (e.g. a UNC path to a file share).</param>
		/// <param name="force">true if the connection is to be forcibly closed, even if the connection is still used.</param>
		/// <remarks>
		/// <para>
		/// No indication (return value or exception) is provided to determine whether closing the connection suceeded.
		/// </para>
		/// <note type="important">
		/// Whether and how the connection is closed depends heavily on the configuration of Windows, the computer, the network, and the remote resource itself.
		/// It cannot be guaranteed that the connection is really being closed.
		/// Therefore, this method should only be seen as a &quot;hint to Windows what to do&quot;.
		/// </note>
		/// </remarks>
		public static void CloseConnection(string resource, bool force)
		{
			NetworkResource.WNetCancelConnection2(resource, 0, force);
		}

		/// <summary>
		/// Establishes a connection to a Windows network resource.
		/// </summary>
		/// <param name="resource">The path of the resource (e.g. a UNC path to a file share).</param>
		/// <param name="username">The logon name under which the connection is to be established (null for the current user).</param>
		/// <param name="password">The password for the logon if <paramref name="username"/> is specified.</param>
		/// <param name="interactive">Specifies whether an interactive logon can be performed if necessary (e.g. the user might be asked to enter credentials).</param>
		/// <returns>
		/// The error which occurred during establishing the network connection.
		/// <see cref="NetworkResourceError.None"/> is returned if the connection was established successfully.
		/// </returns>
		/// <remarks>
		/// <note type="important">
		/// Whether and how the connection is established depends heavily on the configuration of Windows, the computer, the network, and the remote resource itself.
		/// This is even more true for using interactive logon (<paramref name="interactive"/>).
		/// </note>
		/// </remarks>
		/// <exception cref="Win32Exception">An unknown error occurred which could not be translated to <see cref="NetworkResourceError"/>.</exception>
		public static NetworkResourceError OpenConnection(string resource, string username, string password, bool interactive)
		{
			Netresource connection = new Netresource();
			connection.dwType = NetworkResource.ResourcetypeAny;
			connection.LocalName = null;
			connection.RemoteName = resource;
			connection.Provider = null;

			int errorCode = NetworkResource.WNetAddConnection3(IntPtr.Zero, ref connection, password, username, interactive ? (NetworkResource.ConnectInteractive | NetworkResource.ConnectPrompt) : 0);

			NetworkResourceError error = NetworkResourceError.None;

			switch (errorCode)
			{
				default:
					{
						string errorMessage = WindowsApi.GetErrorMessage(errorCode);
						throw new Win32Exception(errorCode, errorMessage);
					}

				case (int)WindowsError.ErrorSuccess:
					{
						error = NetworkResourceError.None;
						break;
					}

				case (int)WindowsError.ErrorBadNetName:
					{
						error = NetworkResourceError.InvalidParameters;
						break;
					}

				case (int)WindowsError.ErrorInvalidPassword:
					{
						error = NetworkResourceError.InvalidPassword;
						break;
					}

				case (int)WindowsError.ErrorCancelled:
					{
						error = NetworkResourceError.CanceledByUser;
						break;
					}

				case (int)WindowsError.ErrorBusy:
					{
						error = NetworkResourceError.Busy;
						break;
					}

				case (int)WindowsError.ErrorNoNetOrBadPath:
					{
						error = NetworkResourceError.Unavailable;
						break;
					}

				case (int)WindowsError.ErrorNoNetwork:
					{
						error = NetworkResourceError.Unavailable;
						break;
					}
			}

			return error;
		}

		[DllImport("mpr.dll", SetLastError = true, EntryPoint = "WNetAddConnection3W", CharSet = CharSet.Unicode)]
		private static extern int WNetAddConnection3(IntPtr hWndOwner, ref Netresource lpNetResource, [MarshalAs(UnmanagedType.LPWStr)] string lpPassword, [MarshalAs(UnmanagedType.LPWStr)] string lpUserName, int dwFlags);

		[DllImport("mpr.dll", SetLastError = false, EntryPoint = "WNetCancelConnection2W", CharSet = CharSet.Unicode)]
		private static extern int WNetCancelConnection2([MarshalAs(UnmanagedType.LPWStr)] string lpName, int dwFlags, [MarshalAs(UnmanagedType.Bool)] bool fForce);

		#endregion




		#region Type: NETRESOURCE

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct Netresource
		{
			public int dwScope;

			public int dwType;

			public int dwDisplayType;

			public int dwUsage;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string LocalName;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string RemoteName;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string Comment;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string Provider;
		}

		#endregion
	}
}