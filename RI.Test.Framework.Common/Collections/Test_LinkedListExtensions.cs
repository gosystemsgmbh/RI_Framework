using System.Collections.Generic;

using RI.Framework.Collections;

#if PLATFORM_NET
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
#if PLATFORM_UNITY
using RI.Test.Framework;
#endif




namespace RI.Test.Framework.Collections
{
	[TestClass]
	public sealed class Test_LinkedListExtensions
	{
		#region Instance Methods

		[TestMethod]
		public void AsItemsBackward_Test ()
		{
			LinkedList<int> list = new LinkedList<int>();

			list.AddLast(1);
			list.AddLast(2);
			list.AddLast(3);

			List<int> result = list.AsItemsBackward().ToList();

			if (result.Count != 3)
			{
				throw new TestAssertionException();
			}

			if (result[0] != 3)
			{
				throw new TestAssertionException();
			}

			if (result[1] != 2)
			{
				throw new TestAssertionException();
			}

			if (result[2] != 1)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void AsItemsForward_Test ()
		{
			LinkedList<int> list = new LinkedList<int>();

			list.AddLast(1);
			list.AddLast(2);
			list.AddLast(3);

			List<int> result = list.AsItemsForward().ToList();

			if (result.Count != 3)
			{
				throw new TestAssertionException();
			}

			if (result[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (result[1] != 2)
			{
				throw new TestAssertionException();
			}

			if (result[2] != 3)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void AsNodesBackward_Test ()
		{
			LinkedList<int> list = new LinkedList<int>();

			list.AddLast(1);
			list.AddLast(2);
			list.AddLast(3);

			List<LinkedListNode<int>> result = list.AsNodesBackward().ToList();

			if (result.Count != 3)
			{
				throw new TestAssertionException();
			}

			if (result[0].Value != 3)
			{
				throw new TestAssertionException();
			}

			if (result[1].Value != 2)
			{
				throw new TestAssertionException();
			}

			if (result[2].Value != 1)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void AsNodesForward_Test ()
		{
			LinkedList<int> list = new LinkedList<int>();

			list.AddLast(1);
			list.AddLast(2);
			list.AddLast(3);

			List<LinkedListNode<int>> result = list.AsNodesForward().ToList();

			if (result.Count != 3)
			{
				throw new TestAssertionException();
			}

			if (result[0].Value != 1)
			{
				throw new TestAssertionException();
			}

			if (result[1].Value != 2)
			{
				throw new TestAssertionException();
			}

			if (result[2].Value != 3)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void FindWhere_Test ()
		{
			LinkedList<int> list = new LinkedList<int>();

			list.AddLast(1);
			list.AddLast(2);
			list.AddLast(3);

			List<LinkedListNode<int>> result = list.FindWhere(x => x.Value > 1);

			if (result.Count != 2)
			{
				throw new TestAssertionException();
			}

			if (result[0].Value != 2)
			{
				throw new TestAssertionException();
			}

			if (result[1].Value != 3)
			{
				throw new TestAssertionException();
			}

			result = list.FindWhere((x, y) => x > 1);

			if (result.Count != 1)
			{
				throw new TestAssertionException();
			}

			if (result[0].Value != 3)
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
