using RI.Framework.ComponentModel;




namespace RI.Framework.Data.EF.Resolvers
{
    /// <summary>
    ///     Implements a dependency resolver for Entity Framework contexts which uses <see cref="ServiceLocator" />.
    /// </summary>
    /// <remarks>
    ///     <note type="note">
    ///         The key parameter of <see cref="DependencyResolverDbDependencyResolver.GetService" /> and <see cref="DependencyResolverDbDependencyResolver.GetServices" /> is ignored.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class ServiceLocatorDbDependencyResolver : DependencyResolverDbDependencyResolver
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="ServiceLocatorDbDependencyResolver" />.
        /// </summary>
        public ServiceLocatorDbDependencyResolver ()
            : base(ServiceLocator.Resolver)
        {
        }

        #endregion
    }
}
