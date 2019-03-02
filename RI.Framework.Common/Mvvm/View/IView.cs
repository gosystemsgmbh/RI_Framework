using RI.Framework.Composition.Model;
using RI.Framework.Mvvm.ViewModel;




namespace RI.Framework.Mvvm.View
{
    /// <summary>
    ///     Defines an interface for views.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    [Export]
    public interface IView
    {
        /// <summary>
        ///     Gets or sets the view model used by the view.
        /// </summary>
        /// <value>
        ///     The view model used by the view.
        /// </value>
        IViewModel ViewModel { get; set; }

        /// <summary>
        ///     Gets whether the view is initialized or not.
        /// </summary>
        /// <value>
        ///     true if the view is initialized, false otherwise.
        /// </value>
        bool IsInitialized { get; }

        /// <summary>
        ///     Initializes the view.
        /// </summary>
        void Initialize ();
    }
}
