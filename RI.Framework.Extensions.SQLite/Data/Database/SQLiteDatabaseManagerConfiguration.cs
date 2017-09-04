using System;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.IO.Paths;

namespace RI.Framework.Data.Database
{
	/// <summary>
	///     Stores the database configuration for an SQLite database manager.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IDatabaseManagerConfiguration" /> for more details.
	///     </para>
	/// </remarks>
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public sealed class SQLiteDatabaseManagerConfiguration : DatabaseManagerConfiguration<SQLiteConnection, SQLiteTransaction, SQLiteConnectionStringBuilder, SQLiteDatabaseManager, SQLiteDatabaseManagerConfiguration>
	{
		/// <summary>
		/// Creates a new instance of <see cref="SQLiteDatabaseManagerConfiguration"/>.
		/// </summary>
		public SQLiteDatabaseManagerConfiguration ()
		{
			this.ConnectionString = new SQLiteConnectionStringBuilder();
		}

		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets the path to the database file.
		/// </summary>
		/// <value>
		///     The path to the database file.
		/// </value>
		public FilePath DatabaseFile
		{
			get
			{
				return this.ConnectionString?.DataSource;
			}
			set
			{
				if (this.ConnectionString != null)
				{
					this.ConnectionString.DataSource = value;
				}
			}
		}

		/// <inheritdoc />
		protected override void VerifyConfiguration (SQLiteDatabaseManager manager)
		{
			base.VerifyConfiguration(manager);

			try
			{
				FilePath filePath = new FilePath(this.ConnectionString.DataSource);
				filePath.VerifyRealFile();
			}
			catch (Exception exception)
			{
				throw new InvalidDatabaseConfigurationException("Invalid database file path: " + (this.ConnectionString.DataSource ?? "[null]") + ".", exception);
			}
		}

		/// <summary>
		/// Gets or sets whether default functions are automatically registered with newly created connections.
		/// </summary>
		/// <value>
		/// true if default functions are automatically registered with newly created connections, false otherwise.
		/// </value>
		public bool RegisterDefaultFunctions { get; set; } = true;

		/// <summary>
		/// Gets or sets whether default collation are automatically registered with newly created connections.
		/// </summary>
		/// <value>
		/// true if default collations are automatically registered with newly created connections, false otherwise.
		/// </value>
		public bool RegisterDefaultCollations { get; set; } = true;

		#endregion
	}
}
