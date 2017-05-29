using System;
using System.Threading;




namespace RI.Framework.Utilities.Threading
{
	internal sealed class ThreadDispatcherSynchronizationContext : SynchronizationContext
	{
		#region Instance Constructor/Destructor

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

		public IThreadDispatcher Dispatcher { get; private set; }

		#endregion




		#region Overrides

		public override void Post (SendOrPostCallback d, object state)
		{
			this.Dispatcher.Post(new Action<SendOrPostCallback, object>((x, y) => x(y)), d, state);
		}

		public override void Send (SendOrPostCallback d, object state)
		{
			this.Dispatcher.Send(new Action<SendOrPostCallback, object>((x, y) => x(y)), d, state);
		}

		#endregion
	}
}
