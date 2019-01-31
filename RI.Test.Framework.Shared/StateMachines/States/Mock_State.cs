using RI.Framework.StateMachines;
using RI.Framework.StateMachines.States;




namespace RI.Test.Framework.StateMachines.States
{
	public abstract class Mock_State : State
	{
		#region Static Properties/Indexer

		public static string TestValue { get; set; }

		protected override void Initialize (StateMachine stateMachine)
		{
			base.Initialize(stateMachine);

			this.UseCaching = true;
		}

		#endregion
	}
}
