using System;
using System.Threading;

namespace RI.Framework.Utilities.Threading
{
	internal sealed class ThreadDispatcherSynchronizationContext : SynchronizationContext
	{
		public ThreadDispatcher Dispatcher { get; private set; }

		public ThreadDispatcherSynchronizationContext (ThreadDispatcher dispatcher)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			this.Dispatcher = dispatcher;
		}

		public override void Post (SendOrPostCallback d, object state)
		{
			this.Dispatcher.Post(new Action<SendOrPostCallback, object>((x, y) => x(y)), d, state);
		}

		public override void Send (SendOrPostCallback d, object state)
		{
			this.Dispatcher.Send(new Action<SendOrPostCallback, object>((x, y) => x(y)), d, state);
		}
	}
}
