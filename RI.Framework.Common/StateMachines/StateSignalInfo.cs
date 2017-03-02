using System;




namespace RI.Framework.StateMachines
{
	/// <summary>
	/// Contains all information about a signal
	/// </summary>
	public sealed class StateSignalInfo
	{
		internal StateSignalInfo (StateMachine stateMachine)
		{
			if (stateMachine == null)
			{
				throw new ArgumentNullException(nameof(stateMachine));
			}

			this.StateMachine = stateMachine;
		}

		/// <summary>
		/// Gets the state machine associated with the signal.
		/// </summary>
		/// <value>
		/// The state machine associated with the signal.
		/// </value>
		public StateMachine StateMachine { get; private set; }

		/// <summary>
		/// Gets the signal.
		/// </summary>
		/// <value>
		/// The signal.
		/// </value>
		public object Signal { get; internal set; }
	}
}