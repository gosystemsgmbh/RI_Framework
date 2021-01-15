using System;

using RI.Framework.Threading.Async;




namespace RI.Framework.Threading.Dispatcher
{
    /// <summary>
    ///     Implements an awaiter which moves execution to a specified <see cref="IThreadDispatcher" />.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    /// TODO: Option to specify priority and options?
    public sealed class ThreadDispatcherAwaiter : CustomAwaiter
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="ThreadDispatcherAwaiter" />.
        /// </summary>
        /// <param name="dispatcher"> The used <see cref="IThreadDispatcher" />. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
        public ThreadDispatcherAwaiter (IThreadDispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            this.Dispatcher = dispatcher;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the used <see cref="IThreadDispatcher" />.
        /// </summary>
        /// <value>
        ///     The used <see cref="IThreadDispatcher" />.
        /// </value>
        public IThreadDispatcher Dispatcher { get; }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override void OnCompleted (Action continuation)
        {
            if (continuation == null)
            {
                throw new ArgumentNullException(nameof(continuation));
            }

            this.Dispatcher.Post(this.Dispatcher.GetCurrentPriorityOrDefault(), this.Dispatcher.GetCurrentOptionsOrDefault(), continuation);
        }

        #endregion
    }
}
