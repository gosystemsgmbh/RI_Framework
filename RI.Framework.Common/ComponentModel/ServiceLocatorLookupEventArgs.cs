using System;
using System.Collections.Generic;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.ComponentModel
{
    /// <summary>
    ///     Event arguments for the <see cref="ServiceLocator.Lookup" /> event.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    /// TODO: Make thread-safe
    public sealed class ServiceLocatorLookupEventArgs : EventArgs
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="ServiceLocatorLookupEventArgs" />.
        /// </summary>
        /// <param name="name"> The name to look up instances of. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        public ServiceLocatorLookupEventArgs (string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsNullOrEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            this.Name = name;
            this.Instances = new List<object>();
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the list of resolved instances.
        /// </summary>
        /// <value>
        ///     The list of resolved instances.
        /// </value>
        public List<object> Instances { get; }

        /// <summary>
        ///     Gets the name to look up instances of.
        /// </summary>
        /// <value>
        ///     The name to look up instances of.
        /// </value>
        public string Name { get; }

        #endregion
    }
}
