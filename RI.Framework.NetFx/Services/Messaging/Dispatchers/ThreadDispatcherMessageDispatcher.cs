using System;
using System.Collections.Generic;

using RI.Framework.Composition.Model;
using RI.Framework.Threading.Dispatcher;
using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Services.Messaging.Dispatchers
{
	/// <summary>
	///     Implements a message dispatcher which uses <see cref="IThreadDispatcher" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IMessageDispatcher" /> for more details.
	///     </para>
	///     <para>
	///         Messages are dispatched using <see cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])" />.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public sealed class ThreadDispatcherMessageDispatcher : IMessageDispatcher
	{
		/// <summary>
		///     Creates a new instance of <see cref="ThreadDispatcherMessageDispatcher" />.
		/// </summary>
		/// <param name="threadDispatcher"> The dispatcher to use. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="threadDispatcher" /> is null. </exception>
		public ThreadDispatcherMessageDispatcher(IThreadDispatcher threadDispatcher)
		{
			if (threadDispatcher == null)
			{
				throw new ArgumentNullException(nameof(threadDispatcher));
			}

			this.SyncRoot = new object();
			this.ThreadDispatcher = threadDispatcher;

			this.Priority = null;
			this.Options = null;
		}

		#region Instance Fields

		private ThreadDispatcherOptions? _options;

		private int? _priority;

		private ThreadDispatcherOptions UsedOptions => this.Options.GetValueOrDefault(this.ThreadDispatcher.DefaultOptions);

		private int UsedPriority => this.Priority.GetValueOrDefault(this.ThreadDispatcher.DefaultPriority);

		#endregion

		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used dispatcher.
		/// </summary>
		/// <value>
		///     The used dispatcher.
		/// </value>
		public IThreadDispatcher ThreadDispatcher { get; }

		/// <summary>
		///     Gets or sets the options used for dispatching messages.
		/// </summary>
		/// <value>
		///     The options used for dispatching messages or null if the default options of the used dispatcher should be used (<see cref="IThreadDispatcher.DefaultOptions" />).
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is null.
		///     </para>
		/// </remarks>
		public ThreadDispatcherOptions? Options
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._options;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._options = value;
				}
			}
		}

		/// <summary>
		///     Gets or sets the priority used for dispatching messages.
		/// </summary>
		/// <value>
		///     The priority used for dispatching messages or null if the default priority of the used dispatcher should be used (<see cref="IThreadDispatcher.DefaultPriority" />).
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is null.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="value" /> is less than zero. </exception>
		public int? Priority
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._priority;
				}
			}
			set
			{
				if (value.HasValue)
				{
					if (value.Value < 0)
					{
						throw new ArgumentOutOfRangeException(nameof(value));
					}
				}

				lock (this.SyncRoot)
				{
					this._priority = value;
				}
			}
		}

		private object SyncRoot { get; }

		#endregion

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		object ISynchronizable.SyncRoot => this.SyncRoot;

		/// <inheritdoc />
		public void Post(IEnumerable<IMessageReceiver> receivers, IMessage message, IMessageService messageService, Action<IMessage> deliveredCallback)
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
				this.ThreadDispatcher.Post(this.UsedPriority, this.UsedOptions, new Action<IEnumerable<IMessageReceiver>, IMessage, IMessageService, Action<IMessage>>((r, m, s, c) =>
				{
					foreach (IMessageReceiver receiver in r)
					{
						receiver.ReceiveMessage(m, s);
					}
					c?.Invoke(m);
				}), receivers, message, messageService, deliveredCallback);
			}
		}
	}
}
