using RI.Framework.Services.Regions;




namespace RI.Framework.Mvvm.ViewModel
{
	/// <summary>
	///     Implements a base class for view models.
	/// </summary>
	public abstract class ViewModelBase : NotificationObject, IViewModel, IRegionElement
	{
		#region Interface: IRegionElement

		/// <inheritdoc />
		public virtual int? SortIndex
		{
			get
			{
				return null;
			}
		}

		/// <inheritdoc />
		public virtual void Activated ()
		{
		}

		/// <inheritdoc />
		public virtual bool CanNavigateFrom ()
		{
			return true;
		}

		/// <inheritdoc />
		public virtual bool CanNavigateTo ()
		{
			return true;
		}

		/// <inheritdoc />
		public virtual void Deactivated ()
		{
		}

		/// <inheritdoc />
		public virtual void NavigatedFrom ()
		{
		}

		/// <inheritdoc />
		public virtual void NavigatedTo ()
		{
		}

		#endregion




		#region Interface: IViewModel

		/// <inheritdoc />
		public bool IsInitialized { get; protected set; }

		/// <inheritdoc />
		public virtual void Initialize ()
		{
		}

		#endregion
	}
}
