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
using RI.Framework.Composition;
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
	/// TODO: Logging
	public sealed class LocalBus : LogSource, IBus
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

			this.ThreadTimeout = TimeSpan.FromMilliseconds(HeavyThread.DefaultThreadTimeout);
			this.ThreadPriority = Thread.CurrentThread.Priority;
			this.ThreadCulture = Thread.CurrentThread.CurrentCulture;
			this.ThreadUICulture = Thread.CurrentThread.CurrentUICulture;

			this.TypeResolver = null;
			this.WorkerThread = null;
			this.WorkAvailable = null;
			this.SendOperations = new List<SendOperationItem>();
			this.ReceiveRegistrations = new List<ReceiverRegistrationItem>();
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
		private bool _defaultIsGlobal;
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

		private List<ReceiverRegistrationItem> ReceiveRegistrations { get; set; }

		private List<SendOperationItem> SendOperations { get; set; }

		private object StartStopSyncRoot { get; set; }

		private TypeInjector TypeResolver { get; set; }

		private WorkSignaler WorkAvailable { get; set; }

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
				WorkThread workerThread;
				lock (this.SyncRoot)
				{
					workerThread = this.WorkerThread;
				}

				workerThread?.Stop();

				lock (this.SyncRoot)
				{
					this.WorkerThread = null;

					this.WorkAvailable?.Dispose();
					this.WorkAvailable = null;

					this.SendOperations?.Clear();
					this.ReceiveRegistrations?.Clear();

					this.TypeResolver = null;
				}
			}
		}

		private void SignalWorkAvailable ()
		{
			this.WorkAvailable?.SignalWorkAvailable();
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
				this.VerifyStarted();
				this.CheckForExceptions();

				ReceiverRegistrationItem item = new ReceiverRegistrationItem(receiverRegistration);

				this.ReceiveRegistrations.Add(item);

				this.SignalWorkAvailable();
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

				bool success = false;
				try
				{
					lock (this.SyncRoot)
					{
						this.TypeResolver = new TypeInjector(this, dependencyResolver);

						this.SendOperations.Clear();
						this.ReceiveRegistrations.Clear();

						this.WorkAvailable = new WorkSignaler(this);

						this.WorkerThread = new WorkThread(this);
						this.WorkerThread.Timeout = (int)this.ThreadTimeout.TotalMilliseconds;
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
				this.ReceiveRegistrations.RemoveAll(x => object.ReferenceEquals(x.ReceiverRegistration, receiverRegistration));

				this.SignalWorkAvailable();
			}
		}

		#endregion




		#region Type: TypeInjector

		private sealed class TypeInjector : DependencyInjector
		{
			#region Instance Constructor/Destructor

			public TypeInjector (LocalBus localBus, IDependencyResolver dependencyResolver)
				: base(dependencyResolver)
			{
				this.LocalBus = localBus;
			}

			#endregion




			#region Instance Properties/Indexer

			public LocalBus LocalBus { get; }

			#endregion




			#region Overrides

			protected override void Intercept (string name, List<object> instances)
			{
				if (!instances.Contains(this.LocalBus.WorkAvailable))
				{
					if (string.Equals(name, CompositionContainer.GetNameOfType(typeof(IBusPipelineWorkSignaler)), StringComparison.OrdinalIgnoreCase) || string.Equals(name, CompositionContainer.GetNameOfType(typeof(IDisposable)), StringComparison.OrdinalIgnoreCase))
					{
						instances.Add(this.LocalBus.WorkAvailable);
					}
				}

				if (!instances.Contains(this.LocalBus))
				{
					if (string.Equals(name, CompositionContainer.GetNameOfType(typeof(IBus)), StringComparison.OrdinalIgnoreCase) || string.Equals(name, CompositionContainer.GetNameOfType(typeof(IDisposable)), StringComparison.OrdinalIgnoreCase))
					{
						instances.Add(this.LocalBus);
					}
				}
			}

			#endregion
		}

		#endregion




		#region Type: WorkSignaler

		private sealed class WorkSignaler : IBusPipelineWorkSignaler, IDisposable
		{
			#region Instance Constructor/Destructor

			public WorkSignaler (LocalBus localBus)
			{
				this.LocalBus = localBus;

				this.Event = new ManualResetEvent(false);
			}

			~WorkSignaler ()
			{
				this.Dispose();
			}

			#endregion




			#region Instance Properties/Indexer

			public ManualResetEvent Event { get; }

			public LocalBus LocalBus { get; }

			#endregion




			#region Instance Methods

			public void Reset ()
			{
				lock (this.LocalBus.SyncRoot)
				{
					this.Event.Reset();
				}
			}

			#endregion




			#region Interface: IBusPipelineWorkSignaler

			public void Initialize (IDependencyResolver dependencyResolver)
			{
			}

			public void SignalWorkAvailable ()
			{
				lock (this.LocalBus.SyncRoot)
				{
					this.Event.Set();
				}
			}

			public void Unload ()
			{
			}

			#endregion




			#region Interface: IDisposable

			public void Dispose ()
			{
				this.Event?.Close();
			}

			#endregion
		}

		#endregion




		#region Type: WorkThread

		private sealed class WorkThread : HeavyThread
		{
			#region Instance Constructor/Destructor

			public WorkThread (LocalBus localBus)
			{
				this.LocalBus = localBus;
			}

			#endregion




			#region Instance Properties/Indexer

			public LocalBus LocalBus { get; }

			public new Thread Thread => base.Thread;

			private IBusConnectionManager ConnectionManager { get; set; }

			private List<IBusConnection> Connections { get; set; }

			private IDependencyResolver DependencyResolver => this.LocalBus.TypeResolver;

			private IBusDispatcher Dispatcher { get; set; }

			private IBusPipeline Pipeline { get; set; }

			private IBusPipelineWorkSignaler PipelineWorkSignaler { get; set; }

			private IBusRouter Router { get; set; }

			#endregion




			#region Instance Methods

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
				this.PipelineWorkSignaler = this.ResolveSingle<IBusPipelineWorkSignaler>(true);
				this.Dispatcher = this.ResolveSingle<IBusDispatcher>(true);
				this.Router = this.ResolveSingle<IBusRouter>(true);

				this.ConnectionManager = this.ResolveSingle<IBusConnectionManager>(false);
				this.Connections = this.ResolveMultiple<IBusConnection>(false);

				this.Pipeline.Initialize(this.DependencyResolver);
				this.PipelineWorkSignaler.Initialize(this.DependencyResolver);
				this.Dispatcher.Initialize(this.DependencyResolver);
				this.Router.Initialize(this.DependencyResolver);

				this.ConnectionManager?.Initialize(this.DependencyResolver);
				this.Connections.ForEach(x => x.Initialize(this.DependencyResolver));
			}

			protected override void OnEnd ()
			{
				this.Connections?.ForEach(x => x.Unload());
				this.ConnectionManager?.Unload();

				this.Router?.Unload();
				this.Dispatcher?.Unload();
				this.PipelineWorkSignaler?.Unload();
				this.Pipeline?.Unload();

				base.OnEnd();
			}

			protected override void OnException (Exception exception, bool canContinue)
			{
				base.OnException(exception, canContinue);

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

				this.Pipeline.StartProcessing();

				while (true)
				{
					WaitHandle.WaitAny(new WaitHandle[] {this.LocalBus.WorkAvailable.Event, this.StopEvent}, this.LocalBus.PollInterval);

					this.LocalBus.WorkAvailable.Reset();

					if (this.StopRequested)
					{
						break;
					}

					this.Pipeline.DoWork();
				}

				this.Pipeline.StopProcessing();
			}

			protected override void OnStarting ()
			{
				base.OnStarting();

				this.Thread.IsBackground = false;
				this.Thread.Name = this.LocalBus.GetType().Name;

				this.Thread.Priority = this.LocalBus.ThreadPriority;
				this.Thread.CurrentCulture = this.LocalBus.ThreadCulture;
				this.Thread.CurrentUICulture = this.LocalBus.ThreadUICulture;
			}

			#endregion
		}

		#endregion
	}
}
