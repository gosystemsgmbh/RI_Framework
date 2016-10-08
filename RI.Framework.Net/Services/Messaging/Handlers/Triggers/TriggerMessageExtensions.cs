using System;
using System.Collections.Generic;

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

		#endregion
	}
}
