using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

using RI.Framework.Composition.Model;
using RI.Framework.Mathematic;
using RI.Framework.Utilities;
using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Services.Logging.Writers
{
	/// <summary>
	///     Implements a log writer which writes to <see cref="System.Diagnostics.EventLog"/>.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="System.Diagnostics.EventLog.WriteEntry(string,EventLogEntryType)" /> is used to write the log messages.
	///     </para>
	///     <para>
	///         See <see cref="ILogWriter" /> for more details.
	///     </para>
	/// </remarks>
	[Export]
	public sealed class EventLogWriter : ILogWriter, ILogSource
	{
		/// <summary>
		/// Gets the used event log.
		/// </summary>
		/// <value>
		/// The used event log.
		/// </value>
		public EventLog EventLog { get; }

		/// <summary>
		///     Creates a new instance of <see cref="EventLogWriter" />.
		/// </summary>
		/// <param name="eventLog">The event log to use.</param>
		/// <exception cref="ArgumentNullException"><paramref name="eventLog"/> is null.</exception>
		public EventLogWriter(EventLog eventLog)
		{
			if (eventLog == null)
			{
				throw new ArgumentNullException(nameof(eventLog));
			}

			this.SyncRoot = new object();

			this.EventLog = eventLog;
		}



		#region Instance Fields

		private ILogFilter _filter;

		#endregion




		#region Instance Properties/Indexer

		private object SyncRoot { get; set; }

		#endregion




		#region Interface: ILogWriter

		/// <inheritdoc />
		public ILogFilter Filter
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._filter;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._filter = value;
				}
			}
		}

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		object ISynchronizable.SyncRoot => this.SyncRoot;

		/// <inheritdoc />
		public void Cleanup(DateTime retentionDate)
		{
			lock (this.SyncRoot)
			{
				int retentionDays = (int)DateTime.Now.Date.Subtract(retentionDate.Date).TotalDays + 1;
				retentionDays = retentionDays.Clamp(1, 365);

				try
				{
					this.EventLog.ModifyOverflowPolicy(OverflowAction.OverwriteOlder, retentionDays);
				}
				catch (InvalidOperationException exception)
				{
					this.Log(LogLevel.Warning, "Could not cleanup event log: {0}", exception.ToDetailedString());
				}
			}
		}

		/// <inheritdoc />
		[SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
		public void Log(DateTime timestamp, int threadId, LogLevel severity, string source, string message)
		{
			if (this.Filter != null)
			{
				if (!this.Filter.Filter(timestamp, threadId, severity, source))
				{
					return;
				}
			}

			lock (this.SyncRoot)
			{
				source = source ?? "null";
				message = message ?? string.Empty;

				StringBuilder finalMessageBuilder = new StringBuilder();
				finalMessageBuilder.Append("[");
				finalMessageBuilder.Append(timestamp.ToSortableString());
				finalMessageBuilder.Append("] [");
				finalMessageBuilder.Append(threadId.ToString("D4", CultureInfo.InvariantCulture));
				finalMessageBuilder.Append("] [");
				finalMessageBuilder.Append(severity.ToString()[0]);
				finalMessageBuilder.Append("] [");
				finalMessageBuilder.Append(source);
				finalMessageBuilder.Append("] ");
				finalMessageBuilder.AppendLine(message);
				string finalMessage = finalMessageBuilder.ToString();

				EventLogEntryType entryType = EventLogEntryType.Information;
				if (severity == LogLevel.Warning)
				{
					entryType = EventLogEntryType.Warning;
				}
				else if ((severity == LogLevel.Error) || (severity == LogLevel.Fatal))
				{
					entryType = EventLogEntryType.Error;
				}

				try
				{
					this.EventLog.WriteEntry(finalMessage, entryType);
				}
				catch
				{
				}
			}
		}

		#endregion
	}
}
