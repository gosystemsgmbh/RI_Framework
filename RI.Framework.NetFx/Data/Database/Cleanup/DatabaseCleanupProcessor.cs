using System.Data.Common;

using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database.Cleanup
{
	public abstract class DatabaseCleanupProcessor<TConnection, TConnectionStringBuilder, TManager> : IDatabaseCleanupProcessor<TConnection, TConnectionStringBuilder, TManager>
		where TConnection : DbConnection
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TConnectionStringBuilder, TManager>
	{
		public abstract void Cleanup (TManager managerversion);

		void IDatabaseCleanupProcessor.Cleanup(IDatabaseManager manager)
		{
			this.Cleanup((TManager)manager);
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
