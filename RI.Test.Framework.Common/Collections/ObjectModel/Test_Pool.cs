using System.Collections.Generic;

using RI.Framework.Collections;
using RI.Framework.Collections.ObjectModel;

#if PLATFORM_NET
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
#if PLATFORM_UNITY
using RI.Test.Framework;
#endif




namespace RI.Test.Framework.Collections.ObjectModel
{
	[TestClass]
	public sealed class Test_Pool
	{
		#region Instance Methods

		[TestMethod]
		public void Test ()
		{
			Pool<int> pool = new Pool<int>();

			if (pool.Count != 0)
			{
				throw new TestAssertionException();
			}

			if (pool.Take() != 0)
			{
				throw new TestAssertionException();
			}

			if (pool.Count != 0)
			{
				throw new TestAssertionException();
			}

			pool.Return(0);

			if (pool.Count != 1)
			{
				throw new TestAssertionException();
			}

			if (pool.Take() != 0)
			{
				throw new TestAssertionException();
			}

			pool.Return(1);
			pool.Return(2);

			if (pool.Count != 2)
			{
				throw new TestAssertionException();
			}

			if (pool.Contains(0))
			{
				throw new TestAssertionException();
			}

			if (!pool.Contains(1))
			{
				throw new TestAssertionException();
			}

			if (!pool.Contains(2))
			{
				throw new TestAssertionException();
			}

			if (pool.Take() != 2)
			{
				throw new TestAssertionException();
			}

			if (pool.Take() != 1)
			{
				throw new TestAssertionException();
			}

			if (pool.Take() != 0)
			{
				throw new TestAssertionException();
			}

			pool.Return(3);
			pool.Return(4);

			if (pool.Count != 2)
			{
				throw new TestAssertionException();
			}

			pool.Clear();

			if (pool.Count != 0)
			{
				throw new TestAssertionException();
			}

			if (!pool.ReturnSafe(5))
			{
				throw new TestAssertionException();
			}

			if (!pool.ReturnSafe(6))
			{
				throw new TestAssertionException();
			}

			List<int> result = pool.FreeItems.ToList();

			if (pool.Count != 2)
			{
				throw new TestAssertionException();
			}

			if (result.Count != 2)
			{
				throw new TestAssertionException();
			}

			if (result[0] != 5)
			{
				throw new TestAssertionException();
			}

			if (result[1] != 6)
			{
				throw new TestAssertionException();
			}

			if (pool.ReturnSafe(5))
			{
				throw new TestAssertionException();
			}

			if (pool.Ensure(10) != 8)
			{
				throw new TestAssertionException();
			}

			if (pool.Count != 10)
			{
				throw new TestAssertionException();
			}

			if (pool.Reduce(6) != 4)
			{
				throw new TestAssertionException();
			}

			if (pool.Count != 6)
			{
				throw new TestAssertionException();
			}

			pool.Clear();

			int indicator = 0;

			pool.Created += x => indicator = 10;
			pool.Taking += x => indicator++;
			pool.Returned += x => indicator++;
			pool.Removed += x => indicator++;

			pool.Take();
			if (indicator != 11)
			{
				throw new TestAssertionException();
			}

			pool.Return(0);
			if (indicator != 12)
			{
				throw new TestAssertionException();
			}

			pool.Clear();
			if (indicator != 13)
			{
				throw new TestAssertionException();
			}

			pool = new Pool<int>(10);

			if (pool.Count != 0)
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
