using System;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Composition
{
    /// <summary>
    ///     Used to encapsulate a single export managed by a <see cref="CompositionCatalog" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         An export can only have either an assigned type (using the <see cref="Type" /> property), an object (using the <see cref="Value" /> property), or a factory (using the <see cref="Factory" /> property), but not more than one.
    ///         Which one is used depends on the <see cref="CompositionCatalog" /> which is managing the export.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public sealed class CompositionCatalogItem
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="CompositionCatalogItem" />.
        /// </summary>
        /// <param name="name"> The name under which the export is exported. </param>
        /// <param name="type"> The type which is exported. </param>
        /// <param name="privateExport"> Specifies whether the type export is a private export (false) or a shared export (true). </param>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> or <paramref name="type" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        public CompositionCatalogItem (string name, Type type, bool privateExport)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            this.Name = name;
            this.Type = type;
            this.Value = null;
            this.Factory = null;
            this.PrivateExport = privateExport;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CompositionCatalogItem" />.
        /// </summary>
        /// <param name="name"> The name under which the export is exported. </param>
        /// <param name="value"> The object which is exported. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> or <paramref name="value" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        public CompositionCatalogItem (string name, object value)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.Name = name;
            this.Type = null;
            this.Value = value;
            this.Factory = null;
            this.PrivateExport = false;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CompositionCatalogItem" />.
        /// </summary>
        /// <param name="name"> The name under which the export is exported. </param>
        /// <param name="factory"> The factory which creates the export. </param>
        /// <param name="privateExport"> Specifies whether the type export is a private export (false) or a shared export (true). </param>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> or <paramref name="factory" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        public CompositionCatalogItem (string name, Delegate factory, bool privateExport)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            this.Name = name;
            this.Type = null;
            this.Value = null;
            this.Factory = factory;
            this.PrivateExport = privateExport;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the factory which creates the export.
        /// </summary>
        /// <value>
        ///     The factory which creates the export or null if a factory is not used.
        /// </value>
        public Delegate Factory { get; }

        /// <summary>
        ///     Gets the name under which the export is exported.
        /// </summary>
        /// <value>
        ///     The name under which the export is exported.
        /// </value>
        public string Name { get; }

        /// <summary>
        ///     Gets whether the export is private or shared.
        /// </summary>
        /// <value>
        ///     true if the export is private, false if the export is shared.
        /// </value>
        public bool PrivateExport { get; }

        /// <summary>
        ///     Gets the type which is exported.
        /// </summary>
        /// <value>
        ///     The type which is exported or null if a type is not exported.
        /// </value>
        public Type Type { get; }

        /// <summary>
        ///     Gets the object which is exported.
        /// </summary>
        /// <value>
        ///     The object which is exported or null an instance is not exported.
        /// </value>
        public object Value { get; }

        #endregion
    }
}
