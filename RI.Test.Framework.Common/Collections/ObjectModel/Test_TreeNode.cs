using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Collections.ObjectModel;




namespace RI.Test.Framework.Collections.ObjectModel
{
	[TestClass]
	public sealed class Test_TreeNode
	{
		#region Instance Methods

		[TestMethod]
		public void Test ()
		{
			TreeNode<int> root = new TreeNode<int>();
			TreeNode<int> n11 = new TreeNode<int>(11);
			TreeNode<int> n12 = new TreeNode<int>(12);
			TreeNode<int> n13 = new TreeNode<int>(13);
			TreeNode<int> n21 = new TreeNode<int>(21);
			TreeNode<int> n22 = new TreeNode<int>(22);
			TreeNode<int> n23 = new TreeNode<int>(23);

			if (root.Value != 0)
			{
				throw new TestAssertionException();
			}

			if (n12.Value != 12)
			{
				throw new TestAssertionException();
			}

			if (n22.Value != 22)
			{
				throw new TestAssertionException();
			}

			if (root.Children.Count != 0)
			{
				throw new TestAssertionException();
			}

			root.Children.Add(n11);
			root.Children.Add(n12);
			root.Children.Add(n13);

			n12.Children.Add(n21);
			n12.Children.Add(n22);
			n12.Children.Add(n23);

			if (root.Children.Count != 3)
			{
				throw new TestAssertionException();
			}

			if (n12.Children.Count != 3)
			{
				throw new TestAssertionException();
			}

			if (n22.Children.Count != 0)
			{
				throw new TestAssertionException();
			}

			if (root.Children[0] != n11)
			{
				throw new TestAssertionException();
			}

			root.Children[0] = new TreeNode<int>(999);

			if (root.Children[0].Value != 999)
			{
				throw new TestAssertionException();
			}

			root.Children[0] = n11;

			if (root.Children[0] != n11)
			{
				throw new TestAssertionException();
			}

			if (n22.Parent != n12)
			{
				throw new TestAssertionException();
			}

			if (n22.Parent.Parent != root)
			{
				throw new TestAssertionException();
			}

			if (n22.Parent.Parent.Parent != null)
			{
				throw new TestAssertionException();
			}

			if (!n12.Children.Contains(n22))
			{
				throw new TestAssertionException();
			}

			if (!n12.RemoveChild(n22))
			{
				throw new TestAssertionException();
			}

			if (n11.RemoveChild(n22))
			{
				throw new TestAssertionException();
			}

			if (n12.Children.Contains(n22))
			{
				throw new TestAssertionException();
			}

			if (!root.Children.AsEnumerable().SequenceEqual(new[] {n11, n12, n13}))
			{
				throw new TestAssertionException();
			}

			TreeNode<int> clone = root.Clone();

			if (clone.Children.Count != 3)
			{
				throw new TestAssertionException();
			}

			root.Children.Clear();

			if (root.Children.Count != 0)
			{
				throw new TestAssertionException();
			}

			if (clone.Children.Count != 3)
			{
				throw new TestAssertionException();
			}

			root = new TreeNode<int>(new[] {new TreeNode<int>()});

			if (root.Children.Count != 1)
			{
				throw new TestAssertionException();
			}

			root = new TreeNode<int>(99, new[] {new TreeNode<int>(), new TreeNode<int>()});

			if (root.Value != 99)
			{
				throw new TestAssertionException();
			}

			if (root.Children.Count != 2)
			{
				throw new TestAssertionException();
			}

			root.Children.Clear();

			if (root.AddChild().Value != 0)
			{
				throw new TestAssertionException();
			}

			if (root.Children.Count != 1)
			{
				throw new TestAssertionException();
			}

			if (root.Children.First().Value != 0)
			{
				throw new TestAssertionException();
			}

			root.Children.Clear();

			if (root.AddChild(50).Value != 50)
			{
				throw new TestAssertionException();
			}

			if (root.Children.Count != 1)
			{
				throw new TestAssertionException();
			}

			if (root.Children.First().Value != 50)
			{
				throw new TestAssertionException();
			}

			root.Children.First().Value++;
			if (root.Children.First().Value != 51)
			{
				throw new TestAssertionException();
			}

			root.Children.Clear();

			if (root.AddChildren(0).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (root.AddChildren(5).Count != 5)
			{
				throw new TestAssertionException();
			}

			if (root.Children.Count != 5)
			{
				throw new TestAssertionException();
			}

			if (root.Children.First().Value != 0)
			{
				throw new TestAssertionException();
			}

			root.Children.Clear();

			if (root.AddChildren(new int[] { }).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (root.AddChildren(new[] {1, 2, 3, 4, 5}).Count != 5)
			{
				throw new TestAssertionException();
			}

			if (root.Children.Count != 5)
			{
				throw new TestAssertionException();
			}

			if (root.Children.First().Value != 1)
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
