namespace RI.Framework.StateMachines
{
	public interface IStateDispatcher
	{
		void DispatchTransient (StateMachineTransientDelegate transientDelegate, StateTransientInfo transientInfo);

		void DispatchSignal (StateMachineSignalDelegate signalDelegate, StateSignalInfo signalInfo);
	}
}