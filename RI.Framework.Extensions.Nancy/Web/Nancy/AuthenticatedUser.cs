using System;
using System.Collections.Generic;

using Nancy;
using Nancy.Security;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;

namespace RI.Framework.Web.Nancy
{
	/// <summary>
	///     Stores the user identity of an authenticated user.
	/// </summary>
	public sealed class AuthenticatedUser : IUserIdentity
	{
		/// <summary>
		///     Creates a new instance of <see cref="AuthenticatedUser" />.
		/// </summary>
		/// <param name="context">The context during which this user identity was created.</param>
		/// <param name="userName">The user name.</param>
		/// <param name="claims">The sequence of user claims (if any, can be null).</param>
		/// <remarks>
		/// <para>
		/// <paramref name="claims"/> is enumerated only once.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="context" /> or <paramref name="userName" /> is null.</exception>
		/// <exception cref="EmptyStringArgumentException"><paramref name="userName" /> is an empty string.</exception>
		public AuthenticatedUser(NancyContext context, string userName, IEnumerable<string> claims)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			if (userName == null)
			{
				throw new ArgumentNullException(nameof(userName));
			}

			if (userName.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(userName));
			}

			this.UserName = userName;
			this.Claims = new List<string>(claims ?? new string[0]);

			this.Identifier = Guid.NewGuid();

			this.LoginTimestampUtc = null;
			this.LastActivityTimestampUtc = null;
			this.LogoutTimestampUtc = null;

			this.HostAddress = context.Request.UserHostAddress;
			this.ProtocolVersion = context.Request.ProtocolVersion;
			this.UserAgent = context.Request.Headers.UserAgent;

			this.Tag = null;
			this.Data = new Dictionary<string, object>(StringComparerEx.OrdinalIgnoreCase);
		}

		/// <summary>
		///     Creates a new instance of <see cref="AuthenticatedUser" />.
		/// </summary>
		/// <param name="context">The context during which this user identity was created.</param>
		/// <param name="userName">The user name.</param>
		/// <param name="claims">The sequence of user claims (if any, can be null).</param>
		/// <exception cref="ArgumentNullException"><paramref name="context" /> or <paramref name="userName" /> is null.</exception>
		/// <exception cref="EmptyStringArgumentException"><paramref name="userName" /> is an empty string.</exception>
		public AuthenticatedUser(NancyContext context, string userName, params string[] claims)
			: this(context, userName, (IEnumerable<string>)claims)
		{
		}

		/// <summary>
		///     Gets the dictionary which can be used to store additional, application-specific data.
		/// </summary>
		/// <value>
		///     The dictionary which can be used to store additional, application-specific data.
		/// </value>
		/// <remarks>
		///     <para>
		///         The dictionary uses <see cref="StringComparerEx.OrdinalIgnoreCase" /> for the keys.
		///     </para>
		/// </remarks>
		public Dictionary<string, object> Data { get; }

		/// <summary>
		///     Gets the GUID currently associated with the user.
		/// </summary>
		/// <value>
		///     The GUID currently associated with the user.
		/// </value>
		public Guid Identifier { get; }

		/// <summary>
		///     Gets or sets the timestamp of the users last activity.
		/// </summary>
		/// <value>
		///     The timestamp of the users last activity or null if the user had not yet any activity.
		/// </value>
		public DateTime? LastActivityTimestampUtc { get; set; }

		/// <summary>
		///     Gets or sets the timestamp of the users login.
		/// </summary>
		/// <value>
		///     The timestamp of the users login or null if the user has not yet been logged in.
		/// </value>
		public DateTime? LoginTimestampUtc { get; set; }

		/// <summary>
		///     Gets or sets the timestamp of the users logout.
		/// </summary>
		/// <value>
		///     The timestamp of the users logout or null if the user has not yet been logged out.
		/// </value>
		public DateTime? LogoutTimestampUtc { get; set; }

		/// <summary>
		///     Gets or sets an object representing additional, application-specific data.
		/// </summary>
		/// <value>
		///     An object representing additional, application-specific data.
		/// </value>
		public object Tag { get; set; }

		/// <summary>
		///     Gets the claims associated with the user.
		/// </summary>
		/// <value>
		///     The claims associated with the user.
		///     An empty sequence is returned if the user has no claims.
		/// </value>
		public IEnumerable<string> Claims { get; }

		/// <summary>
		///     Gets the user name.
		/// </summary>
		/// <value>
		///     The user name.
		/// </value>
		public string UserName { get; }

		/// <summary>
		/// Gets the host address of the users machine.
		/// </summary>
		/// <value>
		/// The host address of the users machine.
		/// </value>
		public string HostAddress { get; }

		/// <summary>
		/// Gets the protocol version used by the user.
		/// </summary>
		/// <value>
		/// The protocol version used by the user.
		/// </value>
		public string ProtocolVersion { get; }

		/// <summary>
		/// Gets the user agent used by the user.
		/// </summary>
		/// <value>
		/// The user agent used by the user.
		/// </value>
		public string UserAgent { get; }

		/// <inheritdoc />
		public override string ToString()
		{
			return "UserName=" + this.UserName + "; Claims=" + (this.Claims.Join(",").ToEmptyIfNullOrEmptyOrWhitespace() ?? "[none]") + "; Identifier=" + this.Identifier.ToString("N").ToUpperInvariant() + "; LastActivity=" + (this.LastActivityTimestampUtc?.ToSortableString() ?? "[null]") + "; Login=" + (this.LoginTimestampUtc?.ToSortableString() ?? "[null]") + "; Logout=" + (this.LogoutTimestampUtc?.ToSortableString() ?? "[null]");
		}
	}
}
