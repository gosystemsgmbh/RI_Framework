using RI.Framework.StateMachines.Dispatchers;




namespace RI.Framework.StateMachines
{
    /// <summary>
    ///     The execution callback which is handed to <see cref="IStateDispatcher" />.<see cref="IStateDispatcher.DispatchUpdate" />.
    /// </summary>
    /// <param name="updateInfo"> The update to execute. </param>
    /// <remarks>
    ///     <para>
    ///         An <see cref="IStateDispatcher" /> uses this callback when it eventually wants to execute the update.
    ///         The actual execution is then done by <see cref="StateMachine" />.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public delegate void StateMachineUpdateDelegate (StateUpdateInfo updateInfo);
}
