using System;
using System.ComponentModel;

using RI.Framework.Services.Regions;




namespace RI.Framework.Mvvm.ViewModel
{
	/// <summary>
	///     Implements a base class for view models.
	/// </summary>
	public abstract class ViewModelBase : IViewModel, IRegionElement
	{
		#region Virtuals

		protected virtual void OnPropertyChanged (string propertyName)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion




		#region Interface: IRegionElement

		int? IRegionElement.SortIndex
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		void IRegionElement.Activated ()
		{
			throw new NotImplementedException();
		}

		bool IRegionElement.CanNavigateFrom ()
		{
			throw new NotImplementedException();
		}

		bool IRegionElement.CanNavigateTo ()
		{
			throw new NotImplementedException();
		}

		void IRegionElement.Deactivated ()
		{
			throw new NotImplementedException();
		}

		void IRegionElement.NavigatedFrom ()
		{
			throw new NotImplementedException();
		}

		void IRegionElement.NavigatedTo ()
		{
			throw new NotImplementedException();
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
