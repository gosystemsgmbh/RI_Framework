using System;




namespace RI.Framework.Data.Database
{
	/// <summary>
	///     Event arguments for the <see cref="IDatabaseManager" />.<see cref="IDatabaseManager.StateChanged" /> event.
	/// </summary>
	[Serializable]
	public sealed class DatabaseStateChangedEventArgs : EventArgs
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DatabaseStateChangedEventArgs" />.
		/// </summary>
		/// <param name="oldState"> The old state of the database. </param>
		/// <param name="newState"> The new state of the database. </param>
		public DatabaseStateChangedEventArgs (DatabaseState oldState, DatabaseState newState)
		{
			this.OldState = oldState;
			this.NewState = newState;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the new state of the database.
		/// </summary>
		/// <value>
		///     The new state of the database.
		/// </value>
		public DatabaseState NewState { get; private set; }


		/// <summary>
		///     Gets the old state of the database.
		/// </summary>
		/// <value>
		///     The old state of the database.
		/// </value>
		public DatabaseState OldState { get; private set; }

		#endregion
	}
}
