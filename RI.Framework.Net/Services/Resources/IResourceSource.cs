using System.Collections.Generic;

using RI.Framework.Composition.Model;




namespace RI.Framework.Services.Resources
{
	/// <summary>
	///     Defines the interface for a resource source used by a resource service.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A resource source is used by a <see cref="IResourceService" /> to locate and load <see cref="IResourceSet" />.
	///     </para>
	/// </remarks>
	[Export]
	public interface IResourceSource
	{
		/// <summary>
		///     Gets all currently available resource sets of this resource source.
		/// </summary>
		/// <value>
		///     All currently available resource sets of this resource source.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		IEnumerable<IResourceSet> AvailableSets { get; }

		/// <summary>
		///     Gets whether the resource source is initialized or not.
		/// </summary>
		/// <value>
		///     true if the resource source is initialized, false otherwise or after the resource source was unloaded.
		/// </value>
		bool IsInitialized { get; }

		/// <summary>
		///     Initializes the resource source.
		/// </summary>
		void Initialize ();

		/// <summary>
		///     Unloads the resource source and all its loaded resource sets.
		/// </summary>
		void Unload ();

		/// <summary>
		///     Updates the available resource sets (<see cref="AvailableSets" />).
		/// </summary>
		void UpdateAvailableSets ();
	}
}
