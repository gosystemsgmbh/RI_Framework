using System;




namespace RI.Framework.Services.Dispatcher
{
	/// <summary>
	///     Defines the interface for a dispatcher operation.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A dispatcher operation is issued by a <see cref="IDispatcherService" /> when broadcasting or dispatching.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public interface IDispatcherOperation
	{
		/// <summary>
		///     Gets the result of the operation.
		/// </summary>
		/// <value>
		///     The result of the operation or null if no result is available (keep in mind that the result itself might be null as well...).
		/// </value>
		/// <remarks>
		///     <para>
		///         This property is only set if a method with a return value has been dispatched.
		///         That return value will then be assigned to this property after the method completed execution.
		///     </para>
		/// </remarks>
		object Result { get; }

		/// <summary>
		///     Gets the current status of the operation.
		/// </summary>
		/// <value>
		///     The status of the operation.
		/// </value>
		DispatcherStatus Status { get; }

		/// <summary>
		///     Cancels the operation.
		/// </summary>
		/// <returns>
		///     true if the operation could be canceled, false if the operation has already finished or could not be canceled anymore.
		/// </returns>
		bool Cancel ();

		/// <summary>
		///     Sets a callback which is called when the operation is finished.
		/// </summary>
		/// <param name="callback"> The callback which is called when the operation is finished. Can be set to null to remove a previously set callback. </param>
		/// <returns>
		///     The dispatcher operation itself (for use in fluent API) if the operations timeout could be set, null if the operation has already finished or could not change its timeout anymore.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The operation is finished when its state is <see cref="DispatcherStatus.Processed" />, <see cref="DispatcherStatus.Canceled" />, or <see cref="DispatcherStatus.Timeout" />.
		///         The <see cref="Status" /> property of the provided <see cref="IDispatcherOperation" /> argument of the callback can be used to determine the status.
		///     </para>
		///     <para>
		///         The callback has two arguments: The finished operation and the arguments of the operation.
		///         For broadcasts, the arguments is a single object which is the broadcasted object.
		///         For dispatches, the arguments is the array of arguments of the dispatched action/function.
		///     </para>
		/// </remarks>
		IDispatcherOperation OnFinished (Action<IDispatcherOperation, object[]> callback);

		/// <summary>
		///     Delays the operation by a given amount of time relative to now.
		/// </summary>
		/// <param name="millisecondsFromNow"> The amount of time to delay, in milliseconds. </param>
		/// <returns>
		///     The dispatcher operation itself (for use in fluent API) if the operation could be rescheduled, null if the operation has already finished or could not be rescheduled anymore.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="millisecondsFromNow" /> is less than zero. </exception>
		IDispatcherOperation Reschedule (int millisecondsFromNow);

		/// <summary>
		///     Delays the operation by a given amount of time relative to now.
		/// </summary>
		/// <param name="timeFromNow"> The amount of time to delay. </param>
		/// <returns>
		///     The dispatcher operation itself (for use in fluent API) if the operation could be rescheduled, null if the operation has already finished or could not be rescheduled anymore.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="timeFromNow" /> is negative. </exception>
		IDispatcherOperation Reschedule (TimeSpan timeFromNow);

		/// <summary>
		///     Delays the operation until a given date and time.
		/// </summary>
		/// <param name="timestamp"> The date and time when the operation should be executed. </param>
		/// <returns>
		///     The dispatcher operation itself (for use in fluent API) if the operation could be rescheduled, null if the operation has already finished or could not be rescheduled anymore.
		/// </returns>
		/// <remarks>
		///     <note type="important">
		///         To avoid troubles with daylight saving time (e.g. issuing a reschedule around the time when the clock jumps or holds for one hour), <paramref name="timestamp" /> is considered to be UTC (compared to <see cref="DateTime" />.<see cref="DateTime.UtcNow" />).
		///     </note>
		/// </remarks>
		IDispatcherOperation Reschedule (DateTime timestamp);

		/// <summary>
		///     Sets a timeout for the operation relative to now.
		/// </summary>
		/// <param name="millisecondsFromNow"> The timeout in milliseconds (zero means no timeout). </param>
		/// <returns>
		///     The dispatcher operation itself (for use in fluent API) if the operations timeout could be set, null if the operation has already finished or could not change its timeout anymore.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="millisecondsFromNow" /> is less than zero. </exception>
		IDispatcherOperation Timeout (int millisecondsFromNow);

		/// <summary>
		///     Sets a timeout for the operation relative to now.
		/// </summary>
		/// <param name="timeFromNow"> The timeout (zero means no timeout). </param>
		/// <returns>
		///     The dispatcher operation itself (for use in fluent API) if the operations timeout could be set, null if the operation has already finished or could not change its timeout anymore.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="timeFromNow" /> is negative. </exception>
		IDispatcherOperation Timeout (TimeSpan timeFromNow);

		/// <summary>
		///     Sets the timeout to a given date and time.
		/// </summary>
		/// <param name="timestamp"> The date and time when the operation should time-out. </param>
		/// <returns>
		///     The dispatcher operation itself (for use in fluent API) if the operations timeout could be set, null if the operation has already finished or could not change its timeout anymore.
		/// </returns>
		/// <remarks>
		///     <note type="important">
		///         To avoid troubles with daylight saving time (e.g. issuing a reschedule around the time when the clock jumps or holds for one hour), <paramref name="timestamp" /> is considered to be UTC (compared to <see cref="DateTime" />.<see cref="DateTime.UtcNow" />).
		///     </note>
		/// </remarks>
		IDispatcherOperation Timeout (DateTime timestamp);
	}
}
