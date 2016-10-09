using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Collections.ObjectModel;




namespace RI.Test.Framework.Collections.ObjectModel
{
	[TestClass]
	public sealed class Test_IPoolExtensions
	{
		#region Instance Methods

		[TestMethod]
		public void ReduceEnsure_Test ()
		{
			Pool<int> pool = new Pool<int>();
			IPool<int> test = pool;

			if (test.Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test.ReduceEnsure(10) != 10)
			{
				throw new TestAssertionException();
			}

			if (test.Count != 10)
			{
				throw new TestAssertionException();
			}

			if (test.ReduceEnsure(5) != -5)
			{
				throw new TestAssertionException();
			}

			if (test.Count != 5)
			{
				throw new TestAssertionException();
			}

			if (test.ReduceEnsure(12) != 7)
			{
				throw new TestAssertionException();
			}

			if (test.Count != 12)
			{
				throw new TestAssertionException();
			}

			if (test.ReduceEnsure(0) != -12)
			{
				throw new TestAssertionException();
			}

			if (test.Count != 0)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ReturnRange_Test ()
		{
			Pool<int> pool = new Pool<int>();
			IPool<int> test = pool;

			if (test.ReturnRange(new[]
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

			if (test.Take() != 3)
			{
				throw new TestAssertionException();
			}

			if (test.Take() != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Take() != 1)
			{
				throw new TestAssertionException();
			}

			if (test.ReturnRange((IList<int>)new[]
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

			if (test.Take() != 3)
			{
				throw new TestAssertionException();
			}

			if (test.Take() != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Take() != 1)
			{
				throw new TestAssertionException();
			}

			if (test.ReturnRange((IEnumerable<int>)new[]
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

			if (test.Take() != 3)
			{
				throw new TestAssertionException();
			}

			if (test.Take() != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Take() != 1)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void TakeRange_Test ()
		{
			Pool<int> pool = new Pool<int>();
			IPool<int> test = pool;

			test.Return(1);
			test.Return(2);
			test.Return(3);

			int[] result = test.TakeRange(4);

			if (test.Count != 0)
			{
				throw new TestAssertionException();
			}

			if (result.Length != 4)
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

			if (result[3] != 0)
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
