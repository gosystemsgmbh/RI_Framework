using RI.Framework.Data.Repository.Filter;




namespace RI.Framework.Data.Repository.Entities
{
	/// <summary>
	///     Implements a base class for entity filtering which defines default behaviour for <see cref="DbEntityBase" /> based entities.
	/// </summary>
	/// <typeparam name="T"> The type of entity this configuration filters. </typeparam>
	/// <remarks>
	///     <para>
	///         See <see cref="DbEntityFilterBase{T}" /> for more details.
	///     </para>
	/// </remarks>
	public abstract class DbEntityFilterBase <T> : EntityFilter<T>
		where T : DbEntityBase
	{
	}
}
