using RI.Framework.StateMachines.Dispatchers;




namespace RI.Framework.StateMachines
{
	/// <summary>
	///     The execution callback which is handed to <see cref="IStateDispatcher" />.<see cref="IStateDispatcher.DispatchTransition" />.
	/// </summary>
	/// <param name="transientInfo"> The transition to execute. </param>
	/// <remarks>
	///     <para>
	///         An <see cref="IStateDispatcher" /> uses this callback when it eventually wants to execute the transition.
	///         The actual execution is then done by <see cref="StateMachine" />.
	///     </para>
	/// </remarks>
	public delegate void StateMachineTransientDelegate (StateTransientInfo transientInfo);
}
