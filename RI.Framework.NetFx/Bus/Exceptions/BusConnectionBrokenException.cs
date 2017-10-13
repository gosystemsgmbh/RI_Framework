using System;
using System.Runtime.Serialization;

using RI.Framework.Bus.Connections;

namespace RI.Framework.Bus.Exceptions
{
	/// <summary>
	///     The <see cref="BusConnectionBrokenException" /> is thrown when a used connection to a remote bus is broken.
	/// </summary>
	[Serializable]
	public class BusConnectionBrokenException : Exception
	{
		#region Constants

		private const string ExceptionMessageWithConnection = "Bus connection is broken: ({0}): {1}";
		private const string ExceptionMessageWithException = "Bus connection is broken ({0}): {1}";
		private const string ExceptionMessageWithoutException = "Bus connection is broken.";

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="BusConnectionBrokenException" />.
		/// </summary>
		public BusConnectionBrokenException()
			: base(BusConnectionBrokenException.ExceptionMessageWithoutException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="BusConnectionBrokenException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		public BusConnectionBrokenException(string message)
			: base(message)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="BusConnectionBrokenException" />.
		/// </summary>
		/// <param name="innerException"> The exception which triggered this exception. </param>
		public BusConnectionBrokenException(Exception innerException)
			: base(string.Format(BusConnectionBrokenException.ExceptionMessageWithException, innerException.GetType().Name, innerException.Message), innerException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="BusConnectionBrokenException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		/// <param name="innerException"> The exception which triggered this exception. </param>
		public BusConnectionBrokenException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="BusConnectionBrokenException" />.
		/// </summary>
		/// <param name="connection"> The connection which broke. </param>
		public BusConnectionBrokenException(IBusConnection connection)
			: base(string.Format(BusConnectionBrokenException.ExceptionMessageWithConnection, connection.GetType().Name, connection.BrokenMessage ?? "[unknown]"))
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="BusConnectionBrokenException" />.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected BusConnectionBrokenException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion
	}
}
