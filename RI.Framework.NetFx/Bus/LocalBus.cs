using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Bus.Connections;
using RI.Framework.Bus.Dispatchers;
using RI.Framework.Bus.Exceptions;
using RI.Framework.Bus.Internals;
using RI.Framework.Bus.Pipeline;
using RI.Framework.Bus.Routers;
using RI.Framework.Collections;
using RI.Framework.Composition.Model;
using RI.Framework.Threading;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bus
{
	/// <summary>
	///     Implements a local bus which can use optional connections to remote busses.
	/// </summary>
	/// <remarks>
	///     See <see cref="IBus" /> for more details.
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public sealed class LocalBus : IBus
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="LocalBus" />.
		/// </summary>
		public LocalBus ()
		{
			this.SyncRoot = new object();
			this.StartStopSyncRoot = new object();

			this.ResponseTimeout = TimeSpan.FromSeconds(10);
			this.CollectionTimeout = TimeSpan.FromSeconds(10);
			this.PollInterval = TimeSpan.FromMilliseconds(20);
			this.DefaultIsGlobal = false;
			this.DefaultExceptionForwarding = true;

			this.ThreadTimeout = TimeSpan.FromMilliseconds(HeavyThread.DefaultThreadTimeout);
			this.ThreadPriority = Thread.CurrentThread.Priority;
			this.ThreadCulture = Thread.CurrentThread.CurrentCulture;
			this.ThreadUICulture = Thread.CurrentThread.CurrentUICulture;

			this.SendOperations = new List<SendOperationItem>();
			this.ReceiveRegistrations = new List<ReceiverRegistrationItem>();

			this.DependencyResolver = null;
			this.WorkerThread = null;
			this.WorkAvailable = null;
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="LocalBus" />.
		/// </summary>
		~LocalBus ()
		{
			this.Dispose(false);
		}

		#endregion




		#region Instance Fields

		private TimeSpan _collectionTimeout;
		private bool _defaultExceptionForwarding;
		private bool _defaultIsGlobal;
		private ILogger _logger;
		private bool _loggingEnabled;
		private TimeSpan _pollInterval;
		private TimeSpan _responseTimeout;
		private CultureInfo _threadCulture;
		private ThreadPriority _threadPriority;
		private TimeSpan _threadTimeout;
		private CultureInfo _threadUICulture;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets the formatting culture of the bus processing thread.
		/// </summary>
		/// <value>
		///     The formatting culture of the bus processing thread.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is the same formatting culture as the formatting culture of the thread which created this instance of <see cref="LocalBus" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="value" /> is null. </exception>
		public CultureInfo ThreadCulture
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._threadCulture;
				}
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				lock (this.SyncRoot)
				{
					this._threadCulture = value;

					if (this.WorkerThread != null)
					{
						this.WorkerThread.Thread.CurrentCulture = value;
					}
				}
			}
		}

		/// <summary>
		///     Gets or sets the priority of the bus processing thread.
		/// </summary>
		/// <value>
		///     The priority of the bus processing thread.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is the same priority as the priority of the thread which created this instance of <see cref="LocalBus" />.
		///     </para>
		/// </remarks>
		public ThreadPriority ThreadPriority
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._threadPriority;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._threadPriority = value;

					if (this.WorkerThread != null)
					{
						this.WorkerThread.Thread.Priority = value;
					}
				}
			}
		}

		/// <summary>
		///     Gets or sets the timeout of the bus processing thread used for start and stop.
		/// </summary>
		/// <value>
		///     The timeout of the bus processing thread used for start and stop.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is <see cref="HeavyThread.DefaultThreadTimeout" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="value" /> is negative. </exception>
		public TimeSpan ThreadTimeout
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._threadTimeout;
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
					this._threadTimeout = value;

					if (this.WorkerThread != null)
					{
						this.WorkerThread.Timeout = (int)value.TotalMilliseconds;
					}
				}
			}
		}

		/// <summary>
		///     Gets or sets the UI culture of the bus processing thread.
		/// </summary>
		/// <value>
		///     The UI culture of the bus processing thread.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is the same UI culture as the UI culture of the thread which created this instance of <see cref="LocalBus" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="value" /> is null. </exception>
		public CultureInfo ThreadUICulture
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._threadUICulture;
				}
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				lock (this.SyncRoot)
				{
					this._threadUICulture = value;

					if (this.WorkerThread != null)
					{
						this.WorkerThread.Thread.CurrentUICulture = value;
					}
				}
			}
		}

		private IDependencyResolver DependencyResolver { get; set; }

		private List<ReceiverRegistrationItem> ReceiveRegistrations { get; }

		private List<SendOperationItem> SendOperations { get; }

		private object StartStopSyncRoot { get; }

		private ManualResetEvent WorkAvailable { get; set; }

		private WorkThread WorkerThread { get; set; }

		#endregion




		#region Instance Methods

		private void CheckForExceptions ()
		{
			Exception exception = this.WorkerThread?.ThreadException;
			if (exception != null)
			{
				throw new BusProcessingPipelineException(exception);
			}
		}

		[SuppressMessage("ReSharper", "UnusedParameter.Local")]
		private void Dispose (bool disposing)
		{
			lock (this.StartStopSyncRoot)
			{
				this.Log(LogLevel.Information, "Stopping message bus");

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

					this.DependencyResolver = null;
				}

				this.Log(LogLevel.Information, "Message bus stopped");
			}
		}

		private void InheritLogger ()
		{
			if (this.WorkerThread != null)
			{
				this.WorkerThread.Logger = this.Logger;
				this.WorkerThread.LoggingEnabled = this.LoggingEnabled;
			}
		}

		private void SignalWorkAvailable ()
		{
			this.WorkAvailable?.Set();
		}

		private void VerifyNotStarted ()
		{
			if (this.IsStarted)
			{
				throw new InvalidOperationException("The local message bus is already started.");
			}
		}

		private void VerifyStarted ()
		{
			if (!this.IsStarted)
			{
				throw new InvalidOperationException("The local message bus is not started.");
			}
		}

		#endregion




		#region Interface: IBus

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
		public bool DefaultExceptionForwarding
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._defaultExceptionForwarding;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._defaultExceptionForwarding = value;
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

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public ILogger Logger
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._logger;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._logger = value;
					this.InheritLogger();
				}
			}
		}

		/// <inheritdoc />
		public bool LoggingEnabled
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._loggingEnabled;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._loggingEnabled = value;
					this.InheritLogger();
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
		List<ReceiverRegistrationItem> IBus.ReceiveRegistrations
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.ReceiveRegistrations;
				}
			}
		}

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
		public object SyncRoot { get; }

		/// <inheritdoc />
		void IDisposable.Dispose ()
		{
			this.Stop();
		}

		/// <inheritdoc />
		Task<object> IBus.Enqueue (SendOperation sendOperation)
		{
			if (sendOperation == null)
			{
				throw new ArgumentNullException(nameof(sendOperation));
			}

			if (!sendOperation.IsProcessed)
			{
				throw new ArgumentException("The send operation was not properly configured.", nameof(sendOperation));
			}

			lock (this.SyncRoot)
			{
				this.VerifyStarted();
				this.CheckForExceptions();

				this.Log(LogLevel.Debug, "Sending: {0}", sendOperation);

				SendOperationItem item = new SendOperationItem(sendOperation);
				this.SendOperations.Add(item);

				this.SignalWorkAvailable();

				return item.Task.Task;
			}
		}

		/// <inheritdoc />
		void IBus.Register (ReceiverRegistration receiverRegistration)
		{
			if (receiverRegistration == null)
			{
				throw new ArgumentNullException(nameof(receiverRegistration));
			}

			if (!receiverRegistration.IsProcessed)
			{
				throw new ArgumentException("The receiver registration was not properly configured.", nameof(receiverRegistration));
			}

			lock (this.SyncRoot)
			{
				this.Log(LogLevel.Debug, "Registering receiver: {0}", receiverRegistration);

				ReceiverRegistrationItem item = new ReceiverRegistrationItem(receiverRegistration);
				this.ReceiveRegistrations.Add(item);

				this.SignalWorkAvailable();
			}
		}

		/// <inheritdoc />
		void IBus.SignalWorkAvailable ()
		{
			lock (this.SyncRoot)
			{
				this.VerifyStarted();

				this.WorkAvailable.Set();
			}
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
				lock (this.SyncRoot)
				{
					this.VerifyNotStarted();
				}

				this.Log(LogLevel.Information, "Starting message bus");

				bool success = false;
				try
				{
					lock (this.SyncRoot)
					{
						this.DependencyResolver = dependencyResolver;

						this.WorkAvailable = new ManualResetEvent(false);

						this.WorkerThread = new WorkThread(this);

						this.InheritLogger();
					}

					this.WorkerThread.Start();

					this.Log(LogLevel.Information, "Message bus started");

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

		/// <inheritdoc />
		public void Stop ()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <inheritdoc />
		void IBus.Unregister (ReceiverRegistration receiverRegistration)
		{
			if (receiverRegistration == null)
			{
				throw new ArgumentNullException(nameof(receiverRegistration));
			}

			lock (this.SyncRoot)
			{
				this.Log(LogLevel.Debug, "Unregistering receiver: {0}", receiverRegistration);

				this.ReceiveRegistrations.RemoveAll(x => object.ReferenceEquals(x.ReceiverRegistration, receiverRegistration));

				this.SignalWorkAvailable();
			}
		}

		#endregion




		#region Type: WorkThread

		private sealed class WorkThread : HeavyThread, ILogSource
		{
			#region Instance Constructor/Destructor

			public WorkThread (LocalBus localBus)
			{
				this.LocalBus = localBus;
			}

			#endregion




			#region Instance Fields

			private ILogger _logger;
			private bool _loggingEnabled;

			#endregion




			#region Instance Properties/Indexer

			public LocalBus LocalBus { get; }

			public new Thread Thread => base.Thread;

			private IBusConnectionManager ConnectionManager { get; set; }

			private List<IBusConnection> Connections { get; set; }

			private IDependencyResolver DependencyResolver => this.LocalBus.DependencyResolver;

			private IBusDispatcher Dispatcher { get; set; }

			[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
			private IEnumerable<ILogSource> LogSources
			{
				get
				{
					if (this.Pipeline is ILogSource)
					{
						yield return this.Pipeline as ILogSource;
					}

					if (this.Dispatcher is ILogSource)
					{
						yield return this.Dispatcher as ILogSource;
					}

					if (this.Router is ILogSource)
					{
						yield return this.Router as ILogSource;
					}

					if (this.ConnectionManager is ILogSource)
					{
						yield return this.ConnectionManager as ILogSource;
					}

					foreach (IBusConnection connection in this.Connections)
					{
						if (connection is ILogSource)
						{
							yield return connection as ILogSource;
						}
					}
				}
			}

			private IBusPipeline Pipeline { get; set; }

			private IBusRouter Router { get; set; }

			#endregion




			#region Instance Methods

			private void InheritLogger ()
			{
				this.LogSources.ForEach(x =>
				{
					x.Logger = this.Logger;
					x.LoggingEnabled = this.LoggingEnabled;
				});
			}

			private List<T> ResolveMultiple <T> (bool mandatory)
				where T : class
			{
				List<T> instances = this.DependencyResolver.GetInstances<T>();
				if ((instances.Count == 0) && mandatory)
				{
					throw new Exception("Could not resolve mandatory multiple instances of " + typeof(T).Name + ".");
				}
				return instances;
			}

			private T ResolveSingle <T> (bool mandatory)
				where T : class
			{
				List<T> instances = this.DependencyResolver.GetInstances<T>();
				if ((instances.Count == 0) && mandatory)
				{
					throw new Exception("Could not resolve a mandatory single instance of " + typeof(T).Name + ".");
				}
				if (instances.Count > 1)
				{
					throw new Exception("Too many resolved instances of " + typeof(T).Name + ".");
				}
				return instances[0];
			}

			#endregion




			#region Overrides

			protected override void OnBegin ()
			{
				base.OnBegin();

				this.Pipeline = this.ResolveSingle<IBusPipeline>(true);
				this.Dispatcher = this.ResolveSingle<IBusDispatcher>(true);
				this.Router = this.ResolveSingle<IBusRouter>(true);
				this.ConnectionManager = this.ResolveSingle<IBusConnectionManager>(false);
				this.Connections = this.ResolveMultiple<IBusConnection>(false);

				this.Log(LogLevel.Information, "Used bus pipeline:           {0}", this.Pipeline);
				this.Log(LogLevel.Information, "Used bus dispatcher:         {0}", this.Dispatcher);
				this.Log(LogLevel.Information, "Used bus router:             {0}", this.Router);
				this.Log(LogLevel.Information, "Used bus connection manager: {0}", this.ConnectionManager?.ToString() ?? "[null]");
				this.Connections.ForEach(x => this.Log(LogLevel.Information, "Used bus connection:         {0}", x));

				this.InheritLogger();

				this.Connections.ForEach(x =>
				{
					this.Log(LogLevel.Information, "Initialize bus connection: {0}", x);
					x.Initialize(this.DependencyResolver);
				});

				if (this.ConnectionManager != null)
				{
					this.Log(LogLevel.Information, "Initialize bus connection manager: {0}", this.ConnectionManager);
					this.ConnectionManager.Initialize(this.DependencyResolver);
				}

				this.Log(LogLevel.Information, "Initialize bus router: {0}", this.Router);
				this.Router.Initialize(this.DependencyResolver);

				this.Log(LogLevel.Information, "Initialize bus dispatcher: {0}", this.Dispatcher);
				this.Dispatcher.Initialize(this.DependencyResolver);

				this.Log(LogLevel.Information, "Initialize bus pipeline: {0}", this.Pipeline);
				this.Pipeline.Initialize(this.DependencyResolver);
			}

			protected override void OnEnd ()
			{
				if (this.Pipeline != null)
				{
					this.Log(LogLevel.Information, "Unload bus pipeline: {0}", this.Pipeline);
					this.Pipeline.Unload();
				}

				if (this.Dispatcher != null)
				{
					this.Log(LogLevel.Information, "Unload bus dispatcher: {0}", this.Dispatcher);
					this.Dispatcher.Unload();
				}

				if (this.Router != null)
				{
					this.Log(LogLevel.Information, "Unload bus router: {0}", this.Router);
					this.Router.Unload();
				}

				if (this.ConnectionManager != null)
				{
					this.Log(LogLevel.Information, "Unload bus connection manager: {0}", this.ConnectionManager);
					this.ConnectionManager.Unload();
				}

				this.Connections?.ForEach(x =>
				{
					this.Log(LogLevel.Information, "Unload bus connection: {0}", x);
					x.Unload();
				});

				base.OnEnd();
			}

			protected override void OnException (Exception exception, bool canContinue)
			{
				base.OnException(exception, canContinue);

				this.Log(LogLevel.Error, "Bus processing pipeline exception: {0}{1}", Environment.NewLine, exception.ToDetailedString());

				lock (this.LocalBus.SyncRoot)
				{
					try
					{
						throw new BusProcessingPipelineException(exception);
					}
					catch (BusProcessingPipelineException bpe)
					{
						foreach (SendOperationItem item in this.LocalBus.SendOperations)
						{
							item.Task.TrySetException(bpe);
						}
					}

					this.LocalBus.SendOperations.Clear();
				}
			}

			protected override void OnRun ()
			{
				base.OnRun();

				this.Log(LogLevel.Information, "Bus pipeline is running");

				while (true)
				{
					int response = WaitHandle.WaitAny(new WaitHandle[] {this.LocalBus.WorkAvailable, this.StopEvent}, this.LocalBus.PollInterval);

					lock (this.LocalBus.SyncRoot)
					{
						this.LocalBus.WorkAvailable.Reset();
					}

					if (this.StopRequested)
					{
						this.Log(LogLevel.Information, "Bus pipeline received stop request");
						break;
					}

					this.Pipeline.DoWork(response != 0);
				}
			}

			protected override void OnStarting ()
			{
				base.OnStarting();

				this.Timeout = (int)this.LocalBus.ThreadTimeout.TotalMilliseconds;

				this.Thread.IsBackground = false;
				this.Thread.Name = this.LocalBus.GetType().Name;

				this.Thread.Priority = this.LocalBus.ThreadPriority;
				this.Thread.CurrentCulture = this.LocalBus.ThreadCulture;
				this.Thread.CurrentUICulture = this.LocalBus.ThreadUICulture;
			}

			#endregion




			#region Interface: ILogSource

			/// <inheritdoc />
			public ILogger Logger
			{
				get
				{
					lock (this.LocalBus.SyncRoot)
					{
						return this._logger;
					}
				}
				set
				{
					lock (this.LocalBus.SyncRoot)
					{
						this._logger = value;
						this.InheritLogger();
					}
				}
			}

			/// <inheritdoc />
			public bool LoggingEnabled
			{
				get
				{
					lock (this.LocalBus.SyncRoot)
					{
						return this._loggingEnabled;
					}
				}
				set
				{
					lock (this.LocalBus.SyncRoot)
					{
						this._loggingEnabled = value;
						this.InheritLogger();
					}
				}
			}

			#endregion
		}

		#endregion
	}
}
