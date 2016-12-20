using System;




namespace RI.Framework.StateMachines
{
	public sealed class StateSignalInfo
	{
		public StateSignalInfo (StateMachine stateMachine)
		{
			if (stateMachine == null)
			{
				throw new ArgumentNullException(nameof(stateMachine));
			}

			this.StateMachine = stateMachine;
		}

		public StateMachine StateMachine { get; private set; }

		public object Signal { get; set; }
	}
}