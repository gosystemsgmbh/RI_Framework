using System;
using System.Data.Common;

using RI.Framework.Data.Database.Scripts;




namespace RI.Framework.Data.Database.Upgrading
{
	/// <summary>
	///     Defines the interface for a database upgrade provider.
	/// </summary>
	public interface IDatabaseUpgradeProvider
	{
		/// <summary>
		///     Gets the maximum version this upgrade provider can upgrade to.
		/// </summary>
		/// <returns>
		///     The maximum version this upgrade provider can upgrade to.
		/// </returns>
		int GetMaxVersion ();

		/// <summary>
		///     Gets the minimum version this upgrade provider can upgrade from.
		/// </summary>
		/// <returns>
		///     The minimum version this upgrade provider can upgrade from.
		/// </returns>
		int GetMinVersion ();

		/// <summary>
		///     Performs an upgrade on the database.
		/// </summary>
		/// <param name="sourceVersion"> The source version to update from. </param>
		/// <param name="connection"> The connection to the database. </param>
		/// <param name="transaction"> The active transaction (can be null if no transaction is used). </param>
		/// <param name="scriptLocator"> The script provider used to retrieve database scripts. </param>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="sourceVersion" /> is less than zero. </exception>
		/// <exception cref="NotSupportedException"> <paramref name="sourceVersion" /> is an unsupported version (too low or too high). </exception>
		/// <exception cref="ArgumentNullException"> <paramref name="connection" /> or <paramref name="scriptLocator" /> is null. </exception>
		void Upgrade (int sourceVersion, DbConnection connection, DbTransaction transaction, IDatabaseScriptLocator scriptLocator);
	}
}
