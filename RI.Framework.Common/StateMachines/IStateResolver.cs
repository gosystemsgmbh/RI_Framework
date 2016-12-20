using System;




namespace RI.Framework.StateMachines
{
	//TODO: Create CompositionContainerStateResolver
	public interface IStateResolver
	{
		IState ResolveState (Type type);
	}
}