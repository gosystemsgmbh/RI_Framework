using System.ComponentModel;




namespace RI.Framework.Mvvm
{
	/// <summary>
	/// Defines an interface for view models.
	/// </summary>
	public interface IViewModel : INotifyPropertyChanged
	{
		void Initialize ();

		bool IsInitialized { get; }
	}
}