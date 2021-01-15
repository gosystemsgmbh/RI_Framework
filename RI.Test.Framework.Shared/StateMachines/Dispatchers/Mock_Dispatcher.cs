using System;
using System.Collections.Generic;
using System.Text;
using RI.Framework.StateMachines;
using RI.Framework.StateMachines.Dispatchers;




namespace RI.Test.Framework.StateMachines.Dispatchers
{
	public sealed class Mock_Dispatcher : IStateDispatcher
	{
		public bool IsSynchronized => false;

		public object SyncRoot => null;

		public void DispatchSignal(StateMachineSignalDelegate signalDelegate, StateSignalInfo signalInfo)
		{
			signalDelegate(signalInfo);
		}

		public void DispatchTransition(StateMachineTransientDelegate transientDelegate, StateTransientInfo transientInfo)
		{
			transientDelegate(transientInfo);
		}

		public void DispatchUpdate(StateMachineUpdateDelegate updateDelegate, StateUpdateInfo updateInfo)
		{
			updateDelegate(updateInfo);
		}
	}
}
