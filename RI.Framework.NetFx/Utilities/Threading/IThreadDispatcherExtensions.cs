﻿using System;




namespace RI.Framework.Utilities.Threading
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

			return dispatcher.GetCurrentPriorityOrDefault(dispatcher.DefaultPriority);
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

			return dispatcher.GetCurrentPriority().GetValueOrDefault(defaultPriority);
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

			return dispatcher.PostDelayed(milliseconds, dispatcher.DefaultPriority, action, parameters);
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

			ThreadDispatcherTimer timer = new ThreadDispatcherTimer(dispatcher, ThreadDispatcherTimerMode.OneShot, priority, milliseconds, action, parameters);
			timer.Start();
			return timer;
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
			return dispatcher.PostDelayed((int)delay.TotalMilliseconds, action, parameters);
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
			return dispatcher.PostDelayed((int)delay.TotalMilliseconds, priority, action, parameters);
		}

		#endregion
	}
}
