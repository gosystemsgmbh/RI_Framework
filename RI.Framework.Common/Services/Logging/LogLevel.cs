using System;




namespace RI.Framework.Services.Logging
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
		///     Error.
		/// </summary>
		Error = 3,
	}
}
