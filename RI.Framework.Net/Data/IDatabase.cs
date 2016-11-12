using System.Data.Common;

namespace RI.Framework.Data
{
	public interface IDatabase
	{
		DatabaseState State { get; }

		DbConnectionStringBuilder ConnectionStringBuilder { get; }

		string ConnectionString { get; set; }

		void InitializeDatabase ();

		void CleanupDatabase ();

		void UpgradeDatabase ();

		void UnloadDatabase ();

		IRepositoryContext CreateRepository ();

		DbConnection CreateConnection ();
	}
}