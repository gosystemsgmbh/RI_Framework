using System;
using System.Collections.Generic;




namespace RI.Framework.StateMachines
{
	//TODO: Performance optimization
	public abstract class StateBase : IState
	{
		protected StateBase ()
		{
			this.IsInitialized = false;
			this.UseCaching = true;

			this.StateMachine = null;

			this.SignalHandlers = new Dictionary<Type, Delegate>();
		}


		public bool IsInitialized { get; protected set; }

		public bool UseCaching { get; protected set; }

		protected StateMachine StateMachine { get; private set; }

		private Dictionary<Type, Delegate> SignalHandlers { get; set; }



		protected virtual void Initialize (StateMachine stateMachine)
		{
		}

		protected virtual void Enter (StateTransientInfo transientInfo)
		{
		}

		protected virtual void Leave (StateTransientInfo transientInfo)
		{
		}

		protected virtual void Signal (StateSignalInfo signalInfo)
		{
			Type signalType = signalInfo.Signal?.GetType();
			if ((signalType != null) && (this.SignalHandlers.ContainsKey(signalType)))
			{
				this.SignalHandlers[signalType].DynamicInvoke(signalInfo.Signal);
			}
			else
			{
				this.OnUnregisteredSignal(signalInfo);
			}
		}

		protected virtual void OnUnregisteredSignal (StateSignalInfo signalInfo)
		{
		}



		private void SetStateMachine (StateMachine stateMachine)
		{
			this.StateMachine = stateMachine;
		}

		void IState.Initialize (StateMachine stateMachine)
		{
			if (stateMachine == null)
			{
				throw new ArgumentNullException(nameof(stateMachine));
			}

			if (!this.IsInitialized)
			{
				this.SetStateMachine(stateMachine);
				this.Initialize(stateMachine);
			}

			this.IsInitialized = true;
		}

		void IState.Enter (StateTransientInfo transientInfo)
		{
			if (transientInfo == null)
			{
				throw new ArgumentNullException(nameof(transientInfo));
			}

			this.SetStateMachine(transientInfo.StateMachine);
			this.Enter(transientInfo);
		}

		void IState.Leave (StateTransientInfo transientInfo)
		{
			if (transientInfo == null)
			{
				throw new ArgumentNullException(nameof(transientInfo));
			}

			this.SetStateMachine(transientInfo.StateMachine);
			this.Leave(transientInfo);
		}

		void IState.Signal (StateSignalInfo signalInfo)
		{
			if (signalInfo == null)
			{
				throw new ArgumentNullException(nameof(signalInfo));
			}

			this.SetStateMachine(signalInfo.StateMachine);
			this.Signal(signalInfo);
		}



		protected void RegisterSignal<TSignal> (Action<TSignal> handler)
		{
			Type type = typeof(TSignal);
			this.SignalHandlers.Remove(type);
			this.SignalHandlers.Add(type, handler);
		}

		protected void UnregisterSignal<TSignal> ()
		{
			Type type = typeof(TSignal);
			this.SignalHandlers.Remove(type);
		}
	}
}