using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;

using RI.Framework.IO.Paths;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database
{
	public abstract class DatabaseManager<TConnection, TConnectionStringBuilder, TManager, TConfiguration> : IDatabaseManager<TConnection, TConnectionStringBuilder, TManager>
		where TConnection : DbConnection
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : DatabaseManager<TConnection, TConnectionStringBuilder, TManager, TConfiguration>
		where TConfiguration : IDatabaseManagerConfiguration<TConnection, TConnectionStringBuilder, TManager>, new()
	{
		public IDatabaseManagerConfiguration<TConnection, TConnectionStringBuilder, TManager> Configuration { get; }
		IDatabaseManagerConfiguration IDatabaseManager.Configuration => this.Configuration;

		public DatabaseState State { get; private set; }
		public int Version { get; private set; }

		private List<TConnection> TrackedConnections { get; }
		private StateChangeEventHandler ConnectionStateChangedHandler { get; }

		protected abstract bool SupportsConnectionTrackingImpl { get; }
		protected abstract bool SupportsReadOnlyImpl { get; }
		protected abstract bool SupportsUpgradeImpl { get; }
		protected abstract bool SupportsCleanupImpl { get; }
		protected abstract bool SupportsBackupImpl { get; }
		protected abstract bool SupportsRestoreImpl { get; }
		protected abstract string DebugDetails { get; }

		public bool SupportsConnectionTracking => this.SupportsConnectionTrackingImpl;
		public bool SupportsReadOnly => this.SupportsReadOnlyImpl;
		public bool SupportsUpgrade => this.SupportsUpgradeImpl && this.Configuration.VersionUpgrader != null;
		public bool SupportsCleanup => this.SupportsCleanupImpl && this.Configuration.CleanupProcessor != null;
		public bool SupportsBackup => this.SupportsBackupImpl && this.Configuration.BackupCreator != null;
		public bool SupportsRestore => this.SupportsRestoreImpl && (this.Configuration.BackupCreator?.SupportsRestore ?? false);

		public bool IsReady => (this.State == DatabaseState.ReadyNew) || (this.State == DatabaseState.ReadyOld);
		public int MinVersion => this.SupportsUpgrade ? this.Configuration.VersionUpgrader.MinVersion : -1;
		public int MaxVersion => this.SupportsUpgrade ? this.Configuration.VersionUpgrader.MaxVersion : -1;
		public bool CanUpgrade => this.SupportsUpgrade && (this.Version >= 0) && (this.Version < this.MaxVersion);

		bool ILogSource.LoggingEnabled
		{
			get
			{
				return this.Configuration.LoggingEnabled;
			}
			set
			{
				this.Configuration.LoggingEnabled = value;
			}
		}
		ILogger ILogSource.Logger
		{
			get
			{
				return this.Configuration.Logger;
			}
			set
			{
				this.Configuration.Logger = value;
			}
		}






		public DatabaseManager ()
		{
			this.Configuration = new TConfiguration();

			this.State = DatabaseState.Uninitialized;
			this.Version = -1;

			this.TrackedConnections = new List<TConnection>();
			this.ConnectionStateChangedHandler = this.ConnectionStateChangedMethod;

			((ILogSource)this).Logger = null;
			((ILogSource)this).LoggingEnabled = true;
		}

		~DatabaseManager ()
		{
			this.Dispose(false);
		}
		public void Close ()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		void IDisposable.Dispose ()
		{
			this.Close();
		}

		DbConnection IDatabaseManager.CreateConnection (bool readOnly, bool track)
		{
			return this.CreateConnection(readOnly, track);
		}
		List<DbConnection> IDatabaseManager.GetTrackedConnections ()
		{
			return new List<DbConnection>(this.GetTrackedConnections().Cast<TConnection>());
		}

		public sealed override bool Equals (object obj)
		{
			return base.Equals(obj);
		}
		public sealed override int GetHashCode ()
		{
			return base.GetHashCode();
		}
		public sealed override string ToString () => this.DebugDetails;






		protected void PrepareConfiguration ()
		{
			//TODO: Implement
			this.Configuration.VerifyConfiguration();
			this.Configuration.InheritLogger();
		}
		protected void DetectVersionAndState ()
		{
			//TODO: Implement
			this.Version = this.Configuration.VersionDetector.Detect(this);
		}

		private void ConnectionStateChangedMethod (object sender, StateChangeEventArgs e)
		{
			TConnection connection = (TConnection)sender;

			//TODO: Raise event for handling connection state changes

			if ((e.CurrentState == ConnectionState.Broken) || (e.CurrentState == ConnectionState.Closed))
			{
				connection.StateChange -= this.ConnectionStateChangedHandler;
				this.TrackedConnections.Remove(connection);
			}
		}
		public void CloseTrackedConnections ()
		{
			List<TConnection> connections = this.GetTrackedConnections();
			connections.ForEach(x =>
			{
				try
				{
					x.Close();
				}
				catch (ObjectDisposedException)
				{
				}
				catch (Exception exception)
				{
					this.Log(LogLevel.Warning, "Exception while closing tracked connection: {0}{1}{2}", this.DebugDetails, Environment.NewLine, exception.ToDetailedString());
				}

				x.StateChange -= this.ConnectionStateChangedHandler;
			});

			this.TrackedConnections?.Clear();
		}
		public List<TConnection> GetTrackedConnections ()
		{
			return new List<TConnection>(this.TrackedConnections ?? new List<TConnection>());
		}






		protected virtual void InitializeImpl ()
		{
		}
		protected virtual void DisposeImpl (bool disposing)
		{
		}
		protected abstract TConnection CreateConnectionImpl (bool readOnly);
		protected virtual void UpgradeImpl (int sourceVersion)
		{
			this.Log(LogLevel.Information, "Performing database upgrade: {0}->{1}: {2}", sourceVersion, sourceVersion + 1, this.DebugDetails);
			this.Configuration.VersionUpgrader.Upgrade(this, sourceVersion);
		}
		protected virtual void BackupImpl (FilePath backupFile)
		{
			this.Log(LogLevel.Information, "Performing database backup: {0}: {1}", backupFile, this.DebugDetails);
			this.Configuration.BackupCreator.Backup(this, backupFile);
		}
		protected virtual void RestoreImpl (FilePath backupFile)
		{
			this.Log(LogLevel.Information, "Performing database restore: {0}: {1}", backupFile, this.DebugDetails);
			this.Configuration.BackupCreator.Restore(this, backupFile);
		}
		protected virtual void CleanupImpl ()
		{
			this.Log(LogLevel.Information, "Performing database cleanup: {0}", this.DebugDetails);
			this.Configuration.CleanupProcessor.Cleanup(this);
		}





		public void Initialize ()
		{
			this.Log(LogLevel.Information, "Initializing database: {0}", this.DebugDetails);

			this.Close();

			GC.ReRegisterForFinalize(this);

			this.PrepareConfiguration();

			this.InitializeImpl();

			this.DetectVersionAndState();
		}
		protected void Dispose (bool disposing)
		{
			this.Log(LogLevel.Information, "Closing database: {0}", this.DebugDetails);

			this.DisposeImpl(disposing);

			this.CloseTrackedConnections();

			this.State = DatabaseState.Uninitialized;
			this.Version = -1;
		}
		public TConnection CreateConnection (bool readOnly, bool track)
		{
			if (!this.IsReady)
			{
				throw new InvalidOperationException(this.GetType().Name + " must be in a ready state to create a connection, current state is " + this.State + ".");
			}

			if ((!this.SupportsReadOnly) && readOnly)
			{
				throw new NotSupportedException(this.GetType().Name + " does not support read-only connections.");
			}

			if ((!this.SupportsConnectionTracking) && track)
			{
				throw new NotSupportedException(this.GetType().Name + " does not support connection tracking.");
			}

			this.PrepareConfiguration();

			TConnection connection = this.CreateConnectionImpl(readOnly);

			if (track)
			{
				this.TrackedConnections.Add(connection);
				connection.StateChange += this.ConnectionStateChangedHandler;
			}

			return connection;
		}
		public void Upgrade (int version)
		{
			if (!this.IsReady)
			{
				throw new InvalidOperationException(this.GetType().Name + " must be in a ready state to perform an upgrade, current state is " + this.State + ".");
			}

			if (!this.SupportsUpgrade)
			{
				throw new NotSupportedException(this.GetType().Name + " does not support upgrades.");
			}

			if((version < this.MinVersion) || (version > this.MaxVersion))
			{
				throw new ArgumentOutOfRangeException(nameof(version), "The specified version " + version + " is not within the supported version range (" + this.MinVersion + "..." + this.MaxVersion + ").");
			}

			if (version < this.Version)
			{
				throw new ArgumentOutOfRangeException(nameof(version), "The supported version " + version + " is lower than the current version (" + this.Version + ").");
			}

			if (version == this.Version)
			{
				return;
			}

			this.PrepareConfiguration();

			for (int currentVersion = this.Version; currentVersion < version; currentVersion++)
			{
				this.UpgradeImpl(version);

				this.DetectVersionAndState();

				if (!this.IsReady)
				{
					break;
				}
			}
		}
		public void Backup (FilePath backupFile)
		{
			if (backupFile == null)
			{
				throw new ArgumentNullException(nameof(backupFile));
			}

			if (!backupFile.IsRealFile)
			{
				throw new InvalidPathArgumentException(nameof(backupFile));
			}

			if (!this.IsReady)
			{
				throw new InvalidOperationException(this.GetType().Name + " must be in a ready state to perform a backup, current state is " + this.State + ".");
			}

			if (!this.SupportsBackup)
			{
				throw new NotSupportedException(this.GetType().Name + " does not support backups.");
			}

			this.PrepareConfiguration();

			this.BackupImpl(backupFile);

			this.DetectVersionAndState();
		}
		public void Restore (FilePath backupFile)
		{
			if (backupFile == null)
			{
				throw new ArgumentNullException(nameof(backupFile));
			}

			if (!backupFile.IsRealFile)
			{
				throw new InvalidPathArgumentException(nameof(backupFile));
			}

			if (!this.IsReady)
			{
				throw new InvalidOperationException(this.GetType().Name + " must be in a ready state to perform a restore, current state is " + this.State + ".");
			}

			if (!this.SupportsRestore)
			{
				throw new NotSupportedException(this.GetType().Name + " does not support restores.");
			}

			this.PrepareConfiguration();

			this.RestoreImpl(backupFile);

			this.DetectVersionAndState();
		}
		public void Cleanup ()
		{
			if (!this.IsReady)
			{
				throw new InvalidOperationException(this.GetType().Name + " must be in a ready state to perform a cleanup, current state is " + this.State + ".");
			}

			if (!this.SupportsCleanup)
			{
				throw new NotSupportedException(this.GetType().Name + " does not support cleanups.");
			}

			this.PrepareConfiguration();

			this.CleanupImpl();

			this.DetectVersionAndState();
		}
	}
}
