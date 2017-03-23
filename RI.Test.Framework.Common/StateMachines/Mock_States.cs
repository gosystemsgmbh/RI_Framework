using System;
using System.Collections.Generic;
using System.Text;

using RI.Framework.StateMachines;




namespace RI.Test.Framework.StateMachines
{
	public abstract class Mock_State : State
	{
		public static string TestValue { get; set; }
	}

	public sealed class Mock_State_A : Mock_State
	{
		protected override void Enter (StateTransientInfo transientInfo)
		{
			base.Enter(transientInfo);

			Mock_State.TestValue += "eA";
		}

		protected override void Leave (StateTransientInfo transientInfo)
		{
			base.Leave(transientInfo);

			Mock_State.TestValue += "lA";
		}

		protected override void Initialize (StateMachine stateMachine)
		{
			base.Initialize(stateMachine);

			Mock_State.TestValue += "iA";
		}

		protected override void Signal (StateSignalInfo signalInfo)
		{
			base.Signal(signalInfo);

			Mock_State.TestValue += (string)signalInfo.Signal;
		}
	}

	public sealed class Mock_State_B : Mock_State
	{
		protected override void Enter(StateTransientInfo transientInfo)
		{
			base.Enter(transientInfo);

			Mock_State.TestValue += "eB";
		}

		protected override void Leave(StateTransientInfo transientInfo)
		{
			base.Leave(transientInfo);

			Mock_State.TestValue += "lB";
		}

		protected override void Initialize(StateMachine stateMachine)
		{
			base.Initialize(stateMachine);

			Mock_State.TestValue += "iB";
		}

		protected override void Signal(StateSignalInfo signalInfo)
		{
			base.Signal(signalInfo);

			Mock_State.TestValue += (string)signalInfo.Signal;
		}
	}

	public sealed class Mock_State_C : Mock_State
	{
		protected override void Enter(StateTransientInfo transientInfo)
		{
			base.Enter(transientInfo);

			Mock_State.TestValue += "eC";
		}

		protected override void Leave(StateTransientInfo transientInfo)
		{
			base.Leave(transientInfo);

			Mock_State.TestValue += "lC";
		}

		protected override void Initialize(StateMachine stateMachine)
		{
			base.Initialize(stateMachine);

			Mock_State.TestValue += "iC";
		}

		protected override void Signal(StateSignalInfo signalInfo)
		{
			base.Signal(signalInfo);

			Mock_State.TestValue += (string)signalInfo.Signal;
		}
	}

	public abstract class Mock_State_D : Mock_State
	{
		protected override void Enter(StateTransientInfo transientInfo)
		{
			base.Enter(transientInfo);

			Mock_State.TestValue += "eD";
		}

		protected override void Leave(StateTransientInfo transientInfo)
		{
			base.Leave(transientInfo);

			Mock_State.TestValue += "lD";
		}

		protected override void Initialize(StateMachine stateMachine)
		{
			base.Initialize(stateMachine);

			Mock_State.TestValue += "iD";

			this.IsInitialized = true;
		}

		protected override void Signal(StateSignalInfo signalInfo)
		{
			base.Signal(signalInfo);

			Mock_State.TestValue += "sD";
		}
	}

	public sealed class Mock_State_D1 : Mock_State_D
	{
		protected override void Enter(StateTransientInfo transientInfo)
		{
			base.Enter(transientInfo);

			Mock_State.TestValue += "eD1";
		}

		protected override void Leave(StateTransientInfo transientInfo)
		{
			base.Leave(transientInfo);

			Mock_State.TestValue += "lD1";
		}

		protected override void Initialize(StateMachine stateMachine)
		{
			base.Initialize(stateMachine);

			Mock_State.TestValue += "iD1";
		}

		protected override void Signal(StateSignalInfo signalInfo)
		{
			base.Signal(signalInfo);

			Mock_State.TestValue += "sD1";
		}
	}

	public sealed class Mock_State_D2 : Mock_State_D
	{
		protected override void Enter(StateTransientInfo transientInfo)
		{
			base.Enter(transientInfo);

			Mock_State.TestValue += "eD2";
		}

		protected override void Leave(StateTransientInfo transientInfo)
		{
			base.Leave(transientInfo);

			Mock_State.TestValue += "lD2";
		}

		protected override void Initialize(StateMachine stateMachine)
		{
			base.Initialize(stateMachine);

			Mock_State.TestValue += "iD2";
		}

		protected override void Signal(StateSignalInfo signalInfo)
		{
			base.Signal(signalInfo);

			Mock_State.TestValue += "sD2";

			this.StateMachine.Transient<Mock_State_D3>();

			Mock_State.TestValue += "***";
		}
	}

	public sealed class Mock_State_D3 : Mock_State_D
	{
		protected override void Enter(StateTransientInfo transientInfo)
		{
			base.Enter(transientInfo);

			Mock_State.TestValue += "eD3";

			this.StateMachine.Transient<Mock_State_D1>();

			Mock_State.TestValue += "###";
		}

		protected override void Leave(StateTransientInfo transientInfo)
		{
			base.Leave(transientInfo);

			Mock_State.TestValue += "lD3";
		}

		protected override void Initialize(StateMachine stateMachine)
		{
			base.Initialize(stateMachine);

			Mock_State.TestValue += "iD3";
		}

		protected override void Signal(StateSignalInfo signalInfo)
		{
			base.Signal(signalInfo);

			Mock_State.TestValue += "sD3";
		}
	}

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

	public sealed class Mock_State_G : Mock_State
	{
		protected override void Enter(StateTransientInfo transientInfo)
		{
			base.Enter(transientInfo);

			Mock_State.TestValue += "eG";
		}

		protected override void Leave(StateTransientInfo transientInfo)
		{
			base.Leave(transientInfo);

			Mock_State.TestValue += "lG";
		}

		protected override void Initialize(StateMachine stateMachine)
		{
			base.Initialize(stateMachine);

			Mock_State.TestValue += "iG";

			this.UseCaching = true;
		}
	}
}
