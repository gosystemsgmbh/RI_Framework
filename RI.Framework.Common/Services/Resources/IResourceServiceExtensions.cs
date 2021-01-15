using System;
using System.Collections.Generic;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Services.Resources.Sources;
using RI.Framework.Utilities;




namespace RI.Framework.Services.Resources
{
    /// <summary>
    ///     Provides common utility/extension methods for the <see cref="IResourceService" /> type.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public static class IResourceServiceExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Gets the IDs of all loaded resource sets.
        /// </summary>
        /// <param name="resourceService"> The resource service. </param>
        /// <returns>
        ///     The hash set with the IDs of all loaded resource sets.
        ///     If no resource sets are loaded, an empty hash set is returned.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="resourceService" /> is null. </exception>
        public static HashSet<string> GetLoadedSetIds (this IResourceService resourceService)
        {
            if (resourceService == null)
            {
                throw new ArgumentNullException(nameof(resourceService));
            }

            HashSet<string> ids = new HashSet<string>(from x in resourceService.GetLoadedSets() select x.Id, StringComparerEx.InvariantCultureIgnoreCase);
            return ids;
        }

        /// <summary>
        ///     Loads all the resource sets whose ID are in a specified sequence of resource set IDs.
        /// </summary>
        /// <param name="resourceService"> The resource service. </param>
        /// <param name="setIdsToLoad"> The resourceset IDs of the resource sets to load. </param>
        /// <param name="lazyLoad"> Specifies whether lazy loading shall be used for the resources of this resource set or not. </param>
        /// <remarks>
        ///     <para>
        ///         <see cref="SetLoadedSetIds" /> calls <see cref="IResourceSet.Load" /> for all found sets which match with an ID from <paramref name="setIdsToLoad" />.
        ///         IDs in <paramref name="setIdsToLoad" /> which are not found in <see cref="IResourceService.GetAvailableSets" /> are ignored and will not be loaded.
        ///     </para>
        ///     <para>
        ///         The resource sets are loaded in addition to any already loaded sets.
        ///         No sets will be unloaded by <see cref="SetLoadedSetIds" />.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="resourceService" /> or <paramref name="setIdsToLoad" /> is null. </exception>
        public static void SetLoadedSetIds (this IResourceService resourceService, IEnumerable<string> setIdsToLoad, bool lazyLoad)
        {
            if (resourceService == null)
            {
                throw new ArgumentNullException(nameof(resourceService));
            }

            if (setIdsToLoad == null)
            {
                throw new ArgumentNullException(nameof(setIdsToLoad));
            }

            HashSet<string> idsToLoad = new HashSet<string>(setIdsToLoad, StringComparerEx.InvariantCultureIgnoreCase);

            lock (resourceService.SyncRoot)
            {
                IEnumerable<IResourceSet> setsToLoad = from x in resourceService.GetAvailableSets() where idsToLoad.Contains(x.Id) select x;
                foreach (IResourceSet setToLoad in setsToLoad)
                {
                    setToLoad.Load(lazyLoad);
                }
            }
        }

        #endregion
    }
}
