using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;




namespace RI.Framework.Collections.ObjectModel
{
	/// <summary>
	///     Implements a base class which can be used for <see cref="ITreeNode{T}" /> implementations.
	/// </summary>
	/// <typeparam name="TNode"> The type of the tree nodes in the tree. This is usually the type itself which inherits from <see cref="TreeNodeBase{TNode}" />, e.g. <c> public class MyNode : TreeNodeBase&lt;MyNode&gt; { } </c>. </typeparam>
	public abstract class TreeNodeBase <TNode> : ITreeNode<TNode>
		where TNode : TreeNodeBase<TNode>
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="TreeNodeBase{TNode}" />.
		/// </summary>
		protected TreeNodeBase ()
		{
			this.Children = new TreeNodeList<TNode>((TNode)this);
			this.Parent = null;
		}

		/// <summary>
		///     Creates a new instance of <see cref="TreeNodeBase{TNode}" />.
		/// </summary>
		/// <param name="children"> A sequence of child nodes initially added to the new tree node. </param>
		/// <remarks>
		///     <para>
		///         <paramref name="children" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="children" /> is null. </exception>
		protected TreeNodeBase (IEnumerable<TNode> children)
			: this()
		{
			if (children == null)
			{
				throw new ArgumentNullException(nameof(children));
			}

			this.Children.AddRange(children);
		}

		#endregion




		#region Virtuals

		/// <summary>
		///     Called after a child node was added to this tree node.
		/// </summary>
		/// <param name="child"> The child node which was added. </param>
		protected virtual void OnChildAdded (TNode child)
		{
		}

		/// <summary>
		///     Called before a child node is being added to this tree node.
		/// </summary>
		/// <param name="child"> The child node being added. </param>
		protected virtual void OnChildAdding (TNode child)
		{
		}

		/// <summary>
		///     Called after a child node was removed from this tree node.
		/// </summary>
		/// <param name="child"> The child node which was removed. </param>
		protected virtual void OnChildRemoved (TNode child)
		{
		}

		/// <summary>
		///     Called before a child node is being removed from this tree node.
		/// </summary>
		/// <param name="child"> The child node being removed. </param>
		protected virtual void OnChildRemoving (TNode child)
		{
		}

		/// <summary>
		///     Called after the parent node of this tree node has changed.
		/// </summary>
		/// <param name="newParent"> The new parent node. </param>
		protected virtual void OnParentChanged (TNode newParent)
		{
		}

		/// <summary>
		///     Called before the parent node of this tree node is being changed.
		/// </summary>
		/// <param name="newParent"> The new parent node. </param>
		protected virtual void OnParentChanging (TNode newParent)
		{
		}

		#endregion




		#region Interface: ITreeNode<TNode>

		/// <inheritdoc />
		public IList<TNode> Children { get; private set; }

		/// <inheritdoc />
		public TNode Parent { get; private set; }

		#endregion




		#region Type: TreeNodeList

		private sealed class TreeNodeList <T> : IList<T>
			where T : TreeNodeBase<T>
		{
			#region Instance Constructor/Destructor

			public TreeNodeList (T node)
			{
				if (node == null)
				{
					throw (new ArgumentNullException(nameof(node)));
				}

				this.Node = node;
				this.Children = new List<T>();
			}

			#endregion




			#region Instance Properties/Indexer

			private List<T> Children { get; set; }

			private T Node { get; set; }

			#endregion




			#region Instance Methods

			[SuppressMessage ("ReSharper", "PossibleNullReferenceException")]
			private void SetItem (int index, TreeNodeOperation operation, T item)
			{
				T oldItem = (operation == TreeNodeOperation.Insert) ? null : this.Children[index];
				T newItem = (operation == TreeNodeOperation.Remove) ? null : item;

				T oldParent = newItem?.Parent;
				oldParent?.RemoveChild(newItem);

				if (oldItem != null)
				{
					this.Node.OnChildRemoving(oldItem);
					oldItem.OnParentChanging(null);
				}

				if (newItem != null)
				{
					this.Node.OnChildAdding(newItem);
					newItem.OnParentChanging(this.Node);
				}

				switch (operation)
				{
					case TreeNodeOperation.Insert:
					{
						newItem.Parent = this.Node;
						this.Children.Insert(index, newItem);
						break;
					}

					case TreeNodeOperation.Update:
					{
						newItem.Parent = this.Node;
						this.Children[index] = newItem;
						oldItem.Parent = null;
						break;
					}

					case TreeNodeOperation.Remove:
					{
						this.Children.RemoveAt(index);
						oldItem.Parent = null;
						break;
					}
				}

				if (oldItem != null)
				{
					this.Node.OnChildRemoved(oldItem);
					oldItem.OnParentChanged(null);
				}

				if (newItem != null)
				{
					this.Node.OnChildAdded(newItem);
					newItem.OnParentChanged(this.Node);
				}
			}

			#endregion




			#region Interface: IList<T>

			public int Count
			{
				get
				{
					return this.Children.Count;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return ((ICollection<T>)this.Children).IsReadOnly;
				}
			}

			public T this [int index]
			{
				get
				{
					if ((index < 0) || (index >= this.Count))
					{
						throw new ArgumentOutOfRangeException(nameof(index));
					}

					return this.Children[index];
				}
				set
				{
					if ((index < 0) || (index >= this.Count))
					{
						throw new ArgumentOutOfRangeException(nameof(index));
					}

					if (value == null)
					{
						throw new ArgumentNullException(nameof(value));
					}

					this.SetItem(index, TreeNodeOperation.Update, value);
				}
			}

			public void Add (T item)
			{
				if (item == null)
				{
					throw new ArgumentNullException(nameof(item));
				}

				this.Insert(this.Count, item);
			}

			public void Clear ()
			{
				while (this.Count > 0)
				{
					this.RemoveAt(this.Count - 1);
				}
			}

			public bool Contains (T item)
			{
				return this.Children.Contains(item);
			}

			public void CopyTo (T[] array, int arrayIndex)
			{
				((ICollection<T>)this.Children).CopyTo(array, arrayIndex);
			}

			IEnumerator IEnumerable.GetEnumerator ()
			{
				return this.Children.GetEnumerator();
			}

			public IEnumerator<T> GetEnumerator ()
			{
				return this.Children.GetEnumerator();
			}

			public int IndexOf (T item)
			{
				return this.Children.IndexOf(item);
			}

			public void Insert (int index, T item)
			{
				if ((index < 0) || (index > this.Count))
				{
					throw new ArgumentOutOfRangeException(nameof(index));
				}

				if (item == null)
				{
					throw new ArgumentNullException(nameof(item));
				}

				if (this.Contains(item))
				{
					return;
				}

				this.SetItem(index, TreeNodeOperation.Insert, item);
			}

			public bool Remove (T item)
			{
				int index = this.IndexOf(item);
				if (index == -1)
				{
					return false;
				}
				this.RemoveAt(index);
				return true;
			}

			public void RemoveAt (int index)
			{
				if ((index < 0) || (index >= this.Count))
				{
					throw new ArgumentOutOfRangeException(nameof(index));
				}

				this.SetItem(index, TreeNodeOperation.Remove, null);
			}

			#endregion
		}

		#endregion




		#region Type: TreeNodeOperation

		[Serializable]
		private enum TreeNodeOperation
		{
			Insert = 0,

			Update = 1,

			Remove = 2,
		}

		#endregion
	}
}
