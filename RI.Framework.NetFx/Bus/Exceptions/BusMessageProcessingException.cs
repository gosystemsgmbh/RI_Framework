using System;
using System.Runtime.Serialization;

using RI.Framework.Bus.Internals;




namespace RI.Framework.Bus.Exceptions
{
	/// <summary>
	///     The <see cref="BusMessageProcessingException" /> is thrown when the receiver threw an exception during message processing and exception forwarding is used to forward the exception to the sender.
	/// </summary>
	[Serializable]
	public class BusMessageProcessingException : BusOperationalException
	{
		#region Constants

		private const string ExceptionMessageWithMessageAndObject = "Exception while receiver processed message (Message: {1}): {0}.";
		private const string ExceptionMessageWithMessageAndException = "Exception while receiver processed message: {0} (Message: {1}).";
		private const string ExceptionMessageWithException = "Exception while receiver processed message: {0}: {1}";
		private const string ExceptionMessageWithoutException = "Exception while receiver processed message.";

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="BusMessageProcessingException" />.
		/// </summary>
		public BusMessageProcessingException ()
			: base(BusMessageProcessingException.ExceptionMessageWithoutException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="BusMessageProcessingException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		public BusMessageProcessingException (string message)
			: base(message)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="BusMessageProcessingException" />.
		/// </summary>
		/// <param name="innerException"> The exception which triggered this exception. </param>
		public BusMessageProcessingException (Exception innerException)
			: base(string.Format(BusMessageProcessingException.ExceptionMessageWithException, innerException.GetType().Name, innerException.Message), innerException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="BusMessageProcessingException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		/// <param name="innerException"> The exception which triggered this exception. </param>
		public BusMessageProcessingException (string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="BusMessageProcessingException" />.
		/// </summary>
		/// <param name="message"> The message whose processing threw an exception. </param>
		/// <param name="exception"> The exception which was thrown when processing the message. </param>
		public BusMessageProcessingException(MessageItem message, Exception exception)
			: base(string.Format(BusMessageProcessingException.ExceptionMessageWithMessageAndException, MessageItem.CreateExceptionMessage(exception, false), message), exception)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="BusMessageProcessingException" />.
		/// </summary>
		/// <param name="message"> The message whose processing threw an exception. </param>
		/// <param name="exception"> The exception which was thrown when processing the message. </param>
		public BusMessageProcessingException(MessageItem message, object exception)
			: base(string.Format(BusMessageProcessingException.ExceptionMessageWithMessageAndObject, MessageItem.CreateExceptionMessage(exception, false), message))
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="BusMessageProcessingException" />.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected BusMessageProcessingException (SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion
	}
}
