using System;




namespace RI.Framework.StateMachines
{
	/// <summary>
	///     Event arguments for state machine events related to signals.
	/// </summary>
	[Serializable]
	public sealed class StateMachineSignalEventArgs : EventArgs
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="StateMachineSignalEventArgs" />.
		/// </summary>
		/// <param name="signalInfo"> Information about the processed signal. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="signalInfo" /> is null. </exception>
		public StateMachineSignalEventArgs (StateSignalInfo signalInfo)
		{
			if (signalInfo == null)
			{
				throw new ArgumentNullException(nameof(signalInfo));
			}

			this.SignalInfo = signalInfo;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets information about the processed signal.
		/// </summary>
		/// <value>
		///     Information about the processed signal.
		/// </value>
		public StateSignalInfo SignalInfo { get; }

		#endregion
	}
}
