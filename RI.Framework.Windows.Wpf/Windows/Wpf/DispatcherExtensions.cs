using System;
using System.Threading.Tasks;
using System.Windows.Threading;

using RI.Framework.Threading.Async;




namespace RI.Framework.Windows.Wpf
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="Dispatcher" /> type.
	/// </summary>
	/// <threadsafety static="true" instance="true" />
	public static class DispatcherExtensions
	{
		#region Static Methods

		/// <summary>
		///     Forces the dispatcher to process all its queued operations.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher. </param>
		/// <remarks>
		///     <para>
		///         The method does not return until all operations are processed.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
		public static void DoAllEvents (this Dispatcher dispatcher)
		{
			dispatcher.DoEvents(DispatcherPriority.SystemIdle);
		}

		/// <summary>
		///     Forces the dispatcher to process all its queued operations.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher. </param>
		/// <returns>
		///     The task which can be used to await the end of processing all ist queued operations.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The method does not return until all operations are processed.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
		public static async Task DoAllEventsAsync (this Dispatcher dispatcher)
		{
			await dispatcher.DoEventsAsync(DispatcherPriority.SystemIdle).ConfigureAwait(false);
		}

		/// <summary>
		///     Forces the dispatcher to process all its queued operations up to and including the specified priority.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher. </param>
		/// <param name="priority"> The priority up to and including all operations are to be processed. </param>
		/// <remarks>
		///     <para>
		///         The method does not return until all operations as specified are processed.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
		public static void DoEvents (this Dispatcher dispatcher, DispatcherPriority priority)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			dispatcher.Invoke(() => { }, priority);
		}

		/// <summary>
		///     Forces the dispatcher to process all its queued operations up to and including the specified priority.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher. </param>
		/// <param name="priority"> The priority up to and including all operations are to be processed. </param>
		/// <returns>
		///     The task which can be used to await the end of processing all ist queued operations up to and including the specified priority.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The method does not return until all operations as specified are processed.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
		public static async Task DoEventsAsync (this Dispatcher dispatcher, DispatcherPriority priority)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			DispatcherOperation operation = dispatcher.InvokeAsync(() => { }, priority);
			await operation.Task.ConfigureAwait(false);
		}

		/// <summary>
		///     Creates an awaiter for a dispatcher.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher to use in the awaiter. </param>
		/// <returns>
		///     The created awaiter.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
		public static DispatcherAwaiter GetAwaiter (this Dispatcher dispatcher)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			return new DispatcherAwaiter(dispatcher);
		}

		#endregion
	}
}
