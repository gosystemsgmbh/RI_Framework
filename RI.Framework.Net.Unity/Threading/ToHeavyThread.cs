using UnityEngine;

using ThreadPriority = System.Threading.ThreadPriority;




namespace RI.Framework.Threading
{
    /// <summary>
    ///     Yield instruction to move a coroutine to a <see cref="HeavyThread" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="ThreadMover" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    /// TODO: Option to specify thread configuration callback
    public sealed class ToHeavyThread : YieldInstruction
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="ToHeavyThread" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         <see cref="System.Threading.ThreadPriority.Normal" /> is used as the thread priority.
        ///     </para>
        /// </remarks>
        public ToHeavyThread ()
            : this(ThreadPriority.Normal)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="ToHeavyThread" />.
        /// </summary>
        /// <param name="priority"> The thread priority to use. </param>
        public ToHeavyThread (ThreadPriority priority)
        {
            this.Priority = priority;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the thread priority to use.
        /// </summary>
        /// <value>
        ///     The thread priority to use.
        /// </value>
        public ThreadPriority Priority { get; }

        #endregion
    }
}
