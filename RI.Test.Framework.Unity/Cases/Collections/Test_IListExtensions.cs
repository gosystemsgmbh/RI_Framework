using System;
using System.Collections.Generic;

using RI.Framework.Collections;
using RI.Framework.Collections.Comparison;
using RI.Framework.Utilities.Comparison;




namespace RI.Test.Framework.Cases.Collections
{
	public sealed class Test_IListExtensions : TestModule
	{
		#region Instance Methods

		[TestMethod]
		public void InsertRange_Test ()
		{
			List<int> list = new List<int>();
			IList<int> test = list.AsList();

			if (test.InsertRange(0, new[]
			                     {
				                     1, 3
			                     }) != 2)
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

			if (list[1] != 3)
			{
				throw new TestAssertionException();
			}

			if (test.InsertRange(1, new[]
			                     {
				                     2, 2, 2
			                     }) != 3)
			{
				throw new TestAssertionException();
			}

			if (list.Count != 5)
			{
				throw new TestAssertionException();
			}

			if (list[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (list[2] != 2)
			{
				throw new TestAssertionException();
			}

			if (list[4] != 3)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void RemoveAtRange_Test ()
		{
			List<int> list = new List<int>();
			IList<int> test = list.AsList();

			if (test.RemoveAtRange(0, 0) != 0)
			{
				throw new TestAssertionException();
			}

			list.Add(1);
			list.Add(2);
			list.Add(3);
			list.Add(4);
			list.Add(5);

			if (test.RemoveAtRange(2, 0) != 0)
			{
				throw new TestAssertionException();
			}

			if (list.Count != 5)
			{
				throw new TestAssertionException();
			}

			if (list[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (list[4] != 5)
			{
				throw new TestAssertionException();
			}

			if (test.RemoveAtRange(2, 2) != 0)
			{
				throw new TestAssertionException();
			}

			if (list.Count != 3)
			{
				throw new TestAssertionException();
			}

			if (list[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (list[2] != 5)
			{
				throw new TestAssertionException();
			}

			list.Clear();

			list.Add(1);
			list.Add(2);
			list.Add(3);
			list.Add(4);
			list.Add(5);

			if (test.RemoveAtRange(new[]
			                       {
				                       1, 3, 3, 1
			                       }) != 2)
			{
				throw new TestAssertionException();
			}

			if (list.Count != 3)
			{
				throw new TestAssertionException();
			}

			if (list[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (list[1] != 3)
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
			IList<int> test = list.AsList();

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
			test = list.AsList();

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

		[TestMethod]
		public void Reverse_Test ()
		{
			List<int> list = new List<int>();
			IList<int> test = list.AsList();

			if (test.Reverse() != 0)
			{
				throw new TestAssertionException();
			}

			list.Add(10);
			list.Add(5);
			list.Add(20);
			list.Add(0);

			if (test.Reverse() != 4)
			{
				throw new TestAssertionException();
			}

			if (list[0] != 0)
			{
				throw new TestAssertionException();
			}

			if (list[1] != 20)
			{
				throw new TestAssertionException();
			}

			if (list[2] != 5)
			{
				throw new TestAssertionException();
			}

			if (list[3] != 10)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Shuffle_Test ()
		{
			Random random = new Random();
			List<int> list1 = new List<int>();
			IList<int> test = list1.AsList();

			if (test.Shuffle(random) != 0)
			{
				throw new TestAssertionException();
			}

			if (test.Shuffle(random, 0) != 0)
			{
				throw new TestAssertionException();
			}

			if (test.Shuffle(random, 1) != 0)
			{
				throw new TestAssertionException();
			}

			list1.Add(1);
			list1.Add(2);
			list1.Add(3);
			list1.Add(4);
			list1.Add(5);

			List<int> list2 = new List<int>(list1);

			if (test.Shuffle(random, 0) != 5)
			{
				throw new TestAssertionException();
			}

			if (!list1.SequenceEqual(list2, CollectionComparerFlags.None))
			{
				throw new TestAssertionException();
			}

			if (test.Shuffle(random, 1) != 5)
			{
				throw new TestAssertionException();
			}

			if (list1.SequenceEqual(list2, CollectionComparerFlags.None))
			{
				throw new TestAssertionException();
			}

			if (test.Shuffle(random) != 5)
			{
				throw new TestAssertionException();
			}

			if (list1.SequenceEqual(list2, CollectionComparerFlags.None))
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Sort_Test ()
		{
			//-----------
			// Empty list
			//-----------

			//Preparation
			List<int> list = new List<int>();
			IList<int> test = list.AsList();

			//Default comparer

			if (test.Sort(true) != 0)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test.Sort(true, new OrderComparison<int>((x, y) => x.CompareTo(y))) != 0)
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (test.Sort(true, (x, y) => x.CompareTo(y)) != 0)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list.Add(10);
			list.Add(5);
			list.Add(20);
			list.Add(0);

			//Default comparer

			if (test.Sort(true) != 4)
			{
				throw new TestAssertionException();
			}

			if (list[0] != 0)
			{
				throw new TestAssertionException();
			}

			if (list[1] != 5)
			{
				throw new TestAssertionException();
			}

			if (list[2] != 10)
			{
				throw new TestAssertionException();
			}

			if (list[3] != 20)
			{
				throw new TestAssertionException();
			}

			if (test.Sort(false) != 4)
			{
				throw new TestAssertionException();
			}

			if (list[0] != 20)
			{
				throw new TestAssertionException();
			}

			if (list[1] != 10)
			{
				throw new TestAssertionException();
			}

			if (list[2] != 5)
			{
				throw new TestAssertionException();
			}

			if (list[3] != 0)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test.Sort(true, new OrderComparison<int>((x, y) => x.CompareTo(y))) != 4)
			{
				throw new TestAssertionException();
			}

			if (list[0] != 0)
			{
				throw new TestAssertionException();
			}

			if (list[1] != 5)
			{
				throw new TestAssertionException();
			}

			if (list[2] != 10)
			{
				throw new TestAssertionException();
			}

			if (list[3] != 20)
			{
				throw new TestAssertionException();
			}

			if (test.Sort(false, new OrderComparison<int>((x, y) => x.CompareTo(y))) != 4)
			{
				throw new TestAssertionException();
			}

			if (list[0] != 20)
			{
				throw new TestAssertionException();
			}

			if (list[1] != 10)
			{
				throw new TestAssertionException();
			}

			if (list[2] != 5)
			{
				throw new TestAssertionException();
			}

			if (list[3] != 0)
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (test.Sort(true, (x, y) => x.CompareTo(y)) != 4)
			{
				throw new TestAssertionException();
			}

			if (list[0] != 0)
			{
				throw new TestAssertionException();
			}

			if (list[1] != 5)
			{
				throw new TestAssertionException();
			}

			if (list[2] != 10)
			{
				throw new TestAssertionException();
			}

			if (list[3] != 20)
			{
				throw new TestAssertionException();
			}

			if (test.Sort(false, (x, y) => x.CompareTo(y)) != 4)
			{
				throw new TestAssertionException();
			}

			if (list[0] != 20)
			{
				throw new TestAssertionException();
			}

			if (list[1] != 10)
			{
				throw new TestAssertionException();
			}

			if (list[2] != 5)
			{
				throw new TestAssertionException();
			}

			if (list[3] != 0)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void SwapDefault_Test ()
		{
			List<int> list = new List<int>()
			{
				1,
				2,
				3,
				4
			};
			IList<int> test = list.AsList();

			if (list[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (list[2] != 3)
			{
				throw new TestAssertionException();
			}

			test.SwapDefault(0, 2);

			if (list[0] != 3)
			{
				throw new TestAssertionException();
			}

			if (list[2] != 1)
			{
				throw new TestAssertionException();
			}

			test.SwapDefault(0, 2, 100);

			if (list[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (list[2] != 3)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void SwapInPlace_Test ()
		{
			List<int> list = new List<int>()
			{
				1,
				2,
				3,
				4
			};
			IList<int> test = list.AsList();

			if (list[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (list[2] != 3)
			{
				throw new TestAssertionException();
			}

			test.SwapInPlace(0, 2);

			if (list[0] != 3)
			{
				throw new TestAssertionException();
			}

			if (list[2] != 1)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void SwapInsert_Test ()
		{
			List<int> list = new List<int>()
			{
				1,
				2,
				3,
				4
			};
			IList<int> test = list.AsList();

			if (list[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (list[2] != 3)
			{
				throw new TestAssertionException();
			}

			test.SwapInsert(0, 2);

			if (list[0] != 3)
			{
				throw new TestAssertionException();
			}

			if (list[2] != 1)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Transform_Test ()
		{
			List<int> list = new List<int>();
			IList<int> test = list.AsList();

			if (test.Transform((x, y) => x + y) != 0)
			{
				throw new TestAssertionException();
			}

			list.Add(10);
			list.Add(5);
			list.Add(20);
			list.Add(0);

			if (test.Transform(x => x * 10) != 4)
			{
				throw new TestAssertionException();
			}

			if (list[0] != 100)
			{
				throw new TestAssertionException();
			}

			if (list[1] != 50)
			{
				throw new TestAssertionException();
			}

			if (list[2] != 200)
			{
				throw new TestAssertionException();
			}

			if (list[3] != 0)
			{
				throw new TestAssertionException();
			}

			list.Clear();
			list.Add(10);
			list.Add(5);
			list.Add(20);
			list.Add(0);

			if (test.Transform((x, y) => x + y) != 4)
			{
				throw new TestAssertionException();
			}

			if (list[0] != 10)
			{
				throw new TestAssertionException();
			}

			if (list[1] != 6)
			{
				throw new TestAssertionException();
			}

			if (list[2] != 22)
			{
				throw new TestAssertionException();
			}

			if (list[3] != 3)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void TryGetIndex_Test ()
		{
			List<int> list = new List<int>();
			IList<int> test = list.AsList();

			int result = 0;
			if (test.TryGetIndex(0, out result))
			{
				throw new TestAssertionException();
			}

			list.Add(1);
			list.Add(2);
			list.Add(3);
			list.Add(4);

			if (!test.TryGetIndex(2, out result))
			{
				throw new TestAssertionException();
			}

			if (result != 3)
			{
				throw new TestAssertionException();
			}

			if (test.TryGetIndex(4, out result))
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
