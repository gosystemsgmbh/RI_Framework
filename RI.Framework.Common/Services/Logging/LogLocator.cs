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

		#endregion
	}
}
