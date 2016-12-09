using System;
using System.Collections.Generic;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities.Exceptions;




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
	[Export]
	public interface IResourceService
	{
		/// <summary>
		///     Gets all currently available resources.
		/// </summary>
		/// <value>
		///     All currently available resources.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		IEnumerable<string> AvailableResources { get; }

		/// <summary>
		///     Gets all currently available resource sets.
		/// </summary>
		/// <value>
		///     All currently available resource sets.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		IEnumerable<IResourceSet> AvailableSets { get; }

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
		///     Gets all currently loaded resources.
		/// </summary>
		/// <value>
		///     All currently loaded resources.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		IEnumerable<string> LoadedResources { get; }

		/// <summary>
		///     Gets all currently loaded resource sets.
		/// </summary>
		/// <value>
		///     All currently loaded resource sets.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		IEnumerable<IResourceSet> LoadedSets { get; }

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
		///     Determines whether a resource with a specified name is available.
		/// </summary>
		/// <param name="name"> The name of the resource to check. </param>
		/// <returns>
		///     true if the resource is available, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		bool HasValue (string name);

		/// <summary>
		///     Reloads all currently loaded resource sets (<see cref="LoadedSets" />).
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
		///     Updates the available resources (<see cref="AvailableResources" />) and resource sets (<see cref="AvailableSets" />).
		/// </summary>
		void UpdateAvailable ();
	}
}
