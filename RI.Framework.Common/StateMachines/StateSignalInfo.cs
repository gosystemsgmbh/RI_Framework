using System;
using System.Collections.Generic;




namespace RI.Framework.StateMachines
{
    /// <summary>
    ///     Contains all information about a signal.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public sealed class StateSignalInfo
    {
        #region Instance Constructor/Destructor

        internal StateSignalInfo (StateMachine stateMachine, object signal, IEnumerable<object> parameters)
        {
            if (stateMachine == null)
            {
                throw new ArgumentNullException(nameof(stateMachine));
            }

            this.StateMachine = stateMachine;
            this.Signal = signal;
            this.Parameters = new List<object>(parameters ?? new object[0]);
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the signal.
        /// </summary>
        /// <value>
        ///     The signal.
        /// </value>
        public object Signal { get; }

        /// <summary>
        ///     Gets the list of signal parameters.
        /// </summary>
        /// <value>
        ///     The list of signal parameters.
        ///     An empty list is returned if no signal parameters were specified.
        /// </value>
        public List<object> Parameters { get; }

        /// <summary>
        ///     Gets the state machine associated with the signal.
        /// </summary>
        /// <value>
        ///     The state machine associated with the signal.
        /// </value>
        public StateMachine StateMachine { get; }

        #endregion
    }
}
