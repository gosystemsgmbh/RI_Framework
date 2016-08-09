using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;




namespace RI.Framework.Utilities.Exceptions
{
	/// <summary>
	///     The <see cref="InvalidTypeArgumentException" /> is thrown when an argument is not of an expected or compatible type.
	/// </summary>
	[Serializable]
	[SuppressMessage ("ReSharper", "ClassCanBeSealed.Global")]
	[SuppressMessage ("ReSharper", "UnusedMember.Global")]
	public class InvalidTypeArgumentException : ArgumentException
	{
		#region Constants

		private const string ExceptionMessage = "The argument type is invalid.";

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="InvalidTypeArgumentException" />.
		/// </summary>
		public InvalidTypeArgumentException ()
				: base(InvalidTypeArgumentException.ExceptionMessage)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="InvalidTypeArgumentException" />.
		/// </summary>
		/// <param name="paramName"> The parameter whose type is invalid. </param>
		public InvalidTypeArgumentException (string paramName)
				: base(InvalidTypeArgumentException.ExceptionMessage, paramName)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="InvalidTypeArgumentException" />.
		/// </summary>
		/// <param name="paramName"> The parameter whose type is invalid. </param>
		/// <param name="message"> The message which describes the exception. </param>
		public InvalidTypeArgumentException (string paramName, string message)
				: base(message, paramName)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="InvalidTypeArgumentException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		/// <param name="innerException"> The exception which triggered this exception. </param>
		public InvalidTypeArgumentException (string message, Exception innerException)
				: base(message, innerException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="InvalidTypeArgumentException" />.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected InvalidTypeArgumentException (SerializationInfo info, StreamingContext context)
				: base(info, context)
		{
		}

		#endregion
	}
}
