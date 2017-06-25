using System;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.StateMachines.Dispatchers
{
	/// <summary>
	///     Defines the interface for a state machine operation dispatcher.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="StateMachine" /> for more details about state machines.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public interface IStateDispatcher : ISynchronizable
	{
		/// <summary>
		///     Called when a signal needs to be dispatched.
		/// </summary>
		/// <param name="signalDelegate"> The execution callback delegate which is used by the dispatcher to invoke the actual execution. </param>
		/// <param name="signalInfo"> The signal to dispatch. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="signalDelegate" /> or <paramref name="signalInfo" /> is null. </exception>
		void DispatchSignal (StateMachineSignalDelegate signalDelegate, StateSignalInfo signalInfo);

		/// <summary>
		///     Called when a transition needs to be dispatched.
		/// </summary>
		/// <param name="transientDelegate"> The execution callback delegate which is used by the dispatcher to invoke the actual execution. </param>
		/// <param name="transientInfo"> The transition to dispatch. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="transientDelegate" /> or <paramref name="transientInfo" /> is null. </exception>
		void DispatchTransition (StateMachineTransientDelegate transientDelegate, StateTransientInfo transientInfo);

		/// <summary>
		///     Called when an update needs to be dispatched.
		/// </summary>
		/// <param name="updateDelegate"> The execution callback delegate which is used by the dispatcher to invoke the actual execution. </param>
		/// <param name="updateInfo"> The update to dispatch. </param>
		/// <remarks>
		///     <note type="implement">
		///         The actual dispatching must be delayed by the dispatcher implementation as specified in <see cref="StateUpdateInfo.UpdateDelay" />.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="updateDelegate" /> or <paramref name="updateInfo" /> is null. </exception>
		void DispatchUpdate (StateMachineUpdateDelegate updateDelegate, StateUpdateInfo updateInfo);
	}
}
