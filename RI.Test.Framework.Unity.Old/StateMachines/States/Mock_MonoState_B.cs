using RI.Framework.Composition.Model;
using RI.Framework.StateMachines;




namespace RI.Test.Framework.StateMachines.States
{
	[Export]
	public sealed class Mock_MonoState_B : Mock_MonoState
	{
		#region Overrides

		protected override void Enter (StateTransientInfo transientInfo)
		{
			base.Enter(transientInfo);

			Mock_State.TestValue += "eB";
		}

		protected override void Initialize (StateMachine stateMachine)
		{
			base.Initialize(stateMachine);

			Mock_State.TestValue += "iB";
		}

		protected override void Leave (StateTransientInfo transientInfo)
		{
			base.Leave(transientInfo);

			Mock_State.TestValue += "lB";
		}

		protected override void Signal (StateSignalInfo signalInfo)
		{
			base.Signal(signalInfo);

			Mock_State.TestValue += (string)signalInfo.Signal;
		}

		#endregion
	}
}
