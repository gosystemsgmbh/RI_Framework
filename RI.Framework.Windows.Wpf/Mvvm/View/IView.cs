using RI.Framework.Composition.Model;




namespace RI.Framework.Mvvm.View
{
	/// <summary>
	///     Defines an interface for views.
	/// </summary>
	[Export]
	public interface IView
	{
		/// <summary>
		///     Gets or sets the data context of the view.
		/// </summary>
		/// <value>
		///     The data context of the view.
		/// </value>
		object DataContext { get; set; }

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
