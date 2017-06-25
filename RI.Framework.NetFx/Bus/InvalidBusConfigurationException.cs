using System;
using System.Runtime.Serialization;




namespace RI.Framework.Bus
{
	/// <summary>
	///     The <see cref="InvalidBusConfigurationException" /> is thrown when a bus context is attempted to be started with an invalid configuration (e.g. no bus connections).
	/// </summary>
	[Serializable]
	public class InvalidBusConfigurationException : Exception
	{
		#region Constants

		private const string ExceptionMessageWithMessage = "Invalid bus configuration: {0}";
		private const string ExceptionMessageWithoutMessage = "Invalid bus configuration.";

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="InvalidBusConfigurationException" />.
		/// </summary>
		public InvalidBusConfigurationException ()
			: base(InvalidBusConfigurationException.ExceptionMessageWithoutMessage)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="InvalidBusConfigurationException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		public InvalidBusConfigurationException (string message)
			: base(string.Format(InvalidBusConfigurationException.ExceptionMessageWithMessage, message))
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="InvalidBusConfigurationException" />.
		/// </summary>
		/// <param name="innerException"> The exception which triggered this exception. </param>
		public InvalidBusConfigurationException (Exception innerException)
			: base(string.Format(InvalidBusConfigurationException.ExceptionMessageWithMessage, innerException.Message), innerException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="InvalidBusConfigurationException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		/// <param name="innerException"> The exception which triggered this exception. </param>
		public InvalidBusConfigurationException (string message, Exception innerException)
			: base(string.Format(InvalidBusConfigurationException.ExceptionMessageWithMessage, message), innerException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="InvalidBusConfigurationException" />.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected InvalidBusConfigurationException (SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion
	}
}
