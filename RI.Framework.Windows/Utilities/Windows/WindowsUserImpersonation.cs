using System;
using System.ComponentModel;
using System.Security;
using System.Security.Principal;

using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Utilities.Windows
{
	/// <summary>
	/// Implements impersonation of a Windows user.
	/// </summary>
	public sealed class WindowsUserImpersonation : IDisposable
	{
		/// <summary>
		/// Creates a new instance of <see cref="WindowsUserImpersonation"/>.
		/// </summary>
		/// <param name="user">The logon (user or domain\user).</param>
		/// <param name="password">The optional password (null or empty string if not used).</param>
		/// <param name="loadUserProfile">Specifies whether the users profile is to be loaded.</param>
		/// <remarks>
		/// <para>
		/// If <paramref name="user"/> does not include a domain, the local machine is used.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="user"/> is null.</exception>
		/// <exception cref="EmptyStringArgumentException"><paramref name="user"/> is an empty string.</exception>
		/// <exception cref="SecurityException">The current user does not have sufficient permissions.</exception>
		/// <exception cref="Win32Exception">The current user does not have sufficient permissions or the impersonation could not be completed.</exception>
		public WindowsUserImpersonation (string user, string password, bool loadUserProfile)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			if (user.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(user));
			}

			string resolvedDomain;
			string resolvedUser;

			WindowsUser.ExtractDomainAndUser(user, true, out resolvedDomain, out resolvedUser);

			this.Domain = resolvedDomain;
			this.User = resolvedUser;
			this.Password = password;
			this.LoadUserProfile = loadUserProfile;

			this.Impersonate();
		}

		/// <summary>
		/// Creates a new instance of <see cref="WindowsUserImpersonation"/>.
		/// </summary>
		/// <param name="domain">The logon domain.</param>
		/// <param name="user">The logon user name.</param>
		/// <param name="password">The optional password (null or empty string if not used).</param>
		/// <param name="loadUserProfile">Specifies whether the users profile is to be loaded.</param>
		/// <remarks>
		/// <para>
		/// If <paramref name="domain"/> is null or an empty string, the local machine is used.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="user"/> is null.</exception>
		/// <exception cref="EmptyStringArgumentException"><paramref name="user"/> is an empty string.</exception>
		/// <exception cref="SecurityException">The current user does not have sufficient permissions.</exception>
		/// <exception cref="Win32Exception">The current user does not have sufficient permissions or the impersonation could not be completed.</exception>
		public WindowsUserImpersonation(string domain, string user, string password, bool loadUserProfile)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			if (user.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(user));
			}

			this.Domain = WindowsUser.ResolveDomain(domain ?? string.Empty);
			this.User = WindowsUser.ResolveUser(user);
			this.Password = password;
			this.LoadUserProfile = loadUserProfile;

			this.Impersonate();
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="WindowsUserImpersonation" />.
		/// </summary>
		~WindowsUserImpersonation ()
		{
			this.Dispose();
		}

		private void Impersonate ()
		{
			this.Token = IntPtr.Zero;
			this.Identity = null;
			this.Context = null;
			this.Profile = null;

			IntPtr token = IntPtr.Zero;
			WindowsIdentity identity = null;
			WindowsImpersonationContext context = null;
			WindowsUserProfile profile = null;

			bool success = false;
			try
			{
				WindowsUser.CreateLogonToken(this.Domain, this.User, this.Password, out token);
				WindowsUser.Impersonate(this.LoadUserProfile, token, out identity, out context, out profile);
				success = true;
			}
			finally
			{
				if (!success)
				{
					if((token != IntPtr.Zero) && (identity != null) && (context != null) && (profile != null))
					{
						WindowsUser.Unimpersonate(token, identity, context, profile);
					}

					if (token != IntPtr.Zero)
					{
						WindowsUser.CloseLogonToken(token);
					}
				}
			}

			this.Token = token;
			this.Identity = identity;
			this.Context = context;
			this.Profile = profile;
		}

		/// <inheritdoc />
		public void Dispose ()
		{
			if ((this.Token != IntPtr.Zero) && (this.Identity != null) && (this.Context != null) && (this.Profile != null))
			{
				WindowsUser.Unimpersonate(this.Token, this.Identity, this.Context, this.Profile);
			}

			if (this.Token != IntPtr.Zero)
			{
				WindowsUser.CloseLogonToken(this.Token);
			}

			this.Token = IntPtr.Zero;
			this.Identity = null;
			this.Context = null;
			this.Profile = null;
		}

		/// <summary>
		/// Gets the used logon domain.
		/// </summary>
		/// <value>
		/// The used logon domain.
		/// </value>
		public string Domain { get; private set; }

		/// <summary>
		/// Gets the used logon user name.
		/// </summary>
		/// <value>
		/// The used logon user name.
		/// </value>
		public string User { get; private set; }

		/// <summary>
		/// Gets the used password.
		/// </summary>
		/// <value>
		/// The used password.
		/// </value>
		public string Password { get; private set; }

		/// <summary>
		/// Gets whether the users profile is loaded.
		/// </summary>
		/// <value>
		/// true if the users profile is loaded, false otherwise.
		/// </value>
		public bool LoadUserProfile { get; private set; }

		/// <summary>
		/// Gets the logon token.
		/// </summary>
		/// <value>
		/// The logon token or <see cref="IntPtr.Zero"/> if this impersonation has been disposed.
		/// </value>
		public IntPtr Token { get; private set; }

		/// <summary>
		/// Gets the impersonated windows identity.
		/// </summary>
		/// <value>
		/// The impersonated windows identity or null if this impersonation has been disposed.
		/// </value>
		public WindowsIdentity Identity { get; private set; }

		/// <summary>
		/// Gets the impersonation context.
		/// </summary>
		/// <value>
		/// The impersonation context or null if this impersonation has been disposed.
		/// </value>
		public WindowsImpersonationContext Context { get; private set; }

		/// <summary>
		/// Gets the user profile.
		/// </summary>
		/// <value>
		/// The user profile or null if this impersonation has been disposed.
		/// </value>
		public WindowsUserProfile Profile { get; private set; }
	}
}