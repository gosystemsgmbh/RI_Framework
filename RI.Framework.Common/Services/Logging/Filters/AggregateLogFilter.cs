﻿using System;
using System.Collections;
using System.Collections.Generic;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Logging.Filters
{
	/// <summary>
	///     Implements a log filter which contains other log filters.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="ILogFilter" /> for more details.
	///     </para>
	/// <para>
	/// The filter mode (<see cref="Mode"/>) defines the behaviour when multiple filters are used.
	/// </para>
	/// <para>
	/// If no filter is used, <see cref="Filter"/> returns true.
	/// </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class AggregateLogFilter : ILogFilter, ISynchronizable, ICollection<ILogFilter>, ICollection
	{
		private AggregateLogFilterMode _mode;




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="AggregateLogFilter" />.
		/// </summary>
		public AggregateLogFilter()
			: this((IEnumerable<ILogFilter>)null)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="AggregateLogFilter" />.
		/// </summary>
		/// <param name="filters"> The sequence of filters which are aggregated. </param>
		/// <remarks>
		///     <para>
		///         <paramref name="filters" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		public AggregateLogFilter(IEnumerable<ILogFilter> filters)
		{
			this.SyncRoot = new object();

			this.Filters = new HashSet<ILogFilter>();
			this.Mode = AggregateLogFilterMode.And;

			if (filters != null)
			{
				foreach (ILogFilter filter in filters)
				{
					this.Add(filter);
				}
			}
		}

		/// <summary>
		///     Creates a new instance of <see cref="AggregateLogFilter" />.
		/// </summary>
		/// <param name="filters"> The array of filters which are aggregated. </param>
		public AggregateLogFilter(params ILogFilter[] filters)
			: this((IEnumerable<ILogFilter>)filters)
		{
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		/// Gets or sets the filter mode.
		/// </summary>
		/// <value>
		/// The filter mode.
		/// </value>
		/// <remarks>
		/// <para>
		/// The default value is <see cref="AggregateLogFilterMode.And"/>.
		/// </para>
		/// </remarks>
		public AggregateLogFilterMode Mode
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._mode;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._mode = value;
				}
			}
		}

		private HashSet<ILogFilter> Filters { get; }

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; }

		#endregion




		#region Interface: ICollection

		/// <inheritdoc />
		bool ICollection.IsSynchronized => ((ISynchronizable)this).IsSynchronized;

		/// <inheritdoc />
		void ICollection.CopyTo(Array array, int index)
		{
			lock (this.SyncRoot)
			{
				int i1 = 0;
				foreach (ILogFilter item in this)
				{
					array.SetValue(item, index + i1);
					i1++;
				}
			}
		}

		#endregion




		#region Interface: ICollection<CompositionCatalog>

		/// <inheritdoc />
		public int Count
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.Filters.Count;
				}
			}
		}

		/// <inheritdoc />
		bool ICollection<ILogFilter>.IsReadOnly => false;

		/// <inheritdoc />
		public void Add(ILogFilter item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			lock (this.SyncRoot)
			{
				this.Filters.Add(item);
			}
		}

		/// <inheritdoc />
		public void Clear()
		{
			lock (this.SyncRoot)
			{
				this.Filters.Clear();
			}
		}

		/// <inheritdoc />
		public bool Contains(ILogFilter item)
		{
			lock (this.SyncRoot)
			{
				return this.Filters.Contains(item);
			}
		}

		/// <inheritdoc />
		void ICollection<ILogFilter>.CopyTo(ILogFilter[] array, int arrayIndex)
		{
			lock (this.SyncRoot)
			{
				this.Filters.CopyTo(array, arrayIndex);
			}
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <inheritdoc />
		public IEnumerator<ILogFilter> GetEnumerator()
		{
			lock (this.SyncRoot)
			{
				return this.Filters.GetEnumerator();
			}
		}

		/// <inheritdoc />
		public bool Remove(ILogFilter item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			lock (this.SyncRoot)
			{
				return this.Filters.Remove(item);
			}
		}

		#endregion



		/// <inheritdoc />
		public bool Filter (DateTime timestamp, int threadId, LogLevel severity, string source)
		{
			lock (this.SyncRoot)
			{
				if (this.Filters.Count == 0)
				{
					return true;
				}

				return this.Mode == AggregateLogFilterMode.And ? this.Filters.All(x => x.Filter(timestamp, threadId, severity, source)) : this.Filters.Any(x => x.Filter(timestamp, threadId, severity, source));
			}
		}
	}
}
