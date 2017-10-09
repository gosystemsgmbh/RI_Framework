using System;
using System.Runtime.Serialization;




namespace RI.Framework.Utilities.Exceptions
{
	/// <summary>
	///     The <see cref="StartupException" /> is thrown when the program cannot complete its startup procedure.
	/// </summary>
	[Serializable]
	public class StartupException : Exception
	{
		#region Constants

		private const string ExceptionMessage = "Startup failed.";

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="StartupException" />.
		/// </summary>
		public StartupException()
			: base(StartupException.ExceptionMessage)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="StartupException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		public StartupException(string message)
			: base(message)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="StartupException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		/// <param name="innerException"> The exception which triggered this exception. </param>
		public StartupException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="StartupException" />.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected StartupException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion
	}
}
