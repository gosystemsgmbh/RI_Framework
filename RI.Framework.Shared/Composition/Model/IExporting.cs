namespace RI.Framework.Composition.Model
{
	/// <summary>
	///     Defines an interface which allows exported types and objects to be informed when they are added to or removed from a <see cref="CompositionContainer" />.
	/// </summary>
	public interface IExporting
	{
		/// <summary>
		///     Called after the object has been added to a container as an export.
		///     Means: The object is now being used for composition and can now be resolved during imports.
		/// </summary>
		/// <param name="name"> The name under which the object is exported. </param>
		/// <param name="container"> The composition container the object was added to. </param>
		void AddedToContainer (string name, CompositionContainer container);

		/// <summary>
		///     Called after the object has been removed from a container as an export.
		///     Means: The object is no longer being used for composition and will no longer be resolved during imports.
		/// </summary>
		/// <param name="name"> The name under which the object was exported. </param>
		/// <param name="container"> The composition container the object was removed from. </param>
		void RemovedFromContainer (string name, CompositionContainer container);
	}
}
