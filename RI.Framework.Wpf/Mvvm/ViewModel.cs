using System.ComponentModel;
using System.Runtime.CompilerServices;




namespace RI.Framework.Mvvm
{
	public abstract class ViewModel : IViewModel
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged (string propertyName)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}