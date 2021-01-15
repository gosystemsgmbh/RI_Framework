using System;
using System.Runtime.CompilerServices;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Threading.Async
{
    /// <summary>
    ///     Implements a base class to implement custom awaiters.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This custom awaiter has no return value so <c> await </c> on this custom awaiter returns void.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    /// TODO: Awaiter for signals/events
    public abstract class CustomAwaiter : ICriticalNotifyCompletion, ISynchronizable
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="CustomAwaiter" />.
        /// </summary>
        protected CustomAwaiter ()
        {
            this.SyncRoot = new object();
        }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Gets the awaiter as required by the compiler.
        /// </summary>
        /// <returns>
        ///     The awaiter, which is nothing else than this instance itself.
        /// </returns>
        public CustomAwaiter GetAwaiter () => this;

        #endregion




        #region Virtuals

        /// <summary>
        ///     Gets whether the awaiter is already completed.
        /// </summary>
        /// <value>
        ///     true if the awaiter is already completed, false otherwise.
        /// </value>
        public virtual bool IsCompleted => false;

        /// <summary>
        ///     Gets the result of the awaiter when completed.
        /// </summary>
        public virtual void GetResult ()
        {
        }

        #endregion




        #region Interface: ICriticalNotifyCompletion

        /// <summary>
        ///     Specifies the continuation when the awaiter completes, in case it is not already completed.
        /// </summary>
        /// <param name="continuation"> The continuation. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="continuation" /> is null. </exception>
        public abstract void OnCompleted (Action continuation);

        /// <inheritdoc cref="OnCompleted" />
        public virtual void UnsafeOnCompleted (Action continuation) => this.OnCompleted(continuation);

        #endregion




        #region Interface: ISynchronizable

        /// <inheritdoc />
        public bool IsSynchronized => true;

        /// <inheritdoc />
        public object SyncRoot { get; }

        #endregion
    }


    /// <summary>
    ///     Implements a base class to implement custom awaiters.
    /// </summary>
    /// <typeparam name="T"> The type of this custom awaiters return value. </typeparam>
    /// <remarks>
    ///     <para>
    ///         This custom awaiter has a return value so <c> await </c> on this custom awaiter returns <typeparamref name="T" />.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public abstract class CustomAwaiter <T> : ICriticalNotifyCompletion, ISynchronizable
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="CustomAwaiter{T}" />.
        /// </summary>
        protected CustomAwaiter ()
        {
            this.SyncRoot = new object();
        }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Gets the awaiter as required by the compiler.
        /// </summary>
        /// <returns>
        ///     The awaiter, which is nothing else than this instance itself.
        /// </returns>
        public CustomAwaiter<T> GetAwaiter () => this;

        #endregion




        #region Abstracts

        /// <summary>
        ///     Gets the result of the awaiter when completed.
        /// </summary>
        /// <returns>
        ///     The awaiters result (which is the <c> await </c> return value).
        /// </returns>
        public abstract T GetResult ();

        #endregion




        #region Virtuals

        /// <summary>
        ///     Gets whether the awaiter is already completed.
        /// </summary>
        /// <value>
        ///     true if the awaiter is already completed, false otherwise.
        /// </value>
        public virtual bool IsCompleted => false;

        #endregion




        #region Interface: ICriticalNotifyCompletion

        /// <summary>
        ///     Specifies the continuation when the awaiter completes, in case it is not already completed.
        /// </summary>
        /// <param name="continuation"> The continuation. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="continuation" /> is null. </exception>
        public abstract void OnCompleted (Action continuation);

        /// <inheritdoc cref="OnCompleted" />
        public virtual void UnsafeOnCompleted(Action continuation) => this.OnCompleted(continuation);

        #endregion




        #region Interface: ISynchronizable

        /// <inheritdoc />
        public bool IsSynchronized => true;

        /// <inheritdoc />
        public object SyncRoot { get; }

        #endregion
    }
}
