using System;
using System.Collections.Generic;
using System.Threading;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Services.Messaging.Dispatchers
{
	/// <summary>
	///     Implements a message dispatcher which uses <see cref="ThreadPool"/>.
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
		/// <summary>
		///     Creates a new instance of <see cref="DefaultMessageDispatcher" />.
		/// </summary>
		public DefaultMessageDispatcher()
		{
			this.SyncRoot = new object();
		}

		#region Instance Properties/Indexer

		private object SyncRoot { get; }

		#endregion

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		object ISynchronizable.SyncRoot => this.SyncRoot;

		/// <inheritdoc />
		public void Post(List<IMessageReceiver> receivers, IMessage message, IMessageService messageService, Action<IMessage> deliveredCallback)
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

		private sealed class DispatchCapture
		{
			#region Instance Constructor/Destructor

			public DispatchCapture(Delegate action, params object[] arguments)
			{
				if (action == null)
				{
					throw new ArgumentNullException(nameof(action));
				}

				if (arguments == null)
				{
					throw new ArgumentNullException(nameof(arguments));
				}

				this.Action = action;
				this.Arguments = arguments;
				this.Context = SynchronizationContext.Current;
			}

			#endregion




			#region Instance Properties/Indexer

			public Delegate Action { get; }

			public object[] Arguments { get; }

			public SynchronizationContext Context { get; }

			#endregion




			#region Instance Methods

			public void Execute()
			{
				if (this.Context != null)
				{
					this.Context.Post(x =>
					{
						DispatchCapture capture = ((DispatchCapture)x);
						capture.Action.DynamicInvoke(capture.Arguments);
					}, this);
				}
				else
				{
					ThreadPool.QueueUserWorkItem(x =>
					{
						DispatchCapture capture = ((DispatchCapture)x);
						capture.Action.DynamicInvoke(capture.Arguments);
					}, this);
				}
			}

			#endregion
		}
	}
}
