using System;
using System.Threading;




namespace RI.Framework.Threading.Dispatcher
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
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
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
		public IThreadDispatcher Dispatcher { get; }

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
			this.Dispatcher.Post(this.Dispatcher.GetCurrentPriorityOrDefault(), this.Dispatcher.GetCurrentOptionsOrDefault(), new Action<SendOrPostCallback, object>((x, y) => x(y)), d, state);
		}

		/// <inheritdoc />
		public override void Send (SendOrPostCallback d, object state)
		{
			this.Dispatcher.Send(this.Dispatcher.GetCurrentPriorityOrDefault(), this.Dispatcher.GetCurrentOptionsOrDefault(), new Action<SendOrPostCallback, object>((x, y) => x(y)), d, state);
		}

		#endregion
	}
}
