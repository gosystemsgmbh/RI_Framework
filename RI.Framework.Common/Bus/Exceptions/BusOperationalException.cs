using System;
using System.Runtime.Serialization;




namespace RI.Framework.Bus.Exceptions
{
    /// <summary>
    ///     The <see cref="BusOperationalException" /> is thrown when there was an exceptional event or some bus components is in an exceptional state but the bus continues to be operational without side-effects.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    [Serializable]
    public class BusOperationalException : Exception
    {
        #region Constants

        private const string ExceptionMessageWithException = "Bus operational exception: {0}: {1}";
        private const string ExceptionMessageWithoutException = "Bus operational exception.";

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="BusOperationalException" />.
        /// </summary>
        public BusOperationalException ()
            : base(BusOperationalException.ExceptionMessageWithoutException)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="BusOperationalException" />.
        /// </summary>
        /// <param name="message"> The message which describes the exception. </param>
        public BusOperationalException (string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="BusOperationalException" />.
        /// </summary>
        /// <param name="innerException"> The exception which triggered this exception. </param>
        public BusOperationalException (Exception innerException)
            : base(string.Format(BusOperationalException.ExceptionMessageWithException, innerException.GetType().Name, innerException.Message), innerException)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="BusOperationalException" />.
        /// </summary>
        /// <param name="message"> The message which describes the exception. </param>
        /// <param name="innerException"> The exception which triggered this exception. </param>
        public BusOperationalException (string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="BusOperationalException" />.
        /// </summary>
        /// <param name="info"> The serialization data. </param>
        /// <param name="context"> The type of the source of the serialization data. </param>
        protected BusOperationalException (SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
