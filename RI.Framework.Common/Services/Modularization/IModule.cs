using RI.Framework.Composition.Model;




namespace RI.Framework.Services.Modularization
{
    /// <summary>
    ///     Defines the interface for a module.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A module implements a self-contained functionality or service of an application which is implemented separate of other modules and managed by an <see cref="IModuleService" />.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    /// TODO: Add support for dynamic multi-stage initialization
    [Export]
    public interface IModule
    {
        /// <summary>
        ///     Gets whether the module is initialized or not.
        /// </summary>
        /// <value>
        ///     true if the module is initialized, false otherwise or after the module was unloaded.
        /// </value>
        bool IsInitialized { get; }

        /// <summary>
        ///     Initializes the module.
        /// </summary>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IModuleService" /> implementation.
        ///     </note>
        /// </remarks>
        void Initialize ();

        /// <summary>
        ///     Unloads the module.
        /// </summary>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IModuleService" /> implementation.
        ///     </note>
        /// </remarks>
        void Unload ();
    }
}
