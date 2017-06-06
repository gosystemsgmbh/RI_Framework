using System;




namespace RI.Framework.Services.Logging
{
	/// <summary>
	///     Provides extension methods for the <see cref="ILogSource" /> interface to add simple logging to types.
	/// </summary>
	public static class ILogSourceExtensions
	{
		#region Static Methods

		/// <summary>
		///     Logs a message.
		/// </summary>
		/// <param name="source"> The source object which logs the message. </param>
		/// <param name="severity"> The severity of the message. </param>
		/// <param name="format"> The message. </param>
		/// <param name="args"> The arguments which will be expanded into the message (comparable to <see cref="string.Format(string, object[])" />). </param>
		/// <remarks>
		///     <para>
		///         <see cref="ILogService" /> is used, obtained through <see cref="LogLocator" />.
		///         If no <see cref="ILogService" /> is available, no logging is performed.
		///     </para>
		/// </remarks>
		public static void Log (this ILogSource source, LogLevel severity, string format, params object[] args)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			LogLocator.Log(severity, source.GetType().Name, format, args);
		}

		#endregion
	}
}
