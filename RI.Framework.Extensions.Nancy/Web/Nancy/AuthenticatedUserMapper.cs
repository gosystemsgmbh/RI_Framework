using System;
using System.Collections.Generic;
using System.Linq;

using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;

using RI.Framework.Collections;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Web.Nancy
{
	/// <summary>
	///     Implements an in-memory mapper for user identities.
	/// </summary>
	public sealed class AuthenticatedUserMapper : LogSource, IUserMapper, ISynchronizable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="AuthenticatedUserMapper" />.
		/// </summary>
		/// <param name="idleTimeout"> The user idle timeout after which it is automatically logged out. </param>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="idleTimeout" /> is zero or less. </exception>
		public AuthenticatedUserMapper (TimeSpan idleTimeout)
		{
			if (idleTimeout.TotalMilliseconds <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(idleTimeout));
			}

			this.SyncRoot = new object();

			this.IdleTimeout = idleTimeout;

			this.AuthenticatedUsers = new Dictionary<Guid, AuthenticatedUser>();
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the user idle timeout after which it is automatically logged out.
		/// </summary>
		/// <value>
		///     The user idle timeout.
		/// </value>
		public TimeSpan IdleTimeout { get; }

		private Dictionary<Guid, AuthenticatedUser> AuthenticatedUsers { get; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Performs a cleanup of all authenticated users by removing all users which had no activity within the default idle timeout.
		/// </summary>
		public void Cleanup () => this.Cleanup(this.IdleTimeout);

		/// <summary>
		///     Performs a cleanup of all authenticated users by removing all users which had no activity within a specified idle timeout.
		/// </summary>
		/// <param name="idleTimeout"> The user idle timeout.idle timeout after which it is automatically logged out. </param>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="idleTimeout" /> is zero or less. </exception>
		public void Cleanup (TimeSpan idleTimeout)
		{
			if (idleTimeout.TotalMilliseconds <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(idleTimeout));
			}

			lock (this.SyncRoot)
			{
				DateTime utcNow = DateTime.UtcNow;
				this.AuthenticatedUsers.RemoveWhere(x => x.Value.LastActivityTimestampUtc.HasValue ? utcNow.Subtract(x.Value.LastActivityTimestampUtc.Value) >= idleTimeout : (x.Value.LoginTimestampUtc.HasValue ? utcNow.Subtract(x.Value.LoginTimestampUtc.Value) >= idleTimeout : true)).ForEach(x => this.Log(LogLevel.Information, "Automatically logged-out user {0} with ID {1} after {2} seconds", x.Value.UserName, x.Value.Identifier.ToString("N").ToUpperInvariant(), idleTimeout.TotalSeconds));
			}
		}

		/// <summary>
		///     Gets the authenticated user with the specified GUID.
		/// </summary>
		/// <param name="identifier"> The users GUID. </param>
		/// <returns>
		///     The user or null if there is no authenticated user with the specified GUID
		/// </returns>
		public AuthenticatedUser GetUser (Guid identifier)
		{
			lock (this.SyncRoot)
			{
				this.Cleanup();

				if (this.AuthenticatedUsers.ContainsKey(identifier))
				{
					return this.AuthenticatedUsers[identifier];
				}

				return null;
			}
		}

		/// <inheritdoc cref="IUserMapper.GetUserFromIdentifier" />
		public AuthenticatedUser GetUserFromIdentifier (Guid identifier, NancyContext context)
		{
			lock (this.SyncRoot)
			{
				this.Cleanup();

				if (this.AuthenticatedUsers.ContainsKey(identifier))
				{
					AuthenticatedUser user = this.AuthenticatedUsers[identifier];
					if (string.Equals(user.HostAddress, context.Request.UserHostAddress, StringComparison.OrdinalIgnoreCase))
					{
						user.LastActivityTimestampUtc = DateTime.UtcNow;
						return user;
					}

					this.Logout(user);
				}

				return null;
			}
		}

		/// <summary>
		///     Gets a list of all currently logged-in users.
		/// </summary>
		/// <returns>
		///     The list of all currently logged-in users.
		///     An empty list is returned if no users are currently logged-in.
		/// </returns>
		public List<AuthenticatedUser> GetUsers ()
		{
			lock (this.SyncRoot)
			{
				this.Cleanup();

				return new List<AuthenticatedUser>(this.AuthenticatedUsers.Values);
			}
		}

		/// <summary>
		///     Adds a user as a currently authenticated user.
		/// </summary>
		/// <param name="user"> The user. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="user" /> is null. </exception>
		public void Login (AuthenticatedUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			lock (this.SyncRoot)
			{
				DateTime utcNow = DateTime.UtcNow;
				user.LoginTimestampUtc = utcNow;
				user.LastActivityTimestampUtc = utcNow;
				user.LogoutTimestampUtc = null;
				this.AuthenticatedUsers.Remove(user.Identifier);
				this.AuthenticatedUsers.Add(user.Identifier, user);
			}
		}

		/// <summary>
		///     Removes a user as a currently authenticated user.
		/// </summary>
		/// <param name="user"> The user. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="user" /> is null. </exception>
		public void Logout (AuthenticatedUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			lock (this.SyncRoot)
			{
				user.LogoutTimestampUtc = DateTime.UtcNow;
				this.AuthenticatedUsers.Remove(user.Identifier);
			}
		}

		/// <summary>
		///     Removes a user as a currently authenticated user.
		/// </summary>
		/// <param name="userId"> The users ID. </param>
		public void Logout (Guid userId)
		{
			lock (this.SyncRoot)
			{
				if (this.AuthenticatedUsers.ContainsKey(userId))
				{
					AuthenticatedUser user = this.AuthenticatedUsers[userId];
					this.Logout(user);
				}
			}
		}

		/// <summary>
		///     Removes all currently authenticated users
		/// </summary>
		public void LogoutAll ()
		{
			lock (this.SyncRoot)
			{
				while (this.AuthenticatedUsers.Count > 0)
				{
					this.Logout(this.AuthenticatedUsers.First().Value);
				}
			}
		}

		#endregion




		#region Interface: ISynchronizable

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; }

		#endregion




		#region Interface: IUserMapper

		/// <inheritdoc />
		IUserIdentity IUserMapper.GetUserFromIdentifier (Guid identifier, NancyContext context) => this.GetUserFromIdentifier(identifier, context);

		#endregion
	}
}
