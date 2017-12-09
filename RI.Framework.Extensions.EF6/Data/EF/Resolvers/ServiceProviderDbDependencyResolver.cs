using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.DependencyResolution;




namespace RI.Framework.Data.EF.Resolvers
{
	/// <summary>
	///     Implements a dependency resolver for Entity Framework contexts which uses an <see cref="IServiceProvider" />.
	/// </summary>
	/// <remarks>
	///     <note type="note">
	///         The key parameter of <see cref="GetService" /> and <see cref="GetServices" /> is ignored.
	///     </note>
	///     <note type="note">
	///         <see cref="GetServices" /> only returns one instance at maximum as <see cref="IServiceProvider" /> is only designed to resolve single instances.
	///     </note>
	/// </remarks>
	public class ServiceProviderDbDependencyResolver : IDbDependencyResolver
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ServiceProviderDbDependencyResolver" />.
		/// </summary>
		/// <param name="serviceProvider"> The used service provider. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="serviceProvider" /> is null. </exception>
		public ServiceProviderDbDependencyResolver (IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
			{
				throw new ArgumentNullException(nameof(serviceProvider));
			}

			this.ServiceProvider = serviceProvider;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used service provider.
		/// </summary>
		/// <value>
		///     The used service provider.
		/// </value>
		public IServiceProvider ServiceProvider { get; }

		#endregion




		#region Interface: IDbDependencyResolver

		/// <inheritdoc />
		public object GetService (Type type, object key)
		{
			return this.ServiceProvider.GetService(type);
		}

		/// <inheritdoc />
		public IEnumerable<object> GetServices (Type type, object key)
		{
			List<object> instances = new List<object>();
			object instance = this.ServiceProvider.GetService(type);
			if (instance != null)
			{
				instances.Add(instance);
			}
			return instances;
		}

		#endregion
	}
}
