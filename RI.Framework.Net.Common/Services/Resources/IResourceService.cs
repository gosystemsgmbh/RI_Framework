using System;
using System.Collections.Generic;
using System.Globalization;
using RI.Framework.Composition.Model;
using RI.Framework.Services.Resources.Converters;
using RI.Framework.Services.Resources.Sources;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;


namespace RI.Framework.Services.Resources
{
    /// <summary>
    ///     Defines the interface for a resource service.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A resource service is used to retrieve resources.
    ///     </para>
    ///     <para>
    ///         A resource service loads one or more specified <see cref="IResourceSet" />s from one or more available <see cref="IResourceSource" />s.
    ///     </para>
    ///     <para>
    ///         Names of resources are considered case-insensitive.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
	public interface IResourceService : ISynchronizable
	{
		/// <summary>
		///     Gets all currently available resource converters.
		/// </summary>
		/// <value>
		///     All currently available resource converters.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		IEnumerable<IResourceConverter> Converters { get; }

		/// <summary>
		///     Gets all currently used resource sources.
		/// </summary>
		/// <value>
		///     All currently used resource sources.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		IEnumerable<IResourceSource> Sources { get; }

		/// <summary>
		///     Adds a resource converter and starts using it for all subsequent conversions.
		/// </summary>
		/// <param name="resourceConverter"> The resource converter to add. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already added resource converter should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="resourceConverter" /> is null. </exception>
		void AddConverter (IResourceConverter resourceConverter);

		/// <summary>
		///     Adds a resource source.
		/// </summary>
		/// <param name="resourceSource"> The resource source to add. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already added resource source should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="resourceSource" /> is null. </exception>
		void AddSource (IResourceSource resourceSource);

	    /// <summary>
	    ///     Gets all available cultures.
	    /// </summary>
	    /// <returns>
	    ///     The hash set with all available cultures.
	    ///     If no cultures are available (e.g. none was specified in any resource set), an empty hash set is returned.
	    /// </returns>
	    HashSet<CultureInfo> GetAvailableCultures ();

        /// <summary>
        ///     Gets the names of all available resources.
        /// </summary>
        /// <returns>
        ///     The hash set with the names of all available resources.
        ///     If no resources are available, an empty hash set is returned.
        /// </returns>
        HashSet<string> GetAvailableResources ();

        /// <summary>
        ///     Gets all currently available resource sets.
        /// </summary>
        /// <returns>
        ///     The list with all available resource sets.
        ///     If no resource sets are available, an empty list is returned.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         The returned list must be sorted according to <see cref="IResourceSet.Priority"/>.
        ///     </note>
        /// </remarks>
        List<IResourceSet> GetAvailableSets ();

        /// <summary>
        ///     Gets all currently loaded resource sets.
        /// </summary>
        /// <returns>
        ///     The list with all loaded resource sets.
        ///     If no resource sets are loaded, an empty list is returned.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         The returned list must be sorted according to <see cref="IResourceSet.Priority"/>.
        ///     </note>
        /// </remarks>
        List<IResourceSet> GetLoadedSets ();

        /// <summary>
        ///     Gets a resource as its originally loaded type.
        /// </summary>
        /// <param name="name"> The name of the resource. </param>
        /// <returns>
        ///     The resource value or null if the resource is not available.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        object GetRawValue (string name);

        /// <summary>
        ///     Gets a resource for a specified culture as its originally loaded type.
        /// </summary>
        /// <param name="name"> The name of the resource. </param>
        /// <param name="culture"> The culture or null to lookup the resource solely based on resource set priority. </param>
        /// <returns>
        ///     The resource value or null if the resource is not available.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        object GetRawValue (string name, CultureInfo culture);

        /// <summary>
        ///     Gets a resource as a value of a certain type.
        /// </summary>
        /// <typeparam name="T"> The resource type. </typeparam>
        /// <param name="name"> The name of the resource. </param>
        /// <returns>
        ///     The resource value or the default value of <typeparamref name="T" /> if the resource is not available.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        /// <exception cref="InvalidTypeArgumentException"> The resource cannot be converted to type <typeparamref name="T" />. </exception>
        T GetValue <T> (string name);

        /// <summary>
        ///     Gets a resource for a specified culture as a value of a certain type.
        /// </summary>
        /// <typeparam name="T"> The resource type. </typeparam>
        /// <param name="name"> The name of the resource. </param>
        /// <param name="culture"> The culture or null to lookup the resource solely based on resource set priority. </param>
        /// <returns>
        ///     The resource value or the default value of <typeparamref name="T" /> if the resource is not available.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        /// <exception cref="InvalidTypeArgumentException"> The resource cannot be converted to type <typeparamref name="T" />. </exception>
        T GetValue <T> (string name, CultureInfo culture);

        /// <summary>
        ///     Gets a resource as a value of a certain type.
        /// </summary>
        /// <param name="type"> The resource type. </param>
        /// <param name="name"> The name of the resource. </param>
        /// <returns>
        ///     The resource value or the default value of <paramref name="type" /> if the resource is not available.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> or <paramref name="type" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        /// <exception cref="InvalidTypeArgumentException"> The resource cannot be converted to type <paramref name="type" />. </exception>
        object GetValue (string name, Type type);

        /// <summary>
        ///     Gets a resource for a specified culture as a value of a certain type.
        /// </summary>
        /// <param name="type"> The resource type. </param>
        /// <param name="name"> The name of the resource. </param>
        /// <param name="culture"> The culture or null to lookup the resource solely based on resource set priority. </param>
        /// <returns>
        ///     The resource value or the default value of <paramref name="type" /> if the resource is not available.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> or <paramref name="type" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        /// <exception cref="InvalidTypeArgumentException"> The resource cannot be converted to type <paramref name="type" />. </exception>
        object GetValue(string name, Type type, CultureInfo culture);

        /// <summary>
        ///     Loads a resource sets and makes its resources available.
        /// </summary>
        /// <param name="resourceSet"> The resource set to load. </param>
        /// <param name="lazyLoad"> Specifies whether lazy loading shall be used for the resources of this resource set or not. </param>
        /// <remarks>
        ///     <para>
        ///         Lazy loading means that the actual value of a resource is only loaded into memory and converted to the appropriate type when <see cref="GetRawValue(string)" /> or <see cref="GetRawValue(string,CultureInfo)"/> is called for it.
        ///     </para>
        ///     <note type="implement">
        ///         Loading an already loaded resource set can be used to reload a specific set.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="resourceSet" /> is null. </exception>
        void LoadSet (IResourceSet resourceSet, bool lazyLoad);

		/// <summary>
		///     Loads all currently available resource sets and makes its resources available.
		/// </summary>
		/// <param name="lazyLoad"> Specifies whether lazy loading shall be used for the resources or not. </param>
		/// <remarks>
		///     <para>
		///         Lazy loading means that the actual value of a resource is only loaded into memory and converted to the appropriate type when <see cref="GetRawValue(string)" /> or <see cref="GetRawValue(string,CultureInfo)"/> is called for it.
		///     </para>
		/// </remarks>
		void LoadSets (bool lazyLoad);

        /// <summary>
        ///     Reloads all currently loaded resource sets.
        /// </summary>
        void ReloadSets ();

		/// <summary>
		///     Removes a resource converter and stops using it for all subsequent conversions.
		/// </summary>
		/// <param name="resourceConverter"> The resource converter to remove. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already removed resource converter should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="resourceConverter" /> is null. </exception>
		void RemoveConverter (IResourceConverter resourceConverter);

		/// <summary>
		///     Removes a resource source.
		/// </summary>
		/// <param name="resourceSource"> The resource source to remove. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already removed resource source should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="resourceSource" /> is null. </exception>
		void RemoveSource (IResourceSource resourceSource);

		/// <summary>
		///     Unloads a resource set.
		/// </summary>
		/// <param name="resourceSet"> The resource set to unload. </param>
		/// <remarks>
		///     <note type="implement">
		///         Unloading an already unloaded resource set should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="resourceSet" /> is null. </exception>
		void UnloadSet (IResourceSet resourceSet);

		/// <summary>
		///     Unloads all currently loaded resource sets.
		/// </summary>
		void UnloadSets ();
	}
}
