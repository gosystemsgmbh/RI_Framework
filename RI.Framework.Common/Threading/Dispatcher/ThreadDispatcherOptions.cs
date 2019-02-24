using System;
using System.Threading;




namespace RI.Framework.Threading.Dispatcher
{
    /// <summary>
    ///     Describes delegate execution options.
    /// </summary>
    [Serializable]
    [Flags]
    public enum ThreadDispatcherOptions
    {
        /// <summary>
        ///     No options specified. That is: Nothing is captured.
        /// </summary>
        None = 0x00,

        /// <summary>
        ///     The default options of the dispatcher are used.
        /// </summary>
        Default = 0xFF,

        /// <summary>
        ///     <see cref="Thread.CurrentCulture" /> is captured and used for executing a delegate.
        /// </summary>
        CaptureCurrentCulture = 0x01,

        /// <summary>
        ///     <see cref="Thread.CurrentUICulture" /> is captured and used for executing a delegate.
        /// </summary>
        CaptureCurrentUICulture = 0x02,

        /// <summary>
        ///     <see cref="SynchronizationContext.Current" /> is captured and used for executing a delegate.
        /// </summary>
        CaptureSynchronizationContext = 0x04,

        /// <summary>
        ///     <see cref="ExecutionContext" /> is captured and used for executing a delegate.
        /// </summary>
        CaptureExecutionContext = 0x08,

        /// <summary>
        ///     <see cref="Thread.CurrentCulture" /> and <see cref="Thread.CurrentUICulture" /> are captured and used for executing a delegate.
        /// </summary>
        CaptureCulture = ThreadDispatcherOptions.CaptureCurrentCulture | ThreadDispatcherOptions.CaptureCurrentUICulture,

        /// <summary>
        ///     Everything is captured and used for executing a delegate.
        /// </summary>
        CaptureAll = ThreadDispatcherOptions.CaptureCulture | ThreadDispatcherOptions.CaptureSynchronizationContext | ThreadDispatcherOptions.CaptureExecutionContext,
    }
}
