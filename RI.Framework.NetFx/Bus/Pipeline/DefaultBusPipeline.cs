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
using RI.Framework.Composition.Model;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bus.Pipeline
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
		}

		#endregion




		#region Instance Properties/Indexer

		private IBus Bus { get; set; }
		private IBusConnectionManager ConnectionManager { get; set; }
		private IBusDispatcher Dispatcher { get; set; }
		private Queue<MessageItem> LocalResponses { get; set; }
		private IBusRouter Router { get; set; }

		#endregion




		#region Instance Methods

		private void ProcessMessage (MessageItem messageItem)
		{
			if (messageItem.ResponseTo.HasValue)
			{
				lock (this.Bus.SyncRoot)
				{
					this.Bus.SendOperations.Where(x => (x.Request.Id == messageItem.ResponseTo.Value) && (x.State == SendOperationItemState.Waiting)).ForEach(x =>
					{
						x.Responses.Add(messageItem);
						x.Results.Add(messageItem.Payload);
						x.Exception = messageItem.Exception;
						if (x.Exception != null)
						{
							this.Log(LogLevel.Debug, "Send operation failed with forwarded exception: {0}{1}{2}", x, Environment.NewLine, x.Exception.ToDetailedString());
							x.State = SendOperationItemState.ForwardedException;
							x.Task.TrySetException(new BusMessageProcessingException(x.Exception));
						}
						else
						{
							if (x.SendOperation.IsBroadcast)
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

			bool toLocal = this.Router.ForwardToLocal(messageItem);
			bool toGlobal = this.Router.ForwardToGlobal(messageItem);

			if (toLocal)
			{
				lock (this.Bus.SyncRoot)
				{
					this.Bus.ReceiveRegistrations.Where(x => this.Router.ShouldReceive(messageItem, x)).ForEach(x =>
					{
						this.Log(LogLevel.Debug, "Dispatching message processing: Request=[{0}], Receiver=[{1}]", messageItem, x);
						this.Dispatcher.Dispatch(new Action<MessageItem, ReceiverRegistrationItem>((m, r) =>
						{
							this.Log(LogLevel.Debug, "Executing message processing: Request=[{0}], Receiver=[{1}]", m, r);
							bool exceptionForwarding1 = r.ReceiverRegistration.ExceptionForwarding.GetValueOrDefault(this.Bus.DefaultExceptionForwarding) || m.ExceptionForwarding;
							ReceiverExceptionHandler exceptionHandler1 = r.ReceiverRegistration.ExceptionHandler;
							bool catchException1 = exceptionForwarding1 || (exceptionHandler1 != null);
							Func<string, object, Task<object>> callback = r.ReceiverRegistration.Callback;
							Exception exception;
							Task<object> task;
							if (!catchException1)
							{
								exception = null;
								task = callback(m.Address, m.Payload);
							}
							else
							{
								try
								{
									exception = null;
									task = callback(m.Address, m.Payload);
								}
								catch (Exception ex)
								{
									exception = ex;
									task = null;
								}
							}
							if (exception != null)
							{
								object result = exceptionHandler1?.Invoke(m.Address, m.Payload, exception, ref exceptionForwarding1);
								task = Task.FromResult(result);
							}
							if (task.IsCompleted)
							{
								this.ResponseHandler(m, task.Result, exceptionForwarding1 ? exception : null);
							}
							else
							{
								task.ContinueWith((c1, s1) =>
								{
									this.Dispatcher.Dispatch(new Action<Task<object>, Tuple<MessageItem, ReceiverRegistrationItem>>((c2, s2) =>
									{
										bool exceptionForwarding2 = s2.Item2.ReceiverRegistration.ExceptionForwarding.GetValueOrDefault(this.Bus.DefaultExceptionForwarding) || s2.Item1.ExceptionForwarding;
										ReceiverExceptionHandler exceptionHandler2 = s2.Item2.ReceiverRegistration.ExceptionHandler;
										bool catchException2 = exceptionForwarding2 || (exceptionHandler2 != null);
										object result;
										if ((c2.Exception != null) && catchException2)
										{
											result = exceptionHandler2?.Invoke(s2.Item1.Address, s2.Item1.Payload, c2.Exception, ref exceptionForwarding2);
										}
										else
										{
											result = c2.Result;
										}
										this.ResponseHandler(s2.Item1, result, exceptionForwarding2 ? c2.Exception : null);
									}), c1, s1);
								}, new Tuple<MessageItem, ReceiverRegistrationItem>(m, r), CancellationToken.None, TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.RunContinuationsAsynchronously, TaskScheduler.Current);
							}
						}), messageItem, x);
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
					x.Request.Timeout = (int)x.SendOperation.Timeout.GetValueOrDefault(x.SendOperation.IsBroadcast ? this.Bus.CollectionTimeout : this.Bus.ResponseTimeout).TotalMilliseconds;
					x.Request.IsBroadcast = x.SendOperation.IsBroadcast;
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
						brokenConnections.ForEach(x => this.Log(LogLevel.Warning, "Broken connection: {0}", x));
						if (brokenConnections.Count > 0)
						{
							this.Bus.SendOperations.Where(x => x.Request.ToGlobal && (x.State == SendOperationItemState.Waiting)).ForEach(x =>
							{
								this.Log(LogLevel.Debug, "Send operation failed with broken connection: {0}", x);
								x.State = SendOperationItemState.Broken;
								x.Task.TrySetException(new BusConnectionBrokenException(brokenConnections[0]));
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
			}
		}

		#endregion
	}
}
