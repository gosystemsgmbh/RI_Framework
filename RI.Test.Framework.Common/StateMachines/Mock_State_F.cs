using RI.Framework.StateMachines;




namespace RI.Test.Framework.StateMachines
{
	public sealed class Mock_State_F : Mock_State
	{
		protected override void Enter(StateTransientInfo transientInfo)
		{
			base.Enter(transientInfo);

			Mock_State.TestValue += "eF";
		}

		protected override void Leave(StateTransientInfo transientInfo)
		{
			base.Leave(transientInfo);

			Mock_State.TestValue += "lF";
		}

		protected override void Initialize(StateMachine stateMachine)
		{
			base.Initialize(stateMachine);

			Mock_State.TestValue += "iF";

			this.UseCaching = false;
		}
	}
}