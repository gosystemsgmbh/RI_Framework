using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Utilities.Threading
{
	/// <summary>
	///     Implements a heavy-weight thread which encapsulates thread setup and execution and also captures exceptions.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="HeavyThread" /> is usually used for long-running and/or dedicated threads where a higher control and encapsulation of the threads execution is required.
	///     </para>
	///     <para>
	///         See <see cref="Start" /> and <see cref="Stop" /> for a description of the thread execution sequence.
	///     </para>
	/// </remarks>
	public abstract class HeavyThread : IDisposable, ISynchronizable
	{
		#region Constants

		/// <summary>
		///     The default thread timeout.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The default value is 10000.
		///     </para>
		/// </remarks>
		public const int DefaultThreadTimeout = 10000;

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="HeavyThread" />.
		/// </summary>
		protected HeavyThread ()
		{
			this.SyncRoot = new object();
			this.StartStopSyncRoot = new object();
			this.Thread = null;
			this.ThreadException = null;
			this.IsRunning = false;
			this.HasStoppedGracefully = null;
			this.Timeout = HeavyThread.DefaultThreadTimeout;
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="HeavyThread" />.
		/// </summary>
		~HeavyThread ()
		{
			this.Dispose(false);
		}

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		object ISynchronizable.SyncRoot => this.SyncRoot;

		#endregion




		#region Instance Fields

		private Exception _exception;

		private bool? _hasStoppedGracefully;

		private bool _isRunning;

		private int _timeout;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets whether the thread has gracefully ended or was forcibly terminated.
		/// </summary>
		/// <value>
		///     true if the thread ended gracefully, false if the thread was forcibly terminated, or null if the thread was not started or is still running.
		/// </value>
		/// <remarks>
		///     <para>
		///         See <see cref="Stop" /> for more details.
		///     </para>
		/// </remarks>
		public bool? HasStoppedGracefully
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._hasStoppedGracefully;
				}
			}
			private set
			{
				lock (this.SyncRoot)
				{
					this._hasStoppedGracefully = value;
				}
			}
		}

		/// <summary>
		///     Gets whether the thread is running.
		/// </summary>
		/// <value>
		///     true if the thread is running, false otherwise.
		/// </value>
		public bool IsRunning
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._isRunning;
				}
			}
			private set
			{
				lock (this.SyncRoot)
				{
					this._isRunning = value;
				}
			}
		}

		/// <summary>
		///     Gets the exception of the thread.
		/// </summary>
		/// <value>
		///     The exception of the thread or null if no exception occurred.
		/// </value>
		/// <remarks>
		///     <para>
		///         <see cref="ThreadException" /> is set for any exception of the thread, at any stage during start, run, and stop, except for <see cref="ThreadAbortException" />s, which are silently ignored.
		///     </para>
		/// </remarks>
		public Exception ThreadException
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._exception;
				}
			}
			private set
			{
				lock (this.SyncRoot)
				{
					this._exception = value;
				}
			}
		}

		/// <summary>
		///     Gets or sets the timeout of the thread in milliseconds used for start and stop.
		/// </summary>
		/// <value>
		///     The timeout of the thread in milliseconds used for start and stop.
		/// </value>
		/// <remarks>
		///     <para>
		///         This timeout is used during <see cref="Start" /> while waiting for <see cref="OnBegin" /> to finish and during <see cref="Dispose(bool)" /> while waiting for <see cref="OnStop" /> to take effect.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="value" /> is less than zero. </exception>
		public int Timeout
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._timeout;
				}
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				lock (this.SyncRoot)
				{
					this._timeout = value;
				}
			}
		}

		/// <summary>
		///     Gets the actual thread instance used to run the thread.
		/// </summary>
		/// <value>
		///     The actual thread instance used to run the thread.
		/// </value>
		protected Thread Thread { get; private set; }

		private object StartStopSyncRoot { get; set; }

		private object SyncRoot { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Checks and rethrows any exception occurred in this thread.
		/// </summary>
		/// <remarks>
		///     <para>
		///         This method returns without any effect if no exception occurred.
		///     </para>
		/// </remarks>
		/// <exception cref="InvalidOperationException"> This function was called from inside the thread. </exception>
		/// <exception cref="HeavyThreadException"> An exception occured in this thread. </exception>
		public void CheckForException ()
		{
			this.VerifyNotFromThread(nameof(this.CheckForException));

			Exception threadException = this.ThreadException;
			if (threadException != null)
			{
				throw new HeavyThreadException(this.ThreadException);
			}
		}

		/// <summary>
		///     Starts the thread.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The sequence to start goes as follows:
		///     </para>
		///     <list type="number">
		///         <item>
		///             <para>
		///                 <see cref="Thread" /> is created and initialized.
		///             </para>
		///         </item>
		///         <item>
		///             <para>
		///                 <see cref="OnStart" /> is called.
		///             </para>
		///         </item>
		///         <item>
		///             <para>
		///                 The thread actually starts executing.
		///             </para>
		///         </item>
		///         <item>
		///             <para>
		///                 <see cref="Start" /> is waiting till <see cref="OnBegin" /> finished inside the thread or the timeout specified by <see cref="Timeout" /> occurrs.
		///             </para>
		///         </item>
		///         <item>
		///             <para>
		///                 <see cref="Start" /> returns while <see cref="OnRun" /> is executed inside the thread.
		///             </para>
		///         </item>
		///         <item>
		///             <para>
		///                 As soon as <see cref="OnRun" /> finishes, <see cref="OnEnd" /> is executed inside the thread and the thread ended afterwards.
		///             </para>
		///         </item>
		///     </list>
		/// </remarks>
		/// <exception cref="InvalidOperationException"> The thread is already running. </exception>
		/// <exception cref="TimeoutException"> The thread failed to return from <see cref="OnBegin" /> within <see cref="Timeout" />. </exception>
		/// <exception cref="HeavyThreadException"> An exception occurred inside the thread during execution of <see cref="OnBegin" />. </exception>
		[SuppressMessage ("ReSharper", "AccessToDisposedClosure")]
		[SuppressMessage ("ReSharper", "EmptyGeneralCatchClause")]
		public void Start ()
		{
			lock (this.StartStopSyncRoot)
			{
				if (this.IsRunning)
				{
					throw new InvalidOperationException(this.GetType().Name + " is already running.");
				}

				bool success = false;
				try
				{
					using (ManualResetEvent startEvent = new ManualResetEvent(false))
					{
						lock (this.SyncRoot)
						{
							this.Thread = new Thread(() =>
							                         {
								                         bool running = false;
								                         try
								                         {
									                         this.OnBegin();
									                         running = true;
									                         startEvent.Set();
									                         this.OnRun();
									                         this.OnEnd();
								                         }
								                         catch (ThreadAbortException)
								                         {
								                         }
								                         catch (Exception exception)
								                         {
									                         try
									                         {
										                         this.ThreadException = exception;
									                         }
									                         catch
									                         {
									                         }
									                         try
									                         {
										                         this.OnException(exception, running);
									                         }
									                         catch
									                         {
									                         }
								                         }
							                         });

							this.Thread.Name = this.GetType().Name;
							this.Thread.IsBackground = true;
							this.Thread.Priority = ThreadPriority.Normal;
							this.Thread.SetApartmentState(ApartmentState.STA);

							this.OnStart();

							this.Thread.Start();
						}

						bool started = startEvent.WaitOne(this.Timeout);

						lock (this.SyncRoot)
						{
							if (!started)
							{
								if (this.ThreadException != null)
								{
									throw new HeavyThreadException(this.GetType().Name + " failed to start (exception).", this.ThreadException);
								}
								else
								{
									throw new TimeoutException(this.GetType().Name + " failed to start (timeout).");
								}
							}

							this.IsRunning = true;
							success = true;
						}
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
		///     Stops the thread and frees all resources.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The sequence to stop goes as follows:
		///     </para>
		///     <list type="number">
		///         <item>
		///             <para>
		///                 <see cref="OnStop" /> is called.
		///             </para>
		///         </item>
		///         <item>
		///             <para>
		///                 <see cref="Stop" /> waits until the thread ended (<see cref="OnRun" /> and <see cref="OnEnd" /> finished executing inside the thread) or the timeout specified by <see cref="Timeout" /> occurrs.
		///             </para>
		///         </item>
		///         <item>
		///             <para>
		///                 If the thread did not gracefully end on its own or a timeout occurred respectively (see step above), the thread is terminated using <see cref="System.Threading.Thread" />.<see cref="System.Threading.Thread.Abort()" />.
		///             </para>
		///         </item>
		///         <item>
		///             <para>
		///                 <see cref="Stop" /> returns.
		///             </para>
		///         </item>
		///     </list>
		/// </remarks>
		public void Stop ()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///     Determines whether the caller of this function is executed inside the thread or not.
		/// </summary>
		/// <returns>
		///     true if the caller of this function is executed inside this thread, false otherwise.
		/// </returns>
		protected bool IsInThread ()
		{
			lock (this.SyncRoot)
			{
				if (this.Thread == null)
				{
					return false;
				}

				return this.Thread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId;
			}
		}

		/// <summary>
		///     Ensures that the caller of this function is not executed inside the thread.
		/// </summary>
		/// <param name="operation"> The name of the performed operation. </param>
		/// <exception cref="InvalidOperationException"> The caller of this function is executed inside this thread. </exception>
		protected void VerifyNotFromThread (string operation)
		{
			if (this.IsInThread())
			{
				throw new InvalidOperationException(operation + " cannot be used from inside the thread of " + this.GetType().Name + ".");
			}
		}

		#endregion




		#region Virtuals

		/// <summary>
		///     Stops the thread and frees all resources.
		/// </summary>
		/// <param name="disposing"> true if called from <see cref="Stop" /> or <see cref="IDisposable.Dispose" />, false if called from the destructor. </param>
		/// <remarks>
		///     <para>
		///         See <see cref="Stop" /> for a description of the sequence when stopping/disposing the thread.
		///     </para>
		/// </remarks>
		/// <exception cref="InvalidOperationException"> This function was called from inside the thread. </exception>
		[SuppressMessage ("ReSharper", "EmptyGeneralCatchClause")]
		protected virtual void Dispose (bool disposing)
		{
			lock (this.StartStopSyncRoot)
			{
				this.VerifyNotFromThread(nameof(this.Dispose));

				lock (this.SyncRoot)
				{
					this.OnStop();
				}

				bool terminated;
				try
				{
					terminated = this.Thread.Join(this.Timeout);
				}
				catch
				{
					terminated = false;
				}

				lock (this.SyncRoot)
				{
					if (!terminated)
					{
						try
						{
							this.Thread.Abort();
						}
						catch
						{
						}
					}

					this.Thread = null;
					this.HasStoppedGracefully = terminated;
					this.IsRunning = false;
				}
			}
		}

		/// <summary>
		///     Called when the thread begins execution.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         Do not execute the actual operations of the thread inside <see cref="OnBegin" /> as this might trigger a timeout.
		///         See <see cref="Start" /> for more details.
		///     </note>
		///     <note type="note">
		///         This method is called inside the thread.
		///     </note>
		/// </remarks>
		protected virtual void OnBegin ()
		{
		}

		/// <summary>
		///     Called when the thread ends.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The thread ends as soon as <see cref="OnRun" /> returns.
		///     </para>
		///     <note type="note">
		///         This method is called inside the thread.
		///     </note>
		/// </remarks>
		protected virtual void OnEnd ()
		{
		}

		/// <summary>
		///     Called when an exception occurred inside the thread.
		/// </summary>
		/// <param name="exception"> The exception. </param>
		/// <param name="running"> Indicates whether the exception occured during the actual run (true; inside <see cref="OnRun" /> or <see cref="OnEnd" />) or during start (false; inside <see cref="OnBegin" />). </param>
		/// <remarks>
		///     <note type="note">
		///         This method is called inside the thread.
		///     </note>
		/// </remarks>
		protected virtual void OnException (Exception exception, bool running)
		{
		}

		/// <summary>
		///     Called when the thread is running and supposed to perform its operations.
		/// </summary>
		/// <remarks>
		///     <note type="note">
		///         This method is called inside the thread.
		///     </note>
		/// </remarks>
		protected virtual void OnRun ()
		{
		}

		/// <summary>
		///     Called before the thread is started.
		/// </summary>
		/// <remarks>
		///     <para>
		///         Thread execution configuration, such as priority or apartment state, must be configured in this method.
		///     </para>
		///     <note type="note">
		///         This method is called by <see cref="Start" />.
		///     </note>
		/// </remarks>
		protected virtual void OnStart ()
		{
		}

		/// <summary>
		///     Called to stop the thread.
		/// </summary>
		/// <remarks>
		///     <para>
		///         This method can be used to signalize the thread to end and return from <see cref="OnRun" />.
		///     </para>
		///     <note type="important">
		///         If the thread does not end on its own after <see cref="OnStop" /> was called, the thread is terminated.
		///         See <see cref="Stop" /> for more details.
		///     </note>
		///     <note type="note">
		///         This method is called by <see cref="Stop" />.
		///     </note>
		/// </remarks>
		protected virtual void OnStop ()
		{
		}

		#endregion




		#region Interface: IDisposable

		/// <inheritdoc />
		void IDisposable.Dispose ()
		{
			this.Stop();
		}

		#endregion
	}
}
