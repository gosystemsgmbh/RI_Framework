using System;
using System.Collections.Generic;

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
		/// <summary>
		/// Gets whether a logging service is available and can be used by <see cref="LogLocator"/>.
		/// </summary>
		/// <value>
		/// true if a logging service is available and can be used by <see cref="LogLocator"/>, false otherwise.
		/// </value>
		public static bool IsAvailable => LogLocator.Service != null;

		/// <summary>
		/// Gets the available logging service.
		/// </summary>
		/// <value>
		/// The available logging service or null if no logging service is available.
		/// </value>
		public static ILogService Service => ServiceLocator.GetInstance<ILogService>();

		/// <inheritdoc cref="ILogService.Writers"/>
		public static IEnumerable<ILogWriter> Writers => LogLocator.Service?.Writers;


		#region Static Methods

		/// <inheritdoc cref="ILogService.Cleanup(DateTime)"/>
		public static void Cleanup (DateTime retentionDate) => LogLocator.Service?.Cleanup(retentionDate);

		/// <inheritdoc cref="ILogService.Cleanup(TimeSpan)"/>
		public static void Cleanup (TimeSpan retentionTime) => LogLocator.Service?.Cleanup(retentionTime);

		/// <inheritdoc cref="ILogService.Log(LogLevel,string,string,object[])"/>
		public static void Log (LogLevel severity, string source, string format, params object[] args) => LogLocator.Service?.Log(severity, source, format, args);

		/// <inheritdoc cref="ILogService.Log(DateTime,int,LogLevel,string,string,object[])"/>
		public static void Log (DateTime timestamp, int threadId, LogLevel severity, string source, string format, params object[] args) => LogLocator.Service?.Log(timestamp, threadId, severity, source, format, args);

		#endregion
	}
}
