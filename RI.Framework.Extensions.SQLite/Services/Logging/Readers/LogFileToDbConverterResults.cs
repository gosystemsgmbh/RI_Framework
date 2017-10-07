using System;
using System.Collections.Generic;
using System.Linq;

using RI.Framework.IO.Paths;




namespace RI.Framework.Services.Logging.Readers
{
	/// <summary>
	///     Holds conversion results from a single conversion performed by <see cref="LogFileToDbConverter" />.
	/// </summary>
	public sealed class LogFileToDbConverterResults
	{
		#region Instance Constructor/Destructor

		internal LogFileToDbConverterResults ()
		{
			this.Files = new List<FilePath>();
			this.Errors = new List<Tuple<FilePath, int>>();
			this.Entries = new List<Tuple<FilePath, int>>();
			this.Exception = null;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets a list of numbers of entries per log file.
		/// </summary>
		/// <value>
		///     The list of numbers of entries per log file.
		/// </value>
		/// <remarks>
		///     <para>
		///         The tuple contains the file and the number of valid log entries read.
		///     </para>
		/// </remarks>
		public List<Tuple<FilePath, int>> Entries { get; private set; }

		/// <summary>
		///     Gets a list of log file reading errors.
		/// </summary>
		/// <value>
		///     The list of log file reading errors.
		/// </value>
		/// <remarks>
		///     <para>
		///         The tuple contains the file and the line number of the error.
		///     </para>
		/// </remarks>
		public List<Tuple<FilePath, int>> Errors { get; private set; }

		/// <summary>
		///     Gets or sets the exception occured during a conversion.
		/// </summary>
		/// <value>
		///     The exception occured during a conversion.
		/// </value>
		public Exception Exception { get; set; }

		/// <summary>
		///     Gets a list of files which were converted.
		/// </summary>
		/// <value>
		///     The list of files which were converted.
		/// </value>
		public List<FilePath> Files { get; private set; }

		/// <summary>
		///     Gets the total number of log entries read.
		/// </summary>
		/// <value>
		///     The total number of log entries read.
		/// </value>
		/// TODO: Use number extension and remove using of System.Linq
		public int TotalEntries => this.Entries.Sum(x => x.Item2);

		#endregion
	}
}
