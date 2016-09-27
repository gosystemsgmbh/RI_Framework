namespace RI.Framework.Services.Regions
{
	/// <summary>
	///     Supports region awareness of elements activated or added to a region.
	/// </summary>
	public interface IRegionElement
	{
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
