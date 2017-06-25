using System;




namespace RI.Framework.Bus.Dispatchers
{
	public interface IBusDispatcher
	{
		bool DispatchError (Delegate action, object[] parameters);
		bool DispatchReception (Delegate action, object[] parameters);
	}
}
