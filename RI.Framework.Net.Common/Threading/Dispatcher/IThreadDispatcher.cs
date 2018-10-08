using System;
using System.ComponentModel;
using System.Threading.Tasks;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Threading.Dispatcher
{
	/// <summary>
	///     Defines the interface for thread-bound dispatchers.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="ThreadDispatcher" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public interface IThreadDispatcher : ISynchronizeInvoke, IDisposable, ISynchronizable
	{
		/// <summary>
		///     Gets or sets whether exceptions, thrown when executing delegates, are catched and the dispatcher continues its operations.
		/// </summary>
		/// <value>
		///     true if exceptions are catched, false otherwise.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The default value is expected to be false.
		///     </note>
		///     <note type="implement">
		///         The <see cref="Exception" /> event is raised regardless of the value of <see cref="CatchExceptions" />.
		///     </note>
		/// </remarks>
		bool CatchExceptions { get; set; }

		/// <summary>
		///     Gets or sets the default options.
		/// </summary>
		/// <value>
		///     The default options.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The default value is expected to be <see cref="ThreadDispatcherOptions.None" />.
		///     </note>
		/// </remarks>
		ThreadDispatcherOptions DefaultOptions { get; set; }

		/// <summary>
		///     Gets or sets the default priority.
		/// </summary>
		/// <value>
		///     The default priority.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The default value is expected to be <c> int.MaxValue / 2 </c>.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="value" /> is less than zero. </exception>
		int DefaultPriority { get; set; }

		/// <summary>
		///     Gets whether the dispatcher is running.
		/// </summary>
		/// <value>
		///     true if the dispatcher is running, false otherwise.
		/// </value>
		bool IsRunning { get; }

		/// <summary>
		///     Gets whether the dispatcher is shutting down.
		/// </summary>
		/// <value>
		///     true if the dispatcher is shutting down, false otherwise.
		///     false is also returned if the dispatcher is not running.
		/// </value>
		bool IsShuttingDown { get; }

		/// <summary>
		///     Gets the active shutdown mode.
		/// </summary>
		/// <value>
		///     The active shutdown mode or <see cref="ThreadDispatcherShutdownMode.None" /> if the dispatcher is not shutting down.
		///     <see cref="ThreadDispatcherShutdownMode.None" /> is also returned if the dispatcher is not running.
		/// </value>
		ThreadDispatcherShutdownMode ShutdownMode { get; }

		/// <summary>
		///     Gets the watchdog timeout.
		/// </summary>
		/// <value>
		///     The watchdog timeout or null if no watchdog is used.
		/// </value>
		/// <exception cref="ArgumentOutOfRangeException"> The value is negative. </exception>
		TimeSpan? WatchdogTimeout { get; set; }

		/// <summary>
		///     Raised when an exception occurred during execution of a delegate.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         The event is raised from the thread <see cref="IThreadDispatcher" /> runs in.
		///     </note>
		/// </remarks>
		event EventHandler<ThreadDispatcherExceptionEventArgs> Exception;

		/// <summary>
		///     Raised when a watchdog was not reset within the timeout period.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         The event is raised from a separate watchdog thread.
		///     </note>
		/// </remarks>
		event EventHandler<ThreadDispatcherWatchdogEventArgs> Watchdog;

		/// <summary>
		///     Adds an object to the list of objects which are kept alive at least as long as this thread dispatcher is running.
		/// </summary>
		/// <param name="obj"> The object to add to the keep-alive list. </param>
		/// <returns>
		///     true if the thread dispatcher is running and the object was added to the keep-alive list, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The keep-alive list simply storeas a reference to the specified object, preventing the garbage collector from collecting the object.
		///         Therefore, the keep-alive list is useful for keeping objects which are needed later, e.g. during the processing of a delegate, but are not required anywhere else.
		///         An example for this is a <see cref="ThreadDispatcherTimer" /> which needs to live as long as the timer is running, e.g. a one-shot timer where the <see cref="ThreadDispatcherTimer" /> instance is not required anymore after it was created.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="obj" /> is null. </exception>
		bool AddKeepAlive (object obj);

		/// <summary>
		///     Stops processing the delegate queue but does not wait for its shutdown.
		/// </summary>
		/// <param name="finishPendingDelegates"> Specifies whether already pending delegates should be processed before the dispatcher is shut down. </param>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running or it is already being shut down. </exception>
		void BeginShutdown (bool finishPendingDelegates);

		/// <summary>
		///     Waits until all queued operations have been processed.
		/// </summary>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running. </exception>
		void DoProcessing ();

		/// <summary>
		///     Waits until all queued operations of a specified priority have been processed.
		/// </summary>
		/// <param name="priority"> The priority. </param>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than zero. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running. </exception>
		void DoProcessing (int priority);

		/// <summary>
		///     Waits until all queued operations have been processed.
		/// </summary>
		/// <returns>
		///     The task which can be used to await the completion of the processing.
		/// </returns>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running. </exception>
		Task DoProcessingAsync ();

		/// <summary>
		///     Waits until all queued operations of a specified priority have been processed.
		/// </summary>
		/// <param name="priority"> The priority. </param>
		/// <returns>
		///     The task which can be used to await the completion of the processing.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than zero. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running. </exception>
		Task DoProcessingAsync (int priority);

		/// <summary>
		///     Determines under which options the current code is executed.
		/// </summary>
		/// <returns>
		///     The options of the currently executed code or null if the current code is not executed by the dispatcher.
		///     null is returned if the dispatcher is not running.
		/// </returns>
		ThreadDispatcherOptions? GetCurrentOptions ();

		/// <summary>
		///     Determines under which priority the current code is executed.
		/// </summary>
		/// <returns>
		///     The priority of the currently executed code or null if the current code is not executed by the dispatcher.
		///     null is returned if the dispatcher is not running.
		/// </returns>
		int? GetCurrentPriority ();

		/// <summary>
		///     Determines whether the caller of this function is executed inside the dispatchers thread or not.
		/// </summary>
		/// <returns>
		///     true if the caller of this function is executed inside this thread, false otherwise.
		///     false is returned if the dispatcher is not running.
		/// </returns>
		bool IsInThread ();

		/// <summary>
		///     Enqueues a delegate to the dispatchers queue and does not wait for its execution.
		/// </summary>
		/// <param name="action"> The delegate. </param>
		/// <param name="parameters"> Optional parameters of the delagate. </param>
		/// <returns>
		///     The dispatcher operation object which can be used to track the execution of the enqueued delegate.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The delegate is enqueued with the default priority (<see cref="DefaultPriority" />).
		///     </para>
		///     <note type="important">
		///         A delegate can be enqueued before the dispatcher is run.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is being shut down. </exception>
		ThreadDispatcherOperation Post (Delegate action, params object[] parameters);

		/// <summary>
		///     Enqueues a delegate to the dispatchers queue and does not wait for its execution.
		/// </summary>
		/// <param name="priority"> The priority. Can be -1 to use the default priority. </param>
		/// <param name="action"> The delegate. </param>
		/// <param name="parameters"> Optional parameters of the delagate. </param>
		/// <returns>
		///     The dispatcher operation object which can be used to track the execution of the enqueued delegate.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The higher the priority, the earlier the operation is executed (highest priority, first executed).
		///     </para>
		///     <note type="important">
		///         A delegate can be enqueued before the dispatcher is run.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than minus one. </exception>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is being shut down. </exception>
		ThreadDispatcherOperation Post (int priority, Delegate action, params object[] parameters);

		/// <summary>
		///     Enqueues a delegate to the dispatchers queue and does not wait for its execution.
		/// </summary>
		/// <param name="priority"> The priority. Can be -1 to use the default priority. </param>
		/// <param name="options"> The used execution options. </param>
		/// <param name="action"> The delegate. </param>
		/// <param name="parameters"> Optional parameters of the delagate. </param>
		/// <returns>
		///     The dispatcher operation object which can be used to track the execution of the enqueued delegate.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The higher the priority, the earlier the operation is executed (highest priority, first executed).
		///     </para>
		///     <note type="important">
		///         A delegate can be enqueued before the dispatcher is run.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than minus one. </exception>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is being shut down. </exception>
		ThreadDispatcherOperation Post (int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters);

		/// <summary>
		///     Enqueues a delegate to the dispatchers queue and does not wait for its execution.
		/// </summary>
		/// <param name="executionContext"> The context under which the delegate is executed. Can be null to use the executing threads context. </param>
		/// <param name="priority"> The priority. Can be -1 to use the default priority. </param>
		/// <param name="options"> The used execution options. </param>
		/// <param name="action"> The delegate. </param>
		/// <param name="parameters"> Optional parameters of the delagate. </param>
		/// <returns>
		///     The dispatcher operation object which can be used to track the execution of the enqueued delegate.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The higher the priority, the earlier the operation is executed (highest priority, first executed).
		///     </para>
		///     <note type="important">
		///         A delegate can be enqueued before the dispatcher is run.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than minus one. </exception>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is being shut down. </exception>
		ThreadDispatcherOperation Post (ThreadDispatcherExecutionContext executionContext, int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters);

		/// <summary>
		///     Removes an object from the list of objects which are kept alive at least as long as this thread dispatcher is running.
		/// </summary>
		/// <param name="obj"> The object to remove from the keep-alive list. </param>
		/// <returns>
		///     true if the thread dispatcher is running and the object was removed from the keep-alive list, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         <see cref="AddKeepAlive" /> for more details.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="obj" /> is null. </exception>
		bool RemoveKeepAlive (object obj);

		/// <summary>
		///     Enqueues a delegate to the dispatchers queue and waits for its execution to be completed.
		/// </summary>
		/// <param name="action"> The delegate. </param>
		/// <param name="parameters"> Optional parameters of the delagate. </param>
		/// <returns>
		///     The return value of the executed delegate or null if the delegate has no return value.
		/// </returns>
		/// <remarks>
		///     <remarks>
		///         <para>
		///             The delegate is enqueued with the default priority (<see cref="DefaultPriority" />).
		///         </para>
		///     </remarks>
		///     <para>
		///         <see cref="Send(Delegate,object[])" /> blocks until all previously enqueued delegates of higher or same priority were processed.
		///     </para>
		///     <para>
		///         <see cref="Send(Delegate,object[])" /> can be called from the dispatchers thread.
		///         Therefore, <see cref="Send(Delegate,object[])" /> calls can be cascaded.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
		/// <exception cref="ThreadDispatcherException"> An exception occurred during execution of the delegate. </exception>
		/// <exception cref="OperationCanceledException"> The execution of the delegate was canceled. </exception>
		object Send (Delegate action, params object[] parameters);

		/// <summary>
		///     Enqueues a delegate to the dispatchers queue and waits for its execution to be completed.
		/// </summary>
		/// <param name="priority"> The priority. Can be -1 to use the default priority. </param>
		/// <param name="action"> The delegate. </param>
		/// <param name="parameters"> Optional parameters of the delagate. </param>
		/// <returns>
		///     The return value of the executed delegate or null if the delegate has no return value.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The higher the priority, the earlier the operation is executed (highest priority, first executed).
		///     </para>
		///     <para>
		///         <see cref="Send(int,Delegate,object[])" /> blocks until all previously enqueued delegates of higher or same priority were processed.
		///     </para>
		///     <para>
		///         <see cref="Send(int,Delegate,object[])" /> can be called from the dispatchers thread.
		///         Therefore, <see cref="Send(int,Delegate,object[])" /> calls can be cascaded.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than minus one. </exception>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
		/// <exception cref="ThreadDispatcherException"> An exception occurred during execution of the delegate. </exception>
		/// <exception cref="OperationCanceledException"> The execution of the delegate was canceled. </exception>
		object Send (int priority, Delegate action, params object[] parameters);

		/// <summary>
		///     Enqueues a delegate to the dispatchers queue and waits for its execution to be completed.
		/// </summary>
		/// <param name="priority"> The priority. Can be -1 to use the default priority. </param>
		/// <param name="options"> The used execution options. </param>
		/// <param name="action"> The delegate. </param>
		/// <param name="parameters"> Optional parameters of the delagate. </param>
		/// <returns>
		///     The return value of the executed delegate or null if the delegate has no return value.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The higher the priority, the earlier the operation is executed (highest priority, first executed).
		///     </para>
		///     <para>
		///         <see cref="Send(int,ThreadDispatcherOptions,Delegate,object[])" /> blocks until all previously enqueued delegates of higher or same priority were processed.
		///     </para>
		///     <para>
		///         <see cref="Send(int,ThreadDispatcherOptions,Delegate,object[])" /> can be called from the dispatchers thread.
		///         Therefore, <see cref="Send(int,Delegate,object[])" /> calls can be cascaded.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than minus one. </exception>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
		/// <exception cref="ThreadDispatcherException"> An exception occurred during execution of the delegate. </exception>
		/// <exception cref="OperationCanceledException"> The execution of the delegate was canceled. </exception>
		object Send (int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters);

		/// <summary>
		///     Enqueues a delegate to the dispatchers queue and waits for its execution to be completed.
		/// </summary>
		/// <param name="action"> The delegate. </param>
		/// <param name="parameters"> Optional parameters of the delagate. </param>
		/// <returns>
		///     The return value of the executed delegate or null if the delegate has no return value.
		/// </returns>
		/// <remarks>
		///     <remarks>
		///         <para>
		///             The delegate is enqueued with the default priority (<see cref="DefaultPriority" />).
		///         </para>
		///     </remarks>
		///     <para>
		///         <see cref="Send(Delegate,object[])" /> blocks until all previously enqueued delegates of higher or same priority were processed.
		///     </para>
		///     <para>
		///         <see cref="Send(Delegate,object[])" /> can be called from the dispatchers thread.
		///         Therefore, <see cref="Send(Delegate,object[])" /> calls can be cascaded.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
		/// <exception cref="ThreadDispatcherException"> An exception occurred during execution of the delegate. </exception>
		/// <exception cref="OperationCanceledException"> The execution of the delegate was canceled. </exception>
		Task<object> SendAsync (Delegate action, params object[] parameters);

		/// <summary>
		///     Enqueues a delegate to the dispatchers queue and waits for its execution to be completed.
		/// </summary>
		/// <param name="priority"> The priority. Can be -1 to use the default priority. </param>
		/// <param name="action"> The delegate. </param>
		/// <param name="parameters"> Optional parameters of the delagate. </param>
		/// <returns>
		///     The return value of the executed delegate or null if the delegate has no return value.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The higher the priority, the earlier the operation is executed (highest priority, first executed).
		///     </para>
		///     <para>
		///         <see cref="Send(int,Delegate,object[])" /> blocks until all previously enqueued delegates of higher or same priority were processed.
		///     </para>
		///     <para>
		///         <see cref="Send(int,Delegate,object[])" /> can be called from the dispatchers thread.
		///         Therefore, <see cref="Send(int,Delegate,object[])" /> calls can be cascaded.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than minus one. </exception>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
		/// <exception cref="ThreadDispatcherException"> An exception occurred during execution of the delegate. </exception>
		/// <exception cref="OperationCanceledException"> The execution of the delegate was canceled. </exception>
		Task<object> SendAsync (int priority, Delegate action, params object[] parameters);

		/// <summary>
		///     Enqueues a delegate to the dispatchers queue and waits for its execution to be completed.
		/// </summary>
		/// <param name="priority"> The priority. Can be -1 to use the default priority. </param>
		/// <param name="options"> The used execution options. </param>
		/// <param name="action"> The delegate. </param>
		/// <param name="parameters"> Optional parameters of the delagate. </param>
		/// <returns>
		///     The return value of the executed delegate or null if the delegate has no return value.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The higher the priority, the earlier the operation is executed (highest priority, first executed).
		///     </para>
		///     <para>
		///         <see cref="Send(int,ThreadDispatcherOptions,Delegate,object[])" /> blocks until all previously enqueued delegates of higher or same priority were processed.
		///     </para>
		///     <para>
		///         <see cref="Send(int,ThreadDispatcherOptions,Delegate,object[])" /> can be called from the dispatchers thread.
		///         Therefore, <see cref="Send(int,Delegate,object[])" /> calls can be cascaded.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than minus one. </exception>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
		/// <exception cref="ThreadDispatcherException"> An exception occurred during execution of the delegate. </exception>
		/// <exception cref="OperationCanceledException"> The execution of the delegate was canceled. </exception>
		Task<object> SendAsync (int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters);

		/// <summary>
		///     Stops processing the delegate queue and waits for its shutdown.
		/// </summary>
		/// <param name="finishPendingDelegates"> Specifies whether already pending delegates should be processed before the dispatcher is shut down. </param>
		/// <remarks>
		///     <note type="note">
		///         <see cref="Shutdown" /> cannot be called from inside the dispatcher thread while the dispatcher is running.
		///         Use <see cref="BeginShutdown" /> from inside the dispatcher thread instead.
		///     </note>
		/// </remarks>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running, is already being shut down, or the method was called from the dispatcher thread. </exception>
		void Shutdown (bool finishPendingDelegates);

		/// <summary>
		///     Stops processing the delegate queue and waits for its shutdown
		/// </summary>
		/// <param name="finishPendingDelegates"> Specifies whether already pending delegates should be processed before the dispatcher is shut down. </param>
		/// <remarks>
		///     <note type="note">
		///         <see cref="Shutdown" /> cannot be called from inside the dispatcher thread while the dispatcher is running.
		///         Use <see cref="BeginShutdown" /> from inside the dispatcher thread instead.
		///     </note>
		/// </remarks>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running, is already being shut down, or the method was called from the dispatcher thread. </exception>
		Task ShutdownAsync (bool finishPendingDelegates);
	}
}
