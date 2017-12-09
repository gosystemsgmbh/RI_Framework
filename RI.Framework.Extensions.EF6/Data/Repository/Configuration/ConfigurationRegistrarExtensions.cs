using System;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq.Expressions;
using System.Reflection;

using RI.Framework.Composition;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Data.Repository.Configuration
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="ConfigurationRegistrar" /> type.
	/// </summary>
	public static class ConfigurationRegistrarExtensions
	{
		#region Static Methods

		/// <summary>
		///     Registers an entity configuration class instance.
		/// </summary>
		/// <param name="registrar"> The entity configuration registrar. </param>
		/// <param name="entityTypeConfiguration"> The entity configuration class instance to register. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="registrar" /> or <paramref name="entityTypeConfiguration" /> is null. </exception>
		public static void Add (this ConfigurationRegistrar registrar, IEntityConfiguration entityTypeConfiguration)
		{
			if (registrar == null)
			{
				throw new ArgumentNullException(nameof(registrar));
			}

			if (entityTypeConfiguration == null)
			{
				throw new ArgumentNullException(nameof(entityTypeConfiguration));
			}

			Expression<Func<ConfigurationRegistrar, ConfigurationRegistrar>> fakeExpression = x => x.Add((EntityTypeConfiguration<string>)null);
			MethodInfo fakeMethod = ((MethodCallExpression)fakeExpression.Body).Method;
			MethodInfo genericMethod = fakeMethod.GetGenericMethodDefinition();
			MethodInfo concreteMethod = genericMethod.MakeGenericMethod(entityTypeConfiguration.EntityType);
			concreteMethod.Invoke(registrar, new object[] {entityTypeConfiguration});
		}

		/// <summary>
		///     Registers entity configuration class instances from a composition container.
		/// </summary>
		/// <param name="registrar"> The entity configuration registrar. </param>
		/// <param name="container"> The composition container to use. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="registrar" /> or <paramref name="container" /> is null. </exception>
		public static void AddFromCompositionContainer (this ConfigurationRegistrar registrar, CompositionContainer container)
		{
			if (registrar == null)
			{
				throw new ArgumentNullException(nameof(registrar));
			}

			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			registrar.AddFromDependencyResolver(container);
		}

		/// <summary>
		///     Registers entity configuration class instances from a dependency resolver.
		/// </summary>
		/// <param name="registrar"> The entity configuration registrar. </param>
		/// <param name="resolver"> The dependency resolver to use. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="registrar" /> or <paramref name="resolver" /> is null. </exception>
		public static void AddFromDependencyResolver (this ConfigurationRegistrar registrar, IDependencyResolver resolver)
		{
			if (registrar == null)
			{
				throw new ArgumentNullException(nameof(registrar));
			}

			if (resolver == null)
			{
				throw new ArgumentNullException(nameof(resolver));
			}

			resolver.GetInstances<IEntityConfiguration>().ForEach(registrar.Add);
		}

		/// <summary>
		///     Registers entity configuration class instances from <see cref="ServiceLocator" />.
		/// </summary>
		/// <param name="registrar"> The entity configuration registrar. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="registrar" /> is null. </exception>
		public static void AddFromServiceLocator (this ConfigurationRegistrar registrar)
		{
			if (registrar == null)
			{
				throw new ArgumentNullException(nameof(registrar));
			}

			registrar.AddFromDependencyResolver(ServiceLocator.Resolver);
		}

		/// <summary>
		///     Registers entity configuration class instances from <see cref="Singleton" />.
		/// </summary>
		/// <param name="registrar"> The entity configuration registrar. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="registrar" /> is null. </exception>
		public static void AddFromSingleton (this ConfigurationRegistrar registrar)
		{
			if (registrar == null)
			{
				throw new ArgumentNullException(nameof(registrar));
			}

			registrar.AddFromDependencyResolver(Singleton.Resolver);
		}

		#endregion
	}
}
