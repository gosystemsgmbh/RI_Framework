using System;

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

		/// <summary>
		/// Gets or sets whether pending delegates hould be processed when shutting down (<see cref="HeavyThread.Stop"/>).
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
		/// Creates a new instance of <see cref="HeavyThreadDispatcher"/>.
		/// </summary>
		public HeavyThreadDispatcher ()
		{
			this.FinishPendingDelegatesOnShutdown = false;
			this.Dispatcher = null;
		}

		/// <inheritdoc />
		protected override void Dispose (bool disposing)
		{
			base.Dispose(disposing);

			this.Dispatcher = null;
		}

		/// <inheritdoc />
		protected override void OnBegin ()
		{
			base.OnBegin();

			this.Dispatcher = new ThreadDispatcher();
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

			this.Dispatcher.Shutdown(this.FinishPendingDelegatesOnShutdown);
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
			lock (this.SyncRoot)
			{
				this.VerifyRunning();

				return this.Dispatcher.Send(action, parameters);
			}
		}
	}
}
