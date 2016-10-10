using System;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using RI.Framework.IO.Paths;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Data.SQLite
{
	/// <summary>
	///     Represents an SQLite database file and provides database file management functionality not provided by ADO.NET or Entity Framework.
	/// </summary>
	/// <remarks>
	///     <note type="note">
	///         An instance of <see cref="SQLiteFile" /> can be reused by changing <see cref="ConnectionStringBuilder" />, <see cref="ConnectionString" />, and/or <see cref="File" />.
	///         However, changes made while the SQLite file is initialized or open will not be automatically applied, closing and re-initializing/re-opening is necessary.
	///     </note>
	///     <note type="important">
	///         <see cref="SQLiteFile" /> is intended to have exactly one long-living connection (<see cref="Connection" />) as long as the SQLite file is open.
	///         Furthermore, it is intended for single-threading use, so <see cref="SQLiteFile" /> and its created connection can only be used from the thread which created the instance of <see cref="SQLiteFile" />.
	///         If the database connection (<see cref="Connection" />) must be used on another thread, use <see cref="SQLiteConnection.Clone" /> to clone the database connection.
	///         However, any cloned connection is not managed nor tracked by <see cref="SQLiteFile" /> and might therefore lead to conflicts if the cloned database connection lives longer than the one managed by <see cref="SQLiteFile" />.
	///     </note>
	/// </remarks>
	public sealed class SQLiteFile : IDisposable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="SQLiteFile" />.
		/// </summary>
		public SQLiteFile ()
			: this(new SQLiteConnectionStringBuilder())
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="SQLiteFile" />.
		/// </summary>
		/// <param name="file"> The path to the database file. </param>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="file" /> is not null and contains wildcards. </exception>
		public SQLiteFile (FilePath file)
			: this(new SQLiteConnectionStringBuilder
			       {
				       DataSource = file
			       })
		{
			if (file != null)
			{
				if (file.HasWildcards)
				{
					throw new InvalidPathArgumentException(nameof(file));
				}
			}
		}

		/// <summary>
		///     Creates a new instance of <see cref="SQLiteFile" />.
		/// </summary>
		/// <param name="connectionString"> The database connection string. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="connectionString" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="connectionString" /> is an empty string. </exception>
		public SQLiteFile (string connectionString)
			: this(new SQLiteConnectionStringBuilder(connectionString))
		{
			if (connectionString == null)
			{
				throw new ArgumentNullException(nameof(connectionString));
			}

			if (connectionString.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(connectionString));
			}
		}

		/// <summary>
		///     Creates a new instance of <see cref="SQLiteFile" />.
		/// </summary>
		/// <param name="connectionStringBuilder"> The database connection string builder. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="connectionStringBuilder" /> is null. </exception>
		public SQLiteFile (SQLiteConnectionStringBuilder connectionStringBuilder)
		{
			if (connectionStringBuilder == null)
			{
				throw new ArgumentNullException(nameof(connectionStringBuilder));
			}

			this.ConnectionStringBuilder = connectionStringBuilder;

			this.State = SQLiteFileState.Closed;
			this.UsedConnectionString = null;
			this.Connection = null;
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="SQLiteFile" />.
		/// </summary>
		~SQLiteFile ()
		{
			this.Dispose(false);
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the current database connection.
		/// </summary>
		/// <value>
		///     The current database connection or null if the SQLite file is not open.
		/// </value>
		public SQLiteConnection Connection { get; private set; }

		/// <summary>
		///     Gets or sets the current database connection string.
		/// </summary>
		/// <value>
		///     The current database connection string.
		/// </value>
		public string ConnectionString
		{
			get
			{
				return this.ConnectionStringBuilder.ConnectionString;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				if (value.IsEmpty())
				{
					throw new EmptyStringArgumentException(nameof(value));
				}

				this.ConnectionStringBuilder.ConnectionString = value;
			}
		}

		/// <summary>
		///     Gets the current database connection string builder.
		/// </summary>
		/// <value>
		///     The current database connection string builder.
		/// </value>
		public SQLiteConnectionStringBuilder ConnectionStringBuilder { get; private set; }

		/// <summary>
		///     Gets or sets the current database file.
		/// </summary>
		/// <value>
		///     The current database file.
		/// </value>
		public FilePath File
		{
			get
			{
				string dataSource = this.ConnectionStringBuilder.DataSource;
				return dataSource == null ? null : new FilePath(dataSource);
			}
			set
			{
				if (value != null)
				{
					if (value.HasWildcards)
					{
						throw new InvalidPathArgumentException(nameof(value));
					}
				}

				this.ConnectionStringBuilder.DataSource = value;
			}
		}

		/// <summary>
		///     Gets the current state of the SQLite file.
		/// </summary>
		/// <value>
		///     The current state of the SQLite file.
		/// </value>
		public SQLiteFileState State { get; private set; }

		private string UsedConnectionString { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Closes the database connection and the SQLite file.
		/// </summary>
		public void Close ()
		{
			this.Dispose(true);
		}

		/// <summary>
		///     Initializes the SQLite file.
		/// </summary>
		public void Initialize ()
		{
			if (this.State != SQLiteFileState.Closed)
			{
				throw new InvalidOperationException("SQLite file not closed.");
			}

			this.UsedConnectionString = this.ConnectionString;
			this.Log("Initializing SQLite file: {0}", this.UsedConnectionString);

			if (this.File.CreateIfNotExist())
			{
				this.Log("Created SQLite file: {0}", this.File);
			}

			this.Connection = null;

			this.State = SQLiteFileState.Initialized;
		}

		/// <summary>
		///     Opens the SQLite file and the database connection.
		/// </summary>
		public void Open ()
		{
			if (this.State != SQLiteFileState.Initialized)
			{
				throw new InvalidOperationException("SQLite file not initialized.");
			}

			if (!this.File.Exists)
			{
				throw new FileNotFoundException("SQLite file not found.", this.File);
			}

			this.Log("Opening SQLite file: {0}", this.UsedConnectionString);

			this.Connection = new SQLiteConnection(this.UsedConnectionString);

			this.State = SQLiteFileState.Open;
		}

		[SuppressMessage ("ReSharper", "UnusedParameter.Local")]
		private void Dispose (bool disposing)
		{
			this.State = SQLiteFileState.Closed;

			if (this.Connection != null)
			{
				this.Log("Closing SQLite file: {0}", this.UsedConnectionString);

				this.Connection.Close();
				this.Connection = null;
			}

			this.UsedConnectionString = null;
		}

		private void Log (string format, params object[] args)
		{
			LogLocator.LogDebug(nameof(SQLiteFile), format, args);
		}

		#endregion




		#region Interface: IDisposable

		/// <inheritdoc />
		void IDisposable.Dispose ()
		{
			this.Close();
		}

		#endregion
	}
}
