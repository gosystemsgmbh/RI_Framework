using System;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Composition.Model
{
    /// <summary>
    ///     Defines under which name a type is exported when used in model-based exporting (using <see cref="CompositionCatalog" />).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Multiple <see cref="ExportAttribute" /> can be used to export a type under more than one name.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public sealed class ExportAttribute : Attribute
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Exports a type under its default name.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         See <see cref="CompositionContainer" /> for more details about the default name of a type.
        ///     </para>
        /// </remarks>
        public ExportAttribute ()
        {
            this.Name = null;
            this.Inherited = true;
            this.Private = false;
        }

        /// <summary>
        ///     Exports a type under the specified types default name.
        /// </summary>
        /// <param name="type"> The type under whose default name the type is exported. </param>
        /// <remarks>
        ///     <para>
        ///         See <see cref="CompositionContainer" /> for more details about the default name of a type.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
        public ExportAttribute (Type type)
            : this()
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            this.Name = CompositionContainer.GetNameOfType(type);
        }

        /// <summary>
        ///     Exports a type under the specified name.
        /// </summary>
        /// <param name="name"> The name under which the type is exported. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        public ExportAttribute (string name)
            : this()
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            this.Name = name;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets or sets whether types, inheriting from the type this <see cref="ExportAttribute" /> is decorating, are also exported under the name specified by this <see cref="ExportAttribute" />.
        /// </summary>
        /// <value>
        ///     true if the this <see cref="ExportAttribute" /> is inherited, false otherwise.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is true.
        ///     </para>
        /// </remarks>
        public bool Inherited { get; set; }

        /// <summary>
        ///     Gets the name under which the decorated type is exported.
        /// </summary>
        /// <value>
        ///     The name under which the decorated type is exported.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets or sets whether this export is a private export or not.
        /// </summary>
        /// <value>
        ///     true if the export is private, false if the export is shared.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is false.
        ///     </para>
        /// </remarks>
        public bool Private { get; set; }

        #endregion
    }
}
