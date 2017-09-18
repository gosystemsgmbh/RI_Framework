using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Threading.Async;




namespace RI.Framework.Threading.Tasks
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="Task" /> type.
	/// </summary>
	/// <threadsafety static="true" instance="true" />
	public static class TaskExtensions
	{
		#region Static Methods

		/// <summary>
		///     Creates an awaitable which flows the current <see cref="CultureInfo" /> around the execution of a <see cref="Task" />.
		/// </summary>
		/// <param name="task"> The task. </param>
		/// <returns>
		///     The awaiter.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="task" /> is null. </exception>
		public static CultureFlower WithCurrentCulture (this Task task)
		{
			if (task == null)
			{
				throw new ArgumentNullException(nameof(task));
			}

			return new CultureFlower(task);
		}

		/// <summary>
		///     Creates an awaitable which flows the current <see cref="SynchronizationContext" /> around the execution of a <see cref="Task" />.
		/// </summary>
		/// <param name="task"> The task. </param>
		/// <returns>
		///     The awaiter.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="task" /> is null. </exception>
		public static SynchronizationContextFlower WithCurrentSynchronizationContext (this Task task)
		{
			if (task == null)
			{
				throw new ArgumentNullException(nameof(task));
			}

			return new SynchronizationContextFlower(task);
		}

		/// <summary>
		/// Creates an awaiter for a timespan which waits the amount of time of the timespan before completion.
		/// </summary>
		/// <param name="timespan">The timespan to wait.</param>
		/// <returns>
		/// The awaiter.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="timespan"/> is negative.</exception>
		public static TaskAwaiter GetAwaiter (this TimeSpan timespan)
		{
			if (timespan.TotalMilliseconds < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(timespan));
			}

			return ((int)timespan.TotalMilliseconds).GetAwaiter();
		}

		/// <summary>
		/// Creates an awaiter for an integer which waits the amount of time in milliseconds, before completion.
		/// </summary>
		/// <param name="milliseconds">The number of milliseconds to wait.</param>
		/// <returns>
		/// The awaiter.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="milliseconds"/> is negative.</exception>
		public static TaskAwaiter GetAwaiter (this int milliseconds)
		{
			if (milliseconds < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(milliseconds));
			}

			if (milliseconds == 0)
			{
				return Task.CompletedTask.GetAwaiter();
			}

			return Task.Delay(milliseconds).GetAwaiter();
		}

		/// <summary>
		/// Creates an awaiter which waits for a cancellation token to be cancelled.
		/// </summary>
		/// <param name="ct">The cancellation token.</param>
		/// <returns>
		/// The awaiter.
		/// </returns>
		public static TaskAwaiter GetAwaiter(this CancellationToken ct)
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
		/// Creates an awaiter which waits for a process to be exited.
		/// </summary>
		/// <param name="process">The process.</param>
		/// <returns>
		/// The awaiter.
		/// The awaiters result represents the exit code of the process.
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

		/// <summary>
		/// Creates a task which waits for all tasks to be completed.
		/// </summary>
		/// <param name="tasks">The tasks to wait for.</param>
		/// <returns>
		/// The created task.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="tasks"/> is null.</exception>
		public static Task All (this IEnumerable<Task> tasks)
		{
			if (tasks == null)
			{
				throw new ArgumentNullException(nameof(tasks));
			}

			return Task.WhenAll(tasks);
		}

		/// <summary>
		/// Creates a task which waits for any task to be completed.
		/// </summary>
		/// <param name="tasks">The tasks to wait for.</param>
		/// <returns>
		/// The created task.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="tasks"/> is null.</exception>
		public static Task Any (this IEnumerable<Task> tasks)
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
