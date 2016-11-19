using System.Linq;




namespace RI.Framework.Data.EF
{
	/// <summary>
	///     Defines an interface for a repository set filter.
	/// </summary>
	/// <typeparam name="T"> The type of entities to be filtered. </typeparam>
	/// <remarks>
	///     <para>
	///         This interface is supported by <see cref="RepositoryDbSet{T}.GetFiltered" />.
	///         If the filter parameter implements <see cref="IRepositoryDbSetFilter{T}" />, <see cref="Filter" /> is used for filtering, applied before paging.
	///     </para>
	/// </remarks>
	public interface IRepositoryDbSetFilter <T>
		where T : class
	{
		/// <summary>
		///     Filters the repository set.
		/// </summary>
		/// <param name="set"> The set to be filtered. </param>
		/// <returns>
		///     The query provider which does the filtering or null if no filter shall be applied.
		/// </returns>
		IQueryable<T> Filter (RepositoryDbSet<T> set);
	}
}
