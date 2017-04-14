using RI.Framework.Composition.Model;




namespace RI.Framework.StateMachines
{
	/// <summary>
	///     Defines the interface for a state instance.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="StateMachine" /> for more details about state machines.
	///     </para>
	/// </remarks>
	[Export]
	public interface IState
	{
		/// <summary>
		///     Gets whether the state is initialized or not.
		/// </summary>
		/// <value>
		///     true if the state is initialized, false otherwise.
		/// </value>
		bool IsInitialized { get; }

		/// <summary>
		///     Gets whether the state is cacheable or can be reused respectively.
		/// </summary>
		/// <value>
		///     true if the state is cacheable, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         If <see cref="UseCaching" /> is false, the state is never cached, regardless of the state machine configurations <see cref="StateMachineConfiguration.EnableAutomaticCaching" /> setting.
		///     </para>
		/// </remarks>
		bool UseCaching { get; }

		/// <summary>
		///     Called by <see cref="StateMachine" /> when the state becomes the new current state.
		/// </summary>
		/// <param name="transientInfo"> The transition being executed. </param>
		void Enter (StateTransientInfo transientInfo);

		/// <summary>
		///     Initializes the state.
		/// </summary>
		/// <param name="stateMachine"> The state machine which initializes the state. </param>
		/// <remarks>
		///     <para>
		///         <see cref="Initialize" /> is called during the transition for a next state, before the previous state is left, but only if <see cref="IsInitialized" /> is false.
		///     </para>
		/// </remarks>
		void Initialize (StateMachine stateMachine);

		/// <summary>
		///     Called by <see cref="StateMachine" /> when the state is left and is no longer the current state.
		/// </summary>
		/// <param name="transientInfo"> The transition being executed. </param>
		void Leave (StateTransientInfo transientInfo);

		/// <summary>
		///     Called by <see cref="StateMachine" /> when the state is the current state and receives a signal.
		/// </summary>
		/// <param name="signalInfo"> The signal being executed. </param>
		void Signal (StateSignalInfo signalInfo);
	}
}
