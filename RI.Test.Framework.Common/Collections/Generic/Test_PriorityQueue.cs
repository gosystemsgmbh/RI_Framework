using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Collections.Generic;
using RI.Framework.Collections.Linq;




namespace RI.Test.Framework.Collections.Generic
{
	[TestClass]
	public sealed class Test_PriorityQueue
	{
		[TestMethod]
		public void Test ()
		{
			PriorityQueue<int> test = new PriorityQueue<int>();

			if (test.Count != 0)
			{
				throw new TestAssertionException();
			}

			test.Enqueue(1, 0);

			if (test.Count != 1)
			{
				throw new TestAssertionException();
			}

			test.Enqueue(2, 0);

			if (test.Count != 2)
			{
				throw new TestAssertionException();
			}

			test.Enqueue(3, 1);

			if (test.Count != 3)
			{
				throw new TestAssertionException();
			}

			test.Enqueue(4, 2);

			if (test.Count != 4)
			{
				throw new TestAssertionException();
			}

			int priority;

			if (test.Dequeue(out priority) != 4)
			{
				throw new TestAssertionException();
			}
			if (priority != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Dequeue(out priority) != 3)
			{
				throw new TestAssertionException();
			}
			if (priority != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Dequeue(out priority) != 1)
			{
				throw new TestAssertionException();
			}
			if (priority != 0)
			{
				throw new TestAssertionException();
			}

			if (test.Dequeue(out priority) != 2)
			{
				throw new TestAssertionException();
			}
			if (priority != 0)
			{
				throw new TestAssertionException();
			}

			test.Enqueue(1, 0);
			if (test.Peek(out priority) != 1)
			{
				throw new TestAssertionException();
			}

			test.Enqueue(2, 0);
			if (test.Peek(out priority) != 1)
			{
				throw new TestAssertionException();
			}

			test.Enqueue(3, 1);
			if (test.Peek(out priority) != 3)
			{
				throw new TestAssertionException();
			}

			test.Enqueue(4, 2);
			if (test.Peek(out priority) != 4)
			{
				throw new TestAssertionException();
			}

			if (test.Count != 4)
			{
				throw new TestAssertionException();
			}

			int[] array = new int[4];

			test.CopyTo(array, 0);
			if (array[0] != 4)
			{
				throw new TestAssertionException();
			}
			if (array[3] != 2)
			{
				throw new TestAssertionException();
			}

			((ICollection)test).CopyTo(array, 0);
			if (array[0] != 4)
			{
				throw new TestAssertionException();
			}
			if (array[3] != 2)
			{
				throw new TestAssertionException();
			}

			array = ((IEnumerable)test).ToList().Cast<int>().ToArray();
			if (array[0] != 4)
			{
				throw new TestAssertionException();
			}
			if (array[3] != 2)
			{
				throw new TestAssertionException();
			}

			test.Clear();

			if (test.Count != 0)
			{
				throw new TestAssertionException();
			}

			test.Enqueue(1, 3);
			if (test.Peek(out priority) != 1)
			{
				throw new TestAssertionException();
			}

			test.Enqueue(2, 0);
			if (test.Peek(out priority) != 1)
			{
				throw new TestAssertionException();
			}

			test.Enqueue(3, 2);
			if (test.Peek(out priority) != 1)
			{
				throw new TestAssertionException();
			}

			test.Enqueue(4, 1);
			if (test.Peek(out priority) != 1)
			{
				throw new TestAssertionException();
			}

			array = new int[4];

			test.CopyTo(array, 0);
			if (array[0] != 1)
			{
				throw new TestAssertionException();
			}
			if (array[1] != 3)
			{
				throw new TestAssertionException();
			}
			if (array[2] != 4)
			{
				throw new TestAssertionException();
			}
			if (array[3] != 2)
			{
				throw new TestAssertionException();
			}
		}
	}
}
