using System;




namespace RI.Framework.Services.Logging
{
	/// <summary>
	///     Provides extension methods for the <see cref="ILogSource" /> interface to add simple logging to types.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="ILogSourceExtensions" /> uses <see cref="LogLocator" />.
	///     </para>
	/// </remarks>
	public static class ILogSourceExtensions
	{
		#region Static Methods

		/// <inheritdoc cref="ILogService.Log(LogLevel,string,string,object[])" />
		public static void Log (this ILogSource source, LogLevel severity, string format, params object[] args)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			LogLocator.Log(severity, source.GetType().Name, format, args);
		}

		/// <inheritdoc cref="ILogService.Log(DateTime,int,LogLevel,string,string,object[])" />
		public static void Log (this ILogSource source, DateTime timestamp, int threadId, LogLevel severity, string format, params object[] args)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			LogLocator.Log(timestamp, threadId, severity, source.GetType().Name, format, args);
		}

		#endregion
	}
}
