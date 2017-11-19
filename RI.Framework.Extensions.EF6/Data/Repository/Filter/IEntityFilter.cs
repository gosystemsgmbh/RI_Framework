using System;
using System.Collections;
using System.Linq;

using RI.Framework.Composition.Model;




namespace RI.Framework.Data.Repository.Filter
{
	/// <summary>
	///     Defines the interface for entity filter classes.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Entity filter classes are used to filter entities in the context of an <see cref="DbRepositorySet{T}" />.
	///     </para>
	///     <para>
	///         Entity filter classes are created during <see cref="DbRepositoryContext.OnFiltersCreating" />.
	///     </para>
	/// </remarks>
	[Export]
	public interface IEntityFilter
	{
		/// <summary>
		///     Gets the type of entities this entity filter filters.
		/// </summary>
		/// <value>
		///     The type of entities this entity filter filters.
		/// </value>
		Type EntityType { get; }

		/// <summary>
		///     Filters a repository set for entities of the type this entity filter supports.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="set"> The repository set to filter. </param>
		/// <param name="customSequence"> The custom sequence of entities to be filtered or null if all entities from <paramref name="set" /> should be filtered. </param>
		/// <param name="filter"> The filter object passed to the sets filter method. </param>
		/// <param name="sort"> The sorting object passed to the sets filter method. </param>
		/// <returns>
		///     The query provider which does the filtering or null if no filter shall be applied.
		/// </returns>
		/// <remarks>
		///     <para>
		///         Despite its name, an <see cref="IEntityFilter" /> can not only be used for filtering but also for sorting, depending on the applied entity type and the filter object (<paramref name="filter" />).
		///         This is also why <see cref="Filter" /> returns an <see cref="IOrderedQueryable" /> instead of an <see cref="IQueryable" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="repository" /> or <paramref name="set" /> is null. </exception>
		IOrderedQueryable Filter (DbRepositoryContext repository, DbRepositorySet set, IEnumerable customSequence, object filter, object sort);
	}
}
