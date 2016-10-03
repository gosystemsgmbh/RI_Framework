namespace RI.Framework.Mvvm.View
{
	/// <summary>
	///     Defines an interface for views.
	/// </summary>
	public interface IView
	{
		/// <summary>
		///     Gets whether the view model is initialized or not.
		/// </summary>
		/// <value>
		///     true if the view model is initialized, false otherwise.
		/// </value>
		bool IsInitialized { get; }

		/// <summary>
		///     Initializes the view.
		/// </summary>
		void Initialize ();
	}
}
