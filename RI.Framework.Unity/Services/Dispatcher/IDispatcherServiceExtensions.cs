using System;




namespace RI.Framework.Services.Dispatcher
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="IDispatcherService" /> type and its implementations.
	/// </summary>
	public static class IDispatcherServiceExtensions
	{
		/// <summary>
		///     Broadcasts an object of a specified type to all receivers registered for that type, using the default priority.
		/// </summary>
		/// <typeparam name="T"> The type to broadcast. </typeparam>
		/// <param name="dispatcher">The dispatcher service to use.</param>
		/// <param name="broadcast"> The actual object to be broadcasted (e.g. an event object). </param>
		/// <returns>
		///     The dispatcher operation which can be used to control the delivery of the broadcast.
		///     See <see cref="IDispatcherOperation" /> for more details.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="broadcast" /> or <paramref name="dispatcher"/> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		public static IDispatcherOperation Broadcast <T> (this IDispatcherService dispatcher, T broadcast)
			where T : class
		{
			if (dispatcher == null)
			{
				throw new ArgumentException(nameof(dispatcher));
			}

			return dispatcher.Broadcast(DispatcherPriority.Default, broadcast);
		}

		/// <summary>
		///     Dispatches the execution of a method (with no parameter, no return value), using the default priority.
		/// </summary>
		/// <param name="dispatcher">The dispatcher service to use.</param>
		/// <param name="action"> The method to be executed. </param>
		/// <returns>
		///     The dispatcher operation which can be used to control the execution of the method.
		///     See <see cref="IDispatcherOperation" /> for more details.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> or <paramref name="dispatcher"/> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		public static IDispatcherOperation Dispatch(this IDispatcherService dispatcher, Action action)
		{
			if (dispatcher == null)
			{
				throw new ArgumentException(nameof(dispatcher));
			}

			return dispatcher.Dispatch(DispatcherPriority.Default, action);
		}

		/// <summary>
		///     Dispatches the execution of a method (with 1 parameter, no return value), using the default priority.
		/// </summary>
		/// <typeparam name="T"> The type of the parameter. </typeparam>
		/// <param name="dispatcher">The dispatcher service to use.</param>
		/// <param name="action"> The method to be executed. </param>
		/// <param name="arg"> The value of the parameter. </param>
		/// <returns>
		///     The dispatcher operation which can be used to control the execution of the method.
		///     See <see cref="IDispatcherOperation" /> for more details.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> or <paramref name="dispatcher"/> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		public static IDispatcherOperation Dispatch<T>(this IDispatcherService dispatcher, Action<T> action, T arg)
		{
			if (dispatcher == null)
			{
				throw new ArgumentException(nameof(dispatcher));
			}

			return dispatcher.Dispatch(DispatcherPriority.Default, action, arg);
		}

		/// <summary>
		///     Dispatches the execution of a method (with 2 parameters, no return value), using the default priority.
		/// </summary>
		/// <typeparam name="T1"> The type of the first parameter. </typeparam>
		/// <typeparam name="T2"> The type of the second parameter. </typeparam>
		/// <param name="dispatcher">The dispatcher service to use.</param>
		/// <param name="action"> The method to be executed. </param>
		/// <param name="arg1"> The value of the first parameter. </param>
		/// <param name="arg2"> The value of the second parameter. </param>
		/// <returns>
		///     The dispatcher operation which can be used to control the execution of the method.
		///     See <see cref="IDispatcherOperation" /> for more details.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> or <paramref name="dispatcher"/> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		public static IDispatcherOperation Dispatch<T1, T2>(this IDispatcherService dispatcher, Action<T1, T2> action, T1 arg1, T2 arg2)
		{
			if (dispatcher == null)
			{
				throw new ArgumentException(nameof(dispatcher));
			}

			return dispatcher.Dispatch(DispatcherPriority.Default, action, arg1, arg2);
		}

		/// <summary>
		///     Dispatches the execution of a method (with 3 parameters, no return value), using the default priority.
		/// </summary>
		/// <typeparam name="T1"> The type of the first parameter. </typeparam>
		/// <typeparam name="T2"> The type of the second parameter. </typeparam>
		/// <typeparam name="T3"> The type of the third parameter. </typeparam>
		/// <param name="dispatcher">The dispatcher service to use.</param>
		/// <param name="action"> The method to be executed. </param>
		/// <param name="arg1"> The value of the first parameter. </param>
		/// <param name="arg2"> The value of the second parameter. </param>
		/// <param name="arg3"> The value of the third parameter. </param>
		/// <returns>
		///     The dispatcher operation which can be used to control the execution of the method.
		///     See <see cref="IDispatcherOperation" /> for more details.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> or <paramref name="dispatcher"/> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		public static IDispatcherOperation Dispatch<T1, T2, T3>(this IDispatcherService dispatcher, Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
		{
			if (dispatcher == null)
			{
				throw new ArgumentException(nameof(dispatcher));
			}

			return dispatcher.Dispatch(DispatcherPriority.Default, action, arg1, arg2, arg3);
		}

		/// <summary>
		///     Dispatches the execution of a method (with 4 parameters, no return value), using the default priority.
		/// </summary>
		/// <typeparam name="T1"> The type of the first parameter. </typeparam>
		/// <typeparam name="T2"> The type of the second parameter. </typeparam>
		/// <typeparam name="T3"> The type of the third parameter. </typeparam>
		/// <typeparam name="T4"> The type of the fourth parameter. </typeparam>
		/// <param name="dispatcher">The dispatcher service to use.</param>
		/// <param name="action"> The method to be executed. </param>
		/// <param name="arg1"> The value of the first parameter. </param>
		/// <param name="arg2"> The value of the second parameter. </param>
		/// <param name="arg3"> The value of the third parameter. </param>
		/// <param name="arg4"> The value of the fourth parameter. </param>
		/// <returns>
		///     The dispatcher operation which can be used to control the execution of the method.
		///     See <see cref="IDispatcherOperation" /> for more details.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> or <paramref name="dispatcher"/> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		public static IDispatcherOperation Dispatch<T1, T2, T3, T4>(this IDispatcherService dispatcher, Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			if (dispatcher == null)
			{
				throw new ArgumentException(nameof(dispatcher));
			}

			return dispatcher.Dispatch(DispatcherPriority.Default, action, arg1, arg2, arg3, arg4);
		}

		/// <summary>
		///     Dispatches the execution of a method (with no parameter, using a return value), using the default priority.
		/// </summary>
		/// <typeparam name="TResult"> The type of the return value. </typeparam>
		/// <param name="dispatcher">The dispatcher service to use.</param>
		/// <param name="func"> The method to be executed. </param>
		/// <returns>
		///     The dispatcher operation which can be used to control the execution of the method.
		///     See <see cref="IDispatcherOperation" /> for more details.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="func" /> or <paramref name="dispatcher"/> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		public static IDispatcherOperation Dispatch<TResult>(this IDispatcherService dispatcher, Func<TResult> func)
		{
			if (dispatcher == null)
			{
				throw new ArgumentException(nameof(dispatcher));
			}

			return dispatcher.Dispatch(DispatcherPriority.Default, func);
		}

		/// <summary>
		///     Dispatches the execution of a method (with 1 parameter, using a return value), using the default priority.
		/// </summary>
		/// <typeparam name="T"> The type of the parameter. </typeparam>
		/// <typeparam name="TResult"> The type of the return value. </typeparam>
		/// <param name="dispatcher">The dispatcher service to use.</param>
		/// <param name="func"> The method to be executed. </param>
		/// <param name="arg"> The value of the parameter. </param>
		/// <returns>
		///     The dispatcher operation which can be used to control the execution of the method.
		///     See <see cref="IDispatcherOperation" /> for more details.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="func" /> or <paramref name="dispatcher"/> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		public static IDispatcherOperation Dispatch<T, TResult>(this IDispatcherService dispatcher, Func<T, TResult> func, T arg)
		{
			if (dispatcher == null)
			{
				throw new ArgumentException(nameof(dispatcher));
			}

			return dispatcher.Dispatch(DispatcherPriority.Default, func, arg);
		}

		/// <summary>
		///     Dispatches the execution of a method (with 2 parameters, using a return value), using the default priority.
		/// </summary>
		/// <typeparam name="T1"> The type of the first parameter. </typeparam>
		/// <typeparam name="T2"> The type of the second parameter. </typeparam>
		/// <typeparam name="TResult"> The type of the return value. </typeparam>
		/// <param name="dispatcher">The dispatcher service to use.</param>
		/// <param name="func"> The method to be executed. </param>
		/// <param name="arg1"> The value of the first parameter. </param>
		/// <param name="arg2"> The value of the second parameter. </param>
		/// <returns>
		///     The dispatcher operation which can be used to control the execution of the method.
		///     See <see cref="IDispatcherOperation" /> for more details.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="func" /> or <paramref name="dispatcher"/> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		public static IDispatcherOperation Dispatch<T1, T2, TResult>(this IDispatcherService dispatcher, Func<T1, T2, TResult> func, T1 arg1, T2 arg2)
		{
			if (dispatcher == null)
			{
				throw new ArgumentException(nameof(dispatcher));
			}

			return dispatcher.Dispatch(DispatcherPriority.Default, func, arg1, arg2);
		}

		/// <summary>
		///     Dispatches the execution of a method (with 3 parameters, using a return value), using the default priority.
		/// </summary>
		/// <typeparam name="T1"> The type of the first parameter. </typeparam>
		/// <typeparam name="T2"> The type of the second parameter. </typeparam>
		/// <typeparam name="T3"> The type of the third parameter. </typeparam>
		/// <typeparam name="TResult"> The type of the return value. </typeparam>
		/// <param name="dispatcher">The dispatcher service to use.</param>
		/// <param name="func"> The method to be executed. </param>
		/// <param name="arg1"> The value of the first parameter. </param>
		/// <param name="arg2"> The value of the second parameter. </param>
		/// <param name="arg3"> The value of the third parameter. </param>
		/// <returns>
		///     The dispatcher operation which can be used to control the execution of the method.
		///     See <see cref="IDispatcherOperation" /> for more details.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="func" /> or <paramref name="dispatcher"/> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		public static IDispatcherOperation Dispatch<T1, T2, T3, TResult>(this IDispatcherService dispatcher, Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3)
		{
			if (dispatcher == null)
			{
				throw new ArgumentException(nameof(dispatcher));
			}

			return dispatcher.Dispatch(DispatcherPriority.Default, func, arg1, arg2, arg3);
		}

		/// <summary>
		///     Dispatches the execution of a method (with 4 parameters, using a return value), using the default priority.
		/// </summary>
		/// <typeparam name="T1"> The type of the first parameter. </typeparam>
		/// <typeparam name="T2"> The type of the second parameter. </typeparam>
		/// <typeparam name="T3"> The type of the third parameter. </typeparam>
		/// <typeparam name="T4"> The type of the fourth parameter. </typeparam>
		/// <typeparam name="TResult"> The type of the return value. </typeparam>
		/// <param name="dispatcher">The dispatcher service to use.</param>
		/// <param name="func"> The method to be executed. </param>
		/// <param name="arg1"> The value of the first parameter. </param>
		/// <param name="arg2"> The value of the second parameter. </param>
		/// <param name="arg3"> The value of the third parameter. </param>
		/// <param name="arg4"> The value of the fourth parameter. </param>
		/// <returns>
		///     The dispatcher operation which can be used to control the execution of the method.
		///     See <see cref="IDispatcherOperation" /> for more details.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="func" /> or <paramref name="dispatcher"/> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		public static IDispatcherOperation Dispatch<T1, T2, T3, T4, TResult>(this IDispatcherService dispatcher, Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			if (dispatcher == null)
			{
				throw new ArgumentException(nameof(dispatcher));
			}

			return dispatcher.Dispatch(DispatcherPriority.Default, func, arg1, arg2, arg3, arg4);
		}
	}
}