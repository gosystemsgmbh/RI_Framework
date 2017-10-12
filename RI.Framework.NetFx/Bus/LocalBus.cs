using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Bus.Connections;
using RI.Framework.Bus.Exceptions;
using RI.Framework.Bus.Internals;
using RI.Framework.Bus.Pipeline;
using RI.Framework.Bus.Routers;
using RI.Framework.Bus.Serializers;
using RI.Framework.Threading;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Bus
{
	/// <summary>
	/// Implements a local message bus.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The &quot;Local&quot; in <see cref="LocalBus"/> stands for the in-process locality of the message bus.
	/// Basically, each process (or application domain, if you distinguish those) has its own local message bus, distributing the messages to all receivers within the same process.
	/// However, multiple local message busses can be connected, across processes, machines, or networks, to form a global message bus which consists of independent but connected local message busses.
	/// </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	/// TODO: Logging
	/// TODO: Documentation
	public sealed class LocalBus : LogSource, IBus
	{
		private TimeSpan _pollInterval;
		private TimeSpan _responseTimeout;
		private TimeSpan _collectionTimeout;
		private bool _defaultIsGlobal;

		/// <inheritdoc />
		public TimeSpan ResponseTimeout
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._responseTimeout;
				}
			}
			set
			{
				if (value.IsNegative())
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				lock (this.SyncRoot)
				{
					this._responseTimeout = value;
				}
			}
		}

		/// <inheritdoc />
		public TimeSpan CollectionTimeout
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._collectionTimeout;
				}
			}
			set
			{
				if (value.IsNegative())
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				lock (this.SyncRoot)
				{
					this._collectionTimeout = value;
				}
			}
		}

		/// <inheritdoc />
		public bool DefaultIsGlobal
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._defaultIsGlobal;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._defaultIsGlobal = value;
				}
			}
		}

		/// <inheritdoc />
		public TimeSpan PollInterval
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._pollInterval;
				}
			}
			set
			{
				if (value.IsNegative())
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				lock (this.SyncRoot)
				{
					this._pollInterval = value;
				}
			}
		}

		/// <inheritdoc />
		public object SyncRoot { get; }

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		private object StartStopSyncRoot { get; set; }

		/// <inheritdoc />
		List<SendOperationItem> IBus.SendOperations
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.SendOperations;
				}
			}
		}

		/// <inheritdoc />
		List<ReceiveRegistrationItem> IBus.ReceiveRegistrations
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.ReceiveRegistrations;
				}
			}
		}

		private List<SendOperationItem> SendOperations { get; set; }

		private List<ReceiveRegistrationItem> ReceiveRegistrations { get; set; }

		private ManualResetEvent WorkAvailable { get; set; }

		private WorkThread WorkerThread { get; set; }

		private IDependencyResolver DependencyResolver { get; set; }

		/// <summary>
		/// Creates a new instance of <see cref="LocalBus"/>.
		/// </summary>
		public LocalBus ()
		{
			this.SyncRoot = new object();
			this.StartStopSyncRoot = new object();

			this.ResponseTimeout = TimeSpan.FromSeconds(10);
			this.CollectionTimeout = TimeSpan.FromSeconds(10);
			this.PollInterval = TimeSpan.FromMilliseconds(20);
			this.DefaultIsGlobal = false;

			this.DependencyResolver = null;
			this.WorkerThread = null;
			this.WorkAvailable = null;
			this.SendOperations = new List<SendOperationItem>();
			this.ReceiveRegistrations = new List<ReceiveRegistrationItem>();
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="LocalBus" />.
		/// </summary>
		~LocalBus()
		{
			this.Dispose(false);
		}

		/// <inheritdoc />
		public void Stop ()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <inheritdoc />
		void IDisposable.Dispose()
		{
			this.Stop();
		}

		/// <inheritdoc />
		public void Start (IDependencyResolver dependencyResolver)
		{
			if (dependencyResolver == null)
			{
				throw new ArgumentNullException(nameof(dependencyResolver));
			}

			lock (this.StartStopSyncRoot)
			{
				bool success = false;
				try
				{
					lock (this.SyncRoot)
					{
						this.VerifyNotStarted();

						this.DependencyResolver = dependencyResolver;

						this.SendOperations.Clear();
						this.ReceiveRegistrations.Clear();

						this.WorkAvailable = new ManualResetEvent(false);

						this.WorkerThread = new WorkThread(this);
						this.WorkerThread.Timeout += (int)this.ResponseTimeout.TotalMilliseconds + (int)this.PollInterval.TotalMilliseconds;
					}

					this.WorkerThread.Start();

					success = true;
				}
				finally
				{
					if (!success)
					{
						this.Stop();
					}
				}
			}
		}

		[SuppressMessage("ReSharper", "UnusedParameter.Local")]
		private void Dispose(bool disposing)
		{
			lock (this.StartStopSyncRoot)
			{
				WorkThread workerThread;
				lock (this.SyncRoot)
				{
					workerThread = this.WorkerThread;
				}

				workerThread?.Stop();

				lock (this.SyncRoot)
				{
					this.WorkerThread = null;

					this.WorkAvailable?.Close();
					this.WorkAvailable = null;

					this.SendOperations?.Clear();
					this.ReceiveRegistrations?.Clear();

					this.DependencyResolver = null;
				}
			}
		}

		private void VerifyStarted()
		{
			if (!this.IsStarted)
			{
				throw new InvalidOperationException("The local message bus is not started.");
			}
		}

		private void VerifyNotStarted()
		{
			if (this.IsStarted)
			{
				throw new InvalidOperationException("The local message bus is already started.");
			}
		}

		private void SignalWorkAvailable ()
		{
			lock (this.SyncRoot)
			{
				this.WorkAvailable.Set();
			}
		}

		private void CheckForExceptions()
		{
			if (this.WorkerThread.ThreadException != null)
			{
				throw new LocalBusException(this.WorkerThread.ThreadException);
			}
		}

		/// <inheritdoc />
		Task<object> IBus.Enqueue(SendOperation sendOperation)
		{
			if (sendOperation == null)
			{
				throw new ArgumentNullException(nameof(sendOperation));
			}

			lock (this.SyncRoot)
			{
				this.VerifyStarted();
				this.CheckForExceptions();

				SendOperationItem item = new SendOperationItem(sendOperation);

				this.SendOperations.Add(item);
				this.SignalWorkAvailable();

				return item.Task.Task;
			}
		}

		/// <inheritdoc />
		void IBus.Register(ReceiverRegistration receiverRegistration)
		{
			if (receiverRegistration == null)
			{
				throw new ArgumentNullException(nameof(receiverRegistration));
			}

			lock (this.SyncRoot)
			{
				this.VerifyStarted();
				this.CheckForExceptions();

				ReceiveRegistrationItem item = new ReceiveRegistrationItem(receiverRegistration);
				this.ReceiveRegistrations.Add(item);
			}
		}

		/// <inheritdoc />
		void IBus.Unregister(ReceiverRegistration receiverRegistration)
		{
			if (receiverRegistration == null)
			{
				throw new ArgumentNullException(nameof(receiverRegistration));
			}

			lock (this.SyncRoot)
			{
				this.ReceiveRegistrations.RemoveAll(x => object.ReferenceEquals(x.ReceiverRegistration, receiverRegistration));
			}
		}

		/// <inheritdoc />
		public SendOperation Send ()
		{
			lock (this.SyncRoot)
			{
				this.VerifyStarted();
				this.CheckForExceptions();
				return new SendOperation(this);
			}
		}

		/// <inheritdoc />
		public ReceiverRegistration Receive ()
		{
			lock (this.SyncRoot)
			{
				this.VerifyStarted();
				this.CheckForExceptions();
				return new ReceiverRegistration(this);
			}
		}

		/// <inheritdoc />
		public bool IsStarted
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.WorkerThread != null;
				}
			}
		}

		private sealed class WorkThread : HeavyThread
		{
			public WorkThread (LocalBus localBus)
			{
				this.LocalBus = localBus;
			}

			public LocalBus LocalBus { get; }

			private IDependencyResolver DependencyResolver => this.LocalBus.DependencyResolver;

			private IBusPipeline Pipeline { get; set; }

			private IBusDispatcher Dispatcher { get; set; }

			private IBusRouter Router { get; set; }

			private IBusSerializer Serializer { get; set; }

			private IBusConnectionManager ConnectionManager { get; set; }

			private List<IBusConnection> Connections { get; set; }

			private T ResolveSingle<T> (bool mandatory)
				where T : class
			{
				//TODO: Exception on multiple
				T instance = this.DependencyResolver.GetInstance<T>();
				if ((instance == null) && mandatory)
				{
					throw new Exception("Could not resolve a mandatory single instance of " + typeof(T).Name + ".");
				}
				return instance;
			}

			private List<T> ResolveMultiple<T> (bool mandatory)
				where T : class
			{
				List<T> instances = this.DependencyResolver.GetInstances<T>();
				if ((instances.Count == 0) && mandatory)
				{
					throw new Exception("Could not resolve mandatory multiple instances of " + typeof(T).Name + ".");
				}
				return instances;
			}

			protected override void OnException (Exception exception, bool canContinue)
			{
				lock (this.SyncRoot)
				{
					lock (this.LocalBus.SyncRoot)
					{
						foreach (SendOperationItem item in this.LocalBus.SendOperations)
						{
							item.Task.TrySetException(new LocalBusException(exception));
						}

						this.LocalBus.SendOperations.Clear();
					}
				}

				base.OnException(exception, canContinue);
			}

			protected override void OnStarting ()
			{
				base.OnStarting();

				//TODO: thread configuration
			}

			protected override void OnBegin ()
			{
				base.OnBegin();

				this.Pipeline = this.ResolveSingle<IBusPipeline>(true);
				this.Dispatcher = this.ResolveSingle<IBusDispatcher>(true);
				this.Router = this.ResolveSingle<IBusRouter>(true);

				this.Serializer = this.ResolveSingle<IBusSerializer>(false);
				this.ConnectionManager = this.ResolveSingle<IBusConnectionManager>(false);
				this.Connections = this.ResolveMultiple<IBusConnection>(false);

				this.Pipeline.Initialize(this.DependencyResolver);
				this.Dispatcher.Initialize(this.DependencyResolver);
				this.Router.Initialize(this.DependencyResolver);

				this.Serializer?.Initialize(this.DependencyResolver);
				this.ConnectionManager?.Initialize(this.DependencyResolver);
				this.Connections.ForEach(x => x.Initialize(this.DependencyResolver));
			}

			protected override void OnEnd ()
			{
				this.Connections?.ForEach(x => x.Unload());
				this.ConnectionManager?.Unload();
				this.Serializer?.Unload();

				this.Router?.Unload();
				this.Dispatcher?.Unload();
				this.Pipeline?.Unload();

				base.OnEnd();
			}

			protected override void OnRun ()
			{
				base.OnRun();

				lock (this.LocalBus.SyncRoot)
				{
					this.Pipeline.StartProcessing(this.LocalBus.SignalWorkAvailable);
				}

				while (true)
				{
					WaitHandle.WaitAny(new WaitHandle[]
					{
						this.LocalBus.WorkAvailable,
						this.StopEvent
					}, this.LocalBus.PollInterval);

					lock (this.LocalBus.SyncRoot)
					{
						this.LocalBus.WorkAvailable.Reset();
					}

					if (this.StopRequested)
					{
						break;
					}

					this.Pipeline.DoWork();
				}

				lock (this.LocalBus.SyncRoot)
				{
					this.Pipeline.StopProcessing();
				}
			}
		}
	}
}
