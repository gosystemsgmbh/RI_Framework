using System;
using System.Runtime.Serialization;

namespace RI.Framework.Utilities.Exceptions
{
	/// <summary>
	///     The <see cref="InvalidStateOrExecutionPathException" /> is thrown when the program reaches a state or execution path which it should never reach and therefore assumes it is in an invalid state.
	/// </summary>
	[Serializable]
	public class InvalidStateOrExecutionPathException : Exception
	{
		#region Constants

		private const string ExceptionMessage = "Invalid state or execution path.";

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="InvalidStateOrExecutionPathException" />.
		/// </summary>
		public InvalidStateOrExecutionPathException()
			: base(InvalidStateOrExecutionPathException.ExceptionMessage)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="InvalidStateOrExecutionPathException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		public InvalidStateOrExecutionPathException(string message)
			: base(message)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="InvalidStateOrExecutionPathException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		/// <param name="innerException"> The exception which triggered this exception. </param>
		public InvalidStateOrExecutionPathException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="InvalidStateOrExecutionPathException" />.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected InvalidStateOrExecutionPathException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion
	}
}
