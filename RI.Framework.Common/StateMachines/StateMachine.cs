using System;
using System.Collections.Generic;
using System.Reflection;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.StateMachines.Caches;
using RI.Framework.StateMachines.Dispatchers;
using RI.Framework.StateMachines.Resolvers;
using RI.Framework.StateMachines.States;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;
using RI.Framework.Utilities.Reflection;




namespace RI.Framework.StateMachines
{
    /// <summary>
    ///     The actual state machine.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <b> GENERAL </b>
    ///     </para>
    ///     <para>
    ///         <see cref="StateMachine" /> is the actual state machine which manages the current state, signals, and state transitions.
    ///     </para>
    ///     <para>
    ///         Multiple <see cref="StateMachine" /> instances can be used independently.
    ///         However, it is important that they do not share certain aspects of their configuration.
    ///         See <see cref="StateMachineConfiguration" /> for more details.
    ///     </para>
    ///     <para>
    ///         A state machine has four core concepts: states, signals, transitions, and dispatching.
    ///     </para>
    ///     <para>
    ///         <b> STATES </b>
    ///     </para>
    ///     <para>
    ///         A state machine always has a current state (<see cref="State" />) which is either null or an instance of <see cref="IState" />.
    ///         The initial state after a <see cref="StateMachine" /> is created is always null (and is therefore usually used to expresss &quot;no state&quot; and/or to shut down the state machine by a transition to the null state).
    ///         Instead of directly using instances of <see cref="IState" /> when working with state machines, you use the states <see cref="Type" />.
    ///         The state machine then resolves the actual <see cref="IState" /> instance of that type.
    ///         See <see cref="Resolve" /> for more details about how <see cref="IState" /> instances are resolved by their types.
    ///         This also allows to cache state instances so that they can be reused, instead of creating a new instance each time a state becomes the current state.
    ///         See <see cref="IStateCache" /> for more details about state caches.
    ///     </para>
    ///     <para>
    ///         <b> SIGNALS </b>
    ///     </para>
    ///     <para>
    ///         A signal is used to inform the current state about an event to which the current state might react (or not, depending on the current state and the signal).
    ///         Basically, a signal is just an object of any type which is passed to the current state.
    ///         A signal is issued using one of the <c>Signal</c> methods.
    ///         The current state receives the signal using <see cref="IState.Signal" />.
    ///         Depending on the signal, the state might initiate a transition or perform some other state-specific actions.
    ///         Signals are used to decouple the states from event sources so that events can be signalled independently of the current state.
    ///     </para>
    ///     <para>
    ///         A very powerful concept is the combination of state inheritance and signals.
    ///         Signals which are handled identically for all or a group of states can be handled in a base class which implements <see cref="IState" />.
    ///         Other states can then inherit from that base class and only add signal handling for their special state-specific cases.
    ///     </para>
    ///     <para>
    ///         <b> TRANSITIONS </b>
    ///     </para>
    ///     <para>
    ///         Transitions are nothing else than changing the current state.
    ///         A transition is initiated using one of the <c>Transient</c> methods.
    ///         During a transition, the previous states <see cref="IState.Leave" /> method is called, then <see cref="State" /> is updated to the next state, and then the next states <see cref="IState.Enter" /> method is called.
    ///     </para>
    ///     <para>
    ///         To maintain valid state transitions, only states themselves should initiate a state transition.
    ///         See the example below for more details about maintaining valid state transitions.
    ///     </para>
    ///     <para>
    ///         <b> DISPATCHING </b>
    ///     </para>
    ///     <para>
    ///         Dispatching is a very important concept for creating state machines that behave in a logical way.
    ///         When a signal or a transition is initiated (see above), the state machine does not immediately execute the signal or transition.
    ///         Instead, it is handed to the dispatcher (<see cref="IStateDispatcher" />), specified by the state machine configuration (<see cref="Configuration" />, <see cref="StateMachineConfiguration.Dispatcher" />).
    ///         The dispatcher is then responsible for the actual execution of the signals and transitions.
    ///         Usually, the signals and transitions are placed in a queue and are then processed one-by-one.
    ///         However, this depends on the actual implementation of the <see cref="IStateDispatcher" />.
    ///     </para>
    ///     <para>
    ///         The reason why dispatching is important for a logical behaviour is the requirement that all state machine operations (signals and transitions) are completed before another operation is executed.
    ///         This guarantees, for example, that all state code executed in <see cref="IState.Signal" />, <see cref="IState.Enter" /> or <see cref="IState.Leave" /> is executed under the same current state, even if the state issues a signal or a transient.
    ///     </para>
    ///     <note type="important">
    ///         Some virtual methods are called from within locks to <see cref="SyncRoot" />.
    ///         Be careful in inheriting classes when calling outside code from those methods (e.g. through events, callbacks, or other virtual methods) to not produce deadlocks!
    ///     </note>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    /// <example>
    ///     <code language="cs">
    ///  <![CDATA[
    ///  // create a state machine with default configuration (which uses the dispatcher service for dispatching transitions and signals)
    ///  var stateMachine = new StateMachine(new DefaultStateMachineConfiguration());
    ///  
    ///  // dispatch some transitions and signals
    ///  
    ///  // transition from state null to StateA
    ///  stateMachine.Transient<StateA>();
    ///  
    ///  // send a signal to StateA
    ///  stateMachine.Signal("some signal, event, data, etc. to be handled by the current state");
    ///  
    ///  // INVALID TRANSITION!
    ///  // this transition is invalid (can be dispatched but will not be executed)
    ///  // reason: as the transition from null to StateA was not yet executed when the transition to StateB was issued,
    ///  // this transition was dispatched as a transition from null to StateB. However, when this transition is executed,
    ///  // it would be a transition from StateA to StateB and NOT from null to StateB as it was assumed at the time this transition was dispatched.
    ///  // ONLY STATES SHOULD ISSUE TRANSIENTS! USE SIGNALS FROM OUTSIDE OF STATES TO INITIATE STATE TRANSITIONS! (sorry for shouting, I needed to draw attention to this very important concept)
    ///  stateMachine.Transient<StateB>();
    ///  
    ///  // send a signal to StateA
    ///  // (we are still in state a as the transition to StateB was invalid)
    ///  stateMachine.Signal("some signal, event, data, etc. to be handled by the current state");
    ///  
    ///  
    ///  // ...
    ///  
    ///  
    ///  [Export]
    ///  public class BaseState : MonoState
    ///  {
    /// 		protected override void Enter (StateTransitionInfo transientInfo)
    /// 		{
    /// 			base.Enter(transientInfo);
    /// 			Debug.Log("Enter base");
    /// 		}
    ///  
    /// 		protected override void Leave (StateTransitionInfo transientInfo)
    /// 		{
    /// 			base.Leave(transientInfo);
    /// 			Debug.Log("Leave base");
    /// 		}
    ///  
    /// 		protected override void Signal (StateSignalInfo signalInfo)
    /// 		{
    /// 			base.Signal(signalInfo);
    /// 			Debug.Log("Signal base");
    /// 		}
    ///  }
    ///  
    ///  [Export]
    ///  public class StateA : BaseState
    ///  {
    /// 		protected override void Enter (StateTransitionInfo transientInfo)
    /// 		{
    /// 			base.Enter(transientInfo);
    /// 			Debug.Log("Enter A");
    /// 		}
    ///  
    /// 		protected override void Leave (StateTransitionInfo transientInfo)
    /// 		{
    /// 			base.Leave(transientInfo);
    /// 			Debug.Log("Leave A");
    /// 		}
    ///  
    /// 		protected override void Signal (StateSignalInfo signalInfo)
    /// 		{
    /// 			base.Signal(signalInfo);
    /// 			Debug.Log("Signal A");
    /// 		}
    ///  }
    ///  
    ///  [Export]
    ///  public class StateB : BaseState
    ///  {
    /// 		protected override void Enter (StateTransitionInfo transientInfo)
    /// 		{
    /// 			base.Enter(transientInfo);
    /// 			Debug.Log("Enter B");
    /// 		}
    ///  
    /// 		protected override void Leave (StateTransitionInfo transientInfo)
    /// 		{
    /// 			base.Leave(transientInfo);
    /// 			Debug.Log("Leave B");
    /// 		}
    ///  
    /// 		protected override void Signal (StateSignalInfo signalInfo)
    /// 		{
    /// 			base.Signal(signalInfo);
    /// 			Debug.Log("Signal B");
    /// 		}
    ///  }
    ///  
    ///  
    ///  // ...
    ///  
    ///  
    ///  // the debug output will be:
    ///  // Enter base
    ///  // Enter A
    ///  // Signal base
    ///  // Signal A
    ///  // Signal base
    ///  // Signal A
    ///  ]]>
    ///  </code>
    /// </example>
    /// TODO: State machine persistency
    /// TODO: Transition guards
    /// TODO: Signal "guards" (not quite, but similar technical concept)
    /// TODO: Update "guards" (not quite, but similar technical concept)
    public class StateMachine : LogSource, ISynchronizable
    {
        #region Static Constructor/Destructor

        static StateMachine ()
        {
            StateMachine.GlobalSyncRoot = new object();
            StateMachine.StateVariableInfos = new Dictionary<Type, Dictionary<string, StateVariableInfo>>();
        }

        #endregion




        #region Static Properties/Indexer

        private static Dictionary<Type, Dictionary<string, StateVariableInfo>> StateVariableInfos { get; }

        private static object GlobalSyncRoot { get; }

        #endregion




        #region Static Methods

        private static Dictionary<string, StateVariableInfo> GetStateVariableInfos (Type type)
        {
            lock (StateMachine.GlobalSyncRoot)
            {
                if (!StateMachine.StateVariableInfos.ContainsKey(type))
                {
                    Dictionary<string, StateVariableInfo> stateVariableInfos = new Dictionary<string, StateVariableInfo>(StringComparerEx.Ordinal);

                    PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    foreach (PropertyInfo property in properties)
                    {
                        StateVariableAttribute attribute = property.GetCustomAttributes(typeof(StateVariableAttribute), false).OfType<StateVariableAttribute>().FirstOrDefault();
                        if (attribute == null)
                        {
                            continue;
                        }

                        StateVariableInfo info = new StateVariableInfo();
                        info.Property = property;
                        info.Attribute = attribute;
                        info.GetMethod = property.GetGetMethod(true);
                        info.SetMethod = property.GetSetMethod(true);
                        //TODO: Option to get default value from attribute (or even function?)
                        info.DefaultValue = property.PropertyType.GetDefaultValue();

                        stateVariableInfos.Add(property.Name, info);
                    }

                    StateMachine.StateVariableInfos.Add(type, stateVariableInfos);
                }

                return StateMachine.StateVariableInfos[type];
            }
        }

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="StateMachine" />.
        /// </summary>
        /// <param name="configuration"> The state machines configuration. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="configuration" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="configuration" /> does not specify a <see cref="IStateDispatcher" />, <see cref="IStateResolver" />, or <see cref="IStateCache" />. </exception>
        public StateMachine (StateMachineConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            lock (configuration.SyncRoot)
            {
                if (configuration.Dispatcher == null)
                {
                    throw new ArgumentException("No state dispatcher specified.", nameof(configuration));
                }

                if (configuration.Resolver == null)
                {
                    throw new ArgumentException("No state resolver specified.", nameof(configuration));
                }

                if (configuration.Cache == null)
                {
                    throw new ArgumentException("No state cache specified.", nameof(configuration));
                }

                configuration.IsLocked = true;
            }

            this.SyncRoot = new object();

            this.Configuration = configuration;

            this.TransientDelegate = this.ExecuteTransient;
            this.SignalDelegate = this.ExecuteSignal;
            this.UpdateDelegate = this.ExecuteUpdate;

            this.State = null;
            this.StateEnterTimestampUtc = DateTime.UtcNow;
            this.StateEnterTimestampLocal = this.StateEnterTimestampUtc.ToLocalTime();
        }

        #endregion




        #region Instance Fields

        private IState _state;

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the state machine configuration.
        /// </summary>
        /// <value>
        ///     The state machine configuration.
        /// </value>
        /// <remarks>
        ///     <note type="note">
        ///         Be careful when changing properties of <see cref="Configuration" /> while <see cref="State" /> is not null.
        ///         See <see cref="StateMachineConfiguration" /> for more details.
        ///     </note>
        /// </remarks>
        public StateMachineConfiguration Configuration { get; }

        /// <summary>
        ///     Gets the current state.
        /// </summary>
        /// <value>
        ///     The current state or null if there is currently no state.
        /// </value>
        public IState State
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._state;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._state = value;
                }
            }
        }

        private StateMachineSignalDelegate SignalDelegate { get; }

        private DateTime StateEnterTimestampLocal { get; set; }

        private DateTime StateEnterTimestampUtc { get; set; }

        private StateMachineTransientDelegate TransientDelegate { get; }

        private StateMachineUpdateDelegate UpdateDelegate { get; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Sends a signal to the current state.
        /// </summary>
        /// <typeparam name="TSignal"> The type of the signal. </typeparam>
        /// <param name="signal"> The signal. </param>
        public void Signal <TSignal> (TSignal signal)
        {
            this.Signal((object)signal);
        }

        /// <summary>
        ///     Sends a signal to the current state.
        /// </summary>
        /// <param name="signal"> The signal. </param>
        public void Signal (object signal)
        {
            lock (this.SyncRoot)
            {
                this.SignalInternal(signal, new object[0]);
            }
        }

        /// <summary>
        ///     Sends a signal to the current state.
        /// </summary>
        /// <typeparam name="TSignal"> The type of the signal. </typeparam>
        /// <param name="signal"> The signal. </param>
        /// <param name="parameters"> Specifies optional signal parameters passed to the current state. </param>
        public void Signal<TSignal> (TSignal signal, params object[] parameters)
        {
            this.Signal((object)signal, parameters);
        }

        /// <summary>
        ///     Sends a signal to the current state.
        /// </summary>
        /// <param name="signal"> The signal. </param>
        /// <param name="parameters"> Specifies optional signal parameters passed to the current state. </param>
        public void Signal (object signal, params object[] parameters)
        {
            lock (this.SyncRoot)
            {
                this.SignalInternal(signal, parameters);
            }
        }

        /// <summary>
        ///     Initiates a transition to another state.
        /// </summary>
        /// <typeparam name="TState"> The type of state to transition to. </typeparam>
        public void Transient <TState> ()
            where TState : IState
        {
            this.Transient(typeof(TState));
        }

        /// <summary>
        ///     Initiates a transition to another state.
        /// </summary>
        /// <param name="state"> The type of state to transition to. </param>
        public void Transient (Type state)
        {
            IState nextState = this.Resolve(state);

            lock (this.SyncRoot)
            {
                this.TransientInternal(nextState, new object[0]);
            }
        }

        /// <summary>
        ///     Initiates a transition to another state.
        /// </summary>
        /// <typeparam name="TState"> The type of state to transition to. </typeparam>
        /// <param name="parameters"> Specifies optional transition parameters passed to the next state. </param>
        public void Transient <TState> (params object[] parameters)
            where TState : IState
        {
            this.Transient(typeof(TState), parameters);
        }

        /// <summary>
        ///     Initiates a transition to another state.
        /// </summary>
        /// <param name="state"> The type of state to transition to. </param>
        /// <param name="parameters"> Specifies optional transition parameters passed to the next state. </param>
        public void Transient (Type state, params object[] parameters)
        {
            IState nextState = this.Resolve(state);

            lock (this.SyncRoot)
            {
                this.TransientInternal(nextState, parameters ?? new object[0]);
            }
        }

        /// <summary>
        ///     Updates the current state.
        /// </summary>
        public void Update ()
        {
            lock (this.SyncRoot)
            {
                this.UpdateInternal(0);
            }
        }

        /// <summary>
        ///     Sends a signal to the current state.
        /// </summary>
        /// <param name="signal"> The signal. </param>
        /// <param name="parameters"> Specifies optional signal parameters passed to the current state. </param>
        protected void SignalInternal (object signal, object[] parameters)
        {
            lock (this.SyncRoot)
            {
                StateSignalInfo signalInfo = new StateSignalInfo(this, signal, parameters ?? new object[0]);
                this.DispatchSignal(signalInfo);
            }
        }

        /// <summary>
        ///     Initiates a transition to another state.
        /// </summary>
        /// <param name="nextState"> The state instance to transition to. </param>
        /// <param name="parameters"> Specifies optional transition parameters passed to the next state. </param>
        protected void TransientInternal (IState nextState, object[] parameters)
        {
            lock (this.SyncRoot)
            {
                IState previousState = this.State;
                StateTransientInfo transientInfo = new StateTransientInfo(this, previousState, nextState, parameters ?? new object[0]);
                this.DispatchTransient(transientInfo);
            }
        }

        /// <summary>
        ///     Updates the current state.
        /// </summary>
        /// <param name="delay"> The delay after which the update is executed. Use zero to indicate "as soon as possible". </param>
        protected void UpdateInternal (int delay)
        {
            if (delay < 0)
            {
                delay = 0;
            }

            lock (this.SyncRoot)
            {
                StateUpdateInfo updateInfo = new StateUpdateInfo(this, this.State, delay, this.StateEnterTimestampUtc, this.StateEnterTimestampLocal);
                this.DispatchUpdate(updateInfo);
            }
        }

        #endregion




        #region Virtuals

        private void DispatchSignal (StateSignalInfo signalInfo)
        {
            this.Log(LogLevel.Debug, "Dispatching signal: {0}", signalInfo.Signal?.ToString() ?? "[null]");

            this.Configuration.Dispatcher.DispatchSignal(this.SignalDelegate, signalInfo);
        }

        private void DispatchTransient (StateTransientInfo transientInfo)
        {
            this.Log(LogLevel.Debug, "Dispatching transient: {0} -> {1}", transientInfo.PreviousState?.GetType().Name ?? "[null]", transientInfo.NextState?.GetType().Name ?? "[null]");

            this.Configuration.Dispatcher.DispatchTransition(this.TransientDelegate, transientInfo);
        }

        private void DispatchUpdate (StateUpdateInfo updateInfo)
        {
            this.Configuration.Dispatcher.DispatchUpdate(this.UpdateDelegate, updateInfo);
        }

        private void ExecuteSignal (StateSignalInfo signalInfo)
        {
            lock (this.SyncRoot)
            {
                this.Log(LogLevel.Debug, "Executing signal: {0}", signalInfo.Signal?.ToString() ?? "[null]");

                if (this.State?.StateMachine != null)
                {
                    if (!object.ReferenceEquals(this.State.StateMachine, this))
                    {
                        throw new SharedStateException(this.State.GetType());
                    }
                }

                this.OnBeforeSignal(signalInfo);
                this.State?.Signal(signalInfo);
                this.OnAfterSignal(signalInfo);

                List<StateMachine> subMachines = this.State?.GetSubMachines();
                subMachines?.ForEach(x => x.Signal(signalInfo.Signal, signalInfo.Parameters));
            }
        }

        private void ExecuteTransient (StateTransientInfo transientInfo)
        {
            lock (this.SyncRoot)
            {
                IState previousState = transientInfo.PreviousState;
                IState nextState = transientInfo.NextState;

                this.Log(LogLevel.Debug, "Executing transient: {0} -> {1}", transientInfo.PreviousState?.GetType().Name ?? "[null]", transientInfo.NextState?.GetType().Name ?? "[null]");

                if (nextState?.StateMachine != null)
                {
                    if (!object.ReferenceEquals(nextState.StateMachine, this))
                    {
                        throw new SharedStateException(nextState.GetType());
                    }
                }

                if (!object.ReferenceEquals(this.State, previousState))
                {
                    this.Log(LogLevel.Debug, "Transient aborted: {0} -> {1}", transientInfo.PreviousState?.GetType().Name ?? "[null]", transientInfo.NextState?.GetType().Name ?? "[null]");

                    this.OnTransitionAborted(transientInfo);

                    return;
                }

                if (!((nextState?.IsInitialized).GetValueOrDefault(true)))
                {
                    nextState?.Initialize(this);
                }

                this.Cache(nextState);

                this.OnBeforeLeave(transientInfo);
                previousState?.Leave(transientInfo);
                this.OnAfterLeave(transientInfo);

                List<StateMachine> subMachines = previousState?.GetSubMachines();
                subMachines?.ForEach(x => x.Transient(null));

                this.State = nextState;
                this.StateEnterTimestampUtc = DateTime.UtcNow;
                this.StateEnterTimestampLocal = this.StateEnterTimestampUtc.ToLocalTime();

                this.TransferStateVariables(previousState, nextState);
            
                this.OnBeforeEnter(transientInfo);
                nextState?.Enter(transientInfo);
                this.OnAfterEnter(transientInfo);

                int? interval = nextState?.UpdateInterval;
                if (interval.HasValue)
                {
                    this.UpdateInternal(interval.Value);
                }
            }
        }

        private void ExecuteUpdate (StateUpdateInfo updateInfo)
        {
            lock (this.SyncRoot)
            {
                if (this.State?.StateMachine != null)
                {
                    if (!object.ReferenceEquals(this.State.StateMachine, this))
                    {
                        throw new SharedStateException(this.State.GetType());
                    }
                }

                if (!object.ReferenceEquals(this.State, updateInfo.UpdateState))
                {
                    this.Log(LogLevel.Debug, "Update aborted: {0} ", updateInfo.UpdateState?.GetType().Name ?? "[null]");

                    this.OnUpdateAborted(updateInfo);

                    return;
                }

                this.OnBeforeUpdate(updateInfo);
                this.State?.Update(updateInfo);
                this.OnAfterUpdate(updateInfo);

                if (updateInfo.UpdateDelay > 0)
                {
                    int? interval = this.State?.UpdateInterval;
                    if (interval.HasValue)
                    {
                        this.UpdateInternal(interval.Value);
                    }
                }
                else
                {
                    List<StateMachine> subMachines = this.State?.GetSubMachines();
                    subMachines?.ForEach(x => x.Update());
                }
            }
        }

        /// <summary>
        ///     Called after a state was entered.
        /// </summary>
        /// <param name="transientInfo"> The transition currently being executed. </param>
        /// <remarks>
        ///     <note type="important">
        ///         This method is called inside a lock to <see cref="SyncRoot" />.
        ///     </note>
        /// </remarks>
        protected virtual void OnAfterEnter (StateTransientInfo transientInfo)
        {
        }

        /// <summary>
        ///     Called after a state was left.
        /// </summary>
        /// <param name="transientInfo"> The transition currently being executed. </param>
        /// <remarks>
        ///     <note type="important">
        ///         This method is called inside a lock to <see cref="SyncRoot" />.
        ///     </note>
        /// </remarks>
        protected virtual void OnAfterLeave (StateTransientInfo transientInfo)
        {
        }

        /// <summary>
        ///     Called after a signal was processed.
        /// </summary>
        /// <param name="signalInfo"> The signal currently being executed. </param>
        /// <remarks>
        ///     <note type="important">
        ///         This method is called inside a lock to <see cref="SyncRoot" />.
        ///     </note>
        /// </remarks>
        protected virtual void OnAfterSignal (StateSignalInfo signalInfo)
        {
        }

        /// <summary>
        ///     Called after an update was processed.
        /// </summary>
        /// <param name="updateInfo"> The update currently being executed. </param>
        /// <remarks>
        ///     <note type="important">
        ///         This method is called inside a lock to <see cref="SyncRoot" />.
        ///     </note>
        /// </remarks>
        protected virtual void OnAfterUpdate (StateUpdateInfo updateInfo)
        {
        }

        /// <summary>
        ///     Called before a state is entered.
        /// </summary>
        /// <param name="transientInfo"> The transition currently being executed. </param>
        /// <remarks>
        ///     <note type="important">
        ///         This method is called inside a lock to <see cref="SyncRoot" />.
        ///     </note>
        /// </remarks>
        protected virtual void OnBeforeEnter (StateTransientInfo transientInfo)
        {
        }

        /// <summary>
        ///     Called before a state is left.
        /// </summary>
        /// <param name="transientInfo"> The transition currently being executed. </param>
        /// <remarks>
        ///     <note type="important">
        ///         This method is called inside a lock to <see cref="SyncRoot" />.
        ///     </note>
        /// </remarks>
        protected virtual void OnBeforeLeave (StateTransientInfo transientInfo)
        {
        }

        /// <summary>
        ///     Called before a signal is processed.
        /// </summary>
        /// <param name="signalInfo"> The signal currently being executed. </param>
        /// <remarks>
        ///     <note type="important">
        ///         This method is called inside a lock to <see cref="SyncRoot" />.
        ///     </note>
        /// </remarks>
        protected virtual void OnBeforeSignal (StateSignalInfo signalInfo)
        {
        }

        /// <summary>
        ///     Called before an update is processed.
        /// </summary>
        /// <param name="updateInfo"> The update currently being executed. </param>
        /// <remarks>
        ///     <note type="important">
        ///         This method is called inside a lock to <see cref="SyncRoot" />.
        ///     </note>
        /// </remarks>
        protected virtual void OnBeforeUpdate (StateUpdateInfo updateInfo)
        {
        }

        /// <summary>
        ///     Called when a transition was aborted because the source/previous state does not match the current state at the time the transition was eventually executed by the dispatcher.
        /// </summary>
        /// <param name="transientInfo"> The aborted transition. </param>
        /// <remarks>
        ///     <note type="important">
        ///         This method is called inside a lock to <see cref="SyncRoot" />.
        ///     </note>
        /// </remarks>
        protected virtual void OnTransitionAborted (StateTransientInfo transientInfo)
        {
        }

        /// <summary>
        ///     Called when an update was aborted because the intended state does not match the current state at the time the update was eventually executed by the dispatcher.
        /// </summary>
        /// <param name="updateInfo"> The aborted update. </param>
        /// <remarks>
        ///     <note type="important">
        ///         This method is called inside a lock to <see cref="SyncRoot" />.
        ///     </note>
        /// </remarks>
        protected virtual void OnUpdateAborted (StateUpdateInfo updateInfo)
        {
        }

        /// <summary>
        ///     Resolves the state instance of a state type using the configured state resolver and/or state cache.
        /// </summary>
        /// <param name="type"> The type of the state to resolve. </param>
        /// <returns>
        ///     The state instance or null if <paramref name="type" /> is null.
        /// </returns>
        /// <exception cref="StateNotFoundException"> The state instance specified by <paramref name="type" /> cannot be resolved. </exception>
        protected IState Resolve (Type type)
        {
            if (type == null)
            {
                return null;
            }

            bool useCache;
            IStateCache cache;
            IStateResolver resolver;

            lock (this.Configuration.SyncRoot)
            {
                useCache = this.Configuration.CachingEnabled;
                cache = this.Configuration.Cache;
                resolver = this.Configuration.Resolver;
            }

            IState state = null;

            if (useCache)
            {
                lock (cache.SyncRoot)
                {
                    if (cache.ContainsState(type))
                    {
                        state = cache.GetState(type);
                    }
                }
            }

            if (state == null)
            {
                lock (resolver.SyncRoot)
                {
                    state = resolver.ResolveState(type);
                }
            }

            if (state == null)
            {
                throw new StateNotFoundException(type);
            }

            return state;
        }

        /// <summary>
        ///     Caches a state instance.
        /// </summary>
        /// <param name="state"> The state instance to cache. </param>
        /// <remarks>
        ///     <para>
        ///         The state instance is only cached if caching is globally enabled in configuration (<see cref="Configuration"/>) and <paramref name="state"/> uses caching (<see cref="IState.UseCaching"/>).
        ///     </para>
        /// </remarks>
        protected void Cache (IState state)
        {
            if (state == null)
            {
                return;
            }

            bool cacheEnabled;
            IStateCache cache;

            lock (this.Configuration.SyncRoot)
            {
                cacheEnabled = this.Configuration.CachingEnabled;
                cache = this.Configuration.Cache;
            }

            if (state.UseCaching && cacheEnabled)
            {
                cache.AddState(state);
            }
        }

        private void TransferStateVariables (IState previousState, IState nextState)
        {
            Dictionary<string, object> variables = new Dictionary<string, object>(StringComparerEx.Ordinal);

            if (previousState != null)
            {
                Dictionary<string, StateVariableInfo> stateVariableInfos = StateMachine.GetStateVariableInfos(previousState.GetType());
                foreach (KeyValuePair<string, StateVariableInfo> stateVariableInfo in stateVariableInfos)
                {
                    if (stateVariableInfo.Value.Attribute.Handling == StateVariableHandling.Unspecified)
                    {
                        continue;
                    }

                    if (stateVariableInfo.Value.Attribute.Handling == StateVariableHandling.Transient)
                    {
                        string name = stateVariableInfo.Key;
                        object value = stateVariableInfo.Value.GetMethod.Invoke(previousState, null);
                        variables.Add(name, value);
                    }

                    stateVariableInfo.Value.SetMethod.Invoke(previousState, new[] {stateVariableInfo.Value.DefaultValue});
                }
            }

            if (nextState != null)
            {
                Dictionary<string, StateVariableInfo> stateVariableInfos = StateMachine.GetStateVariableInfos(nextState.GetType());
                foreach (KeyValuePair<string, StateVariableInfo> stateVariableInfo in stateVariableInfos)
                {
                    if (stateVariableInfo.Value.Attribute.Handling == StateVariableHandling.Unspecified)
                    {
                        continue;
                    }

                    object value = stateVariableInfo.Value.DefaultValue;
                    if (stateVariableInfo.Value.Attribute.Handling == StateVariableHandling.Transient)
                    {
                        if (variables.ContainsKey(stateVariableInfo.Key))
                        {
                            value = variables[stateVariableInfo.Key];
                        }
                    }

                    stateVariableInfo.Value.SetMethod.Invoke(nextState, new[] {value});
                }
            }
        }

        #endregion




        #region Interface: ISynchronizable

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        public object SyncRoot { get; }

        #endregion




        #region Type: StateVariableInfo

        private sealed class StateVariableInfo
        {
            #region Instance Properties/Indexer

            public StateVariableAttribute Attribute { get; set; }

            public object DefaultValue { get; set; }

            public MethodInfo GetMethod { get; set; }

            public PropertyInfo Property { get; set; }

            public MethodInfo SetMethod { get; set; }

            #endregion
        }

        #endregion
    }
}
