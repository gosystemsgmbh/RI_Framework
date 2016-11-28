using System.Collections.Generic;
using System.Linq;

using RI.Framework.Data.EF.Filter;




namespace RI.Framework.Data.EF
{
	/// <summary>
	///     Defines an interface for a repository set filter.
	/// </summary>
	/// <typeparam name="T"> The type of entities to be filtered. </typeparam>
	/// <remarks>
	///     <para>
	///         This interface is supported by the sets filter method and by <see cref="EntityFilter{T}" />.
	///         If the filter parameter implements <see cref="IRepositoryDbSetFilter{T}" />, <see cref="Filter" /> is used for filtering, applied before paging.
	///     </para>
	/// </remarks>
	public interface IRepositoryDbSetFilter <T>
		where T : class
	{
		/// <summary>
		///     Filters the repository set.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="set"> The set to be filtered. </param>
		/// <param name="customSequence"> The custom sequence of entities to be filtered or null if all entities from <paramref name="set" /> should be filtered. </param>
		/// <returns>
		///     The query provider which does the filtering or null if no filter shall be applied.
		/// </returns>
		IQueryable<T> Filter (RepositoryDbContext repository, RepositoryDbSet<T> set, IEnumerable<T> customSequence);
	}
}
