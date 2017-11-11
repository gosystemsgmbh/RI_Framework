using System;

using RI.Framework.Bus.Internals;




namespace RI.Framework.Bus
{
	/// <summary>
	///     Event arguments for the <see cref="IBus" />.<see cref="IBus.ProcessingException" /> event.
	/// </summary>
	[Serializable]
	public sealed class BusMessageProcessingExceptionEventArgs : EventArgs
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="BusMessageProcessingExceptionEventArgs" />.
		/// </summary>
		/// <param name="message"> The message. </param>
		/// <param name="result"> The result to respond to the sender. </param>
		/// <param name="exception"> The exception. </param>
		/// <param name="forwardException"> Specifies whether the exception should be forwarded to the sender. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> or <paramref name="exception" /> is null. </exception>
		public BusMessageProcessingExceptionEventArgs (MessageItem message, object result, Exception exception, bool forwardException)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			if (exception == null)
			{
				throw new ArgumentNullException(nameof(exception));
			}

			this.Message = message;
			this.Result = result;
			this.Exception = exception;
			this.ForwardException = forwardException;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets os sets the exception.
		/// </summary>
		/// <value>
		///     The exception.
		/// </value>
		public Exception Exception { get; set; }

		/// <summary>
		///     Gets or sets whether the exception should be forwarded to the sender.
		/// </summary>
		/// <value>
		///     true if the exception should be forwarded to the sender, false otherwise.
		/// </value>
		public bool ForwardException { get; set; }

		/// <summary>
		///     Gets the message.
		/// </summary>
		/// <value>
		///     The message.
		/// </value>
		public MessageItem Message { get; }

		/// <summary>
		///     Gets or sets the result to respond to the sender.
		/// </summary>
		/// <value>
		///     The result to respond to the sender.
		/// </value>
		public object Result { get; set; }

		#endregion
	}
}
