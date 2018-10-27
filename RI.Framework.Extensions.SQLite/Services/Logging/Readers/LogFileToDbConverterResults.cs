using System;
using System.Collections.Generic;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.IO.Paths;
using RI.Framework.Mathematic;




namespace RI.Framework.Services.Logging.Readers
{
    /// <summary>
    ///     Holds conversion results from a single conversion performed by <see cref="LogFileToDbConverter" />.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class LogFileToDbConverterResults
    {
        #region Instance Constructor/Destructor

        internal LogFileToDbConverterResults ()
        {
            this.Files = new HashSet<FilePath>();
            this.Errors = new Dictionary<FilePath, List<int>>();
            this.Entries = new Dictionary<FilePath, int>();
            this.Exception = null;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets number of converted entries per log file.
        /// </summary>
        /// <value>
        ///     The number of converted entries per log file.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The key represents the converted file, the value its number of converted entries.
        ///     </para>
        /// </remarks>
        public Dictionary<FilePath, int> Entries { get; }

        /// <summary>
        ///     Gets conversion errors per log file.
        /// </summary>
        /// <value>
        ///     The conversion errors per log file.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The key represents the converted file, the value is a list of line numbers with an error.
        ///     </para>
        /// </remarks>
        public Dictionary<FilePath, List<int>> Errors { get; }

        /// <summary>
        ///     Gets or sets the exception occured during the conversion.
        /// </summary>
        /// <value>
        ///     The exception occured during the conversion.
        /// </value>
        public Exception Exception { get; internal set; }

        /// <summary>
        ///     Gets the set of files which were converted.
        /// </summary>
        /// <value>
        ///     The set of files which were converted.
        /// </value>
        public HashSet<FilePath> Files { get; }

        /// <summary>
        ///     Gets the total number of log entries converted.
        /// </summary>
        /// <value>
        ///     The total number of log entries converted.
        /// </value>
        public int TotalEntries => this.Entries.Select(x => x.Value).Sum();

        #endregion




        #region Instance Methods

        internal void AddError (FilePath file, int lineNumber)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (lineNumber < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            }

            if (!this.Errors.ContainsKey(file))
            {
                this.Errors.Add(file, new List<int>());
            }

            this.Errors[file].Add(lineNumber);
        }

        #endregion
    }
}
