using System;
using System.Threading;

namespace RI.Framework.Threading
{
	/// <summary>
	///     Implements an awaiter which continues on the <see cref="ThreadPool" />.
	/// </summary>
	public sealed class ThreadPoolAwaiter : CustomAwaiter
	{
		#region Instance Constructor/Destructor

		/// <summary>
		/// Creates a new instance of <see cref="ThreadPoolAwaiter"/>.
		/// </summary>
		public ThreadPoolAwaiter ()
		{
		}

		#endregion


		/// <summary>
		/// Gets a default instance of <see cref="ThreadPoolAwaiter"/> which can be used with async/await.
		/// </summary>
		/// <value>
		/// A default instance of <see cref="ThreadPoolAwaiter"/>.
		/// </value>
		public static ThreadPoolAwaiter SwitchTo => new ThreadPoolAwaiter();




		#region Interface: INotifyCompletion

		/// <inheritdoc />
		public override void OnCompleted (Action continuation)
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
