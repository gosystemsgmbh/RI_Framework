using System;




namespace RI.Framework.Data.SQLite
{
	/// <summary>
	///     Describes the state of an <see cref="SQLiteFile" />.
	/// </summary>
	[Serializable]
	public enum SQLiteFileState
	{
		/// <summary>
		///     The SQLite file is closed.
		/// </summary>
		/// <remarks>
		///     >
		///     <para>
		///         The connection is not available.
		///     </para>
		/// </remarks>
		Closed = 0,

		/// <summary>
		///     The SQLite file is initialized.
		/// </summary>
		/// <remarks>
		///     >
		///     <para>
		///         The connection is not available.
		///     </para>
		/// </remarks>
		Initialized = 1,

		/// <summary>
		///     The SQLite file is open.
		/// </summary>
		/// <remarks>
		///     >
		///     <para>
		///         The connection is available.
		///     </para>
		/// </remarks>
		Open = 2,
	}
}
