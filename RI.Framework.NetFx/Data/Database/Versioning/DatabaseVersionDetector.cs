using System.Data.Common;

using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database.Versioning
{
	public abstract class DatabaseVersionDetector<TConnection, TConnectionStringBuilder, TManager> : IDatabaseVersionDetector<TConnection, TConnectionStringBuilder, TManager>
		where TConnection : DbConnection
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TConnectionStringBuilder, TManager>
	{
		public abstract int Detect (TManager manager);

		int IDatabaseVersionDetector.Detect(IDatabaseManager manager)
		{
			return this.Detect((TManager)manager);
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
