using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Threading.Async
{
    /// <summary>
    ///     Implements a base class to implement custom flowers.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Obviously, &quot;flowers&quot; as in &quot;flow&quot;ers, not the nice things you buy your partner on Valentines day...
    ///     </para>
    ///     <para>
    ///         Custom flowers are used to flow certain things around an <c> await </c> by capturing and restoring them.
    ///     </para>
    ///     <note type="important">
    ///         Some virtual methods are called from within locks to <see cref="ISynchronizable.SyncRoot" />.
    ///         Be careful in inheriting classes when calling outside code from those methods (e.g. through events, callbacks, or other virtual methods) to not produce deadlocks!
    ///     </note>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public abstract class CustomFlower : CustomAwaiter
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="CustomFlower" />.
        /// </summary>
        /// <param name="task"> The <see cref="Task" /> to flow things around when awaited. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="task" /> is null. </exception>
        protected CustomFlower (Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            this.TaskAwaiter = task.GetAwaiter();
            this.ConfiguredTaskAwaiter = null;
            this.CustomAwaiter = null;

            this.IsCaptured = false;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CustomFlower" />.
        /// </summary>
        /// <param name="taskAwaiter"> The <see cref="TaskAwaiter" /> to flow things around when awaited. </param>
        protected CustomFlower (TaskAwaiter taskAwaiter)
        {
            this.TaskAwaiter = taskAwaiter;
            this.ConfiguredTaskAwaiter = null;
            this.CustomAwaiter = null;

            this.IsCaptured = false;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CustomFlower" />.
        /// </summary>
        /// <param name="configuredTaskAwaitable"> The <see cref="ConfiguredTaskAwaitable" /> to flow things around when awaited. </param>
        protected CustomFlower (ConfiguredTaskAwaitable configuredTaskAwaitable)
        {
            this.TaskAwaiter = null;
            this.ConfiguredTaskAwaiter = configuredTaskAwaitable.GetAwaiter();
            this.CustomAwaiter = null;

            this.IsCaptured = false;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CustomFlower" />.
        /// </summary>
        /// <param name="customAwaiter"> The <see cref="CustomAwaiter" /> to flow things around when awaited. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="customAwaiter" /> is null. </exception>
        protected CustomFlower (CustomAwaiter customAwaiter)
        {
            this.TaskAwaiter = null;
            this.ConfiguredTaskAwaiter = null;
            this.CustomAwaiter = customAwaiter.GetAwaiter();

            this.IsCaptured = false;
        }

        #endregion




        #region Instance Fields

        private bool _isCaptured;

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets whether the flow was captured and needs restore.
        /// </summary>
        /// <value>
        ///     true if the flow was captured and needs restore, false otherwise.
        /// </value>
        public bool IsCaptured
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._isCaptured;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._isCaptured = value;
                }
            }
        }

        private ConfiguredTaskAwaitable.ConfiguredTaskAwaiter? ConfiguredTaskAwaiter { get; }

        private CustomAwaiter CustomAwaiter { get; }

        private TaskAwaiter? TaskAwaiter { get; }

        #endregion




        #region Abstracts

        /// <summary>
        ///     Called to capture what needs to flow around the <c> await </c>.
        /// </summary>
        /// <remarks>
        ///     <note type="important">
        ///         This method is called inside a lock to <see cref="ISynchronizable.SyncRoot" />.
        ///     </note>
        /// </remarks>
        protected abstract void Capture ();

        /// <summary>
        ///     Called to restore what needs to flow around the <c> await </c>.
        /// </summary>
        /// <remarks>
        ///     <note type="important">
        ///         This method is called inside a lock to <see cref="ISynchronizable.SyncRoot" />.
        ///     </note>
        /// </remarks>
        protected abstract void Restore ();

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override bool IsCompleted => this.TaskAwaiter?.IsCompleted ?? this.ConfiguredTaskAwaiter?.IsCompleted ?? this.CustomAwaiter.IsCompleted;

        /// <inheritdoc />
        public override void GetResult ()
        {
            lock (this.SyncRoot)
            {
                if (this.IsCaptured)
                {
                    this.Restore();
                }
            }

            if (this.TaskAwaiter != null)
            {
                this.TaskAwaiter.Value.GetResult();
                return;
            }

            if (this.ConfiguredTaskAwaiter != null)
            {
                this.ConfiguredTaskAwaiter.Value.GetResult();
                return;
            }

            this.CustomAwaiter.GetResult();
        }

        /// <inheritdoc />
        public override void OnCompleted (Action continuation)
        {
            if (continuation == null)
            {
                throw new ArgumentNullException(nameof(continuation));
            }

            lock (this.SyncRoot)
            {
                this.Capture();

                this.IsCaptured = true;
            }

            if (this.TaskAwaiter != null)
            {
                this.TaskAwaiter.Value.OnCompleted(continuation);
                return;
            }

            if (this.ConfiguredTaskAwaiter != null)
            {
                this.ConfiguredTaskAwaiter.Value.OnCompleted(continuation);
                return;
            }

            this.CustomAwaiter.OnCompleted(continuation);
        }

        /// <inheritdoc />
        public override void UnsafeOnCompleted (Action continuation)
        {
            if (continuation == null)
            {
                throw new ArgumentNullException(nameof(continuation));
            }

            lock (this.SyncRoot)
            {
                this.Capture();

                this.IsCaptured = true;
            }

            if (this.TaskAwaiter != null)
            {
                this.TaskAwaiter.Value.UnsafeOnCompleted(continuation);
                return;
            }

            if (this.ConfiguredTaskAwaiter != null)
            {
                this.ConfiguredTaskAwaiter.Value.UnsafeOnCompleted(continuation);
                return;
            }

            this.CustomAwaiter.UnsafeOnCompleted(continuation);
        }

        #endregion
    }

    /// <summary>
    ///     Implements a base class to implement custom flowers.
    /// </summary>
    /// <typeparam name="T"> The type of the return value of the <c> await </c> that is flowed around. </typeparam>
    /// <remarks>
    ///     <para>
    ///         Obviously, &quot;flowers&quot; as in &quot;flow&quot;ers, not the nice things you buy your partner on Valentines day...
    ///     </para>
    ///     <para>
    ///         Custom flowers are used to flow certain things around an <c> await </c> by capturing and restoring them.
    ///     </para>
    ///     <note type="important">
    ///         Some virtual methods are called from within locks to <see cref="ISynchronizable.SyncRoot" />.
    ///         Be careful in inheriting classes when calling outside code from those methods (e.g. through events, callbacks, or other virtual methods) to not produce deadlocks!
    ///     </note>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public abstract class CustomFlower <T> : CustomAwaiter<T>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="CustomFlower" />.
        /// </summary>
        /// <param name="task"> The <see cref="Task{T}" /> to flow things around when awaited. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="task" /> is null. </exception>
        protected CustomFlower (Task<T> task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            this.TaskAwaiter = task.GetAwaiter();
            this.ConfiguredTaskAwaiter = null;
            this.CustomAwaiter = null;

            this.IsCaptured = false;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CustomFlower" />.
        /// </summary>
        /// <param name="taskAwaiter"> The <see cref="TaskAwaiter{T}" /> to flow things around when awaited. </param>
        protected CustomFlower (TaskAwaiter<T> taskAwaiter)
        {
            this.TaskAwaiter = taskAwaiter;
            this.ConfiguredTaskAwaiter = null;
            this.CustomAwaiter = null;

            this.IsCaptured = false;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CustomFlower" />.
        /// </summary>
        /// <param name="configuredTaskAwaitable"> The <see cref="ConfiguredTaskAwaitable{T}" /> to flow things around when awaited. </param>
        protected CustomFlower (ConfiguredTaskAwaitable<T> configuredTaskAwaitable)
        {
            this.TaskAwaiter = null;
            this.ConfiguredTaskAwaiter = configuredTaskAwaitable.GetAwaiter();
            this.CustomAwaiter = null;

            this.IsCaptured = false;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CustomFlower" />.
        /// </summary>
        /// <param name="customAwaiter"> The <see cref="CustomAwaiter{T}" /> to flow things around when awaited. </param>
        protected CustomFlower (CustomAwaiter<T> customAwaiter)
        {
            this.TaskAwaiter = null;
            this.ConfiguredTaskAwaiter = null;
            this.CustomAwaiter = customAwaiter.GetAwaiter();

            this.IsCaptured = false;
        }

        #endregion




        #region Instance Fields

        private bool _isCaptured;

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets whether the flow was captured and needs restore.
        /// </summary>
        /// <value>
        ///     true if the flow was captured and needs restore, false otherwise.
        /// </value>
        public bool IsCaptured
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._isCaptured;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._isCaptured = value;
                }
            }
        }

        private ConfiguredTaskAwaitable<T>.ConfiguredTaskAwaiter? ConfiguredTaskAwaiter { get; }

        private CustomAwaiter<T> CustomAwaiter { get; }

        private TaskAwaiter<T>? TaskAwaiter { get; }

        #endregion




        #region Abstracts

        /// <summary>
        ///     Called to capture what needs to flow around the <c> await </c>.
        /// </summary>
        /// <remarks>
        ///     <note type="important">
        ///         This method is called inside a lock to <see cref="ISynchronizable.SyncRoot" />.
        ///     </note>
        /// </remarks>
        protected abstract void Capture ();

        /// <summary>
        ///     Called to restore what needs to flow around the <c> await </c>.
        /// </summary>
        /// <remarks>
        ///     <note type="important">
        ///         This method is called inside a lock to <see cref="ISynchronizable.SyncRoot" />.
        ///     </note>
        /// </remarks>
        protected abstract void Restore ();

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override bool IsCompleted => this.TaskAwaiter?.IsCompleted ?? this.ConfiguredTaskAwaiter?.IsCompleted ?? this.CustomAwaiter.IsCompleted;

        /// <inheritdoc />
        public override T GetResult ()
        {
            lock (this.SyncRoot)
            {
                if (this.IsCaptured)
                {
                    this.Restore();
                }
            }

            if (this.TaskAwaiter != null)
            {
                return this.TaskAwaiter.Value.GetResult();
            }

            if (this.ConfiguredTaskAwaiter != null)
            {
                return this.ConfiguredTaskAwaiter.Value.GetResult();
            }

            return this.CustomAwaiter.GetResult();
        }

        /// <inheritdoc />
        public override void OnCompleted (Action continuation)
        {
            if (continuation == null)
            {
                throw new ArgumentNullException(nameof(continuation));
            }

            lock (this.SyncRoot)
            {
                this.Capture();

                this.IsCaptured = true;
            }

            if (this.TaskAwaiter != null)
            {
                this.TaskAwaiter.Value.OnCompleted(continuation);
                return;
            }

            if (this.ConfiguredTaskAwaiter != null)
            {
                this.ConfiguredTaskAwaiter.Value.OnCompleted(continuation);
                return;
            }

            this.CustomAwaiter.OnCompleted(continuation);
        }

        /// <inheritdoc />
        public override void UnsafeOnCompleted (Action continuation)
        {
            if (continuation == null)
            {
                throw new ArgumentNullException(nameof(continuation));
            }

            lock (this.SyncRoot)
            {
                this.Capture();

                this.IsCaptured = true;
            }

            if (this.TaskAwaiter != null)
            {
                this.TaskAwaiter.Value.UnsafeOnCompleted(continuation);
                return;
            }

            if (this.ConfiguredTaskAwaiter != null)
            {
                this.ConfiguredTaskAwaiter.Value.UnsafeOnCompleted(continuation);
                return;
            }

            this.CustomAwaiter.UnsafeOnCompleted(continuation);
        }

        #endregion
    }
}
