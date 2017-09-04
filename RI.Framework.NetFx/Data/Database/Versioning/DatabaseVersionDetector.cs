using System.Data.Common;

using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database.Versioning
{
	/// <summary>
	/// Implements a base class for database version detectors.
	/// </summary>
	/// <typeparam name="TConnection">The database connection type, subclass of <see cref="DbConnection"/>.</typeparam>
	/// <typeparam name="TTransaction">The database transaction type, subclass of <see cref="DbTransaction"/>.</typeparam>
	/// <typeparam name="TConnectionStringBuilder">The connection string builder type, subclass of <see cref="DbConnectionStringBuilder"/>.</typeparam>
	/// <typeparam name="TManager">The type of the database manager.</typeparam>
	/// <typeparam name="TConfiguration">The type of database configuration.</typeparam>
	/// <remarks>
	/// <para>
	/// It is recommended that database version detector implementations use this base class as it already implements most of the logic which is database-independent.
	/// </para>
	/// <para>
	/// See <see cref="IDatabaseVersionDetector"/> for more details.
	/// </para>
	/// </remarks>
	public abstract class DatabaseVersionDetector<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> : IDatabaseVersionDetector<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
		where TConnection : DbConnection
		where TTransaction : DbTransaction
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
		where TConfiguration : class, IDatabaseManagerConfiguration<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>, new()
	{
		/// <inheritdoc />
		public abstract bool Detect (TManager manager, out DatabaseState? state, out int version);

		/// <inheritdoc />
		public abstract bool RequiresScriptLocator { get; }

		/// <inheritdoc />
		bool IDatabaseVersionDetector.Detect(IDatabaseManager manager, out DatabaseState? state, out int version)
		{
			return this.Detect((TManager)manager, out state, out version);
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
