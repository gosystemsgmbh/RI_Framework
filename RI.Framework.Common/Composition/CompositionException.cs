using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;




namespace RI.Framework.Composition
{
	/// <summary>
	///     The <see cref="CompositionException" /> is thrown when a composition fails.
	/// </summary>
	/// <remarks>
	///     <note type="important">
	///         If a <see cref="CompositionException" /> is thrown during a composition, the state of the <see cref="CompositionContainer" /> and all its compositions are undefined and might remain unusable.
	///         Therefore, a <see cref="CompositionException" /> should always be treated as a serious error which prevents the program from continueing normally.
	///     </note>
	/// </remarks>
	[Serializable]
	[SuppressMessage ("ReSharper", "UnusedMember.Global")]
	[SuppressMessage ("ReSharper", "MemberCanBeInternal")]
	public sealed class CompositionException : InvalidOperationException
	{
		#region Constants

		private const string GenericExceptionMessage = "Composition failed.";

		private const string SpecificExceptionMessage = "Composition failed: {0}";

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="CompositionException" />.
		/// </summary>
		public CompositionException ()
				: base(CompositionException.GenericExceptionMessage)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="CompositionException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		public CompositionException (string message)
				: base(string.Format(CompositionException.SpecificExceptionMessage, message))
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="CompositionException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		/// <param name="innerException"> The exception which triggered this exception. </param>
		public CompositionException (string message, Exception innerException)
				: base(string.Format(CompositionException.SpecificExceptionMessage, message), innerException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="CompositionException" />.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		private CompositionException (SerializationInfo info, StreamingContext context)
				: base(info, context)
		{
		}

		#endregion
	}
}
