using System;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Threading.Dispatcher
{
    /// <summary>
    ///     Implements a <see cref="TaskFactory" /> which uses a <see cref="IThreadDispatcher" /> for execution.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public sealed class ThreadDispatcherTaskFactory : TaskFactory, ISynchronizable
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="ThreadDispatcherTaskFactory" />.
        /// </summary>
        /// <param name="dispatcher"> The used dispatcher. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
        public ThreadDispatcherTaskFactory (IThreadDispatcher dispatcher)
            : base(dispatcher.GetTaskScheduler())
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




        #region Interface: ISynchronizable

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        object ISynchronizable.SyncRoot => this.SyncRoot;

        #endregion
    }
}
