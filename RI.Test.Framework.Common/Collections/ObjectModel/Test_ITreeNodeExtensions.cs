using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Collections.ObjectModel;
using RI.Framework.Collections.Linq;




namespace RI.Test.Framework.Collections.ObjectModel
{
	[TestClass]
	public sealed class Test_ITreeNodeExtensions
	{
		#region Instance Methods

		[TestMethod]
		public void GetChildren_Test ()
		{
			TreeNode<int> root = new TreeNode<int>();
			TreeNode<int> n11 = new TreeNode<int>(11);
			TreeNode<int> n12 = new TreeNode<int>(12);
			TreeNode<int> n13 = new TreeNode<int>(13);
			TreeNode<int> n21 = new TreeNode<int>(21);
			TreeNode<int> n22 = new TreeNode<int>(22);
			TreeNode<int> n23 = new TreeNode<int>(23);

			root.Children.Add(n11);
			root.Children.Add(n12);
			root.Children.Add(n13);

			n12.Children.Add(n21);
			n12.Children.Add(n22);
			n12.Children.Add(n23);

			List<TreeNode<int>> result = root.GetChildren(false);

			if (result.Count != 3)
			{
				throw new TestAssertionException();
			}

			if (result[0].Value != 11)
			{
				throw new TestAssertionException();
			}

			if (result[1].Value != 12)
			{
				throw new TestAssertionException();
			}

			if (result[2].Value != 13)
			{
				throw new TestAssertionException();
			}

			result = root.GetChildren(true);

			if (result.Count != 4)
			{
				throw new TestAssertionException();
			}

			if (result[0] != root)
			{
				throw new TestAssertionException();
			}

			if (result[1].Value != 11)
			{
				throw new TestAssertionException();
			}

			if (result[2].Value != 12)
			{
				throw new TestAssertionException();
			}

			if (result[3].Value != 13)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void GetChildrenRecursive_Test ()
		{
			TreeNode<int> root = new TreeNode<int>();
			TreeNode<int> n11 = new TreeNode<int>(11);
			TreeNode<int> n12 = new TreeNode<int>(12);
			TreeNode<int> n13 = new TreeNode<int>(13);
			TreeNode<int> n21 = new TreeNode<int>(21);
			TreeNode<int> n22 = new TreeNode<int>(22);
			TreeNode<int> n23 = new TreeNode<int>(23);

			root.Children.Add(n11);
			root.Children.Add(n12);
			root.Children.Add(n13);

			n12.Children.Add(n21);
			n12.Children.Add(n22);
			n12.Children.Add(n23);

			List<TreeNode<int>> result = root.GetChildrenRecursive(false);

			if (result.Count != 6)
			{
				throw new TestAssertionException();
			}

			if (result[0].Value != 11)
			{
				throw new TestAssertionException();
			}

			if (result[2].Value != 13)
			{
				throw new TestAssertionException();
			}

			if (result[3].Value != 21)
			{
				throw new TestAssertionException();
			}

			if (result[5].Value != 23)
			{
				throw new TestAssertionException();
			}

			result = root.GetChildrenRecursive(true);

			if (result.Count != 7)
			{
				throw new TestAssertionException();
			}

			if (result[0] != root)
			{
				throw new TestAssertionException();
			}

			if (result[1].Value != 11)
			{
				throw new TestAssertionException();
			}

			if (result[6].Value != 23)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void GetLeafs_Test ()
		{
			TreeNode<int> root = new TreeNode<int>();
			TreeNode<int> n11 = new TreeNode<int>(11);
			TreeNode<int> n12 = new TreeNode<int>(12);
			TreeNode<int> n13 = new TreeNode<int>(13);
			TreeNode<int> n21 = new TreeNode<int>(21);
			TreeNode<int> n22 = new TreeNode<int>(22);
			TreeNode<int> n23 = new TreeNode<int>(23);

			root.Children.Add(n11);
			root.Children.Add(n12);
			root.Children.Add(n13);

			n12.Children.Add(n21);
			n12.Children.Add(n22);
			n12.Children.Add(n23);

			List<TreeNode<int>> result = root.GetLeafs();

			if (result.Count != 2)
			{
				throw new TestAssertionException();
			}

			if (result[0].Value != 11)
			{
				throw new TestAssertionException();
			}

			if (result[1].Value != 13)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void GetLeafsRecursive_Test ()
		{
			TreeNode<int> root = new TreeNode<int>();
			TreeNode<int> n11 = new TreeNode<int>(11);
			TreeNode<int> n12 = new TreeNode<int>(12);
			TreeNode<int> n13 = new TreeNode<int>(13);
			TreeNode<int> n21 = new TreeNode<int>(21);
			TreeNode<int> n22 = new TreeNode<int>(22);
			TreeNode<int> n23 = new TreeNode<int>(23);

			root.Children.Add(n11);
			root.Children.Add(n12);
			root.Children.Add(n13);

			n12.Children.Add(n21);
			n12.Children.Add(n22);
			n12.Children.Add(n23);

			List<TreeNode<int>> result = root.GetLeafsRecursive();

			if (result.Count != 5)
			{
				throw new TestAssertionException();
			}

			if (result[0].Value != 11)
			{
				throw new TestAssertionException();
			}

			if (result[2].Value != 22)
			{
				throw new TestAssertionException();
			}

			if (result[4].Value != 13)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void GetParents_Test ()
		{
			TreeNode<int> root = new TreeNode<int>();
			TreeNode<int> n11 = new TreeNode<int>(11);
			TreeNode<int> n12 = new TreeNode<int>(12);
			TreeNode<int> n13 = new TreeNode<int>(13);
			TreeNode<int> n21 = new TreeNode<int>(21);
			TreeNode<int> n22 = new TreeNode<int>(22);
			TreeNode<int> n23 = new TreeNode<int>(23);

			root.Children.Add(n11);
			root.Children.Add(n12);
			root.Children.Add(n13);

			n12.Children.Add(n21);
			n12.Children.Add(n22);
			n12.Children.Add(n23);

			List<TreeNode<int>> result = n22.GetParents(false);

			if (result.Count != 2)
			{
				throw new TestAssertionException();
			}

			if (result[0] != root)
			{
				throw new TestAssertionException();
			}

			if (result[1].Value != 12)
			{
				throw new TestAssertionException();
			}

			result = n22.GetParents(true);

			if (result.Count != 3)
			{
				throw new TestAssertionException();
			}

			if (result[0] != root)
			{
				throw new TestAssertionException();
			}

			if (result[1].Value != 12)
			{
				throw new TestAssertionException();
			}

			if (result[2].Value != 22)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void GetSiblings_Test ()
		{
			TreeNode<int> root = new TreeNode<int>();
			TreeNode<int> n11 = new TreeNode<int>(11);
			TreeNode<int> n12 = new TreeNode<int>(12);
			TreeNode<int> n13 = new TreeNode<int>(13);
			TreeNode<int> n21 = new TreeNode<int>(21);
			TreeNode<int> n22 = new TreeNode<int>(22);
			TreeNode<int> n23 = new TreeNode<int>(23);

			root.Children.Add(n11);
			root.Children.Add(n12);
			root.Children.Add(n13);

			n12.Children.Add(n21);
			n12.Children.Add(n22);
			n12.Children.Add(n23);

			List<TreeNode<int>> result = n22.GetSiblings(false);

			if (result.Count != 2)
			{
				throw new TestAssertionException();
			}

			if (result[0].Value != 21)
			{
				throw new TestAssertionException();
			}

			if (result[1].Value != 23)
			{
				throw new TestAssertionException();
			}

			result = n22.GetSiblings(true);

			if (result.Count != 3)
			{
				throw new TestAssertionException();
			}

			if (result[0].Value != 21)
			{
				throw new TestAssertionException();
			}

			if (result[1].Value != 22)
			{
				throw new TestAssertionException();
			}

			if (result[2].Value != 23)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Info_Test ()
		{
			TreeNode<int> root = new TreeNode<int>();
			TreeNode<int> n11 = new TreeNode<int>(11);
			TreeNode<int> n12 = new TreeNode<int>(12);
			TreeNode<int> n13 = new TreeNode<int>(13);
			TreeNode<int> n21 = new TreeNode<int>(21);
			TreeNode<int> n22 = new TreeNode<int>(22);
			TreeNode<int> n23 = new TreeNode<int>(23);

			root.Children.Add(n11);
			root.Children.Add(n12);
			root.Children.Add(n13);

			n12.Children.Add(n21);
			n12.Children.Add(n22);
			n12.Children.Add(n23);

			if (root.GetLevel() != 0)
			{
				throw new TestAssertionException();
			}

			if (n12.GetLevel() != 1)
			{
				throw new TestAssertionException();
			}

			if (n22.GetLevel() != 2)
			{
				throw new TestAssertionException();
			}

			if (root.GetRoot() != root)
			{
				throw new TestAssertionException();
			}

			if (n12.GetRoot() != root)
			{
				throw new TestAssertionException();
			}

			if (n22.GetRoot().Value != 0)
			{
				throw new TestAssertionException();
			}

			if (root.IsChild())
			{
				throw new TestAssertionException();
			}

			if (!n12.IsChild())
			{
				throw new TestAssertionException();
			}

			if (!n22.IsChild())
			{
				throw new TestAssertionException();
			}

			if (root.IsLeaf())
			{
				throw new TestAssertionException();
			}

			if (n12.IsLeaf())
			{
				throw new TestAssertionException();
			}

			if (!n22.IsLeaf())
			{
				throw new TestAssertionException();
			}

			if (!root.IsParent())
			{
				throw new TestAssertionException();
			}

			if (!n12.IsParent())
			{
				throw new TestAssertionException();
			}

			if (n22.IsParent())
			{
				throw new TestAssertionException();
			}

			if (!root.IsRoot())
			{
				throw new TestAssertionException();
			}

			if (n12.IsRoot())
			{
				throw new TestAssertionException();
			}

			if (n22.IsRoot())
			{
				throw new TestAssertionException();
			}

			if (root.IsSibling())
			{
				throw new TestAssertionException();
			}

			if (!n12.IsSibling())
			{
				throw new TestAssertionException();
			}

			if (!n22.IsSibling())
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void RemoveChild_Test ()
		{
			TreeNode<int> root = new TreeNode<int>();
			TreeNode<int> n11 = new TreeNode<int>(11);
			TreeNode<int> n12 = new TreeNode<int>(12);
			TreeNode<int> n13 = new TreeNode<int>(13);
			TreeNode<int> n21 = new TreeNode<int>(21);
			TreeNode<int> n22 = new TreeNode<int>(22);
			TreeNode<int> n23 = new TreeNode<int>(23);

			root.Children.Add(n11);
			root.Children.Add(n12);
			root.Children.Add(n13);

			n12.Children.Add(n21);
			n12.Children.Add(n22);
			n12.Children.Add(n23);

			if (root.RemoveChild(n22))
			{
				throw new TestAssertionException();
			}

			if (!n12.RemoveChild(n21))
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void RemoveChildRecursive_Test ()
		{
			TreeNode<int> root = new TreeNode<int>();
			TreeNode<int> n11 = new TreeNode<int>(11);
			TreeNode<int> n12 = new TreeNode<int>(12);
			TreeNode<int> n13 = new TreeNode<int>(13);
			TreeNode<int> n21 = new TreeNode<int>(21);
			TreeNode<int> n22 = new TreeNode<int>(22);
			TreeNode<int> n23 = new TreeNode<int>(23);

			root.Children.Add(n11);
			root.Children.Add(n12);
			root.Children.Add(n13);

			n12.Children.Add(n21);
			n12.Children.Add(n22);
			n12.Children.Add(n23);

			if (n11.RemoveChildRecursive(n22))
			{
				throw new TestAssertionException();
			}

			if (!n12.RemoveChildRecursive(n21))
			{
				throw new TestAssertionException();
			}

			if (!root.RemoveChildRecursive(n22))
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void RemoveChildren_Test ()
		{
			TreeNode<int> root = new TreeNode<int>();
			TreeNode<int> n11 = new TreeNode<int>(11);
			TreeNode<int> n12 = new TreeNode<int>(12);
			TreeNode<int> n13 = new TreeNode<int>(13);
			TreeNode<int> n21 = new TreeNode<int>(21);
			TreeNode<int> n22 = new TreeNode<int>(22);
			TreeNode<int> n23 = new TreeNode<int>(23);

			root.Children.Add(n11);
			root.Children.Add(n12);
			root.Children.Add(n13);

			n12.Children.Add(n21);
			n12.Children.Add(n22);
			n12.Children.Add(n23);

			if (root.RemoveChildren(new[]
			                        {
				                        n21, n22, n23
			                        }) != 0)
			{
				throw new TestAssertionException();
			}

			if (n12.RemoveChildren(new[]
			                       {
				                       n21, n22, root
			                       }) != 2)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void RemoveChildrenRecursive_Test ()
		{
			TreeNode<int> root = new TreeNode<int>();
			TreeNode<int> n11 = new TreeNode<int>(11);
			TreeNode<int> n12 = new TreeNode<int>(12);
			TreeNode<int> n13 = new TreeNode<int>(13);
			TreeNode<int> n21 = new TreeNode<int>(21);
			TreeNode<int> n22 = new TreeNode<int>(22);
			TreeNode<int> n23 = new TreeNode<int>(23);

			root.Children.Add(n11);
			root.Children.Add(n12);
			root.Children.Add(n13);

			n12.Children.Add(n21);
			n12.Children.Add(n22);
			n12.Children.Add(n23);

			if (n11.RemoveChildrenRecursive(new[]
			                                {
				                                n21, n22, n23
			                                }) != 0)
			{
				throw new TestAssertionException();
			}

			if (n12.RemoveChildrenRecursive(new[]
			                                {
				                                n21, n22, root
			                                }) != 2)
			{
				throw new TestAssertionException();
			}

			if (root.RemoveChildrenRecursive(new[]
			                                 {
				                                 n23, n11, root
			                                 }) != 2)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void RemoveChildrenWhere_Test ()
		{
			TreeNode<int> root = new TreeNode<int>();
			TreeNode<int> n11 = new TreeNode<int>(11);
			TreeNode<int> n12 = new TreeNode<int>(12);
			TreeNode<int> n13 = new TreeNode<int>(13);
			TreeNode<int> n21 = new TreeNode<int>(21);
			TreeNode<int> n22 = new TreeNode<int>(22);
			TreeNode<int> n23 = new TreeNode<int>(23);

			root.Children.Add(n11);
			root.Children.Add(n12);
			root.Children.Add(n13);

			n12.Children.Add(n21);
			n12.Children.Add(n22);
			n12.Children.Add(n23);

			List<TreeNode<int>> result = root.RemoveChildrenWhere(x => x.Value > 20);

			if (result.Count != 0)
			{
				throw new TestAssertionException();
			}

			result = n12.RemoveChildrenWhere(x => x.Value > 21);

			if (result.Count != 2)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void RemoveChildrenWhereRecursive_Test ()
		{
			TreeNode<int> root = new TreeNode<int>();
			TreeNode<int> n11 = new TreeNode<int>(11);
			TreeNode<int> n12 = new TreeNode<int>(12);
			TreeNode<int> n13 = new TreeNode<int>(13);
			TreeNode<int> n21 = new TreeNode<int>(21);
			TreeNode<int> n22 = new TreeNode<int>(22);
			TreeNode<int> n23 = new TreeNode<int>(23);

			root.Children.Add(n11);
			root.Children.Add(n12);
			root.Children.Add(n13);

			n12.Children.Add(n21);
			n12.Children.Add(n22);
			n12.Children.Add(n23);

			List<TreeNode<int>> result = n22.RemoveChildrenWhereRecursive(x => x.Value > 20);

			if (result.Count != 0)
			{
				throw new TestAssertionException();
			}

			result = root.RemoveChildrenWhereRecursive(x => x.Value > 12);

			if (result.Count != 4)
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
