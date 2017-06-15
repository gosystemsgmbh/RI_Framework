using System;
using System.Globalization;
using System.IO;

using RI.Framework.Utilities;




namespace RI.Framework.Services.Logging.Writers
{
	internal sealed class LogFileFormatter
	{
		#region Constants

		public const string FirstLinePrefix = "#";

		public const string SubsequentLinePrefix = ">";

		#endregion




		#region Instance Constructor/Destructor

		public LogFileFormatter ()
		{
			this.Reset();
		}

		#endregion




		#region Instance Properties/Indexer

		private int[] CurrentLengths { get; set; }

		#endregion




		#region Instance Methods

		public void Reset ()
		{
			this.CurrentLengths = new int[] {0, 0, 0, 0, 0, 0,};
		}

		public void Write (TextWriter writer, DateTime timestamp, int threadId, LogLevel severity, string source, string message)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}

			message = message ?? string.Empty;
			int newLineIndex = message.IndexOf('\n');
			string firstLine = newLineIndex == -1 ? message.Trim() : message.Substring(0, newLineIndex).Trim();
			string[] subsequentLines = newLineIndex == -1 ? null : message.Substring(newLineIndex + 1).Trim().SplitLines(StringSplitOptions.None);

			string[] headers = new string[6];

			headers[0] = LogFileFormatter.FirstLinePrefix.PadRight(this.CurrentLengths[0], ' ');
			headers[1] = (" [" + timestamp.ToSortableString('-') + "]").PadRight(this.CurrentLengths[1], ' ');
			headers[2] = (" [" + threadId.ToString("D", CultureInfo.InvariantCulture) + "]").PadRight(this.CurrentLengths[2], ' ');
			headers[3] = (" [" + severity + "]").PadRight(this.CurrentLengths[3], ' ');
			headers[4] = (" [" + (source ?? "null") + "]").PadRight(this.CurrentLengths[4], ' ');
			headers[5] = (" ").PadRight(this.CurrentLengths[5], ' ');

			int headerLength = 0;

			for (int i1 = 0; i1 < headers.Length; i1++)
			{
				writer.Write(headers[i1]);
				this.CurrentLengths[i1] = Math.Max(headers[i1].Length, this.CurrentLengths[i1]);
				headerLength += this.CurrentLengths[i1];
			}

			writer.WriteLine(firstLine);

			if (subsequentLines != null)
			{
				foreach (string subsequentLine in subsequentLines)
				{
					if (!subsequentLine.IsEmptyOrWhitespace())
					{
						writer.Write(LogFileFormatter.SubsequentLinePrefix.PadRight(headerLength, ' '));
						writer.WriteLine(subsequentLine);
					}
				}
			}
		}

		#endregion
	}
}
