using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Collections;




namespace RI.Test.Framework.Cases.Collections
{
	[SuppressMessage ("ReSharper", "InconsistentNaming")]
	[SuppressMessage ("ReSharper", "UnusedMember.Global")]
	public sealed class Test_ICollectionExtensions : TestModule
	{
		#region Instance Methods

		[TestMethod]
		public void AddRange_Test ()
		{
			List<int> list = new List<int>();
			ICollection<int> test = list.AsCollection();

			if (test.AddRange(new[]
			{
				1, 2, 30, 40, 500, 600
			}) != 6)
			{
				throw new TestAssertionException();
			}

			if (list.Count != 6)
			{
				throw new TestAssertionException();
			}

			if (list[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (list[5] != 600)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void RemoveAll_Test ()
		{
			List<int> list = new List<int>
			{
				1,
				2,
				2,
				2,
				5
			};
			ICollection<int> test = list.AsCollection();

			if (test.RemoveAll(2) != 3)
			{
				throw new TestAssertionException();
			}

			if (list.Count != 2)
			{
				throw new TestAssertionException();
			}

			if (list[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (list[1] != 5)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void RemoveAllRange_Test ()
		{
			List<int> list = new List<int>
			{
				1,
				1,
				1,
				2,
				2,
				3,
				4,
				5
			};
			ICollection<int> test = list.AsCollection();

			if (test.RemoveAllRange(new[]
			{
				1, 2, 30, 40, 500, 600
			}) != 2)
			{
				throw new TestAssertionException();
			}

			if (list.Count != 3)
			{
				throw new TestAssertionException();
			}

			if (list[0] != 3)
			{
				throw new TestAssertionException();
			}

			if (list[2] != 5)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void RemoveRange_Test ()
		{
			List<int> list = new List<int>
			{
				1,
				2,
				3,
				4,
				5
			};
			ICollection<int> test = list.AsCollection();

			if (test.RemoveRange(new[]
			{
				1, 2, 30, 40, 500, 600
			}) != 2)
			{
				throw new TestAssertionException();
			}

			if (list.Count != 3)
			{
				throw new TestAssertionException();
			}

			if (list[0] != 3)
			{
				throw new TestAssertionException();
			}

			if (list[2] != 5)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void RemoveWhere_Test ()
		{
			List<int> list = new List<int>
			{
				1,
				1,
				1,
				2,
				2,
				3,
				4,
				5
			};
			ICollection<int> test = list.AsCollection();

			if (test.RemoveWhere(x => x < 3).Count != 5)
			{
				throw new TestAssertionException();
			}

			if (list.Count != 3)
			{
				throw new TestAssertionException();
			}

			if (list[0] != 3)
			{
				throw new TestAssertionException();
			}

			if (list[2] != 5)
			{
				throw new TestAssertionException();
			}

			list = new List<int>
			{
				1,
				1,
				1,
				2,
				2,
				3,
				4,
				5
			};
			test = list.AsCollection();

			if (test.RemoveWhere((x, y) => ( x == 2 ) || ( x == 3 )).Count != 2)
			{
				throw new TestAssertionException();
			}

			if (list.Count != 6)
			{
				throw new TestAssertionException();
			}

			if (list[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (list[5] != 5)
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
