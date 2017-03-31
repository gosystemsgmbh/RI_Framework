using System;
using System.Collections.Generic;
using System.Windows;




namespace RI.Framework.Services.Resources.Converters
{
	/// <summary>
	///     Provides WPF specific utility/extension methods for the <see cref="IResourceService" /> type.
	/// </summary>
	public static class WpfResourceServiceExtensions
	{
		#region Static Methods

		/// <summary>
		///     Loads all the resource sets whose ID are in a specified sequence of resource set IDs and then updates a WPF resource dictionary.
		/// </summary>
		/// <param name="resourceService"> The resource service. </param>
		/// <param name="setIdsToLoad"> The resourceset IDs of the resource sets to load. </param>
		/// <param name="resourceDictionary"> The WPF resource dictionary to update with the resources of the loaded sets. </param>
		/// <remarks>
		///     <para>
		///         <see cref="SetLoadedSetIds" /> calls <see cref="IResourceServiceExtensions" />.<see cref="IResourceServiceExtensions.SetLoadedSetIds" /> to do the actual loading of resource sets.
		///     </para>
		///     <para>
		///         <paramref name="resourceDictionary" /> is always cleared before the resource sets are loaded and populated with all available raw resource values after the resource sets were loaded.
		///         Therefore, <paramref name="resourceDictionary" /> should not be the applications main resource dictionary but rather a separate merged resource dictionary which is dedicated for use with the resource service.
		///     </para>
		///     <para>
		///         If a raw resource value is of the type <see cref="ResourceDictionary" />, it is not added but merged but <paramref name="resourceDictionary" />.
		///     </para>
		/// </remarks>
		public static void SetLoadedSetIds (this IResourceService resourceService, IEnumerable<string> setIdsToLoad, ResourceDictionary resourceDictionary)
		{
			if (resourceService == null)
			{
				throw new ArgumentNullException(nameof(resourceService));
			}

			if (setIdsToLoad == null)
			{
				throw new ArgumentNullException(nameof(setIdsToLoad));
			}

			if (resourceDictionary == null)
			{
				throw new ArgumentNullException(nameof(resourceDictionary));
			}

			resourceDictionary.Clear();
			resourceDictionary.MergedDictionaries.Clear();

			resourceService.SetLoadedSetIds(setIdsToLoad, false);

			HashSet<string> resources = resourceService.GetAvailableResourceNames();
			foreach (string resource in resources)
			{
				object value = resourceService.GetRawValue(resource);
				if (value is ResourceDictionary)
				{
					resourceDictionary.MergedDictionaries.Add((ResourceDictionary)value);
				}
				else
				{
					resourceDictionary.Add(resource, value);
				}
			}
		}

		#endregion
	}
}
