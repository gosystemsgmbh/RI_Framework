using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using RI.Framework.Collections;
using RI.Framework.Composition;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Data.Repository.Validation
{
	/// <summary>
	///     Implements a registrar which is used to register the entity validation classes which are to be used by a <see cref="DbRepositoryContext" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="ValidationRegistrar" /> is used during <see cref="DbRepositoryContext.OnValidatorsCreating" /> to register the entity validation classes.
	///     </para>
	/// </remarks>
	public sealed class ValidationRegistrar
	{
		#region Instance Constructor/Destructor

		internal ValidationRegistrar (IList<IEntityValidation> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException(nameof(list));
			}

			this.List = list;
		}

		#endregion




		#region Instance Properties/Indexer

		private IList<IEntityValidation> List { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Registers an entity validation class instance.
		/// </summary>
		/// <param name="entityTypeValidator"> The entity validation class instance to register. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entityTypeValidator" /> is null. </exception>
		public void Add (IEntityValidation entityTypeValidator)
		{
			if (entityTypeValidator == null)
			{
				throw new ArgumentNullException(nameof(entityTypeValidator));
			}

			this.List.Add(entityTypeValidator);
		}

		/// <summary>
		///     Registers an instance of each non-abstract entity validation class type deriving from <see cref="IEntityValidation" /> which can be found in an assembly.
		/// </summary>
		/// <param name="assembly"> The assembly to search for entity validation class types. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="assembly" /> is null. </exception>
		public void AddFromAssembly (Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException(nameof(assembly));
			}

			List<IEntityValidation> validators = (from x in assembly.GetTypes() where typeof(IEntityValidation).IsAssignableFrom(x) && (!x.IsAbstract) select (IEntityValidation)Activator.CreateInstance(x)).ToList();
			this.List.AddRange(validators);
		}

		/// <summary>
		///     Registers entity validation class instances from a composition container.
		/// </summary>
		/// <param name="container"> The composition container to use. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="container" /> is null. </exception>
		public void AddFromCompositionContainer (CompositionContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			this.AddFromDependencyResolver(container);
		}

		/// <summary>
		///     Registers entity validation class instances from a dependency resolver.
		/// </summary>
		/// <param name="resolver"> The dependency resolver to use. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="resolver" /> is null. </exception>
		public void AddFromDependencyResolver (IDependencyResolver resolver)
		{
			if (resolver == null)
			{
				throw new ArgumentNullException(nameof(resolver));
			}

			resolver.GetInstances<IEntityValidation>().ForEach(this.Add);
		}

		/// <summary>
		///     Registers entity validation class instances from <see cref="ServiceLocator" />.
		/// </summary>
		public void AddFromServiceLocator ()
		{
			this.AddFromDependencyResolver(ServiceLocator.Resolver);
		}

		/// <summary>
		///     Registers entity validation class instances from <see cref="Singleton" />.
		/// </summary>
		public void AddFromSingleton ()
		{
			this.AddFromDependencyResolver(Singleton.Resolver);
		}

		#endregion
	}
}
