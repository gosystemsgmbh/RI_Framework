using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

using RI.Framework.Utilities;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Logging
{
	/// <summary>
	///     Implements a default log writer which is suitable for most scenarios.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="Debugger.Log" /> is used to write the log messages.
	///     </para>
	///     <para>
	///         See <see cref="ILogWriter" /> for more details.
	///     </para>
	/// </remarks>
	public sealed class LogWriter : ILogWriter
	{
		#region Interface: ILogWriter

		/// <inheritdoc />
		void ILogWriter.Cleanup (DateTime retentionDate)
		{
		}

		/// <inheritdoc />
		public void Log (DateTime timestamp, int threadId, LogLevel severity, string source, string message)
		{
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
				Debugger.Log((int)severity, source, finalMessage);
			}
		}

		/// <summary>
		/// Creates a new instance of <see cref="LogWriter"/>.
		/// </summary>
		public LogWriter()
		{
			this.SyncRoot = new object();
		}

		private object SyncRoot { get; set; }

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		object ISynchronizable.SyncRoot => this.SyncRoot;

		#endregion
	}
}
