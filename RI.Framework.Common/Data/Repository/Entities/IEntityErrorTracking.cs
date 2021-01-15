namespace RI.Framework.Data.Repository.Entities
{
    /// <summary>
    ///     Allows an entity, when tracked by a <see cref="IRepositoryContext" />, to be aware of its own validation errors.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public interface IEntityErrorTracking
    {
        /// <summary>
        ///     Called during entity validation to inform an entity about its validation errors.
        /// </summary>
        /// <param name="errors"> The errors of the entity (null if the entity has no validation errors). </param>
        void SetErrors (RepositorySetErrors errors);
    }
}
