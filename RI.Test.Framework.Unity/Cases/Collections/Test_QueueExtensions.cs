using System.Collections.Generic;

using RI.Framework.Collections;




namespace RI.Test.Framework.Cases.Collections
{
	public sealed class Test_QueueExtensions : TestModule
	{
		#region Instance Methods

		[TestMethod]
		public void DequeueAll_Test ()
		{
			Queue<int> test = new Queue<int>();
			test.Enqueue(1);
			test.Enqueue(2);
			test.Enqueue(3);

			List<int> result = test.DequeueAll();

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

			if (test.Count != 0)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void EnqueueRange_Test ()
		{
			Queue<int> test = new Queue<int>();

			if (test.EnqueueRange(new[]
			                      {
				                      1, 2, 3
			                      }) != 3)
			{
				throw new TestAssertionException();
			}

			if (test.Count != 3)
			{
				throw new TestAssertionException();
			}

			if (test.Dequeue() != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Dequeue() != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Dequeue() != 3)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void PeekAll_Test ()
		{
			Queue<int> test = new Queue<int>();
			test.Enqueue(1);
			test.Enqueue(2);
			test.Enqueue(3);

			List<int> result = test.PeekAll();

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

			if (test.Count != 3)
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
