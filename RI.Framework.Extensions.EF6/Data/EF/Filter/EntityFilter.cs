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
		public abstract IOrderedQueryable<T> Filter (DbRepositoryContext repository, DbRepositorySet<T> set, IEnumerable<T> customSequence, object filter, object sort);

		#endregion




		#region Interface: IEntityFilter

		/// <inheritdoc />
		Type IEntityFilter.EntityType => typeof(T);

		/// <inheritdoc />
		IOrderedQueryable IEntityFilter.Filter (DbRepositoryContext repository, DbRepositorySet set, IEnumerable customSequence, object filter, object sort)
		{
			if (repository == null)
			{
				throw new ArgumentNullException(nameof(repository));
			}

			if (set == null)
			{
				throw new ArgumentNullException(nameof(set));
			}

			return this.Filter(repository, (DbRepositorySet<T>)set, (IEnumerable<T>)customSequence, filter, sort);
		}

		#endregion
	}
}
