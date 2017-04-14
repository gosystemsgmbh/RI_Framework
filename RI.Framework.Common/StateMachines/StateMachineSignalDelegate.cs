namespace RI.Framework.StateMachines
{
	/// <summary>
	///     The execution callback which is handed to <see cref="IStateDispatcher" />.<see cref="IStateDispatcher.DispatchSignal" />.
	/// </summary>
	/// <param name="signalInfo"> The signal to execute. </param>
	/// <remarks>
	///     <para>
	///         An <see cref="IStateDispatcher" /> uses this callback when it eventually wants to execute the signal.
	///         The actual execution is then done by <see cref="StateMachine" />.
	///     </para>
	/// </remarks>
	public delegate void StateMachineSignalDelegate (StateSignalInfo signalInfo);
}
