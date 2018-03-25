using System;




namespace RI.Framework.StateMachines
{
	/// <summary>
	///     Event arguments for state machine events related to updates.
	/// </summary>
	[Serializable]
	public sealed class StateMachineUpdateEventArgs : EventArgs
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="StateMachineUpdateEventArgs" />.
		/// </summary>
		/// <param name="updateInfo"> Information about the processed update. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="updateInfo" /> is null. </exception>
		public StateMachineUpdateEventArgs (StateUpdateInfo updateInfo)
		{
			if (updateInfo == null)
			{
				throw new ArgumentNullException(nameof(updateInfo));
			}

			this.UpdateInfo = updateInfo;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets information about the processed update.
		/// </summary>
		/// <value>
		///     Information about the processed update.
		/// </value>
		public StateUpdateInfo UpdateInfo { get; }

		#endregion
	}
}
