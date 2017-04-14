using System;
using System.Collections.Generic;




namespace RI.Framework.Collections.ObjectModel
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="ITreeNode{T}" /> type and its implementations.
	/// </summary>
	public static class ITreeNodeExtensions
	{
		#region Static Methods

		/// <summary>
		///     Gets a list of all child nodes of a tree node.
		/// </summary>
		/// <typeparam name="T"> The type of the tree nodes in the tree. </typeparam>
		/// <param name="treeNode"> The tree node. </param>
		/// <param name="includeSelf"> Specifies whether <paramref name="treeNode" /> itself should be included in the resulting list. </param>
		/// <returns>
		///     The list with all child nodes.
		///     The list is empty if there are no child nodes and <paramref name="includeSelf" /> is false.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="treeNode" /> is null. </exception>
		public static List<T> GetChildren <T> (this ITreeNode<T> treeNode, bool includeSelf)
			where T : class, ITreeNode<T>
		{
			if (treeNode == null)
			{
				throw new ArgumentNullException(nameof(treeNode));
			}

			List<T> children = new List<T>();
			if (includeSelf)
			{
				children.Add((T)treeNode);
			}
			if (treeNode.Children != null)
			{
				children.AddRange(treeNode.Children);
			}
			return children;
		}

		/// <summary>
		///     Gets a list of all child nodes, grand-child nodes, grand-grand-child nodes, etc. of a tree node.
		/// </summary>
		/// <typeparam name="T"> The type of the tree nodes in the tree. </typeparam>
		/// <param name="treeNode"> The tree node. </param>
		/// <param name="includeSelf"> Specifies whether <paramref name="treeNode" /> itself should be included in the resulting list. </param>
		/// <returns>
		///     The list with all child nodes, grand-child nodes, gran-grand child nodes, etc.
		///     The list is empty if there are no child nodes and <paramref name="includeSelf" /> is false.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="treeNode" /> is null. </exception>
		public static List<T> GetChildrenRecursive <T> (this ITreeNode<T> treeNode, bool includeSelf)
			where T : class, ITreeNode<T>
		{
			if (treeNode == null)
			{
				throw new ArgumentNullException(nameof(treeNode));
			}

			List<T> children = new List<T>();
			if (includeSelf)
			{
				children.Add((T)treeNode);
			}
			ITreeNodeExtensions.GetChildrenRecursiveInternal(treeNode, children);
			return children;
		}

		/// <summary>
		///     Gets a list of all leaf nodes of a tree node.
		/// </summary>
		/// <typeparam name="T"> The type of the tree nodes in the tree. </typeparam>
		/// <param name="treeNode"> The tree node. </param>
		/// <returns>
		///     The list with all leaf nodes.
		///     The list contains only <paramref name="treeNode" /> if it is a leaf itself.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="treeNode" /> is null. </exception>
		public static List<T> GetLeafs <T> (this ITreeNode<T> treeNode)
			where T : class, ITreeNode<T>
		{
			if (treeNode == null)
			{
				throw new ArgumentNullException(nameof(treeNode));
			}

			List<T> leafs = new List<T>();
			if (treeNode.Children == null)
			{
				return leafs;
			}
			foreach (T child in treeNode.Children)
			{
				if (child.IsLeaf())
				{
					leafs.Add(child);
				}
			}
			return leafs;
		}

		/// <summary>
		///     Gets a list of all leaf nodes of and below a tree node.
		/// </summary>
		/// <typeparam name="T"> The type of the tree nodes in the tree. </typeparam>
		/// <param name="treeNode"> The tree node. </param>
		/// <returns>
		///     The list with all leaf nodes.
		///     The list contains only <paramref name="treeNode" /> if it is a leaf itself.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="treeNode" /> is null. </exception>
		public static List<T> GetLeafsRecursive <T> (this ITreeNode<T> treeNode)
			where T : class, ITreeNode<T>
		{
			if (treeNode == null)
			{
				throw new ArgumentNullException(nameof(treeNode));
			}

			List<T> leafs = new List<T>();
			ITreeNodeExtensions.GetLeafsRecursiveInternal(treeNode, leafs);
			return leafs;
		}

		/// <summary>
		///     Gets the level or rank respectively of a tree node, relative to the root.
		/// </summary>
		/// <typeparam name="T"> The type of the tree nodes in the tree. </typeparam>
		/// <param name="treeNode"> The tree node. </param>
		/// <returns>
		///     The level of the tree node relative to the root.
		///     The root itself has a level of zero.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="treeNode" /> is null. </exception>
		public static int GetLevel <T> (this ITreeNode<T> treeNode)
			where T : class, ITreeNode<T>
		{
			if (treeNode == null)
			{
				throw new ArgumentNullException(nameof(treeNode));
			}

			int count = 0;
			T parent = (T)treeNode;
			while (parent.Parent != null)
			{
				count++;
				parent = parent.Parent;
			}
			return count;
		}

		/// <summary>
		///     Gets a list of all parent nodes up to the root node of a tree node.
		/// </summary>
		/// <typeparam name="T"> The type of the tree nodes in the tree. </typeparam>
		/// <param name="treeNode"> The tree node. </param>
		/// <param name="includeSelf"> Specifies whether <paramref name="treeNode" /> itself should be included in the resulting list. </param>
		/// <returns>
		///     The list with all parent nodes.
		///     The list is empty if <paramref name="treeNode" /> itself is the root node and <paramref name="includeSelf" /> is false.
		///     The list is ordered so that the first item is the root node and the last item is <paramref name="treeNode" /> itself or its parent, depending on <paramref name="includeSelf" />.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="treeNode" /> is null. </exception>
		public static List<T> GetParents <T> (this ITreeNode<T> treeNode, bool includeSelf)
			where T : class, ITreeNode<T>
		{
			if (treeNode == null)
			{
				throw new ArgumentNullException(nameof(treeNode));
			}

			List<T> parents = new List<T>();
			T parent = (T)treeNode;
			while (parent.Parent != null)
			{
				parent = parent.Parent;
				if (parent != null)
				{
					parents.Add(parent);
				}
			}
			parents.Reverse();
			if (includeSelf)
			{
				parents.Add((T)treeNode);
			}
			return parents;
		}

		/// <summary>
		///     Gets the root node of a tree node.
		/// </summary>
		/// <typeparam name="T"> The type of the tree nodes in the tree. </typeparam>
		/// <param name="treeNode"> The tree node. </param>
		/// <returns>
		///     The root node of a tree node.
		///     If <paramref name="treeNode" /> itself is the root node, <paramref name="treeNode" /> is returned.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="treeNode" /> is null. </exception>
		public static T GetRoot <T> (this ITreeNode<T> treeNode)
			where T : class, ITreeNode<T>
		{
			if (treeNode == null)
			{
				throw new ArgumentNullException(nameof(treeNode));
			}

			T parent = (T)treeNode;
			while (parent.Parent != null)
			{
				parent = parent.Parent;
			}
			return parent;
		}

		/// <summary>
		///     Gets a list of all sibling nodes of a tree node.
		/// </summary>
		/// <typeparam name="T"> The type of the tree nodes in the tree. </typeparam>
		/// <param name="treeNode"> The tree node. </param>
		/// <param name="includeSelf"> Specifies whether <paramref name="treeNode" /> itself should be included in the resulting list. </param>
		/// <returns>
		///     The list with all sibling nodes.
		///     The list is empty if there are no sibling nodes and <paramref name="includeSelf" /> is false.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="treeNode" /> is null. </exception>
		public static List<T> GetSiblings <T> (this ITreeNode<T> treeNode, bool includeSelf)
			where T : class, ITreeNode<T>
		{
			if (treeNode == null)
			{
				throw new ArgumentNullException(nameof(treeNode));
			}

			List<T> siblings = null;
			if (treeNode.Parent != null)
			{
				if (treeNode.Parent.Children != null)
				{
					siblings = new List<T>(treeNode.Parent.Children.Count);
					foreach (T sibling in treeNode.Parent.Children)
					{
						if ((!object.ReferenceEquals(sibling, treeNode)) || includeSelf)
						{
							siblings.Add(sibling);
						}
					}
				}
				else
				{
					if (includeSelf)
					{
						siblings = new List<T>(1)
						{
							(T)treeNode
						};
					}
				}
			}
			else
			{
				if (includeSelf)
				{
					siblings = new List<T>(1)
					{
						(T)treeNode
					};
				}
			}
			return siblings ?? new List<T>();
		}

		/// <summary>
		///     Determines whether a tree node is a child node.
		/// </summary>
		/// <typeparam name="T"> The type of the tree nodes in the tree. </typeparam>
		/// <param name="treeNode"> The tree node. </param>
		/// <returns>
		///     true if the tree node is a child node, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         A child node is a tree node which has a parent node.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="treeNode" /> is null. </exception>
		public static bool IsChild <T> (this ITreeNode<T> treeNode)
			where T : class, ITreeNode<T>
		{
			if (treeNode == null)
			{
				throw new ArgumentNullException(nameof(treeNode));
			}

			return treeNode.Parent != null;
		}

		/// <summary>
		///     Determines whether a tree node is a leaf node.
		/// </summary>
		/// <typeparam name="T"> The type of the tree nodes in the tree. </typeparam>
		/// <param name="treeNode"> The tree node. </param>
		/// <returns>
		///     true if the tree node is a leaf node, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         A leaf node is a tree node which has no child nodes.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="treeNode" /> is null. </exception>
		public static bool IsLeaf <T> (this ITreeNode<T> treeNode)
			where T : class, ITreeNode<T>
		{
			if (treeNode == null)
			{
				throw new ArgumentNullException(nameof(treeNode));
			}

			if (treeNode.Children == null)
			{
				return true;
			}

			return treeNode.Children.Count == 0;
		}

		/// <summary>
		///     Determines whether a tree node is a parent node.
		/// </summary>
		/// <typeparam name="T"> The type of the tree nodes in the tree. </typeparam>
		/// <param name="treeNode"> The tree node. </param>
		/// <returns>
		///     true if the tree node is a parent node, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         A parent node is a tree node which has at least one child node.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="treeNode" /> is null. </exception>
		public static bool IsParent <T> (this ITreeNode<T> treeNode)
			where T : class, ITreeNode<T>
		{
			if (treeNode == null)
			{
				throw new ArgumentNullException(nameof(treeNode));
			}

			if (treeNode.Children == null)
			{
				return false;
			}

			return treeNode.Children.Count != 0;
		}

		/// <summary>
		///     Determines whether a tree node is a root node.
		/// </summary>
		/// <typeparam name="T"> The type of the tree nodes in the tree. </typeparam>
		/// <param name="treeNode"> The tree node. </param>
		/// <returns>
		///     true if the tree node is a root node, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         A root node is a tree node which has no parent node.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="treeNode" /> is null. </exception>
		public static bool IsRoot <T> (this ITreeNode<T> treeNode)
			where T : class, ITreeNode<T>
		{
			if (treeNode == null)
			{
				throw new ArgumentNullException(nameof(treeNode));
			}

			return treeNode.Parent == null;
		}

		/// <summary>
		///     Determines whether a tree node is a sibling node or has sibling nodes respectively.
		/// </summary>
		/// <typeparam name="T"> The type of the tree nodes in the tree. </typeparam>
		/// <param name="treeNode"> The tree node. </param>
		/// <returns>
		///     true if the tree node is a sibling node, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         A sibling node is a tree node which parent has at least one other child node besides the tree node itself.
		///         A tree node which has no parent is not a sibling node.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="treeNode" /> is null. </exception>
		public static bool IsSibling <T> (this ITreeNode<T> treeNode)
			where T : class, ITreeNode<T>
		{
			if (treeNode == null)
			{
				throw new ArgumentNullException(nameof(treeNode));
			}

			return (treeNode.Parent?.Children?.Count).GetValueOrDefault(0) > 1;
		}

		/// <summary>
		///     Removes a child node from a tree node.
		/// </summary>
		/// <typeparam name="T"> The type of the tree nodes in the tree. </typeparam>
		/// <param name="treeNode"> The tree node. </param>
		/// <param name="item"> The child node to remove. </param>
		/// <returns>
		///     true if the child node was removed, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="treeNode" /> or <paramref name="item" /> is null. </exception>
		public static bool RemoveChild <T> (this ITreeNode<T> treeNode, T item)
			where T : class, ITreeNode<T>
		{
			if (treeNode == null)
			{
				throw new ArgumentNullException(nameof(treeNode));
			}

			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (treeNode.Children == null)
			{
				return false;
			}

			return treeNode.Children.Remove(item);
		}

		/// <summary>
		///     Removes a child node recursively from a tree node.
		/// </summary>
		/// <typeparam name="T"> The type of the tree nodes in the tree. </typeparam>
		/// <param name="treeNode"> The tree node. </param>
		/// <param name="item"> The child node to remove. </param>
		/// <returns>
		///     true if the child node was removed, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="treeNode" /> or <paramref name="item" /> is null. </exception>
		public static bool RemoveChildRecursive <T> (this ITreeNode<T> treeNode, T item)
			where T : class, ITreeNode<T>
		{
			if (treeNode == null)
			{
				throw new ArgumentNullException(nameof(treeNode));
			}

			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (treeNode.Children == null)
			{
				return false;
			}

			bool result = treeNode.Children.Remove(item);
			foreach (T child in treeNode.Children)
			{
				if (child.RemoveChildRecursive(item))
				{
					result = true;
				}
			}
			return result;
		}

		/// <summary>
		///     Removes multiple child nodes from a tree node.
		/// </summary>
		/// <typeparam name="T"> The type of the tree nodes in the tree. </typeparam>
		/// <param name="treeNode"> The tree node. </param>
		/// <param name="items"> The sequence of child nodes to remove. </param>
		/// <returns>
		///     The number of child nodes which were removed.
		///     Zero if the sequence contains no elements or no child nodes were removed.
		/// </returns>
		/// <remarks>
		///     <para>
		///         <paramref name="items" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="treeNode" /> or <paramref name="items" /> is null. </exception>
		public static int RemoveChildren <T> (this ITreeNode<T> treeNode, IEnumerable<T> items)
			where T : class, ITreeNode<T>
		{
			if (treeNode == null)
			{
				throw new ArgumentNullException(nameof(treeNode));
			}

			if (items == null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			if (treeNode.Children == null)
			{
				return 0;
			}

			return treeNode.Children.RemoveRange(items);
		}

		/// <summary>
		///     Removes multiple child nodes recursively from a tree node.
		/// </summary>
		/// <typeparam name="T"> The type of the tree nodes in the tree. </typeparam>
		/// <param name="treeNode"> The tree node. </param>
		/// <param name="items"> The sequence of child nodes to remove. </param>
		/// <returns>
		///     The number of child nodes which were removed.
		///     Zero if the sequence contains no elements or no child nodes were removed.
		/// </returns>
		/// <remarks>
		///     <para>
		///         <paramref name="items" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="treeNode" /> or <paramref name="items" /> is null. </exception>
		public static int RemoveChildrenRecursive <T> (this ITreeNode<T> treeNode, IEnumerable<T> items)
			where T : class, ITreeNode<T>
		{
			if (treeNode == null)
			{
				throw new ArgumentNullException(nameof(treeNode));
			}

			if (items == null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			if (treeNode.Children == null)
			{
				return 0;
			}

			int count = 0;
			foreach (T item in items)
			{
				if (treeNode.RemoveChildRecursive(item))
				{
					count++;
				}
			}
			return count;
		}

		/// <summary>
		///     Removes all child nodes from a tree node which satisfy a specified condition.
		/// </summary>
		/// <typeparam name="T"> The type of the tree nodes in the tree. </typeparam>
		/// <param name="treeNode"> The tree node. </param>
		/// <param name="condition"> The function which tests each child node for a condition. </param>
		/// <returns>
		///     The list with child nodes which were removed.
		///     The list is empty if no child nodes were removed.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="treeNode" /> or <paramref name="condition" /> is null. </exception>
		public static List<T> RemoveChildrenWhere <T> (this ITreeNode<T> treeNode, Func<T, bool> condition)
			where T : class, ITreeNode<T>
		{
			if (treeNode == null)
			{
				throw new ArgumentNullException(nameof(treeNode));
			}

			if (condition == null)
			{
				throw new ArgumentNullException(nameof(condition));
			}

			if (treeNode.Children == null)
			{
				return new List<T>();
			}

			return treeNode.Children.RemoveWhere(condition);
		}

		/// <summary>
		///     Removes recursively all child nodes from a tree node which satisfy a specified condition.
		/// </summary>
		/// <typeparam name="T"> The type of the tree nodes in the tree. </typeparam>
		/// <param name="treeNode"> The tree node. </param>
		/// <param name="condition"> The function which tests each child node for a condition. </param>
		/// <returns>
		///     The list with child nodes which were removed.
		///     The list is empty if no child nodes were removed.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="treeNode" /> or <paramref name="condition" /> is null. </exception>
		public static List<T> RemoveChildrenWhereRecursive <T> (this ITreeNode<T> treeNode, Func<T, bool> condition)
			where T : class, ITreeNode<T>
		{
			if (treeNode == null)
			{
				throw new ArgumentNullException(nameof(treeNode));
			}

			if (condition == null)
			{
				throw new ArgumentNullException(nameof(condition));
			}

			List<T> removedNodes = new List<T>();
			ITreeNodeExtensions.RemoveChildrenWhereRecursiveInternal(treeNode, condition, removedNodes);
			return removedNodes;
		}

		private static void GetChildrenRecursiveInternal <T> (ITreeNode<T> treeNode, ICollection<T> children)
			where T : class, ITreeNode<T>
		{
			if (treeNode.Children != null)
			{
				children.AddRange(treeNode.Children);
				foreach (T child in treeNode.Children)
				{
					ITreeNodeExtensions.GetChildrenRecursiveInternal(child, children);
				}
			}
		}

		private static void GetLeafsRecursiveInternal <T> (ITreeNode<T> treeNode, ICollection<T> leafs)
			where T : class, ITreeNode<T>
		{
			if (treeNode.IsLeaf())
			{
				leafs.Add((T)treeNode);
			}
			else
			{
				foreach (T child in treeNode.Children)
				{
					ITreeNodeExtensions.GetLeafsRecursiveInternal(child, leafs);
				}
			}
		}

		private static void RemoveChildrenWhereRecursiveInternal <T> (ITreeNode<T> treeNode, Func<T, bool> condition, ICollection<T> removedNodes)
			where T : class, ITreeNode<T>
		{
			if (treeNode.Children != null)
			{
				removedNodes.AddRange(treeNode.Children.RemoveWhere(condition));
				foreach (T child in treeNode.Children)
				{
					ITreeNodeExtensions.RemoveChildrenWhereRecursiveInternal(child, condition, removedNodes);
				}
			}
		}

		#endregion
	}
}
