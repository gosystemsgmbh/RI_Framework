using RI.Framework.Services.Logging;
using RI.Framework.Services.Regions;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Mvvm.ViewModel
{
	/// <summary>
	///     Implements a base class for view models.
	/// </summary>
	public abstract class ViewModelBase : NotificationObject, IViewModel, IRegionElement, ILogSource
	{
		#region Virtuals

		/// <inheritdoc cref="IViewModel.Initialize" />
		protected virtual void Initialize ()
		{
		}


		/// <inheritdoc />
		public bool LoggingEnabled { get; set; } = true;

		/// <inheritdoc />
		public ILogger Logger { get; set; } = LogLocator.Logger;

		#endregion




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
		public bool IsInitialized { get; private set; }

		/// <inheritdoc />
		void IViewModel.Initialize ()
		{
			this.Log(LogLevel.Debug, "Initializing view model");

			this.Initialize();

			this.IsInitialized = true;
		}

		#endregion
	}
}
