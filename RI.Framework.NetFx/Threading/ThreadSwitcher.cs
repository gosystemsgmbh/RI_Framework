using System;
using System.Threading;




namespace RI.Framework.Threading
{
	/// <summary>
	///     Provides utility/extension methods to switch to different threads and contextes.
	/// </summary>
	public static class ThreadSwitcher
	{
		#region Static Methods

		/// <summary>
		///     Gets an awaiter to switch execution to a <see cref="SynchronizationContext" />.
		/// </summary>
		/// <param name="synchronizationContext"> The synchronization context to switch to. </param>
		/// <returns>
		///     The awaiter.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="synchronizationContext" /> is null. </exception>
		public static SynchronizationContextAwaiter ToSynchronizationContext (SynchronizationContext synchronizationContext)
		{
			if (synchronizationContext == null)
			{
				throw new ArgumentNullException(nameof(synchronizationContext));
			}

			return new SynchronizationContextAwaiter(synchronizationContext);
		}

		/// <summary>
		///     Gets an awaiter to switch execution to a <see cref="IThreadDispatcher" />.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher to switch to. </param>
		/// <returns>
		///     The awaiter.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
		public static ThreadDispatcherAwaiter ToThreadDispatcher (IThreadDispatcher dispatcher)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			return new ThreadDispatcherAwaiter(dispatcher);
		}

		/// <summary>
		///     Gets an awaiter to switch execution to the <see cref="ThreadPool" />.
		/// </summary>
		/// <returns>
		///     The awaiter.
		/// </returns>
		public static ThreadPoolAwaiter ToThreadPool ()
		{
			return new ThreadPoolAwaiter();
		}

		#endregion
	}
}
