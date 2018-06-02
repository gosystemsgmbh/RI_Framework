namespace RI.Framework.Data.Repository.Entities
{
	/// <summary>
	///     Allows an entity, when tracked by a <see cref="IRepositoryContext" />, to be aware of its validation errors.
	/// </summary>
	public interface IEntityErrorTracking
	{
		/// <summary>
		///     Called during entity validation to inform an entity about its errors.
		/// </summary>
		/// <param name="errors"> The errors of the entity. </param>
		void SetErrors (RepositorySetErrors errors);
	}
}
