﻿using System.Data.Common;

using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database.Cleanup
{
	/// <summary>
	/// Implements a base class for database cleanup processors.
	/// </summary>
	/// <typeparam name="TConnection">The database connection type, subclass of <see cref="DbConnection"/>.</typeparam>
	/// <typeparam name="TTransaction">The database transaction type, subclass of <see cref="DbTransaction"/>.</typeparam>
	/// <typeparam name="TConnectionStringBuilder">The connection string builder type, subclass of <see cref="DbConnectionStringBuilder"/>.</typeparam>
	/// <typeparam name="TManager">The type of the database manager.</typeparam>
	/// <typeparam name="TConfiguration">The type of database configuration.</typeparam>
	/// <remarks>
	/// <para>
	/// It is recommended that database cleanup processor implementations use this base class as it already implements most of the logic which is database-independent.
	/// </para>
	/// <para>
	/// See <see cref="IDatabaseCleanupProcessor"/> for more details.
	/// </para>
	/// </remarks>
	public abstract class DatabaseCleanupProcessor<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> : IDatabaseCleanupProcessor<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
		where TConnection : DbConnection
		where TTransaction : DbTransaction
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
		where TConfiguration : class, IDatabaseManagerConfiguration<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>, new()
	{
		/// <inheritdoc />
		public abstract bool Cleanup (TManager manager);

		/// <inheritdoc />
		public abstract bool RequiresScriptLocator { get; }

		/// <inheritdoc />
		bool IDatabaseCleanupProcessor.Cleanup(IDatabaseManager manager)
		{
			return this.Cleanup((TManager)manager);
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
