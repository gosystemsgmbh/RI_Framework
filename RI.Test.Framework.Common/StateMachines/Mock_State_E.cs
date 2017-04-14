using RI.Framework.StateMachines;




namespace RI.Test.Framework.StateMachines
{
	public sealed class Mock_State_E : Mock_State
	{
		protected override void Initialize(StateMachine stateMachine)
		{
			base.Initialize(stateMachine);

			this.RegisterSignal<string>(this.MySignal);
		}

		private void MySignal (string signal)
		{
			Mock_State.TestValue += signal;

			if (signal == "X")
			{
				this.UnregisterSignal<string>();
			}
		}
	}
}