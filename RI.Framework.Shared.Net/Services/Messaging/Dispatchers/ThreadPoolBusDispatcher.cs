using System;
using System.Collections.Generic;
using System.Threading;

using RI.Framework.Composition.Model;
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
	///         Bus operations are dispatched using <see cref="ThreadPool.QueueUserWorkItem(WaitCallback)" />.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	[Obsolete("The message service is obsolete. Use the message bus instead (RI.Framework.Bus.*).", false)]
	public sealed class ThreadPoolMessageDispatcher : IMessageDispatcher
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ThreadPoolMessageDispatcher" />.
		/// </summary>
		public ThreadPoolMessageDispatcher ()
		{
			this.SyncRoot = new object();
		}

		#endregion




		#region Interface: IMessageDispatcher

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; }

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
				ThreadPool.QueueUserWorkItem(_ =>
				{
					foreach (IMessageReceiver receiver in receivers)
					{
						receiver.ReceiveMessage(message, messageService);
					}
					deliveredCallback?.Invoke(message);
				});
			}
		}

		#endregion
	}
}
