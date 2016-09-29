namespace RI.Framework.Services.Regions
{
	/// <summary>
	///     Supports region awareness of elements activated or added to a region.
	/// </summary>
	public interface IRegionElement
	{
		/// <summary>
		///     Gets, if available, the sort index of the element in its container.
		/// </summary>
		/// <value>
		///     The sort index of the element in its container or null if no index is provided by the element.
		/// </value>
		int? SortIndex { get; }

		/// <summary>
		///     The element was activated.
		/// </summary>
		void Activated ();

		/// <summary>
		///     The element was deactivated.
		/// </summary>
		void Deactivated ();
	}
}
