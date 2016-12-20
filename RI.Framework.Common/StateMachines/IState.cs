namespace RI.Framework.StateMachines
{
	public interface IState
	{
		bool UseCaching { get; }

		void Initialize (StateMachine stateMachine);

		bool IsInitialized { get; }

		void Enter (StateTransientInfo transientInfo);

		void Leave (StateTransientInfo transientInfo);

		void Signal (StateSignalInfo signalInfo);
	}
}