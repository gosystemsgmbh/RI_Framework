using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.DependencyResolution;

using RI.Framework.Composition;




namespace RI.Framework.Data.EF
{
	/// <summary>
	///     Implements a dependency resolver for Entity Framework contexts which uses a <see cref="CompositionContainer" />.
	/// </summary>
	/// <remarks>
	///     <note type="note">
	///         The key parameter of <see cref="GetService" /> and <see cref="GetServices" /> is ignored.
	///     </note>
	/// </remarks>
	public sealed class CompositionContainerDbDependencyResolver : IDbDependencyResolver
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="CompositionContainerDbDependencyResolver" />.
		/// </summary>
		/// <param name="container"> The used composition container. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="container" /> is null. </exception>
		public CompositionContainerDbDependencyResolver (CompositionContainer container)
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
		public CompositionContainer Container { get; private set; }

		#endregion




		#region Interface: IDbDependencyResolver

		/// <inheritdoc />
		public object GetService (Type type, object key)
		{
			return this.Container.GetExport<object>(type);
		}

		/// <inheritdoc />
		public IEnumerable<object> GetServices (Type type, object key)
		{
			return this.Container.GetExports<object>(type);
		}

		#endregion
	}
}
