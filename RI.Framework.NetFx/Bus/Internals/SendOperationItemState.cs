using System;

namespace RI.Framework.Bus.Internals
{
	/// <summary>
	/// Describes the state of a send operation.
	/// </summary>
	[Serializable]
	public enum SendOperationItemState
	{
		/// <summary>
		/// The send operation is newly created and is not yet processed.
		/// </summary>
		New = 0,

		/// <summary>
		/// The message has been sent and the send operation is waiting for response.
		/// </summary>
		Waiting = 1,

		/// <summary>
		/// The send operation finished successfully.
		/// </summary>
		Finished = 3,

		/// <summary>
		/// The send operation timed out.
		/// </summary>
		TimedOut = 4,

		/// <summary>
		/// The send operation was cancelled.
		/// </summary>
		Cancelled = 5,

		/// <summary>
		/// The send operation failed because at least one connection is broken and the message was intended to be sent globally.
		/// </summary>
		Broken = 6,
	}
}
