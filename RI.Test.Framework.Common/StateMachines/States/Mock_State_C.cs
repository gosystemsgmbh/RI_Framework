using RI.Framework.StateMachines;

namespace RI.Test.Framework.StateMachines.States
{
	public sealed class Mock_State_C : Mock_State
	{
		#region Overrides

		protected override void Enter (StateTransientInfo transientInfo)
		{
			base.Enter(transientInfo);

			Mock_State.TestValue += "eC";
		}

		protected override void Initialize (StateMachine stateMachine)
		{
			base.Initialize(stateMachine);

			Mock_State.TestValue += "iC";
		}

		protected override void Leave (StateTransientInfo transientInfo)
		{
			base.Leave(transientInfo);

			Mock_State.TestValue += "lC";
		}

		protected override void Signal (StateSignalInfo signalInfo)
		{
			base.Signal(signalInfo);

			Mock_State.TestValue += (string)signalInfo.Signal;
		}

		#endregion
	}
}
