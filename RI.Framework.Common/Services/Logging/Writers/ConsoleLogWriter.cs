using System;
using System.Globalization;
using System.Text;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities;
using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Services.Logging.Writers
{
	/// <summary>
	/// Implements a log writer which writes to the process console.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="ILogWriter" /> for more details.
	///     </para>
	/// </remarks>
	[Export]
	public sealed class ConsoleLogWriter : ILogWriter
	{
		/// <summary>
		/// Creates a new instance of <see cref="ConsoleLogWriter"/>.
		/// </summary>
		public ConsoleLogWriter ()
		{
			this.SyncRoot = new object();
		}

		private object SyncRoot { get; set; }

		private ILogFilter _filter;

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
		public void Cleanup (DateTime retentionDate)
		{
		}

		/// <inheritdoc />
		public void Log (DateTime timestamp, int threadId, LogLevel severity, string source, string message)
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
				Console.WriteLine(finalMessage);
			}
		}

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		object ISynchronizable.SyncRoot => this.SyncRoot;
	}
}
