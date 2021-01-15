using System;




namespace RI.Framework.Utilities.Logging
{
	/// <summary>
	///     Defines the severity of a log message.
	/// </summary>
	[Serializable]
	public enum LogLevel
	{
		/// <summary>
		///     Debug.
		/// </summary>
		Debug = 0,

		/// <summary>
		///     Information.
		/// </summary>
		Information = 1,

		/// <summary>
		///     Warning.
		/// </summary>
		Warning = 2,

		/// <summary>
		///     Error (error or handled exception; recoverable).
		/// </summary>
		Error = 3,

		/// <summary>
		///     Fatal (crash or unhandled exception; unrecoverable).
		/// </summary>
		Fatal = 4,
	}
}
