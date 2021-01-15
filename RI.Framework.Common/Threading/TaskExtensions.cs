using System;
using System.Collections.Generic;
using System.Threading.Tasks;




namespace RI.Framework.Threading
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="Task" /> type.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    /// TODO: Add overloads for Any, All
    /// TODO: Add WithTimeout
    /// TODO: Add WithCancellation
    public static class TaskExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Creates a task which waits for all tasks to be completed.
        /// </summary>
        /// <param name="tasks"> The tasks to wait for. </param>
        /// <returns>
        ///     The created task which waits for all tasks in the <paramref name="tasks"/> sequence.
        /// </returns>
        /// <remarks>
        /// <para>
        /// <paramref name="tasks"/> is enumerated only once.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="tasks" /> is null. </exception>
        public static Task All (this IEnumerable<Task> tasks)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            return Task.WhenAll(tasks);
        }

        /// <summary>
        ///     Creates a task which waits for any task to be completed.
        /// </summary>
        /// <param name="tasks"> The tasks to wait for. </param>
        /// <returns>
        ///     The created task which waits for any task in the <paramref name="tasks"/> sequence and returns the first completed task.
        /// </returns>
        /// <remarks>
        /// <para>
        /// <paramref name="tasks"/> is enumerated only once.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="tasks" /> is null. </exception>
        public static Task<Task> Any (this IEnumerable<Task> tasks)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            return Task.WhenAny(tasks);
        }

        /// <summary>
        ///     Creates a task which waits for all tasks to be completed.
        /// </summary>
        /// <typeparam name="T">The type of the task results.</typeparam>
        /// <param name="tasks"> The tasks to wait for. </param>
        /// <returns>
        ///     The created task which waits for all tasks in the <paramref name="tasks"/> sequence and returns all the results.
        /// </returns>
        /// <remarks>
        /// <para>
        /// <paramref name="tasks"/> is enumerated only once.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="tasks" /> is null. </exception>
        public static Task<T[]> All<T>(this IEnumerable<Task<T>> tasks)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            return Task.WhenAll(tasks);
        }

        /// <summary>
        ///     Creates a task which waits for any task to be completed.
        /// </summary>
        /// <typeparam name="T">The type of the task results.</typeparam>
        /// <param name="tasks"> The tasks to wait for. </param>
        /// <returns>
        ///     The created task which waits for any task in the <paramref name="tasks"/> sequence and returns the first completed task.
        /// </returns>
        /// <remarks>
        /// <para>
        /// <paramref name="tasks"/> is enumerated only once.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="tasks" /> is null. </exception>
        public static Task<Task<T>> Any<T>(this IEnumerable<Task<T>> tasks)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            return Task.WhenAny(tasks);
        }

        #endregion
    }
}
