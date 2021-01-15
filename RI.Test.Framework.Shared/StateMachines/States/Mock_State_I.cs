using RI.Framework.StateMachines;
using RI.Framework.StateMachines.States;




namespace RI.Test.Framework.StateMachines.States
{
	public sealed class Mock_State_I : State
	{
		#region Overrides

		public string TestValue1 { get; set; }

		protected override void Initialize (StateMachine stateMachine)
		{
			base.Initialize(stateMachine);

			this.UseCaching = false;
		}

		[StateVariable]
		public string TestValue2 { get; set; }

		[StateVariable(StateVariableHandling.Unspecified)]
		public string TestValue3 { get; set; }

		[StateVariable(StateVariableHandling.Transient)]
		public string TestValue4 { get; set; }

		[StateVariable(StateVariableHandling.NonTransient)]
		public string TestValue5 { get; set; }

		#endregion
	}
}
