using System;
using System.Runtime.Serialization;




namespace RI.Framework.Bus.Exceptions
{
	/// <summary>
	///     The <see cref="BusProcessingPipelineException" /> is thrown when the bus processing pipeline had an exception and is no longer working.
	/// </summary>
	[Serializable]
	public class BusProcessingPipelineException : Exception
	{
		#region Constants

		private const string ExceptionMessageWithException = "Exception in bus processing pipeline: {0}: {1}";
		private const string ExceptionMessageWithoutException = "Exception in bus processing pipeline.";

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="BusProcessingPipelineException" />.
		/// </summary>
		public BusProcessingPipelineException ()
			: base(BusProcessingPipelineException.ExceptionMessageWithoutException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="BusProcessingPipelineException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		public BusProcessingPipelineException (string message)
			: base(message)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="BusProcessingPipelineException" />.
		/// </summary>
		/// <param name="innerException"> The exception which triggered this exception. </param>
		public BusProcessingPipelineException (Exception innerException)
			: base(string.Format(BusProcessingPipelineException.ExceptionMessageWithException, innerException.GetType().Name, innerException.Message), innerException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="BusProcessingPipelineException" />.
		/// </summary>
		/// <param name="message"> The message which describes the exception. </param>
		/// <param name="innerException"> The exception which triggered this exception. </param>
		public BusProcessingPipelineException (string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="BusProcessingPipelineException" />.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected BusProcessingPipelineException (SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion
	}
}
