using System;
using System.Threading;




namespace RI.Framework.Utilities.Threading
{
	/// <summary>
	///     Implements a <see cref="SynchronizationContext" /> which uses a <see cref="IThreadDispatcher" /> for execution.
	/// </summary>
	public sealed class ThreadDispatcherSynchronizationContext : SynchronizationContext
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ThreadDispatcherSynchronizationContext" />.
		/// </summary>
		/// <param name="dispatcher"> The used dispatcher. </param>
		public ThreadDispatcherSynchronizationContext (IThreadDispatcher dispatcher)
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
		///     Gets the used dispatcher.
		/// </summary>
		/// <value>
		///     The used dispatcher.
		/// </value>
		public IThreadDispatcher Dispatcher { get; private set; }

		#endregion




		#region Overrides

		/// <inheritdoc />
		public override SynchronizationContext CreateCopy ()
		{
			return new ThreadDispatcherSynchronizationContext(this.Dispatcher);
		}

		/// <inheritdoc />
		public override void Post (SendOrPostCallback d, object state)
		{
			this.Dispatcher.Post(new Action<SendOrPostCallback, object>((x, y) => x(y)), d, state);
		}

		/// <inheritdoc />
		public override void Send (SendOrPostCallback d, object state)
		{
			this.Dispatcher.Send(new Action<SendOrPostCallback, object>((x, y) => x(y)), d, state);
		}

		#endregion
	}
}
