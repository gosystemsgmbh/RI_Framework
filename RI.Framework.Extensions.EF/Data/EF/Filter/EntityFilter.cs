using System;
using System.Collections.Generic;
using System.Linq;




namespace RI.Framework.Data.EF.Filter
{
	/// <summary>
	///     Implements a base class for entity filters.
	/// </summary>
	/// <typeparam name="T"> The type of entities this filter filters. </typeparam>
	/// <remarks>
	///     <para>
	///         See <see cref="IEntityFilter" /> for more details.
	///     </para>
	/// </remarks>
	public abstract class EntityFilter <T> : IEntityFilter
		where T : class
	{
		#region Abstracts

		/// <summary>
		///     Filters a repository set for entities of the type this entity filter supports.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="set"> The repository set to filter. </param>
		/// <param name="customSequence"> The custom sequence of entities to be filtered or null if all entities from <paramref name="set" /> should be filtered. </param>
		/// <param name="filter"> The filter object passed to the sets filter method. </param>
		/// <returns>
		///     The query provider which does the filtering or null if no filter shall be applied.
		/// </returns>
		/// <remarks>
		///     <para>
		///         Despite its name, an <see cref="EntityFilter{T}" /> can not only be used for filtering but also for sorting, depending on the applied entity type and the filter object (<paramref name="filter" />).
		///         This is also why <see cref="Filter" /> returns an <see cref="IOrderedQueryable{T}" /> instead of an <see cref="IQueryable{T}" />.
		///     </para>
		/// </remarks>
		public abstract IOrderedQueryable<T> Filter (RepositoryDbContext repository, RepositoryDbSet<T> set, IEnumerable<T> customSequence, object filter);

		#endregion




		#region Interface: IEntityFilter

		/// <inheritdoc />
		Type IEntityFilter.EntityType => typeof(T);

		#endregion
	}
}
