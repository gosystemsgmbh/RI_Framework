using System;

using RI.Framework.Composition.Model;




namespace RI.Framework.Services.Messaging.Handlers
{
	[Export]
	public sealed class TriggerMessageHandler : IMessageReceiver
	{
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

			throw new NotImplementedException();
		}

		#endregion
	}
}
