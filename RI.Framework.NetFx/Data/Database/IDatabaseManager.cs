using System;
using System.Collections.Generic;
using System.Data.Common;

using RI.Framework.IO.Paths;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database
{
	public interface IDatabaseManager : IDisposable, ILogSource
	{
		IDatabaseManagerConfiguration Configuration { get; }

		DatabaseState State { get; }
		int Version { get; }

		DatabaseState InitialState { get; }
		int InitialVersion { get; }

		bool IsReady { get; }
		bool CanUpgrade { get; }
		int MinVersion { get; }
		int MaxVersion { get; }

		bool SupportsConnectionTracking { get; }
		bool SupportsReadOnly { get; }
		bool SupportsCleanup { get; }
		bool SupportsBackup { get; }
		bool SupportsRestore { get; }
		bool SupportsUpgrade { get; }

		void Initialize ();
		void Close ();

		DbConnection CreateConnection (bool readOnly, bool track);

		void Cleanup ();
		void Backup (FilePath backupFile);
		void Restore (FilePath backupFile);
		void Upgrade (int version);
		void Upgrade ();

		List<DbConnection> GetTrackedConnections ();
		void CloseTrackedConnections ();

		event EventHandler<DatabaseStateChangedEventArgs> StateChanged;
		event EventHandler<DatabaseVersionChangedEventArgs> VersionChanged;
		event EventHandler<DatabaseConnectionChangedEventArgs> ConnectionChanged;
		event EventHandler<DatabaseConnectionCreatedEventArgs> ConnectionCreated;
	}

	public interface IDatabaseManager<TConnection, TConnectionStringBuilder, TManager> : IDatabaseManager
		where TConnection : DbConnection
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TConnectionStringBuilder, TManager>
	{
		new IDatabaseManagerConfiguration<TConnection, TConnectionStringBuilder, TManager> Configuration { get; }

		new TConnection CreateConnection (bool readOnly, bool track);

		new List<TConnection> GetTrackedConnections();

		new event EventHandler<DatabaseConnectionChangedEventArgs<TConnection>> ConnectionChanged;
		new event EventHandler<DatabaseConnectionCreatedEventArgs<TConnection>> ConnectionCreated;
	}
}
