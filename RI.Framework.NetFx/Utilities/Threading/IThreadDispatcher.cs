using System;
using System.Threading.Tasks;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Utilities.Threading
{
	/// <summary>
	///     Defines the interface for thread-bound dispatchers.
	/// </summary>
	public interface IThreadDispatcher : ISynchronizable
	{
		/// <summary>
		///     Gets or sets whether exceptions, thrown when executing delegates, are catched and the dispatcher continues its operations.
		/// </summary>
		/// <value>
		///     true if exceptions are catched, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is false.
		///     </para>
		///     <para>
		///         The <see cref="Exception" /> event is raised regardless of the value of <see cref="CatchExceptions" />.
		///     </para>
		/// </remarks>
		bool CatchExceptions { get; set; }

		/// <summary>
		///     Gets or sets the default priority.
		/// </summary>
		/// <value>
		///     The default priority.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The default value, if not explicitly set, is expected to be half of <see cref="int.MaxValue" />.
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
		/// </value>
		bool IsShuttingDown { get; }

		/// <summary>
		///     Gets the active shutdown mode.
		/// </summary>
		/// <value>
		///     <see cref="ThreadDispatcherShutdownMode.None" /> if the dispatcher is not running or is not being shut down, <see cref="ThreadDispatcherShutdownMode.FinishPending" /> if the dispatcher is shutting down and all already pending delegates are processed before the shutdown completes, <see cref="ThreadDispatcherShutdownMode.DiscardPending" /> if the dispatcher is shutting down and all already pending delegates are discarded.
		/// </value>
		ThreadDispatcherShutdownMode ShutdownMode { get; }

		/// <summary>
		///     Raised when an exception occurred during execution of a enqueued delegate.
		/// </summary>
		/// <remarks>
		///     <note type="important">
		///         The event is raised from the thread <see cref="IThreadDispatcher" /> runs in.
		///     </note>
		/// </remarks>
		event EventHandler<ThreadDispatcherExceptionEventArgs> Exception;

		/// <summary>
		///     Waits until all queued operations have been processed.
		/// </summary>
		void DoProcessing ();

		/// <summary>
		///     Waits until all queued operations have been processed.
		/// </summary>
		Task DoProcessingAsync ();

		/// <summary>
		///     Determines whether the caller of this function is executed inside the dispatchers thread or not.
		/// </summary>
		/// <returns>
		///     true if the caller of this function is executed inside this thread, false otherwise or if the dispatcher is not running.
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
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
		ThreadDispatcherOperation Post (Delegate action, params object[] parameters);

		/// <summary>
		///     Enqueues a delegate to the dispatchers queue and does not wait for its execution.
		/// </summary>
		/// <param name="priority"> The priority. </param>
		/// <param name="action"> The delegate. </param>
		/// <param name="parameters"> Optional parameters of the delagate. </param>
		/// <returns>
		///     The dispatcher operation object which can be used to track the execution of the enqueued delegate.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The higher the priority, the earlier the operation is executed (highest priority, first executed).
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than zero. </exception>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
		ThreadDispatcherOperation Post (int priority, Delegate action, params object[] parameters);

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
		///         <see cref="Send(Delegate,object[])" /> blocks until all previously enqueued delegates were processed.
		///     </para>
		///     <para>
		///         <see cref="Send(Delegate,object[])" /> can be called from the dispatchers thread.
		///         Therefore, <see cref="Send(Delegate,object[])" /> calls can be cascaded.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
		object Send (Delegate action, params object[] parameters);

		/// <summary>
		///     Enqueues a delegate to the dispatchers queue and waits for its execution to be completed.
		/// </summary>
		/// <param name="priority"> The priority. </param>
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
		///         <see cref="Send(int,Delegate,object[])" /> blocks until all previously enqueued delegates were processed.
		///     </para>
		///     <para>
		///         <see cref="Send(int,Delegate,object[])" /> can be called from the dispatchers thread.
		///         Therefore, <see cref="Send(int,Delegate,object[])" /> calls can be cascaded.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than zero. </exception>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
		object Send (int priority, Delegate action, params object[] parameters);

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
		///         <see cref="Send(Delegate,object[])" /> blocks until all previously enqueued delegates were processed.
		///     </para>
		///     <para>
		///         <see cref="Send(Delegate,object[])" /> can be called from the dispatchers thread.
		///         Therefore, <see cref="Send(Delegate,object[])" /> calls can be cascaded.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
		Task<object> SendAsync (Delegate action, params object[] parameters);

		/// <summary>
		///     Enqueues a delegate to the dispatchers queue and waits for its execution to be completed.
		/// </summary>
		/// <param name="priority"> The priority. </param>
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
		///         <see cref="Send(int,Delegate,object[])" /> blocks until all previously enqueued delegates were processed.
		///     </para>
		///     <para>
		///         <see cref="Send(int,Delegate,object[])" /> can be called from the dispatchers thread.
		///         Therefore, <see cref="Send(int,Delegate,object[])" /> calls can be cascaded.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than zero. </exception>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running or is being shut down. </exception>
		Task<object> SendAsync (int priority, Delegate action, params object[] parameters);

		/// <summary>
		///     Stops processing the delegate queue.
		/// </summary>
		/// <param name="finishPendingDelegates"> Specifies whether already pending delegates should be processed before the dispatcher is shut down. </param>
		/// <exception cref="InvalidOperationException"> The dispatcher is not running or it is already being shut down. </exception>
		void Shutdown (bool finishPendingDelegates);

		/// <summary>
		/// Determines under which priority the current code is executed.
		/// </summary>
		/// <returns>
		/// The priority of the currently executed code or null if the current code is not executed by the dispatcher.
		/// </returns>
		int? GetCurrentPriority ();
	}
}
