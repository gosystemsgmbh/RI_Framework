using System;
using System.Collections.Generic;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Regions
{
	/// <summary>
	///     Defines the interface for a region adapter.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A region adapter is used by a <see cref="IRegionService" /> to map between its containers (representing regions) and the elements added to it.
	///     </para>
	/// </remarks>
	[Export]
	public interface IRegionAdapter
	{
		/// <summary>
		///     Activates an element in a container.
		/// </summary>
		/// <param name="container"> The container. </param>
		/// <param name="element"> The element. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="container" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The type of <paramref name="container" /> or <paramref name="element" /> is not handled by this region adapter. </exception>
		void Activate (object container, object element);

		/// <summary>
		///     Adds an element to a container.
		/// </summary>
		/// <param name="container"> The container. </param>
		/// <param name="element"> The element. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="container" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The type of <paramref name="container" /> or <paramref name="element" /> is not handled by this region adapter. </exception>
		void Add (object container, object element);

		/// <summary>
		///     Removes all elements of a container
		/// </summary>
		/// <param name="container"> The container. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="container" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The type of <paramref name="container" /> is not handled by this region adapter. </exception>
		void Clear (object container);

		/// <summary>
		///     Deactivates an element in a container.
		/// </summary>
		/// <param name="container"> The container. </param>
		/// <param name="element"> The element. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="container" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The type of <paramref name="container" /> or <paramref name="element" /> is not handled by this region adapter. </exception>
		void Deactivate (object container, object element);

		/// <summary>
		///     Gets all elements of a container
		/// </summary>
		/// <param name="container"> The container. </param>
		/// <returns>
		///     The list which contains all the elements of the container.
		///     An empty list is returned if the container contains no elements.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="container" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The type of <paramref name="container" /> is not handled by this region adapter. </exception>
		List<object> Get (object container);

		/// <summary>
		///     Checks whether a specified type is supported by this region adapter for containers.
		/// </summary>
		/// <param name="type"> The type of the container. </param>
		/// <param name="inheritanceDepth"> Returns the depth of the inheritance this region adapter supports the specified type. </param>
		/// <returns>
		///     true if this region adapter can use the specified type as container, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		bool IsCompatibleContainer (Type type, out int inheritanceDepth);

		/// <summary>
		///     Removes an element from a container.
		/// </summary>
		/// <param name="container"> The container. </param>
		/// <param name="element"> The element. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="container" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The type of <paramref name="container" /> or <paramref name="element" /> is not handled by this region adapter. </exception>
		void Remove (object container, object element);
	}
}
