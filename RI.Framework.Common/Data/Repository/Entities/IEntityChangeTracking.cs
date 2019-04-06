using System;




namespace RI.Framework.Data.Repository.Entities
{
    /// <summary>
    ///     Allows an entity, when tracked by a <see cref="IRepositoryContext" />, to be aware of its own changes.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public interface IEntityChangeTracking
    {
        /// <summary>
        ///     Called for new entities during <see cref="IRepositoryContext.SaveChanges" /> or <see cref="IRepositoryContext.Commit" />.
        /// </summary>
        /// <param name="changeTrackingContext"> The change tracking context as provided by <see cref="IRepositoryContext" />. </param>
        /// <param name="timestamp"> The timestamp of the creation (that is: when <see cref="IRepositoryContext.SaveChanges"/> or <see cref="IRepositoryContext.Commit" /> was called). </param>
        void SetCreation (object changeTrackingContext, DateTime timestamp);

        /// <summary>
        ///     Called for modified entities during <see cref="IRepositoryContext.SaveChanges" /> or <see cref="IRepositoryContext.Commit" />.
        /// </summary>
        /// <param name="changeTrackingContext"> The change tracking context as provided by <see cref="IRepositoryContext" />. </param>
        /// <param name="timestamp"> The timestamp of the modification (that is: when <see cref="IRepositoryContext.SaveChanges"/> or <see cref="IRepositoryContext.Commit" /> was called). </param>
        void SetModification (object changeTrackingContext, DateTime timestamp);
    }
}
