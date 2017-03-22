using System;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Utilities.Threading
{
	/// <summary>
	/// Defines the interface for thread-bound dispatchers.
	/// </summary>
	public interface IThreadDispatcher : ISynchronizable
	{
		/// <summary>
		/// Gets the active shutdown mode.
		/// </summary>
		/// <value>
		/// <see cref="ThreadDispatcherShutdownMode.None"/> if the dispatcher is not running or is not being shut down, <see cref="ThreadDispatcherShutdownMode.FinishPending"/> if the dispatcher is shutting down and all already pending delegates are processed before the shutdown completes, <see cref="ThreadDispatcherShutdownMode.DiscardPending"/> if the dispatcher is shutting down and all already pending delegates are discarded.
		/// </value>
		ThreadDispatcherShutdownMode ShutdownMode { get; }

		/// <summary>
		/// Gets whether the dispatcher is running.
		/// </summary>
		/// <value>
		/// true if the dispatcher is running, false otherwise.
		/// </value>
		bool IsRunning { get; }

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
		bool CatchExceptions { get; set; }

		/// <summary>
		/// Raised when an exception occurred during execution of a enqueued delegate.
		/// </summary>
		/// <remarks>
		/// <note type="important">
		/// The event is raised from the thread <see cref="IThreadDispatcher"/> runs in.
		/// </note>
		/// </remarks>
		event EventHandler<ThreadDispatcherExceptionEventArgs> Exception;

		/// <summary>
		/// Stops processing the delegate queue.
		/// </summary>
		/// <param name="finishPendingDelegates">Specifies whether already pending delegates should be processed before the dispatcher is shut down.</param>
		/// <exception cref="InvalidOperationException">The dispatcher is not running or it is already being shut down.</exception>
		void Shutdown (bool finishPendingDelegates);

		/// <summary>
		/// Enqueues a delegate to the dispatchers queue and does not wait for its execution.
		/// </summary>
		/// <param name="action">The delegate.</param>
		/// <param name="parameters">Optional parameters of the delagate.</param>
		/// <returns>
		/// The dispatcher operation object which can be used to track the execution of the enqueued delegate.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
		/// <exception cref="InvalidOperationException">The dispatcher is not running or is being shut down.</exception>
		ThreadDispatcherOperation Post (Delegate action, params object[] parameters);

		/// <summary>
		/// Enqueues a delegate to the dispatchers queue and waits for its execution to be completed.
		/// </summary>
		/// <param name="action">The delegate.</param>
		/// <param name="parameters">Optional parameters of the delagate.</param>
		/// <returns>
		/// The return value of the executed delegate or null if the delegate has no return value. 
		/// </returns>
		/// <remarks>
		/// <para>
		/// <see cref="Send"/> blocks until all previously enqueued delegates were processed.
		/// </para>
		/// <para>
		/// <see cref="Send"/> can be called from the dispatchers thread.
		/// Therefore, <see cref="Send"/> calls can be cascaded.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
		/// <exception cref="InvalidOperationException">The dispatcher is not running or is being shut down.</exception>
		object Send (Delegate action, params object[] parameters);

		/// <summary>
		/// Waits until all queued operations have been processed.
		/// </summary>
		void DoProcessing ();

		/// <summary>
		///     Determines whether the caller of this function is executed inside the dispatchers thread or not.
		/// </summary>
		/// <returns>
		///     true if the caller of this function is executed inside this thread, false otherwise or if the dispatcher is not running.
		/// </returns>
		bool IsInThread();
	}
}
