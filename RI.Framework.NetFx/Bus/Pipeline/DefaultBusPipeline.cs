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
	public sealed class DefaultBusPipeline : IBusPipeline
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

		/// <inheritdoc />
		public void StartProcessing ()
		{
		}

		/// <inheritdoc />
		public void StopProcessing ()
		{
		}

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
						if (x.SendOperation.IsBroadcast)
						{
							if (x.SendOperation.ExpectedResults.HasValue && (x.Results.Count >= x.SendOperation.ExpectedResults.Value))
							{
								x.State = SendOperationItemState.Finished;
								x.Task.TrySetResult(x.Results);
							}
						}
						else
						{
							x.State = SendOperationItemState.Finished;
							x.Task.TrySetResult(x.Results[0]);
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
						this.Dispatcher.Dispatch(new Action<MessageItem, ReceiverRegistrationItem>((m, r) =>
						{
							Func<string, object, Task<object>> callback = r.ReceiverRegistration.Callback;
							Task<object> task;
							if (r.ReceiverRegistration.ExceptionHandler == null)
							{
								task = callback(m.Address, m.Payload);
							}
							else
							{
								try
								{
									task = callback(m.Address, m.Payload);
								}
								catch (Exception exception)
								{
									object result = r.ReceiverRegistration.ExceptionHandler(m.Address, m.Payload, exception);
									task = Task.FromResult(result);
								}
							}
							if (task.IsCompleted)
							{
								this.ResponseHandler(m, task.Result);
							}
							else
							{
								task.ContinueWith((c1, s1) =>
								{
									this.Dispatcher.Dispatch(new Action<MessageItem, Task<object>>((s2, c2) =>
									{
										object result;
										if ((c2.Exception != null) && (r.ReceiverRegistration.ExceptionHandler != null))
										{
											result = r.ReceiverRegistration.ExceptionHandler(m.Address, m.Payload, c2.Exception);
										}
										else
										{
											result = c2.Result;
										}
										this.ResponseHandler(s2, result);
									}), s1, c1);
								}, m, CancellationToken.None, TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.RunContinuationsAsynchronously, TaskScheduler.Current);
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

		private void ResponseHandler (MessageItem message, object result)
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
		public object SyncRoot { get; set; }

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
					x.Request.Id = Guid.NewGuid();
					x.Request.Sent = utcNow;
					x.Request.FromGlobal = false;
					x.Request.ResponseTo = null;
					newMessages.Add(x.Request);
				});

				this.Bus.SendOperations.Where(x => x.State == SendOperationItemState.Waiting).ForEach(x =>
				{
					TimeSpan duration = utcNow.Subtract(x.Request.Sent);
					if (duration.TotalMilliseconds > x.Request.Timeout)
					{
						if (!x.Request.IsBroadcast)
						{
							x.State = SendOperationItemState.TimedOut;
							x.Task.TrySetException(new BusResponseTimeoutException(x.Request));
						}
						else
						{
							x.State = SendOperationItemState.Finished;
							x.Task.TrySetResult(x.Results);
						}
					}
				});

				if (this.ConnectionManager != null)
				{
					lock (this.ConnectionManager.SyncRoot)
					{
						IBusConnection brokenConnection = this.ConnectionManager.Connections.FirstOrDefault(x => x.IsBroken);
						if (brokenConnection != null)
						{
							this.Bus.SendOperations.Where(x => x.Request.ToGlobal && (x.State == SendOperationItemState.Waiting)).ForEach(x =>
							{
								x.State = SendOperationItemState.Broken;
								x.Task.TrySetException(new BusConnectionBrokenException(brokenConnection));
							});
						}
					}
				}

				removedMessages = this.Bus.SendOperations.RemoveAll(x => (x.State == SendOperationItemState.Finished) || (x.State == SendOperationItemState.TimedOut) || (x.State == SendOperationItemState.Cancelled) || (x.State == SendOperationItemState.Broken));
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
