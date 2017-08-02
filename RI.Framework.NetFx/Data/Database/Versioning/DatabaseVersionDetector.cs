using System.Data.Common;

using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database.Versioning
{
	public abstract class DatabaseVersionDetector<TConnection, TConnectionStringBuilder, TManager> : IDatabaseVersionDetector<TConnection, TConnectionStringBuilder, TManager>
		where TConnection : DbConnection
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TConnectionStringBuilder, TManager>
	{
		public abstract bool Detect (TManager manager, out DatabaseState? state, out int version);

		bool IDatabaseVersionDetector.Detect(IDatabaseManager manager, out DatabaseState? state, out int version)
		{
			return this.Detect((TManager)manager, out state, out version);
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
