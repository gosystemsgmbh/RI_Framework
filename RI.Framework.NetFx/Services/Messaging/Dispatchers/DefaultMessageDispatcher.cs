using System;
using System.Collections.Generic;
using System.Threading;

using RI.Framework.Composition.Model;
using RI.Framework.Threading;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Messaging.Dispatchers
{
	/// <summary>
	///     Implements a message dispatcher which uses <see cref="ThreadPool" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IMessageDispatcher" /> for more details.
	///     </para>
	///     <para>
	///         <see cref="DefaultMessageDispatcher" /> uses <see cref="SynchronizationContext" />, which is captured at the time of dispatching, to dispatch messages or falls back to <see cref="ThreadPool.QueueUserWorkItem(WaitCallback,object)" /> if no <see cref="SynchronizationContext" /> is available.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public sealed class DefaultMessageDispatcher : IMessageDispatcher
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DefaultMessageDispatcher" />.
		/// </summary>
		public DefaultMessageDispatcher ()
		{
			this.SyncRoot = new object();
		}

		#endregion




		#region Instance Properties/Indexer

		private object SyncRoot { get; }

		#endregion




		#region Interface: IMessageDispatcher

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		object ISynchronizable.SyncRoot => this.SyncRoot;

		/// <inheritdoc />
		public void Post (List<IMessageReceiver> receivers, IMessage message, IMessageService messageService, Action<IMessage> deliveredCallback)
		{
			if (receivers == null)
			{
				throw new ArgumentNullException(nameof(receivers));
			}

			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			lock (this.SyncRoot)
			{
				DispatchCapture capture = new DispatchCapture(new Action<List<IMessageReceiver>, IMessage, IMessageService, Action<IMessage>>((r, m, s, c) =>
				{
					foreach (IMessageReceiver receiver in r)
					{
						receiver.ReceiveMessage(m, s);
					}
					c?.Invoke(m);
				}), receivers, message, messageService, deliveredCallback);

				capture.Execute();
			}
		}

		#endregion
	}
}
