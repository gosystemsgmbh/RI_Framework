using System.Diagnostics.CodeAnalysis;

using RI.Framework.Utilities.Comparison;




namespace RI.Test.Framework.Cases.Utilities.Comparison
{
	[SuppressMessage ("ReSharper", "InconsistentNaming")]
	[SuppressMessage ("ReSharper", "UnusedMember.Global")]
	public sealed class Test_OrderComparison : TestModule
	{
		#region Instance Methods

		[TestMethod]
		public void Test ()
		{
			OrderComparison<int> comparer = new OrderComparison<int>((x, y) => ( x % 5 ).CompareTo(y % 5));

			if (comparer.ReverseOrder)
			{
				throw new TestAssertionException();
			}

			if (comparer.Compare(0, 0) != 0)
			{
				throw new TestAssertionException();
			}

			if (comparer.Compare(1, 0) != 1)
			{
				throw new TestAssertionException();
			}

			if (comparer.Compare(0, 1) != -1)
			{
				throw new TestAssertionException();
			}

			if (comparer.Compare(0, 5) != 0)
			{
				throw new TestAssertionException();
			}

			if (comparer.Compare(6, 0) != 1)
			{
				throw new TestAssertionException();
			}

			if (comparer.Compare(0, 11) != -1)
			{
				throw new TestAssertionException();
			}

			comparer = new OrderComparison<int>(false, (x, y) => ( x % 5 ).CompareTo(y % 5));

			if (comparer.ReverseOrder)
			{
				throw new TestAssertionException();
			}

			if (comparer.Comparer(0, 0) != 0)
			{
				throw new TestAssertionException();
			}

			if (comparer.Comparer(1, 0) != 1)
			{
				throw new TestAssertionException();
			}

			if (comparer.Comparer(0, 1) != -1)
			{
				throw new TestAssertionException();
			}

			if (comparer.Comparer(0, 5) != 0)
			{
				throw new TestAssertionException();
			}

			if (comparer.Comparer(6, 0) != 1)
			{
				throw new TestAssertionException();
			}

			if (comparer.Comparer(0, 11) != -1)
			{
				throw new TestAssertionException();
			}

			comparer = new OrderComparison<int>(true, (x, y) => ( x % 5 ).CompareTo(y % 5));

			if (!comparer.ReverseOrder)
			{
				throw new TestAssertionException();
			}

			if (comparer.Compare(0, 0) != 0)
			{
				throw new TestAssertionException();
			}

			if (comparer.Compare(1, 0) != -1)
			{
				throw new TestAssertionException();
			}

			if (comparer.Compare(0, 1) != 1)
			{
				throw new TestAssertionException();
			}

			if (comparer.Compare(0, 5) != 0)
			{
				throw new TestAssertionException();
			}

			if (comparer.Compare(6, 0) != -1)
			{
				throw new TestAssertionException();
			}

			if (comparer.Compare(0, 11) != 1)
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
