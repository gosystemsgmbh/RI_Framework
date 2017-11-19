using System;
using System.Windows;




namespace RI.Framework.Utilities.Crash
{
	internal partial class CrashDialogWindow : Window
	{
		#region Instance Constructor/Destructor

		public CrashDialogWindow ()
		{
			this.InitializeComponent();
		}

		#endregion




		#region Instance Events

		public event EventHandler ShowReportDetails;

		#endregion




		#region Instance Methods

		private void Button_OnClick (object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void ReportDetails_OnClick (object sender, RoutedEventArgs e)
		{
			this.ShowReportDetails?.Invoke(this, EventArgs.Empty);
		}

		#endregion
	}
}
