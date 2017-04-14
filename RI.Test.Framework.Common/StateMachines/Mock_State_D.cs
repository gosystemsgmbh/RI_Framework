using RI.Framework.StateMachines;




namespace RI.Test.Framework.StateMachines
{
	public abstract class Mock_State_D : Mock_State
	{
		protected override void Enter(StateTransientInfo transientInfo)
		{
			base.Enter(transientInfo);

			Mock_State.TestValue += "eD";
		}

		protected override void Leave(StateTransientInfo transientInfo)
		{
			base.Leave(transientInfo);

			Mock_State.TestValue += "lD";
		}

		protected override void Initialize(StateMachine stateMachine)
		{
			base.Initialize(stateMachine);

			Mock_State.TestValue += "iD";

			this.IsInitialized = true;
		}

		protected override void Signal(StateSignalInfo signalInfo)
		{
			base.Signal(signalInfo);

			Mock_State.TestValue += "sD";
		}
	}
}