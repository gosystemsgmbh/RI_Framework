using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.DependencyResolution;

using RI.Framework.ComponentModel;




namespace RI.Framework.Data.EF.Resolvers
{
	/// <summary>
	///     Implements a dependency resolver for Entity Framework contexts which uses an <see cref="IDependencyResolver" />.
	/// </summary>
	/// <remarks>
	///     <note type="note">
	///         The key parameter of <see cref="GetService" /> and <see cref="GetServices" /> is ignored.
	///     </note>
	/// </remarks>
	public class DependencyResolverDbDependencyResolver : IDbDependencyResolver
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DependencyResolverDbDependencyResolver" />.
		/// </summary>
		/// <param name="resolver"> The used dependency resolver. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="resolver" /> is null. </exception>
		public DependencyResolverDbDependencyResolver (IDependencyResolver resolver)
		{
			if (resolver == null)
			{
				throw new ArgumentNullException(nameof(resolver));
			}

			this.Resolver = resolver;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used dependency resolver.
		/// </summary>
		/// <value>
		///     The used dependency resolver.
		/// </value>
		public IDependencyResolver Resolver { get; }

		#endregion




		#region Interface: IDbDependencyResolver

		/// <inheritdoc />
		public object GetService (Type type, object key)
		{
			return this.Resolver.GetInstance(type);
		}

		/// <inheritdoc />
		public IEnumerable<object> GetServices (Type type, object key)
		{
			return this.Resolver.GetInstances(type);
		}

		#endregion
	}
}
