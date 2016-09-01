using RI.Framework.Utilities.Comparison;

#if PLATFORM_NET
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
#if PLATFORM_UNITY
#endif




namespace RI.Test.Framework.Utilities.Comparison
{
	[TestClass]
	public sealed class Test_EqualityComparison
	{
		#region Instance Methods

		[TestMethod]
		public void Test ()
		{
			EqualityComparison<int> comparer = new EqualityComparison<int>((x, y) => ( x % 5 ) == ( y % 5 ), obj => obj % 5);

			if (!comparer.Equals(0, 0))
			{
				throw new TestAssertionException();
			}

			if (!comparer.Equals(1, 1))
			{
				throw new TestAssertionException();
			}

			if (!comparer.Equals(0, 5))
			{
				throw new TestAssertionException();
			}

			if (!comparer.Equals(2, 12))
			{
				throw new TestAssertionException();
			}

			if (comparer.Equals(2, 3))
			{
				throw new TestAssertionException();
			}

			if (comparer.GetHashCode(0) != 0)
			{
				throw new TestAssertionException();
			}

			if (comparer.GetHashCode(1) != 1)
			{
				throw new TestAssertionException();
			}

			if (comparer.GetHashCode(4) != 4)
			{
				throw new TestAssertionException();
			}

			if (comparer.GetHashCode(5) != 0)
			{
				throw new TestAssertionException();
			}

			comparer = new EqualityComparison<int>((x, y) => ( x % 5 ) == ( y % 5 ));

			if (!comparer.Comparer(0, 0))
			{
				throw new TestAssertionException();
			}

			if (!comparer.Comparer(1, 1))
			{
				throw new TestAssertionException();
			}

			if (!comparer.Comparer(0, 5))
			{
				throw new TestAssertionException();
			}

			if (!comparer.Comparer(2, 12))
			{
				throw new TestAssertionException();
			}

			if (comparer.Comparer(2, 3))
			{
				throw new TestAssertionException();
			}

			if (comparer.Hasher(0) != 0)
			{
				throw new TestAssertionException();
			}

			if (comparer.Hasher(1) != 1)
			{
				throw new TestAssertionException();
			}

			if (comparer.Hasher(4) != 4)
			{
				throw new TestAssertionException();
			}

			if (comparer.Hasher(5) != 5)
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
