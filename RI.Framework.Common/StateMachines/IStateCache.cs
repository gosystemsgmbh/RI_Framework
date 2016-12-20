using System;




namespace RI.Framework.StateMachines
{
	public interface IStateCache
	{
		bool ContainsState (Type state);

		IState GetState (Type state);

		void AddState (IState state);

		void RemoveState (IState state);

		void Clear ();
	}
}