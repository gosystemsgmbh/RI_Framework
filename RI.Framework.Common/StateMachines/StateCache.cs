using System;
using System.Collections.Generic;




namespace RI.Framework.StateMachines
{
	public sealed class StateCache : IStateCache
	{
		public StateCache ()
		{
			this.States = new Dictionary<Type, IState>();
		}

		public Dictionary<Type, IState> States { get; private set; }

		public bool ContainsState (Type state)
		{
			if (state == null)
			{
				throw new ArgumentNullException(nameof(state));
			}

			return this.States.ContainsKey(state);
		}

		public IState GetState (Type state)
		{
			if (state == null)
			{
				throw new ArgumentNullException(nameof(state));
			}

			if (!this.States.ContainsKey(state))
			{
				throw new KeyNotFoundException();
			}

			return this.States[state];
		}

		public void AddState (IState state)
		{
			if (state == null)
			{
				throw new ArgumentNullException(nameof(state));
			}

			Type type = state.GetType();
			if (this.States.ContainsKey(type))
			{
				this.States[type] = state;
			}
			else
			{
				this.States.Add(type, state);
			}
		}

		public void RemoveState (IState state)
		{
			if (state == null)
			{
				throw new ArgumentNullException(nameof(state));
			}

			Type type = state.GetType();
			this.States.Remove(type);
		}

		public void Clear ()
		{
			this.States.Clear();
		}
	}
}