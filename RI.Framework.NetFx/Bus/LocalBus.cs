using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
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
	public sealed class LocalBus : LogSource, IDisposable, ISynchronizable
	{
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

		private bool IsStarted { get; set; }

		private List<SendOperationItem> SendOperations { get; set; }

		private ManualResetEvent WorkAvailable { get; set; }

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

			this.IsStarted = false;
			this.WorkAvailable = new ManualResetEvent(true);
			this.SendOperations = new List<SendOperationItem>();
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
					}

					//TODO: Implement

					lock (this.SyncRoot)
					{
						this.IsStarted = true;
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
				//TODO: Implement

				lock (this.SyncRoot)
				{
					this.IsStarted = false;
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

		internal Task<object> Enqueue(SendOperation sendOperation)
		{
			SendOperationItem item;

			lock (this.SyncRoot)
			{
				this.VerifyStarted();

				item = new SendOperationItem(sendOperation);

				this.SendOperations.Add(item);
				this.WorkAvailable.Set();
			}

			return item.Task.Task;
		}

		/// <summary>
		/// Creates a new message which can be sent.
		/// </summary>
		/// <returns>
		/// The message which can be configured and sent.
		/// </returns>
		/// <exception cref="InvalidOperationException">The local message bus is stopped.</exception>
		public SendOperation Message ()
		{
			lock (this.SyncRoot)
			{
				this.VerifyStarted();
				return new SendOperation(this);
			}
		}

		/// <summary>
		/// Receives all messages.
		/// </summary>
		/// <param name="callback">The callback, called for each message.</param>
		/// <exception cref="ArgumentNullException"><paramref name="callback"/> is null.</exception>
		/// <exception cref="InvalidOperationException">The local message bus is stopped.</exception>
		public void Receive (ReceiveCallback callback)
		{
			if (callback == null)
			{
				throw new ArgumentNullException(nameof(callback));
			}

			lock (this.SyncRoot)
			{
				this.VerifyStarted();

				//TODO: Implement
			}
		}

		/// <summary>
		/// Receives all messages with a specified type of payload.
		/// </summary>
		/// <typeparam name="TPayload">The type of the payload.</typeparam>
		/// <typeparam name="TResponse">The type of the response.</typeparam>
		/// <param name="callback">The callback, called for each message.</param>
		/// <exception cref="ArgumentNullException"><paramref name="callback"/> is null.</exception>
		/// <exception cref="InvalidOperationException">The local message bus is stopped.</exception>
		public void Receive<TPayload, TResponse> (ReceiveCallback<TPayload, TResponse> callback)
			where TPayload : class
			where TResponse : class
		{
			if (callback == null)
			{
				throw new ArgumentNullException(nameof(callback));
			}

			lock (this.SyncRoot)
			{
				this.VerifyStarted();

				//TODO: Implement
			}
		}

		/// <summary>
		/// Receives all messages sent to a specified address.
		/// </summary>
		/// <param name="address">The address.</param>
		/// <param name="callback">The callback, called for each message.</param>
		/// <exception cref="ArgumentNullException"><paramref name="address"/> or <paramref name="callback"/> is null.</exception>
		/// <exception cref="EmptyStringArgumentException"><paramref name="address"/> is an empty string.</exception>
		/// <exception cref="InvalidOperationException">The local message bus is stopped.</exception>
		public void Receive (string address, ReceiveCallback callback)
		{
			if (address == null)
			{
				throw new ArgumentNullException(nameof(callback));
			}

			if (address.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(address));
			}

			if (callback == null)
			{
				throw new ArgumentNullException(nameof(callback));
			}

			lock (this.SyncRoot)
			{
				this.VerifyStarted();

				//TODO: Implement
			}
		}

		/// <summary>
		/// Receives all messages sent to a specified addres and with a specified type of payload.
		/// </summary>
		/// <typeparam name="TPayload">The type of the payload.</typeparam>
		/// <typeparam name="TResponse">The type of the response.</typeparam>
		/// <param name="address">The address.</param>
		/// <param name="callback">The callback, called for each message.</param>
		/// <exception cref="ArgumentNullException"><paramref name="address"/> or <paramref name="callback"/> is null.</exception>
		/// <exception cref="EmptyStringArgumentException"><paramref name="address"/> is an empty string.</exception>
		/// <exception cref="InvalidOperationException">The local message bus is stopped.</exception>
		public void Receive <TPayload, TResponse> (string address, ReceiveCallback<TPayload, TResponse> callback)
			where TPayload : class
			where TResponse : class
		{
			if (address == null)
			{
				throw new ArgumentNullException(nameof(callback));
			}

			if (address.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(address));
			}

			if (callback == null)
			{
				throw new ArgumentNullException(nameof(callback));
			}

			lock (this.SyncRoot)
			{
				this.VerifyStarted();

				//TODO: Implement
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
	}
}
