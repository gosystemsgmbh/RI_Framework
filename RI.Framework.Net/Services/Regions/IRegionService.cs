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
	///         Names of regions are considered case-insensitive.
	///     </para>
	///     <para>
	///         Elements inserted into regions can be made aware of their status within a region by implementing the <see cref="IRegionElement" /> interface.
	///         Note that the handling of <see cref="IRegionElement" /> is to be implemented in the region adapters, not the region service itself.
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
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="InvalidOperationException"> The region specified by <paramref name="region" /> does not exist. </exception>
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
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="InvalidOperationException"> The region specified by <paramref name="region" /> does not exist. </exception>
		void AddElement (string region, object element);

		/// <summary>
		///     Adds a region and associates it with a container.
		/// </summary>
		/// <param name="region"> The name of the region. </param>
		/// <param name="container"> The container which represents the region. </param>
		/// <remarks>
		///     <note type="implement">
		///         An already existing region with the same name should be overwritten or removed first respectively.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="container" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The type of <paramref region="container" /> is not handled by any region adapter. </exception>
		void AddRegion (string region, object container);

		/// <summary>
		///     Removes all elements from a region.
		/// </summary>
		/// <param name="region"> The name of the region. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="InvalidOperationException"> The region specified by <paramref name="region" /> does not exist. </exception>
		void ClearElements (string region);

		/// <summary>
		///     Deactivates all elements in a region.
		/// </summary>
		/// <param name="region"> The name of the region. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="InvalidOperationException"> The region specified by <paramref name="region" /> does not exist. </exception>
		void DeactivateAllElements (string region);

		/// <summary>
		///     Gets all elements of a region.
		/// </summary>
		/// <param name="region"> The name of the region. </param>
		/// <returns>
		///     The list with all elements of the region.
		///     An empty list is returned if the region contains no elements.
		///     null is returned if the specified region does not exist.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		List<object> GetElements (string region);

		/// <summary>
		///     Gets the container associated with a region.
		/// </summary>
		/// <param name="region"> The name of the region. </param>
		/// <returns>
		///     The container associated with the region or null if the region does not exist.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		object GetRegionContainer (string region);

		/// <summary>
		///     Gets the first region name which is associated with a container.
		/// </summary>
		/// <param name="container"> The container. </param>
		/// <returns>
		///     The first name of the region associated with the container or null if no region is associated with the container.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="container" /> is null. </exception>
		string GetRegionName (object container);

		/// <summary>
		///     Gets all region names which are associated with a container.
		/// </summary>
		/// <param name="container"> The container. </param>
		/// <returns>
		///     The set with all names of the region associated with the container.
		///     An empty set is returned if no region is associated with the container.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="container" /> is null. </exception>
		HashSet<string> GetRegionNames (object container);

		/// <summary>
		///     Determines whether a region contains a specified element.
		/// </summary>
		/// <param name="region"> The name of the region. </param>
		/// <param name="element"> The element which is to be checked whether it is in the region. </param>
		/// <returns>
		///     true if the element is in the specified region, false otherwise. false is also returned if the specified region does not exist.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		bool HasElement (string region, object element);

		/// <summary>
		///     Determines whether a region exists.
		/// </summary>
		/// <param name="region"> The name of the region. </param>
		/// <returns>
		///     true if the region exists, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		bool HasRegion (string region);

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
		/// <exception cref="InvalidOperationException"> The specified region adapter is still in use. </exception>
		void RemoveAdapter (IRegionAdapter regionAdapter);

		/// <summary>
		///     Removes an element from a region.
		/// </summary>
		/// <param name="region"> The name of the region. </param>
		/// <param name="element"> The element to remove from the region. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="InvalidOperationException"> The region specified by <paramref name="region" /> does not exist. </exception>
		void RemoveElement (string region, object element);

		/// <summary>
		///     Removes a region and unassociates its container.
		/// </summary>
		/// <param name="region"> The name of the region. </param>
		/// <remarks>
		///     <note type="implement">
		///         Nothing should happen if the region does not exist.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		void RemoveRegion (string region);

		/// <summary>
		///     Sets the only element in a region and activates it.
		/// </summary>
		/// <param name="region"> The name of the region. </param>
		/// <param name="element"> The element which is to be set the only element in the region. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="InvalidOperationException"> The region specified by <paramref name="region" /> does not exist. </exception>
		void SetElement (string region, object element);
	}
}
