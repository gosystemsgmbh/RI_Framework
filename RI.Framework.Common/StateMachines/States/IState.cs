using RI.Framework.Composition.Model;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.StateMachines.States
{
	/// <summary>
	///     Defines the interface for a state instance.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="StateMachine" /> for more details about state machines.
	///     </para>
	/// <note type="important">
	/// Some methods are called by <see cref="StateMachine"/> while in a lock to its <see cref="StateMachine.SyncRoot"/>.
	/// Be caruful in implementing classes when calling outside code from those methods (e.g. through events, callbacks, or other virtual methods) to not produce deadlocks!
	/// </note>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public interface IState : ISynchronizable
	{
		/// <summary>
		///     Gets whether the state is initialized or not.
		/// </summary>
		/// <value>
		///     true if the state is initialized, false otherwise.
		/// </value>
		bool IsInitialized { get; }

		/// <summary>
		///     Gets the update interval in milliseconds which defines the time between two calls to <see cref="Update" />.
		/// </summary>
		/// <value>
		///     The update interval in milliseconds or null if <see cref="Update" /> should not be called periodically.
		/// </value>
		int? UpdateInterval { get; }

		/// <summary>
		///     Gets whether the state is cacheable or can be reused respectively.
		/// </summary>
		/// <value>
		///     true if the state is cacheable, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         If <see cref="UseCaching" /> is false, the state is never cached, regardless of the state machine configurations <see cref="StateMachineConfiguration.CachingEnabled" /> setting.
		///     </para>
		/// </remarks>
		bool UseCaching { get; }

		/// <summary>
		///     Called by <see cref="StateMachine" /> when the state becomes the new current state.
		/// </summary>
		/// <param name="transientInfo"> The transition being executed. </param>
		/// <remarks>
		/// <note type="important">
		/// This method is called by <see cref="StateMachine"/> while in a lock to its <see cref="StateMachine.SyncRoot"/>.
		/// </note>
		/// </remarks>
		void Enter (StateTransientInfo transientInfo);

		/// <summary>
		///     Initializes the state.
		/// </summary>
		/// <param name="stateMachine"> The state machine which initializes the state. </param>
		/// <remarks>
		///     <para>
		///         <see cref="Initialize" /> is called during the transition for a next state, before the previous state is left, but only if <see cref="IsInitialized" /> is false.
		///     </para>
		/// <note type="important">
		/// This method is called by <see cref="StateMachine"/> while in a lock to its <see cref="StateMachine.SyncRoot"/>.
		/// </note>
		/// </remarks>
		void Initialize (StateMachine stateMachine);

		/// <summary>
		///     Called by <see cref="StateMachine" /> when the state is left and is no longer the current state.
		/// </summary>
		/// <param name="transientInfo"> The transition being executed. </param>
		/// <remarks>
		/// <note type="important">
		/// This method is called by <see cref="StateMachine"/> while in a lock to its <see cref="StateMachine.SyncRoot"/>.
		/// </note>
		/// </remarks>
		void Leave (StateTransientInfo transientInfo);

		/// <summary>
		///     Called by <see cref="StateMachine" /> when the state is the current state and receives a signal.
		/// </summary>
		/// <param name="signalInfo"> The signal being executed. </param>
		/// <remarks>
		/// <note type="important">
		/// This method is called by <see cref="StateMachine"/> while in a lock to its <see cref="StateMachine.SyncRoot"/>.
		/// </note>
		/// </remarks>
		void Signal (StateSignalInfo signalInfo);

		/// <summary>
		///     Periodically called by <see cref="StateMachine" /> to update the state or when <see cref="StateMachine" />.<see cref="StateMachine.Update" /> is called.
		/// </summary>
		/// <param name="updateInfo"> The update information. </param>
		/// <remarks>
		///     <para>
		///         See <see cref="UpdateInterval" /> for more details.
		///     </para>
		/// <note type="important">
		/// This method is called by <see cref="StateMachine"/> while in a lock to its <see cref="StateMachine.SyncRoot"/>.
		/// </note>
		/// </remarks>
		void Update (StateUpdateInfo updateInfo);
	}
}
