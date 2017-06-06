using System;
using System.Text;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities.ObjectModel;

using UnityEngine;




namespace RI.Framework.Services.Logging
{
	/// <summary>
	///     Implements a default log writer which is suitable for most scenarios.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <c> Debug.Log </c>, <c> Debug.LogWarning </c>, or <c> Debug.LogError </c> is used to write the log messages, depending on the severity of the message.
	///     </para>
	///     <para>
	///         See <see cref="ILogWriter" /> for more details.
	///     </para>
	/// </remarks>
	[Export]
	public sealed class LogWriter : ILogWriter
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="LogWriter" />.
		/// </summary>
		public LogWriter ()
		{
			this.SyncRoot = new object();
		}

		#endregion




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
		void ILogWriter.Cleanup (DateTime retentionDate)
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
				StringBuilder finalMessageBuilder = new StringBuilder();
				finalMessageBuilder.Append("[");
				finalMessageBuilder.Append(source ?? "null");
				finalMessageBuilder.Append("] ");
				finalMessageBuilder.Append(message ?? string.Empty);
				string finalMessage = finalMessageBuilder.ToString();

				switch (severity)
				{
					case LogLevel.Debug:
					{
						Debug.Log(finalMessage);
						break;
					}

					case LogLevel.Information:
					{
						Debug.Log(finalMessage);
						break;
					}

					case LogLevel.Warning:
					{
						Debug.LogWarning(finalMessage);
						break;
					}

					case LogLevel.Error:
					{
						Debug.LogError(finalMessage);
						break;
					}

					case LogLevel.Fatal:
					{
						Debug.LogError(finalMessage);
						break;
					}
				}
			}
		}

		#endregion
	}
}
