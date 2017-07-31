using System.Data.Common;

using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database.Versioning
{
	public interface IDatabaseVersionDetector : ILogSource
	{
		int Detect (IDatabaseManager manager);
	}

	public interface IDatabaseVersionDetector<TConnection, TConnectionStringBuilder, TManager> : IDatabaseVersionDetector
		where TConnection : DbConnection
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TConnectionStringBuilder, TManager>
	{
		int Detect (TManager manager);
	}
}
