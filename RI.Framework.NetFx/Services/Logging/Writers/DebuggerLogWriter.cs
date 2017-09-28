using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging.Filters;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Logging.Writers
{
	/// <summary>
	///     Implements a log writer which writes to <see cref="Debugger" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="Debugger.Log" /> is used to write the log messages.
	///     </para>
	///     <para>
	///         See <see cref="ILogWriter" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public sealed class DebuggerLogWriter : ILogWriter
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DebuggerLogWriter" />.
		/// </summary>
		public DebuggerLogWriter ()
		{
			this.SyncRoot = new object();
		}

		#endregion




		#region Instance Fields

		private ILogFilter _filter;

		#endregion




		#region Instance Properties/Indexer

		private object SyncRoot { get; }

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
		void ILogWriter.Cleanup (DateTime retentionDate)
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

				Debugger.Log((int)severity, source, finalMessage);
			}
		}

		#endregion
	}
}
