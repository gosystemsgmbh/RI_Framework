namespace RI.Framework.Services.Messaging.Handlers
{
	/// <summary>
	/// Contains trigger message and data name definitions.
	/// </summary>
	public static class TriggerMessageNames
	{
		internal const string MessageNamePrefix = "Trigger.";

		/// <summary>
		/// The request message name for arming a trigger.
		/// </summary>
		public const string MessageNameRequestArm = TriggerMessageNames.MessageNamePrefix + "Request.Arm";

		/// <summary>
		/// The request message name for disarming a trigger.
		/// </summary>
		public const string MessageNameRequestDisarm = TriggerMessageNames.MessageNamePrefix + "Request.Disarm";

		/// <summary>
		/// The request message name for subscribing to a trigger.
		/// </summary>
		public const string MessageNameRequestSubscribe = TriggerMessageNames.MessageNamePrefix + "Request.Subscribe";

		/// <summary>
		/// The request message name for unsubscribing from a trigger.
		/// </summary>
		public const string MessageNameRequestUnsubscribe = TriggerMessageNames.MessageNamePrefix + "Request.Unsubscribe";

		/// <summary>
		/// The response message name for signaling the change of a trigger.
		/// </summary>
		public const string MessageNameResponseChanged = TriggerMessageNames.MessageNamePrefix + "Response.Changed";

		/// <summary>
		/// The message data name which describes the trigger name.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Data type: <see cref="string"/>.
		/// </para>
		/// <para>
		/// Used with the follwing messages:
		/// <see cref="MessageNameRequestArm"/>, <see cref="MessageNameRequestDisarm"/>, <see cref="MessageNameRequestSubscribe"/>, <see cref="MessageNameRequestUnsubscribe"/>, <see cref="MessageNameResponseChanged"/>.
		/// </para>
		/// </remarks>
		public const string DataNameTriggerName = "TriggerName";

		/// <summary>
		/// The message data name which describes the subscriber name.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Data type: <see cref="string"/>.
		/// </para>
		/// <para>
		/// Used with the follwing messages:
		/// <see cref="MessageNameRequestArm"/>, <see cref="MessageNameRequestDisarm"/>, <see cref="MessageNameRequestSubscribe"/>, <see cref="MessageNameRequestUnsubscribe"/>.
		/// </para>
		/// </remarks>
		public const string DataNameSubscriberId = "SubscriberId";

		/// <summary>
		/// The message data name which describes the AND-triggered state.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Data type: <see cref="bool"/>.
		/// </para>
		/// <para>
		/// Used with the follwing messages:
		/// <see cref="MessageNameResponseChanged"/>.
		/// </para>
		/// </remarks>
		public const string DataNameTriggeredAnd = "TriggeredAnd";

		/// <summary>
		/// The message data name which describes the OR-triggered state.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Data type: <see cref="bool"/>.
		/// </para>
		/// <para>
		/// Used with the follwing messages:
		/// <see cref="MessageNameResponseChanged"/>.
		/// </para>
		/// </remarks>
		public const string DataNameTriggeredOr = "TriggeredOr";
	}
}