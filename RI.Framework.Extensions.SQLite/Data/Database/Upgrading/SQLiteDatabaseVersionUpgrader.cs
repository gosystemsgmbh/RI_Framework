using System.Data.SQLite;

namespace RI.Framework.Data.Database.Upgrading
{
	public sealed class SQLiteDatabaseVersionUpgrader : DatabaseVersionUpgrader<SQLiteConnection, SQLiteConnectionStringBuilder, SQLiteDatabaseManager>
	{
	}
}
