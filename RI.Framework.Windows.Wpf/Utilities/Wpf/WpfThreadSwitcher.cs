using System;
using System.Windows.Threading;




namespace RI.Framework.Utilities.Wpf
{
	/// <summary>
	///     Provides utility/extension methods to switch to different threads and contextes related to WPF threading.
	/// </summary>
	public static class WpfThreadSwitcher
	{
		#region Static Methods

		/// <summary>
		///     Gets an awaiter to switch execution to a <see cref="Dispatcher" />.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher to switch to. </param>
		/// <returns>
		///     The awaiter.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
		public static DispatcherAwaiter ToSynchronizationContext (Dispatcher dispatcher)
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
