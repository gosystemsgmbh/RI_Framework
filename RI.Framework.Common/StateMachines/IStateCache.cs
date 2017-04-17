using System;
using System.Collections.Generic;




namespace RI.Framework.StateMachines
{
	/// <summary>
	///     Defines the interface for a state instance cache.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="StateMachine" /> for more details about state machines.
	///     </para>
	/// </remarks>
	public interface IStateCache
	{
		/// <summary>
		///     Adds a state instance to the cache.
		/// </summary>
		/// <param name="state"> The state instance to add. </param>
		/// <remarks>
		///     <note type="implement">
		///         If the cache does already contain the state instance, nothing should happen.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="state" /> is null. </exception>
		void AddState (IState state);

		/// <summary>
		///     Removes all state instances from the cache.
		/// </summary>
		void Clear ();

		/// <summary>
		///     Determines whether the cache contains a state instance of a specified state type.
		/// </summary>
		/// <param name="state"> The state type. </param>
		/// <returns>
		///     true if the cache contains a state instance for the state type, false otherwise.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         State instances in the cache which derive from <paramref name="state" /> should not be considered.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="state" /> is null. </exception>
		bool ContainsState (Type state);

		/// <summary>
		///     Gets the state instance of a specified state type.
		/// </summary>
		/// <param name="state"> The state type. </param>
		/// <returns>
		///     The state instance or null if the cache does not contain a state instance for <paramref name="state" />.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         State instances in the cache which derive from <paramref name="state" /> should not be considered.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="state" /> is null. </exception>
		/// <exception cref="KeyNotFoundException"> <paramref name="state" /> was not found in the cache. </exception>
		IState GetState (Type state);

		/// <summary>
		///     Removes a state instance from the cache.
		/// </summary>
		/// <param name="state"> The state instance to remove. </param>
		/// <remarks>
		///     <note type="implement">
		///         If the cache does not contain the state instance, nothing should happen.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="state" /> is null. </exception>
		void RemoveState (IState state);
	}
}
