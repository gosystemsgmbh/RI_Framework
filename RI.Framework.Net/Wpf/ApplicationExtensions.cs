using System;
using System.Windows;
using System.Windows.Threading;




namespace RI.Framework.Wpf
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="Application" /> type.
	/// </summary>
	public static class ApplicationExtensions
	{
		#region Static Methods

		/// <summary>
		///     Forces the application to process all its queued operations.
		/// </summary>
		/// <param name="application"> The application. </param>
		/// <remarks>
		///     <para>
		///         The method does not return until all operations are processed.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="application" /> is null. </exception>
		public static void DoAllEvents (this Application application)
		{
			if (application == null)
			{
				throw new ArgumentNullException(nameof(application));
			}

			application.Dispatcher.DoAllEvents();
		}

		/// <summary>
		///     Forces the application to process all its queued operations up to and including the specified priority.
		/// </summary>
		/// <param name="application"> The application. </param>
		/// <param name="priority"> The priority up to and including all operations are to be processed. </param>
		/// <remarks>
		///     <para>
		///         The method does not return until all operations as specified are processed.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="application" /> is null. </exception>
		public static void DoEvents (this Application application, DispatcherPriority priority)
		{
			if (application == null)
			{
				throw new ArgumentNullException(nameof(application));
			}

			application.Dispatcher.DoEvents(priority);
		}

		#endregion
	}
}
