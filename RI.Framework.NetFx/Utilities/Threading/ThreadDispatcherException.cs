using System;
using System.Runtime.Serialization;




namespace RI.Framework.Utilities.Threading
{
	/// <summary>
	///     The <see cref="ThreadDispatcherException" /> is thrown when an operation of a <see cref="ThreadDispatcher" /> had an exception.
	/// </summary>
	[Serializable]
	public class ThreadDispatcherException : Exception
	{
		#region Constants

		private const string ExceptionMessageWithException = "Exception in dispatcher ({0}): {1}";
		private const string ExceptionMessageWithoutException = "Exception in dispatcher.";

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ThreadDispatcherException" />.
		/// </summary>
		public ThreadDispatcherException ()
			: base(ThreadDispatcherException.ExceptionMessageWithoutException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="ThreadDispatcherException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		public ThreadDispatcherException (string message)
			: base(message)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="ThreadDispatcherException" />.
		/// </summary>
		/// <param name="innerException"> The exception which triggered this exception. </param>
		public ThreadDispatcherException (Exception innerException)
			: base(string.Format(ThreadDispatcherException.ExceptionMessageWithException, innerException.GetType().Name, innerException.Message), innerException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="ThreadDispatcherException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		/// <param name="innerException"> The exception which triggered this exception. </param>
		public ThreadDispatcherException (string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="ThreadDispatcherException" />.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected ThreadDispatcherException (SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion
	}
}
