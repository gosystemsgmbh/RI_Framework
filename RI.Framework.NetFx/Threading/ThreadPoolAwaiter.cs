using System;
using System.Runtime.CompilerServices;
using System.Threading;




namespace RI.Framework.Threading
{
	/// <summary>
	///     Implements an awaiter which continues on the <see cref="ThreadPool" />.
	/// </summary>
	public sealed class ThreadPoolAwaiter : INotifyCompletion
	{
		#region Instance Constructor/Destructor

		internal ThreadPoolAwaiter ()
		{
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets whether the continuation action has already completed.
		/// </summary>
		/// <value>
		///     true if the continuation action has already completed and does not need to be scheduled, false otherwise.
		/// </value>
		public bool IsCompleted => false;

		#endregion




		#region Instance Methods

		/// <summary>
		///     Gets the result of the scheduled continuation action.
		/// </summary>
		public void GetResult ()
		{
		}

		#endregion




		#region Interface: INotifyCompletion

		/// <summary>
		///     Schedules the continuation action on the thread pool.
		/// </summary>
		/// <param name="continuation"> The continuation action. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="continuation" /> is null. </exception>
		public void OnCompleted (Action continuation)
		{
			if (continuation == null)
			{
				throw new ArgumentNullException(nameof(continuation));
			}

			ThreadPool.QueueUserWorkItem(x =>
			{
				Action cont = (Action)x;
				cont();
			}, continuation);
		}

		#endregion
	}
}
