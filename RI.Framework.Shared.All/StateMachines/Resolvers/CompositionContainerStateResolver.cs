using System;

using RI.Framework.Composition;




namespace RI.Framework.StateMachines.Resolvers
{
	/// <summary>
	///     Implements a state instance resolver which uses a <see cref="CompositionContainer" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IStateResolver" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class CompositionContainerStateResolver : DependencyResolverStateResolver
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="CompositionContainerStateResolver" />.
		/// </summary>
		/// <param name="container"> The used composition container </param>
		/// <exception cref="ArgumentNullException"> <paramref name="container" /> is null. </exception>
		public CompositionContainerStateResolver (CompositionContainer container)
			: base(container)
		{
		}

		#endregion
	}
}
