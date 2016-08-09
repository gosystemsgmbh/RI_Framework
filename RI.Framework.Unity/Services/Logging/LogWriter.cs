using System;
using System.Text;

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
			StringBuilder finalMessageBuilder = new StringBuilder();
			finalMessageBuilder.Append("[");
			finalMessageBuilder.Append(source);
			finalMessageBuilder.Append("] ");
			finalMessageBuilder.Append(message);
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
			}
		}

		#endregion
	}
}
