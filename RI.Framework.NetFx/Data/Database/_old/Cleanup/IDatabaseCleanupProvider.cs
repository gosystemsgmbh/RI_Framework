using System;
using System.Data.Common;

using RI.Framework.Data.Database.Scripts;




namespace RI.Framework.Data.Database.Cleanup
{
	/// <summary>
	///     Defines the interface for a database cleanup provider.
	/// </summary>
	public interface IDatabaseCleanupProvider
	{
		/// <summary>
		///     Performs a cleanup on the database.
		/// </summary>
		/// <param name="currentVersion"> The current version of the database. </param>
		/// <param name="connection"> The connection to the database. </param>
		/// <param name="transaction"> The active transaction (can be null if no transaction is used). </param>
		/// <param name="scriptLocator"> The script provider used to retrieve database scripts. </param>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="currentVersion" /> is less than zero. </exception>
		/// <exception cref="NotSupportedException"> <paramref name="currentVersion" /> is an unsupported version (too low or too high). </exception>
		/// <exception cref="ArgumentNullException"> <paramref name="connection" /> or <paramref name="scriptLocator" /> is null. </exception>
		void Cleanup (int currentVersion, DbConnection connection, DbTransaction transaction, IDatabaseScriptLocator scriptLocator);
	}
}
