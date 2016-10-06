using System;
using System.Data.SQLite;

using RI.Framework.IO.Paths;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Data.SQLite
{
	/// <summary>
	///     Manages all aspects of an SQLite database.
	/// </summary>
	/// <remarks>
	///     <para>
	///         An <see cref="SQLiteDatabaseManager" /> allows centralized management of all aspects of an SQLite database and its database file.
	///         This includes management of the database file itself (as a file in the file system) before or after any actual database connections are established.
	///         Therefore, <see cref="SQLiteDatabaseManager" /> is used for managing aspects and performing common taks of an SQLite database and its file which are not provided by ADO.NET or Entity Framework.
	///     </para>
	/// </remarks>
	public sealed class SQLiteDatabaseManager
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="SQLiteDatabaseManager" />.
		/// </summary>
		/// <param name="databaseFile"> The database file to use. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="databaseFile" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="databaseFile" />  has wildcards. </exception>
		public SQLiteDatabaseManager (FilePath databaseFile)
		{
			if (databaseFile == null)
			{
				throw new ArgumentNullException(nameof(databaseFile));
			}

			if (databaseFile.HasWildcards)
			{
				throw new InvalidPathArgumentException(nameof(databaseFile));
			}

			this.ConnectionString = new SQLiteConnectionStringBuilder();
			this.ConnectionString.DataSource = databaseFile;
		}

		/// <summary>
		///     Creates a new instance of <see cref="SQLiteDatabaseManager" />.
		/// </summary>
		/// <param name="connectionStringBuilder"> The connection string builder which describes database to use. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="connectionStringBuilder" /> is null. </exception>
		public SQLiteDatabaseManager (SQLiteConnectionStringBuilder connectionStringBuilder)
		{
			if (connectionStringBuilder == null)
			{
				throw new ArgumentNullException(nameof(connectionStringBuilder));
			}

			this.ConnectionString = connectionStringBuilder;
		}

		/// <summary>
		///     Creates a new instance of <see cref="SQLiteDatabaseManager" />.
		/// </summary>
		/// <param name="connectionString"> The connection string which describes database to use. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="connectionString" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="connectionString" /> is an empty string. </exception>
		public SQLiteDatabaseManager (string connectionString)
		{
			if (connectionString == null)
			{
				throw new ArgumentNullException(nameof(connectionString));
			}

			if (connectionString.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(connectionString));
			}

			this.ConnectionString = new SQLiteConnectionStringBuilder(connectionString);
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the connection string builder which can be used to configure the connection before a connection is opened.
		/// </summary>
		/// <value>
		///     The connection string builder.
		/// </value>
		/// <remarks>
		///     <para>
		///         Changes to <see cref="ConnectionString" /> are only effective for new connections which are created using <see cref="OpenConnection" />.
		///     </para>
		/// </remarks>
		public SQLiteConnectionStringBuilder ConnectionString { get; private set; }

		/// <summary>
		///     Gets the database file to use.
		/// </summary>
		/// <remarks>
		///     The database file to use.
		/// </remarks>
		public FilePath DatabaseFile
		{
			get
			{
				return this.ConnectionString.DataSource;
			}
		}

		#endregion




		#region Instance Methods

		/// <summary>
		///     Creates a new connection to the database.
		/// </summary>
		/// <returns>
		///     The new connection to the database.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The returned database connection is not automatically opened.
		///     </para>
		///     <para>
		///         This method can be used to create database connections for multiple threads.
		///         Note that a database connection can only be used by the thread which created the connection.
		///     </para>
		/// </remarks>
		public SQLiteConnection CreateConnection ()
		{
			this.InitializeFile();

			string connectionString = this.ConnectionString.ToString();

			this.Log(LogLevel.Debug, "Creating database connection: {0}", connectionString);

			return new SQLiteConnection(connectionString);
		}

		public void CreateCopy (FilePath file)
		{
			if (file == null)
			{
				throw new ArgumentNullException(nameof(file));
			}

			if (file.HasWildcards)
			{
				throw new InvalidPathArgumentException(nameof(file));
			}

			this.InitializeFile();
		}

		public void MigrateSelf ()
		{
		}

		public void MigrateTo (FilePath file)
		{
		}

		private void InitializeFile ()
		{
			if (this.DatabaseFile.CreateIfNotExist())
			{
				this.Log(LogLevel.Information, "Database file created because it did not exist: {0}", this.DatabaseFile);
			}
		}

		private void Log (LogLevel severity, string format, params object[] args)
		{
			LogLocator.Log(severity, this.GetType().Name, format, args);
		}

		#endregion
	}
}
