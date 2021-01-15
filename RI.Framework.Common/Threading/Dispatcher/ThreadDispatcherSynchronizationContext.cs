using System;
using System.Threading;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Threading.Dispatcher
{
    /// <summary>
    ///     Implements a <see cref="SynchronizationContext" /> which uses a <see cref="IThreadDispatcher" /> for execution.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public sealed class ThreadDispatcherSynchronizationContext : SynchronizationContext, ISynchronizable
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="ThreadDispatcherSynchronizationContext" />.
        /// </summary>
        /// <param name="dispatcher"> The used dispatcher. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
        public ThreadDispatcherSynchronizationContext (IThreadDispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            this.SyncRoot = new object();
            this.Dispatcher = dispatcher;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the used dispatcher.
        /// </summary>
        /// <value>
        ///     The used dispatcher.
        /// </value>
        public IThreadDispatcher Dispatcher { get; }

        private object SyncRoot { get; }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override SynchronizationContext CreateCopy ()
        {
            return new ThreadDispatcherSynchronizationContext(this.Dispatcher);
        }

        /// <inheritdoc />
        public override void Post (SendOrPostCallback d, object state)
        {
            lock (this.SyncRoot)
            {
                this.Dispatcher.Post(this.Dispatcher.GetCurrentPriorityOrDefault(), this.Dispatcher.GetCurrentOptionsOrDefault(), new Action<SendOrPostCallback, object>((x, y) => x(y)), d, state);
            }
        }

        /// <inheritdoc />
        public override void Send (SendOrPostCallback d, object state)
        {
            lock (this.SyncRoot)
            {
                this.Dispatcher.Send(this.Dispatcher.GetCurrentPriorityOrDefault(), this.Dispatcher.GetCurrentOptionsOrDefault(), new Action<SendOrPostCallback, object>((x, y) => x(y)), d, state);
            }
        }

        #endregion




        #region Interface: ISynchronizable

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        object ISynchronizable.SyncRoot => this.SyncRoot;

        #endregion
    }
}
