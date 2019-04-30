using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Collections.DirectLinq;




namespace RI.Framework.Threading
{
    /// <summary>
    ///     Implements a dispatch capture which can be used to dispatch a delegate to either the current <see cref="SynchronizationContext" /> or the <see cref="ThreadPool" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Whether the delegate will be executed on the current <see cref="SynchronizationContext" /> or the <see cref="ThreadPool" /> is decided when an instance of <see cref="DispatchCapture" /> is created (or the current context is &quot;captured&quot; respectively).
    ///     </para>
    ///     <para>
    ///         If at the time of capture <see cref="SynchronizationContext.Current" /> is not null, that <see cref="SynchronizationContext" /> will be used for execution when calling <see cref="Execute" />. If it is null, <see cref="ThreadPool" /> will be used.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public sealed class DispatchCapture
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DispatchCapture" />.
        /// </summary>
        /// <param name="action"> The delegate to execute. </param>
        /// <param name="arguments"> The parameters of the delegate. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
        public DispatchCapture (Delegate action, params object[] arguments)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            this.Action = action;
            this.Arguments = arguments ?? new object[0];
            this.Context = SynchronizationContext.Current;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="DispatchCapture" />.
        /// </summary>
        /// <param name="action"> The delegate to execute. </param>
        /// <param name="arguments"> The parameters of the delegate. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
        public DispatchCapture(Delegate action, IEnumerable<object> arguments)
        :this(action, arguments?.ToArray())
        {
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the delegate to execute.
        /// </summary>
        /// <value>
        ///     The delegate to execute.
        /// </value>
        public Delegate Action { get; }

        /// <summary>
        ///     Gets the parameters of the delegate.
        /// </summary>
        /// <value>
        ///     The parameters of the delegate.
        /// </value>
        public object[] Arguments { get; }

        /// <summary>
        ///     Gets the captured <see cref="SynchronizationContext" />.
        /// </summary>
        /// <value>
        ///     The captured <see cref="SynchronizationContext" /> or null if none was available during capture.
        /// </value>
        public SynchronizationContext Context { get; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Executes the delegate.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The execution is just scheduled and <see cref="Execute"/> returns immediately without waiting for the execution to be completed.
        ///     </para>
        ///     <para>
        ///         Unhandled exceptions, thrown by <see cref="Action"/>, will not be forwarded and remain unhandled.
        ///     </para>
        /// </remarks>
        public void Execute ()
        {
            if (this.Context != null)
            {
                this.Context.Post(x =>
                {
                    DispatchCapture capture = ((DispatchCapture)x);
                    capture.Action.DynamicInvoke(capture.Arguments);
                }, this);
            }
            else
            {
                ThreadPool.QueueUserWorkItem(x =>
                {
                    DispatchCapture capture = ((DispatchCapture)x);
                    capture.Action.DynamicInvoke(capture.Arguments);
                }, this);
            }
        }

        /// <summary>
        ///     Executes the delegate.
        /// </summary>
        /// <param name="forwardExceptions">Specifies whether unhandled exceptions, thrown by <see cref="Action"/>, are forwarded to the returned task.</param>
        /// <returns>
        ///     The task which can be used to await the completion of the execution.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The execution is just scheduled and <see cref="ExecuteAsync"/> returns immediately without waiting for the execution to be completed.
        ///     </para>
        ///     <para>
        ///         If <paramref name="forwardExceptions"/> is false, unhandled exceptions, thrown by <see cref="Action"/>, will not be forwarded and remain unhandled.
        ///     </para>
        /// </remarks> 
        public Task<object> ExecuteAsync (bool forwardExceptions)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            if (this.Context != null)
            {
                this.Context.Post(x =>
                {
                    DispatchCapture capture = ((DispatchCapture)x);
                    object result;
                    if (forwardExceptions)
                    {
                        try
                        {
                            result = capture.Action.DynamicInvoke(capture.Arguments);
                        }
                        catch (Exception exception)
                        {
                            tcs.TrySetException(exception);
                            return;
                        }
                    }
                    else
                    {
                        result = capture.Action.DynamicInvoke(capture.Arguments);
                    }
                    tcs.TrySetResult(result);
                }, this);
            }
            else
            {
                ThreadPool.QueueUserWorkItem(x =>
                {
                    DispatchCapture capture = ((DispatchCapture)x);
                    object result;
                    if (forwardExceptions)
                    {
                        try
                        {
                            result = capture.Action.DynamicInvoke(capture.Arguments);
                        }
                        catch (Exception exception)
                        {
                            tcs.TrySetException(exception);
                            return;
                        }
                    }
                    else
                    {
                        result = capture.Action.DynamicInvoke(capture.Arguments);
                    }
                    tcs.TrySetResult(result);
                }, this);
            }
            return tcs.Task;
        }

        #endregion
    }
}
