using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

using RI.Framework.IO.Paths;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Windows;




namespace RI.Framework.RotenInformatik
{
	/// <summary>
	///     Shows a common dialog to notify users about crashes.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A <see cref="CrashDialog" /> is used to inform the user that a crash occurred and that the application needs to be closed immediately.
	///         It also provides to functionality to send a corresponding crash report using <see cref="CrashReport" />, after getting the users consent (opt-out).
	///     </para>
	///     <para>
	///         A <see cref="CrashDialog" /> can either be based on an exception or an arbitrary string-based error.
	///     </para>
	/// </remarks>
	public sealed class CrashDialog
	{
		#region Constants

		private const char IndentCharacter = ' ';

		private const string ReportDetailsFileExtension = "txt";

		private static readonly Encoding ReportDetailsEncoding = Encoding.UTF8;

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="CrashDialog" />.
		/// </summary>
		/// <param name="exception"> The exception. </param>
		public CrashDialog (Exception exception)
			: this(exception?.ToDetailedString(CrashDialog.IndentCharacter), null)
		{
			this.Exception = exception;
		}

		/// <summary>
		///     Creates a new instance of <see cref="CrashDialog" />.
		/// </summary>
		/// <param name="exception"> The exception. </param>
		/// <param name="additionalData"> Additional data (only for sending crash reports). </param>
		public CrashDialog (Exception exception, IDictionary<string, string> additionalData)
			: this(exception?.ToDetailedString(CrashDialog.IndentCharacter), additionalData)
		{
			this.Exception = exception;
		}

		/// <summary>
		///     Creates a new instance of <see cref="CrashDialog" />.
		/// </summary>
		/// <param name="error"> The error. </param>
		public CrashDialog (string error)
			: this(error, null)
		{
			this.Exception = null;
		}

		/// <summary>
		///     Creates a new instance of <see cref="CrashDialog" />.
		/// </summary>
		/// <param name="error"> The error. </param>
		/// <param name="additionalData"> Additional data (only for sending crash reports). </param>
		public CrashDialog (string error, IDictionary<string, string> additionalData)
		{
			this.Exception = null;

			this.Error = error;
			this.AdditionalData = additionalData;

			this.Report = null;

			this.AllowReport = true;
			this.TextTitle = null;
			this.TextLabel = null;
			this.TextReportConsent = null;
			this.TextReportDetails = null;
			this.TextButton = null;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the additional data sent with the crash report.
		/// </summary>
		/// <value>
		///     The additional data sent with the crash report or null if no additional data is available.
		/// </value>
		public IDictionary<string, string> AdditionalData { get; private set; }

		/// <summary>
		///     Gets or sets whether crash reports are allowed.
		/// </summary>
		/// <value>
		///     true if crash reports are allowed, false otherwise.
		/// </value>
		public bool AllowReport { get; set; }

		/// <summary>
		///     Gets the error.
		/// </summary>
		/// <value>
		///     The error.
		/// </value>
		/// <remarks>
		///     <para>
		///         If the crash is an exception, this property returns the exception details.
		///     </para>
		/// </remarks>
		public string Error { get; private set; }

		/// <summary>
		///     Gets the exception on which this crash is based.
		/// </summary>
		/// <value>
		///     The exception on which this crash is based or null if it is not based on an exception.
		/// </value>
		public Exception Exception { get; private set; }

		/// <summary>
		///     Gets or sets the text for the button which closes the dialog.
		/// </summary>
		/// <value>
		///     The text for the button which closes the dialog.
		/// </value>
		/// <remarks>
		///     <para>
		///         If the text is null or an empty string, the default English text will be displayed.
		///     </para>
		/// </remarks>
		public string TextButton { get; set; }

		/// <summary>
		///     Gets or sets the text for the label which informs the user that a fatal error occurred.
		/// </summary>
		/// <value>
		///     The text for the label which informs the user that a fatal error occurred.
		/// </value>
		/// <remarks>
		///     <para>
		///         If the text is null or an empty string, the default English text will be displayed.
		///     </para>
		/// </remarks>
		public string TextLabel { get; set; }

		/// <summary>
		///     Gets or sets the text for the check box which asks for the users consent to send a crash report.
		/// </summary>
		/// <value>
		///     The text for the text for the check box which asks for the users consent to send a crash report.
		/// </value>
		/// <remarks>
		///     <para>
		///         If the text is null or an empty string, the default English text will be displayed.
		///     </para>
		/// </remarks>
		public string TextReportConsent { get; set; }

		/// <summary>
		///     Gets or sets the text for the hyperlink which opens details about the crash report.
		/// </summary>
		/// <value>
		///     The text for the text for the hyperlink which opens details about the crash report.
		/// </value>
		/// <remarks>
		///     <para>
		///         If the text is null or an empty string, the default English text will be displayed.
		///     </para>
		/// </remarks>
		public string TextReportDetails { get; set; }

		/// <summary>
		///     Gets or sets the title of the dialog.
		/// </summary>
		/// <value>
		///     The title of the dialog.
		/// </value>
		/// <remarks>
		///     <para>
		///         If the text is null or an empty string, the default English text will be displayed.
		///     </para>
		/// </remarks>
		public string TextTitle { get; set; }

		private CrashReport Report { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Shows the crash dialog.
		/// </summary>
		/// <remarks>
		///     <para>
		///         If a crash report is to be sent, this is done automatically before the method returns.
		///     </para>
		/// </remarks>
		public void ShowDialog ()
		{
			this.Report = this.Exception == null ? new CrashReport(this.Error, this.AdditionalData) : new CrashReport(this.Exception, this.AdditionalData);

			CrashDialogWindow dialog = new CrashDialogWindow();

			if (!this.TextTitle.IsNullOrEmptyOrWhitespace())
			{
				dialog.Title = this.TextTitle;
			}
			if (!this.TextLabel.IsNullOrEmptyOrWhitespace())
			{
				dialog.Label.Content = this.TextLabel;
			}
			if (!this.TextReportConsent.IsNullOrEmptyOrWhitespace())
			{
				dialog.ReportConsent.Content = this.TextReportConsent;
			}
			if (!this.TextReportDetails.IsNullOrEmptyOrWhitespace())
			{
				dialog.ReportDetails.Text = this.TextReportDetails;
			}
			if (!this.TextButton.IsNullOrEmptyOrWhitespace())
			{
				dialog.Button.Content = this.TextButton;
			}

			dialog.ReportPanel.Visibility = this.AllowReport ? Visibility.Visible : Visibility.Collapsed;
			dialog.Details.Text = this.Error;

			dialog.ShowReportDetails += (sender, args) =>
			{
				FilePath tempFile = FilePath.GetTempFile().ChangeExtension(CrashDialog.ReportDetailsFileExtension);
				tempFile.WriteText(this.Report.GetDetails(), CrashDialog.ReportDetailsEncoding);
				WindowsShell.OpenFile(tempFile);
			};

			dialog.ShowDialog();

			if (dialog.ReportConsent.IsChecked.GetValueOrDefault(false) && this.AllowReport)
			{
				this.Report.Send();
			}
		}

		#endregion
	}
}
