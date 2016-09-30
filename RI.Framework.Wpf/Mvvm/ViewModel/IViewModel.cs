using System.ComponentModel;




namespace RI.Framework.Mvvm.ViewModel
{
	/// <summary>
	///     Defines an interface for view models.
	/// </summary>
	public interface IViewModel : INotifyPropertyChanged
	{
		bool IsInitialized { get; }

		void Initialize ();
	}
}
