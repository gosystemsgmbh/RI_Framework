using System;
using System.Threading;
using System.Threading.Tasks;

namespace RI.Framework.Threading
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="IThreadDispatcher" /> type.
	/// </summary>
	/// <threadsafety static="true" instance="true" />
	public static class IThreadDispatcherExtensions
	{
		#region Static Methods

		/// <summary>
		///     Determines under which priority the current code is executed.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher. </param>
		/// <returns>
		///     The priority of the currently executed code or the default priority of the dispatcher if the current code is not executed by the dispatcher.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" />  is null. </exception>
		public static int GetCurrentPriorityOrDefault (this IThreadDispatcher dispatcher)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			lock (dispatcher.SyncRoot)
			{
				return dispatcher.GetCurrentPriorityOrDefault(dispatcher.DefaultPriority);
			}
		}

		/// <summary>
		///     Determines under which priority the current code is executed.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher. </param>
		/// <param name="defaultPriority"> The default priority to return if the current code is not executed by the dispatcher. </param>
		/// <returns>
		///     The priority of the currently executed code or <paramref name="defaultPriority" /> if the current code is not executed by the dispatcher.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" />  is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="defaultPriority" /> is less than zero. </exception>
		public static int GetCurrentPriorityOrDefault (this IThreadDispatcher dispatcher, int defaultPriority)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			if (defaultPriority < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(defaultPriority));
			}

			lock (dispatcher.SyncRoot)
			{
				return dispatcher.GetCurrentPriority().GetValueOrDefault(defaultPriority);
			}
		}

		/// <summary>
		///     Determines under which options the current code is executed.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher. </param>
		/// <returns>
		///     The options of the currently executed code or the default options of the dispatcher if the current code is not executed by the dispatcher.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" />  is null. </exception>
		public static ThreadDispatcherOptions GetCurrentOptionsOrDefault(this IThreadDispatcher dispatcher)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			lock (dispatcher.SyncRoot)
			{
				return dispatcher.GetCurrentOptionsOrDefault(dispatcher.DefaultOptions);
			}
		}

		/// <summary>
		///     Determines under which options the current code is executed.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher. </param>
		/// <param name="defaultOptions"> The default options to return if the current code is not executed by the dispatcher. </param>
		/// <returns>
		///     The options of the currently executed code or <paramref name="defaultOptions" /> if the current code is not executed by the dispatcher.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" />  is null. </exception>
		public static ThreadDispatcherOptions GetCurrentOptionsOrDefault(this IThreadDispatcher dispatcher, ThreadDispatcherOptions defaultOptions)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			lock (dispatcher.SyncRoot)
			{
				return dispatcher.GetCurrentOptions().GetValueOrDefault(defaultOptions);
			}
		}

		/// <summary>
		///     Enqueues a delegate to the dispatchers queue for delayed execution.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher. </param>
		/// <param name="milliseconds"> The delay of the execution in milliseconds. </param>
		/// <param name="action"> The delegate. </param>
		/// <param name="parameters"> Optional parameters of the delagate. </param>
		/// <returns>
		///     The dispatcher timer created for the delayed execution.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The delegate is enqueued with the default priority (<see cref="IThreadDispatcher.DefaultPriority" />).
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> or <paramref name="action" /> is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="milliseconds" /> is less than zero. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
		public static ThreadDispatcherTimer PostDelayed (this IThreadDispatcher dispatcher, int milliseconds, Delegate action, params object[] parameters)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			lock (dispatcher.SyncRoot)
			{
				return dispatcher.PostDelayed(milliseconds, dispatcher.DefaultPriority, dispatcher.DefaultOptions, action, parameters);
			}
		}

		/// <summary>
		///     Enqueues a delegate to the dispatchers queue for delayed execution.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher. </param>
		/// <param name="milliseconds"> The delay of the execution in milliseconds. </param>
		/// <param name="priority"> The priority. </param>
		/// <param name="action"> The delegate. </param>
		/// <param name="parameters"> Optional parameters of the delagate. </param>
		/// <returns>
		///     The dispatcher timer created for the delayed execution.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> or <paramref name="action" /> is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="milliseconds" /> or <paramref name="priority" /> is less than zero. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
		public static ThreadDispatcherTimer PostDelayed (this IThreadDispatcher dispatcher, int milliseconds, int priority, Delegate action, params object[] parameters)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			lock (dispatcher.SyncRoot)
			{
				return dispatcher.PostDelayed(milliseconds, priority, dispatcher.DefaultOptions, action, parameters);
			}
		}

		/// <summary>
		///     Enqueues a delegate to the dispatchers queue for delayed execution.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher. </param>
		/// <param name="milliseconds"> The delay of the execution in milliseconds. </param>
		/// <param name="priority"> The priority. </param>
		/// <param name="options">The used execution options.</param>
		/// <param name="action"> The delegate. </param>
		/// <param name="parameters"> Optional parameters of the delagate. </param>
		/// <returns>
		///     The dispatcher timer created for the delayed execution.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> or <paramref name="action" /> is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="milliseconds" /> or <paramref name="priority" /> is less than zero. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
		public static ThreadDispatcherTimer PostDelayed (this IThreadDispatcher dispatcher, int milliseconds, int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			if (milliseconds < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(milliseconds));
			}

			if (priority < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(priority));
			}

			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			lock (dispatcher.SyncRoot)
			{
				if (!dispatcher.IsRunning)
				{
					throw new InvalidOperationException(nameof(ThreadDispatcher) + " is not running.");
				}

				if (dispatcher.ShutdownMode != ThreadDispatcherShutdownMode.None)
				{
					throw new InvalidOperationException(nameof(ThreadDispatcher) + " is already shutting down.");
				}

				ThreadDispatcherTimer timer = new ThreadDispatcherTimer(dispatcher, ThreadDispatcherTimerMode.OneShot, priority, options, milliseconds, action, parameters);
				timer.Start();
				return timer;
			}
		}

		/// <summary>
		///     Enqueues a delegate to the dispatchers queue for delayed execution.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher. </param>
		/// <param name="delay"> The delay of the execution. </param>
		/// <param name="action"> The delegate. </param>
		/// <param name="parameters"> Optional parameters of the delagate. </param>
		/// <returns>
		///     The dispatcher timer created for the delayed execution.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The delegate is enqueued with the default priority (<see cref="IThreadDispatcher.DefaultPriority" />).
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> or <paramref name="action" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
		public static ThreadDispatcherTimer PostDelayed (this IThreadDispatcher dispatcher, TimeSpan delay, Delegate action, params object[] parameters)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			lock (dispatcher.SyncRoot)
			{
				return dispatcher.PostDelayed((int)delay.TotalMilliseconds, action, parameters);
			}
		}

		/// <summary>
		///     Enqueues a delegate to the dispatchers queue for delayed execution.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher. </param>
		/// <param name="delay"> The delay of the execution. </param>
		/// <param name="priority"> The priority. </param>
		/// <param name="action"> The delegate. </param>
		/// <param name="parameters"> Optional parameters of the delagate. </param>
		/// <returns>
		///     The dispatcher timer created for the delayed execution.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> or <paramref name="action" /> is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than zero. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
		public static ThreadDispatcherTimer PostDelayed (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, Delegate action, params object[] parameters)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			lock (dispatcher.SyncRoot)
			{
				return dispatcher.PostDelayed((int)delay.TotalMilliseconds, priority, action, parameters);
			}
		}

		/// <summary>
		///     Enqueues a delegate to the dispatchers queue for delayed execution.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher. </param>
		/// <param name="delay"> The delay of the execution. </param>
		/// <param name="priority"> The priority. </param>
		/// <param name="options">The used execution options.</param>
		/// <param name="action"> The delegate. </param>
		/// <param name="parameters"> Optional parameters of the delagate. </param>
		/// <returns>
		///     The dispatcher timer created for the delayed execution.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> or <paramref name="action" /> is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than zero. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
		public static ThreadDispatcherTimer PostDelayed (this IThreadDispatcher dispatcher, TimeSpan delay, int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			lock (dispatcher.SyncRoot)
			{
				return dispatcher.PostDelayed((int)delay.TotalMilliseconds, priority, options, action, parameters);
			}
		}

		/// <summary>
		/// Gets the <see cref="SynchronizationContext"/> associated with the dispatcher.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher. </param>
		/// <returns>
		/// The <see cref="SynchronizationContext"/> associated with the dispatcher or null if the dispatcher is not running.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
		public static SynchronizationContext GetSynchronizationContext (this IThreadDispatcher dispatcher)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			lock (dispatcher.SyncRoot)
			{
				if (!dispatcher.IsRunning)
				{
					return null;
				}

				if (dispatcher is ThreadDispatcher)
				{
					return ((ThreadDispatcher)dispatcher).Context;
				}

				if (dispatcher is HeavyThreadDispatcher)
				{
					return ((HeavyThreadDispatcher)dispatcher).Dispatcher.Context;
				}

				return new ThreadDispatcherSynchronizationContext(dispatcher);
			}
		}

		/// <summary>
		/// Gets the <see cref="TaskScheduler"/> associated with the dispatcher.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher. </param>
		/// <returns>
		/// The <see cref="TaskScheduler"/> associated with the dispatcher or null if the dispatcher is not running.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
		public static TaskScheduler GetTaskScheduler (this IThreadDispatcher dispatcher)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			lock (dispatcher.SyncRoot)
			{
				if (!dispatcher.IsRunning)
				{
					return null;
				}

				if (dispatcher is ThreadDispatcher)
				{
					return ((ThreadDispatcher)dispatcher).Scheduler;
				}

				if (dispatcher is HeavyThreadDispatcher)
				{
					return ((HeavyThreadDispatcher)dispatcher).Dispatcher.Scheduler;
				}

				return new ThreadDispatcherTaskScheduler(dispatcher);
			}
		}

		#endregion
	}
}
