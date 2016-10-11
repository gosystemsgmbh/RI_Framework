using System;
using System.Data.SQLite;




namespace RI.Framework.Data.SQLite
{
	/// <summary>
	///     Defines the interface for an SQLite file migration step.
	/// </summary>
	/// <remarks>
	///     <para>
	///         An SQLite file migration step is used to update the database version from exactly one source version () to its next version (that is, the source version plus one).
	///     </para>
	///     <para>
	///         Multiple SQLite file migration steps are combined in an <see cref="SQLiteFileMigrationChain" /> before executed by <see cref="SQLiteFile" />.<see cref="SQLiteFile.Migrate" />.
	///     </para>
	/// </remarks>
	/// TODO: Provide logging
	public interface ISQLiteFileMigrationStep
	{
		/// <summary>
		///     Gets the source version supported by this migration step.
		/// </summary>
		/// <value>
		///     The source version supported by this migration step.
		/// </value>
		int SourceVersion { get; }

		/// <summary>
		///     Executes this migration step on a given temporary database connection.
		/// </summary>
		/// <param name="temporaryConnection"> The temporary open connection to the database. </param>
		/// <returns>
		///     true if this migration step executed successfully, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="temporaryConnection" /> is null. </exception>
		bool ExecuteStep (SQLiteConnection temporaryConnection);
	}
}
