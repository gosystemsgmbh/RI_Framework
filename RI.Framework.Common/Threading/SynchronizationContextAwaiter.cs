using System;
using System.Threading;

using RI.Framework.Threading.Async;




namespace RI.Framework.Threading
{
    /// <summary>
    ///     Implements an awaiter which moves execution to a specified <see cref="SynchronizationContext" />.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    /// TODO: Option to specify state?
    public sealed class SynchronizationContextAwaiter : CustomAwaiter
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SynchronizationContextAwaiter" />.
        /// </summary>
        /// <param name="synchronizationContext"> The used <see cref="SynchronizationContext" />. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="synchronizationContext" /> is null. </exception>
        public SynchronizationContextAwaiter (SynchronizationContext synchronizationContext)
        {
            if (synchronizationContext == null)
            {
                throw new ArgumentNullException(nameof(synchronizationContext));
            }

            this.SynchronizationContext = synchronizationContext;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the used <see cref="SynchronizationContext" />.
        /// </summary>
        /// <value>
        ///     The used <see cref="SynchronizationContext" />.
        /// </value>
        public SynchronizationContext SynchronizationContext { get; }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override void OnCompleted (Action continuation)
        {
            if (continuation == null)
            {
                throw new ArgumentNullException(nameof(continuation));
            }

            this.SynchronizationContext.Post(x =>
            {
                Action cont = (Action)x;
                cont();
            }, continuation);
        }

        #endregion
    }
}
