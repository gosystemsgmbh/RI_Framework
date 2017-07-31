using System.Data.Common;

using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database.Upgrading
{
	public abstract class DatabaseVersionUpgrader<TConnection, TConnectionStringBuilder, TManager> : IDatabaseVersionUpgrader<TConnection, TConnectionStringBuilder, TManager>
		where TConnection : DbConnection
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TConnectionStringBuilder, TManager>
	{
		public abstract void Upgrade (TManager manager, int sourceVersion);

		public abstract int MinVersion { get; }

		public abstract int MaxVersion { get; }

		void IDatabaseVersionUpgrader.Upgrade(IDatabaseManager manager, int sourceVersion)
		{
			this.Upgrade((TManager)manager, sourceVersion);
		}

		private bool _loggingEnabled;

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
