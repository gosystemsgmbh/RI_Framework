namespace RI.Framework.Bus.Internals
{
	public sealed class ReceiveRegistrationItem
	{
		public ReceiveRegistrationItem(ReceiverRegistration receiverRegistration)
		{
			this.ReceiverRegistration = receiverRegistration;
		}

		public ReceiverRegistration ReceiverRegistration { get; }
	}
}