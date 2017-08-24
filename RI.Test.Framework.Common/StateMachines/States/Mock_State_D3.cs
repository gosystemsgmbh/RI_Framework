using RI.Framework.StateMachines;




namespace RI.Test.Framework.StateMachines
{
	public sealed class Mock_State_D3 : Mock_State_D
	{
		#region Overrides

		protected override void Enter (StateTransientInfo transientInfo)
		{
			base.Enter(transientInfo);

			Mock_State.TestValue += "eD3";

			transientInfo.StateMachine.Transient<Mock_State_D1>();

			Mock_State.TestValue += "###";
		}

		protected override void Initialize (StateMachine stateMachine)
		{
			base.Initialize(stateMachine);

			Mock_State.TestValue += "iD3";
		}

		protected override void Leave (StateTransientInfo transientInfo)
		{
			base.Leave(transientInfo);

			Mock_State.TestValue += "lD3";
		}

		protected override void Signal (StateSignalInfo signalInfo)
		{
			base.Signal(signalInfo);

			Mock_State.TestValue += "sD3";
		}

		#endregion
	}
}
