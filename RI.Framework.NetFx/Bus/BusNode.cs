using System;
using System.Threading.Tasks;

using RI.Framework.Bus.Endpoints;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;

namespace RI.Framework.Bus
{
	public sealed class BusNode
	{
		internal BusNode (BusContext context, string name)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			this.Context = context;
			this.Id = name.Trim();
		}




		public BusContext Context { get; }
		public string Id { get; }




		public void RegisterReceiver (IBusEndpoint receiver, Type messageType)
		{
		}

		public void UnregisterReceiver (IBusEndpoint receiver, Type messageType)
		{
		}

		public BusTransmission SendMessage (IBusEndpoint sender, Type messageType, object message)
		{
			return null;
		}

		public async Task<BusTransmission> SendMessageAsync (IBusEndpoint sender, Type messageType, object message)
		{
			return null;
		}
	}
}
