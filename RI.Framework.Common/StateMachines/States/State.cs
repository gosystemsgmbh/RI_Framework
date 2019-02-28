using System;
using System.Collections.Generic;

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
    ///     <para>
    ///         <see cref="State" /> adds additional convenience for creating sub state machines.
    ///         See <see cref="CreateSubMachine()" /> and <see cref="CreateSubMachine(Func{StateMachines.StateMachine})"/> for more information.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
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
            this.StateMachine = null;

            this.UseCaching = true;
            this.UpdateInterval = null;

            this.DisableSignalRegistration = false;
            this.DisableSignalDispatching = false;

            this.SubMachines = new HashSet<StateMachine>();
            this.SignalHandlers = new Dictionary<Type, Delegate>();
            this.SignalDispatcher = MethodCallDispatcher.FromTarget(this, nameof(this.Signal));
        }

        #endregion




        #region Instance Fields

        private bool _isInitialized;

        private StateMachine _stateMachine;

        private int? _updateInterval;

        private bool _useCaching;

        private bool _disableSignalDispatching;

        private bool _disableSignalRegistration;

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

        private MethodCallDispatcher SignalDispatcher { get; }

        private Dictionary<Type, Delegate> SignalHandlers { get; }

        private HashSet<StateMachine> SubMachines { get; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Creates a new sub state machine which is a hierarchical subordinate to this state.
        /// </summary>
        /// <returns>
        ///     The created sub state machine.
        /// </returns>
        /// <remarks>
        /// <para>
        /// An instance of <see cref="StateMachine"/> is created using a cloned configuration from the associated <see cref="StateMachine"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">No state machine is associated with this state instance (<see cref="StateMachine"/> is null).</exception>
        protected StateMachine CreateSubMachine ()
        {
            lock (this.SyncRoot)
            {
                if (this.StateMachine == null)
                {
                    throw new InvalidOperationException();
                }

                return this.CreateSubMachine(() =>
                {
                    StateMachineConfiguration configuration = this.StateMachine.Configuration.Clone();
                    StateMachine machine = new StateMachine(configuration);
                    return machine;
                });
            }
        }

        /// <summary>
        ///     Creates a new sub state machine which is a hierarchical subordinate to this state.
        /// </summary>
        /// <param name="creator"> The creator callback which is used to create and configure the sub state machine. </param>
        /// <returns>
        ///     The created sub state machine.
        /// </returns>
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
        ///     Called by the default implementation of <see cref="Signal" /> for all signals which are neither registered nor dispatched.
        /// </summary>
        /// <param name="signalInfo"> The signal being executed. </param>
        protected virtual void OnUnhandledSignal (StateSignalInfo signalInfo)
        {
        }

        /// <inheritdoc cref="IState.Signal" />
        /// <remarks>
        ///     <para>
        ///         The default implementation uses signal registration and dispatching (when not disabled through <see cref="DisableSignalRegistration" /> or <see cref="DisableSignalDispatching" />).
        ///         Signal registration takes precedence over dispatching.
        ///         If a signal has been registered (<see cref="RegisterSignal{TSignal}"/>), the registered handler is called.
        ///         If the signal is not registered or registration is disabled, it is dispatched using <see cref="MethodCallDispatcher" /> looking for methods with name <c> Signal </c>.
        ///         If the signal is not dispatched or dispatching is disabled, <see cref="OnUnhandledSignal" /> is called.
        ///     </para>
        ///     <note type="important">
        ///         Signal dispatching impacts performance as reflection is used to determine the target method to be called.
        ///         Signal registration impacts performance as <see cref="Delegate.DynamicInvoke" /> is used to call the corresponding handler.
        ///         Therefore, it is recommended to disable registration (using <see cref="DisableSignalRegistration" />) or dispatching (<see cref="DisableSignalDispatching" />) in performance critical scenarios.
        ///         Use <see cref="OnUnhandledSignal" /> instead in such scenarios.
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

            this.OnUnhandledSignal(signalInfo);
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
                    return object.ReferenceEquals(this.StateMachine?.State, this);
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
                    return this._stateMachine;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._stateMachine = value;
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
                    if (value.HasValue)
                    {
                        if (value.Value <= 0)
                        {
                            throw new ArgumentOutOfRangeException(nameof(value));
                        }
                    }
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

            this.Enter(transientInfo);
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
                    this.StateMachine = stateMachine;
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

        /// <inheritdoc />
        public List<StateMachine> GetSubMachines()
        {
            lock (this.SyncRoot)
            {
                return new List<StateMachine>(this.SubMachines);
            }
        }

        #endregion
    }
}
