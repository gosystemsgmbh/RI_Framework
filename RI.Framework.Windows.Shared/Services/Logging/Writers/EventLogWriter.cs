using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

using RI.Framework.Composition.Model;
using RI.Framework.Mathematic;
using RI.Framework.Services.Logging.Filters;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Logging.Writers
{
	/// <summary>
	///     Implements a log writer which writes to <see cref="System.Diagnostics.EventLog" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="System.Diagnostics.EventLog.WriteEntry(string,EventLogEntryType)" /> is used to write the log messages.
	///     </para>
	///     <para>
	///         See <see cref="ILogWriter" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public sealed class EventLogWriter : LogSource, ILogWriter, IDisposable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="EventLogWriter" />.
		/// </summary>
		/// <param name="eventLog"> The event log to use. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="eventLog" /> is null. </exception>
		public EventLogWriter (EventLog eventLog)
		{
			if (eventLog == null)
			{
				throw new ArgumentNullException(nameof(eventLog));
			}

			this.SyncRoot = new object();

			this.Disposed = false;
			this.DisposedEventHandler = this.DisposedEvent;

			this.EventLog = eventLog;
			this.EventLog.Disposed += this.DisposedEventHandler;
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="EventLogWriter" />.
		/// </summary>
		~EventLogWriter ()
		{
			this.Dispose(false);
		}

		#endregion




		#region Instance Fields

		private ILogFilter _filter;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used event log.
		/// </summary>
		/// <value>
		///     The used event log.
		/// </value>
		public EventLog EventLog { get; }

		private bool Disposed { get; set; }

		private EventHandler DisposedEventHandler { get; }

		private object SyncRoot { get; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Closes this log writer and all used underlying streams.
		/// </summary>
		/// <remarks>
		///     <para>
		///         After the log writer is closed, all calls to <see cref="Log" /> do not have any effect but do not fail.
		///     </para>
		/// </remarks>
		public void Close ()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		[SuppressMessage("ReSharper", "UnusedParameter.Local")]
		[SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
		private void Dispose (bool disposing)
		{
			lock (this.SyncRoot)
			{
				this.EventLog.Disposed -= this.DisposedEventHandler;
				this.Disposed = true;
			}
		}

		private void DisposedEvent (object sender, EventArgs args)
		{
			this.Close();
		}

		#endregion




		#region Interface: IDisposable

		/// <inheritdoc />
		void IDisposable.Dispose ()
		{
			this.Close();
		}

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
		public void Cleanup (DateTime retentionDate)
		{
			lock (this.SyncRoot)
			{
				if (this.Disposed)
				{
					return;
				}

				int retentionDays = (int)DateTime.Now.Date.Subtract(retentionDate.Date).TotalDays + 1;
				retentionDays = retentionDays.Clamp(1, 365);

				this.Log(LogLevel.Information, "Cleaning up event log; retention days: {0}", retentionDays);

				try
				{
					this.EventLog.ModifyOverflowPolicy(OverflowAction.OverwriteOlder, retentionDays);
				}
				catch (Exception exception)
				{
					this.Log(LogLevel.Warning, "Could not cleanup event log: {0}", exception.ToDetailedString());
				}
			}
		}

		/// <inheritdoc />
		[SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
		public void Log (DateTime timestamp, int threadId, LogLevel severity, string source, string message)
		{
			ILogFilter filter = this.Filter;
			if (filter != null)
			{
				if (!filter.Filter(timestamp, threadId, severity, source))
				{
					return;
				}
			}

			lock (this.SyncRoot)
			{
				if (this.Disposed)
				{
					return;
				}

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
				finalMessageBuilder.Append(message);
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
