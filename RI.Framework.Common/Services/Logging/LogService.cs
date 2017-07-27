using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

using RI.Framework.Collections;
using RI.Framework.Composition;
using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging.Filters;
using RI.Framework.Services.Logging.Writers;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Services.Logging
{
	/// <summary>
	///     Implements a default logging service which is suitable for most scenarios.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This logging service uses <see cref="ILogWriter" />s from two sources.
	///         One are the explicitly specified log writers added through <see cref="AddWriter" />.
	///         The second is a <see cref="CompositionContainer" /> if this <see cref="LogService" /> is added as an export (the log writers are then imported through composition).
	///         <see cref="Writers" /> gives the sequence containing all log writers from all sources.
	///     </para>
	///     <para>
	///         See <see cref="ILogService" /> for more details.
	///     </para>
	/// </remarks>
	[Export]
	public sealed class LogService : ILogService
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="LogService" />.
		/// </summary>
		public LogService ()
		{
			this.LogSyncRoot = new object();
			this.WritersManual = new List<ILogWriter>();
		}

		#endregion




		#region Instance Fields

		private ILogFilter _filter;

		#endregion




		#region Instance Properties/Indexer

		private object LogSyncRoot { get; }

		[Import(typeof(ILogWriter), Recomposable = true)]
		private Import WritersImported { get; set; }

		private List<ILogWriter> WritersManual { get; }

		#endregion




		#region Interface: ILogService

		/// <inheritdoc />
		public ILogFilter Filter
		{
			get
			{
				lock (this.LogSyncRoot)
				{
					return this._filter;
				}
			}
			set
			{
				lock (this.LogSyncRoot)
				{
					this._filter = value;
				}
			}
		}

		/// <inheritdoc />
		public IEnumerable<ILogWriter> Writers
		{
			get
			{
				foreach (ILogWriter logWriter in this.WritersManual)
				{
					yield return logWriter;
				}

				foreach (ILogWriter logWriter in this.WritersImported.Values<ILogWriter>())
				{
					yield return logWriter;
				}
			}
		}

		/// <inheritdoc />
		public void AddWriter (ILogWriter logWriter)
		{
			if (logWriter == null)
			{
				throw new ArgumentNullException(nameof(logWriter));
			}

			if (this.WritersManual.Contains(logWriter))
			{
				return;
			}

			this.WritersManual.Add(logWriter);
		}

		/// <inheritdoc />
		public void Cleanup (DateTime retentionDate)
		{
			lock (this.LogSyncRoot)
			{
				foreach (ILogWriter logWriter in this.Writers)
				{
					logWriter.Cleanup(retentionDate);
				}
			}
		}

		/// <inheritdoc />
		public void Cleanup (TimeSpan retentionTime)
		{
			if (retentionTime.Ticks <= 0)
			{
				return;
			}

			this.Cleanup(DateTime.Now.Subtract(retentionTime));
		}

		/// <inheritdoc />
		public void Log (LogLevel severity, string source, string format, params object[] args)
		{
			DateTime timestamp = DateTime.Now;
			int threadId = Thread.CurrentThread.ManagedThreadId;

			ILogFilter filter = this.Filter;
			if (filter != null)
			{
				if (!filter.Filter(timestamp, threadId, severity, source))
				{
					return;
				}
			}

			lock (this.LogSyncRoot)
			{
				string message = string.Format(CultureInfo.InvariantCulture, format ?? string.Empty, args ?? new object[0]);

				foreach (ILogWriter logWriter in this.Writers)
				{
					logWriter.Log(timestamp, threadId, severity, source, message);
				}
			}
		}

		/// <inheritdoc />
		public void Log (DateTime timestamp, int threadId, LogLevel severity, string source, string format, params object[] args)
		{
			ILogFilter filter = this.Filter;
			if (filter != null)
			{
				if (!filter.Filter(timestamp, threadId, severity, source))
				{
					return;
				}
			}

			lock (this.LogSyncRoot)
			{
				string message = string.Format(CultureInfo.InvariantCulture, format ?? string.Empty, args ?? new object[0]);

				foreach (ILogWriter logWriter in this.Writers)
				{
					logWriter.Log(timestamp, threadId, severity, source, message);
				}
			}
		}

		/// <inheritdoc />
		public void RemoveWriter (ILogWriter logWriter)
		{
			if (logWriter == null)
			{
				throw new ArgumentNullException(nameof(logWriter));
			}

			if (!this.WritersManual.Contains(logWriter))
			{
				return;
			}

			this.WritersManual.RemoveAll(logWriter);
		}

		#endregion
	}
}
