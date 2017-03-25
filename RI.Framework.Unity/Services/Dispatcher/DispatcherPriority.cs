using System;
using System.Threading;




namespace RI.Framework.Services.Dispatcher
{
	/// <summary>
	///     Defines the priority of a dispatcher operation.
	/// </summary>
	[Serializable]
	public enum DispatcherPriority
	{
		/// <summary>
		///     The operation is executed immediately when dispatched.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The created <see cref="IDispatcherOperation" /> will already have its <see cref="IDispatcherOperation.Status" /> set to <see cref="DispatcherStatus.Processed" />.
		///     </para>
		/// </remarks>
		Now = 0,

		/// <summary>
		///     The operation is executed during the current frame.
		/// </summary>
		Frame = 1,

		/// <summary>
		///     The operation is executed during the frame which had no other dispatcher operations of higher priority (<see cref="Now" /> or <see cref="Frame" />).
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         Use with caution! An operation with this priority is non-deterministic and might never be executed in cases where operations of higher priorities are constantly dispatched!
		///     </note>
		/// </remarks>
		Later = 2,

		/// <summary>
		///     The operation is executed during the frame which had no other dispatcher operations of higher priority (<see cref="Now" />, <see cref="Frame" />, or <see cref="Later" />) and whose previous frame had no dispatcher operations at all.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         Use with caution! An operation with this priority is non-deterministic and might never be executed in cases where operations of higher priorities are constantly dispatched!
		///     </note>
		/// </remarks>
		Idle = 3,

		/// <summary>
		/// The operation is executed in a background thread, using <see cref="ThreadPool"/>.
		/// </summary>
		/// <remarks>
		///     <note type="note">
		/// The dispatcher service uses <see cref="ThreadPool"/> as is, without any changes to it.
		/// You can control the <see cref="ThreadPool"/> as you see necessary.
		///     </note>
		///     <note type="note">
		/// A dispatcher operation with <see cref="Background"/> priority cannot be canceled, rescheduled, or its timeout set after the end of the frame the operation was dispatched in.
		///     </note>
		///     <note type="important">
		/// The <see cref="IDispatcherOperation.Result"/> property is set from the background thread assigned by <see cref="ThreadPool"/>.
		/// The <see cref="IDispatcherOperation.Status"/> property is set from the main/foreground thread.
		/// Therefore, only use <see cref="IDispatcherOperation.Status"/> to check whether a background operation has finished!
		///     </note>
		/// </remarks>
		Background = 4,

		/// <summary>
		///     The operation is executed with the lowest possible priority (which is <see cref="Idle"/>).
		/// </summary>
		Lowest = DispatcherPriority.Idle,

		/// <summary>
		///     The operation is executed with the highest possible priority (which is <see cref="Now"/>).
		/// </summary>
		Highest = DispatcherPriority.Now,
	}
}
