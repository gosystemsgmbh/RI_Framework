using System;

using RI.Framework.Composition;




namespace RI.Framework.Data.EF.Resolvers
{
	/// <summary>
	///     Implements a dependency resolver for Entity Framework contexts which uses a <see cref="CompositionContainer" />.
	/// </summary>
	/// <remarks>
	///     <note type="note">
	///         The key parameter of <see cref="DependencyResolverDbDependencyResolver.GetService" /> and <see cref="DependencyResolverDbDependencyResolver.GetServices" /> is ignored.
	///     </note>
	/// </remarks>
	public sealed class CompositionContainerDbDependencyResolver : DependencyResolverDbDependencyResolver
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="CompositionContainerDbDependencyResolver" />.
		/// </summary>
		/// <param name="container"> The used composition container. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="container" /> is null. </exception>
		public CompositionContainerDbDependencyResolver (CompositionContainer container)
			: base(container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			this.Container = container;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used composition container.
		/// </summary>
		/// <value>
		///     The used composition container.
		/// </value>
		public CompositionContainer Container { get; }

		#endregion
	}
}
