namespace RI.Framework.Services.Regions
{
	/// <summary>
	///     Defines an interface which allows element types to be informed when their container changes.
	/// </summary>
	public interface IRegionElement
	{
		/// <summary>
		///     Called after the element was activated in a container.
		/// </summary>
		/// <param name="container"> The container. </param>
		void ActivatedInContainer (object container);

		/// <summary>
		///     Called after the element was added to a container.
		/// </summary>
		/// <param name="container"> The container. </param>
		void AddedToContainer (object container);

		/// <summary>
		///     Called after the element was removed from a container.
		/// </summary>
		/// <param name="container"> The container. </param>
		void RemovedFromContainer (object container);

		/// <summary>
		///     Called after the element was set as the only element in a container.
		/// </summary>
		/// <param name="container"> The container. </param>
		void SetInContainer (object container);
	}
}
