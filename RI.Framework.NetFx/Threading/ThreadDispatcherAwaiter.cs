using System;
using System.Runtime.CompilerServices;




namespace RI.Framework.Threading
{
	/// <summary>
	///     Implements an awaiter which continues on a specified <see cref="IThreadDispatcher" />.
	/// </summary>
	public sealed class ThreadDispatcherAwaiter : INotifyCompletion
	{
		#region Instance Constructor/Destructor

		internal ThreadDispatcherAwaiter (IThreadDispatcher dispatcher)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			this.Dispatcher = dispatcher;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used <see cref="IThreadDispatcher" />.
		/// </summary>
		/// <value>
		///     The used <see cref="IThreadDispatcher" />.
		/// </value>
		public IThreadDispatcher Dispatcher { get; }

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
		///     Schedules the continuation action on the dispatcher.
		/// </summary>
		/// <param name="continuation"> The continuation action. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="continuation" /> is null. </exception>
		public void OnCompleted (Action continuation)
		{
			if (continuation == null)
			{
				throw new ArgumentNullException(nameof(continuation));
			}

			this.Dispatcher.Post(continuation);
		}

		#endregion
	}
}
