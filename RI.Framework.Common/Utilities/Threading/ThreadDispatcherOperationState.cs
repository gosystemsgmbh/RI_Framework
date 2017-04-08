using System;




namespace RI.Framework.Utilities.Threading
{
	/// <summary>
	/// Describes the current processing state of a <see cref="ThreadDispatcherOperation"/>.
	/// </summary>
	[Serializable]
	public enum ThreadDispatcherOperationState
	{
		/// <summary>
		/// The operation is still pending for processing.
		/// </summary>
		Waiting = 0,

		/// <summary>
		/// The operation is currently being processed.
		/// </summary>
		Executing = 1,

		/// <summary>
		/// The operation has successfully finished processing (without an exception).
		/// </summary>
		Finished = 2,

		/// <summary>
		/// The operation was canceled before it could be processed.
		/// </summary>
		Canceled = 3,

		/// <summary>
		/// The operation has finished processing but an exception ocurred during processing.
		/// </summary>
		Exception = 4,

		/// <summary>
		/// The thread, in which the operation was supposed to be executed, was aborted during the execution of the delegate.
		/// </summary>
		Aborted = 5,
	}
}