using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;




namespace RI.Framework.Utilities.Exceptions
{
	/// <summary>
	///     The <see cref="InvalidPathArgumentException" /> is thrown when an argument is not a valid path or a path of an unexpected type.
	/// </summary>
	[Serializable]
	[SuppressMessage ("ReSharper", "ClassCanBeSealed.Global")]
	[SuppressMessage ("ReSharper", "UnusedMember.Global")]
	public class InvalidPathArgumentException : ArgumentException
	{
		#region Constants

		private const string ExceptionMessage = "The path argument is invalid.";

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="InvalidPathArgumentException" />.
		/// </summary>
		public InvalidPathArgumentException ()
				: base(InvalidPathArgumentException.ExceptionMessage)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="InvalidPathArgumentException" />.
		/// </summary>
		/// <param name="paramName"> The parameter whose type is invalid. </param>
		public InvalidPathArgumentException (string paramName)
				: base(InvalidPathArgumentException.ExceptionMessage, paramName)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="InvalidPathArgumentException" />.
		/// </summary>
		/// <param name="paramName"> The parameter whose type is invalid. </param>
		/// <param name="message"> The message which describes the exception. </param>
		public InvalidPathArgumentException (string paramName, string message)
				: base(message, paramName)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="InvalidPathArgumentException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		/// <param name="innerException"> The exception which triggered this exception. </param>
		public InvalidPathArgumentException (string message, Exception innerException)
				: base(message, innerException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="InvalidPathArgumentException" />.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected InvalidPathArgumentException (SerializationInfo info, StreamingContext context)
				: base(info, context)
		{
		}

		#endregion
	}
}
