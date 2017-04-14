using System;




namespace RI.Framework.StateMachines
{
	/// <summary>
	///     Event arguments for state machine events related to transitions.
	/// </summary>
	[Serializable]
	public sealed class StateMachineTransientEventArgs : EventArgs
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="StateMachineTransientEventArgs" />.
		/// </summary>
		/// <param name="transientInfo"> Information about the processed transition. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="transientInfo" /> is null. </exception>
		public StateMachineTransientEventArgs (StateTransientInfo transientInfo)
		{
			if (transientInfo == null)
			{
				throw new ArgumentNullException(nameof(transientInfo));
			}

			this.TransientInfo = transientInfo;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets information about the processed transition.
		/// </summary>
		/// <value>
		///     Information about the processed transition.
		/// </value>
		public StateTransientInfo TransientInfo { get; private set; }

		#endregion
	}
}
