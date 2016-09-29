using System;
using System.ComponentModel;




namespace RI.Framework.Mvvm
{
	public abstract class ViewModel : IViewModel
	{
		#region Virtuals

		protected virtual void OnPropertyChanged (string propertyName)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion




		#region Interface: IViewModel

		public bool IsInitialized { get; }

		public event PropertyChangedEventHandler PropertyChanged;

		public void Initialize ()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
