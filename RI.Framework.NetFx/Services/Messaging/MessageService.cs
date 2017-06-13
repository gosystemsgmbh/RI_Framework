using System;
using System.Collections.Generic;

using RI.Framework.Collections;
using RI.Framework.Composition;
using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging;




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
	public sealed class MessageService : IMessageService, ILogSource
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="MessageService" />.
		/// </summary>
		public MessageService ()
		{
			this.DispatchersManual = new List<IMessageDispatcher>();
			this.ReceiversManual = new List<IMessageReceiver>();
		}

		#endregion




		#region Instance Properties/Indexer

		[Import(typeof(IMessageDispatcher), Recomposable = true)]
		private Import DispatchersImported { get; set; }

		private List<IMessageDispatcher> DispatchersManual { get; set; }

		[Import(typeof(IMessageReceiver), Recomposable = true)]
		private Import ReceiversImported { get; set; }

		private List<IMessageReceiver> ReceiversManual { get; set; }

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
		}

		/// <inheritdoc />
		public void Post (IMessage message)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			foreach (IMessageDispatcher dispatcher in this.Dispatchers)
			{
				dispatcher.Post(this.Receivers, message, this);
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
		}

		/// <inheritdoc />
		public void Send (IMessage message)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			foreach (IMessageDispatcher dispatcher in this.Dispatchers)
			{
				dispatcher.Send(this.Receivers, message, this);
			}
		}

		#endregion
	}
}
