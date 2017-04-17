using System;
using System.Threading;




namespace RI.Framework.Services.Dispatcher
{
	/// <summary>
	///     Defines the priority of a dispatcher operation.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="Now" />: The operation is executed in the same thread it is dispatched from.
	///         The created <see cref="IDispatcherOperation" /> will already have its <see cref="IDispatcherOperation.Status" /> set to <see cref="DispatcherStatus.Processed" />.
	///     </para>
	///     <para>
	///         <see cref="Frame" />: The operation is executed in the main/foreground thread.
	///     </para>
	///     <para>
	///         <see cref="Later" />: The operation is executed in the main/foreground thread.
	///     </para>
	///     <para>
	///         <see cref="Idle" />: The operation is executed in the main/foreground thread.
	///     </para>
	///     <para>
	///         <see cref="Background" />: The operation is executed in a background thread assigned by <see cref="ThreadPool" />.
	///         The behaviour of <see cref="ThreadPool" /> depends on how it is configured and the used platform.
	///         A dispatcher operation with <see cref="Background" /> priority cannot be canceled, rescheduled, or its timeout set after the end of the frame the operation was dispatched in.
	///         The <see cref="IDispatcherOperation.Result" /> property is set from the background thread.
	///         The <see cref="IDispatcherOperation.Status" /> property is set from the main/foreground thread.
	///         Any finish callback (<see cref="IDispatcherOperation.OnFinished" />) is called from the main/foreground thread.
	///         Therefore, only use <see cref="IDispatcherOperation.Status" /> or the finish callback to process or detect the end of an operations processing!
	///     </para>
	///     <note type="important">
	///         Use <see cref="Later" /> and <see cref="Idle" /> with caution! An operation with one of those priorities is non-deterministic and might never be executed in cases where operations of higher priorities are constantly dispatched (e.g. the dispatcher never becomes idle)!
	///     </note>
	///     <para>
	///         <see cref="Now" />: xxx
	///     </para>
	///     <para>
	///         <see cref="Now" />: xxx
	///     </para>
	///     <para>
	///         <see cref="Now" />: xxx
	///     </para>
	///     <para>
	///         <see cref="Now" />: xxx
	///     </para>
	/// </remarks>
	[Serializable]
	public enum DispatcherPriority
	{
		/// <summary>
		///     The operation is executed immediately when dispatched.
		/// </summary>
		Now = 0,

		/// <summary>
		///     The operation is executed during the current frame.
		/// </summary>
		Frame = 1,

		/// <summary>
		///     The operation is executed during the frame which had no other dispatcher operations of higher priority (<see cref="Now" /> or <see cref="Frame" />).
		/// </summary>
		Later = 2,

		/// <summary>
		///     The operation is executed during the frame which had no other dispatcher operations of higher priority (<see cref="Now" />, <see cref="Frame" />, or <see cref="Later" />) and whose previous frame had no dispatcher operations at all.
		/// </summary>
		Idle = 3,

		/// <summary>
		///     The operation is executed in a background thread, using <see cref="ThreadPool" />.
		/// </summary>
		Background = 4,

		/// <summary>
		///     The operation is executed with the lowest possible priority (which is <see cref="Idle" />).
		/// </summary>
		Lowest = DispatcherPriority.Idle,

		/// <summary>
		///     The operation is executed with the highest possible priority (which is <see cref="Now" />).
		/// </summary>
		Highest = DispatcherPriority.Now,

		/// <summary>
		///     The operation is executed with the default priority (which is <see cref="Frame" />).
		/// </summary>
		Default = DispatcherPriority.Frame,
	}
}
