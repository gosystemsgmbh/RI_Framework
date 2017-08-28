using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using RI.Framework.Collections;
using RI.Framework.Composition.Model;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Services.Messaging.Handlers.Triggers
{
	/// <summary>
	///     Implements a message handler which provides trigger functionality.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Trigger functionality is implemented using requests to <see cref="TriggerMessageHandler" /> and responses from <see cref="TriggerMessageHandler" />.
	///     </para>
	///     <para>
	///         The following requests can be made by sending a message to <see cref="TriggerMessageHandler" />: Subscribe, Unsubscribe, Arm, Disarm.
	///         For each request which changes a trigger, <see cref="TriggerMessageHandler" /> sends a response (&quot;Trigger Changed&quot;).
	///     </para>
	///     <para>
	///         Modules or other part of an application can subscribe to and unsubscribe from a trigger as well as arm or disarm a trigger.
	///         A trigger is either untriggered (none of its subscribers armed the trigger), OR-triggered (one or more but not all subscribers armed the trigger), or AND-triggered (all subscribers armed the trigger).
	///     </para>
	///     <para>
	///         See <see cref="TriggerMessageNames" /> and <see cref="TriggerMessageExtensions" /> for more details about the names of the messages and the trigger message data.
	///     </para>
	/// <para>
	/// Trigger message names and trigger names are threated case-insensitive.
	/// </para>
	///     <note type="note">
	///         A request which does not change a trigger will not cause a response.
	///     </note>
	/// </remarks>
	[Export]
	public sealed class TriggerMessageHandler : LogSource, IMessageReceiver
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="TriggerMessageHandler" />.
		/// </summary>
		public TriggerMessageHandler ()
		{
			this.Triggers = new TriggerCollection();
			this.ChangedTriggers = new HashSet<string>(TriggerMessageHandler.TriggerNameComparer);
		}

		internal static readonly StringComparerEx TriggerNameComparer = StringComparerEx.OrdinalIgnoreCase;

		internal static readonly StringComparison TriggerNameComparison = StringComparison.OrdinalIgnoreCase;

		#endregion




		#region Instance Properties/Indexer

		private HashSet<string> ChangedTriggers { get; set; }

		private TriggerCollection Triggers { get; set; }

		#endregion




		#region Instance Methods

		private Trigger[] CheckTriggersChanged ()
		{
			Trigger[] changedTriggers = (from x in this.ChangedTriggers select this.Triggers[x]).ToArray();
			this.ChangedTriggers.Clear();
			this.Triggers.RemoveWhere(x => x.Subscribers.Count == 0);
			return changedTriggers;
		}

		private void EnsureTriggerExists (string triggerName, string subscriberId)
		{
			if (!this.Triggers.Contains(triggerName))
			{
				this.Triggers.Add(new Trigger(triggerName));
				this.ChangedTriggers.Add(triggerName);
			}

			if (!this.Triggers[triggerName].Subscribers.ContainsKey(subscriberId))
			{
				this.Triggers[triggerName].Subscribers.Add(subscriberId, false);
				this.ChangedTriggers.Add(triggerName);
			}
		}

		private void TriggerArm (string triggerName, string subscriberId)
		{
			this.EnsureTriggerExists(triggerName, subscriberId);

			this.Triggers[triggerName].Subscribers[subscriberId] = true;
			this.ChangedTriggers.Add(triggerName);
		}

		private void TriggerDisarm (string triggerName, string subscriberId)
		{
			this.EnsureTriggerExists(triggerName, subscriberId);

			this.Triggers[triggerName].Subscribers[subscriberId] = false;
			this.ChangedTriggers.Add(triggerName);
		}

		private void TriggerSubscribe (string triggerName, string subscriberId)
		{
			this.EnsureTriggerExists(triggerName, subscriberId);
		}

		private void TriggerUnsubscribe (string triggerName, string subscriberId)
		{
			if (this.Triggers.Contains(triggerName))
			{
				if (this.Triggers[triggerName].Subscribers.ContainsKey(subscriberId))
				{
					this.Triggers[triggerName].Subscribers.Remove(subscriberId);
					this.ChangedTriggers.Add(triggerName);
				}
			}
		}

		#endregion




		#region Interface: IMessageReceiver

		/// <inheritdoc />
		public void ReceiveMessage (IMessage message, IMessageService messageService)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			if (messageService == null)
			{
				throw new ArgumentNullException(nameof(messageService));
			}

			if (message.Name.StartsWith(TriggerMessageNames.MessageNamePrefix, TriggerMessageHandler.TriggerNameComparison) && (!string.Equals(message.Name, TriggerMessageNames.MessageNameResponseChanged, TriggerMessageHandler.TriggerNameComparison)))
			{
				string triggerName;
				string subscriberId;

				try
				{
					triggerName = message.GetTriggerName();
					subscriberId = message.GetSubscriberId();
				}
				catch (InvalidOperationException)
				{
					triggerName = null;
					subscriberId = null;
				}

				if ((triggerName != null) && (subscriberId != null))
				{
					if (string.Equals(message.Name, TriggerMessageNames.MessageNameRequestSubscribe, TriggerMessageHandler.TriggerNameComparison))
					{
						this.TriggerSubscribe(triggerName, subscriberId);
					}
					else if (string.Equals(message.Name, TriggerMessageNames.MessageNameRequestUnsubscribe, TriggerMessageHandler.TriggerNameComparison))
					{
						this.TriggerUnsubscribe(triggerName, subscriberId);
					}
					else if (string.Equals(message.Name, TriggerMessageNames.MessageNameRequestArm, TriggerMessageHandler.TriggerNameComparison))
					{
						this.TriggerArm(triggerName, subscriberId);
					}
					else if (string.Equals(message.Name, TriggerMessageNames.MessageNameRequestDisarm, TriggerMessageHandler.TriggerNameComparison))
					{
						this.TriggerDisarm(triggerName, subscriberId);
					}

					Trigger[] changedTriggers = this.CheckTriggersChanged();
					foreach (Trigger changedTrigger in changedTriggers)
					{
						this.Log(LogLevel.Debug, "Trigger changed: {0} -> [{1}/{2}]", changedTrigger.Name, changedTrigger.ArmedCount, changedTrigger.SubscriberCount);

						Message triggerChangedMessage = new Message(TriggerMessageNames.MessageNameResponseChanged);
						triggerChangedMessage.SetTriggerName(changedTrigger.Name);
						triggerChangedMessage.SetAndTriggered(changedTrigger.AndTriggered);
						triggerChangedMessage.SetOrTriggered(changedTrigger.OrTriggered);
						messageService.Post(triggerChangedMessage);
					}
				}
			}
		}

		#endregion




		#region Type: Trigger

		private sealed class Trigger
		{
			#region Instance Constructor/Destructor

			public Trigger (string name)
			{
				if (name == null)
				{
					throw new ArgumentNullException(nameof(name));
				}

				if (name.IsEmptyOrWhitespace())
				{
					throw new EmptyStringArgumentException(nameof(name));
				}

				this.Name = name;

				this.Subscribers = new Dictionary<string, bool>(TriggerMessageHandler.TriggerNameComparer);
			}

			#endregion




			#region Instance Properties/Indexer

			public bool AndTriggered
			{
				get
				{
					return this.ArmedCount == this.SubscriberCount;
				}
			}

			public int ArmedCount
			{
				get
				{
					return this.Subscribers.Count(x => x.Value);
				}
			}

			public string Name { get; private set; }

			public bool OrTriggered
			{
				get
				{
					return this.ArmedCount > 0;
				}
			}

			public int SubscriberCount
			{
				get
				{
					return this.Subscribers.Count;
				}
			}

			public Dictionary<string, bool> Subscribers { get; private set; }

			#endregion
		}

		#endregion




		#region Type: TriggerCollection

		private sealed class TriggerCollection : KeyedCollection<string, Trigger>
		{
			#region Instance Constructor/Destructor

			public TriggerCollection ()
				: base(TriggerMessageHandler.TriggerNameComparer)
			{
			}

			#endregion




			#region Overrides

			protected override string GetKeyForItem (Trigger item) => item?.Name;

			#endregion
		}

		#endregion
	}
}
