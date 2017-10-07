using System;




namespace RI.Framework.Utilities.Logging
{
	/// <summary>
	///     Provides extension methods for the <see cref="ILogSource" /> interface to add simple logging to types which implement <see cref="ILogSource" />.
	/// </summary>
	public static class ILogSourceExtensions
	{
		#region Static Methods

		/// <inheritdoc cref="ILogger.Log(LogLevel,string,string,object[])" />
		public static void Log (this ILogSource source, LogLevel severity, string format, params object[] args)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			if (!source.LoggingEnabled)
			{
				return;
			}

			if (source.Logger == null)
			{
				return;
			}

			source.Logger.Log(severity, source.GetType().Name, format, args);
		}

		/// <inheritdoc cref="ILogger.Log(DateTime,int,LogLevel,string,string,object[])" />
		public static void Log (this ILogSource source, DateTime timestamp, int threadId, LogLevel severity, string format, params object[] args)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			if (!source.LoggingEnabled)
			{
				return;
			}

			if (source.Logger == null)
			{
				return;
			}

			source.Logger.Log(timestamp, threadId, severity, source.GetType().Name, format, args);
		}

		#endregion
	}
}
