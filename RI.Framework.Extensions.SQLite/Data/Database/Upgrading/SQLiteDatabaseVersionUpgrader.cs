using System.Data.SQLite;

namespace RI.Framework.Data.Database.Upgrading
{
	public sealed class SQLiteDatabaseVersionUpgrader : IDatabaseVersionUpgrader<SQLiteConnection, SQLiteConnectionStringBuilder, SQLiteDatabaseManager>
	{
	}
}
