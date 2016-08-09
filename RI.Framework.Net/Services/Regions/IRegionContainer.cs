namespace RI.Framework.Services.Regions
{
	/// <summary>
	///     Defines an interface which allows container types to be informed about element changes.
	/// </summary>
	public interface IRegionContainer
	{
		/// <summary>
		///     Called after an element was activated in the container.
		/// </summary>
		/// <param name="element"> The activated element. </param>
		void ElementActivated (object element);

		/// <summary>
		///     Called after an element was added to the container.
		/// </summary>
		/// <param name="element"> The added element. </param>
		void ElementAdded (object element);

		/// <summary>
		///     Called after an element was removed from the container.
		/// </summary>
		/// <param name="element"> The removed element. </param>
		void ElementRemoved (object element);

		/// <summary>
		///     Called after an element was set as the only element in the container.
		/// </summary>
		/// <param name="element"> The set element. </param>
		void ElementSet (object element);
	}
}
