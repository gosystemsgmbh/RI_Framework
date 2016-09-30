namespace RI.Framework.Services.Regions
{
	/// <summary>
	///     Supports region awareness of elements.
	/// </summary>
	/// <remarks>
	///     <see cref="IRegionElement" /> can be implemented by elements which are used with regions to make them aware of certain region operations being performed on them.
	/// </remarks>
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
		///     Determines whether the element allows being navigated away.
		/// </summary>
		/// <returns> </returns>
		bool CanNavigateFrom ();

		/// <summary>
		///     Determines whether the element allows being navigated to.
		/// </summary>
		/// <returns> </returns>
		bool CanNavigateTo ();

		/// <summary>
		///     The element was deactivated.
		/// </summary>
		void Deactivated ();

		/// <summary>
		///     The element was navigated away from.
		/// </summary>
		void NavigatedFrom ();

		/// <summary>
		///     The element was navigated to.
		/// </summary>
		void NavigatedTo ();
	}
}
