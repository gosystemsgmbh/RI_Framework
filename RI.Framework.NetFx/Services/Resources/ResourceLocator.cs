using System;
using System.Collections.Generic;
using System.Globalization;

using RI.Framework.Services.Resources.Converters;
using RI.Framework.Services.Resources.Sources;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Resources
{
	/// <summary>
	///     Provides a centralized and global resource provider.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="ResourceLocator" /> is merely a convenience utility as it uses <see cref="ServiceLocator" /> to retrieve and use a <see cref="IResourceService" />.
	///     </para>
	///     <para>
	///         <see cref="ResourceLocator" /> has additional methods to retrieve resources for specific uses (e.g. text).
	///     </para>
	/// </remarks>
	public static class ResourceLocator
	{
		#region Static Properties/Indexer

		/// <inheritdoc cref="IResourceService.Converters" />
		public static IEnumerable<IResourceConverter> Converters => ResourceLocator.Service?.Converters ?? new IResourceConverter[0];

		/// <summary>
		///     Gets whether a resource service is available and can be used by <see cref="ResourceLocator" />.
		/// </summary>
		/// <value>
		///     true if a resource service is available and can be used by <see cref="ResourceLocator" />, false otherwise.
		/// </value>
		public static bool IsAvailable => ResourceLocator.Service != null;

		/// <summary>
		///     Gets the available resource service.
		/// </summary>
		/// <value>
		///     The available resource service or null if no resource service is available.
		/// </value>
		public static IResourceService Service => ServiceLocator.GetInstance<IResourceService>();

		/// <inheritdoc cref="IResourceService.Sources" />
		public static IEnumerable<IResourceSource> Sources => ResourceLocator.Service?.Sources ?? new IResourceSource[0];

		#endregion




		#region Static Methods

		/// <inheritdoc cref="IResourceService.GetAvailableResources" />
		public static HashSet<string> GetAvailableResources () => ResourceLocator.Service?.GetAvailableResources() ?? new HashSet<string>();

		/// <inheritdoc cref="IResourceService.GetAvailableSets" />
		public static List<IResourceSet> GetAvailableSets () => ResourceLocator.Service?.GetAvailableSets() ?? new List<IResourceSet>();

		/// <inheritdoc cref="IResourceService.GetLoadedSets" />
		public static List<IResourceSet> GetLoadedSets () => ResourceLocator.Service?.GetLoadedSets() ?? new List<IResourceSet>();

		/// <inheritdoc cref="IResourceService.GetRawValue" />
		public static object GetRawValue (string name) => ResourceLocator.Service?.GetRawValue(name);

		/// <summary>
		///     Gets a text resource as a string.
		/// </summary>
		/// <param name="name"> The name of the text resource. </param>
		/// <returns>
		///     The text string or null if the resource is not available.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		public static string GetText (string name)
		{
			return ResourceLocator.GetValue<string>(name);
		}

		/// <summary>
		///     Gets a text resource as a formatted string.
		/// </summary>
		/// <param name="name"> The name of the text resource to use as the format string. </param>
		/// <param name="formatProvider"> The format provider or null to use <see cref="CultureInfo.CurrentCulture" />. </param>
		/// <param name="args"> The formatting arguments. </param>
		/// <returns>
		///     The formatted string or null if the resource is not available.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		public static string GetTextFormat (string name, IFormatProvider formatProvider, params object[] args)
		{
			string format = ResourceLocator.GetText(name);
			if (format == null)
			{
				return null;
			}

			return string.Format(formatProvider ?? CultureInfo.CurrentCulture, format, args);
		}

		/// <inheritdoc cref="IResourceService.GetValue{T}(string)" />
		public static T GetValue <T> (string name) => ResourceLocator.IsAvailable ? ResourceLocator.Service.GetValue<T>(name) : default(T);

		/// <inheritdoc cref="IResourceService.GetValue(string,Type)" />
		public static object GetValue (string name, Type type) => ResourceLocator.Service?.GetValue(name, type);

		/// <inheritdoc cref="IResourceService.LoadSet" />
		public static void LoadSet (IResourceSet resourceSet, bool lazyLoad) => ResourceLocator.Service?.LoadSet(resourceSet, lazyLoad);

		/// <inheritdoc cref="IResourceService.ReloadSets" />
		public static void ReloadSets () => ResourceLocator.Service?.ReloadSets();

		/// <inheritdoc cref="IResourceService.UnloadSet" />
		public static void UnloadSet (IResourceSet resourceSet) => ResourceLocator.Service?.UnloadSet(resourceSet);

		/// <inheritdoc cref="IResourceService.UnloadSets" />
		public static void UnloadSets () => ResourceLocator.Service?.UnloadSets();

		#endregion
	}
}
