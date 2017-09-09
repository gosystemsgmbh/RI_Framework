using System;

namespace RI.Framework.Services.Logging.Filters
{
	/// <summary>
	/// Defines the filter mode for an <see cref="AggregateLogFilter"/>.
	/// </summary>
	[Serializable]
	public enum AggregateLogFilterMode
	{
		/// <summary>
		/// A log entry is written if it passes all log filters of an <see cref="AggregateLogFilter"/>.
		/// </summary>
		And = 0,

		/// <summary>
		/// A log entry is written if it passes at least one log filters of an <see cref="AggregateLogFilter"/>.
		/// </summary>
		Or = 1,
	}
}
