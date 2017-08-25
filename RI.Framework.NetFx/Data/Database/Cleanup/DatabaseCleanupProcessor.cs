﻿using System.Data.Common;

using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database.Cleanup
{
	/// <summary>
	/// Implements a base class for database cleanup processors.
	/// </summary>
	/// <typeparam name="TConnection">The database connection type, subclass of <see cref="DbConnection"/>.</typeparam>
	/// <typeparam name="TConnectionStringBuilder">The connection string builder type, subclass of <see cref="DbConnectionStringBuilder"/>.</typeparam>
	/// <typeparam name="TManager">The type of the database manager which is using this cleanup processor.</typeparam>
	/// <remarks>
	/// <para>
	/// It is recommended that database cleanup processor implementations use this base class as it already implements most of the logic which is database-independent.
	/// </para>
	/// <para>
	/// See <see cref="IDatabaseCleanupProcessor"/> for more details.
	/// </para>
	/// </remarks>
	public abstract class DatabaseCleanupProcessor<TConnection, TConnectionStringBuilder, TManager> : IDatabaseCleanupProcessor<TConnection, TConnectionStringBuilder, TManager>
		where TConnection : DbConnection
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TConnectionStringBuilder, TManager>
	{
		/// <inheritdoc />
		public abstract bool Cleanup (TManager managerversion);

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