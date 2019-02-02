using System;
using System.Collections.Generic;

using RI.Framework.Collections;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;
using RI.Framework.Utilities.Reflection;




namespace RI.Framework.StateMachines.States
{
    /// <summary>
    ///     Implements a base class which can be used for state implementation.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See <see cref="IState" /> for more details.
    ///     </para>
    ///     <para>
    ///         <see cref="State" /> adds additional convenience through signal registration and dispatching, reducing the amount of boilerplate code required when dealing with signals.
    ///         See <see cref="Signal" /> for more information.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    /// TODO: Forward updates and signals to sub-statemachines
    /// TODO: Check and compare other implementations of IState
    public abstract class State : LogSource, IState
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="State" />.
        /// </summary>
        protected State ()
        {
            this.SyncRoot = new object();

            this.IsInitialized = false;
            this.UseCaching = true;
            this.UpdateInterval = null;
            this.DisableSignalRegistration = false;
            this.DisableSignalDispatching = false;

            this.SignalHandlers = new Dictionary<Type, Delegate>();
            this.SubMachines = new HashSet<StateMachine>();
            this.ActiveMachines = new HashSet<StateMachine>();
            this.SignalDispatcher = MethodCallDispatcher.FromTarget(this, nameof(this.Signal));
        }

        #endregion




        #region Instance Fields

        private bool _disableSignalDispatching;

        private bool _disableSignalRegistration;

        private bool _isInitialized;

        private int? _updateInterval;

        private bool _useCaching;

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets or sets whether signal dispatching is disabled or not.
        /// </summary>
        /// <value>
        ///     true if signal dispatching is disabled, false otherwise.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         See <see cref="Signal" /> for more information about signal dispatching.
        ///     </para>
        /// </remarks>
        protected bool DisableSignalDispatching
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._disableSignalDispatching;
                }
            }
            set
            {
                lock (this.SyncRoot)
                {
                    this._disableSignalDispatching = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets whether signal registration is disabled or not.
        /// </summary>
        /// <value>
        ///     true if signal registration is disabled, false otherwise.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         Signals can still be registered when disabled but they will not be forwarded until registration is enabled again.
        ///     </para>
        ///     <para>
        ///         See <see cref="Signal" /> for more information about signal registration.
        ///     </para>
        /// </remarks>
        protected bool DisableSignalRegistration
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._disableSignalRegistration;
                }
            }
            set
            {
                lock (this.SyncRoot)
                {
                    this._disableSignalRegistration = value;
                }
            }
        }

        private HashSet<StateMachine> ActiveMachines { get; }

        private MethodCallDispatcher SignalDispatcher { get; }

        private Dictionary<Type, Delegate> SignalHandlers { get; }

        private HashSet<StateMachine> SubMachines { get; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Creates a new sub state machine which is a hierarchical subordinate to this state.
        /// </summary>
        /// <param name="creator"> The creator callback which is used to create the sub state machine. </param>
        /// <returns>
        ///     The created sub state machine.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         The created sub state machine is stored by this state instance (see <see cref="GetSubMachines"/>).
        ///         This state instance will issue a transient to null on the created sub state machine every time this state is left (<see cref="Leave" />).
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="creator" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="creator" /> returned null instead of a <see cref="StateMachine" /> or derived instance. </exception>
        protected StateMachine CreateSubMachine (Func<StateMachine> creator)
        {
            if (creator == null)
            {
                throw new ArgumentNullException(nameof(creator));
            }

            lock (this.SyncRoot)
            {
                StateMachine subStateMachine = creator();
                if (subStateMachine == null)
                {
                    throw new InvalidOperationException();
                }

                this.SubMachines.Add(subStateMachine);

                return subStateMachine;
            }
        }

        /// <summary>
        ///     Gets the list of all sub state machines which are hierarchical subordinates to this state.
        /// </summary>
        /// <returns>
        ///     The list of all sub state machines which are hierarchical subordinates to this state.
        ///     If the state has no sub state machines, an empty list is returned.
        /// </returns>
        protected List<StateMachine> GetSubMachines ()
        {
            lock (this.SyncRoot)
            {
                return new List<StateMachine>(this.SubMachines);
            }
        }

        /// <summary>
        ///     Registers a type-specific signal handler.
        /// </summary>
        /// <typeparam name="TSignal"> The type of the signals handled by <paramref name="handler" />. </typeparam>
        /// <param name="handler"> The delegate which handles signals of type <typeparamref name="TSignal" />. </param>
        /// <remarks>
        ///     <para>
        ///         Any handler already registered for the signal type <typeparamref name="TSignal" /> will be unregistered first.
        ///         Only one handler for a particular signal type can be registered.
        ///     </para>
        ///     <para>
        ///         See <see cref="Signal" /> for more information about signal registration.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="handler" /> is null. </exception>
        protected void RegisterSignal <TSignal> (Action<TSignal, StateSignalInfo> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            lock (this.SyncRoot)
            {
                Type type = typeof(TSignal);
                this.SignalHandlers.Remove(type);
                this.SignalHandlers.Add(type, handler);
            }
        }

        /// <summary>
        ///     Unregisters the signal handler for the specified signal type.
        /// </summary>
        /// <typeparam name="TSignal"> The type of the signal. </typeparam>
        /// <remarks>
        ///     <para>
        ///         Nothing happens if no signal handler for the specified signal type is registered.
        ///     </para>
        ///     <para>
        ///         See <see cref="Signal" /> for more information about signal registration.
        ///     </para>
        /// </remarks>
        protected void UnregisterSignal <TSignal> ()
        {
            lock (this.SyncRoot)
            {
                Type type = typeof(TSignal);
                this.SignalHandlers.Remove(type);
            }
        }

        #endregion




        #region Virtuals

        /// <inheritdoc cref="IState.Enter" />
        protected virtual void Enter (StateTransientInfo transientInfo)
        {
        }

        /// <inheritdoc cref="IState.Initialize" />
        protected virtual void Initialize (StateMachine stateMachine)
        {
        }

        /// <inheritdoc cref="IState.Leave" />
        protected virtual void Leave (StateTransientInfo transientInfo)
        {
        }

        /// <summary>
        ///     Called by <see cref="Signal" /> for all signals which are neither registered nor dispatched.
        /// </summary>
        /// <param name="signalInfo"> The signal being executed. </param>
        protected virtual void OnUnregisteredUndispatchedSignal (StateSignalInfo signalInfo)
        {
        }

        /// <inheritdoc cref="IState.Signal" />
        /// <remarks>
        ///     <para>
        ///         The default implementation uses signal registration and dispatching (when not disabled through <see cref="DisableSignalRegistration" /> or <see cref="DisableSignalDispatching" />).
        ///         Signal registration takes precedence over dispatching.
        ///         If a signal has been registered, the registered handler is called.
        ///         If the signal is not registered or registration is disabled, it is dispatched using <see cref="MethodCallDispatcher" /> looking for methods with name <c> Signal </c>.
        ///         If the signal is not dispatched or dispatching is disabled, <see cref="OnUnregisteredUndispatchedSignal" /> is called.
        ///     </para>
        ///     <note type="important">
        ///         Signal dispatching impacts performance as reflection is used to determine the target method to be called.
        ///         Signal registration impacts performance as <see cref="Delegate.DynamicInvoke" /> is used to call the corresponding handler.
        ///         Therefore, it is recommended to disable registration (using <see cref="DisableSignalRegistration" />) or dispatching (<see cref="DisableSignalDispatching" />) in performance critical scenarios.
        ///         Use <see cref="OnUnregisteredUndispatchedSignal" /> instead in such scenarios.
        ///     </note>
        /// </remarks>
        protected virtual void Signal (StateSignalInfo signalInfo)
        {
            if (!this.DisableSignalRegistration)
            {
                Type signalType = signalInfo.Signal?.GetType();

                Delegate signalHandler = null;
                if (signalType != null)
                {
                    lock (this.SyncRoot)
                    {
                        if (this.SignalHandlers.ContainsKey(signalType))
                        {
                            signalHandler = this.SignalHandlers[signalType];
                        }
                    }
                }

                if (signalHandler != null)
                {
                    signalHandler.DynamicInvoke(signalInfo.Signal, signalInfo);
                    return;
                }
            }

            if (!this.DisableSignalDispatching)
            {
                if (this.SignalDispatcher.Dispatch(signalInfo.Signal, out _, (name, type) =>
                {
                    if ((type == typeof(StateMachine)) || (type == signalInfo.StateMachine.GetType()))
                    {
                        return signalInfo.StateMachine;
                    }

                    return null;
                }))
                {
                    return;
                }
            }

            this.OnUnregisteredUndispatchedSignal(signalInfo);
        }

        /// <inheritdoc cref="IState.Update" />
        protected virtual void Update (StateUpdateInfo updateInfo)
        {
        }

        #endregion




        #region Interface: IState

        /// <inheritdoc />
        public bool IsActive
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this.ActiveMachines.Count > 0;
                }
            }
        }

        /// <inheritdoc />
        public bool IsInitialized
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._isInitialized;
                }
            }
            protected set
            {
                lock (this.SyncRoot)
                {
                    this._isInitialized = value;
                }
            }
        }

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        public StateMachine StateMachine
        {
            get
            {
                lock (this.SyncRoot)
                {
                    if (this.ActiveMachines.Count > 1)
                    {
                        throw new InvalidOperationException("An instance of the state " + this.GetType().Name + " is used in more than one state machines.");
                    }
                    else if (this.ActiveMachines.Count == 1)
                    {
                        return this.ActiveMachines.First();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <inheritdoc />
        public object SyncRoot { get; }

        /// <inheritdoc />
        public int? UpdateInterval
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._updateInterval;
                }
            }
            protected set
            {
                lock (this.SyncRoot)
                {
                    this._updateInterval = value;
                }
            }
        }

        /// <inheritdoc />
        public bool UseCaching
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._useCaching;
                }
            }
            protected set
            {
                lock (this.SyncRoot)
                {
                    this._useCaching = value;
                }
            }
        }

        /// <inheritdoc />
        void IState.Enter (StateTransientInfo transientInfo)
        {
            if (transientInfo == null)
            {
                throw new ArgumentNullException(nameof(transientInfo));
            }

            lock (this.SyncRoot)
            {
                this.ActiveMachines.Add(transientInfo.StateMachine);
            }

            this.Enter(transientInfo);
        }

        /// <inheritdoc />
        public List<StateMachine> GetActiveMachines ()
        {
            lock (this.SyncRoot)
            {
                return new List<StateMachine>(this.ActiveMachines);
            }
        }

        /// <inheritdoc />
        void IState.Initialize (StateMachine stateMachine)
        {
            if (stateMachine == null)
            {
                throw new ArgumentNullException(nameof(stateMachine));
            }

            bool initialize = false;
            lock (this.SyncRoot)
            {
                if (!this.IsInitialized)
                {
                    initialize = true;
                    this.IsInitialized = true;
                }
            }

            if (initialize)
            {
                this.Initialize(stateMachine);
            }
        }

        /// <inheritdoc />
        void IState.Leave (StateTransientInfo transientInfo)
        {
            if (transientInfo == null)
            {
                throw new ArgumentNullException(nameof(transientInfo));
            }

            this.Leave(transientInfo);

            lock (this.SyncRoot)
            {
                this.ActiveMachines.Remove(transientInfo.StateMachine);
                this.SubMachines.ForEach(x => x.Transient(null));
            }
        }

        /// <inheritdoc />
        void IState.Signal (StateSignalInfo signalInfo)
        {
            if (signalInfo == null)
            {
                throw new ArgumentNullException(nameof(signalInfo));
            }

            this.Signal(signalInfo);
        }

        /// <inheritdoc />
        void IState.Update (StateUpdateInfo updateInfo)
        {
            if (updateInfo == null)
            {
                throw new ArgumentNullException(nameof(updateInfo));
            }

            this.Update(updateInfo);
        }

        #endregion
    }
}
