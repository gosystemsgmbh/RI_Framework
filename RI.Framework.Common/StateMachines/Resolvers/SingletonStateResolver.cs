using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.StateMachines.Resolvers
{
	/// <summary>
	///     Implements a state instance resolver which uses <see cref="Singleton" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IStateResolver" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class SingletonStateResolver : DependencyResolverStateResolver
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="SingletonStateResolver" />.
		/// </summary>
		public SingletonStateResolver ()
			: base(Singleton.Resolver)
		{
		}

		#endregion
	}
}
