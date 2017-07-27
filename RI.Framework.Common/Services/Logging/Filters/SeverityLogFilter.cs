using System;

using RI.Framework.Utilities.Logging;

namespace RI.Framework.Services.Logging.Filters
{
	/// <summary>
	///     Implements a log filter based on the severity (<see cref="LogLevel" />) of messages.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="ILogFilter" /> for more details.
	///     </para>
	/// </remarks>
	public sealed class SeverityLogFilter : ILogFilter
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="SeverityLogFilter" />.
		/// </summary>
		/// <remarks>
		///     <para>
		///         <see cref="LogLevel.Debug" /> is used for <see cref="MinSeverity" />, effectively allowing all messages by default.
		///     </para>
		/// </remarks>
		public SeverityLogFilter ()
			: this(LogLevel.Debug)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="SeverityLogFilter" />.
		/// </summary>
		/// <param name="minSeverity"> The minimum severity to use, assigned to <see cref="MinSeverity" />. </param>
		public SeverityLogFilter (LogLevel minSeverity)
		{
			this.MinSeverity = minSeverity;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets the minimum severity a message must have in order to pass the log filter.
		/// </summary>
		/// <value>
		///     The minimum severity a message must have in order to pass the log filter.
		/// </value>
		public LogLevel MinSeverity { get; set; }

		#endregion




		#region Interface: ILogFilter

		/// <inheritdoc />
		public bool Filter (DateTime timestamp, int threadId, LogLevel severity, string source)
		{
			return severity >= this.MinSeverity;
		}

		#endregion
	}
}
