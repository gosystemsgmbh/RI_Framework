using System;
using System.Runtime.Serialization;

namespace RI.Framework.Bus.Exceptions
{
	/// <summary>
	///     The <see cref="LocalBusException" /> is thrown when the processing pipeline of a <see cref="LocalBus"/> had an exception.
	/// </summary>
	[Serializable]
	public class LocalBusException : Exception
	{
		#region Constants

		private const string ExceptionMessageWithException = "Exception in local bus processing pipeline ({0}): {1}";
		private const string ExceptionMessageWithoutException = "Exception in local bus processing pipeline.";

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="LocalBusException" />.
		/// </summary>
		public LocalBusException()
			: base(LocalBusException.ExceptionMessageWithoutException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="LocalBusException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		public LocalBusException(string message)
			: base(message)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="LocalBusException" />.
		/// </summary>
		/// <param name="innerException"> The exception which triggered this exception. </param>
		public LocalBusException(Exception innerException)
			: base(string.Format(LocalBusException.ExceptionMessageWithException, innerException.GetType().Name, innerException.Message), innerException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="LocalBusException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		/// <param name="innerException"> The exception which triggered this exception. </param>
		public LocalBusException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="LocalBusException" />.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected LocalBusException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion
	}
}
