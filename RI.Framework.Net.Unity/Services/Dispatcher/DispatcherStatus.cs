using System;




namespace RI.Framework.Services.Dispatcher
{
    /// <summary>
    ///     Describes the current state of a dispatcher operation.
    /// </summary>
    [Serializable]
    public enum DispatcherStatus
    {
        /// <summary>
        ///     The operation is still queued for execution.
        /// </summary>
        Queued = 0,

        /// <summary>
        ///     The operation is currently being executed.
        /// </summary>
        Processing = 1,

        /// <summary>
        ///     The operation has finished execution and is considered finished.
        /// </summary>
        Processed = 2,

        /// <summary>
        ///     The operation has been canceled without being executed.
        /// </summary>
        Canceled = 3,

        /// <summary>
        ///     The operation has timed-out without being executed.
        /// </summary>
        Timeout = 4,
    }
}
