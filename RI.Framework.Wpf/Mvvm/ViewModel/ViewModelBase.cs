using System.ComponentModel;

using RI.Framework.Services.Regions;




namespace RI.Framework.Mvvm.ViewModel
{
	/// <summary>
	///     Implements a base class for view models.
	/// </summary>
	public abstract class ViewModelBase : IViewModel, IRegionElement
	{
		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets whether the view is initialized or not.
		/// </summary>
		/// <value>
		///     true if the view is initialized, false otherwise.
		/// </value>
		protected bool IsInitialized { get; set; }

		#endregion




		#region Instance Events

		private event PropertyChangedEventHandler PropertyChanged;

		#endregion




		#region Virtuals

		/// <inheritdoc cref="IRegionElement.SortIndex" />
		protected virtual int? SortIndex
		{
			get
			{
				return null;
			}
		}

		/// <inheritdoc cref="IRegionElement.Activated" />
		protected virtual void Activated ()
		{
		}

		/// <inheritdoc cref="IRegionElement.CanNavigateFrom" />
		protected virtual bool CanNavigateFrom ()
		{
			return true;
		}

		/// <inheritdoc cref="IRegionElement.CanNavigateTo" />
		protected virtual bool CanNavigateTo ()
		{
			return true;
		}

		/// <inheritdoc cref="IRegionElement.Deactivated" />
		protected virtual void Deactivated ()
		{
		}

		/// <inheritdoc cref="IViewModel.Initialize" />
		protected virtual void Initialize ()
		{
		}

		/// <inheritdoc cref="IRegionElement.NavigatedFrom" />
		protected virtual void NavigatedFrom ()
		{
		}

		/// <inheritdoc cref="IRegionElement.NavigatedTo" />
		protected virtual void NavigatedTo ()
		{
		}

		/// <summary>
		///     Handles the change of all property values by raising the <see cref="PropertyChanged" /> event.
		/// </summary>
		protected virtual void OnAllPropertiesChanged ()
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
		}

		/// <summary>
		///     Handles the change of a property value by raising the <see cref="PropertyChanged" /> event.
		/// </summary>
		/// <param name="propertyName"> The name of the property which has changed. </param>
		protected virtual void OnPropertyChanged (string propertyName)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion




		#region Interface: IRegionElement

		/// <inheritdoc />
		int? IRegionElement.SortIndex
		{
			get
			{
				return this.SortIndex;
			}
		}

		/// <inheritdoc />
		void IRegionElement.Activated ()
		{
			this.Activated();
		}

		/// <inheritdoc />
		bool IRegionElement.CanNavigateFrom ()
		{
			return this.CanNavigateFrom();
		}

		/// <inheritdoc />
		bool IRegionElement.CanNavigateTo ()
		{
			return this.CanNavigateTo();
		}

		/// <inheritdoc />
		void IRegionElement.Deactivated ()
		{
			this.Deactivated();
		}

		/// <inheritdoc />
		void IRegionElement.NavigatedFrom ()
		{
			this.NavigatedFrom();
		}

		/// <inheritdoc />
		void IRegionElement.NavigatedTo ()
		{
			this.NavigatedTo();
		}

		#endregion




		#region Interface: IViewModel

		/// <inheritdoc />
		bool IViewModel.IsInitialized
		{
			get
			{
				return this.IsInitialized;
			}
		}

		/// <inheritdoc />
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add
			{
				this.PropertyChanged += value;
			}
			remove
			{
				this.PropertyChanged -= value;
			}
		}

		/// <inheritdoc />
		void IViewModel.Initialize ()
		{
			this.Initialize();
		}

		#endregion
	}
}
