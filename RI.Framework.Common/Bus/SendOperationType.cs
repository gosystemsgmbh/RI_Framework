namespace RI.Framework.Bus
{
    /// <summary>
    /// Specifies the send operation type of a <see cref="SendOperation"/>.
    /// </summary>
    public enum SendOperationType
    {
        /// <summary>
        /// The send operation type has not (yet) been defined.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The send operation sends one message and expects exactly one response.
        /// </summary>
        Single = 1,

        /// <summary>
        /// The send operation sends one message and expects zero, one, or more responses.
        /// </summary>
        Broadcast = 2,

        /// <summary>
        /// The send operation sends one message and expects no response.
        /// </summary>
        FireAndForget = 3,
    }
}
