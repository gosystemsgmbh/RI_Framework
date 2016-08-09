using System;
using System.Collections.Generic;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Collections.ObjectModel
{
	/// <summary>
	///     Implements a simple tree node with an assigned value per tree node.
	/// </summary>
	/// <typeparam name="TValue"> The type of the value which is assigned to a tree node. </typeparam>
	/// <example>
	///     <para>
	///         The following example shows how a <see cref="TreeNode{T}" /> can be used:
	///     </para>
	///     <code language="cs">
	/// <![CDATA[
	/// // create root node
	/// var admiral = new TreeNode<string>("Francine F. Tallwood");
	/// 
	/// // create subnodes
	/// var captain1 = admiral.AddChild("Gregory M. T. Heads");
	/// var captain2 = admiral.AddChild("Adrian Heisenburg");
	/// 
	/// // create subsubnodes
	/// var commander1 = captain1.AddChild("Harry Gunlaf");
	/// var commander2 = captain1.AdChild("Wayne Kacynczkyij");
	/// var commander3 = captain2.AddChild("Barbara Walker");
	/// var commander4 = captain2.AddChild("Ferdinand Eisenmeier");
	/// ]]>
	/// </code>
	/// </example>
	public sealed class TreeNode <TValue> : TreeNodeBase<TreeNode<TValue>>,
	                                        ICloneable<TreeNode<TValue>>,
	                                        ICloneable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="TreeNodeBase{TNode}" />.
		/// </summary>
		public TreeNode ()
				: this(default(TValue))
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="TreeNodeBase{TNode}" />.
		/// </summary>
		/// <param name="value"> The value assigned to this tree node. </param>
		public TreeNode (TValue value)
		{
			this.Value = value;
		}

		/// <summary>
		///     Creates a new instance of <see cref="TreeNodeBase{TNode}" />.
		/// </summary>
		/// <param name="children"> A sequence if child nodes initially added to the new tree node. </param>
		/// <remarks>
		///     <para>
		///         <paramref name="children" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="children" /> is null. </exception>
		public TreeNode (IEnumerable<TreeNode<TValue>> children)
				: this(default(TValue), children)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="TreeNodeBase{TNode}" />.
		/// </summary>
		/// <param name="value"> The value assigned to this tree node. </param>
		/// <param name="children"> A sequence if child nodes initially added to the new tree node. </param>
		/// <remarks>
		///     <para>
		///         <paramref name="children" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="children" /> is null. </exception>
		public TreeNode (TValue value, IEnumerable<TreeNode<TValue>> children)
				: base(children)
		{
			this.Value = value;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets the value assigned to this tree node.
		/// </summary>
		/// <value>
		///     The value assigned to this tree node.
		/// </value>
		public TValue Value { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Creates and adds a new child node to this tree node.
		/// </summary>
		/// <returns>
		///     The newly created and added child node.
		/// </returns>
		public TreeNode<TValue> AddChild ()
		{
			TreeNode<TValue> child = new TreeNode<TValue>();
			this.Children.Add(child);
			return child;
		}

		/// <summary>
		///     Creates and adds a new child node to this tree node.
		/// </summary>
		/// <param name="value"> The value to be assigned to the new child node. </param>
		/// <returns>
		///     The newly created and added child node.
		/// </returns>
		public TreeNode<TValue> AddChild (TValue value)
		{
			TreeNode<TValue> child = new TreeNode<TValue>(value);
			this.Children.Add(child);
			return child;
		}

		/// <summary>
		///     Creates and adds a number of new child nodes to this tree node.
		/// </summary>
		/// <param name="numChildren"> The number of child nodes to add. </param>
		/// <returns>
		///     The list of newly created and added child nodes.
		///     The list is empty if <paramref name="numChildren" /> is zero.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="numChildren" /> is less than zero. </exception>
		public List<TreeNode<TValue>> AddChildren (int numChildren)
		{
			if (numChildren < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(numChildren));
			}

			List<TreeNode<TValue>> children = new List<TreeNode<TValue>>();
			for (int i1 = 0; i1 < numChildren; i1++)
			{
				TreeNode<TValue> child = new TreeNode<TValue>();
				this.Children.Add(child);
				children.Add(child);
			}
			return children;
		}

		/// <summary>
		///     Creates and adds new child nodes to this tree node.
		/// </summary>
		/// <param name="values"> The sequence of values to be assigned to the new child nodes. </param>
		/// <returns>
		///     The list of newly created and added child nodes.
		///     The list is empty if <paramref name="values" /> contained no elements.
		/// </returns>
		/// <remarks>
		///     <para>
		///         <paramref name="values" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="values" /> is null. </exception>
		public List<TreeNode<TValue>> AddChildren (IEnumerable<TValue> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException(nameof(values));
			}

			List<TreeNode<TValue>> children = new List<TreeNode<TValue>>();
			foreach (TValue value in values)
			{
				TreeNode<TValue> child = new TreeNode<TValue>(value);
				this.Children.Add(child);
				children.Add(child);
			}
			return children;
		}

		private TValue CloneValue ()
		{
			object value = null;

			ICloneable<TValue> cloneable1 = this.Value as ICloneable<TValue>;
			if (cloneable1 != null)
			{
				value = cloneable1.Clone();
			}

			if (value is TValue)
			{
				return (TValue)value;
			}

			ICloneable cloneable2 = this.Value as ICloneable;
			if (cloneable2 != null)
			{
				value = cloneable2.Clone();
			}

			if (value is TValue)
			{
				return (TValue)value;
			}

			return this.Value;
		}

		#endregion




		#region Interface: ICloneable<TreeNode<TValue>>

		/// <summary>
		///     Creates a clone of this tree node, including its assigned value and all its child nodes.
		/// </summary>
		/// <returns>
		///     The clone of this tree node.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The clone will be a root node with a fully cloned tree structure of and below this tree node, which is structurally independent from this tree node or any other node in this tree.
		///     </para>
		///     <para>
		///         The <see cref="Value" /> of the cloned tree nodes are also cloned if <typeparamref name="TValue" /> implements <see cref="ICloneable{T}" /> or <see cref="ICloneable" />.
		///     </para>
		/// </remarks>
		public TreeNode<TValue> Clone ()
		{
			TreeNode<TValue> clone = new TreeNode<TValue>(this.CloneValue());
			foreach (TreeNode<TValue> child in this.Children)
			{
				clone.Children.Add(child.Clone());
			}
			return clone;
		}

		/// <inheritdoc cref="TreeNode{TValue}.Clone" />
		object ICloneable.Clone ()
		{
			return this.Clone();
		}

		#endregion
	}
}
