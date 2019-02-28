using System;
using System.Collections.Generic;

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
    ///     <note type="important">
    ///         Some methods are called by <see cref="StateMachine" /> while in a lock to its <see cref="StateMachines.StateMachine.SyncRoot" />.
    ///         Be careful in implementing classes when calling outside code from those methods (e.g. through events, callbacks, or other virtual methods) to not produce deadlocks!
    ///     </note>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
    public interface IState : ISynchronizable
    {
        /// <summary>
        ///     Gets whether the state is active.
        /// </summary>
        /// <value>
        ///     true if the state is active (means: it is the current state of <see cref="StateMachine"/>), false otherwise.
        /// </value>
        bool IsActive { get; }

        /// <summary>
        ///     Gets the state machine this state instance is associated with.
        /// </summary>
        /// <value>
        ///     The state machine this state instance is associated with or null if this state instance is not associated with any state machine.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         The associated state machine must never be changed once associated.
        ///     </note>
        /// </remarks>
        StateMachine StateMachine { get; }

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
        /// <exception cref="ArgumentOutOfRangeException">The value is not null and less than one.</exception>
        int? UpdateInterval { get; }

        /// <summary>
        ///     Gets whether the state instance is cacheable or can be reused respectively.
        /// </summary>
        /// <value>
        ///     true if the state instance is cacheable, false otherwise.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         If <see cref="UseCaching" /> is false, the state is never cached, regardless of the state machine configurations <see cref="StateMachineConfiguration.CachingEnabled" /> setting.
        ///     </para>
        /// </remarks>
        bool UseCaching { get; }

        /// <summary>
        ///     Gets the collection of sub state machines owned by this state.
        /// </summary>
        /// <returns>
        ///     The collection of sub state machines owned by this state.
        /// </returns>
        /// <remarks>
        /// <note type="implement">
        ///      If this state supports hierarchical state machines but has no sub state machines, an empty list is returned.
        ///      If this state does not support hierarchical state machines, null is returned.
        /// </note>
        /// </remarks>
        List<StateMachine> GetSubMachines ();

        /// <summary>
        ///     Called by <see cref="StateMachine" /> when the state becomes the new current state.
        /// </summary>
        /// <param name="transientInfo"> The transition being executed. </param>
        /// <remarks>
        ///     <note type="important">
        ///         This method is called by <see cref="StateMachine" /> while in a lock to its <see cref="StateMachines.StateMachine.SyncRoot" />.
        ///     </note>
        /// </remarks>
        /// <exception cref="SharedStateException">The state machine associated with this transient does not match the state machine already associated with this state instance.</exception>
        void Enter (StateTransientInfo transientInfo);

        /// <summary>
        ///     Initializes the state.
        /// </summary>
        /// <param name="stateMachine"> The state machine which initializes the state. </param>
        /// <remarks>
        ///     <para>
        ///         <see cref="Initialize" /> is called during the transition for a next state, before the previous state is left, but only if <see cref="IsInitialized" /> is false.
        ///     </para>
        ///     <note type="important">
        ///         This method is called by <see cref="StateMachine" /> while in a lock to its <see cref="StateMachines.StateMachine.SyncRoot" />.
        ///     </note>
        ///     <note type="implement">
        ///         <see cref="Initialize"/> must set <see cref="StateMachine"/> to associate this state instance with a state machine.
        ///     </note>
        /// </remarks>
        /// <exception cref="SharedStateException">The state machine associated with this initialization does not match the state machine already associated with this state instance.</exception>
        void Initialize (StateMachine stateMachine);

        /// <summary>
        ///     Called by <see cref="StateMachine" /> when the state is left and is no longer the current state.
        /// </summary>
        /// <param name="transientInfo"> The transition being executed. </param>
        /// <remarks>
        ///     <note type="important">
        ///         This method is called by <see cref="StateMachine" /> while in a lock to its <see cref="StateMachines.StateMachine.SyncRoot" />.
        ///     </note>
        /// </remarks>
        /// <exception cref="SharedStateException">The state machine associated with this transient does not match the state machine already associated with this state instance.</exception>
        void Leave (StateTransientInfo transientInfo);

        /// <summary>
        ///     Called by <see cref="StateMachine" /> when the state is the current state and receives a signal.
        /// </summary>
        /// <param name="signalInfo"> The signal being executed. </param>
        /// <remarks>
        ///     <note type="important">
        ///         This method is called by <see cref="StateMachine" /> while in a lock to its <see cref="StateMachines.StateMachine.SyncRoot" />.
        ///     </note>
        /// </remarks>
        /// <exception cref="SharedStateException">The state machine associated with this signal does not match the state machine already associated with this state instance.</exception>
        void Signal (StateSignalInfo signalInfo);

        /// <summary>
        ///     Periodically called by <see cref="StateMachine" /> to update the state (based on <see cref="UpdateInterval"/>) or when <see cref="StateMachines.StateMachine.Update" /> is called.
        /// </summary>
        /// <param name="updateInfo"> The update information. </param>
        /// <remarks>
        ///     <note type="important">
        ///         This method is called by <see cref="StateMachine" /> while in a lock to its <see cref="StateMachines.StateMachine.SyncRoot" />.
        ///     </note>
        /// </remarks>
        /// <exception cref="SharedStateException">The state machine associated with this update does not match the state machine already associated with this state instance.</exception>
        void Update (StateUpdateInfo updateInfo);
    }
}
