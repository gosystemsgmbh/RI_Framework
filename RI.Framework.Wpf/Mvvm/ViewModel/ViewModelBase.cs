using System;
using System.ComponentModel;




namespace RI.Framework.Mvvm.ViewModel
{
	/// <summary>
	///     Implements a base class for view models.
	/// </summary>
	public abstract class ViewModelBase : IViewModel
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
