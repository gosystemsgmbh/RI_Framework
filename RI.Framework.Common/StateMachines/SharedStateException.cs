using System;
using System.Runtime.Serialization;




namespace RI.Framework.StateMachines
{
    /// <summary>
    ///     The <see cref="SharedStateException" /> is thrown when a single state instance is used by more than one state machine.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    [Serializable]
    public class SharedStateException : Exception
    {
        #region Constants

        private const string ExceptionMessage = "A single instance of the state \"{0}\" is shared with multiple state machines.";

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SharedStateException" />.
        /// </summary>
        /// <param name="state"> The states type which is shared among multiple state machines. </param>
        public SharedStateException(Type state)
            : base(string.Format(SharedStateException.ExceptionMessage, state?.Name ?? "[null]"))
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SharedStateException" />.
        /// </summary>
        /// <param name="message"> The message which describes the exception. </param>
        /// <param name="innerException"> The exception which triggered this exception. </param>
        public SharedStateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SharedStateException" />.
        /// </summary>
        /// <param name="info"> The serialization data. </param>
        /// <param name="context"> The type of the source of the serialization data. </param>
        protected SharedStateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
