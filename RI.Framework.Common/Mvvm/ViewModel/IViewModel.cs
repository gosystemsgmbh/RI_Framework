﻿using System.ComponentModel;

using RI.Framework.Composition.Model;




namespace RI.Framework.Mvvm.ViewModel
{
    /// <summary>
    ///     Defines an interface for view models.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    [Export]
    public interface IViewModel : INotifyPropertyChanged
    {
        /// <summary>
        ///     Gets whether the view model is initialized or not.
        /// </summary>
        /// <value>
        ///     true if the view model is initialized, false otherwise.
        /// </value>
        bool IsInitialized { get; }

        /// <summary>
        ///     Initializes the view model.
        /// </summary>
        void Initialize ();
    }
}
