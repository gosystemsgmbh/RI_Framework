using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using RI.Framework.Data.Database.Backup;
using RI.Framework.Data.Database.Cleanup;
using RI.Framework.Data.Database.Scripts;
using RI.Framework.Data.Database.Upgrading;
using RI.Framework.Data.Database.Versioning;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database
{
	/// <summary>
	/// Implements a base class for database managers.
	/// </summary>
	/// <typeparam name="TConnection">The database connection type, subclass of <see cref="DbConnection"/>.</typeparam>
	/// <typeparam name="TTransaction">The database transaction type, subclass of <see cref="DbTransaction"/>.</typeparam>
	/// <typeparam name="TConnectionStringBuilder">The connection string builder type, subclass of <see cref="DbConnectionStringBuilder"/>.</typeparam>
	/// <typeparam name="TManager">The type of the database manager.</typeparam>
	/// <typeparam name="TConfiguration">The type of database configuration.</typeparam>
	/// <remarks>
	/// <para>
	/// It is recommended that database manager implementations use this base class as it already implements most of the logic which is database-independent.
	/// </para>
	/// <para>
	/// See <see cref="IDatabaseManager"/> for more details.
	/// </para>
	/// </remarks>
	public abstract class DatabaseManager<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> : IDatabaseManager<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
		where TConnection : DbConnection
		where TTransaction : DbTransaction
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
		where TConfiguration : class, IDatabaseManagerConfiguration<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>, new()
	{
		/// <inheritdoc cref="IDatabaseManager.Configuration"/>
		public TConfiguration Configuration { get; }

		/// <inheritdoc />
		IDatabaseManagerConfiguration<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> IDatabaseManager<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>.Configuration => this.Configuration;

		/// <inheritdoc />
		IDatabaseManagerConfiguration IDatabaseManager.Configuration => this.Configuration;

		/// <inheritdoc />
		public DatabaseState State { get; private set; }

		/// <inheritdoc />
		public int Version { get; private set; }

		/// <inheritdoc />
		public DatabaseState InitialState { get; private set; }

		/// <inheritdoc />
		public int InitialVersion { get; private set; }

		private List<TConnection> TrackedConnections { get; }

		private StateChangeEventHandler ConnectionStateChangedHandler { get; }

		/// <summary>
		/// Gets whether this database manager implementation supports connection tracking.
		/// </summary>
		/// <value>
		/// true if connection tracking is supported, false otherwise.
		/// </value>
		protected abstract bool SupportsConnectionTrackingImpl { get; }

		/// <summary>
		/// Gets whether this database manager implementation supports read-only connections.
		/// </summary>
		/// <value>
		/// true if read-only connections are supported, false otherwise.
		/// </value>
		protected abstract bool SupportsReadOnlyImpl { get; }

		/// <summary>
		/// Gets whether this database manager implementation supports script retrieval.
		/// </summary>
		/// <value>
		/// true if script retrieval is supported, false otherwise.
		/// </value>
		protected abstract bool SupportsScriptsImpl { get; }

		/// <summary>
		/// Gets whether this database manager implementation supports upgrading.
		/// </summary>
		/// <value>
		/// true if upgrading is supported, false otherwise.
		/// </value>
		protected abstract bool SupportsUpgradeImpl { get; }

		/// <summary>
		/// Gets whether this database manager implementation supports cleanup.
		/// </summary>
		/// <value>
		/// true if cleanup is supported, false otherwise.
		/// </value>
		protected abstract bool SupportsCleanupImpl { get; }

		/// <summary>
		/// Gets whether this database manager implementation supports backup.
		/// </summary>
		/// <value>
		/// true if backup is supported, false otherwise.
		/// </value>
		protected abstract bool SupportsBackupImpl { get; }

		/// <summary>
		/// Gets whether this database manager implementation supports restore.
		/// </summary>
		/// <value>
		/// true if restore is supported, false otherwise.
		/// </value>
		protected abstract bool SupportsRestoreImpl { get; }

		/// <summary>
		/// Gets a string which describes the current database manager instance and can be used for logging debugging details.
		/// </summary>
		/// <value>
		/// A string which describes the current database manager instance
		/// </value>
		protected virtual string DebugDetails => string.Format(CultureInfo.InvariantCulture, "{0}; State={1}; Version={2}; MinVersion={3}; MaxVersion={4}; Connections={5}; ConnectionString=[{6}]", this.GetType().Name, this.State, this.Version, this.MinVersion, this.MaxVersion, this.TrackedConnections.Count, this.Configuration.ConnectionString?.ConnectionString ?? "[null]");

		/// <inheritdoc />
		public bool SupportsConnectionTracking => this.SupportsConnectionTrackingImpl;

		/// <inheritdoc />
		public bool SupportsReadOnly => this.SupportsReadOnlyImpl;

		/// <inheritdoc />
		public bool SupportsScripts => this.SupportsScriptsImpl && (this.Configuration.ScriptLocator != null);

		/// <inheritdoc />
		public bool SupportsUpgrade => this.SupportsUpgradeImpl && (this.Configuration.VersionUpgrader != null);

		/// <inheritdoc />
		public bool SupportsCleanup => this.SupportsCleanupImpl && (this.Configuration.CleanupProcessor != null);

		/// <inheritdoc />
		public bool SupportsBackup => this.SupportsBackupImpl && (this.Configuration.BackupCreator != null);

		/// <inheritdoc />
		public bool SupportsRestore => this.SupportsRestoreImpl && (this.Configuration.BackupCreator?.SupportsRestore ?? false);

		/// <inheritdoc />
		public bool IsReady => (this.State == DatabaseState.ReadyNew) || (this.State == DatabaseState.ReadyOld) || (this.State == DatabaseState.ReadyUnknown);

		/// <inheritdoc />
		public int MinVersion => this.SupportsUpgrade ? this.Configuration.VersionUpgrader.GetMinVersion(this) : -1;

		/// <inheritdoc />
		public int MaxVersion => this.SupportsUpgrade ? this.Configuration.VersionUpgrader.GetMaxVersion(this) : -1;

		/// <inheritdoc />
		public bool CanUpgrade => this.SupportsUpgrade && (this.IsReady || (this.State == DatabaseState.New)) && (this.Version >= 0) && (this.Version < this.MaxVersion);

		/// <inheritdoc />
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

		/// <inheritdoc />
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





		/// <summary>
		/// Creates a new instance of <see cref="DatabaseManager{TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration}"/>.
		/// </summary>
		protected DatabaseManager ()
		{
			this.Configuration = new TConfiguration();

			this.TrackedConnections = new List<TConnection>();
			this.ConnectionStateChangedHandler = this.ConnectionStateChangedMethod;

			((ILogSource)this).Logger = LogLocator.Logger;
			((ILogSource)this).LoggingEnabled = true;

			this.InitialState = DatabaseState.Uninitialized;
			this.InitialVersion = -1;

			this.SetStateAndVersion(DatabaseState.Uninitialized, -1);
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="DatabaseManager{TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration}"/>.
		/// </summary>
		~DatabaseManager ()
		{
			this.Dispose(false);
		}

		/// <inheritdoc />
		public void Close ()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <inheritdoc />
		void IDisposable.Dispose ()
		{
			this.Close();
		}

		/// <inheritdoc />
		DbConnection IDatabaseManager.CreateConnection (bool readOnly, bool track)
		{
			return this.CreateConnection(readOnly, track);
		}

		/// <inheritdoc />
		public IDatabaseProcessingStep<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> CreateProcessingStep ()
		{
			this.PrepareConfiguration();

			return this.CreateProcessingStepImpl();
		}

		/// <inheritdoc />
		IDatabaseProcessingStep IDatabaseManager.CreateProcessingStep () => this.CreateProcessingStep();

		/// <inheritdoc />
		List<DbConnection> IDatabaseManager.GetTrackedConnections ()
		{
			return new List<DbConnection>(this.GetTrackedConnections());
		}

		/// <inheritdoc />
		[SuppressMessage("ReSharper", "BaseObjectEqualsIsObjectEquals")]
		public sealed override bool Equals (object obj)
		{
			return base.Equals(obj);
		}

		/// <inheritdoc />
		[SuppressMessage("ReSharper", "BaseObjectGetHashCodeCallInGetHashCode")]
		public sealed override int GetHashCode ()
		{
			return base.GetHashCode();
		}

		/// <inheritdoc />
		public sealed override string ToString () => this.DebugDetails;





		/// <summary>
		/// Prepares and verifies the database configuration (<see cref="Configuration"/>) before it is used for operations.
		/// </summary>
		protected void PrepareConfiguration ()
		{
			this.PrepareConfigurationImpl();
		}

		/// <summary>
		/// Performs a database state and version detection and updates <see cref="State"/> and <see cref="Version"/>.
		/// </summary>
		protected void DetectStateAndVersion ()
		{
			DatabaseState? state;
			int version;

			bool valid = this.DetectStateAndVersionImpl(out state, out version);

			if ((!valid) || (version < 0) || (state.GetValueOrDefault(DatabaseState.Uninitialized) == DatabaseState.DamagedOrInvalid) || (state.GetValueOrDefault(DatabaseState.ReadyUnknown) == DatabaseState.DamagedOrInvalid))
			{
				state = DatabaseState.DamagedOrInvalid;
				version = -1;
			}
			else if (!state.HasValue)
			{
				if (this.SupportsUpgrade)
				{
					if (version == 0)
					{
						state = DatabaseState.New;
					}
					else if (version < this.MinVersion)
					{
						state = DatabaseState.TooOld;
					}
					else if((version >= this.MinVersion) && (version < this.MaxVersion))
					{
						state = DatabaseState.ReadyOld;
					}
					else if (version == this.MaxVersion)
					{
						state = DatabaseState.ReadyNew;
					}
					else if (version > this.MaxVersion)
					{
						state = DatabaseState.TooNew;
					}
					else
					{
						state = DatabaseState.ReadyUnknown;
					}
				}
				else
				{
					state = version == 0 ? DatabaseState.Unavailable : DatabaseState.ReadyUnknown;
				}
			}

			this.SetStateAndVersion(state.Value, version);
		}

		private void SetStateAndVersion (DatabaseState state, int version)
		{
			DatabaseState oldState = this.State;
			int oldVersion = this.Version;

			this.State = state;
			this.Version = version;

			if (oldState != state)
			{
				this.Log(LogLevel.Information, "Database state changed: {0} -> {1}: {2}", oldState, state, this.DebugDetails);
				this.OnStateChanged(oldState, state);
			}

			if (oldVersion != version)
			{
				this.Log(LogLevel.Information, "Database version changed: {0} -> {1}: {2}", oldVersion, version, this.DebugDetails);
				this.OnVersionChanged(oldVersion, version);
			}
		}

		private void ConnectionStateChangedMethod (object sender, StateChangeEventArgs e)
		{
			TConnection connection = sender as TConnection;
			if (connection == null)
			{
				return;
			}

			if ((e.CurrentState == ConnectionState.Broken) || (e.CurrentState == ConnectionState.Closed))
			{
				connection.StateChange -= this.ConnectionStateChangedHandler;
				this.TrackedConnections.Remove(connection);
			}

			if (e.OriginalState != e.CurrentState)
			{
				this.OnConnectionChanged(connection, e.OriginalState, e.CurrentState);
			}
		}

		/// <inheritdoc />
		public void CloseTrackedConnections ()
		{
			List<TConnection> connections = this.GetTrackedConnections() ?? new List<TConnection>();
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

		/// <inheritdoc />
		public List<TConnection> GetTrackedConnections ()
		{
			if (!this.SupportsConnectionTracking)
			{
				return null;
			}

			return new List<TConnection>(this.TrackedConnections ?? new List<TConnection>());
		}

		/// <inheritdoc />
		public event EventHandler<DatabaseStateChangedEventArgs> StateChanged;

		/// <inheritdoc />
		public event EventHandler<DatabaseVersionChangedEventArgs> VersionChanged;

		/// <inheritdoc />
		public event EventHandler<DatabaseConnectionChangedEventArgs<TConnection>> ConnectionChanged
		{
			add
			{
				this.ConnectionChangedInternal2 += value;
			}
			remove
			{
				this.ConnectionChangedInternal2 -= value;
			}
		}

		/// <inheritdoc />
		event EventHandler<DatabaseConnectionChangedEventArgs> IDatabaseManager.ConnectionChanged
		{
			add
			{
				this.ConnectionChangedInternal1 += value;
			}
			remove
			{
				this.ConnectionChangedInternal1 -= value;
			}
		}

		private event EventHandler<DatabaseConnectionChangedEventArgs> ConnectionChangedInternal1;

		private event EventHandler<DatabaseConnectionChangedEventArgs<TConnection>> ConnectionChangedInternal2;

		/// <inheritdoc />
		public event EventHandler<DatabaseConnectionCreatedEventArgs<TConnection>> ConnectionCreated
		{
			add
			{
				this.ConnectionCreatedInternal2 += value;
			}
			remove
			{
				this.ConnectionCreatedInternal2 -= value;
			}
		}

		/// <inheritdoc />
		event EventHandler<DatabaseConnectionCreatedEventArgs> IDatabaseManager.ConnectionCreated
		{
			add
			{
				this.ConnectionCreatedInternal1 += value;
			}
			remove
			{
				this.ConnectionCreatedInternal1 -= value;
			}
		}

		private event EventHandler<DatabaseConnectionCreatedEventArgs> ConnectionCreatedInternal1;

		private event EventHandler<DatabaseConnectionCreatedEventArgs<TConnection>> ConnectionCreatedInternal2;

		/// <inheritdoc />
		public event EventHandler<DatabaseScriptRetrievedEventArgs> ScriptRetrieved;

		/// <summary>
		/// Called when the current database state has changed.
		/// </summary>
		/// <param name="oldState">The previous state.</param>
		/// <param name="newState">The new current state.</param>
		/// <remarks>
		/// <para>
		/// The default implementation raises the <see cref="StateChanged"/> event.
		/// </para>
		/// </remarks>
		protected virtual void OnStateChanged (DatabaseState oldState, DatabaseState newState)
		{
			this.StateChanged?.Invoke(this, new DatabaseStateChangedEventArgs(oldState, newState));
		}

		/// <summary>
		/// Called when the current database version has changed.
		/// </summary>
		/// <param name="oldVersion">The previous version.</param>
		/// <param name="newVersion">The new current version.</param>
		/// <remarks>
		/// <para>
		/// The default implementation raises the <see cref="VersionChanged"/> event.
		/// </para>
		/// </remarks>
		protected virtual void OnVersionChanged (int oldVersion, int newVersion)
		{
			this.VersionChanged?.Invoke(this, new DatabaseVersionChangedEventArgs(oldVersion, newVersion));
		}

		/// <summary>
		/// Called when the state of a tracked connection has changed.
		/// </summary>
		/// <param name="connection">The tracked connection.</param>
		/// <param name="oldState">The previous state.</param>
		/// <param name="newState">The new current state.</param>
		/// <remarks>
		/// <para>
		/// The default implementation raises the <see cref="ConnectionChanged"/> event.
		/// </para>
		/// </remarks>
		protected virtual void OnConnectionChanged (TConnection connection, ConnectionState oldState, ConnectionState newState)
		{
			DatabaseConnectionChangedEventArgs<TConnection> args = new DatabaseConnectionChangedEventArgs<TConnection>(connection, oldState, newState);
			this.ConnectionChangedInternal1?.Invoke(this, args);
			this.ConnectionChangedInternal2?.Invoke(this, args);
		}

		/// <summary>
		/// Called when a connection has been created.
		/// </summary>
		/// <param name="connection">The tracked connection.</param>
		/// <param name="readOnly">Indicates whether the connection is read-only.</param>
		/// <param name="tracked">Indicates whether the connection is going to be tracked.</param>
		/// <remarks>
		/// <para>
		/// The default implementation raises the <see cref="ConnectionCreated"/> event.
		/// </para>
		/// </remarks>
		protected virtual void OnConnectionCreated (TConnection connection, bool readOnly, bool tracked)
		{
			DatabaseConnectionCreatedEventArgs<TConnection> args = new DatabaseConnectionCreatedEventArgs<TConnection>(connection, readOnly, tracked);
			this.ConnectionCreatedInternal1?.Invoke(this, args);
			this.ConnectionCreatedInternal2?.Invoke(this, args);
		}

		/// <summary>
		/// Called when a script has been retrieved
		/// </summary>
		/// <param name="name">The name of the retrieved script.</param>
		/// <param name="preprocess">Specifies whether the script is preprocessed.</param>
		/// <param name="scriptBatches">The list of retrieved individual batches.</param>
		/// <remarks>
		/// <para>
		/// The default implementation raises the <see cref="ScriptRetrieved"/> event.
		/// </para>
		/// </remarks>
		protected virtual void OnScriptRetrieved(string name, bool preprocess, List<string> scriptBatches)
		{
			this.ScriptRetrieved?.Invoke(this, new DatabaseScriptRetrievedEventArgs(name, preprocess, scriptBatches));
		}





		/// <summary>
		/// Performs the actual state and version detection as required by this database manager implementation.
		/// </summary>
		/// <param name="state">Returns the state of the database. Can be null to perform state detection based on <paramref name="version"/> as implemented in <see cref="DatabaseManager{TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration}"/>.</param>
		/// <param name="version">Returns the version of the database.</param>
		/// <returns>
		/// true if the state and version could be successfully determined, false if the database is damaged or in an invalid state.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The default implementation calls <see cref="IDatabaseVersionDetector.Detect"/>.
		/// </para>
		/// </remarks>
		protected virtual bool DetectStateAndVersionImpl (out DatabaseState? state, out int version)
		{
			return this.Configuration.VersionDetector.Detect(this, out state, out version);
		}

		/// <summary>
		/// Prepares and verifies the database configuration (<see cref="Configuration"/>) before it is used for operations, as it is required by this database manager implementation.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The default implementation calls <see cref="IDatabaseManagerConfiguration.VerifyConfiguration"/> and <see cref="IDatabaseManagerConfiguration.InheritLogger"/>.
		/// </para>
		/// </remarks>
		protected virtual void PrepareConfigurationImpl ()
		{
			this.Configuration.VerifyConfiguration(this);
			this.Configuration.InheritLogger();
		}

		/// <summary>
		/// Performs initialization specific to this database manager implementation.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The default implementation does nothing.
		/// </para>
		/// </remarks>
		protected virtual void InitializeImpl ()
		{
		}

		/// <summary>
		/// Performs disposing specific to this database manager implementation.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The default implementation does nothing.
		/// </para>
		/// </remarks>
		protected virtual void DisposeImpl (bool disposing)
		{
		}

		/// <summary>
		/// Creates a new database connection.
		/// </summary>
		/// <param name="readOnly">Specifies whether the connection should be read-only.</param>
		/// <returns>
		/// The newly created and already opened connection or null if the connection could not be created.
		/// Details can be obtained from the log.
		/// </returns>
		protected abstract TConnection CreateConnectionImpl (bool readOnly);

		/// <summary>
		/// Creates a new database processing step.
		/// </summary>
		/// <returns>
		/// The newly created database processing step.
		/// </returns>
		protected abstract IDatabaseProcessingStep<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> CreateProcessingStepImpl();

		/// <summary>
		/// Retrieves a script batch
		/// </summary>
		/// <param name="name">The name of the script.</param>
		/// <param name="preprocess">Specifies whether the script is to be preprocessed.</param>
		/// <returns>
		/// The list of batches in the script.
		/// If the script is empty or does not contain any bacthes respectively, an empty list is returned.
		/// If the script could not be found, null is returned.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The default implementation calls <see cref="IDatabaseScriptLocator.GetScriptBatch"/>.
		/// </para>
		/// </remarks>
		protected virtual List<string> GetScriptBatchImpl(string name, bool preprocess)
		{
			return this.Configuration.ScriptLocator.GetScriptBatch(this, name, preprocess);
		}

		/// <summary>
		/// Performs an upgrade from <paramref name="sourceVersion"/> to <paramref name="sourceVersion"/> + 1.
		/// </summary>
		/// <param name="sourceVersion">The current version to upgrade from.</param>
		/// <returns>
		/// true if the upgrade was successful, false otherwise.
		/// Details can be obtained from the log.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The default implementation calls <see cref="IDatabaseVersionUpgrader.Upgrade"/>.
		/// </para>
		/// <para>
		/// <see cref="UpgradeImpl"/> might be called multiple times for a single upgrade operation as the upgrading is performed incrementally, calling <see cref="UpgradeImpl"/> once for each version increment.
		/// </para>
		/// </remarks>
		protected virtual bool UpgradeImpl (int sourceVersion)
		{
			this.Log(LogLevel.Information, "Performing database upgrade: {0}->{1}: {2}", sourceVersion, sourceVersion + 1, this.DebugDetails);
			return this.Configuration.VersionUpgrader.Upgrade(this, sourceVersion);
		}

		/// <summary>
		/// Performs a backup.
		/// </summary>
		/// <param name="backupTarget">The backup creator specific object which describes the backup target.</param>
		/// <returns>
		/// true if the backup was successful, false otherwise.
		/// Details can be obtained from the log.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The default implementation calls <see cref="IDatabaseBackupCreator.Backup"/>.
		/// </para>
		/// </remarks>
		protected virtual bool BackupImpl (object backupTarget)
		{
			this.Log(LogLevel.Information, "Performing database backup: [{0}]: {1}", backupTarget, this.DebugDetails);
			return this.Configuration.BackupCreator.Backup(this, backupTarget);
		}

		/// <summary>
		/// Performs a restore.
		/// </summary>
		/// <param name="backupSource">The backup creator specific object which describes the backup source.</param>
		/// <returns>
		/// true if the restore was successful, false otherwise.
		/// Details can be obtained from the log.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The default implementation calls <see cref="IDatabaseBackupCreator.Restore"/>.
		/// </para>
		/// </remarks>
		protected virtual bool RestoreImpl (object backupSource)
		{
			this.Log(LogLevel.Information, "Performing database restore: [{0}]: {1}", backupSource, this.DebugDetails);
			return this.Configuration.BackupCreator.Restore(this, backupSource);
		}

		/// <summary>
		/// Performs a cleanup.
		/// </summary>
		/// <returns>
		/// true if the cleanup was successful, false otherwise.
		/// Details can be obtained from the log.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The default implementation calls <see cref="IDatabaseCleanupProcessor.Cleanup"/>.
		/// </para>
		/// </remarks>
		protected virtual bool CleanupImpl ()
		{
			this.Log(LogLevel.Information, "Performing database cleanup: {0}", this.DebugDetails);
			return this.Configuration.CleanupProcessor.Cleanup(this);
		}





		/// <inheritdoc />
		public void Initialize ()
		{
			this.Log(LogLevel.Information, "Initializing database: {0}", this.DebugDetails);

			if (this.State != DatabaseState.Uninitialized)
			{
				this.Close();
			}

			GC.ReRegisterForFinalize(this);

			this.PrepareConfiguration();

			this.InitializeImpl();

			this.DetectStateAndVersion();

			this.InitialState = this.State;
			this.InitialVersion = this.Version;
		}

		/// <summary>
		///     Disposes this database manager and frees all resources.
		/// </summary>
		/// <param name="disposing"> true if called from <see cref="IDisposable.Dispose" /> or <see cref="Close"/>, false if called from the destructor. </param>
		protected void Dispose (bool disposing)
		{
			this.Log(LogLevel.Information, "Closing database: {0}", this.DebugDetails);

			this.DisposeImpl(disposing);

			this.CloseTrackedConnections();

			this.InitialState = DatabaseState.Uninitialized;
			this.InitialVersion = -1;

			this.SetStateAndVersion(DatabaseState.Uninitialized, -1);
		}

		/// <inheritdoc />
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

			if (connection != null)
			{
				if (track)
				{
					this.TrackedConnections.Add(connection);
					connection.StateChange += this.ConnectionStateChangedHandler;
				}

				this.OnConnectionCreated(connection, readOnly, track);
			}

			return connection;
		}

		/// <inheritdoc />
		public List<string> GetScriptBatch(string name, bool preprocess)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			if (!this.SupportsScripts)
			{
				throw new NotSupportedException(this.GetType().Name + " does not support script retrieval.");
			}

			this.PrepareConfiguration();

			List<string> result = this.GetScriptBatchImpl(name, preprocess);

			if (result != null)
			{
				this.OnScriptRetrieved(name, preprocess, result);
			}

			return result;
		}

		/// <inheritdoc />
		public bool Upgrade (int version)
		{
			if ((!this.IsReady) && (this.State != DatabaseState.New))
			{
				throw new InvalidOperationException(this.GetType().Name + " must be in a ready state or the new state to perform an upgrade, current state is " + this.State + ".");
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
				return true;
			}

			this.PrepareConfiguration();

			int currentVersion = this.Version;
			while ( currentVersion < version)
			{
				bool result = this.UpgradeImpl(currentVersion);

				this.DetectStateAndVersion();
				currentVersion = this.Version;

				if ((!this.IsReady) || (!result))
				{
					return false;
				}
			}

			return true;
		}

		/// <inheritdoc />
		public bool Upgrade ()
		{
			return this.Upgrade(this.MaxVersion);
		}

		/// <inheritdoc />
		public bool Backup (object backupTarget)
		{
			if (backupTarget == null)
			{
				throw new ArgumentNullException(nameof(backupTarget));
			}

			if (this.State == DatabaseState.Uninitialized)
			{
				throw new InvalidOperationException(this.GetType().Name + " must be initialized to perform a backup, current state is " + this.State + ".");
			}

			if (!this.SupportsBackup)
			{
				throw new NotSupportedException(this.GetType().Name + " does not support backups.");
			}

			this.PrepareConfiguration();

			bool result = this.BackupImpl(backupTarget);

			this.DetectStateAndVersion();

			return result;
		}

		/// <inheritdoc />
		public bool Restore (object backupSource)
		{
			if (backupSource == null)
			{
				throw new ArgumentNullException(nameof(backupSource));
			}

			if (this.State == DatabaseState.Uninitialized)
			{
				throw new InvalidOperationException(this.GetType().Name + " must be initialized to perform a restore, current state is " + this.State + ".");
			}

			if (!this.SupportsRestore)
			{
				throw new NotSupportedException(this.GetType().Name + " does not support restores.");
			}

			this.PrepareConfiguration();

			bool result = this.RestoreImpl(backupSource);

			this.DetectStateAndVersion();

			return result;
		}

		/// <inheritdoc />
		public bool Cleanup ()
		{
			if ((!this.IsReady) && (this.State != DatabaseState.New))
			{
				throw new InvalidOperationException(this.GetType().Name + " must be in a ready state or the new state to perform a cleanup, current state is " + this.State + ".");
			}

			if (!this.SupportsCleanup)
			{
				throw new NotSupportedException(this.GetType().Name + " does not support cleanups.");
			}

			this.PrepareConfiguration();

			bool result = this.CleanupImpl();

			this.DetectStateAndVersion();

			return result;
		}
	}
}
