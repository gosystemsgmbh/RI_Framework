using System;

using RI.Framework.ComponentModel;
using RI.Framework.Composition.Model;
using RI.Framework.StateMachines.States;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.StateMachines.Resolvers
{
    /// <summary>
    ///     Implements a state instance resolver which uses an <see cref="IDependencyResolver" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See <see cref="IStateResolver" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
    public class DependencyResolverStateResolver : IStateResolver
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DependencyResolverStateResolver" />.
        /// </summary>
        /// <param name="resolver"> The used dependency resolver. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="resolver" /> is null. </exception>
        public DependencyResolverStateResolver (IDependencyResolver resolver)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            this.Resolver = resolver;

            this.SyncRoot = new object();
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        /// Gets the used <see cref="IDependencyResolver"/>.
        /// </summary>
        /// <value>
        /// The used <see cref="IDependencyResolver"/>.
        /// </value>
        public IDependencyResolver Resolver { get; }

        #endregion




        #region Interface: IStateResolver

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        public object SyncRoot { get; }

        /// <inheritdoc />
        public IState ResolveState (Type type)
        {
            lock (this.SyncRoot)
            {
                return this.Resolver.GetInstance(type) as IState;
            }
        }

        #endregion
    }
}
