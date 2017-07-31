using System;
using System.Data.Common;

using RI.Framework.Data.Database.Scripts;




namespace RI.Framework.Data.Database.Versioning
{
	/// <summary>
	///     Defines the interface for a database version detector.
	/// </summary>
	public interface IDatabaseVersionDetector
	{
		/// <summary>
		///     Detects the current version of a database.
		/// </summary>
		/// <param name="connection"> The connection to the database. </param>
		/// <param name="scriptLocator"> The script provider used to retrieve database scripts. </param>
		/// <returns>
		///     The current version of the database or -1 if the version could not be determined.
		///     A version of 0 means the database needs to be initialized (e.g. creating tables, indices, etc.).
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="connection" /> or <paramref name="scriptLocator" /> is null. </exception>
		int DetectCurrentVersion (DbConnection connection, IDatabaseScriptLocator scriptLocator);
	}
}
