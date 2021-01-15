using System;

using RI.Framework.StateMachines.States;




namespace RI.Framework.StateMachines
{
    /// <summary>
    ///     Contains all information about an update.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public sealed class StateUpdateInfo
    {
        #region Instance Constructor/Destructor

        internal StateUpdateInfo (StateMachine stateMachine, IState updateState, int updateDelay, DateTime stateEnteredUtc, DateTime stateEnteredLocal)
        {
            if (stateMachine == null)
            {
                throw new ArgumentNullException(nameof(stateMachine));
            }

            this.StateMachine = stateMachine;
            this.UpdateState = updateState;
            this.UpdateDelay = updateDelay;
            this.StateEnteredUtc = stateEnteredUtc;
            this.StateEnteredLocal = stateEnteredLocal;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the local timestamp when the current state became active.
        /// </summary>
        /// <value>
        ///     The local timestamp when the current state became active.
        /// </value>
        public DateTime StateEnteredLocal { get; }

        /// <summary>
        ///     Gets the UTC timestamp when the current state became active.
        /// </summary>
        /// <value>
        ///     The UTC timestamp when the current state became active.
        /// </value>
        public DateTime StateEnteredUtc { get; }

        /// <summary>
        ///     Gets the state machine associated with the update.
        /// </summary>
        /// <value>
        ///     The state machine associated with the update.
        /// </value>
        public StateMachine StateMachine { get; }

        /// <summary>
        ///     Gets the delay of the update in milliseconds.
        /// </summary>
        /// <value>
        ///     The delay of the update in milliseconds.
        ///     Zero if the update is to be executed immediately.
        /// </value>
        public int UpdateDelay { get; }

        /// <summary>
        ///     Gets the state for which this update is intended.
        /// </summary>
        /// <value>
        ///     The state for which this update is intended.
        /// </value>
        public IState UpdateState { get; }

        #endregion
    }
}
