using System;
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
		#region Virtuals

		/// <summary>
		///     Filters a repository set for entities of the type this entity filter supports.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="set"> The repository set to filter. </param>
		/// <param name="filter"> The filter object specified passed to <see cref="RepositoryDbSet{T}.GetFiltered{TKey}" /> </param>
		/// <returns>
		///     The query provider which does the filtering or null if no filter shall be applied.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The default implementation uses <see cref="IRepositoryDbSetFilter{T}" /> if <paramref name="filter" /> derives from it or does no filtering otherwise.
		///     </para>
		/// </remarks>
		public virtual IQueryable<T> Filter (RepositoryDbContext repository, RepositoryDbSet<T> set, object filter)
		{
			return (filter as IRepositoryDbSetFilter<T>)?.Filter(repository, set);
		}

		#endregion




		#region Interface: IEntityFilter

		/// <inheritdoc />
		Type IEntityFilter.EntityType => typeof(T);

		#endregion
	}
}
