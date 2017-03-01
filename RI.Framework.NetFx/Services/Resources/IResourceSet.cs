using System;
using System.Collections.Generic;
using System.Globalization;

using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Resources
{
	/// <summary>
	///     Defines the interface for a resource set.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A resource set is managed by a <see cref="IResourceSource" />.
	///     </para>
	/// </remarks>
	public interface IResourceSet
	{
		/// <summary>
		///     Gets whether this resource set can be selected by the user.
		/// </summary>
		/// <value>
		///     true if this resource set can be selected by the user, false otherwise.
		/// </value>
		bool Selectable { get; }

		/// <summary>
		///     Gets whether this resource set shall always be loaded.
		/// </summary>
		/// <value>
		///     true if this resource set shall always be loaded, false otherwise.
		/// </value>
		bool AlwaysLoad { get; }

		/// <summary>
		///     Gets all the resource values provided by this resource set.
		/// </summary>
		/// <value>
		///     All the resource values provided by this resource set.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		IEnumerable<string> AvailableResources { get; }

		/// <summary>
		///     Gets the formatting culture which is associated with this resource set.
		/// </summary>
		/// <value>
		///     The formatting culture which is associated with this resource set or null if no formatting culture is associated with this resource set.
		/// </value>
		CultureInfo FormattingCulture { get; }

		/// <summary>
		///     Gets the group this resource set belongs to.
		/// </summary>
		/// <value>
		///     The group this resource set belongs to.
		/// </value>
		/// <remarks>
		///     <para>
		///         The group of a resource set is an application specified identifier to group resource sets.
		///     </para>
		/// </remarks>
		string Group { get; }

		/// <summary>
		///     Gets the ID of the resource set.
		/// </summary>
		/// <value>
		///     The ID of the resource set.
		/// </value>
		/// <remarks>
		///     <para>
		///         The ID of a resource set is not intended for displaying to the user.
		///         It is intended to uniquely identify a resource set.
		///     </para>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		string Id { get; }

		/// <summary>
		///     Gets whether the resource set was lazy loaded or not.
		/// </summary>
		/// <value>
		///     true if the resource set is currently loaded and lazy loading is used, false otherwise.
		/// </value>
		bool IsLazyLoaded { get; }

		/// <summary>
		///     Gets whether the resource set is currently loaded or not.
		/// </summary>
		/// <value>
		///     true if the resource set is currently loaded, false otherwise or after the resource set was unloaded.
		/// </value>
		bool IsLoaded { get; }

		/// <summary>
		///     Gets the name of the resource set.
		/// </summary>
		/// <value>
		///     The name of the resource set.
		/// </value>
		/// <remarks>
		///     <para>
		///         The name of a resource set is intended for displaying to the user.
		///         It should not be used to uniquely identify a resource set.
		///     </para>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		string Name { get; }

		/// <summary>
		///     Gets the priority of this resource set.
		/// </summary>
		/// <value>
		///     The priority of this resource set.
		/// </value>
		/// <remarks>
		///     <para>
		///         The priority of a resource set determines the order in which multiple resource sets are loaded.
		///         Resource sets with higher priority will be loaded after resource sets with lower priority and override resource values of resource sets with lower priorities.
		///     </para>
		/// </remarks>
		int Priority { get; }

		/// <summary>
		///     Gets the UI culture which is associated with this resource set.
		/// </summary>
		/// <value>
		///     The UI culture which is associated with this resource set or null if no UI culture is associated with this resource set.
		/// </value>
		CultureInfo UiCulture { get; }

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
		///     Loads this resource set.
		/// </summary>
		/// <param name="lazyLoad"> Specifies whether lazy loading shall be used for the resources of this resource set or not. </param>
		/// <returns>
		///     true if lazy loading is supported and used, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         Loading an already loaded resource set can be used to reload a specific set.
		///     </para>
		///     <para>
		///         Lazy loading means that the actual value of a resource is only loaded into memory and converted to the appropriate type when <see cref="GetRawValue" /> is called for it.
		///     </para>
		/// </remarks>
		bool Load (bool lazyLoad);

		/// <summary>
		///     Unloads this resource set.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         Unloading an already unloaded resource set should have no effect.
		///     </note>
		/// </remarks>
		void Unload ();

		/// <summary>
		///     Updates the available resources (<see cref="AvailableResources" />).
		/// </summary>
		void UpdateAvailable ();
	}
}
