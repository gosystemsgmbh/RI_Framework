using System;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.ComponentModel
{
    /// <summary>
    ///     Event arguments for the <see cref="ServiceLocator.Translate" /> event.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    /// TODO: Make thread-safe
    public sealed class ServiceLocatorTranslationEventArgs : EventArgs
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="ServiceLocatorTranslationEventArgs" />.
        /// </summary>
        /// <param name="type"> The type to translate into a name for subsequent lookup. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
        public ServiceLocatorTranslationEventArgs (Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            this.Type = type;
            this.Name = null;
        }

        #endregion




        #region Instance Fields

        private string _name;

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets or sets the translated name.
        /// </summary>
        /// <value>
        ///     The translated name or null if the type could not be translated.
        /// </value>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="value" /> is an empty string. </exception>
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                if (value != null)
                {
                    if (value.IsNullOrEmptyOrWhitespace())
                    {
                        throw new EmptyStringArgumentException(nameof(value));
                    }
                }

                this._name = value;
            }
        }

        /// <summary>
        ///     Gets the type to translate into a name for subsequent lookup.
        /// </summary>
        /// <value>
        ///     The type to translate into a name for subsequent lookup.
        /// </value>
        public Type Type { get; }

        #endregion
    }
}
