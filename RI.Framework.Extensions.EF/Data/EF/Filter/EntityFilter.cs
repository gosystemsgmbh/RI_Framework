using System;
using System.Collections;
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

		/// <inheritdoc cref="IEntityFilter.Filter" />
		public abstract IOrderedQueryable<T> Filter (RepositoryDbContext repository, RepositoryDbSet<T> set, IEnumerable<T> customSequence, object filter);

		/// <inheritdoc />
		IOrderedQueryable IEntityFilter.Filter(RepositoryDbContext repository, RepositoryDbSet set, IEnumerable customSequence, object filter)
		{
			return this.Filter(repository, (RepositoryDbSet<T>)set, (IEnumerable<T>)customSequence, filter);
		}

		#endregion




		#region Interface: IEntityFilter

		/// <inheritdoc />
		Type IEntityFilter.EntityType => typeof(T);

		#endregion
	}
}
