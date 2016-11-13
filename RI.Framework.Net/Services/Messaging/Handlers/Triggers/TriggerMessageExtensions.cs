using System;
using System.Collections.Generic;
using System.Globalization;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Messaging.Handlers.Triggers
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="IMessage" /> type for working with trigger messages.
	/// </summary>
	public static class TriggerMessageExtensions
	{
		#region Static Methods

		/// <summary>
		///     Creates and sends a message to arm a trigger.
		/// </summary>
		/// <param name="messageService"> The message service used to send the message. </param>
		/// <param name="triggerName"> The trigger name to arm. </param>
		/// <param name="subscriberId"> The subscriber ID which arms the trigger. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="messageService" />, <paramref name="triggerName" /> or <paramref name="subscriberId" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="triggerName" /> or <paramref name="subscriberId" /> is an empty string. </exception>
		public static void ArmTrigger (this IMessageService messageService, string triggerName, string subscriberId)
		{
			if (messageService == null)
			{
				throw new ArgumentNullException(nameof(messageService));
			}

			if (triggerName == null)
			{
				throw new ArgumentNullException(nameof(triggerName));
			}

			if (triggerName.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(triggerName));
			}

			if (subscriberId == null)
			{
				throw new ArgumentNullException(nameof(subscriberId));
			}

			if (subscriberId.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(subscriberId));
			}

			Message armMessage = new Message(TriggerMessageNames.MessageNameRequestArm);
			armMessage.SetSubscriberId(subscriberId);
			armMessage.SetTriggerName(triggerName);

			messageService.Post(armMessage);
		}

		/// <summary>
		///     Creates and sends a message to disarm a trigger.
		/// </summary>
		/// <param name="messageService"> The message service used to send the message. </param>
		/// <param name="triggerName"> The trigger name to disarm. </param>
		/// <param name="subscriberId"> The subscriber ID which disarms the trigger. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="messageService" />, <paramref name="triggerName" /> or <paramref name="subscriberId" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="triggerName" /> or <paramref name="subscriberId" /> is an empty string. </exception>
		public static void DisarmTrigger (this IMessageService messageService, string triggerName, string subscriberId)
		{
			if (messageService == null)
			{
				throw new ArgumentNullException(nameof(messageService));
			}

			if (triggerName == null)
			{
				throw new ArgumentNullException(nameof(triggerName));
			}

			if (triggerName.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(triggerName));
			}

			if (subscriberId == null)
			{
				throw new ArgumentNullException(nameof(subscriberId));
			}

			if (subscriberId.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(subscriberId));
			}

			Message armMessage = new Message(TriggerMessageNames.MessageNameRequestDisarm);
			armMessage.SetSubscriberId(subscriberId);
			armMessage.SetTriggerName(triggerName);

			messageService.Post(armMessage);
		}

		/// <summary>
		///     Gets the AND-triggered state.
		/// </summary>
		/// <param name="message"> The trigger message. </param>
		/// <returns>
		///     The AND-triggered state.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="message" /> seems not to be a trigger message because either the AND-triggered state is not set or it is not of the type <see cref="bool" />. </exception>
		public static bool GetAndTriggered (this IMessage message)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			try
			{
				return (bool)message.GetData(TriggerMessageNames.DataNameAndTriggered);
			}
			catch (InvalidCastException exception)
			{
				throw new InvalidOperationException(exception.Message, exception);
			}
			catch (KeyNotFoundException exception)
			{
				throw new InvalidOperationException(exception.Message, exception);
			}
		}

		/// <summary>
		///     Gets the OR-triggered state.
		/// </summary>
		/// <param name="message"> The trigger message. </param>
		/// <returns>
		///     The OR-triggered state.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="message" /> seems not to be a trigger message because either the OR-triggered state is not set or it is not of the type <see cref="bool" />. </exception>
		public static bool GetOrTriggered (this IMessage message)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			try
			{
				return (bool)message.GetData(TriggerMessageNames.DataNameOrTriggered);
			}
			catch (InvalidCastException exception)
			{
				throw new InvalidOperationException(exception.Message, exception);
			}
			catch (KeyNotFoundException exception)
			{
				throw new InvalidOperationException(exception.Message, exception);
			}
		}

		/// <summary>
		///     Gets the subscriber ID.
		/// </summary>
		/// <param name="message"> The trigger message. </param>
		/// <returns>
		///     The subscriber ID.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="message" /> seems not to be a trigger message because either the subscriber ID is not set or it is not of the type <see cref="string" />. </exception>
		public static string GetSubscriberId (this IMessage message)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			try
			{
				return (string)message.GetData(TriggerMessageNames.DataNameSubscriberId);
			}
			catch (InvalidCastException exception)
			{
				throw new InvalidOperationException(exception.Message, exception);
			}
			catch (KeyNotFoundException exception)
			{
				throw new InvalidOperationException(exception.Message, exception);
			}
		}

		/// <summary>
		///     Gets the trigger name.
		/// </summary>
		/// <param name="message"> The trigger message. </param>
		/// <returns>
		///     The trigger name.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="message" /> seems not to be a trigger message because either the trigger name is not set or it is not of the type <see cref="string" />. </exception>
		public static string GetTriggerName (this IMessage message)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			try
			{
				return (string)message.GetData(TriggerMessageNames.DataNameTriggerName);
			}
			catch (InvalidCastException exception)
			{
				throw new InvalidOperationException(exception.Message, exception);
			}
			catch (KeyNotFoundException exception)
			{
				throw new InvalidOperationException(exception.Message, exception);
			}
		}

		/// <summary>
		///     Checks whether a message is a valid trigger changed response.
		/// </summary>
		/// <param name="message"> The message. </param>
		/// <returns>
		///     true if <paramref name="message" /> is a valid trigger chenged response, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> is null. </exception>
		public static bool IsTriggerChangedResponse (this IMessage message)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			return string.Equals(message.Name, TriggerMessageNames.MessageNameResponseChanged, StringComparison.Ordinal);
		}

		/// <summary>
		///     Determines whether a message is a trigger changed response and if so, checks whether the specified trigger is AND-triggered.
		/// </summary>
		/// <param name="message"> The trigger message. </param>
		/// <param name="triggerName"> The trigger name to check. </param>
		/// <returns>
		///     If the message is a trigger changed response and the trigger name matches, true is returned if AND-triggered or false if not AND-triggered.
		///     If the message is not a trigger respond message or the trigger names do not match, null is returned.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> or <paramref name="triggerName" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="triggerName" /> is an empty string. </exception>
		/// TODO: Split into IsTriggeredOr and IsTriggeredAnd
		public static bool? IsTriggered (this IMessage message, string triggerName)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			if (triggerName == null)
			{
				throw new ArgumentNullException(nameof(triggerName));
			}

			if (triggerName.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(triggerName));
			}

			if (!message.IsTriggerChangedResponse())
			{
				return null;
			}

			if (!string.Equals(message.GetTriggerName(), triggerName, StringComparison.InvariantCultureIgnoreCase))
			{
				return null;
			}

			return message.GetAndTriggered();
		}

		/// <summary>
		///     Sets the AND-triggered state.
		/// </summary>
		/// <param name="message"> The trigger message. </param>
		/// <param name="state"> The AND-triggered state to set. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> is null. </exception>
		public static void SetAndTriggered (this IMessage message, bool state)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			message.SetData(TriggerMessageNames.DataNameAndTriggered, state);
		}

		/// <summary>
		///     Sets the OR-triggered state.
		/// </summary>
		/// <param name="message"> The trigger message. </param>
		/// <param name="state"> The OR-triggered state to set. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> is null. </exception>
		public static void SetOrTriggered (this IMessage message, bool state)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			message.SetData(TriggerMessageNames.DataNameOrTriggered, state);
		}

		/// <summary>
		///     Sets the subscriber ID.
		/// </summary>
		/// <param name="message"> The trigger message. </param>
		/// <param name="subscriberId"> The subscriber ID to set. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> or <paramref name="subscriberId" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="subscriberId" /> is an empty string. </exception>
		public static void SetSubscriberId (this IMessage message, string subscriberId)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			if (subscriberId == null)
			{
				throw new ArgumentNullException(nameof(subscriberId));
			}

			if (subscriberId.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(subscriberId));
			}

			message.SetData(TriggerMessageNames.DataNameSubscriberId, subscriberId);
		}

		/// <summary>
		///     Sets the trigger name.
		/// </summary>
		/// <param name="message"> The trigger message. </param>
		/// <param name="triggerName"> The trigger name to set. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> or <paramref name="triggerName" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="triggerName" /> is an empty string. </exception>
		public static void SetTriggerName (this IMessage message, string triggerName)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			if (triggerName == null)
			{
				throw new ArgumentNullException(nameof(triggerName));
			}

			if (triggerName.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(triggerName));
			}

			message.SetData(TriggerMessageNames.DataNameTriggerName, triggerName);
		}

		/// <summary>
		///     Creates and sends a message to subscribe to a trigger.
		/// </summary>
		/// <param name="messageService"> The message service used to send the message. </param>
		/// <param name="triggerName"> The trigger name to subscribe to. </param>
		/// <returns>
		///     An automatically generated subscriber ID which is used to subscribe to the trigger.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The subscriber ID is generated using <see cref="Guid" />.<see cref="Guid.NewGuid" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="messageService" /> or <paramref name="triggerName" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="triggerName" /> is an empty string. </exception>
		public static string SubscribeToTrigger (this IMessageService messageService, string triggerName)
		{
			if (messageService == null)
			{
				throw new ArgumentNullException(nameof(messageService));
			}

			if (triggerName == null)
			{
				throw new ArgumentNullException(nameof(triggerName));
			}

			if (triggerName.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(triggerName));
			}

			string subscriberId = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

			Message subscriptionMessage = new Message(TriggerMessageNames.MessageNameRequestSubscribe);
			subscriptionMessage.SetSubscriberId(subscriberId);
			subscriptionMessage.SetTriggerName(triggerName);

			messageService.Post(subscriptionMessage);

			return subscriberId;
		}

		/// <summary>
		///     Creates and sends a message to unsubscribe from a trigger.
		/// </summary>
		/// <param name="messageService"> The message service used to send the message. </param>
		/// <param name="triggerName"> The trigger name to unsubscribe from. </param>
		/// <param name="subscriberId"> The subscriber ID which unsubscribes from the trigger. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="messageService" />, <paramref name="triggerName" /> or <paramref name="subscriberId" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="triggerName" /> or <paramref name="subscriberId" /> is an empty string. </exception>
		public static void UnsubscribeFromTrigger (this IMessageService messageService, string triggerName, string subscriberId)
		{
			if (messageService == null)
			{
				throw new ArgumentNullException(nameof(messageService));
			}

			if (triggerName == null)
			{
				throw new ArgumentNullException(nameof(triggerName));
			}

			if (triggerName.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(triggerName));
			}

			if (subscriberId == null)
			{
				throw new ArgumentNullException(nameof(subscriberId));
			}

			if (subscriberId.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(subscriberId));
			}

			Message subscriptionMessage = new Message(TriggerMessageNames.MessageNameRequestUnsubscribe);
			subscriptionMessage.SetSubscriberId(subscriberId);
			subscriptionMessage.SetTriggerName(triggerName);

			messageService.Post(subscriptionMessage);
		}

		#endregion
	}
}
