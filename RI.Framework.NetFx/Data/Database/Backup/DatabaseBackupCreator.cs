using System.Data.Common;

using RI.Framework.IO.Paths;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database.Backup
{
	/// <summary>
	/// Implements a base class for database backup creators.
	/// </summary>
	/// <typeparam name="TConnection">The database connection type, subclass of <see cref="DbConnection"/>.</typeparam>
	/// <typeparam name="TConnectionStringBuilder">The connection string builder type, subclass of <see cref="DbConnectionStringBuilder"/>.</typeparam>
	/// <typeparam name="TManager">The type of the database manager which is using this backup creator.</typeparam>
	/// <remarks>
	/// <para>
	/// It is recommended that database backup creator implementations use this base class as it already implements most of the logic which is database-independent.
	/// </para>
	/// <para>
	/// See <see cref="IDatabaseBackupCreator"/> for more details.
	/// </para>
	/// </remarks>
	public abstract class DatabaseBackupCreator<TConnection, TConnectionStringBuilder, TManager> : IDatabaseBackupCreator<TConnection, TConnectionStringBuilder, TManager>
		where TConnection : DbConnection
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TConnectionStringBuilder, TManager>
	{
		/// <inheritdoc />
		bool IDatabaseBackupCreator.Backup(IDatabaseManager manager, FilePath backupFile)
		{
			return this.Backup((TManager)manager, backupFile);
		}

		/// <inheritdoc />
		bool IDatabaseBackupCreator.Restore(IDatabaseManager manager, FilePath backupFile)
		{
			return this.Restore((TManager)manager, backupFile);
		}

		/// <inheritdoc />
		public abstract bool RequiresScriptLocator { get; }

		/// <inheritdoc />
		public abstract bool SupportsRestore { get; }

		/// <inheritdoc />
		public abstract bool Backup (TManager manager, FilePath backupFile);

		/// <inheritdoc />
		public abstract bool Restore (TManager manager, FilePath backupFile);

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
