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
		///     Gets the container type this region adapter supports.
		/// </summary>
		/// <value>
		///     The container type this region adapter supports.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		Type ContainerType { get; }

		/// <summary>
		///     Gets all element types this region adapter supports.
		/// </summary>
		/// <value>
		///     All element types this region adapter supports.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		IEnumerable<Type> ElementTypes { get; }

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
		///     Checks whether a container contains an element.
		/// </summary>
		/// <param name="container"> The container. </param>
		/// <param name="element"> The element. </param>
		/// <returns>
		///     true if the container contains the element, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="container" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The type of <paramref name="container" /> or <paramref name="element" /> is not handled by this region adapter. </exception>
		bool Contains (object container, object element);

		/// <summary>
		///     Removes an element from a container.
		/// </summary>
		/// <param name="container"> The container. </param>
		/// <param name="element"> The element. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="container" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The type of <paramref name="container" /> or <paramref name="element" /> is not handled by this region adapter. </exception>
		void Remove (object container, object element);

		/// <summary>
		///     Sets the only element of a container.
		/// </summary>
		/// <param name="container"> The container. </param>
		/// <param name="element"> The element. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="container" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The type of <paramref name="container" /> or <paramref name="element" /> is not handled by this region adapter. </exception>
		void Set (object container, object element);
	}
}
