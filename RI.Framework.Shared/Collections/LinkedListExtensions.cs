using System;
using System.Collections.Generic;




namespace RI.Framework.Collections
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="LinkedList{T}" /> type.
	/// </summary>
	public static class LinkedListExtensions
	{
		#region Static Methods

		/// <summary>
		///     Enumerates the items of a linked list backwards.
		/// </summary>
		/// <typeparam name="T"> The type of the items in <paramref name="list" />. </typeparam>
		/// <param name="list"> The linked list. </param>
		/// <returns>
		///     The <see cref="IEnumerable{T}" /> which enumerates the linked list items.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="list" /> is null. </exception>
		public static IEnumerable<T> AsItemsBackward <T> (this LinkedList<T> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException(nameof(list));
			}

			LinkedListNode<T> node = list.Last;
			while (node != null)
			{
				yield return node.Value;
				node = node.Previous;
			}
		}

		/// <summary>
		///     Enumerates the items of a linked list forwards.
		/// </summary>
		/// <typeparam name="T"> The type of the items in <paramref name="list" />. </typeparam>
		/// <param name="list"> The linked list. </param>
		/// <returns>
		///     The <see cref="IEnumerable{T}" /> which enumerates the linked list nodes.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="list" /> is null. </exception>
		public static IEnumerable<T> AsItemsForward <T> (this LinkedList<T> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException(nameof(list));
			}

			LinkedListNode<T> node = list.First;
			while (node != null)
			{
				yield return node.Value;
				node = node.Next;
			}
		}

		/// <summary>
		///     Allows a linked list to be enumerated as its nodes, starting at the last node, rather than its values.
		/// </summary>
		/// <typeparam name="T"> The type of the items in <paramref name="list" />. </typeparam>
		/// <param name="list"> The linked list. </param>
		/// <returns>
		///     The <see cref="IEnumerable{T}" /> which enumerates the linked list nodes.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="list" /> is null. </exception>
		public static IEnumerable<LinkedListNode<T>> AsNodesBackward <T> (this LinkedList<T> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException(nameof(list));
			}

			LinkedListNode<T> node = list.Last;
			while (node != null)
			{
				yield return node;
				node = node.Previous;
			}
		}

		/// <summary>
		///     Allows a linked list to be enumerated as its nodes, starting at the first node, rather than its values.
		/// </summary>
		/// <typeparam name="T"> The type of the items in <paramref name="list" />. </typeparam>
		/// <param name="list"> The linked list. </param>
		/// <returns>
		///     The <see cref="IEnumerable{T}" /> which enumerates the linked list nodes.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="list" /> is null. </exception>
		public static IEnumerable<LinkedListNode<T>> AsNodesForward <T> (this LinkedList<T> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException(nameof(list));
			}

			LinkedListNode<T> node = list.First;
			while (node != null)
			{
				yield return node;
				node = node.Next;
			}
		}

		/// <summary>
		///     Finds all nodes of a linked list which satisfy a condition.
		/// </summary>
		/// <typeparam name="T"> The type of the items in <paramref name="list" />. </typeparam>
		/// <param name="list"> The linked list. </param>
		/// <param name="condition"> The function which tests each node for a condition, providing the nodes index and the node itself. </param>
		/// <returns>
		///     The list of nodes which satisfy the specified condition.
		///     The list is empty if no node satisfies the condition.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="list" /> or <paramref name="condition" /> is null. </exception>
		public static List<LinkedListNode<T>> FindWhere <T> (this LinkedList<T> list, Func<int, LinkedListNode<T>, bool> condition)
		{
			if (list == null)
			{
				throw new ArgumentNullException(nameof(list));
			}

			if (condition == null)
			{
				throw new ArgumentNullException(nameof(condition));
			}

			List<LinkedListNode<T>> nodes = new List<LinkedListNode<T>>();
			int index = 0;
			foreach (LinkedListNode<T> node in list.AsNodesForward())
			{
				if (condition(index, node))
				{
					nodes.Add(node);
				}
				index++;
			}
			return nodes;
		}

		/// <summary>
		///     Finds all nodes of a linked list which satisfy a condition.
		/// </summary>
		/// <typeparam name="T"> The type of the items in <paramref name="list" />. </typeparam>
		/// <param name="list"> The linked list. </param>
		/// <param name="condition"> The function which tests each node for a condition, providing the node itself. </param>
		/// <returns>
		///     The list of nodes which satisfy the specified condition.
		///     The list is empty if no node satisfies the condition.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="list" /> or <paramref name="condition" /> is null. </exception>
		public static List<LinkedListNode<T>> FindWhere <T> (this LinkedList<T> list, Func<LinkedListNode<T>, bool> condition)
		{
			if (list == null)
			{
				throw new ArgumentNullException(nameof(list));
			}

			if (condition == null)
			{
				throw new ArgumentNullException(nameof(condition));
			}

			List<LinkedListNode<T>> nodes = new List<LinkedListNode<T>>();
			foreach (LinkedListNode<T> node in list.AsNodesForward())
			{
				if (condition(node))
				{
					nodes.Add(node);
				}
			}
			return nodes;
		}

		#endregion
	}
}
