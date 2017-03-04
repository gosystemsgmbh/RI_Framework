using System;
using System.Collections.Generic;

using RI.Framework.Utilities;




namespace RI.Framework.RotenInformatik
{
	/// <summary>
	///     Sends crash reports to Roten Informatik.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="CrashReport" /> is used to send anonymous exception and error reports (a.k.a. crash reports) in order to create crash statistics and improve product quality by collecting technical details about crashes.
	///     </para>
	///     <para>
	///         Two types of crash reports are possible: Exception and Error.
	///         Exception reports are based on an exception where all possible exception details are sent, including the exception message, the stack trace, and all inner exceptions.
	///         Error reports are arbirary error strings which are sent as-is.
	///     </para>
	///     <para>
	///         In addition to the actual exception or error, additional string-based data can be attached to the report.
	///     </para>
	///     <note type="important">
	///         Make sure that no sensitive data is included in the crash report and that the users privacy and their secrets are kept!
	///     </note>
	/// </remarks>
	public sealed class CrashReport
	{
		#region Constants

		private const char IndentCharacter = ' ';

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="CrashReport" />.
		/// </summary>
		/// <param name="exception"> The exception. </param>
		public CrashReport (Exception exception)
			: this(exception?.ToDetailedString(CrashReport.IndentCharacter), null)
		{
			this.Exception = exception;
		}

		/// <summary>
		///     Creates a new instance of <see cref="CrashReport" />.
		/// </summary>
		/// <param name="exception"> The exception. </param>
		/// <param name="additionalData"> Additional data. </param>
		public CrashReport (Exception exception, IDictionary<string, string> additionalData)
			: this(exception?.ToDetailedString(CrashReport.IndentCharacter), additionalData)
		{
			this.Exception = exception;
		}

		/// <summary>
		///     Creates a new instance of <see cref="CrashReport" />.
		/// </summary>
		/// <param name="error"> The error. </param>
		public CrashReport (string error)
			: this(error, null)
		{
			this.Exception = null;
		}

		/// <summary>
		///     Creates a new instance of <see cref="CrashReport" />.
		/// </summary>
		/// <param name="error"> The error. </param>
		/// <param name="additionalData"> Additional data. </param>
		public CrashReport (string error, IDictionary<string, string> additionalData)
		{
			this.Exception = null;

			this.Error = error;
			this.AdditionalData = additionalData == null ? null : (additionalData.Count == 0 ? null : new Dictionary<string, string>(additionalData, StringComparerEx.InvariantCultureIgnoreCase));
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the additional data sent with the report.
		/// </summary>
		/// <value>
		///     The additional data sent with the report or null if no additional data is available.
		/// </value>
		public Dictionary<string, string> AdditionalData { get; private set; }

		/// <summary>
		///     Gets the error of this crash report.
		/// </summary>
		/// <value>
		///     The error of this crash report.
		/// </value>
		/// <remarks>
		///     <para>
		///         If the crash report is an exception report, this property returns the exception details.
		///     </para>
		/// </remarks>
		public string Error { get; private set; }

		/// <summary>
		///     Gets the exception on which this crash report is based.
		/// </summary>
		/// <value>
		///     The exception on which this crash report is based or null if it is not an exception report.
		/// </value>
		public Exception Exception { get; private set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Gets the details about the report being sent.
		/// </summary>
		/// <returns>
		///     Details about the report being sent.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The details is a human-readable text which shows all data which is being sent by the report.
		///         It is intended for transparency to show users, if desired, which data is sent.
		///     </para>
		/// </remarks>
		public string GetDetails ()
		{
			//TODO: Implement
			//TODO: Log
			return "Test123";
		}

		/// <summary>
		///     Sends the report.
		/// </summary>
		public void Send ()
		{
			//TODO: Implement
			//TODO: Log
		}

		#endregion
	}
}
