using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Collections.Generic
{
	/// <summary>
	///     Implements a priority queue.
	/// </summary>
	/// <typeparam name="T"> The type of items stored in the priority queue. </typeparam>
	/// <remarks>
	///     <para>
	///         A priority queue stores items sorted by their assigned priority.
	///         The priority is assigned to an item when the item is added to the priority queue using <see cref="Enqueue" />.
	///         The higher the priority, the earlier the item is dequeued (highest priority, first out).
	///         For items of the same priority, the order in which they are added is maintained (first in, first out).
	///     </para>
	///     <para>
	///         null are valid item values if <typeparamref name="T" /> is a reference type.
	///     </para>
	///     <para>
	///         The performance of the priority queue degrades with the number of different priorities used.
	///         Regardless of the actual numeric priority values or the distribution of the priority values respectively, a priority queue with, for example, 10 used priorities is on average 10 times faster than a priority queue with 100 used priorities.
	///     </para>
	/// </remarks>
	public sealed class PriorityQueue <T> : ICollection, IEnumerable<T>, IEnumerable, ISynchronizable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="PriorityQueue{T}" />.
		/// </summary>
		public PriorityQueue ()
		{
			this.SyncRoot = new object();
			this.Chain = new LinkedList<PriorityItem>();
			this.Table = new Hashtable();
		}

		#endregion




		#region Instance Properties/Indexer

		private LinkedList<PriorityItem> Chain { get; set; }

		private object SyncRoot { get; set; }

		private Hashtable Table { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Removes all items from the priority queue.
		/// </summary>
		/// <remarks>
		///     <para>
		///         This is a O(1) operation.
		///     </para>
		/// </remarks>
		public void Clear ()
		{
			this.Chain.Clear();
			this.Table.Clear();
		}

		/// <inheritdoc cref="ICollection.CopyTo" />
		public void CopyTo (T[] array, int index)
		{
			foreach (T item in this)
			{
				array[index] = item;
				index++;
			}
		}

		/// <summary>
		///     Gets the next item in the queue and removes it.
		/// </summary>
		/// <param name="priority"> The priority of the item. </param>
		/// <returns>
		///     The item.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(1) operation.
		///     </para>
		/// </remarks>
		/// <exception cref="InvalidOperationException"> The priority queue is empty. </exception>
		public T Dequeue (out int priority)
		{
			return this.Get(true, out priority);
		}

		/// <summary>
		///     Adds an item to the queue.
		/// </summary>
		/// <param name="item"> The item. </param>
		/// <param name="priority"> The priority of the item. </param>
		/// <remarks>
		///     <para>
		///         This is a O(x) operation, where x is the number of priorities currently in use, if <paramref name="priority" /> is currently not yet in use, or a O(1) operation if there are already other items of the same priority.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than zero. </exception>
		[SuppressMessage ("ReSharper", "PossibleNullReferenceException")]
		public void Enqueue (T item, int priority)
		{
			if (priority < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(priority));
			}

			int firstPriority = 0;
			int lastPriority = 0;

			if (this.Table.Count > 0)
			{
				firstPriority = this.Chain.First.Value.Priority;
				lastPriority = this.Chain.Last.Value.Priority;
			}

			PriorityItem priorityItem;
			if (this.Table.Count == 0)
			{
				priorityItem = new PriorityItem(priority);
				this.Table.Add(priority, priorityItem);
				this.Chain.AddFirst(priorityItem);
			}
			else if (this.Table.ContainsKey(priority))
			{
				priorityItem = (PriorityItem)this.Table[priority];
			}
			else if (priority < firstPriority)
			{
				priorityItem = new PriorityItem(priority);
				this.Table.Add(priority, priorityItem);
				this.Chain.AddFirst(priorityItem);
			}
			else if (priority > lastPriority)
			{
				priorityItem = new PriorityItem(priority);
				this.Table.Add(priority, priorityItem);
				this.Chain.AddLast(priorityItem);
			}
			else if ((priority - firstPriority) < (lastPriority - priority))
			{
				priorityItem = new PriorityItem(priority);
				this.Table.Add(priority, priorityItem);
				LinkedListNode<PriorityItem> node = this.Chain.First;
				while (node.Value.Priority < priority)
				{
					node = node.Next;
				}
				this.Chain.AddBefore(node, priorityItem);
			}
			else
			{
				priorityItem = new PriorityItem(priority);
				this.Table.Add(priority, priorityItem);
				LinkedListNode<PriorityItem> node = this.Chain.Last;
				while (node.Value.Priority > priority)
				{
					node = node.Previous;
				}
				this.Chain.AddAfter(node, priorityItem);
			}

			priorityItem.Enqueue(item);
		}

		/// <summary>
		///     Gets the next item in the queue without removing it.
		/// </summary>
		/// <param name="priority"> The priority of the item. </param>
		/// <returns>
		///     The item.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(1) operation.
		///     </para>
		/// </remarks>
		/// <exception cref="InvalidOperationException"> The priority queue is empty. </exception>
		public T Peek (out int priority)
		{
			return this.Get(false, out priority);
		}

		private T Get (bool remove, out int priority)
		{
			if (this.Chain.Count == 0)
			{
				throw new InvalidOperationException("The priority queue is empty.");
			}

			PriorityItem priorityItem = this.Chain.Last.Value;
			priority = priorityItem.Priority;
			T item = remove ? priorityItem.Dequeue() : priorityItem.Peek();
			if (priorityItem.Count == 0)
			{
				this.Chain.RemoveLast();
				this.Table.Remove(priority);
			}
			return item;
		}

		#endregion




		#region Interface: ICollection

		/// <inheritdoc />
		/// <remarks>
		///     <para>
		///         This is a O(x) operation where x is the number of priorities currently in use.
		///     </para>
		/// </remarks>
		public int Count
		{
			get
			{
				int count = 0;
				foreach (PriorityItem chainItem in this.Chain)
				{
					count += chainItem.Count;
				}
				return count;
			}
		}

		/// <inheritdoc />
		bool ICollection.IsSynchronized => ((ISynchronizable)this).IsSynchronized;

		/// <inheritdoc />
		object ICollection.SyncRoot => ((ISynchronizable)this).SyncRoot;

		/// <inheritdoc />
		void ICollection.CopyTo (Array array, int index)
		{
			this.CopyTo((T[])array, index);
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return this.GetEnumerator();
		}

		#endregion




		#region Interface: IEnumerable<T>

		/// <inheritdoc />
		public IEnumerator<T> GetEnumerator ()
		{
			LinkedListNode<PriorityItem> node = this.Chain.Last;
			while (node != null)
			{
				foreach (T item in node.Value)
				{
					yield return item;
				}
				node = node.Previous;
			}
		}

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => false;

		/// <inheritdoc />
		object ISynchronizable.SyncRoot => this.SyncRoot;

		#endregion




		#region Type: PriorityItem

		private sealed class PriorityItem : Queue<T>
		{
			#region Instance Constructor/Destructor

			public PriorityItem (int priority)
			{
				if (priority < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(priority));
				}

				this.Priority = priority;
			}

			#endregion




			#region Instance Properties/Indexer

			public int Priority { get; private set; }

			#endregion
		}

		#endregion
	}
}
