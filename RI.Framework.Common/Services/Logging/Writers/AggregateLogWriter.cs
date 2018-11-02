using System;
using System.Collections;
using System.Collections.Generic;

using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging.Filters;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Logging.Writers
{
    /// <summary>
    ///     Implements a log writer which contains other log writers.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See <see cref="ILogWriter" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
    public sealed class AggregateLogWriter : ILogWriter, ICollection<ILogWriter>, ICollection, IReadOnlyCollection<ILogWriter>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="AggregateLogWriter" />.
        /// </summary>
        public AggregateLogWriter ()
            : this((IEnumerable<ILogWriter>)null)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="AggregateLogWriter" />.
        /// </summary>
        /// <param name="writers"> The sequence of writers which are aggregated. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="writers" /> is enumerated exactly once.
        ///     </para>
        /// </remarks>
        public AggregateLogWriter (IEnumerable<ILogWriter> writers)
        {
            this.SyncRoot = new object();

            this.Writers = new HashSet<ILogWriter>();

            if (writers != null)
            {
                foreach (ILogWriter writer in writers)
                {
                    this.Add(writer);
                }
            }

            this.UpdateCopy();
        }

        /// <summary>
        ///     Creates a new instance of <see cref="AggregateLogWriter" />.
        /// </summary>
        /// <param name="writers"> The array of writers which are aggregated. </param>
        public AggregateLogWriter (params ILogWriter[] writers)
            : this((IEnumerable<ILogWriter>)writers)
        {
        }

        #endregion




        #region Instance Fields

        private ILogFilter _filter;

        #endregion




        #region Instance Properties/Indexer

        private List<ILogWriter> Copy { get; set; }

        private HashSet<ILogWriter> Writers { get; }

        #endregion




        #region Instance Methods

        private void UpdateCopy ()
        {
            this.Copy = new List<ILogWriter>(this.Writers);
        }

        #endregion




        #region Interface: ICollection

        /// <inheritdoc />
        bool ICollection.IsSynchronized => ((ISynchronizable)this).IsSynchronized;

        /// <inheritdoc />
        void ICollection.CopyTo (Array array, int index)
        {
            lock (this.SyncRoot)
            {
                int i1 = 0;
                foreach (ILogWriter item in this)
                {
                    array.SetValue(item, index + i1);
                    i1++;
                }
            }
        }

        #endregion




        #region Interface: ICollection<ILogWriter>

        /// <inheritdoc />
        public int Count
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this.Writers.Count;
                }
            }
        }

        /// <inheritdoc />
        bool ICollection<ILogWriter>.IsReadOnly => false;

        /// <inheritdoc />
        public void Add (ILogWriter item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            lock (this.SyncRoot)
            {
                this.Writers.Add(item);

                this.UpdateCopy();
            }
        }

        /// <inheritdoc />
        public void Clear ()
        {
            lock (this.SyncRoot)
            {
                this.Writers.Clear();

                this.UpdateCopy();
            }
        }

        /// <inheritdoc />
        public bool Contains (ILogWriter item)
        {
            lock (this.SyncRoot)
            {
                return this.Writers.Contains(item);
            }
        }

        /// <inheritdoc />
        void ICollection<ILogWriter>.CopyTo (ILogWriter[] array, int arrayIndex)
        {
            lock (this.SyncRoot)
            {
                this.Writers.CopyTo(array, arrayIndex);
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return this.GetEnumerator();
        }

        /// <inheritdoc />
        public IEnumerator<ILogWriter> GetEnumerator ()
        {
            lock (this.SyncRoot)
            {
                return this.Writers.GetEnumerator();
            }
        }

        /// <inheritdoc />
        public bool Remove (ILogWriter item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            lock (this.SyncRoot)
            {
                bool result = this.Writers.Remove(item);

                this.UpdateCopy();

                return result;
            }
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
        public object SyncRoot { get; }

        /// <inheritdoc />
        public void Cleanup (DateTime retentionDate)
        {
            lock (this.SyncRoot)
            {
                foreach (ILogWriter writer in this.Writers)
                {
                    writer.Cleanup(retentionDate);
                }
            }
        }

        /// <inheritdoc />
        public void Log (DateTime timestamp, int threadId, LogLevel severity, string source, string message)
        {
            lock (this.SyncRoot)
            {
                foreach (ILogWriter writer in this.Writers)
                {
                    writer.Log(timestamp, threadId, severity, source, message);
                }
            }
        }

        #endregion
    }
}
