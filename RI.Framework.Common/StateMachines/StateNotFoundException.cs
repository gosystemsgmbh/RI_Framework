using System;
using System.Runtime.Serialization;




namespace RI.Framework.StateMachines
{
    /// <summary>
    ///     The <see cref="StateNotFoundException" /> is thrown when <see cref="StateMachine.Resolve" /> is unable to resolve the instance for a requested state type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    [Serializable]
    public class StateNotFoundException : Exception
    {
        #region Constants

        private const string ExceptionMessage = "The state \"{0}\" cannot be resolved.";

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="StateNotFoundException" />.
        /// </summary>
        /// <param name="state"> The states type which could not be resolved. </param>
        public StateNotFoundException (Type state)
            : base(string.Format(StateNotFoundException.ExceptionMessage, state?.Name ?? "[null]"))
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="StateNotFoundException" />.
        /// </summary>
        /// <param name="message"> The message which describes the exception. </param>
        /// <param name="innerException"> The exception which triggered this exception. </param>
        public StateNotFoundException (string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="StateNotFoundException" />.
        /// </summary>
        /// <param name="info"> The serialization data. </param>
        /// <param name="context"> The type of the source of the serialization data. </param>
        protected StateNotFoundException (SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
