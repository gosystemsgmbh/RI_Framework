using System;
using System.Data.Common;

using RI.Framework.Data.Repository;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Data.Database
{
	/// <summary>
	///     Defines the interface for a database manager.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A database manager encapsulates low-level database functionality so that high-level database functionality (such as <see cref="IRepositoryContext" />, querying, ORM-ing, etc.) can be implemented on top without having to worry about low-level and/or database-specific details.
	///     </para>
	///     <para>
	///         The provided low-level database functionalities are:
	///         Initialization (managing the connection string, creating a new database if it does not already exist, and creating connections), Versioning (detecting the database version and validating against known/supported versions), Upgrading (migrating a database version from an old version to the latest known/supported version), Cleanup (cleaning and optimization of the database).
	///     </para>
	/// </remarks>
	/// TODO: SupportsConnectionTracking
	public interface IDatabaseManager : IDisposable
	{
		/// <summary>
		///     Gets or sets the connection string.
		/// </summary>
		/// <value>
		///     The connection string.
		/// </value>
		/// <remarks>
		///     <note type="note">
		///         Changes to the connection string only takes effect when calling <see cref="InitializeDatabase" />.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="value" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="value" /> is an empty string. </exception>
		string ConnectionString { get; set; }

		/// <summary>
		///     Gets the connection string builder.
		/// </summary>
		/// <value>
		///     The connection string builder.
		/// </value>
		/// <remarks>
		///     <note type="note">
		///         Changes to the connection string only takes effect when calling <see cref="InitializeDatabase" />.
		///     </note>
		/// </remarks>
		DbConnectionStringBuilder ConnectionStringBuilder { get; }

		/// <summary>
		///     Gets the current version of the database.
		/// </summary>
		/// <value>
		///     The current version of the database, 0 if the database state is <see cref="DatabaseState.New" />, -1 if the database state is <see cref="DatabaseState.Uninitialized" /> or <see cref="DatabaseState.DamagedOrInvalid" />.
		/// </value>
		int CurrentVersion { get; }

		/// <summary>
		///     Gets the latest known/supported version of the database which can be applied through an upgrade.
		/// </summary>
		/// <value>
		///     The latest known/supported version of the database which can be applied through an upgrade or -1 if the database state is <see cref="DatabaseState.Uninitialized" />
		/// </value>
		int MaxVersion { get; }

		/// <summary>
		///     Gets the earliest known/supported version of the database which can be upgraded from.
		/// </summary>
		/// <value>
		///     The earliest known/supported version of the database which can be upgraded from or -1 if the database state is <see cref="DatabaseState.Uninitialized" />
		/// </value>
		int MinVersion { get; }

		/// <summary>
		///     Gets the current state of the database.
		/// </summary>
		/// <value>
		///     The current state of the database.
		/// </value>
		DatabaseState State { get; }

		/// <summary>
		///     Gets whether the database supportes read-only connections.
		/// </summary>
		/// <value>
		///     true if the database supports read-only connections, false otherwise.
		/// </value>
		bool SupportsReadOnlyConnections { get; }

		/// <summary>
		///     Performs cleaning and optimization of the database.
		/// </summary>
		/// <remarks>
		///     <para>
		///         <see cref="State" /> is set to <see cref="DatabaseState.Ready" /> or <see cref="DatabaseState.DamagedOrInvalid" />.
		///     </para>
		/// </remarks>
		/// <exception cref="InvalidOperationException"> The database is not in the <see cref="DatabaseState.Ready" /> state. </exception>
		void CleanupDatabase ();

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
		DbConnection CreateConnection (bool readOnly);

		/// <summary>
		///     Initializes the database.
		/// </summary>
		/// <remarks>
		///     <para>
		///         <see cref="State" /> is set to <see cref="DatabaseState.Ready" />, <see cref="DatabaseState.New" />, <see cref="DatabaseState.Old" />, <see cref="DatabaseState.TooNew" />, <see cref="DatabaseState.TooOld" />, or <see cref="DatabaseState.DamagedOrInvalid" />.
		///     </para>
		///     <para>
		///         Initialization includes the following:
		///         Creating an initial connection for verification, creating a new database if it does not already exist, and detecting the version of the database.
		///     </para>
		///     <para>
		///         <see cref="InitializeDatabase" /> can be called in any state of the database.
		///         If the database is not in the <see cref="DatabaseState.Uninitialized" /> state, the database is unloaded first.
		///     </para>
		/// </remarks>
		void InitializeDatabase ();

		/// <summary>
		///     Unloads the database.
		/// </summary>
		/// <remarks>
		///     <para>
		///         <see cref="State" /> is set to <see cref="DatabaseState.Uninitialized" />.
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
		///         <see cref="State" /> is set to <see cref="DatabaseState.Ready" /> or <see cref="DatabaseState.DamagedOrInvalid" />.
		///     </para>
		/// </remarks>
		/// <exception cref="InvalidOperationException"> The database is not in the <see cref="DatabaseState.New" /> or <see cref="DatabaseState.Old" /> state. </exception>
		void UpgradeDatabase ();
	}
}
