using System;
using System.Data.SQLite;

using RI.Framework.IO.Paths;




namespace RI.Framework.Data.SQLite
{
	/// <summary>
	///     Defines the interface for SQLite file version detectors.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="ISQLiteFileVersionDetector" />s are used to detect the database version of an SQLite file.
	///         See <see cref="SQLiteFile" />.<see cref="SQLiteFile.Version" /> for more information.
	///     </para>
	/// </remarks>
	/// TODO: Provide logging
	public interface ISQLiteFileVersionDetector
	{
		/// <summary>
		///     Tries to detect the database version.
		/// </summary>
		/// <param name="temporaryConnection"> The temporary open connection to the database. </param>
		/// <param name="newFile"> Indicates whether it is a newly created SQLite file (true) or it already existed (false). </param>
		/// <param name="file"> The path to the used SQLite file. </param>
		/// <returns>
		///     The detected database version or null if the database version cannot be detected.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="temporaryConnection" /> or <paramref name="file" /> is null. </exception>
		int? DetectVersion (SQLiteConnection temporaryConnection, bool newFile, FilePath file);
	}
}
