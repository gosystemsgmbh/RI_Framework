using System;

namespace RI.Framework.Threading.Dispatcher
{
	/// <summary>
	///     Event arguments for the <see cref="IThreadDispatcher" />.<see cref="IThreadDispatcher.Watchdog" /> event.
	/// </summary>
	public sealed class ThreadDispatcherWatchdogEventArgs : EventArgs
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ThreadDispatcherWatchdogEventArgs" />.
		/// </summary>
		/// <param name="timeout"> The timeout. </param>
		/// <param name="currentOperation"> The current operation which was in progress at the time of the timeout or null if no operation was in progress. </param>
		public ThreadDispatcherWatchdogEventArgs(TimeSpan timeout, ThreadDispatcherOperation currentOperation)
		{
			this.Timeout = timeout;
			this.CurrentOperation = currentOperation;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the timeout.
		/// </summary>
		/// <value>
		///     The timeout.
		/// </value>
		public TimeSpan Timeout { get; }

		/// /// <summary>
		///     Gets the current operation which was in progress at the time of the timeout or null if no operation was in progress
		/// </summary>
		/// <value>
		///     The current operation which was in progress at the time of the timeout or null if no operation was in progress.
		/// </value>
		public ThreadDispatcherOperation CurrentOperation { get; }

		#endregion
	}
}
