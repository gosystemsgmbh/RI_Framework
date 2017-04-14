namespace RI.Framework.Services.Logging
{
	/// <summary>
	///     Provides a centralized and global logging provider.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="LogLocator" /> is merely a convenience utility as it uses <see cref="ServiceLocator" /> to retrieve and use a <see cref="ILogService" />.
	///     </para>
	/// </remarks>
	public static class LogLocator
	{
		#region Static Methods

		/// <summary>
		///     Logs a message.
		/// </summary>
		/// <param name="severity"> The severity of the message. </param>
		/// <param name="source"> The source of the message. </param>
		/// <param name="format"> The message. </param>
		/// <param name="args"> The arguments which will be expanded into the message (comparable to <see cref="string.Format(string, object[])" />). </param>
		public static void Log (LogLevel severity, string source, string format, params object[] args)
		{
			ILogService logService = ServiceLocator.GetInstance<ILogService>();
			logService?.Log(severity, source, format, args);
		}

		/// <summary>
		///     Logs a debug message.
		/// </summary>
		/// <param name="source"> The source of the message. </param>
		/// <param name="format"> The message. </param>
		/// <param name="args"> The arguments which will be expanded into the message (comparable to <see cref="string.Format(string, object[])" />). </param>
		public static void LogDebug (string source, string format, params object[] args)
		{
			LogLocator.Log(LogLevel.Debug, source, format, args);
		}

		/// <summary>
		///     Logs an error message.
		/// </summary>
		/// <param name="source"> The source of the message. </param>
		/// <param name="format"> The message. </param>
		/// <param name="args"> The arguments which will be expanded into the message (comparable to <see cref="string.Format(string, object[])" />). </param>
		public static void LogError (string source, string format, params object[] args)
		{
			LogLocator.Log(LogLevel.Error, source, format, args);
		}

		/// <summary>
		///     Logs a fatal message.
		/// </summary>
		/// <param name="source"> The source of the message. </param>
		/// <param name="format"> The message. </param>
		/// <param name="args"> The arguments which will be expanded into the message (comparable to <see cref="string.Format(string, object[])" />). </param>
		public static void LogFatal (string source, string format, params object[] args)
		{
			LogLocator.Log(LogLevel.Fatal, source, format, args);
		}

		/// <summary>
		///     Logs an information message.
		/// </summary>
		/// <param name="source"> The source of the message. </param>
		/// <param name="format"> The message. </param>
		/// <param name="args"> The arguments which will be expanded into the message (comparable to <see cref="string.Format(string, object[])" />). </param>
		public static void LogInformation (string source, string format, params object[] args)
		{
			LogLocator.Log(LogLevel.Information, source, format, args);
		}

		/// <summary>
		///     Logs a warning message.
		/// </summary>
		/// <param name="source"> The source of the message. </param>
		/// <param name="format"> The message. </param>
		/// <param name="args"> The arguments which will be expanded into the message (comparable to <see cref="string.Format(string, object[])" />). </param>
		public static void LogWarning (string source, string format, params object[] args)
		{
			LogLocator.Log(LogLevel.Warning, source, format, args);
		}

		#endregion
	}
}
