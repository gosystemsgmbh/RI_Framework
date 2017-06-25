using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Messaging.Dispatchers
{
	/// <summary>
	///     Implements a message dispatcher which uses a WPF application dispatcher instance.
	/// </summary>
	/// <remarks>
	///     <para>
	///         All messages are dispatched using <see cref="DispatcherPriority.Normal" /> priority.
	///     </para>
	///     <para>
	///         See <see cref="IMessageDispatcher" /> for more details.
	///     </para>
	/// </remarks>
	[Export]
	public sealed class WpfMessageDispatcher : IMessageDispatcher
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="WpfMessageDispatcher" />.
		/// </summary>
		/// <param name="application"> The application object to get the dispatcher from. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="application" /> is null. </exception>
		public WpfMessageDispatcher (Application application)
		{
			if (application == null)
			{
				throw new ArgumentNullException(nameof(application));
			}

			this.SyncRoot = new object();
			this.Dispatcher = application.Dispatcher;
		}

		/// <summary>
		///     Creates a new instance of <see cref="WpfMessageDispatcher" />.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher to use. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
		public WpfMessageDispatcher (Dispatcher dispatcher)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			this.SyncRoot = new object();
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
		public Dispatcher Dispatcher { get; private set; }

		#endregion




		#region Interface: IMessageDispatcher

		/// <inheritdoc />
		public void Post (IEnumerable<IMessageReceiver> receivers, IMessage message, IMessageService messageService)
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
				this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action<IEnumerable<IMessageReceiver>, IMessage, IMessageService>((a, b, s) =>
				{
					foreach (IMessageReceiver receiver in a)
					{
						receiver.ReceiveMessage(b, s);
					}
				}), receivers, message, messageService);
			}
		}

		#endregion


		private object SyncRoot { get; set; }

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		object ISynchronizable.SyncRoot => this.SyncRoot;
	}
}
