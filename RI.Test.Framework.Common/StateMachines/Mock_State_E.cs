using RI.Framework.StateMachines;




namespace RI.Test.Framework.StateMachines
{
	public sealed class Mock_State_E : Mock_State
	{
		#region Instance Methods

		private void MySignal (string signal)
		{
			Mock_State.TestValue += signal;

			if (signal == "X")
			{
				this.UnregisterSignal<string>();
			}
		}

		#endregion




		#region Overrides

		protected override void Initialize (StateMachine stateMachine)
		{
			base.Initialize(stateMachine);

			this.RegisterSignal<string>(this.MySignal);
		}

		#endregion
	}
}
