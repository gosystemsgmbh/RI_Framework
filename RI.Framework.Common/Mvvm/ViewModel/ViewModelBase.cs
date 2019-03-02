using RI.Framework.Services.Logging;
using RI.Framework.Services.Regions;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Mvvm.ViewModel
{
    /// <summary>
    ///     Implements a base class for view models.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public abstract class ViewModelBase : NotificationObject, IViewModel, IRegionElement, ILogSource
    {
        private bool _isInitialized;
        private int? _sortIndex;

        /// <summary>
        /// Creates a new instance of <see cref="ViewModelBase"/>.
        /// </summary>
        protected ViewModelBase ()
        {
            this.IsInitialized = false;
            this.SortIndex = null;
        }




        #region Virtuals

        /// <inheritdoc cref="IViewModel.Initialize" />
        protected virtual void Initialize ()
        {
        }

        #endregion




        #region Interface: ILogSource

        /// <inheritdoc />
        public LogLevel LogFilter { get; set; } = LogLevel.Debug;

        /// <inheritdoc />
        public ILogger Logger { get; set; } = LogLocator.Logger;

        /// <inheritdoc />
        public bool LoggingEnabled { get; set; } = true;

        #endregion




        #region Interface: IRegionElement

        /// <inheritdoc />
        public int? SortIndex
        {
            get => this._sortIndex;
            private set
            {
                this.OnPropertyChanging(nameof(this.SortIndex));
                this._sortIndex = value;
                this.OnPropertyChanged(nameof(this.SortIndex));
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
        public bool IsInitialized
        {
            get => this._isInitialized;
            private set
            {
                this.OnPropertyChanging(nameof(this.IsInitialized));
                this._isInitialized = value;
                this.OnPropertyChanged(nameof(this.IsInitialized));
            }
        }

        /// <inheritdoc />
        void IViewModel.Initialize ()
        {
            if (!this.IsInitialized)
            {
                this.Log(LogLevel.Debug, "Initializing view model");
                this.Initialize();
                this.IsInitialized = true;
            }
        }

        #endregion
    }
}
