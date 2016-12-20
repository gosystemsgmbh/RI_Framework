using System;




namespace RI.Framework.StateMachines
{
	public sealed class StateTransientInfo
	{
		public StateTransientInfo(StateMachine stateMachine)
		{
			if (stateMachine == null)
			{
				throw new ArgumentNullException(nameof(stateMachine));
			}

			this.StateMachine = stateMachine;
		}

		public StateMachine StateMachine { get; private set; }

		public IState PreviousState { get; set; }

		public IState NextState { get; set; }
	}
}