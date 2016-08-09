using System;
using System.Collections.Generic;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Regions
{
	/// <summary>
	///     Defines the interface for a region service.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A region service is used to modularize the user interface of an application.
	///         The modularization is done by defining regions in the user interface into which modules of the application can insert their user interface content by only knowing the name of a region.
	///         This allows applications modules to implement their own user interface without any dependency of the overall user interface structure.
	///     </para>
	///     <para>
	///         Each region is represented by a container into which elements are inserted.
	///         The region service uses <see cref="IRegionAdapter" />s to do the mapping between the containers and elements so they do not have a dependency on each other.
	///         The supported types for containers and elements are defined by the available region adapters.
	///     </para>
	///     <para>
	///         Containers can optionally implement <see cref="IRegionContainer" /> to get notified when its elements are changed.
	///         Similarly, elements can optionally implement <see cref="IRegionElement" /> to get notified when they are being added/removed from a container.
	///     </para>
	///     <para>
	///         Names of settings are considered case-insensitive.
	///     </para>
	/// </remarks>
	[Export]
	public interface IRegionService
	{
		/// <summary>
		///     Gets all currently available region adapters.
		/// </summary>
		/// <value>
		///     All currently available region adapters.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		IEnumerable<IRegionAdapter> Adapters { get; }

		/// <summary>
		///     Gets all currently available regions.
		/// </summary>
		/// <value>
		///     All currently available regions.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		IEnumerable<string> Regions { get; }

		/// <summary>
		///     Activates an element in a region.
		/// </summary>
		/// <param name="region"> The name of the region. </param>
		/// <param name="element"> The element which is to be activated in the region. </param>
		/// <remarks>
		///     <note type="implement">
		///         Activating an already activated element should have no effect.
		///     </note>
		///     <note type="implement">
		///         The behaviour when the specified element is not in the specified region is undefined and shall be determined by the responsible region adapter.
		///         If the region adapter cannot deal with the situation of an unavailable element in the region, <see cref="InvalidOperationException" /> shall be thrown.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The type of <paramref name="element" /> is not handled by any region adapter. </exception>
		/// <exception cref="InvalidOperationException"> The region specified by <paramref name="region" /> does not exist or the element specified by <paramref name="element" /> does not exist in the region. </exception>
		void ActivateElement (string region, object element);

		/// <summary>
		///     Adds a region adapter.
		/// </summary>
		/// <param name="regionAdapter"> The region adapter to add. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already added region adapter should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="regionAdapter" /> is null. </exception>
		void AddAdapter (IRegionAdapter regionAdapter);

		/// <summary>
		///     Adds an element to a region.
		/// </summary>
		/// <param name="region"> The name of the region. </param>
		/// <param name="element"> The element to add to the region. </param>
		/// <remarks>
		///     <note type="implement">
		///         Adding an already added element should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The type of <paramref name="element" /> is not handled by any region adapter. </exception>
		/// <exception cref="InvalidOperationException"> The region specified by <paramref name="region" /> does not exist. </exception>
		void AddElement (string region, object element);

		/// <summary>
		///     Removes all elements from a region.
		/// </summary>
		/// <param name="region"> The name of the region. </param>
		/// <remarks>
		///     <note type="implement">
		///         Clearing an already empty region should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="InvalidOperationException"> The region specified by <paramref name="region" /> does not exist. </exception>
		void ClearElements (string region);

		/// <summary>
		///     Deletes a region and unassociates its container.
		/// </summary>
		/// <param name="name"> The name of the region. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		void DeleteRegion (string name);

		/// <summary>
		///     Gets the container associated with a region.
		/// </summary>
		/// <param name="name"> The name of the region. </param>
		/// <returns>
		///     The container associated with the region or null if the region does not exist.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		object GetRegion (string name);

		/// <summary>
		///     Gets the region name which is associated with a container.
		/// </summary>
		/// <param name="container"> The container. </param>
		/// <returns>
		///     The name of the region associated with the container or null if no region is associated with the container.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="container" /> is null. </exception>
		string GetRegionName (object container);

		/// <summary>
		///     Determines whether a region contains a specified element.
		/// </summary>
		/// <param name="region"> The name of the region. </param>
		/// <param name="element"> The element which is to be checked whether it is in the region. </param>
		/// <returns>
		///     true if the element is in the specified region, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The type of <paramref name="element" /> is not handled by any region adapter. </exception>
		/// <exception cref="InvalidOperationException"> The region specified by <paramref name="region" /> does not exist. </exception>
		bool HasElement (string region, object element);

		/// <summary>
		///     Removes a region adapter.
		/// </summary>
		/// <param name="regionAdapter"> The region adapter to remove. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already removed region adapter should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="regionAdapter" /> is null. </exception>
		void RemoveAdapter (IRegionAdapter regionAdapter);

		/// <summary>
		///     Removes an element from a region.
		/// </summary>
		/// <param name="region"> The name of the region. </param>
		/// <param name="element"> The element to remove from the region. </param>
		/// <remarks>
		///     <note type="implement">
		///         Removing an already removed element should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The type of <paramref name="element" /> is not handled by any region adapter. </exception>
		/// <exception cref="InvalidOperationException"> The region specified by <paramref name="region" /> does not exist. </exception>
		void RemoveElement (string region, object element);

		/// <summary>
		///     Sets the only element in a region.
		/// </summary>
		/// <param name="region"> The name of the region. </param>
		/// <param name="element"> The element which is to be set the only element in the region. </param>
		/// <remarks>
		///     <note type="implement">
		///         Setting an already set element should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The type of <paramref name="element" /> is not handled by any region adapter. </exception>
		/// <exception cref="InvalidOperationException"> The region specified by <paramref name="region" /> does not exist. </exception>
		void SetElement (string region, object element);

		/// <summary>
		///     Associates a region with a container.
		/// </summary>
		/// <param name="name"> The name of the region. </param>
		/// <param name="container"> The container which represents the region. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> or <paramref name="container" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The type of <paramref name="container" /> is not handled by any region adapter. </exception>
		void SetRegion (string name, object container);
	}
}
