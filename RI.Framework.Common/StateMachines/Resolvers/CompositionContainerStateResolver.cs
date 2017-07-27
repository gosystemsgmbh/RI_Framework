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
			:base(container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			this.Container = container;
		}

		#endregion




		#region Instance Properties/Indexer

		private CompositionContainer Container { get; }

		#endregion
	}
}
