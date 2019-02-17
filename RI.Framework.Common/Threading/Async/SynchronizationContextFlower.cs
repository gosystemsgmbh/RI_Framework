using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;




namespace RI.Framework.Threading.Async
{
    /// <summary>
    ///     Implements a flower which flows the current <see cref="SynchronizationContext" /> around an <c> await </c>.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public sealed class SynchronizationContextFlower : CustomFlower
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SynchronizationContextFlower" />.
        /// </summary>
        /// <param name="task"> The <see cref="Task" /> to flow things around when awaited. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="task" /> is null. </exception>
        public SynchronizationContextFlower (Task task)
            : base(task)
        {
            this.SynchronizationContext = null;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SynchronizationContextFlower" />.
        /// </summary>
        /// <param name="taskAwaiter"> The <see cref="TaskAwaiter" /> to flow things around when awaited. </param>
        public SynchronizationContextFlower (TaskAwaiter taskAwaiter)
            : base(taskAwaiter)
        {
            this.SynchronizationContext = null;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SynchronizationContextFlower" />.
        /// </summary>
        /// <param name="configuredTaskAwaitable"> The <see cref="ConfiguredTaskAwaitable" /> to flow things around when awaited. </param>
        public SynchronizationContextFlower (ConfiguredTaskAwaitable configuredTaskAwaitable)
            : base(configuredTaskAwaitable)
        {
            this.SynchronizationContext = null;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SynchronizationContextFlower" />.
        /// </summary>
        /// <param name="customAwaiter"> The <see cref="CustomAwaiter" /> to flow things around when awaited. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="customAwaiter" /> is null. </exception>
        public SynchronizationContextFlower (CustomAwaiter customAwaiter)
            : base(customAwaiter)
        {
            this.SynchronizationContext = null;
        }

        #endregion




        #region Instance Properties/Indexer

        private SynchronizationContext SynchronizationContext { get; set; }

        #endregion




        #region Overrides

        /// <inheritdoc />
        protected override void Capture ()
        {
            this.SynchronizationContext = SynchronizationContext.Current;
        }

        /// <inheritdoc />
        protected override void Restore ()
        {
            SynchronizationContext.SetSynchronizationContext(this.SynchronizationContext);
        }

        #endregion
    }

    /// <summary>
    ///     Implements a flower which flows the current <see cref="SynchronizationContext" /> around an <c> await </c>.
    /// </summary>
    /// <typeparam name="T"> The type of the return value of the <c> await </c> that is flowed around. </typeparam>
    /// <threadsafety static="true" instance="true" />
    public sealed class SynchronizationContextFlower <T> : CustomFlower<T>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SynchronizationContextFlower{T}" />.
        /// </summary>
        /// <param name="task"> The <see cref="Task{T}" /> to flow things around when awaited. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="task" /> is null. </exception>
        public SynchronizationContextFlower (Task<T> task)
            : base(task)
        {
            this.SynchronizationContext = null;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SynchronizationContextFlower{T}" />.
        /// </summary>
        /// <param name="taskAwaiter"> The <see cref="TaskAwaiter{T}" /> to flow things around when awaited. </param>
        public SynchronizationContextFlower (TaskAwaiter<T> taskAwaiter)
            : base(taskAwaiter)
        {
            this.SynchronizationContext = null;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SynchronizationContextFlower{T}" />.
        /// </summary>
        /// <param name="configuredTaskAwaitable"> The <see cref="ConfiguredTaskAwaitable{T}" /> to flow things around when awaited. </param>
        public SynchronizationContextFlower (ConfiguredTaskAwaitable<T> configuredTaskAwaitable)
            : base(configuredTaskAwaitable)
        {
            this.SynchronizationContext = null;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SynchronizationContextFlower{T}" />.
        /// </summary>
        /// <param name="customAwaiter"> The <see cref="CustomAwaiter{T}" /> to flow things around when awaited. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="customAwaiter" /> is null. </exception>
        public SynchronizationContextFlower (CustomAwaiter<T> customAwaiter)
            : base(customAwaiter)
        {
            this.SynchronizationContext = null;
        }

        #endregion




        #region Instance Properties/Indexer

        private SynchronizationContext SynchronizationContext { get; set; }

        #endregion




        #region Overrides

        /// <inheritdoc />
        protected override void Capture ()
        {
            this.SynchronizationContext = SynchronizationContext.Current;
        }

        /// <inheritdoc />
        protected override void Restore ()
        {
            SynchronizationContext.SetSynchronizationContext(this.SynchronizationContext);
        }

        #endregion
    }
}
