using System;

using RI.Framework.Threading.Async;




namespace RI.Framework.Services.Dispatcher
{
    /// <summary>
    ///     Implements an awaiter which moves execution to a specified <see cref="IDispatcherService" />.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    /// TODO: Option to specify priority?
    public sealed class DispatcherServiceAwaiter : CustomAwaiter
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DispatcherServiceAwaiter" />.
        /// </summary>
        /// <param name="dispatcher"> The used <see cref="IDispatcherService" />. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
        public DispatcherServiceAwaiter(IDispatcherService dispatcher)
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
        ///     Gets the used <see cref="IDispatcherService" />.
        /// </summary>
        /// <value>
        ///     The used <see cref="IDispatcherService" />.
        /// </value>
        public IDispatcherService Dispatcher { get; }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override void OnCompleted (Action continuation)
        {
            if (continuation == null)
            {
                throw new ArgumentNullException(nameof(continuation));
            }

            this.Dispatcher.Dispatch(continuation);
        }

        #endregion
    }
}
