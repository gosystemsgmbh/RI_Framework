using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Bus.Exceptions;
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
	/// TODO: Logging
	/// TODO: Documentation
	public sealed class LocalBus : LogSource, IDisposable, ISynchronizable
	{
		private TimeSpan _pollInterval;
		private TimeSpan _defaultTimeout;
		private bool _defaultIsGlobal;

		/// <summary>
		/// Gets or sets the timeout after which a <see cref="ResponseTimeoutException"/> is thrown or the collection of responses is finished.
		/// </summary>
		/// <value>
		/// The timeout after which a <see cref="ResponseTimeoutException"/> is thrown or the collection of responses is finished.
		/// </value>
		/// <remarks>
		/// <para>
		/// The default value is 10 seconds.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is negative.</exception>
		public TimeSpan DefaultTimeout
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._defaultTimeout;
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
					this._defaultTimeout = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets whether messages are sent globally by default.
		/// </summary>
		/// <value>
		/// true if messages are sent globally by default, false otherwise.
		/// </value>
		/// <remarks>
		/// <para>
		/// The default value is false.
		/// </para>
		/// </remarks>
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

		/// <summary>
		/// Gets or sets the polling interval.
		/// </summary>
		/// <value>
		/// The polling interval.
		/// </value>
		/// <remarks>
		/// <para>
		/// The default value is 20 milliseconds.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is negative.</exception>
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

		/// <summary>
		/// Gets the dependency resolver which is used for resolving types needed by the local bus.
		/// </summary>
		/// <value>
		/// The dependency resolver which is used for resolving types needed by the local bus.
		/// </value>
		public IDependencyResolver DependencyResolver { get; }

		/// <inheritdoc />
		public object SyncRoot { get; }

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		private object StartStopSyncRoot { get; set; }

		private List<SendOperationItem> SendOperations { get; set; }

		private List<ReceiveRegistrationItem> ReceiveRegistrations { get; set; }

		private ManualResetEvent WorkAvailable { get; set; }

		private WorkThread WorkerThread { get; set; }

		/// <summary>
		/// Creates a new instance of <see cref="LocalBus"/>.
		/// </summary>
		/// <param name="dependencyResolver">The dependency resolver which is used for resolving types needed by the local bus.</param>
		/// <exception cref="ArgumentNullException"><paramref name="dependencyResolver"/> is null.</exception>
		public LocalBus (IDependencyResolver dependencyResolver)
		{
			if (dependencyResolver == null)
			{
				throw new ArgumentNullException(nameof(dependencyResolver));
			}

			this.DependencyResolver = dependencyResolver;

			this.SyncRoot = new object();
			this.StartStopSyncRoot = new object();

			this.DefaultTimeout = TimeSpan.FromSeconds(10);
			this.DefaultIsGlobal = false;
			this.PollInterval = TimeSpan.FromMilliseconds(20);

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

		/// <summary>
		///     Stops the local bus and closes all connections.
		/// </summary>
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

		/// <summary>
		/// Starts the local bus.
		/// </summary>
		/// <exception cref="InvalidOperationException">The local message bus is already started.</exception>
		public void Start ()
		{
			lock (this.StartStopSyncRoot)
			{
				bool success = false;
				try
				{
					lock (this.SyncRoot)
					{
						this.VerifyNotStarted();

						this.SendOperations.Clear();
						this.ReceiveRegistrations.Clear();

						this.WorkAvailable = new ManualResetEvent(false);

						this.WorkerThread = new WorkThread(this);
						this.WorkerThread.Timeout = (int)this.DefaultTimeout.TotalMilliseconds * 2;

						this.WorkerThread.Start();

						success = true;
					}
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

		/// <summary>
		///     Stops the local bus and closes all connections.
		/// </summary>
		/// <param name="disposing"> true if called from <see cref="Stop" /> or <see cref="IDisposable.Dispose" />, false if called from the destructor. </param>
		[SuppressMessage("ReSharper", "UnusedParameter.Local")]
		private void Dispose(bool disposing)
		{
			lock (this.StartStopSyncRoot)
			{
				WorkThread workerThread;
				lock (this.SyncRoot)
				{
					workerThread = this.WorkerThread;
					this.WorkerThread = null;
				}

				workerThread?.Stop();

				lock (this.SyncRoot)
				{
					this.WorkAvailable?.Close();
					this.WorkAvailable = null;

					this.SendOperations?.Clear();
					this.ReceiveRegistrations?.Clear();
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

		/// <summary>
		/// Gets whether the local bus is started.
		/// </summary>
		/// <value>
		/// true if the local bus is started, false otherwise.
		/// </value>
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

		private void SignalWorkAvailable ()
		{
			this.WorkAvailable.Set();
		}

		private void CheckForExceptions()
		{
			if (this.WorkerThread.ThreadException != null)
			{
				throw new LocalBusException(this.WorkerThread.ThreadException);
			}
		}

		internal Task<object> Enqueue (SendOperation sendOperation)
		{
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

		internal void Register (ReceiveRegistration receiveRegistration)
		{
			lock (this.SyncRoot)
			{
				this.VerifyStarted();
				this.CheckForExceptions();

				ReceiveRegistrationItem item = new ReceiveRegistrationItem(receiveRegistration);
				this.ReceiveRegistrations.Add(item);
			}
		}

		internal void Unregister (ReceiveRegistration receiveRegistration)
		{
			lock (this.SyncRoot)
			{
				this.ReceiveRegistrations.RemoveAll(x => object.ReferenceEquals(x.ReceiveRegistration, receiveRegistration));
			}
		}

		/// <summary>
		/// Creates a new send operation which can be configured.
		/// </summary>
		/// <returns>
		/// The new send operation.
		/// </returns>
		/// <exception cref="InvalidOperationException">The local message bus is stopped.</exception>
		/// <exception cref="LocalBusException">The local bus processing pipeline encountered an exception.</exception>
		public SendOperation Send ()
		{
			lock (this.SyncRoot)
			{
				this.VerifyStarted();
				this.CheckForExceptions();
				return new SendOperation(this);
			}
		}

		/// <summary>
		/// Creates a new receive registration which can be configured.
		/// </summary>
		/// <returns>
		/// The new receive registration.
		/// </returns>
		/// <exception cref="InvalidOperationException">The local message bus is stopped.</exception>
		/// <exception cref="LocalBusException">The local bus processing pipeline encountered an exception.</exception>
		public ReceiveRegistration Receive ()
		{
			lock (this.SyncRoot)
			{
				this.VerifyStarted();
				this.CheckForExceptions();
				return new ReceiveRegistration(this);
			}
		}







		private sealed class SendOperationItem
		{
			public SendOperationItem (SendOperation sendOperation)
			{
				this.SendOperation = sendOperation;

				this.Task = new TaskCompletionSource<object>(this.SendOperation, TaskCreationOptions.DenyChildAttach);
			}

			public SendOperation SendOperation { get; }

			public TaskCompletionSource<object> Task { get; }
		}

		private sealed class ReceiveRegistrationItem
		{
			public ReceiveRegistrationItem(ReceiveRegistration receiveRegistration)
			{
				this.ReceiveRegistration = receiveRegistration;
			}

			public ReceiveRegistration ReceiveRegistration { get; }
		}

		private sealed class WorkThread : HeavyThread
		{
			public WorkThread (LocalBus localBus)
			{
				this.LocalBus = localBus;
			}

			public LocalBus LocalBus { get; }

			protected override void OnException (Exception exception, bool canContinue)
			{
				lock (this.LocalBus.SyncRoot)
				{
					foreach (SendOperationItem item in this.LocalBus.SendOperations)
					{
						item.Task.TrySetException(new LocalBusException(exception));
					}

					this.LocalBus.SendOperations.Clear();
				}

				base.OnException(exception, canContinue);
			}

			//TODO: Implement
		}
	}
}
