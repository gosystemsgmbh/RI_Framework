using System;




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
		///     The operation is executed at the end of the current frame.
		/// </summary>
		Frame = 1,

		/// <summary>
		///     The operation is executed at the end of the frame which had no other dispatcher operations of higher priority (<see cref="Now" /> or <see cref="Frame" />).
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         Use with caution! An operation with this priority is non-deterministic and might never be executed in cases where operations of higher priorities are constantly dispatched!
		///     </note>
		/// </remarks>
		Later = 2,

		/// <summary>
		///     The operation is executed at the end of the frame which had no other dispatcher operations of higher priority (<see cref="Now" />, <see cref="Frame" />, or <see cref="Later" />) and whose previous frame had no dispatcher operations at all.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         Use with caution! An operation with this priority is non-deterministic and might never be executed in cases where operations of higher priorities are constantly dispatched!
		///     </note>
		/// </remarks>
		Idle = 3,

		/// <summary>
		///     The operation is executed with the lowest possible priority.
		/// </summary>
		Lowest = DispatcherPriority.Idle,

		/// <summary>
		///     The operation is executed with the highest possible priority.
		/// </summary>
		Highest = DispatcherPriority.Now,
	}
}
