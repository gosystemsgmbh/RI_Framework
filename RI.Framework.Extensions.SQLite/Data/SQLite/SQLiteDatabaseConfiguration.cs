using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Data.Database;
using RI.Framework.IO.Paths;




namespace RI.Framework.Data.SQLite
{
	/// <summary>
	///     Stores the database configuration for an SQLite database.
	/// </summary>
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public sealed class SQLiteDatabaseConfiguration : DatabaseConfiguration<SQLiteDatabaseConfiguration>
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="SQLiteDatabaseConfiguration" />.
		/// </summary>
		public SQLiteDatabaseConfiguration ()
		{
			this.ConnectionString = new SQLiteConnectionStringBuilder();
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the connection string builder.
		/// </summary>
		/// <value>
		///     The connection string builder.
		/// </value>
		public SQLiteConnectionStringBuilder ConnectionString { get; private set; }

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
				return this.ConnectionString.DataSource;
			}
			set
			{
				this.ConnectionString.DataSource = value;
			}
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected override void Clone (SQLiteDatabaseConfiguration clone)
		{
			base.Clone(clone);

			clone.ConnectionString = new SQLiteConnectionStringBuilder(this.ConnectionString.ConnectionString);
		}

		#endregion
	}
}
