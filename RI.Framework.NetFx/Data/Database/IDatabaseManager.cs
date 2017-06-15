using System;
using System.Data.Common;

using RI.Framework.Data.Repository;




namespace RI.Framework.Data.Database
{
	/// <summary>
	///     Defines the interface for a database manager.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A database manager encapsulates low-level database functionality so that high-level database functionality (such as <see cref="IRepositoryContext" />) can be implemented on top without having to worry about low-level and/or database-specific details.
	///         A database manager is intended to handle all low-level database stuff, such as connection management and migration, so that the high-level functionality can focus on the actual task of working with the data or accessing the database respectively.
	///     </para>
	///     <para>
	///         The link from the database manager to higher-level functionality are the connections which can be created using <see cref="CreateConnection" />.
	///     </para>
	///     <para>
	///         The database cannot be used if it is in any other state than <see cref="DatabaseState.Ready" />.
	///         However, if the database is in the state <see cref="DatabaseState.New" /> or <see cref="DatabaseState.Old" />, the database can be made ready by upgrading to the latest known/supported version using <see cref="UpgradeDatabase" />.
	///     </para>
	///     <para>
	///         Database upgrades are handled using <see cref="DatabaseMigrationStep" />. See <see cref="DatabaseMigrationStep" /> for more details.
	///     </para>
	///     <para>
	///         If the database supports connection tracking (see <see cref="SupportsConnectionTracking" />), <see cref="UnloadDatabase" /> or <see cref="CloseConnections" /> will close all currently non-closed connections which were created using <see cref="CreateConnection" />.
	///     </para>
	/// </remarks>
	public interface IDatabaseManager : IDisposable
	{
		/// <summary>
		///     Gets the database configuration.
		/// </summary>
		/// <value>
		///     The database configuration.
		/// </value>
		DatabaseConfiguration Configuration { get; }

		/// <summary>
		///     Gets the current state of the database.
		/// </summary>
		/// <value>
		///     The current state of the database.
		/// </value>
		DatabaseState CurrentState { get; }

		/// <summary>
		///     Gets the current version of the database.
		/// </summary>
		/// <value>
		///     The current version of the database or 0 if the database state is <see cref="DatabaseState.Uninitialized" /> or <see cref="DatabaseState.DamagedOrInvalid" />.
		/// </value>
		/// <remarks>
		///     <note type="note">
		///         This property is only set during <see cref="InitializeDatabase" /> and <see cref="UpgradeDatabase" />.
		///     </note>
		/// </remarks>
		int CurrentVersion { get; }

		/// <summary>
		///     Gets the initial state of the database after initialization.
		/// </summary>
		/// <value>
		///     The initial state of the database after initialization.
		/// </value>
		DatabaseState InitialState { get; }

		/// <summary>
		///     Gets the initial version of the database after initialization.
		/// </summary>
		/// <value>
		///     The initial version of the database or 0 if the database state is <see cref="DatabaseState.Uninitialized" /> or <see cref="DatabaseState.DamagedOrInvalid" />.
		/// </value>
		/// <remarks>
		///     <note type="note">
		///         This property is only set during <see cref="InitializeDatabase" />.
		///     </note>
		/// </remarks>
		int InitialVersion { get; }

		/// <summary>
		///     Gets the latest known/supported version of the database which can be applied through an upgrade.
		/// </summary>
		/// <value>
		///     The latest known/supported version of the database which can be applied through an upgrade or 0 if the database state is <see cref="DatabaseState.Uninitialized" />.
		/// </value>
		/// <remarks>
		///     <note type="note">
		///         This property is only set during <see cref="InitializeDatabase" />.
		///     </note>
		/// </remarks>
		int MaxVersion { get; }

		/// <summary>
		///     Gets the earliest known/supported version of the database which can be upgraded from.
		/// </summary>
		/// <value>
		///     The earliest known/supported version of the database which can be upgraded from or 0 if the database state is <see cref="DatabaseState.Uninitialized" />.
		/// </value>
		/// <remarks>
		///     <note type="note">
		///         This property is only set during <see cref="InitializeDatabase" />.
		///     </note>
		/// </remarks>
		int MinVersion { get; }

		/// <summary>
		///     Gets whether the database supports connection tracking.
		/// </summary>
		/// <value>
		///     true if the database supports connection tracking, false otherwise.
		/// </value>
		bool SupportsConnectionTracking { get; }

		/// <summary>
		///     Gets whether the database supportes read-only connections.
		/// </summary>
		/// <value>
		///     true if the database supports read-only connections, false otherwise.
		/// </value>
		bool SupportsReadOnlyConnections { get; }

		/// <summary>
		///     Raised when a new connection to the database has been created using <see cref="CreateConnection" />.
		/// </summary>
		event EventHandler<DatabaseConnectionCreatedEventArgs> ConnectionCreated;

		/// <summary>
		///     Raised when the current state of the database has changed.
		/// </summary>
		event EventHandler<DatabaseStateChangedEventArgs> StateChanged;

		/// <summary>
		///     Performs cleaning and optimization of the database.
		/// </summary>
		/// <remarks>
		///     <para>
		///         <see cref="CurrentState" /> is set to <see cref="DatabaseState.Ready" /> or <see cref="DatabaseState.DamagedOrInvalid" />.
		///     </para>
		/// </remarks>
		/// <exception cref="InvalidOperationException"> The database is not in the <see cref="DatabaseState.Ready" /> state. </exception>
		void CleanupDatabase ();

		/// <summary>
		///     Closes all connections created using <see cref="CreateConnection" />.
		/// </summary>
		/// <exception cref="InvalidOperationException"> The database is not in the <see cref="DatabaseState.Ready" /> state. </exception>
		/// <exception cref="NotSupportedException"> This database does not support connection tracking (see <see cref="SupportsConnectionTracking" />). </exception>
		void CloseConnections ();

		/// <summary>
		///     Creates a connection to the database for general usage.
		/// </summary>
		/// <param name="readOnly"> Specifies whether a read-only connection should be created, if possible. </param>
		/// <returns>
		///     The connection to the database.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The <paramref name="readOnly" /> parameter is only a hint.
		///         If <paramref name="readOnly" /> is true but the database does not support read-only connections, the connection is created anyway.
		///     </para>
		/// </remarks>
		/// <exception cref="InvalidOperationException"> The database is not in the <see cref="DatabaseState.Ready" /> state. </exception>
		/// <exception cref="NotSupportedException"> <paramref name="readOnly" /> is true and this database does not support read-only connections (see <see cref="SupportsReadOnlyConnections" />). </exception>
		DbConnection CreateConnection (bool readOnly);

		/// <summary>
		///     Initializes the database.
		/// </summary>
		/// <remarks>
		///     <para>
		///         <see cref="CurrentState" /> is set to <see cref="DatabaseState.Ready" />, <see cref="DatabaseState.New" />, <see cref="DatabaseState.Old" />, <see cref="DatabaseState.TooNew" />, <see cref="DatabaseState.TooOld" />, or <see cref="DatabaseState.DamagedOrInvalid" />.
		///     </para>
		///     <para>
		///         <see cref="InitializeDatabase" /> can be called in any state of the database.
		///         If the database is not in the <see cref="DatabaseState.Uninitialized" /> state, the database is unloaded first.
		///     </para>
		/// </remarks>
		/// <exception cref="InvalidDatabaseConfigurationException"> The database manager has invalid configuration. </exception>
		void InitializeDatabase ();

		/// <summary>
		///     Unloads the database.
		/// </summary>
		/// <remarks>
		///     <para>
		///         <see cref="CurrentState" /> is set to <see cref="DatabaseState.Uninitialized" />.
		///     </para>
		///     <para>
		///         Nothing happens if the database is already in the <see cref="DatabaseState.Uninitialized" /> state.
		///     </para>
		/// </remarks>
		void UnloadDatabase ();

		/// <summary>
		///     Upgrades the database to the latest known/supported version.
		/// </summary>
		/// <remarks>
		///     <para>
		///         <see cref="CurrentState" /> is set to <see cref="DatabaseState.Ready" /> or <see cref="DatabaseState.DamagedOrInvalid" />.
		///     </para>
		/// </remarks>
		/// <exception cref="InvalidOperationException"> The database is not in the <see cref="DatabaseState.New" /> or <see cref="DatabaseState.Old" /> state. </exception>
		void UpgradeDatabase ();
	}
}
