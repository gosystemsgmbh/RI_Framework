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

        private HashSet<IBusConnection> BrokenConnections { get; }

        private IBus Bus { get; set; }
        private IBusConnectionManager ConnectionManager { get; set; }
        private IBusDispatcher Dispatcher { get; set; }
        private Queue<MessageItem> LocalResponses { get; }
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
                Exception handlerException = exception;
                Exception eventException = exception;

                bool forwardException = receiver.ReceiverRegistration.ExceptionForwarding.GetValueOrDefault(this.Bus.DefaultExceptionForwarding) || message.ExceptionForwarding;
                bool handlerForward = forwardException;
                bool eventForward = forwardException;

                object result = receiver.ReceiverRegistration.ExceptionHandler?.Invoke(message.Address, message.Payload, ref handlerException, ref handlerForward);
                result = this.Bus.RaiseProcessingException(message, result, ref eventException, ref eventForward);

                exception = eventException ?? handlerException;
                forwardException = eventForward || handlerForward;

                if ((!forwardException) && (exception != null))
                {
                    throw new BusMessageProcessingException(exception);
                }

                this.ResponseHandler(message, result, exception);

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
                        x.Results.Add(messageItem.Payload);
                        x.Exception = messageItem.Exception;
                        if (x.Exception != null)
                        {
                            this.Log(LogLevel.Debug, "Send operation failed with forwarded exception: {0}{1}{2}", x, Environment.NewLine, MessageItem.CreateExceptionMessage(x.Exception, true));
                            x.State = SendOperationItemState.ForwardedException;
                            Exception realException = x.Exception as Exception;
                            if (realException != null)
                            {
                                x.Task.TrySetException(new BusMessageProcessingException(x.Request, realException));
                            }
                            else
                            {
                                x.Task.TrySetException(new BusMessageProcessingException(x.Request, x.Exception));
                            }
                        }
                        else
                        {
                            //TODO: Handle fire-and-forget
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
                            Exception exception = null;
                            Task<object> task = null;
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
            response.IsBroadcast = false;
            response.Id = Guid.NewGuid();
            response.Sent = DateTime.UtcNow;
            response.FromGlobal = false;
            response.ResponseTo = message.Id;
            response.ExceptionForwarding = false;
            response.Exception = exception;
            response.RoutingInfo = message.RoutingInfo;

            this.Log(LogLevel.Debug, "Finished message processing: Request=[{0}], Response=[{1}]", message, response);

            this.Bus.RaiseSendingResponse(response);

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
            List<MessageItem> newMessages = new List<MessageItem>();

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
                    x.Request.ToGlobal = x.SendOperation.Global.GetValueOrDefault(this.Bus.DefaultIsGlobal);
                    
                    //TODO: Handle fire-and-forget
                    x.Request.Timeout = (int)x.SendOperation.Timeout.GetValueOrDefault(x.SendOperation.OperationType == SendOperationType.Broadcast ? this.Bus.CollectionTimeout : this.Bus.ResponseTimeout).TotalMilliseconds;

                    //TODO: Handle fire-and-forget
                    x.Request.IsBroadcast = x.SendOperation.OperationType == SendOperationType.Broadcast;
                    x.Request.ExceptionForwarding = x.SendOperation.ExceptionForwarding.GetValueOrDefault(this.Bus.DefaultExceptionForwarding);
                    x.Request.Id = Guid.NewGuid();
                    x.Request.Sent = utcNow;
                    x.Request.FromGlobal = false;
                    x.Request.ResponseTo = null;
                    x.Request.RoutingInfo = null;
                    newMessages.Add(x.Request);
                    this.Log(LogLevel.Debug, "Send operation created: {0}", x);
                });

                this.Bus.SendOperations.Where(x => x.State == SendOperationItemState.Waiting).ForEach(x =>
                {
                    TimeSpan duration = utcNow.Subtract(x.Request.Sent);
                    if (duration.TotalMilliseconds > x.Request.Timeout)
                    {
                        if (!x.Request.IsBroadcast)
                        {
                            this.Log(LogLevel.Debug, "Send operation timed-out after {0} ms: {1}", x.Request.Timeout, x);
                            x.State = SendOperationItemState.TimedOut;
                            x.Task.TrySetException(new BusResponseTimeoutException(x.Request));
                        }
                        else
                        {
                            this.Log(LogLevel.Debug, "Send operation finished collection after {0} ms: {1}", x.Request.Timeout, x);
                            x.State = SendOperationItemState.Finished;
                            x.Task.TrySetResult(x.Results);
                        }
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
                            this.Log(LogLevel.Debug, "Broken connection: {0}", newBrokenConnection);
                            this.Bus.RaiseConnectionBroken(newBrokenConnection);
                        }

                        if (brokenConnections.Count > 0)
                        {
                            this.Bus.SendOperations.Where(x => x.Request.ToGlobal && (x.State == SendOperationItemState.Waiting) && (!x.SendOperation.IgnoreBrokenConnections)).ForEach(x =>
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


            if (((localResponses.Count == 0)) && (newMessages.Count == 0) && (receivedMessages.Count == 0) && (removedMessages == 0))
            {
                return;
            }

            newMessages.ForEach(x => this.Bus.RaiseSendingRequest(x));

            receivedMessages.ForEach(x => this.Router.ReceivedFromRemote(x.Item1, x.Item2));
            newMessages.ForEach(x => this.Router.ReceivedFromLocal(x));

            receivedMessages.ForEach(x => this.ProcessMessage(x.Item1));
            localResponses.ForEach(this.ProcessMessage);
            newMessages.ForEach(this.ProcessMessage);

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
