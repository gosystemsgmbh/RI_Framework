using System;
using System.Threading;




namespace RI.Framework.Utilities.Threading
{
	/// <summary>
	/// Combines a <see cref="HeavyThread"/> and a <see cref="ThreadDispatcher"/> which is run inside the <see cref="HeavyThread"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// See <see cref="HeavyThread"/> and <see cref="ThreadDispatcher"/> for more details.
	/// </para>
	/// </remarks>
	public sealed class HeavyThreadDispatcher : HeavyThread
	{
		private bool _finishPendingDelegatesOnShutdown;
		private ThreadPriority _threadPriority;
		private string _threadName;
		private bool _isBackgroundThread;
		private bool _catchExceptions;

		/// <summary>
		/// Gets or sets whether pending delegates should be processed when shutting down (<see cref="HeavyThread.Stop"/>).
		/// </summary>
		/// <value>
		/// true if pending delegates should be processed during shutdown, false if pending delegates should be discarded when <see cref="HeavyThread.Stop"/> is called.
		/// </value>
		/// <remarks>
		/// <para>
		/// The default value is false.
		/// </para>
		/// </remarks>
		public bool FinishPendingDelegatesOnShutdown
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._finishPendingDelegatesOnShutdown;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._finishPendingDelegatesOnShutdown = value;
				}
			}
		}

		private ThreadDispatcher Dispatcher { get; set; }

		private EventHandler<ThreadDispatcherExceptionEventArgs> DispatcherExceptionHandlerDelegate { get; set; }

		/// <inheritdoc cref="ThreadDispatcher.ShutdownMode"/>
		public ThreadDispatcherShutdownMode ShutdownMode
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.Dispatcher?.ShutdownMode ?? ThreadDispatcherShutdownMode.None;
				}
			}
		}

		/// <summary>
		/// Gets or sets the priority of the thread.
		/// </summary>
		/// <value>
		/// The priority of the thread.
		/// </value>
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

					if (this.Thread != null)
					{
						this.Thread.Priority = value;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the name of the thread.
		/// </summary>
		/// <value>
		/// The name of the thread.
		/// </value>
		public string ThreadName
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._threadName;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._threadName = value;

					if (this.Thread != null)
					{
						this.Thread.Name = value;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the thread is a background thread.
		/// </summary>
		/// <value>
		/// true if the thread is a background thread, false otherwise.
		/// </value>
		public bool IsBackgroundThread
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._isBackgroundThread;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._isBackgroundThread = value;

					if (this.Thread != null)
					{
						this.Thread.IsBackground = value;
					}
				}
			}
		}

		/// <summary>
		/// Creates a new instance of <see cref="HeavyThreadDispatcher"/>.
		/// </summary>
		public HeavyThreadDispatcher ()
		{
			this.DispatcherExceptionHandlerDelegate = this.DispatcherExceptionHandler;

			this.CatchExceptions = false;
			this.FinishPendingDelegatesOnShutdown = false;
			this.ThreadName = this.GetType().Name;
			this.ThreadPriority = ThreadPriority.Normal;
			this.IsBackgroundThread = true;

			this.Dispatcher = null;
		}

		private void DispatcherExceptionHandler (object sender, ThreadDispatcherExceptionEventArgs args)
		{
			this.OnException(args.Exception, true, args.CanContinue);
		}

		/// <summary>
		/// Raised when an exception occurred during execution of a enqueued delegate.
		/// </summary>
		/// <remarks>
		/// <note type="important">
		/// The event is raised from the <see cref="HeavyThreadDispatcher"/>s thread.
		/// </note>
		/// </remarks>
		public event EventHandler<ThreadDispatcherExceptionEventArgs> Exception;

		private void OnException(Exception exception, bool canContinue)
		{
			this.Exception?.Invoke(this, new ThreadDispatcherExceptionEventArgs(exception, canContinue));
		}

		/// <summary>
		/// Gets or sets whether exceptions, thrown when executing delegates, are catched and the dispatcher continues its operations.
		/// </summary>
		/// <value>
		/// true if exceptions are catched, false otherwise.
		/// </value>
		/// <remarks>
		/// <para>
		/// The default value is false.
		/// </para>
		/// <para>
		/// The <see cref="Exception"/> event is raised regardless of the value of <see cref="CatchExceptions"/>.
		/// </para>
		/// </remarks>
		public bool CatchExceptions
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._catchExceptions;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._catchExceptions = value;

					if (this.Dispatcher != null)
					{
						this.Dispatcher.CatchExceptions = value;
					}
				}
			}
		}

		/// <inheritdoc />
		protected override void OnStart ()
		{
			base.OnStart();

			this.Thread.Name = this.ThreadName;
			this.Thread.Priority = this.ThreadPriority;
			this.Thread.IsBackground = this.IsBackgroundThread;
		}

		/// <inheritdoc />
		protected override void Dispose (bool disposing)
		{
			base.Dispose(disposing);

			if (this.Dispatcher != null)
			{
				this.Dispatcher.Exception -= this.DispatcherExceptionHandlerDelegate;
				this.Dispatcher = null;
			}
		}

		/// <inheritdoc />
		protected override void OnException (Exception exception, bool running, bool canContinue)
		{
			base.OnException(exception, running, canContinue);

			this.OnException(exception, canContinue);
		}

		/// <inheritdoc />
		protected override void OnBegin ()
		{
			base.OnBegin();

			this.Dispatcher = new ThreadDispatcher();
			this.Dispatcher.Exception += this.DispatcherExceptionHandlerDelegate;
			this.Dispatcher.CatchExceptions = this.CatchExceptions;
		}

		/// <inheritdoc />
		protected override void OnRun ()
		{
			base.OnRun();

			this.Dispatcher.Run();
		}

		/// <inheritdoc />
		protected override void OnStop ()
		{
			base.OnStop();

			if (this.Dispatcher.IsRunning)
			{
				this.Dispatcher.Shutdown(this.FinishPendingDelegatesOnShutdown);
			}
		}

		/// <inheritdoc cref="HeavyThread.Stop"/>
		/// <param name="finishPendingDelegates">Specifies whether already pending delegates should be processed before the dispatcher is shut down.</param>
		/// <remarks>
		/// <para>
		/// <see cref="FinishPendingDelegatesOnShutdown"/> is set to the value of <paramref name="finishPendingDelegates"/>.
		/// </para>
		/// </remarks>
		public void Stop (bool finishPendingDelegates)
		{
			this.FinishPendingDelegatesOnShutdown = finishPendingDelegates;

			this.Stop();
		}

		/// <inheritdoc cref="ThreadDispatcher.Post"/>
		public ThreadDispatcherOperation Post(Delegate action, params object[] parameters)
		{
			lock (this.SyncRoot)
			{
				this.VerifyRunning();

				return this.Dispatcher.Post(action, parameters);
			}
		}

		/// <inheritdoc cref="ThreadDispatcher.Send"/>
		public object Send (Delegate action, params object[] parameters)
		{
			this.VerifyRunning();

			return this.Dispatcher.Send(action, parameters);
		}
	}
}
