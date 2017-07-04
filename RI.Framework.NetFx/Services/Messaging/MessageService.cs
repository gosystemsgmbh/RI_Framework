using System;
using System.Collections.Generic;

using RI.Framework.Collections;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition;
using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging;
using RI.Framework.Services.Messaging.Dispatchers;




namespace RI.Framework.Services.Messaging
{
	/// <summary>
	///     Implements a default messaging service which is suitable for most scenarios.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This message service manages <see cref="IMessageDispatcher" />s  and <see cref="IMessageReceiver" />s from two sources.
	///         One are the explicitly specified dispatchers and receivers added through <see cref="AddDispatcher" /> and <see cref="AddReceiver" />.
	///         The second is a <see cref="CompositionContainer" /> if this <see cref="MessageService" /> is added as an export (the dispatchers and receivers are then imported through composition).
	///         <see cref="Dispatchers" /> gives the sequence containing all message dispatchers from all sources and <see cref="Receivers" /> gives the sequence containing all message receivers from all sources.
	///     </para>
	///     <para>
	///         See <see cref="IMessageService" /> for more details.
	///     </para>
	/// </remarks>
	[Export]
	public sealed class MessageService : IMessageService, IImporting, ILogSource
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="MessageService" />.
		/// </summary>
		public MessageService ()
		{
			this.SendSyncRoot = new object();

			this.DispatchersUpdated = new List<IMessageDispatcher>();
			this.ReceiversUpdated = new List<IMessageReceiver>();

			this.DispatchersManual = new List<IMessageDispatcher>();
			this.ReceiversManual = new List<IMessageReceiver>();
		}

		#endregion




		#region Instance Properties/Indexer

		[Import(typeof(IMessageDispatcher), Recomposable = true)]
		private Import DispatchersImported { get; set; }

		private List<IMessageDispatcher> DispatchersManual { get; }

		private List<IMessageDispatcher> DispatchersUpdated { get; set; }

		[Import(typeof(IMessageReceiver), Recomposable = true)]
		private Import ReceiversImported { get; set; }

		private List<IMessageReceiver> ReceiversManual { get; }

		private List<IMessageReceiver> ReceiversUpdated { get; set; }

		private object SendSyncRoot { get; }

		#endregion




		#region Instance Methods

		private void UpdateDispatchers ()
		{
			this.Log(LogLevel.Debug, "Updating dispatchers");

			HashSet<IMessageDispatcher> currentDispatchers = new HashSet<IMessageDispatcher>(this.Dispatchers);
			HashSet<IMessageDispatcher> lastDispatchers = new HashSet<IMessageDispatcher>(this.DispatchersUpdated);

			HashSet<IMessageDispatcher> newDispatchers = currentDispatchers.Except(lastDispatchers);
			HashSet<IMessageDispatcher> oldDispatchers = lastDispatchers.Except(currentDispatchers);

			this.DispatchersUpdated.Clear();
			this.DispatchersUpdated.AddRange(currentDispatchers);

			foreach (IMessageDispatcher dispatcher in newDispatchers)
			{
				this.Log(LogLevel.Debug, "Dispatcher added: {0}", dispatcher.GetType().Name);
			}

			foreach (IMessageDispatcher dispatcher in oldDispatchers)
			{
				this.Log(LogLevel.Debug, "Dispatcher removed: {0}", dispatcher.GetType().Name);
			}
		}

		private void UpdateReceivers ()
		{
			this.Log(LogLevel.Debug, "Updating receivers");

			HashSet<IMessageReceiver> currentReceivers = new HashSet<IMessageReceiver>(this.Receivers);
			HashSet<IMessageReceiver> lastReceivers = new HashSet<IMessageReceiver>(this.ReceiversUpdated);

			HashSet<IMessageReceiver> newReceivers = currentReceivers.Except(lastReceivers);
			HashSet<IMessageReceiver> oldReceivers = lastReceivers.Except(currentReceivers);

			this.ReceiversUpdated.Clear();
			this.ReceiversUpdated.AddRange(currentReceivers);

			foreach (IMessageReceiver receiver in newReceivers)
			{
				this.Log(LogLevel.Debug, "Receiver added: {0}", receiver.GetType().Name);
			}

			foreach (IMessageReceiver receiver in oldReceivers)
			{
				this.Log(LogLevel.Debug, "Receiver removed: {0}", receiver.GetType().Name);
			}
		}

		#endregion




		#region Interface: IImporting

		/// <inheritdoc />
		void IImporting.ImportsResolved (CompositionFlags composition, bool updated)
		{
			if (updated)
			{
				this.UpdateDispatchers();
				this.UpdateReceivers();
			}
		}

		/// <inheritdoc />
		void IImporting.ImportsResolving (CompositionFlags composition)
		{
		}

		#endregion




		#region Interface: IMessageService

		/// <inheritdoc />
		public IEnumerable<IMessageDispatcher> Dispatchers
		{
			get
			{
				foreach (IMessageDispatcher dispatcher in this.DispatchersManual)
				{
					yield return dispatcher;
				}

				foreach (IMessageDispatcher dispatcher in this.DispatchersImported.Values<IMessageDispatcher>())
				{
					yield return dispatcher;
				}
			}
		}

		/// <inheritdoc />
		public IEnumerable<IMessageReceiver> Receivers
		{
			get
			{
				foreach (IMessageReceiver receiver in this.ReceiversManual)
				{
					yield return receiver;
				}

				foreach (IMessageReceiver receiver in this.ReceiversImported.Values<IMessageReceiver>())
				{
					yield return receiver;
				}
			}
		}

		/// <inheritdoc />
		public void AddDispatcher (IMessageDispatcher messageDispatcher)
		{
			if (messageDispatcher == null)
			{
				throw new ArgumentNullException(nameof(messageDispatcher));
			}

			if (this.DispatchersManual.Contains(messageDispatcher))
			{
				return;
			}

			this.DispatchersManual.Add(messageDispatcher);

			this.UpdateDispatchers();
		}

		/// <inheritdoc />
		public void AddReceiver (IMessageReceiver messageReceiver)
		{
			if (messageReceiver == null)
			{
				throw new ArgumentNullException(nameof(messageReceiver));
			}

			if (this.ReceiversManual.Contains(messageReceiver))
			{
				return;
			}

			this.ReceiversManual.Add(messageReceiver);

			this.UpdateReceivers();
		}

		/// <inheritdoc />
		public void Post (IMessage message)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			lock (this.SendSyncRoot)
			{
				foreach (IMessageDispatcher dispatcher in this.Dispatchers)
				{
					dispatcher.Post(this.Receivers, message, this);
				}
			}
		}

		/// <inheritdoc />
		public void RemoveDispatcher (IMessageDispatcher messageDispatcher)
		{
			if (messageDispatcher == null)
			{
				throw new ArgumentNullException(nameof(messageDispatcher));
			}

			if (!this.DispatchersManual.Contains(messageDispatcher))
			{
				return;
			}

			this.DispatchersManual.RemoveAll(messageDispatcher);

			this.UpdateDispatchers();
		}

		/// <inheritdoc />
		public void RemoveReceiver (IMessageReceiver messageReceiver)
		{
			if (messageReceiver == null)
			{
				throw new ArgumentNullException(nameof(messageReceiver));
			}

			if (!this.ReceiversManual.Contains(messageReceiver))
			{
				return;
			}

			this.ReceiversManual.RemoveAll(messageReceiver);

			this.UpdateReceivers();
		}

		#endregion
	}
}
