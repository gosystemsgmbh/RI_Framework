using System;

namespace RI.Framework.Bus.Dispatchers
{
	public interface IBusDispatcher
	{
		bool DispatchReception (Delegate action, object[] parameters);

		bool DispatchError (Delegate action, object[] parameters);
	}
}
