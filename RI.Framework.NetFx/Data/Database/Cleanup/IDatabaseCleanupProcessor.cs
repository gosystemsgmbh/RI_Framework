using System.Data.Common;

using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database.Cleanup
{
	public interface IDatabaseCleanupProcessor : ILogSource
	{
		void Cleanup (IDatabaseManager manager);
	}

	public interface IDatabaseCleanupProcessor<TConnection, TConnectionStringBuilder, TManager> : IDatabaseCleanupProcessor
		where TConnection : DbConnection
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TConnectionStringBuilder, TManager>
	{
		void Cleanup (TManager managerversion);
	}
}
