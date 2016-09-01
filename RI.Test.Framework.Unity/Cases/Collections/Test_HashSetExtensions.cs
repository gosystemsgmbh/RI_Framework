using System.Collections.Generic;

using RI.Framework.Collections;




namespace RI.Test.Framework.Cases.Collections
{
	public sealed class Test_HashSetExtensions : TestModule
	{
		#region Instance Methods

		[TestMethod]
		public void AddRange_Test ()
		{
			HashSet<int> set = new HashSet<int>();

			if (set.AddRange(new[]
			                 {
				                 1, 2, 30, 40, 500, 600
			                 }) != 6)
			{
				throw new TestAssertionException();
			}

			if (set.Count != 6)
			{
				throw new TestAssertionException();
			}

			if (set.Contains(3))
			{
				throw new TestAssertionException();
			}

			if (!set.Contains(1))
			{
				throw new TestAssertionException();
			}

			if (set.AddRange(new[]
			                 {
				                 2, 3, 40, 50, 600, 600
			                 }) != 2)
			{
				throw new TestAssertionException();
			}

			if (set.Contains(4))
			{
				throw new TestAssertionException();
			}

			if (!set.Contains(3))
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void RemoveRange_Test ()
		{
			HashSet<int> set = new HashSet<int>
			{
				1,
				2,
				3,
				4,
				5
			};

			if (set.RemoveRange(new[]
			                    {
				                    1, 2, 30, 40, 500, 600
			                    }) != 2)
			{
				throw new TestAssertionException();
			}

			if (set.Count != 3)
			{
				throw new TestAssertionException();
			}

			if (set.Contains(2))
			{
				throw new TestAssertionException();
			}

			if (!set.Contains(3))
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
