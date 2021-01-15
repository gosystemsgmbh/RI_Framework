using System;
using System.Windows.Controls;

using RI.Framework.Mvvm.ViewModel;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities.Logging;




namespace RI.Framework.Mvvm.View
{
    /// <summary>
    ///     Implements a base class for <see cref="UserControl"/>-based views.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public class GenericViewBase : UserControl, IView, ILogSource
    {
        #region Instance Properties/Indexer

        /// <inheritdoc cref="IView.IsInitialized" />
        public new bool IsInitialized { get; private set; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Initializes this view if it was not already initialized before.
        /// </summary>
        protected void PerformInitializationIfNotAlreadyDone ()
        {
            if (!this.IsInitialized)
            {
                this.Initialize();
                this.IsInitialized = true;
            }
        }

        #endregion




        #region Virtuals

        /// <inheritdoc cref="IView.Initialize" />
        protected virtual void Initialize ()
        {
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        protected override void OnInitialized (EventArgs e)
        {
            base.OnInitialized(e);

            this.PerformInitializationIfNotAlreadyDone();
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




        #region Interface: IView

        /// <inheritdoc />
        bool IView.IsInitialized => this.IsInitialized;

        /// <inheritdoc />
        void IView.Initialize ()
        {
            this.PerformInitializationIfNotAlreadyDone();
        }

        /// <inheritdoc />
        IViewModel IView.ViewModel
        {
            get => this.DataContext as IViewModel;
            set => this.DataContext = value;
        }

        #endregion
    }
}
