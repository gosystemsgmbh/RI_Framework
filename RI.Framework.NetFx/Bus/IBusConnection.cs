using System;

using RI.Framework.Composition.Model;

namespace RI.Framework.Bus
{
	[Export]
	public interface IBusConnection
	{
		void Connect ();

		void Disconnect ();

		void AddSubscription (IBusNode busNode, IBusEndpoint busEndpoint, Type messageType);

		void RemoveSubscription (IBusNode busNode, IBusEndpoint busEndpoint, Type messageType);
	}
}
