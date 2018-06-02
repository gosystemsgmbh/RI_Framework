using System;
using System.Collections.Generic;




namespace RI.Framework.Collections.Generic
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="IPriorityQueue{T}" /> type and its implementations.
	/// </summary>
	public static class IPriorityQueueExtensions
	{
		#region Static Methods

		/// <summary>
		///     Removes items from the queue based on a predicate.
		/// </summary>
		/// <param name="priorityQueue"> The priority queue. </param>
		/// <param name="predicate"> The predicate. </param>
		/// <returns>
		///     The list of removed items.
		/// </returns>
		/// <remarks>
		///     <para>
		///         All items for which the predicate returns true are removed, regardless of the priorities and the order in which they are in the queue.
		///     </para>
		///     <note type="important">
		///         This method is considered very slow as it needs to rebuild the whole internal structure of the priority queue.
		///     </note>
		///     <note type="important">
		///         The priority queue is left in an undefined, most probably empty, state if an exception is thrown by <paramref name="predicate" />.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="priorityQueue" /> or <paramref name="predicate" /> is null. </exception>
		public static List<T> Remove <T> (this IPriorityQueue<T> priorityQueue, PriorityQueueRemovePredicate<T> predicate)
		{
			if (priorityQueue == null)
			{
				throw new ArgumentNullException(nameof(priorityQueue));
			}

			if (predicate == null)
			{
				throw new ArgumentNullException(nameof(predicate));
			}

			List<RemovalTuple<T>> queueItems = new List<RemovalTuple<T>>();
			while (priorityQueue.Count > 0)
			{
				int priority;
				T queueItem = priorityQueue.Dequeue(out priority);
				queueItems.Add(new RemovalTuple<T>(queueItem, priority));
			}

			priorityQueue.Clear();

			List<T> removed = new List<T>();
			List<RemovalTuple<T>> preserved = new List<RemovalTuple<T>>();

			foreach (RemovalTuple<T> queueItem in queueItems)
			{
				bool remove = predicate(queueItem.Item, queueItem.Priority);
				if (remove)
				{
					removed.Add(queueItem.Item);
				}
				else
				{
					preserved.Add(queueItem);
				}
			}

			foreach (RemovalTuple<T> preservedItem in preserved)
			{
				priorityQueue.Enqueue(preservedItem.Item, preservedItem.Priority);
			}

			return removed;
		}

		/// <summary>
		///     Removes all occurences of an item from the priority queue.
		/// </summary>
		/// <param name="priorityQueue"> The priority queue. </param>
		/// <param name="item"> The item to remove. </param>
		/// <returns>
		///     The number of times the item was removed from the priority queue.
		/// </returns>
		/// <remarks>
		///     <para>
		///         All matching items are removed, regardless of the priorities and the order in which they are in the queue.
		///     </para>
		///     <note type="important">
		///         This method is considered very slow as it needs to rebuild the whole internal structure of the priority queue.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="priorityQueue" /> is null. </exception>
		public static int RemoveAll <T> (this IPriorityQueue<T> priorityQueue, T item) => priorityQueue.RemoveAll(item, null);

		/// <summary>
		///     Removes all occurences of an item from the priority queue.
		/// </summary>
		/// <param name="priorityQueue"> The priority queue. </param>
		/// <param name="item"> The item to remove. </param>
		/// <param name="comparer"> The comparer to use to test which items to remove or null to use the default comparer. </param>
		/// <returns>
		///     The number of times the item was removed from the priority queue.
		/// </returns>
		/// <remarks>
		///     <para>
		///         All matching items are removed, regardless of the priorities and the order in which they are in the queue.
		///     </para>
		///     <note type="important">
		///         This method is considered very slow as it needs to rebuild the whole internal structure of the priority queue.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="priorityQueue" /> is null. </exception>
		public static int RemoveAll <T> (this IPriorityQueue<T> priorityQueue, T item, IEqualityComparer<T> comparer)
		{
			if (priorityQueue == null)
			{
				throw new ArgumentNullException(nameof(priorityQueue));
			}

			comparer = comparer ?? EqualityComparer<T>.Default;
			List<T> removed = priorityQueue.Remove(((i, p) => comparer.Equals(i, item)));
			return removed.Count;
		}

		#endregion




		#region Type: RemovalTuple

		private sealed class RemovalTuple <T>
		{
			#region Instance Constructor/Destructor

			public RemovalTuple ()
			{
				this.Item = default(T);
				this.Priority = 0;
			}

			public RemovalTuple (T item, int priority)
			{
				this.Item = item;
				this.Priority = priority;
			}

			#endregion




			#region Instance Properties/Indexer

			public T Item { get; set; }

			public int Priority { get; set; }

			#endregion
		}

		#endregion
	}
}
