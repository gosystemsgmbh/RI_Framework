using System;




namespace RI.Framework.Data.Repository.Entities
{
	/// <summary>
	/// Allows an entity, when tracked by a <see cref="IRepositoryContext"/>, to be aware of its changes.
	/// </summary>
	public interface IEntityChangeTracking
	{
		/// <summary>
		/// Called for new entities during <see cref="IRepositoryContext.SaveChanges"/> or <see cref="IRepositoryContext.Commit"/>.
		/// </summary>
		/// <param name="changeTrackingContext">The change tracking context as resolved by <see cref="IRepositoryContext.ChangeTrackingContextResolve"/>.</param>
		/// <param name="timestamp">The timestamp of the creation.</param>
		void SetCreation (object changeTrackingContext, DateTime timestamp);

		/// <summary>
		/// Called for modified entities during <see cref="IRepositoryContext.SaveChanges"/> or <see cref="IRepositoryContext.Commit"/>.
		/// </summary>
		/// <param name="changeTrackingContext">The change tracking context as resolved by <see cref="IRepositoryContext.ChangeTrackingContextResolve"/>.</param>
		/// <param name="timestamp">The timestamp of the modification.</param>
		void SetModification (object changeTrackingContext, DateTime timestamp);
	}
}