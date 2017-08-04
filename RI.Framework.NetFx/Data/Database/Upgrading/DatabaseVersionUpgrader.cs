using System.Data.Common;

using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database.Upgrading
{
	/// <summary>
	/// Implements a base class for database version upgraders.
	/// </summary>
	/// <typeparam name="TConnection">The database connection type, subclass of <see cref="DbConnection"/>.</typeparam>
	/// <typeparam name="TConnectionStringBuilder">The connection string builder type, subclass of <see cref="DbConnectionStringBuilder"/>.</typeparam>
	/// <typeparam name="TManager">The type of the database manager which is using this version upgrader.</typeparam>
	/// <remarks>
	/// <para>
	/// It is recommended that database version upgrader implementations use this base class as it already implements most of the logic which is database-independent.
	/// </para>
	/// <para>
	/// See <see cref="IDatabaseVersionUpgrader"/> for more details.
	/// </para>
	/// </remarks>
	public abstract class DatabaseVersionUpgrader<TConnection, TConnectionStringBuilder, TManager> : IDatabaseVersionUpgrader<TConnection, TConnectionStringBuilder, TManager>
		where TConnection : DbConnection
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TConnectionStringBuilder, TManager>
	{
		/// <inheritdoc />
		public abstract bool Upgrade (TManager manager, int sourceVersion);

		/// <inheritdoc />
		public abstract bool RequiresScriptLocator { get; }

		/// <inheritdoc />
		public abstract int MinVersion { get; }

		/// <inheritdoc />
		public abstract int MaxVersion { get; }

		/// <inheritdoc />
		bool IDatabaseVersionUpgrader.Upgrade(IDatabaseManager manager, int sourceVersion)
		{
			return this.Upgrade((TManager)manager, sourceVersion);
		}

		private bool _loggingEnabled;

		/// <inheritdoc />
		bool ILogSource.LoggingEnabled
		{
			get
			{
				return this._loggingEnabled;
			}
			set
			{
				this._loggingEnabled = value;
			}
		}

		private ILogger _logger;

		/// <inheritdoc />
		ILogger ILogSource.Logger
		{
			get
			{
				return this._logger;
			}
			set
			{
				this._logger = value;
			}
		}
	}
}
