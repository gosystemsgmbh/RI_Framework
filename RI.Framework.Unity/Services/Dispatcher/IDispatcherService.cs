using System;

using RI.Framework.Composition.Model;




namespace RI.Framework.Services.Dispatcher
{
	/// <summary>
	///     Defines the interface for a dispatcher service.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A dispatcher service is used to exchange events and data between modules, for synchronizing processing, and to handle background tasks.
	///     </para>
	///     <note type="implement">
	///         All broadcasts and dispatches must be executed in the order they were issued, depending on their priority.
	///     </note>
	/// </remarks>
	[Export]
	public interface IDispatcherService
	{
		/// <summary>
		/// Cancels all pending operations.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is similar to calling <see cref="IDispatcherOperation.Cancel"/> on all still pending <see cref="IDispatcherOperation"/>s.
		/// </para>
		/// </remarks>
		void CancelAllOperations ();

		/// <summary>
		///     Broadcasts an object of a specified type to all receivers registered for that type.
		/// </summary>
		/// <typeparam name="T"> The type to broadcast. </typeparam>
		/// <param name="priority"> The broadcast priority or when the broadcast shall be delivered to all the receivers respectively. </param>
		/// <param name="broadcast"> The actual object to be broadcasted (e.g. an event object). </param>
		/// <returns>
		///     The dispatcher operation which can be used to control the delivery of the broadcast.
		///     See <see cref="IDispatcherOperation" /> for more details.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="broadcast" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		IDispatcherOperation Broadcast <T> (DispatcherPriority priority, T broadcast) where T : class;

		/// <summary>
		///     Dispatches the execution of a method (with no parameter, no return value).
		/// </summary>
		/// <param name="priority"> The priority of the execution. </param>
		/// <param name="action"> The method to be executed. </param>
		/// <returns>
		///     The dispatcher operation which can be used to control the execution of the method.
		///     See <see cref="IDispatcherOperation" /> for more details.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		IDispatcherOperation Dispatch (DispatcherPriority priority, Action action);

		/// <summary>
		///     Dispatches the execution of a method (with 1 parameter, no return value).
		/// </summary>
		/// <typeparam name="T"> The type of the parameter. </typeparam>
		/// <param name="priority"> The priority of the execution. </param>
		/// <param name="action"> The method to be executed. </param>
		/// <param name="arg"> The value of the parameter. </param>
		/// <returns>
		///     The dispatcher operation which can be used to control the execution of the method.
		///     See <see cref="IDispatcherOperation" /> for more details.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		IDispatcherOperation Dispatch <T> (DispatcherPriority priority, Action<T> action, T arg);

		/// <summary>
		///     Dispatches the execution of a method (with 2 parameters, no return value).
		/// </summary>
		/// <typeparam name="T1"> The type of the first parameter. </typeparam>
		/// <typeparam name="T2"> The type of the second parameter. </typeparam>
		/// <param name="priority"> The priority of the execution. </param>
		/// <param name="action"> The method to be executed. </param>
		/// <param name="arg1"> The value of the first parameter. </param>
		/// <param name="arg2"> The value of the second parameter. </param>
		/// <returns>
		///     The dispatcher operation which can be used to control the execution of the method.
		///     See <see cref="IDispatcherOperation" /> for more details.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		IDispatcherOperation Dispatch <T1, T2> (DispatcherPriority priority, Action<T1, T2> action, T1 arg1, T2 arg2);

		/// <summary>
		///     Dispatches the execution of a method (with 3 parameters, no return value).
		/// </summary>
		/// <typeparam name="T1"> The type of the first parameter. </typeparam>
		/// <typeparam name="T2"> The type of the second parameter. </typeparam>
		/// <typeparam name="T3"> The type of the third parameter. </typeparam>
		/// <param name="priority"> The priority of the execution. </param>
		/// <param name="action"> The method to be executed. </param>
		/// <param name="arg1"> The value of the first parameter. </param>
		/// <param name="arg2"> The value of the second parameter. </param>
		/// <param name="arg3"> The value of the third parameter. </param>
		/// <returns>
		///     The dispatcher operation which can be used to control the execution of the method.
		///     See <see cref="IDispatcherOperation" /> for more details.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		IDispatcherOperation Dispatch <T1, T2, T3> (DispatcherPriority priority, Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3);

		/// <summary>
		///     Dispatches the execution of a method (with 4 parameters, no return value).
		/// </summary>
		/// <typeparam name="T1"> The type of the first parameter. </typeparam>
		/// <typeparam name="T2"> The type of the second parameter. </typeparam>
		/// <typeparam name="T3"> The type of the third parameter. </typeparam>
		/// <typeparam name="T4"> The type of the fourth parameter. </typeparam>
		/// <param name="priority"> The priority of the execution. </param>
		/// <param name="action"> The method to be executed. </param>
		/// <param name="arg1"> The value of the first parameter. </param>
		/// <param name="arg2"> The value of the second parameter. </param>
		/// <param name="arg3"> The value of the third parameter. </param>
		/// <param name="arg4"> The value of the fourth parameter. </param>
		/// <returns>
		///     The dispatcher operation which can be used to control the execution of the method.
		///     See <see cref="IDispatcherOperation" /> for more details.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		IDispatcherOperation Dispatch <T1, T2, T3, T4> (DispatcherPriority priority, Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

		/// <summary>
		///     Dispatches the execution of a method (with no parameter, using a return value).
		/// </summary>
		/// <typeparam name="TResult"> The type of the return value. </typeparam>
		/// <param name="priority"> The priority of the execution. </param>
		/// <param name="func"> The method to be executed. </param>
		/// <returns>
		///     The dispatcher operation which can be used to control the execution of the method.
		///     See <see cref="IDispatcherOperation" /> for more details.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="func" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		IDispatcherOperation DispatchFunc <TResult> (DispatcherPriority priority, Func<TResult> func);

		/// <summary>
		///     Dispatches the execution of a method (with 1 parameter, using a return value).
		/// </summary>
		/// <typeparam name="T"> The type of the parameter. </typeparam>
		/// <typeparam name="TResult"> The type of the return value. </typeparam>
		/// <param name="priority"> The priority of the execution. </param>
		/// <param name="func"> The method to be executed. </param>
		/// <param name="arg"> The value of the parameter. </param>
		/// <returns>
		///     The dispatcher operation which can be used to control the execution of the method.
		///     See <see cref="IDispatcherOperation" /> for more details.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="func" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		IDispatcherOperation DispatchFunc <T, TResult> (DispatcherPriority priority, Func<T, TResult> func, T arg);

		/// <summary>
		///     Dispatches the execution of a method (with 2 parameters, using a return value).
		/// </summary>
		/// <typeparam name="T1"> The type of the first parameter. </typeparam>
		/// <typeparam name="T2"> The type of the second parameter. </typeparam>
		/// <typeparam name="TResult"> The type of the return value. </typeparam>
		/// <param name="priority"> The priority of the execution. </param>
		/// <param name="func"> The method to be executed. </param>
		/// <param name="arg1"> The value of the first parameter. </param>
		/// <param name="arg2"> The value of the second parameter. </param>
		/// <returns>
		///     The dispatcher operation which can be used to control the execution of the method.
		///     See <see cref="IDispatcherOperation" /> for more details.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="func" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		IDispatcherOperation DispatchFunc <T1, T2, TResult> (DispatcherPriority priority, Func<T1, T2, TResult> func, T1 arg1, T2 arg2);

		/// <summary>
		///     Dispatches the execution of a method (with 3 parameters, using a return value).
		/// </summary>
		/// <typeparam name="T1"> The type of the first parameter. </typeparam>
		/// <typeparam name="T2"> The type of the second parameter. </typeparam>
		/// <typeparam name="T3"> The type of the third parameter. </typeparam>
		/// <typeparam name="TResult"> The type of the return value. </typeparam>
		/// <param name="priority"> The priority of the execution. </param>
		/// <param name="func"> The method to be executed. </param>
		/// <param name="arg1"> The value of the first parameter. </param>
		/// <param name="arg2"> The value of the second parameter. </param>
		/// <param name="arg3"> The value of the third parameter. </param>
		/// <returns>
		///     The dispatcher operation which can be used to control the execution of the method.
		///     See <see cref="IDispatcherOperation" /> for more details.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="func" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		IDispatcherOperation DispatchFunc <T1, T2, T3, TResult> (DispatcherPriority priority, Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3);

		/// <summary>
		///     Dispatches the execution of a method (with 4 parameters, using a return value).
		/// </summary>
		/// <typeparam name="T1"> The type of the first parameter. </typeparam>
		/// <typeparam name="T2"> The type of the second parameter. </typeparam>
		/// <typeparam name="T3"> The type of the third parameter. </typeparam>
		/// <typeparam name="T4"> The type of the fourth parameter. </typeparam>
		/// <typeparam name="TResult"> The type of the return value. </typeparam>
		/// <param name="priority"> The priority of the execution. </param>
		/// <param name="func"> The method to be executed. </param>
		/// <param name="arg1"> The value of the first parameter. </param>
		/// <param name="arg2"> The value of the second parameter. </param>
		/// <param name="arg3"> The value of the third parameter. </param>
		/// <param name="arg4"> The value of the fourth parameter. </param>
		/// <returns>
		///     The dispatcher operation which can be used to control the execution of the method.
		///     See <see cref="IDispatcherOperation" /> for more details.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="func" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		IDispatcherOperation DispatchFunc <T1, T2, T3, T4, TResult> (DispatcherPriority priority, Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

		/// <summary>
		///     Registers a receiver for a specified broadcast type.
		/// </summary>
		/// <typeparam name="T"> The broadcast type of the receiver. </typeparam>
		/// <param name="receiver"> The method which handles a broadcast of the specified type <typeparamref name="T" />. </param>
		/// <remarks>
		///     <para>
		///         See <see cref="Broadcast{T}(DispatcherPriority, T)" /> for more details about broadcasting.
		///     </para>
		///     <note type="implement">
		///         It is recommended that registering the same receiver multiple times is ignored.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="receiver" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		void RegisterReceiver <T> (Action<T> receiver) where T : class;

		/// <summary>
		///     Unregisters a receiver for a specified broadcast type.
		/// </summary>
		/// <typeparam name="T"> The broadcast type of the receiver. </typeparam>
		/// <param name="receiver"> The method which handles a broadcast of the specified type <typeparamref name="T" />. </param>
		/// <remarks>
		///     <para>
		///         See <see cref="Broadcast{T}(DispatcherPriority, T)" /> for more details about broadcasting.
		///     </para>
		///     <note type="implement">
		///         It is recommended that unregistering the same receiver multiple times is ignored.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="receiver" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The service is not fully initialized. </exception>
		void UnregisterReceiver <T> (Action<T> receiver) where T : class;
	}
}
