﻿using System;
using System.Windows.Controls;




namespace RI.Framework.Mvvm.View
{
	/// <summary>
	/// Implements a base class for generic views which are hosted inside another WPF element.
	/// </summary>
	public class GenericViewBase : UserControl, IView
	{
		#region Instance Properties/Indexer

		/// <inheritdoc cref="IView.IsInitialized"/>
		public new bool IsInitialized { get; private set; }

		#endregion




		#region Instance Methods

		/// <summary>
		/// Initializes this view if it was not already initialized before.
		/// </summary>
		protected void PerformInitializationIfNotAlreadyDone()
		{
			if (!this.IsInitialized)
			{
				this.Initialize();
				this.IsInitialized = true;
			}
		}

		#endregion




		#region Virtuals

		/// <inheritdoc cref="IView.Initialize"/>
		protected virtual void Initialize()
		{
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			this.PerformInitializationIfNotAlreadyDone();
		}

		#endregion




		#region Interface: IView

		/// <inheritdoc />
		bool IView.IsInitialized => this.IsInitialized;

		/// <inheritdoc />
		void IView.Initialize()
		{
			this.PerformInitializationIfNotAlreadyDone();
		}

		#endregion
	}
}