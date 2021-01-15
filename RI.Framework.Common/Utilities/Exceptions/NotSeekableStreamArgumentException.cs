using System;
using System.IO;
using System.Runtime.Serialization;




namespace RI.Framework.Utilities.Exceptions
{
    /// <summary>
    ///     The <see cref="NotSeekableStreamArgumentException" /> is thrown when a <see cref="Stream" /> argument cannot be accessed randomly.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    [Serializable]
    public class NotSeekableStreamArgumentException : ArgumentException
    {
        #region Constants

        private const string ExceptionMessage = "The Stream argument is not seekable.";

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="NotSeekableStreamArgumentException" />.
        /// </summary>
        public NotSeekableStreamArgumentException ()
            : base(NotSeekableStreamArgumentException.ExceptionMessage)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="NotSeekableStreamArgumentException" />.
        /// </summary>
        /// <param name="paramName"> The parameter which is a not randomly accessible <see cref="Stream" />. </param>
        public NotSeekableStreamArgumentException (string paramName)
            : base(NotSeekableStreamArgumentException.ExceptionMessage, paramName)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="NotSeekableStreamArgumentException" />.
        /// </summary>
        /// <param name="paramName"> The parameter which is a not randomly accessible <see cref="Stream" />. </param>
        /// <param name="message"> The message which describes the exception. </param>
        public NotSeekableStreamArgumentException (string paramName, string message)
            : base(message, paramName)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="NotSeekableStreamArgumentException" />.
        /// </summary>
        /// <param name="message"> The message which describes the exception. </param>
        /// <param name="innerException"> The exception which triggered this exception. </param>
        public NotSeekableStreamArgumentException (string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="NotSeekableStreamArgumentException" />.
        /// </summary>
        /// <param name="info"> The serialization data. </param>
        /// <param name="context"> The type of the source of the serialization data. </param>
        protected NotSeekableStreamArgumentException (SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
