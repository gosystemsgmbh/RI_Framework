using System;




namespace RI.Framework.Data.Database
{
	/// <summary>
	///     Event arguments for database scripts which are executed by an implementation of <see cref="IDatabaseManager" />.
	/// </summary>
	[Serializable]
	public class DatabaseScriptEventArgs : EventArgs
	{
		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets the database script.
		/// </summary>
		/// <value>
		///     The database script.
		/// </value>
		/// <remarks>
		///     <para>
		///         If a value of null or an empty string is set during preparation, the execution of the script is aborted.
		///     </para>
		/// </remarks>
		public string Script { get; set; }

		#endregion
	}
}
