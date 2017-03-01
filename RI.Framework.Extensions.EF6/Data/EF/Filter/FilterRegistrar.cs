using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using RI.Framework.Collections;




namespace RI.Framework.Data.EF.Filter
{
	/// <summary>
	///     Implements a registrar which is used to register the entity filter classes which are to be used by a <see cref="RepositoryDbContext" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="FilterRegistrar" /> is used during <see cref="RepositoryDbContext.OnFiltersCreating" /> to register the entity filter classes.
	///     </para>
	/// </remarks>
	public sealed class FilterRegistrar
	{
		#region Instance Constructor/Destructor

		internal FilterRegistrar (IList<IEntityFilter> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException(nameof(list));
			}

			this.List = list;
		}

		#endregion




		#region Instance Properties/Indexer

		private IList<IEntityFilter> List { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Registers an entity filter class instance.
		/// </summary>
		/// <param name="entityTypeFilter"> The entity filter class instance to register. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entityTypeFilter" /> is null. </exception>
		public void Add (IEntityFilter entityTypeFilter)
		{
			if (entityTypeFilter == null)
			{
				throw new ArgumentNullException(nameof(entityTypeFilter));
			}

			this.List.Add(entityTypeFilter);
		}

		/// <summary>
		///     Registers an instance of each non-abstract entity filter class type deriving from <see cref="IEntityFilter" /> which can be found in an assembly.
		/// </summary>
		/// <param name="assembly"> The assembly to search for entity filter class types. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="assembly" /> is null. </exception>
		public void AddFromAssembly (Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException(nameof(assembly));
			}

			List<IEntityFilter> filters = (from x in assembly.GetTypes() where typeof(IEntityFilter).IsAssignableFrom(x) && (!x.IsAbstract) select (IEntityFilter)Activator.CreateInstance(x)).ToList();
			this.List.AddRange(filters);
		}

		#endregion
	}
}
