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
	public sealed class SQLiteDatabaseConfiguration : DatabaseConfiguration<SQLiteConnectionStringBuilder, SQLiteDatabaseConfiguration>
	{
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
				return this.ConnectionString.DataSource;
			}
			set
			{
				this.ConnectionString.DataSource = value;
			}
		}

		#endregion
	}
}
