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
		///     Delays the operation by a given amount of time relative to now.
		/// </summary>
		/// <param name="millisecondsFromNow"> The amount of time to delay, in milliseconds. </param>
		/// <returns>
		///     true if the operation could be rescheduled, false if the operation has already finished or could not be rescheduled anymore.
		/// </returns>
		bool Reschedule (int millisecondsFromNow);

		/// <summary>
		///     Delays the operation until a given date and time.
		/// </summary>
		/// <param name="timestamp"> The date and time when the operation should be executed. </param>
		/// <returns>
		///     true if the operation could be rescheduled, false if the operation has already finished or could not be rescheduled anymore.
		/// </returns>
		/// <remarks>
		///     <note type="important">
		///         To avoid troubles with daylight saving time (e.g. issuing a reschedule around the time when the clock jumps or holds for one hour), <paramref name="timestamp" /> is considered to be UTC (compared to <see cref="DateTime" />.<see cref="DateTime.UtcNow" />).
		///     </note>
		/// </remarks>
		bool Reschedule (DateTime timestamp);
	}
}
