using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging.Filters;
using RI.Framework.Utilities;
using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Services.Logging.Writers
{
	/// <summary>
	///     Implements a log writer which uses the .NET Framework tracing mechanism.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="TraceListenerAdapterLogWriter" /> is a two-way adapter which writes log entries comming from <see cref="ILogService" /> to <see cref="Trace" /> but also adds a <see cref="System.Diagnostics.TraceListener" /> to <see cref="Trace" /> to write all traces written to <see cref="Trace" /> to <see cref="ILogService" /> (using <see cref="ILogSourceExtensions" />).
	///     </para>
	///     <para>
	///         <see cref="Trace.WriteLine(string)" /> is used to write the log messages.
	///     </para>
	///     <para>
	///         See <see cref="ILogWriter" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public sealed class TraceListenerAdapterLogWriter : ILogWriter, IDisposable, ILogSource
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="TraceListenerAdapterLogWriter" />.
		/// </summary>
		public TraceListenerAdapterLogWriter ()
		{
			this.SyncRoot = new object();

			this.Buffer = null;
			this.IsWriting = false;
			this.IsTracing = false;

			this.TraceListener = new LogWriterTraceListener(this);
			Trace.Listeners.Add(this.TraceListener);
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="TraceListenerAdapterLogWriter" />.
		/// </summary>
		~TraceListenerAdapterLogWriter ()
		{
			this.Dispose(false);
		}

		#endregion




		#region Instance Fields

		private ILogFilter _filter;

		#endregion




		#region Instance Properties/Indexer

		private string Buffer { get; set; }
		private bool IsTracing { get; set; }
		private bool IsWriting { get; set; }

		private object SyncRoot { get; set; }

		private LogWriterTraceListener TraceListener { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Closes this log writer and the used trace listener.
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
				if (this.TraceListener != null)
				{
					Trace.Listeners.Remove(this.TraceListener);

					this.TraceListener.Close();
					this.TraceListener = null;
				}
			}
		}

		private void WriteTrace (string message)
		{
			lock (this.SyncRoot)
			{
				if (this.IsWriting || (this.TraceListener == null))
				{
					return;
				}

				try
				{
					this.IsTracing = true;

					StringBuilder logMessageBuilder = new StringBuilder();

					if (this.Buffer != null)
					{
						logMessageBuilder.Append(this.Buffer);
						this.Buffer = null;
					}

					logMessageBuilder.Append(message);

					string logMessageString = logMessageBuilder.ToString();
					int lastIndex = logMessageString.LastIndexOf("\n", StringComparison.Ordinal);

					string logMessage = null;
					string remaining = null;

					if (lastIndex == -1)
					{
						remaining = logMessageString;
					}
					else if (lastIndex == (logMessageString.Length - 1))
					{
						logMessage = logMessageString.TrimEnd('\r', '\n');
					}
					else
					{
						logMessage = logMessageString.Substring(0, lastIndex).TrimEnd('\r', '\n');
						remaining = logMessageString.Substring(lastIndex + 1);
					}

					this.Buffer = remaining;

					if (logMessage != null)
					{
						this.Log(LogLevel.Debug, "TRACE: {0}", logMessage);
					}
				}
				finally
				{
					this.IsTracing = false;
				}
			}
		}

		private void WriteTraceLine (string message)
		{
			this.WriteTrace(message + Environment.NewLine);
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
		void ILogWriter.Cleanup(DateTime retentionDate)
		{
		}

		/// <inheritdoc />
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
				if (this.IsTracing || (this.TraceListener == null))
				{
					return;
				}

				try
				{
					this.IsWriting = true;

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

					Trace.WriteLine(finalMessage);
				}
				finally
				{
					this.IsWriting = false;
				}
			}
		}

		#endregion




		#region Type: LogWriterTraceListener

		private sealed class LogWriterTraceListener : TraceListener
		{
			#region Instance Constructor/Destructor

			public LogWriterTraceListener (TraceListenerAdapterLogWriter logWriter)
			{
				if (logWriter == null)
				{
					throw new ArgumentNullException(nameof(logWriter));
				}

				this.LogWriter = logWriter;

				this.Name = this.GetType().Name;
			}

			#endregion




			#region Instance Properties/Indexer

			public TraceListenerAdapterLogWriter LogWriter { get; }

			#endregion




			#region Overrides

			public override bool IsThreadSafe => false;

			public override void Write (string message)
			{
				this.LogWriter.WriteTrace(message);
			}

			public override void WriteLine (string message)
			{
				this.LogWriter.WriteTraceLine(message);
			}

			#endregion
		}

		#endregion
	}
}
