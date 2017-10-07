using RI.Framework.Services;




namespace RI.Framework.StateMachines.Resolvers
{
	/// <summary>
	///     Implements a state instance resolver which uses <see cref="ServiceLocator" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IStateResolver" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class ServiceLocatorStateResolver : DependencyResolverStateResolver
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ServiceLocatorStateResolver" />.
		/// </summary>
		public ServiceLocatorStateResolver ()
			: base(ServiceLocator.Resolver)
		{
		}

		#endregion
	}
}
