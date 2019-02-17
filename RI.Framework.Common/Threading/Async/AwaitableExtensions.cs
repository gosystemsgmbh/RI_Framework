using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;




namespace RI.Framework.Threading.Async
{
    /// <summary>
    ///     Provides utility/extension methods to make various types awaitable.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    /// TODO: Add overloads for GetAwaiter(TimeSpan), GetAwaiter(CancellationToken), GetAwaiter(Process)
    public static class AwaitableExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Creates an awaiter for a timespan which waits the amount of time of the timespan before completion.
        /// </summary>
        /// <param name="timespan"> The timespan to wait. </param>
        /// <returns>
        ///     The awaiter.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="timespan" /> is negative. </exception>
        public static TaskAwaiter GetAwaiter (this TimeSpan timespan)
        {
            if (timespan.TotalMilliseconds < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(timespan));
            }

            return Task.Delay(timespan).GetAwaiter();
        }

        /// <summary>
        ///     Creates an awaiter which waits for a cancellation token to be cancelled.
        /// </summary>
        /// <param name="ct"> The cancellation token. </param>
        /// <returns>
        ///     The awaiter.
        /// </returns>
        public static TaskAwaiter GetAwaiter (this CancellationToken ct)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            Task task = tcs.Task;

            if (ct.IsCancellationRequested)
            {
                tcs.SetResult(true);
            }
            else
            {
                ct.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            }

            return task.GetAwaiter();
        }

        /// <summary>
        ///     Creates an awaiter which waits for a process to be exited.
        /// </summary>
        /// <param name="process"> The process. </param>
        /// <returns>
        ///     The awaiter.
        ///     The awaiters result represents the exit code of the process.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="process" /> is null. </exception>
        public static TaskAwaiter<int> GetAwaiter (this Process process)
        {
            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();

            process.EnableRaisingEvents = true;

            EventHandler exitHandler = null;
            exitHandler = (sender, args) =>
            {
                process.Exited -= exitHandler;
                tcs.TrySetResult(process.ExitCode);
            };

            process.Exited += exitHandler;
            if (process.HasExited)
            {
                process.Exited -= exitHandler;
                tcs.TrySetResult(process.ExitCode);
            }

            return tcs.Task.GetAwaiter();
        }

        #endregion
    }
}
