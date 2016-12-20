using System;




namespace RI.Framework.StateMachines
{
	//TODO: Add cascading
	public class StateMachine
	{
		public StateMachine ()
			: this(new StateMachineConfiguration())
		{
		}

		public StateMachine (StateMachineConfiguration configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException(nameof(configuration));
			}

			this.Configuration = configuration;

			this.TransientDelegate = this.ExecuteTransient;
			this.SignalDelegate = this.ExecuteSignal;

			this.State = null;
		}




		public StateMachineConfiguration Configuration { get; private set; }

		public IState State { get; private set; }




		protected virtual void DispatchTransient(StateTransientInfo transientInfo)
		{
			if (this.Configuration.Dispatcher == null)
			{
				this.TransientDelegate(transientInfo);
			}
			else
			{
				this.Configuration.Dispatcher.DispatchTransient(this.TransientDelegate, transientInfo);
			}
		}

		protected virtual void DispatchSignal(StateSignalInfo signalInfo)
		{
			if (this.Configuration.Dispatcher == null)
			{
				this.SignalDelegate(signalInfo);
			}
			else
			{
				this.Configuration.Dispatcher.DispatchSignal(this.SignalDelegate, signalInfo);
			}
		}

		protected virtual IState Resolve (Type type, bool useCache)
		{
			if (type == null)
			{
				return null;
			}

			IState state;

			if (useCache && (this.Configuration.Cache != null) && this.Configuration.Cache.ContainsState(type))
			{
				state = this.Configuration.Cache.GetState(type);
			}
			else
			{
				if (this.Configuration.Resolver == null)
				{
					state = Activator.CreateInstance(type) as IState;
					if (state == null)
					{
						//TODO: Use StateNotFoundException
						throw new InvalidOperationException("The state \"" + type.Name + "\" could not be created.");
					}
				}
				else
				{
					state = this.Configuration.Resolver.ResolveState(type);
					if (state == null)
					{
						//TODO: Use StateNotFoundException
						throw new InvalidOperationException("The state \"" + type.Name + "\" could not be resolved.");
					}
				}
			}

			return state;
		}




		public void Transient<TState> ()
			where TState : IState
		{
			this.Transient(typeof(TState));
		}

		public void Transient (Type state)
		{
			if (state == null)
			{
				throw new ArgumentNullException(nameof(state));
			}

			IState previousState = this.State;
			IState nextState = this.Resolve(state, true);

			StateTransientInfo transientInfo = new StateTransientInfo(this);
			transientInfo.NextState = nextState;
			transientInfo.PreviousState = previousState;

			this.DispatchTransient(transientInfo);
		}

		private StateMachineTransientDelegate TransientDelegate { get; set; }

		protected virtual void ExecuteTransient (StateTransientInfo transientInfo)
		{
			IState previousState = transientInfo.PreviousState;
			IState nextState = transientInfo.NextState;

			if (!object.ReferenceEquals(this.State, previousState))
			{
				return;
			}

			if (!((nextState?.IsInitialized).GetValueOrDefault(true)))
			{
				nextState?.Initialize(this);
			}

			this.OnBeforeLeave(transientInfo);
			this.State?.Leave(transientInfo);
			this.OnAfterLeave(transientInfo);

			this.State = nextState;

			this.OnBeforeEnter(transientInfo);
			this.State?.Enter(transientInfo);
			this.OnAfterEnter(transientInfo);

			if ((nextState != null) && (this.Configuration.Cache != null))
			{
				if (nextState.UseCaching && this.Configuration.EnableAutomaticCaching)
				{
					this.Configuration.Cache.AddState(nextState);
				}
			}
		}

		public void Signal<TSignal> (TSignal signal)
		{
			this.Signal((object)signal);
		}

		public void Signal (object signal)
		{
			StateSignalInfo signalInfo = new StateSignalInfo(this);
			signalInfo.Signal = signal;

			this.DispatchSignal(signalInfo);
		}

		private StateMachineSignalDelegate SignalDelegate { get; set; }

		protected virtual void ExecuteSignal (StateSignalInfo signalInfo)
		{
			this.OnBeforeSignal(signalInfo);
			this.State?.Signal(signalInfo);
			this.OnAfterSignal(signalInfo);
		}




		public event EventHandler<StateMachineTransientEventArgs> BeforeLeave;
		protected virtual void OnBeforeLeave (StateTransientInfo transientInfo)
		{
			this.BeforeLeave?.Invoke(this, new StateMachineTransientEventArgs(transientInfo));
		}

		public event EventHandler<StateMachineTransientEventArgs> AfterLeave;
		protected virtual void OnAfterLeave(StateTransientInfo transientInfo)
		{
			this.AfterLeave?.Invoke(this, new StateMachineTransientEventArgs(transientInfo));
		}

		public event EventHandler<StateMachineTransientEventArgs> BeforeEnter;
		protected virtual void OnBeforeEnter(StateTransientInfo transientInfo)
		{
			this.BeforeEnter?.Invoke(this, new StateMachineTransientEventArgs(transientInfo));
		}

		public event EventHandler<StateMachineTransientEventArgs> AfterEnter;
		protected virtual void OnAfterEnter(StateTransientInfo transientInfo)
		{
			this.AfterEnter?.Invoke(this, new StateMachineTransientEventArgs(transientInfo));
		}

		public event EventHandler<StateMachineSignalEventArgs> BeforeSignal;
		protected virtual void OnBeforeSignal(StateSignalInfo signalInfo)
		{
			this.BeforeSignal?.Invoke(this, new StateMachineSignalEventArgs(signalInfo));
		}

		public event EventHandler<StateMachineSignalEventArgs> AfterSignal;
		protected virtual void OnAfterSignal(StateSignalInfo signalInfo)
		{
			this.AfterSignal?.Invoke(this, new StateMachineSignalEventArgs(signalInfo));
		}
	}
}