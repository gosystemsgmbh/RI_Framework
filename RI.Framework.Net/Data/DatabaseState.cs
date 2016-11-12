using System;




namespace RI.Framework.Data
{
	/// <summary>
	/// Describes the current state of an <see cref="IDatabaseManager"/>.
	/// </summary>
	[Serializable]
	public enum DatabaseState
	{
		/// <summary>
		/// The database is not initialized.
		/// </summary>
		Uninitialized = 0,

		/// <summary>
		/// The database is initialized and ready for use.
		/// </summary>
		Ready = 1,

		/// <summary>
		/// The database is initialized but is not ready for use because it was just created and requires an upgrade.
		/// </summary>
		New = 2,

		/// <summary>
		/// The database is initialized but is not ready for use because its version is old and requires an upgrade.
		/// </summary>
		Old = 3,

		/// <summary>
		/// The database is initialized but is not ready for use because its version is newer than the newest version supported by the database.
		/// </summary>
		TooNew = 4,

		/// <summary>
		/// The database is initialized but is not ready for use because its version is too old and cannot be upgraded.
		/// </summary>
		TooOld = 5,

		/// <summary>
		/// The database is initialized but is not ready for use because it is damaged or invalid.
		/// </summary>
		DamagedOrInvalid = 6,
	}
}