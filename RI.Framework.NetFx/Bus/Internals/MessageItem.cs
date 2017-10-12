using System;

namespace RI.Framework.Bus.Internals
{
	[Serializable]
	public sealed class MessageItem
	{
		public string Address { get; set; }

		public object Payload { get; set; }

		public int Timeout { get; set; }

		public Guid Id { get; set; }

		public DateTime Sent { get; set; }

		public bool IsLocal { get; set; }

		public Guid ResponseTo { get; set; }
	}
}
