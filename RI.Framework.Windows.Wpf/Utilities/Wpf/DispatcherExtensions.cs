using System;
using System.Windows.Threading;




namespace RI.Framework.Utilities.Wpf
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="Dispatcher" /> type.
	/// </summary>
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

		#endregion
	}
}
