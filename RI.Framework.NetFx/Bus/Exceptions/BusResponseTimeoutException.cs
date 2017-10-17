using System;
using System.Runtime.Serialization;

using RI.Framework.Bus.Internals;




namespace RI.Framework.Bus.Exceptions
{
	/// <summary>
	///     The <see cref="BusResponseTimeoutException" /> is thrown when there was no response to a sent message within the specified timeout.
	/// </summary>
	[Serializable]
	public class BusResponseTimeoutException : Exception
	{
		#region Constants

		private const string ExceptionMessageWithException = "Timeout while waiting for response ({0}): {1}";

		private const string ExceptionMessageWithMessage = "Timeout while waiting for response: Address={0}, PayloadType={1}, Broadcast={2}, Timeout={3}";
		private const string ExceptionMessageWithoutException = "Timeout while waiting for response.";

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="BusResponseTimeoutException" />.
		/// </summary>
		public BusResponseTimeoutException ()
			: base(BusResponseTimeoutException.ExceptionMessageWithoutException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="BusResponseTimeoutException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		public BusResponseTimeoutException (string message)
			: base(message)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="BusResponseTimeoutException" />.
		/// </summary>
		/// <param name="innerException"> The exception which triggered this exception. </param>
		public BusResponseTimeoutException (Exception innerException)
			: base(string.Format(BusResponseTimeoutException.ExceptionMessageWithException, innerException.GetType().Name, innerException.Message), innerException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="BusResponseTimeoutException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		/// <param name="innerException"> The exception which triggered this exception. </param>
		public BusResponseTimeoutException (string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="BusResponseTimeoutException" />.
		/// </summary>
		/// <param name="message"> The message whose response timed-out. </param>
		public BusResponseTimeoutException (MessageItem message)
			: base(string.Format(BusResponseTimeoutException.ExceptionMessageWithMessage, message?.Address ?? "[null]", message?.Payload?.GetType()?.Name ?? "[null]", message?.IsBroadcast.ToString() ?? "[null]", message?.Timeout.ToString() ?? "[null]"))
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="BusResponseTimeoutException" />.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected BusResponseTimeoutException (SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion
	}
}
