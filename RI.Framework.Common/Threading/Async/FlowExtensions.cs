using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;




namespace RI.Framework.Threading.Async
{
    /// <summary>
    ///     Provides utility/extension methods to allow various things to flow around <c>await</c>.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public static class FlowExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Creates an awaitable which flows the current <see cref="CultureInfo.CurrentCulture" /> and <see cref="CultureInfo.CurrentUICulture" /> around an <c>await</c>.
        /// </summary>
        /// <param name="task"> The <see cref="Task" /> to flow things around when awaited. </param>
        /// <returns>
        ///     The awaiter.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="task" /> is null. </exception>
        public static CultureFlower WithCurrentCulture(this Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            return new CultureFlower(task);
        }

        /// <summary>
        ///     Creates an awaitable which flows the current <see cref="CultureInfo.CurrentCulture" /> and <see cref="CultureInfo.CurrentUICulture" /> around an <c>await</c>.
        /// </summary>
        /// <param name="taskAwaiter"> The <see cref="TaskAwaiter" /> to flow things around when awaited. </param>
        /// <returns>
        ///     The awaiter.
        /// </returns>
        public static CultureFlower WithCurrentCulture(this TaskAwaiter taskAwaiter)
        {
            return new CultureFlower(taskAwaiter);
        }

        /// <summary>
        ///     Creates an awaitable which flows the current <see cref="CultureInfo.CurrentCulture" /> and <see cref="CultureInfo.CurrentUICulture" /> around an <c>await</c>.
        /// </summary>
        /// <param name="configuredTaskAwaitable"> The <see cref="ConfiguredTaskAwaitable" /> to flow things around when awaited. </param>
        /// <returns>
        ///     The awaiter.
        /// </returns>
        public static CultureFlower WithCurrentCulture(this ConfiguredTaskAwaitable configuredTaskAwaitable)
        {
            return new CultureFlower(configuredTaskAwaitable);
        }

        /// <summary>
        ///     Creates an awaitable which flows the current <see cref="CultureInfo.CurrentCulture" /> and <see cref="CultureInfo.CurrentUICulture" /> around an <c>await</c>.
        /// </summary>
        /// <param name="customAwaiter"> The <see cref="CustomAwaiter" /> to flow things around when awaited. </param>
        /// <returns>
        ///     The awaiter.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="customAwaiter" /> is null. </exception>
        public static CultureFlower WithCurrentCulture(this CustomAwaiter customAwaiter)
        {
            if (customAwaiter == null)
            {
                throw new ArgumentNullException(nameof(customAwaiter));
            }

            return new CultureFlower(customAwaiter);
        }

        /// <summary>
        ///     Creates an awaitable which flows the current <see cref="CultureInfo.CurrentCulture" /> and <see cref="CultureInfo.CurrentUICulture" /> around an <c>await</c>.
        /// </summary>
        /// <typeparam name="T">The type of the task result.</typeparam>
        /// <param name="task"> The <see cref="Task{T}" /> to flow things around when awaited. </param>
        /// <returns>
        ///     The awaiter.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="task" /> is null. </exception>
        public static CultureFlower<T> WithCurrentCulture<T>(this Task<T> task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            return new CultureFlower<T>(task);
        }

        /// <summary>
        ///     Creates an awaitable which flows the current <see cref="CultureInfo.CurrentCulture" /> and <see cref="CultureInfo.CurrentUICulture" /> around an <c>await</c>.
        /// </summary>
        /// <typeparam name="T">The type of the task result.</typeparam>
        /// <param name="taskAwaiter"> The <see cref="TaskAwaiter{T}" /> to flow things around when awaited. </param>
        /// <returns>
        ///     The awaiter.
        /// </returns>
        public static CultureFlower<T> WithCurrentCulture<T>(this TaskAwaiter<T> taskAwaiter)
        {
            return new CultureFlower<T>(taskAwaiter);
        }

        /// <summary>
        ///     Creates an awaitable which flows the current <see cref="CultureInfo.CurrentCulture" /> and <see cref="CultureInfo.CurrentUICulture" /> around an <c>await</c>.
        /// </summary>
        /// <typeparam name="T">The type of the task result.</typeparam>
        /// <param name="configuredTaskAwaitable"> The <see cref="ConfiguredTaskAwaitable{T}" /> to flow things around when awaited. </param>
        /// <returns>
        ///     The awaiter.
        /// </returns>
        public static CultureFlower<T> WithCurrentCulture<T>(this ConfiguredTaskAwaitable<T> configuredTaskAwaitable)
        {
            return new CultureFlower<T>(configuredTaskAwaitable);
        }

        /// <summary>
        ///     Creates an awaitable which flows the current <see cref="CultureInfo.CurrentCulture" /> and <see cref="CultureInfo.CurrentUICulture" /> around an <c>await</c>.
        /// </summary>
        /// <typeparam name="T">The type of the task result.</typeparam>
        /// <param name="customAwaiter"> The <see cref="CustomAwaiter{T}" /> to flow things around when awaited. </param>
        /// <returns>
        ///     The awaiter.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="customAwaiter" /> is null. </exception>
        public static CultureFlower<T> WithCurrentCulture<T>(this CustomAwaiter<T> customAwaiter)
        {
            if (customAwaiter == null)
            {
                throw new ArgumentNullException(nameof(customAwaiter));
            }

            return new CultureFlower<T>(customAwaiter);
        }

        /// <summary>
        ///     Creates an awaitable which flows the current <see cref="SynchronizationContext" /> around an <c>await</c>.
        /// </summary>
        /// <param name="task"> The <see cref="Task" /> to flow things around when awaited. </param>
        /// <returns>
        ///     The awaiter.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="task" /> is null. </exception>
        public static SynchronizationContextFlower WithSynchronizationContext(this Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            return new SynchronizationContextFlower(task);
        }

        /// <summary>
        ///     Creates an awaitable which flows the current <see cref="SynchronizationContext" /> around an <c>await</c>.
        /// </summary>
        /// <param name="taskAwaiter"> The <see cref="TaskAwaiter" /> to flow things around when awaited. </param>
        /// <returns>
        ///     The awaiter.
        /// </returns>
        public static SynchronizationContextFlower WithSynchronizationContext(this TaskAwaiter taskAwaiter)
        {
            return new SynchronizationContextFlower(taskAwaiter);
        }

        /// <summary>
        ///     Creates an awaitable which flows the current <see cref="SynchronizationContext" /> around an <c>await</c>.
        /// </summary>
        /// <param name="configuredTaskAwaitable"> The <see cref="ConfiguredTaskAwaitable" /> to flow things around when awaited. </param>
        /// <returns>
        ///     The awaiter.
        /// </returns>
        public static SynchronizationContextFlower WithSynchronizationContext(this ConfiguredTaskAwaitable configuredTaskAwaitable)
        {
            return new SynchronizationContextFlower(configuredTaskAwaitable);
        }

        /// <summary>
        ///     Creates an awaitable which flows the current <see cref="SynchronizationContext" /> around an <c>await</c>.
        /// </summary>
        /// <param name="customAwaiter"> The <see cref="CustomAwaiter" /> to flow things around when awaited. </param>
        /// <returns>
        ///     The awaiter.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="customAwaiter" /> is null. </exception>
        public static SynchronizationContextFlower WithSynchronizationContext(this CustomAwaiter customAwaiter)
        {
            if (customAwaiter == null)
            {
                throw new ArgumentNullException(nameof(customAwaiter));
            }

            return new SynchronizationContextFlower(customAwaiter);
        }

        /// <summary>
        ///     Creates an awaitable which flows the current <see cref="SynchronizationContext" /> around an <c>await</c>.
        /// </summary>
        /// <typeparam name="T">The type of the task result.</typeparam>
        /// <param name="task"> The <see cref="Task{T}" /> to flow things around when awaited. </param>
        /// <returns>
        ///     The awaiter.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="task" /> is null. </exception>
        public static SynchronizationContextFlower<T> WithSynchronizationContext<T>(this Task<T> task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            return new SynchronizationContextFlower<T>(task);
        }

        /// <summary>
        ///     Creates an awaitable which flows the current <see cref="SynchronizationContext" /> around an <c>await</c>.
        /// </summary>
        /// <typeparam name="T">The type of the task result.</typeparam>
        /// <param name="taskAwaiter"> The <see cref="TaskAwaiter{T}" /> to flow things around when awaited. </param>
        /// <returns>
        ///     The awaiter.
        /// </returns>
        public static SynchronizationContextFlower<T> WithSynchronizationContext<T>(this TaskAwaiter<T> taskAwaiter)
        {
            return new SynchronizationContextFlower<T>(taskAwaiter);
        }

        /// <summary>
        ///     Creates an awaitable which flows the current <see cref="SynchronizationContext" /> around an <c>await</c>.
        /// </summary>
        /// <typeparam name="T">The type of the task result.</typeparam>
        /// <param name="configuredTaskAwaitable"> The <see cref="ConfiguredTaskAwaitable{T}" /> to flow things around when awaited. </param>
        /// <returns>
        ///     The awaiter.
        /// </returns>
        public static SynchronizationContextFlower<T> WithSynchronizationContext<T>(this ConfiguredTaskAwaitable<T> configuredTaskAwaitable)
        {
            return new SynchronizationContextFlower<T>(configuredTaskAwaitable);
        }

        /// <summary>
        ///     Creates an awaitable which flows the current <see cref="SynchronizationContext" /> around an <c>await</c>.
        /// </summary>
        /// <typeparam name="T">The type of the task result.</typeparam>
        /// <param name="customAwaiter"> The <see cref="CustomAwaiter{T}" /> to flow things around when awaited. </param>
        /// <returns>
        ///     The awaiter.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="customAwaiter" /> is null. </exception>
        public static SynchronizationContextFlower<T> WithSynchronizationContext<T>(this CustomAwaiter<T> customAwaiter)
        {
            if (customAwaiter == null)
            {
                throw new ArgumentNullException(nameof(customAwaiter));
            }

            return new SynchronizationContextFlower<T>(customAwaiter);
        }

        #endregion
    }
}
