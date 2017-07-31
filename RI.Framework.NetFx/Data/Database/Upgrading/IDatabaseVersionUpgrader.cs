using System.Data.Common;

using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database.Upgrading
{
	public interface IDatabaseVersionUpgrader : ILogSource
	{
		int MinVersion { get; }

		int MaxVersion { get; }

		void Upgrade (IDatabaseManager manager, int sourceVersion);
	}

	public interface IDatabaseVersionUpgrader<TConnection, TConnectionStringBuilder, TManager> : IDatabaseVersionUpgrader
		where TConnection : DbConnection
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TConnectionStringBuilder, TManager>
	{
		void Upgrade (TManager manager, int sourceVersion);
	}
}
