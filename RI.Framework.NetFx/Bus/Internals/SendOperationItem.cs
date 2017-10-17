using System;
using System.Collections.Generic;
using System.Threading.Tasks;




namespace RI.Framework.Bus.Internals
{
	/// <summary>
	///     Holds a send operation as managed by a bus implementation.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="SendOperationItem" /> contains additional data which is only used internally by the bus implementation.
	///     </para>
	/// </remarks>
	public sealed class SendOperationItem
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="SendOperationItem" />.
		/// </summary>
		/// <param name="sendOperation"> The send operation. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="sendOperation" /> is null. </exception>
		public SendOperationItem (SendOperation sendOperation)
		{
			if (sendOperation == null)
			{
				throw new ArgumentNullException(nameof(sendOperation));
			}

			this.SendOperation = sendOperation;
			this.Task = new TaskCompletionSource<object>(this.SendOperation, TaskCreationOptions.DenyChildAttach | TaskCreationOptions.RunContinuationsAsynchronously);
			this.State = SendOperationItemState.New;
			this.Request = new MessageItem();
			this.Responses = new List<MessageItem>();
			this.Results = new List<object>();
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     The request message which was created and sent based upon the send operation.
		/// </summary>
		public MessageItem Request { get; }

		/// <summary>
		///     Gets the list of response messages received for this send operation.
		/// </summary>
		/// <value>
		///     The list of response messages received for this send operation.
		/// </value>
		public List<MessageItem> Responses { get; }

		/// <summary>
		///     Gets the list of responses received for this send operation.
		/// </summary>
		/// <value>
		///     The list of responses received for this send operation.
		/// </value>
		public List<object> Results { get; }

		/// <summary>
		///     Gets the send operation.
		/// </summary>
		/// <value>
		///     The send operation.
		/// </value>
		public SendOperation SendOperation { get; }

		/// <summary>
		///     Gets or sets the state of the send operation.
		/// </summary>
		/// <value>
		///     The state of the send operation.
		/// </value>
		public SendOperationItemState State { get; set; }

		/// <summary>
		///     Gets the task which is used to wait for the completion of the send operation.
		/// </summary>
		/// <value>
		///     The task which is used to wait for the completion of the send operation.
		/// </value>
		public TaskCompletionSource<object> Task { get; }

		#endregion
	}
}
