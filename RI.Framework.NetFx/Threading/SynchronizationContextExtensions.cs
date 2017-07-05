using System;
using System.Threading;

namespace RI.Framework.Threading
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="SynchronizationContext" /> type.
	/// </summary>
	/// <threadsafety static="true" instance="true" />
	public static class SynchronizationContextExtensions
	{
		/// <summary>
		/// Creates an awaiter for a synchronization context.
		/// </summary>
		/// <param name="synchronizationContext">The synchronization context to use in the awaiter.</param>
		/// <returns>
		/// The created awaiter.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="synchronizationContext" /> is null. </exception>
		public static SynchronizationContextAwaiter GetAwaiter(this SynchronizationContext synchronizationContext)
		{
			if (synchronizationContext == null)
			{
				throw new ArgumentNullException(nameof(synchronizationContext));
			}

			return new SynchronizationContextAwaiter(synchronizationContext);
		}
	}
}
