using System;




namespace RI.Framework.StateMachines
{
	/// <summary>
	///     Contains all information about a signal.
	/// </summary>
	public sealed class StateSignalInfo
	{
		#region Instance Constructor/Destructor

		internal StateSignalInfo (StateMachine stateMachine)
		{
			if (stateMachine == null)
			{
				throw new ArgumentNullException(nameof(stateMachine));
			}

			this.StateMachine = stateMachine;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the signal.
		/// </summary>
		/// <value>
		///     The signal.
		/// </value>
		public object Signal { get; internal set; }

		/// <summary>
		///     Gets the state machine associated with the signal.
		/// </summary>
		/// <value>
		///     The state machine associated with the signal.
		/// </value>
		public StateMachine StateMachine { get; private set; }

		#endregion
	}
}
