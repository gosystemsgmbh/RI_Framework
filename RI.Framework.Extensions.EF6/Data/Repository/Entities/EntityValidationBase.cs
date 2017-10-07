using RI.Framework.Data.EF.Validation;




namespace RI.Framework.Data.Repository.Entities
{
	/// <summary>
	///     Implements a base class for entity validation which defines default behaviour for <see cref="EntityBase" /> based entities.
	/// </summary>
	/// <typeparam name="T"> The type of entity this configuration validates. </typeparam>
	/// <remarks>
	///     <para>
	///         See <see cref="EntityValidation{T}" /> for more details.
	///     </para>
	/// </remarks>
	public abstract class EntityValidationBase <T> : EntityValidation<T>
		where T : EntityBase
	{
	}
}
