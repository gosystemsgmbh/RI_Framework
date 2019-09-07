using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Bus.Connections;
using RI.Framework.Bus.Dispatchers;
using RI.Framework.Bus.Exceptions;
using RI.Framework.Bus.Internals;
using RI.Framework.Bus.Routers;
using RI.Framework.Collections;
using RI.Framework.ComponentModel;
using RI.Framework.Composition.Model;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bus.Pipelines
{
    /// <summary>
    ///     Implements a default bus processing pipeline which is suitable for most scenarios.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See <see cref="IBusPipeline" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    /// TODO: Use TaskScheduler optionally provided by dispatcher
    [Export]
    public sealed class DefaultBusPipeline : LogSource, IBusPipeline
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DefaultBusPipeline" />.
        /// </summary>
        public DefaultBusPipeline ()
        {
            this.SyncRoot = new object();

            this.LocalResponses = new Queue<MessageItem>();
            this.BrokenConnections = new HashSet<IBusConnection>();
        }

        #endregion




        #region Instance Properties/Indexer

        private Queue<MessageItem> LocalResponses { get; }
        private HashSet<IBusConnection> BrokenConnections { get; }

        private IBus Bus { get; set; }
        private IBusConnectionManager ConnectionManager { get; set; }
        private IBusDispatcher Dispatcher { get; set; }
        private IBusRouter Router { get; set; }

        #endregion




        #region Instance Methods

        private void ProcessingHandler (ReceiverRegistrationItem receiver, MessageItem message, Task<object> task, Exception exception)
        {
            bool isCompleted = false;
            if (task != null)
            {
                isCompleted = task.IsCompleted;
                Exception taskException = task.Exception;
                if (isCompleted && (taskException != null))
                {
                    exception = taskException;
                }
            }

            if (exception != null)
            {
                bool forwardException = receiver.ReceiverRegistration.ExceptionForwarding.GetValueOrDefault(this.Bus.DefaultReceiveExceptionForwarding) || message.ExceptionForwarding;
                object response = isCompleted ? task.Result : null;
                bool handled = (receiver.ReceiverRegistration.ExceptionHandler ?? this.Bus.DefaultReceiveExceptionHandler)?.Invoke(receiver.ReceiverRegistration, ref exception, ref forwardException, ref response) ?? false;

                if (handled || (exception == null))
                {
                    this.ResponseHandler(message, response, null);
                }
                else
                {
                    if (forwardException)
                    {
                        this.ResponseHandler(message, response, exception);
                    }
                    else
                    {
                        throw new BusMessageProcessingException(exception);
                    }
                }

                return;
            }

            if (task == null)
            {
                throw new InvalidStateOrExecutionPathException("Either a task or an exception must be available.");
            }

            if (isCompleted)
            {
                object result = task.Result;
                this.ResponseHandler(message, result, null);
            }
            else
            {
                task.ContinueWith((t1, s1) => { this.Dispatcher.Dispatch(new Action<Task<object>, Tuple<ReceiverRegistrationItem, MessageItem>>((t2, s2) => { this.ProcessingHandler(s2.Item1, s2.Item2, t2, null); }), t1, s1); }, new Tuple<ReceiverRegistrationItem, MessageItem>(receiver, message), CancellationToken.None, TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.RunContinuationsAsynchronously, TaskScheduler.Current);
            }
        }

        private void ProcessMessage (MessageItem messageItem)
        {
            if (messageItem.ResponseTo.HasValue)
            {
                this.Bus.RaiseReceivingResponse(messageItem);

                lock (this.Bus.SyncRoot)
                {
                    this.Bus.SendOperations.Where(x => (x.Request.Id == messageItem.ResponseTo.Value) && (x.State == SendOperationItemState.Waiting)).ForEach(x =>
                    {
                        x.Responses.Add(messageItem);

                        object result = messageItem.Payload;
                        object exception = messageItem.Exception;
                        bool hasResult = true;

                        if (exception != null)
                        {
                            bool handled = (x.SendOperation.ExceptionHandler ?? this.Bus.DefaultSendExceptionHandler)?.Invoke(x.SendOperation, ref exception, ref result) ?? false;
                            if ((!handled) && (exception != null))
                            {
                                hasResult = false;
                                this.Log(LogLevel.Debug, "Send operation failed with forwarded exception: {0}{1}{2}", x, Environment.NewLine, MessageItem.CreateExceptionMessage(exception, true));
                                x.Results.Add(result);
                                x.Exception = exception;
                                x.State = SendOperationItemState.ForwardedException;
                                Exception realException = exception as Exception;
                                if (realException != null)
                                {
                                    x.Task.TrySetException(new BusMessageProcessingException(x.Request, realException));
                                }
                                else
                                {
                                    x.Task.TrySetException(new BusMessageProcessingException(x.Request, x.Exception));
                                }
                            }
                        }

                        if (hasResult)
                        {
                            x.Results.Add(result);
                            x.Exception = exception;
                            if (x.SendOperation.OperationType == SendOperationType.Broadcast)
                            {
                                if (x.SendOperation.ExpectedResults.HasValue && (x.Results.Count >= x.SendOperation.ExpectedResults.Value))
                                {
                                    this.Log(LogLevel.Debug, "Send operation finished collection after {0} responses: {1}", x.Results.Count, x);
                                    x.State = SendOperationItemState.Finished;
                                    x.Task.TrySetResult(x.Results);
                                }
                            }
                            else
                            {
                                this.Log(LogLevel.Debug, "Send operation finished with response: {0}", x);
                                x.State = SendOperationItemState.Finished;
                                x.Task.TrySetResult(x.Results[0]);
                            }
                        }
                    });
                }
            }
            else
            {
                this.Bus.RaiseReceivingRequest(messageItem);
            }

            bool toLocal = this.Router.ForwardToLocal(messageItem);
            bool toGlobal = this.Router.ForwardToGlobal(messageItem);

            if (toLocal)
            {
                lock (this.Bus.SyncRoot)
                {
                    this.Bus.ReceiveRegistrations.Where(x => this.Router.ShouldReceive(messageItem, x)).ForEach(x =>
                    {
                        this.Log(LogLevel.Debug, "Dispatching message processing: Request=[{0}], Receiver=[{1}]", messageItem, x);
                        this.Dispatcher.Dispatch(new Action<ReceiverRegistrationItem, MessageItem>((r, m) =>
                        {
                            this.Log(LogLevel.Debug, "Executing message processing: Request=[{0}], Receiver=[{1}]", m, r);
                            Task<object> task = null;
                            Exception exception = null;
                            try
                            {
                                task = r.ReceiverRegistration.Callback(m.Address, m.Payload);
                            }
                            catch (Exception ex)
                            {
                                exception = ex;
                            }
                            this.ProcessingHandler(r, m, task, exception);
                        }), x, messageItem);
                    });
                }
            }

            if (toGlobal && (this.ConnectionManager != null))
            {
                lock (this.ConnectionManager.SyncRoot)
                {
                    this.ConnectionManager.Connections.Where(x => this.Router.ShouldSend(messageItem, x)).ForEach(x => this.ConnectionManager.SendMessage(messageItem, x));
                }
            }
        }

        private void ResponseHandler (MessageItem message, object result, Exception exception)
        {
            MessageItem response = new MessageItem();
            response.Address = message.Address;
            response.Payload = result;
            response.ToGlobal = message.FromGlobal;
            response.Timeout = message.Timeout;
            response.OperationType = message.OperationType;
            response.ExceptionForwarding = false;
            response.Id = Guid.NewGuid();
            response.Sent = DateTime.UtcNow;
            response.FromGlobal = false;
            response.ResponseTo = message.Id;
            response.RoutingInfo = message.RoutingInfo;
            response.Exception = exception;

            this.Log(LogLevel.Debug, "Finished message processing: Request=[{0}], Response=[{1}]", message, response);

            if (response.OperationType == SendOperationType.FireAndForget)
            {
                response.Address = null;
                response.ResponseTo = null;

                if ((response.Payload == null) || (response.Exception != null))
                {
                    lock (this.SyncRoot)
                    {
                        this.Bus.SignalWorkAvailable();
                    }

                    return;
                }
            }

            lock (this.SyncRoot)
            {
                this.LocalResponses.Enqueue(response);
                this.Bus.SignalWorkAvailable();
            }
        }

        #endregion




        #region Interface: IBusPipeline

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        public object SyncRoot { get; }

        /// <inheritdoc />
        public void DoWork (bool polling)
        {
            DateTime utcNow = DateTime.UtcNow;

            List<Tuple<MessageItem, IBusConnection>> receivedMessages = new List<Tuple<MessageItem, IBusConnection>>();
            List<MessageItem> localResponses = new List<MessageItem>();
            List<MessageItem> localRequests = new List<MessageItem>();

            this.ConnectionManager?.DequeueMessages(receivedMessages);
            receivedMessages.ForEach(x => x.Item1.FromGlobal = true);

            lock (this.SyncRoot)
            {
                this.LocalResponses.DequeueInto(localResponses);
            }

            int removedMessages;
            lock (this.Bus.SyncRoot)
            {
                this.Bus.SendOperations.Where(x => (x.State == SendOperationItemState.New) || (x.State == SendOperationItemState.Waiting)).ForEach(x =>
                {
                    if (x.SendOperation.CancellationToken?.IsCancellationRequested ?? false)
                    {
                        this.Log(LogLevel.Debug, "Send operation canceled: {0}", x);
                        x.State = SendOperationItemState.Cancelled;
                        x.Task.TrySetCanceled(x.SendOperation.CancellationToken.Value);
                    }
                });

                this.Bus.SendOperations.Where(x => x.State == SendOperationItemState.New).ForEach(x =>
                {
                    x.State = SendOperationItemState.Waiting;

                    x.Request.Address = x.SendOperation.Address;
                    x.Request.Payload = x.SendOperation.Payload;
                    x.Request.ToGlobal = x.SendOperation.Global.GetValueOrDefault(this.Bus.DefaultSendToGlobal);
                    x.Request.Timeout = (int)x.SendOperation.Timeout.GetValueOrDefault(x.SendOperation.OperationType == SendOperationType.Broadcast ? this.Bus.DefaultBroadcastResponseTimeout : this.Bus.DefaultSingleResponseTimeout).TotalMilliseconds;
                    x.Request.OperationType = x.SendOperation.OperationType;
                    x.Request.ExceptionForwarding = x.SendOperation.ExceptionForwarding.GetValueOrDefault(this.Bus.DefaultSendExceptionForwarding);
                    x.Request.Id = Guid.NewGuid();
                    x.Request.Sent = utcNow;
                    x.Request.FromGlobal = false;
                    x.Request.ResponseTo = null;
                    x.Request.RoutingInfo = null;
                    x.Request.Exception = null;

                    localRequests.Add(x.Request);

                    this.Log(LogLevel.Debug, "Send operation created: {0}", x);
                });

                this.Bus.SendOperations.Where(x => x.State == SendOperationItemState.Waiting).ForEach(x =>
                {
                    TimeSpan duration = utcNow.Subtract(x.Request.Sent);
                    if (duration.TotalMilliseconds > x.Request.Timeout)
                    {
                        if (x.SendOperation.OperationType == SendOperationType.Broadcast)
                        {
                            this.Log(LogLevel.Debug, "Send operation finished collection after {0} ms: {1}", x.Request.Timeout, x);
                            x.State = SendOperationItemState.Finished;
                            x.Task.TrySetResult(x.Results);
                        }
                        else
                        {
                            this.Log(LogLevel.Debug, "Send operation timed-out after {0} ms: {1}", x.Request.Timeout, x);
                            x.State = SendOperationItemState.TimedOut;
                            x.Task.TrySetException(new BusResponseTimeoutException(x.Request));
                        }
                    }
                    else if (x.SendOperation.OperationType == SendOperationType.FireAndForget)
                    {
                        x.State = SendOperationItemState.Finished;
                        x.Task.TrySetResult(null);
                    }
                });

                if (this.ConnectionManager != null)
                {
                    lock (this.ConnectionManager.SyncRoot)
                    {
                        List<IBusConnection> brokenConnections = this.ConnectionManager.Connections.Where(x => x.IsBroken).ToList();
                        IEnumerable<IBusConnection> newBrokenConnections = brokenConnections.Except(this.BrokenConnections);
                        this.BrokenConnections.Clear();
                        this.BrokenConnections.AddRange(brokenConnections);

                        foreach (IBusConnection newBrokenConnection in newBrokenConnections)
                        {
                            this.Log(LogLevel.Debug, "Broken connection: {0} ({1})", newBrokenConnection, newBrokenConnection.BrokenMessage);
                            this.Bus.RaiseConnectionBroken(newBrokenConnection);
                        }

                        if (brokenConnections.Count > 0)
                        {
                            this.Bus.SendOperations.Where(x => x.Request.ToGlobal && (x.State == SendOperationItemState.Waiting) && (!x.SendOperation.IgnoreBrokenConnections.GetValueOrDefault(this.Bus.DefaultSendIgnoredBrokenConnections))).ForEach(x =>
                            {
                                this.Log(LogLevel.Debug, "Send operation failed with broken connection: {0}", x);
                                x.State = SendOperationItemState.Broken;
                                x.Task.TrySetException(new BusConnectionBrokenException(x.Request, brokenConnections[0]));
                            });
                        }
                    }
                }

                removedMessages = this.Bus.SendOperations.RemoveAll(x => (x.State == SendOperationItemState.Finished) || (x.State == SendOperationItemState.TimedOut) || (x.State == SendOperationItemState.Cancelled) || (x.State == SendOperationItemState.Broken) || (x.State == SendOperationItemState.ForwardedException));
            }


            if (((localResponses.Count == 0)) && (localRequests.Count == 0) && (receivedMessages.Count == 0) && (removedMessages == 0))
            {
                return;
            }

            localResponses.ForEach(x => this.Bus.RaiseSendingResponse(x));
            localRequests.ForEach(x => this.Bus.RaiseSendingRequest(x));

            receivedMessages.ForEach(x => this.Router.ReceivedFromRemote(x.Item1, x.Item2));
            localResponses.ForEach(x => this.Router.ReceivedFromLocal(x));
            localRequests.ForEach(x => this.Router.ReceivedFromLocal(x));

            receivedMessages.ForEach(x => this.ProcessMessage(x.Item1));
            localResponses.ForEach(this.ProcessMessage);
            localRequests.ForEach(this.ProcessMessage);

            this.Bus.SignalWorkAvailable();
        }

        /// <inheritdoc />
        public void Initialize (IDependencyResolver dependencyResolver)
        {
            lock (this.SyncRoot)
            {
                this.Bus = dependencyResolver.GetInstance<IBus>();
                this.Router = dependencyResolver.GetInstance<IBusRouter>();
                this.Dispatcher = dependencyResolver.GetInstance<IBusDispatcher>();

                this.ConnectionManager = dependencyResolver.GetInstance<IBusConnectionManager>();
            }
        }

        /// <inheritdoc />
        public void Unload ()
        {
            lock (this.SyncRoot)
            {
                this.LocalResponses?.Clear();

                this.BrokenConnections?.Clear();
            }
        }

        #endregion
    }
}
