using System;

using RI.Framework.StateMachines.States;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.StateMachines.Resolvers
{
	/// <summary>
	///     Defines the interface for a state instance resolver.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="StateMachine" /> for more details about state machines.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public interface IStateResolver : ISynchronizable
	{
		/// <summary>
		///     Called when a state of the specified type needs to be resolved.
		/// </summary>
		/// <param name="type"> The state type to resolve. </param>
		/// <returns>
		///     The state instance or null if the state instance could not be resolved.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		IState ResolveState (Type type);
	}
}
