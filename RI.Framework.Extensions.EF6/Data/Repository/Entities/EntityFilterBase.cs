using RI.Framework.Data.EF.Filter;

namespace RI.Framework.Data.Repository.Entities
{
	/// <summary>
	/// Implements a base class for entity filtering which defines default behaviour for <see cref="EntityBase"/> based entities.
	/// </summary>
	/// <typeparam name="T"> The type of entity this configuration filters. </typeparam>
	/// <remarks>
	///     <para>
	///         See <see cref="EntityFilterBase{T}" /> for more details.
	///     </para>
	/// </remarks>
	public abstract class EntityFilterBase<T> : EntityFilter<T>
		where T : EntityBase
	{
	}
}
