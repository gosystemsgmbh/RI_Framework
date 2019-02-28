using System;
using System.Collections.Generic;

using RI.Framework.Composition.Model;
using RI.Framework.Services.Resources.Converters;
using RI.Framework.Utilities.ObjectModel;


namespace RI.Framework.Services.Resources.Sources
{
    /// <summary>
    ///     Defines the interface for a resource source.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A resource source is used by a <see cref="IResourceService" /> to locate and load <see cref="IResourceSet" />s.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
    public interface IResourceSource : ISynchronizable
    {
        /// <summary>
        ///     Gets whether the resource source is initialized or not.
        /// </summary>
        /// <value>
        ///     true if the resource source is initialized, false otherwise or after the resource source was unloaded.
        /// </value>
        bool IsInitialized { get; }

        /// <summary>
        ///     Gets all currently available resource sets of this resource source.
        /// </summary>
        /// <returns>
        ///     The list with all available resource sets of this resource source.
        ///     If no resource sets are available, an empty list is returned.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IResourceService" /> implementation.
        ///     </note>
        /// </remarks>
        List<IResourceSet> GetAvailableSets ();

        /// <summary>
        ///     Initializes the resource source but does not load any resource sets.
        /// </summary>
        /// <param name="converters"> The sequence which gets the currently used resource converters of the associated <see cref="IResourceService" />. </param>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IResourceService" /> implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="converters" /> is null. </exception>
        void Initialize (IEnumerable<IResourceConverter> converters);

        /// <summary>
        ///     Unloads the resource source and all its loaded resource sets.
        /// </summary>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IResourceService" /> implementation.
        ///     </note>
        /// </remarks>
        void Unload ();

        /// <summary>
        ///     Updates the available resource converters provided by the resource service.
        /// </summary>
        /// <param name="converters"> The sequence which gets the currently used resource converters of the associated <see cref="IResourceService" />. </param>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IResourceService" /> implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="converters" /> is null. </exception>
        void UpdateConverters (IEnumerable<IResourceConverter> converters);
    }
}
