﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Threading;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.StateMachines.Dispatchers
{
    /// <summary>
    ///     Implements a state machine operation dispatcher which uses <see cref="System.Windows.Threading.Dispatcher" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         State machine operations are dispatched using <see cref="System.Windows.Threading.Dispatcher.BeginInvoke(DispatcherPriority,Delegate,object)" />.
    ///     </para>
    ///     <para>
    ///         See <see cref="IStateDispatcher" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
    public sealed class WpfStateDispatcher : IStateDispatcher, IDisposable
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="WpfStateDispatcher" />.
        /// </summary>
        /// <param name="application"> The application object to get the dispatcher from. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="application" /> is null. </exception>
        public WpfStateDispatcher (Application application)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            this.SyncRoot = new object();
            this.Dispatcher = application.Dispatcher;
            this.UpdateTimers = new Dictionary<StateMachine, DispatcherTimer>();
            this.UpdateCallbackHandler = this.UpdateCallback;

            this.Priority = DispatcherPriority.Normal;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="WpfStateDispatcher" />.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher to use. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
        public WpfStateDispatcher (Dispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            this.SyncRoot = new object();
            this.Dispatcher = dispatcher;
            this.UpdateTimers = new Dictionary<StateMachine, DispatcherTimer>();
            this.UpdateCallbackHandler = this.UpdateCallback;

            this.Priority = DispatcherPriority.Normal;
        }

        /// <summary>
        ///     Garbage collects this instance of <see cref="WpfStateDispatcher" />.
        /// </summary>
        ~WpfStateDispatcher ()
        {
            this.Dispose(false);
        }

        #endregion




        #region Instance Fields

        private DispatcherPriority _priority;

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the used dispatcher.
        /// </summary>
        /// <value>
        ///     The used dispatcher.
        /// </value>
        public Dispatcher Dispatcher { get; }

        /// <summary>
        ///     Gets or sets the priority used for dispatching messages.
        /// </summary>
        /// <value>
        ///     The priority used for dispatching messages.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is <see cref="DispatcherPriority.Normal" />.
        ///     </para>
        /// </remarks>
        public DispatcherPriority Priority
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._priority;
                }
            }
            set
            {
                lock (this.SyncRoot)
                {
                    this._priority = value;
                }
            }
        }

        private EventHandler UpdateCallbackHandler { get; }

        private Dictionary<StateMachine, DispatcherTimer> UpdateTimers { get; }

        #endregion




        #region Instance Methods

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private void Dispose (bool disposing)
        {
            lock (this.SyncRoot)
            {
                foreach (KeyValuePair<StateMachine, DispatcherTimer> timer in this.UpdateTimers)
                {
                    timer.Value.Stop();
                }

                this.UpdateTimers.Clear();
            }
        }

        private void UpdateCallback (object sender, object args)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            Tuple<StateMachineUpdateDelegate, StateUpdateInfo> tag = (Tuple<StateMachineUpdateDelegate, StateUpdateInfo>)timer.Tag;
            tag.Item1.Invoke(tag.Item2);
        }

        #endregion




        #region Interface: IDisposable

        /// <inheritdoc />
        public void Dispose ()
        {
            this.Dispose(true);
        }

        #endregion




        #region Interface: IStateDispatcher

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        public object SyncRoot { get; }

        /// <inheritdoc />
        public void DispatchSignal (StateMachineSignalDelegate signalDelegate, StateSignalInfo signalInfo)
        {
            lock (this.SyncRoot)
            {
                this.Dispatcher.BeginInvoke(this.Priority, signalDelegate, signalInfo);
            }
        }

        /// <inheritdoc />
        public void DispatchTransition (StateMachineTransientDelegate transientDelegate, StateTransientInfo transientInfo)
        {
            lock (this.SyncRoot)
            {
                this.Dispatcher.BeginInvoke(this.Priority, transientDelegate, transientInfo);
            }
        }

        /// <inheritdoc />
        public void DispatchUpdate (StateMachineUpdateDelegate updateDelegate, StateUpdateInfo updateInfo)
        {
            lock (this.SyncRoot)
            {
                StateMachine stateMachine = updateInfo.StateMachine;

                if (this.UpdateTimers.ContainsKey(stateMachine))
                {
                    this.UpdateTimers[stateMachine].Stop();
                    this.UpdateTimers.Remove(stateMachine);
                }

                DispatcherTimer timer = new DispatcherTimer(TimeSpan.FromMilliseconds(updateInfo.UpdateDelay), this.Priority, this.UpdateCallbackHandler, this.Dispatcher);
                timer.Tag = new Tuple<StateMachineUpdateDelegate, StateUpdateInfo>(updateDelegate, updateInfo);
                this.UpdateTimers.Add(stateMachine, timer);
                timer.Start();
            }
        }

        #endregion
    }
}
