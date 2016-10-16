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
		///     Loads a resource set.
		/// </summary>
		/// <param name="resourceSet"> The resource set to load. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already loaded resource set should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="resourceSet" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The resource set specified by <paramref name="resourceSet" /> is not in <see cref="AvailableSets" />. </exception>
		void LoadSet (IResourceSet resourceSet);

		/// <summary>
		///     Reloads all currently loaded resource sets (<see cref="LoadedSets" />).
		/// </summary>
		void ReloadLoadedSets ();

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
		///         Specifying an already unloaded resource set should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="resourceSet" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The resource set specified by <paramref name="resourceSet" /> is not in <see cref="AvailableSets" />. </exception>
		void UnloadSet (IResourceSet resourceSet);

		/// <summary>
		///     Updates the available resource sets (<see cref="AvailableSets" />).
		/// </summary>
		void UpdateAvailableSets ();
	}
}
