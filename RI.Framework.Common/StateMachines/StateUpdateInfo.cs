using System;

using RI.Framework.StateMachines.States;




namespace RI.Framework.StateMachines
{
	/// <summary>
	///     Contains all information about an update.
	/// </summary>
	public sealed class StateUpdateInfo
	{
		#region Instance Constructor/Destructor

		internal StateUpdateInfo (StateMachine stateMachine)
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
		///     Gets the local timestamp when the current state became active.
		/// </summary>
		/// <value>
		///     The local timestamp when the current state became active.
		/// </value>
		public DateTime StateEnteredLocal { get; internal set; }

		/// <summary>
		///     Gets the UTC timestamp when the current state became active.
		/// </summary>
		/// <value>
		///     The UTC timestamp when the current state became active.
		/// </value>
		public DateTime StateEnteredUtc { get; internal set; }

		/// <summary>
		///     Gets the state machine associated with the update.
		/// </summary>
		/// <value>
		///     The state machine associated with the update.
		/// </value>
		public StateMachine StateMachine { get; private set; }

		/// <summary>
		///     Gets the delay of the update in milliseconds.
		/// </summary>
		/// <value>
		///     The delay of the update in milliseconds.
		///     Zero if the update is to be executed immediately.
		/// </value>
		public int UpdateDelay { get; internal set; }

		/// <summary>
		///     Gets the state for which this update is intented.
		/// </summary>
		/// <value>
		///     The state for which this update is intented.
		/// </value>
		public IState UpdateState { get; internal set; }

		#endregion
	}
}
