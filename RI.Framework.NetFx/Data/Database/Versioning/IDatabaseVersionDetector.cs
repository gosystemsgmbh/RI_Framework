using System.Data.Common;

using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database.Versioning
{
	public interface IDatabaseVersionDetector : ILogSource
	{
		bool Detect (IDatabaseManager manager, out DatabaseState? state, out int version);
	}

	public interface IDatabaseVersionDetector<TConnection, TConnectionStringBuilder, TManager> : IDatabaseVersionDetector
		where TConnection : DbConnection
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TConnectionStringBuilder, TManager>
	{
		bool Detect (TManager manager, out DatabaseState? state, out int version);
	}
}
