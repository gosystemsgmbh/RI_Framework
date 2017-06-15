using System;
using System.Runtime.Serialization;




namespace RI.Framework.Data.Database
{
	/// <summary>
	///     The <see cref="InvalidDatabaseConfigurationException" /> is thrown when a database is attempted to be initialized with an invalid configuration (e.g. invalid connection string).
	/// </summary>
	[Serializable]
	public class InvalidDatabaseConfigurationException : Exception
	{
		#region Constants

		private const string ExceptionMessageWithMessage = "Invalid database configuration: {0}";
		private const string ExceptionMessageWithoutMessage = "Invalid database configuration.";

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="InvalidDatabaseConfigurationException" />.
		/// </summary>
		public InvalidDatabaseConfigurationException ()
			: base(InvalidDatabaseConfigurationException.ExceptionMessageWithoutMessage)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="InvalidDatabaseConfigurationException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		public InvalidDatabaseConfigurationException (string message)
			: base(string.Format(InvalidDatabaseConfigurationException.ExceptionMessageWithMessage, message))
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="InvalidDatabaseConfigurationException" />.
		/// </summary>
		/// <param name="innerException"> The exception which triggered this exception. </param>
		public InvalidDatabaseConfigurationException (Exception innerException)
			: base(string.Format(InvalidDatabaseConfigurationException.ExceptionMessageWithMessage, innerException.Message), innerException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="InvalidDatabaseConfigurationException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		/// <param name="innerException"> The exception which triggered this exception. </param>
		public InvalidDatabaseConfigurationException (string message, Exception innerException)
			: base(string.Format(InvalidDatabaseConfigurationException.ExceptionMessageWithMessage, message), innerException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="InvalidDatabaseConfigurationException" />.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected InvalidDatabaseConfigurationException (SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion
	}
}
