using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Utilities;




namespace RI.Test.Framework.Utilities
{
	[TestClass]
	public sealed class Test_StringComparerEx
	{
		#region Instance Methods

		[TestMethod]
		public void Test ()
		{
			this.TestComparerEqual(StringComparerEx.TrimmedCurrentCulture, "ABC", "ABC");
			this.TestComparerEqual(StringComparerEx.TrimmedCurrentCulture, " ABC ", "ABC");
			this.TestComparerEqual(StringComparerEx.TrimmedCurrentCulture, " ABC ", " ABC ");
			this.TestComparerDifferent(StringComparerEx.TrimmedCurrentCulture, "ABC", "abc");
			this.TestComparerDifferent(StringComparerEx.TrimmedCurrentCulture, " ABC ", "abc");
			this.TestComparerDifferent(StringComparerEx.TrimmedCurrentCulture, " ABC ", " abc ");

			this.TestComparerEqual(StringComparerEx.TrimmedCurrentCultureIgnoreCase, "ABC", "ABC");
			this.TestComparerEqual(StringComparerEx.TrimmedCurrentCultureIgnoreCase, " ABC ", "ABC");
			this.TestComparerEqual(StringComparerEx.TrimmedCurrentCultureIgnoreCase, " ABC ", " ABC ");
			this.TestComparerEqual(StringComparerEx.TrimmedCurrentCultureIgnoreCase, "ABC", "abc");
			this.TestComparerEqual(StringComparerEx.TrimmedCurrentCultureIgnoreCase, " ABC ", "abc");
			this.TestComparerEqual(StringComparerEx.TrimmedCurrentCultureIgnoreCase, " ABC ", " abc ");

			this.TestComparerEqual(StringComparerEx.TrimmedInvariantCulture, "ABC", "ABC");
			this.TestComparerEqual(StringComparerEx.TrimmedInvariantCulture, " ABC ", "ABC");
			this.TestComparerEqual(StringComparerEx.TrimmedInvariantCulture, " ABC ", " ABC ");
			this.TestComparerDifferent(StringComparerEx.TrimmedInvariantCulture, "ABC", "abc");
			this.TestComparerDifferent(StringComparerEx.TrimmedInvariantCulture, " ABC ", "abc");
			this.TestComparerDifferent(StringComparerEx.TrimmedInvariantCulture, " ABC ", " abc ");

			this.TestComparerEqual(StringComparerEx.TrimmedInvariantCultureIgnoreCase, "ABC", "ABC");
			this.TestComparerEqual(StringComparerEx.TrimmedInvariantCultureIgnoreCase, " ABC ", "ABC");
			this.TestComparerEqual(StringComparerEx.TrimmedInvariantCultureIgnoreCase, " ABC ", " ABC ");
			this.TestComparerEqual(StringComparerEx.TrimmedInvariantCultureIgnoreCase, "ABC", "abc");
			this.TestComparerEqual(StringComparerEx.TrimmedInvariantCultureIgnoreCase, " ABC ", "abc");
			this.TestComparerEqual(StringComparerEx.TrimmedInvariantCultureIgnoreCase, " ABC ", " abc ");

			this.TestComparerEqual(StringComparerEx.TrimmedOrdinal, "ABC", "ABC");
			this.TestComparerEqual(StringComparerEx.TrimmedOrdinal, " ABC ", "ABC");
			this.TestComparerEqual(StringComparerEx.TrimmedOrdinal, " ABC ", " ABC ");
			this.TestComparerDifferent(StringComparerEx.TrimmedOrdinal, "ABC", "abc");
			this.TestComparerDifferent(StringComparerEx.TrimmedOrdinal, " ABC ", "abc");
			this.TestComparerDifferent(StringComparerEx.TrimmedOrdinal, " ABC ", " abc ");

			this.TestComparerEqual(StringComparerEx.TrimmedOrdinalIgnoreCase, "ABC", "ABC");
			this.TestComparerEqual(StringComparerEx.TrimmedOrdinalIgnoreCase, " ABC ", "ABC");
			this.TestComparerEqual(StringComparerEx.TrimmedOrdinalIgnoreCase, " ABC ", " ABC ");
			this.TestComparerEqual(StringComparerEx.TrimmedOrdinalIgnoreCase, "ABC", "abc");
			this.TestComparerEqual(StringComparerEx.TrimmedOrdinalIgnoreCase, " ABC ", "abc");
			this.TestComparerEqual(StringComparerEx.TrimmedOrdinalIgnoreCase, " ABC ", " abc ");
		}

		public void TestComparerDifferent (StringComparerEx comparer, string x, string y)
		{
			if (comparer.Equals(x, y))
			{
				throw new TestAssertionException(comparer.GetType().Name + ": [" + x + "] == [" + y + "]");
			}

			if (comparer.Compare(x, y) == 0)
			{
				throw new TestAssertionException(comparer.GetType().Name + ": [" + x + "] <> [" + y + "] == 0");
			}
		}

		public void TestComparerEqual (StringComparerEx comparer, string x, string y)
		{
			if (!comparer.Equals(x, y))
			{
				throw new TestAssertionException(comparer.GetType().Name + ": [" + x + "] != [" + y + "]");
			}

			if (comparer.Compare(x, y) != 0)
			{
				throw new TestAssertionException(comparer.GetType().Name + ": [" + x + "] <> [" + y + "] != 0");
			}
		}

		#endregion
	}
}
