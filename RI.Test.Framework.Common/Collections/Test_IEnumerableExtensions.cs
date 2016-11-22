using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Collections;
using RI.Framework.Collections.Comparison;
using RI.Framework.Collections.Linq;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Comparison;
using RI.Test.Framework.Mocks;




namespace RI.Test.Framework.Collections
{
	[TestClass]
	public sealed class Test_IEnumerableExtensions
	{
		#region Instance Methods

		[TestMethod]
		public void All_Test ()
		{
			//---------------
			// Non-empty list
			//---------------

			//Preparation
			List<VENP> list = new List<VENP>
			{
				1,
				2,
				3
			};
			IEnumerable<VENP> test = list.AsEnumerable();

			//Without index

			if (!test.All(x => x.Number > 0))
			{
				throw new TestAssertionException();
			}

			if (test.All(x => x.Number > 2))
			{
				throw new TestAssertionException();
			}

			if (test.All(x => x.Number > 3))
			{
				throw new TestAssertionException();
			}

			//With index

			if (!test.All((x, y) => y.Number > 0))
			{
				throw new TestAssertionException();
			}

			if (test.All((x, y) => y.Number > 2))
			{
				throw new TestAssertionException();
			}

			if (test.All((x, y) => y.Number > 3))
			{
				throw new TestAssertionException();
			}

			//With item

			if (test.All(2))
			{
				throw new TestAssertionException();
			}

			if (test.All(2, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (test.All(2, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			//-----------
			// Empty list
			//-----------

			//Preparation
			list.Clear();

			//Without index

			if (test.All(x => x.Number > 0))
			{
				throw new TestAssertionException();
			}

			//With index

			if (test.All((x, y) => y.Number > 0))
			{
				throw new TestAssertionException();
			}

			//With item

			if (test.All(0))
			{
				throw new TestAssertionException();
			}

			if (test.All(0, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (test.All(0, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			//-----------------------------
			// List with all the same items
			//-----------------------------

			//Preparation
			list.Add(5);
			list.Add(5);
			list.Add(5);

			//Without index

			if (test.All(x => x.Number == 0))
			{
				throw new TestAssertionException();
			}

			if (!test.All(x => x.Number == 5))
			{
				throw new TestAssertionException();
			}

			//With index

			if (test.All((x, y) => y.Number == 0))
			{
				throw new TestAssertionException();
			}

			if (!test.All((x, y) => y.Number == 5))
			{
				throw new TestAssertionException();
			}

			//With item

			if (!test.All(5))
			{
				throw new TestAssertionException();
			}

			if (!test.All(5, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (!test.All(5, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Any_Test ()
		{
			//---------------
			// Non-empty list
			//---------------

			//Preparation
			List<VENP> list = new List<VENP>
			{
				1,
				2,
				3
			};
			IEnumerable<VENP> test = list.AsEnumerable();

			//Unconditional

			if (!test.Any())
			{
				throw new TestAssertionException();
			}

			//Without index

			if (!test.Any(x => x.Number > 0))
			{
				throw new TestAssertionException();
			}

			if (test.Any(x => x.Number > 3))
			{
				throw new TestAssertionException();
			}

			//With index

			if (!test.Any((x, y) => y.Number > 0))
			{
				throw new TestAssertionException();
			}

			if (test.Any((x, y) => y.Number > 3))
			{
				throw new TestAssertionException();
			}

			//By index

			if (!test.Any(2))
			{
				throw new TestAssertionException();
			}

			if (test.Any(3))
			{
				throw new TestAssertionException();
			}

			//-----------
			// Empty list
			//-----------

			//Preparation
			list.Clear();

			//Unconditional

			if (test.Any())
			{
				throw new TestAssertionException();
			}

			//Without index

			if (test.Any(x => x.Number > 0))
			{
				throw new TestAssertionException();
			}

			//With index

			if (test.Any((x, y) => y.Number > 0))
			{
				throw new TestAssertionException();
			}

			//By index

			if (test.Any(0))
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Cast_Test ()
		{
			//-----------
			// Empty list
			//-----------

			//Preparation
			List<object> list = new List<object>();
			IEnumerable<object> test = list.AsEnumerable();

			//Any cast

			if (test.Cast<string>().Count != 0)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list.Add(new VENP(1));
			list.Add(new VENP(2));
			list.Add(new VENP(3));

			//Valid cast

			if (test.Cast<VENP>().Count != 3)
			{
				throw new TestAssertionException();
			}

			if (test.Cast<IEquatable<VENP>>()[0] == null)
			{
				throw new TestAssertionException();
			}

			//Invalid cast

			try
			{
				test.Cast<Version>();
				throw new TestAssertionException();
			}
			catch (InvalidCastException)
			{
			}
		}

		[TestMethod]
		public void Concat_Test ()
		{
			//------------
			// Empty lists
			//------------

			//Preparation
			List<VENP> list1 = new List<VENP>();
			List<VENP> list2 = new List<VENP>();
			IEnumerable<VENP> test1 = list1.AsEnumerable();
			IEnumerable<VENP> test2 = list2.AsEnumerable();

			//First and second

			if (test1.Concat(test2).Count != 0)
			{
				throw new TestAssertionException();
			}

			//Second and first

			if (test2.Concat(test1).Count != 0)
			{
				throw new TestAssertionException();
			}

			//---------------------------------------------
			// Non-empty lists (first filled, second empty)
			//---------------------------------------------

			//Preparation
			list1.Add(new VENP(1));
			list1.Add(new VENP(2));
			list1.Add(new VENP(3));

			//First and second

			if (test1.Concat(test2).Count != 3)
			{
				throw new TestAssertionException();
			}

			if (test1.Concat(test2)[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (test1.Concat(test2)[2] != "3")
			{
				throw new TestAssertionException();
			}

			//Second and first

			if (test2.Concat(test1).Count != 3)
			{
				throw new TestAssertionException();
			}

			if (test2.Concat(test1)[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (test2.Concat(test1)[2] != "3")
			{
				throw new TestAssertionException();
			}

			//First with itself

			if (test1.Concat(test1).Count != 6)
			{
				throw new TestAssertionException();
			}

			if (test1.Concat(test1)[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (test1.Concat(test1)[3] != 1)
			{
				throw new TestAssertionException();
			}

			//----------------------------------------------
			// Non-empty lists (first filled, second filled)
			//----------------------------------------------

			list2.Add(new VENP("a"));
			list2.Add(new VENP("b"));
			list2.Add(new VENP("c"));

			//First and second

			if (test1.Concat(test2).Count != 6)
			{
				throw new TestAssertionException();
			}

			if (test1.Concat(test2)[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (test1.Concat(test2)[5] != "c")
			{
				throw new TestAssertionException();
			}

			//Second and first

			if (test2.Concat(test1).Count != 6)
			{
				throw new TestAssertionException();
			}

			if (test2.Concat(test1)[0] != "a")
			{
				throw new TestAssertionException();
			}

			if (test2.Concat(test1)[5] != 3)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Contains_Test ()
		{
			RENP r1 = new RENP("a");
			RENP r2 = new RENP("a");
			VENP v1 = new VENP(1);
			VENP v2 = new VENP(1);

			//-----------
			// Empty list
			//-----------

			//Preparation
			List<object> list1 = new List<object>();
			IEnumerable<object> test1 = list1.AsEnumerable();

			//Default comparer

			if (test1.Contains(r1))
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test1.Contains(r1, new EqualityComparison<object>(object.ReferenceEquals)))
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (test1.Contains(r1, object.ReferenceEquals))
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			List<RENP> list2 = new List<RENP>
			{
				r1
			};
			IEnumerable<RENP> test2 = list2.AsEnumerable();

			//Default comparer

			if (!test2.Contains(r1))
			{
				throw new TestAssertionException();
			}

			if (test2.Contains(r2))
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (!test2.Contains(r1, new EqualityComparison<RENP>(object.ReferenceEquals)))
			{
				throw new TestAssertionException();
			}

			if (test2.Contains(r2, new EqualityComparison<RENP>(object.ReferenceEquals)))
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (!test2.Contains(r1, object.ReferenceEquals))
			{
				throw new TestAssertionException();
			}

			if (test2.Contains(r2, object.ReferenceEquals))
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			List<VENP> list3 = new List<VENP>
			{
				v1
			};
			IEnumerable<VENP> test3 = list3.AsEnumerable();

			//Default comparer

			if (!test3.Contains(v1))
			{
				throw new TestAssertionException();
			}

			if (!test3.Contains(v2))
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (!test3.Contains(v1, new EqualityComparison<VENP>(object.ReferenceEquals)))
			{
				throw new TestAssertionException();
			}

			if (test3.Contains(v2, new EqualityComparison<VENP>(object.ReferenceEquals)))
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (!test3.Contains(v1, object.ReferenceEquals))
			{
				throw new TestAssertionException();
			}

			if (test3.Contains(v2, object.ReferenceEquals))
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Count_Test ()
		{
			//-----------
			// Empty list
			//-----------

			//Preparation
			List<VENP> list = new List<VENP>();
			IEnumerable<VENP> test = list.AsEnumerable();

			//Unconditional

			if (test.Count() != 0)
			{
				throw new TestAssertionException();
			}

			//Without index

			if (test.Count(x => x == 0) != 0)
			{
				throw new TestAssertionException();
			}

			//With index

			if (test.Count((x, y) => y == 0) != 0)
			{
				throw new TestAssertionException();
			}

			//By index

			if (test.Count(0) != 0)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list.Add(1);
			list.Add(2);
			list.Add(2);
			list.Add(3);

			//Unconditional

			if (test.Count() != 4)
			{
				throw new TestAssertionException();
			}

			//Without index

			if (test.Count(x => x == 0) != 0)
			{
				throw new TestAssertionException();
			}

			if (test.Count(x => x == 1) != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Count(x => x == 2) != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Count(x => x == 3) != 1)
			{
				throw new TestAssertionException();
			}

			//With index

			if (test.Count((x, y) => y == 0) != 0)
			{
				throw new TestAssertionException();
			}

			if (test.Count((x, y) => y == 1) != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Count((x, y) => y == 2) != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Count((x, y) => y == 3) != 1)
			{
				throw new TestAssertionException();
			}

			//By index

			if (test.Count(0) != 4)
			{
				throw new TestAssertionException();
			}

			if (test.Count(1) != 3)
			{
				throw new TestAssertionException();
			}

			if (test.Count(2) != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Count(3) != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Count(4) != 0)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Distinct_Test ()
		{
			//-------------------
			// Without duplicates
			//-------------------

			//Preparation
			List<int> list = new List<int>
			{
				0,
				1,
				2,
				3,
				4,
				5,
				6,
				7,
				8,
				9
			};
			IEnumerable<int> test = list.AsEnumerable();

			//Default comparer

			if (test.Distinct().Count != 10)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test.Distinct(new EqualityComparison<int>((x, y) => x == y)).Count != 10)
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (test.Distinct((x, y) => x == y).Count != 10)
			{
				throw new TestAssertionException();
			}

			//----------------
			// With duplicates
			//----------------

			//Preparation
			list.Add(0);
			list.Add(0);
			list.Add(0);
			list.Add(1);
			list.Add(1);
			list.Add(2);

			//Default comparer

			if (test.Distinct().Count != 10)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test.Distinct(new EqualityComparison<int>((x, y) => x == y)).Count != 10)
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (test.Distinct((x, y) => x == y).Count != 10)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ElementAt_Test ()
		{
			//-----------
			// Empty list
			//-----------

			//Preparation
			List<int> list = new List<int>();
			IEnumerable<int> test = list.AsEnumerable();

			//Invalid index

			try
			{
				test.ElementAt(0);
				throw new TestAssertionException();
			}
			catch (InvalidOperationException)
			{
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list.Add(1);
			list.Add(10);
			list.Add(100);

			//Valid index

			if (test.ElementAt(0) != 1)
			{
				throw new TestAssertionException();
			}

			if (test.ElementAt(2) != 100)
			{
				throw new TestAssertionException();
			}

			//Invalid index

			try
			{
				test.ElementAt(3);
				throw new TestAssertionException();
			}
			catch (InvalidOperationException)
			{
			}
		}

		[TestMethod]
		public void ElementAtOrDefault_Test ()
		{
			//-----------
			// Empty list
			//-----------

			//Preparation
			List<int> list = new List<int>();
			IEnumerable<int> test = list.AsEnumerable();

			//Default value

			if (test.ElementAtOrDefault(0) != 0)
			{
				throw new TestAssertionException();
			}

			//Custom value

			if (test.ElementAtOrDefault(0, 99) != 99)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list.Add(1);
			list.Add(10);
			list.Add(100);

			//Default value

			if (test.ElementAtOrDefault(0) != 1)
			{
				throw new TestAssertionException();
			}

			if (test.ElementAtOrDefault(2) != 100)
			{
				throw new TestAssertionException();
			}

			if (test.ElementAtOrDefault(3) != 0)
			{
				throw new TestAssertionException();
			}

			//Custom value

			if (test.ElementAtOrDefault(0, 99) != 1)
			{
				throw new TestAssertionException();
			}

			if (test.ElementAtOrDefault(2, 99) != 100)
			{
				throw new TestAssertionException();
			}

			if (test.ElementAtOrDefault(3, 99) != 99)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Except_Test ()
		{
			//------------
			// Empty lists
			//------------

			//Preparation
			List<int> list1 = new List<int>();
			List<int> list2 = new List<int>();
			IEnumerable<int> test1 = list1.AsEnumerable();
			IEnumerable<int> test2 = list2.AsEnumerable();

			//Default comparer

			if (test1.Except(test2).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test2.Except(test1).Count != 0)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test1.Except(test2, new EqualityComparison<int>((x, y) => x == y)).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test2.Except(test1, new EqualityComparison<int>((x, y) => x == y)).Count != 0)
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (test1.Except(test2, (x, y) => x == y).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test2.Except(test1, (x, y) => x == y).Count != 0)
			{
				throw new TestAssertionException();
			}

			//---------------------------------------------
			// Non-empty lists (first filled, second empty)
			//---------------------------------------------

			//Preparation
			list1.Add(1);
			list1.Add(2);
			list1.Add(3);

			//Default comparer

			if (test1.Except(test2).Count != 3)
			{
				throw new TestAssertionException();
			}

			if (test2.Except(test1).Count != 0)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test1.Except(test2, new EqualityComparison<int>((x, y) => x == y)).Count != 3)
			{
				throw new TestAssertionException();
			}

			if (test2.Except(test1, new EqualityComparison<int>((x, y) => x == y)).Count != 0)
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (test1.Except(test2, (x, y) => x == y).Count != 3)
			{
				throw new TestAssertionException();
			}

			if (test2.Except(test1, (x, y) => x == y).Count != 0)
			{
				throw new TestAssertionException();
			}

			//----------------------------------------------
			// Non-empty lists (first filled, second filled)
			//----------------------------------------------

			//Preparation
			list2.Add(2);
			list2.Add(3);
			list2.Add(4);
			list2.Add(5);
			list2.Add(6);

			//Default comparer

			if (test1.Except(test2).Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test2.Except(test1).Count != 3)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test1.Except(test2, new EqualityComparison<int>((x, y) => x == y)).Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test2.Except(test1, new EqualityComparison<int>((x, y) => x == y)).Count != 3)
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (test1.Except(test2, (x, y) => x == y).Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test2.Except(test1, (x, y) => x == y).Count != 3)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Exclusive_Test ()
		{
			//------------
			// Empty lists
			//------------

			//Preparation
			List<int> list1 = new List<int>();
			List<int> list2 = new List<int>();
			IEnumerable<int> test1 = list1.AsEnumerable();
			IEnumerable<int> test2 = list2.AsEnumerable();

			//Default comparer

			if (test1.Exclusive(test2).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test2.Exclusive(test1).Count != 0)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test1.Exclusive(test2, new EqualityComparison<int>((x, y) => x == y)).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test2.Exclusive(test1, new EqualityComparison<int>((x, y) => x == y)).Count != 0)
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (test1.Exclusive(test2, (x, y) => x == y).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test2.Exclusive(test1, (x, y) => x == y).Count != 0)
			{
				throw new TestAssertionException();
			}

			//---------------------------------------------
			// Non-empty lists (first filled, second empty)
			//---------------------------------------------

			//Preparation
			list1.Add(1);
			list1.Add(2);
			list1.Add(3);

			//Default comparer

			if (test1.Exclusive(test2).Count != 3)
			{
				throw new TestAssertionException();
			}

			if (test2.Exclusive(test1).Count != 3)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test1.Exclusive(test2, new EqualityComparison<int>((x, y) => x == y)).Count != 3)
			{
				throw new TestAssertionException();
			}

			if (test2.Exclusive(test1, new EqualityComparison<int>((x, y) => x == y)).Count != 3)
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (test1.Exclusive(test2, (x, y) => x == y).Count != 3)
			{
				throw new TestAssertionException();
			}

			if (test2.Exclusive(test1, (x, y) => x == y).Count != 3)
			{
				throw new TestAssertionException();
			}

			//----------------------------------------------
			// Non-empty lists (first filled, second filled)
			//----------------------------------------------

			//Preparation
			list2.Add(2);
			list2.Add(3);
			list2.Add(4);
			list2.Add(5);
			list2.Add(6);

			//Default comparer

			if (test1.Exclusive(test2).Count != 4)
			{
				throw new TestAssertionException();
			}

			if (test2.Exclusive(test1).Count != 4)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test1.Exclusive(test2, new EqualityComparison<int>((x, y) => x == y)).Count != 4)
			{
				throw new TestAssertionException();
			}

			if (test2.Exclusive(test1, new EqualityComparison<int>((x, y) => x == y)).Count != 4)
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (test1.Exclusive(test2, (x, y) => x == y).Count != 4)
			{
				throw new TestAssertionException();
			}

			if (test2.Exclusive(test1, (x, y) => x == y).Count != 4)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void First_Test ()
		{
			//-----------
			// Empty list
			//-----------

			//Preparation
			List<int> list = new List<int>();
			IEnumerable<int> test = list.AsEnumerable();

			//Invalid item

			try
			{
				test.First();
				throw new TestAssertionException();
			}
			catch (InvalidOperationException)
			{
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list.Add(1);
			list.Add(10);
			list.Add(100);

			//Valid item

			if (test.First() != 1)
			{
				throw new TestAssertionException();
			}

			if (test.First(x => x > 5) != 10)
			{
				throw new TestAssertionException();
			}

			if (test.First((x, y) => y > 5) != 10)
			{
				throw new TestAssertionException();
			}

			//Invalid item

			try
			{
				test.First(x => x > 100);
				throw new TestAssertionException();
			}
			catch (InvalidOperationException)
			{
			}

			try
			{
				test.First((x, y) => y > 100);
				throw new TestAssertionException();
			}
			catch (InvalidOperationException)
			{
			}
		}

		[TestMethod]
		public void FirstOrDefault_Test ()
		{
			//-----------
			// Empty list
			//-----------

			//Preparation
			List<int> list = new List<int>();
			IEnumerable<int> test = list.AsEnumerable();

			//Unconditional

			if (test.FirstOrDefault() != 0)
			{
				throw new TestAssertionException();
			}

			if (test.FirstOrDefault(99) != 99)
			{
				throw new TestAssertionException();
			}

			//With predicate

			if (test.FirstOrDefault(x => x > 5) != 0)
			{
				throw new TestAssertionException();
			}

			if (test.FirstOrDefault(99, x => x > 5) != 99)
			{
				throw new TestAssertionException();
			}

			if (test.FirstOrDefault((x, y) => y > 5) != 0)
			{
				throw new TestAssertionException();
			}

			if (test.FirstOrDefault(99, (x, y) => y > 5) != 99)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list.Add(1);
			list.Add(10);
			list.Add(100);

			//Unconditional

			if (test.FirstOrDefault() != 1)
			{
				throw new TestAssertionException();
			}

			if (test.FirstOrDefault(99) != 1)
			{
				throw new TestAssertionException();
			}

			//With predicate

			if (test.FirstOrDefault(x => x > 5) != 10)
			{
				throw new TestAssertionException();
			}

			if (test.FirstOrDefault(99, x => x > 5) != 10)
			{
				throw new TestAssertionException();
			}

			if (test.FirstOrDefault(x => x > 100) != 0)
			{
				throw new TestAssertionException();
			}

			if (test.FirstOrDefault(99, x => x > 100) != 99)
			{
				throw new TestAssertionException();
			}

			if (test.FirstOrDefault((x, y) => y > 5) != 10)
			{
				throw new TestAssertionException();
			}

			if (test.FirstOrDefault(99, (x, y) => y > 5) != 10)
			{
				throw new TestAssertionException();
			}

			if (test.FirstOrDefault((x, y) => y > 100) != 0)
			{
				throw new TestAssertionException();
			}

			if (test.FirstOrDefault(99, (x, y) => y > 100) != 99)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ForEach_Test ()
		{
			Action<RENP> testAction1 = x => x.Number++;
			Action<int, RENP> testAction2 = (x, y) => y.Number = x;

			//-----------
			// Empty list
			//-----------

			//Preparation
			List<RENP> list = new List<RENP>();
			IEnumerable<RENP> test = list.AsEnumerable();

			//Without index

			if (test.ForEach(testAction1) != 0)
			{
				throw new TestAssertionException();
			}

			//With index

			if (test.ForEach(testAction2) != 0)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list.Add(1);
			list.Add(9);
			list.Add(10);

			//Without index

			if (test.ForEach(testAction1) != 3)
			{
				throw new TestAssertionException();
			}

			if (list[0].Number != 2)
			{
				throw new TestAssertionException();
			}

			if (list[1].Number != 10)
			{
				throw new TestAssertionException();
			}

			if (list[2].Number != 11)
			{
				throw new TestAssertionException();
			}

			//With index

			if (test.ForEach(testAction2) != 3)
			{
				throw new TestAssertionException();
			}

			if (list[0].Number != 0)
			{
				throw new TestAssertionException();
			}

			if (list[1].Number != 1)
			{
				throw new TestAssertionException();
			}

			if (list[2].Number != 2)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Intersect_Test ()
		{
			//------------
			// Empty lists
			//------------

			//Preparation
			List<int> list1 = new List<int>();
			List<int> list2 = new List<int>();
			IEnumerable<int> test1 = list1.AsEnumerable();
			IEnumerable<int> test2 = list2.AsEnumerable();

			//Default comparer

			if (test1.Intersect(test2).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test2.Intersect(test1).Count != 0)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test1.Intersect(test2, new EqualityComparison<int>((x, y) => x == y)).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test2.Intersect(test1, new EqualityComparison<int>((x, y) => x == y)).Count != 0)
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (test1.Intersect(test2, (x, y) => x == y).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test2.Intersect(test1, (x, y) => x == y).Count != 0)
			{
				throw new TestAssertionException();
			}

			//---------------------------------------------
			// Non-empty lists (first filled, second empty)
			//---------------------------------------------

			//Preparation
			list1.Add(1);
			list1.Add(2);
			list1.Add(3);

			//Default comparer

			if (test1.Intersect(test2).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test2.Intersect(test1).Count != 0)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test1.Intersect(test2, new EqualityComparison<int>((x, y) => x == y)).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test2.Intersect(test1, new EqualityComparison<int>((x, y) => x == y)).Count != 0)
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (test1.Intersect(test2, (x, y) => x == y).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test2.Intersect(test1, (x, y) => x == y).Count != 0)
			{
				throw new TestAssertionException();
			}

			//----------------------------------------------
			// Non-empty lists (first filled, second filled)
			//----------------------------------------------

			//Preparation
			list2.Add(2);
			list2.Add(3);
			list2.Add(4);
			list2.Add(5);
			list2.Add(6);

			//Default comparer

			if (test1.Intersect(test2).Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test2.Intersect(test1).Count != 2)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test1.Intersect(test2, new EqualityComparison<int>((x, y) => x == y)).Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test2.Intersect(test1, new EqualityComparison<int>((x, y) => x == y)).Count != 2)
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (test1.Intersect(test2, (x, y) => x == y).Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test2.Intersect(test1, (x, y) => x == y).Count != 2)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Last_Test ()
		{
			//-----------
			// Empty list
			//-----------

			//Preparation
			List<int> list = new List<int>();
			IEnumerable<int> test = list.AsEnumerable();

			//Invalid item

			try
			{
				test.Last();
				throw new TestAssertionException();
			}
			catch (InvalidOperationException)
			{
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list.Add(1);
			list.Add(10);
			list.Add(100);

			//Valid item

			if (test.Last() != 100)
			{
				throw new TestAssertionException();
			}

			if (test.Last(x => x < 50) != 10)
			{
				throw new TestAssertionException();
			}

			if (test.Last((x, y) => y < 50) != 10)
			{
				throw new TestAssertionException();
			}

			//Invalid item

			try
			{
				test.Last(x => x < 1);
				throw new TestAssertionException();
			}
			catch (InvalidOperationException)
			{
			}

			try
			{
				test.Last((x, y) => y < 1);
				throw new TestAssertionException();
			}
			catch (InvalidOperationException)
			{
			}
		}

		[TestMethod]
		public void LastOrDefault_Test ()
		{
			//-----------
			// Empty list
			//-----------

			//Preparation
			List<int> list = new List<int>();
			IEnumerable<int> test = list.AsEnumerable();

			//Unconditional

			if (test.LastOrDefault() != 0)
			{
				throw new TestAssertionException();
			}

			if (test.LastOrDefault(99) != 99)
			{
				throw new TestAssertionException();
			}

			//With predicate

			if (test.LastOrDefault(x => x < 0) != 0)
			{
				throw new TestAssertionException();
			}

			if (test.LastOrDefault(99, x => x < 0) != 99)
			{
				throw new TestAssertionException();
			}

			if (test.LastOrDefault((x, y) => y < 0) != 0)
			{
				throw new TestAssertionException();
			}

			if (test.LastOrDefault(99, (x, y) => y < 0) != 99)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list.Add(1);
			list.Add(10);
			list.Add(100);

			//Unconditional

			if (test.LastOrDefault() != 100)
			{
				throw new TestAssertionException();
			}

			if (test.LastOrDefault(99) != 100)
			{
				throw new TestAssertionException();
			}

			//With predicate

			if (test.LastOrDefault(x => x < 50) != 10)
			{
				throw new TestAssertionException();
			}

			if (test.LastOrDefault(99, x => x < 50) != 10)
			{
				throw new TestAssertionException();
			}

			if (test.LastOrDefault(x => x < 0) != 0)
			{
				throw new TestAssertionException();
			}

			if (test.LastOrDefault(99, x => x < 0) != 99)
			{
				throw new TestAssertionException();
			}

			if (test.LastOrDefault((x, y) => y < 50) != 10)
			{
				throw new TestAssertionException();
			}

			if (test.LastOrDefault(99, (x, y) => y < 50) != 10)
			{
				throw new TestAssertionException();
			}

			if (test.LastOrDefault((x, y) => y < 0) != 0)
			{
				throw new TestAssertionException();
			}

			if (test.LastOrDefault(99, (x, y) => y < 0) != 99)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void OfType_Test ()
		{
			//-----------
			// Empty list
			//-----------

			//Preparation
			List<object> list = new List<object>();
			IEnumerable<object> test = list.AsEnumerable();

			//Any cast

			if (test.OfType<string>().Count != 0)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list.Add(new VENP(1));
			list.Add(new VENP(2));
			list.Add(new VENP(3));
			list.Add(new RENP(4));
			list.Add(new RENP(5));

			//Valid cast

			if (test.OfType<VENP>().Count != 3)
			{
				throw new TestAssertionException();
			}

			if (test.OfType<RENP>().Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test.OfType<IEquatable<VENP>>()[0] == null)
			{
				throw new TestAssertionException();
			}

			if (test.OfType<IEquatable<RENP>>()[0] == null)
			{
				throw new TestAssertionException();
			}

			//Invalid cast

			if (test.OfType<Version>().Count != 0)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Reverse_Test ()
		{
			//-----------
			// Empty list
			//-----------

			//Preparation
			List<int> list = new List<int>();
			IEnumerable<int> test = list.AsEnumerable();

			//Test

			List<int> result = test.Reverse();
			if (result.Count != 0)
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

			//Test

			result = test.Reverse();
			if (result.Count != 4)
			{
				throw new TestAssertionException();
			}

			if (result[0] != 0)
			{
				throw new TestAssertionException();
			}

			if (result[1] != 20)
			{
				throw new TestAssertionException();
			}

			if (result[2] != 5)
			{
				throw new TestAssertionException();
			}

			if (result[3] != 10)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Same_Test ()
		{
			RENP r1 = new RENP("a");
			RENP r2 = new RENP("a");
			VENP v1 = new VENP(1);
			VENP v2 = new VENP(1);

			//-----------
			// Empty list
			//-----------

			//Preparation
			List<object> list1 = new List<object>();
			IEnumerable<object> test1 = list1.AsEnumerable();

			//Default comparer

			if (test1.Same(r1) != 0)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test1.Same(r1, new EqualityComparison<object>(object.ReferenceEquals)) != 0)
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (test1.Same(r1, object.ReferenceEquals) != 0)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			List<RENP> list2 = new List<RENP>
			{
				r1
			};
			IEnumerable<RENP> test2 = list2.AsEnumerable();

			//Default comparer

			if (test2.Same(r1) != 1)
			{
				throw new TestAssertionException();
			}

			if (test2.Same(r2) != 0)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test2.Same(r1, new EqualityComparison<RENP>(object.ReferenceEquals)) != 1)
			{
				throw new TestAssertionException();
			}

			if (test2.Same(r2, new EqualityComparison<RENP>(object.ReferenceEquals)) != 0)
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (test2.Same(r1, object.ReferenceEquals) != 1)
			{
				throw new TestAssertionException();
			}

			if (test2.Same(r2, object.ReferenceEquals) != 0)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list2.Add(r1);

			//Default comparer

			if (test2.Same(r1) != 2)
			{
				throw new TestAssertionException();
			}

			if (test2.Same(r2) != 0)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test2.Same(r1, new EqualityComparison<RENP>(object.ReferenceEquals)) != 2)
			{
				throw new TestAssertionException();
			}

			if (test2.Same(r2, new EqualityComparison<RENP>(object.ReferenceEquals)) != 0)
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (test2.Same(r1, object.ReferenceEquals) != 2)
			{
				throw new TestAssertionException();
			}

			if (test2.Same(r2, object.ReferenceEquals) != 0)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			List<VENP> list3 = new List<VENP>
			{
				v1
			};
			IEnumerable<VENP> test3 = list3.AsEnumerable();

			//Default comparer

			if (test3.Same(v1) != 1)
			{
				throw new TestAssertionException();
			}

			if (test3.Same(v2) != 1)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test3.Same(v1, new EqualityComparison<VENP>(object.ReferenceEquals)) != 1)
			{
				throw new TestAssertionException();
			}

			if (test3.Same(v2, new EqualityComparison<VENP>(object.ReferenceEquals)) != 0)
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (test3.Same(v1, object.ReferenceEquals) != 1)
			{
				throw new TestAssertionException();
			}

			if (test3.Same(v2, object.ReferenceEquals) != 0)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list3.Add(v2);

			//Default comparer

			if (test3.Same(v1) != 2)
			{
				throw new TestAssertionException();
			}

			if (test3.Same(v2) != 2)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test3.Same(v1, new EqualityComparison<VENP>(object.ReferenceEquals)) != 1)
			{
				throw new TestAssertionException();
			}

			if (test3.Same(v2, new EqualityComparison<VENP>(object.ReferenceEquals)) != 1)
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (test3.Same(v1, object.ReferenceEquals) != 1)
			{
				throw new TestAssertionException();
			}

			if (test3.Same(v2, object.ReferenceEquals) != 1)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Select_Test ()
		{
			//-----------
			// Empty list
			//-----------

			//Preparation
			List<int> list = new List<int>();
			IEnumerable<int> test = list.AsEnumerable();

			//Without index

			if (test.Select(x => x.ToString()).Count != 0)
			{
				throw new TestAssertionException();
			}

			//With index

			if (test.Select((x, y) => (x + y).ToString()).Count != 0)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list.Add(1);
			list.Add(2);
			list.Add(3);
			list.Add(4);

			//Without index

			List<string> result = test.Select(x => x.ToString());

			if (result.Count != 4)
			{
				throw new TestAssertionException();
			}

			if (result[0] != "1")
			{
				throw new TestAssertionException();
			}

			if (result[1] != "2")
			{
				throw new TestAssertionException();
			}

			if (result[2] != "3")
			{
				throw new TestAssertionException();
			}

			if (result[3] != "4")
			{
				throw new TestAssertionException();
			}

			//With index

			result = test.Select((x, y) => (x + y).ToString());

			if (result.Count != 4)
			{
				throw new TestAssertionException();
			}

			if (result[0] != "1")
			{
				throw new TestAssertionException();
			}

			if (result[1] != "3")
			{
				throw new TestAssertionException();
			}

			if (result[2] != "5")
			{
				throw new TestAssertionException();
			}

			if (result[3] != "7")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void SelectMany_Test ()
		{
			//-----------
			// Empty list
			//-----------

			//Preparation
			List<int> list = new List<int>();
			IEnumerable<int> test = list.AsEnumerable();

			//Without index

			if (test.SelectMany(x => x.ToString()).Count != 0)
			{
				throw new TestAssertionException();
			}

			//With index

			if (test.SelectMany((x, y) => (x + y).ToString()).Count != 0)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list.Add(10);
			list.Add(20);
			list.Add(30);
			list.Add(40);

			//Without index

			List<char> result = test.SelectMany(x => x.ToString());

			if (result.Count != 8)
			{
				throw new TestAssertionException();
			}

			if (result[0] != '1')
			{
				throw new TestAssertionException();
			}

			if (result[1] != '0')
			{
				throw new TestAssertionException();
			}

			if (result[6] != '4')
			{
				throw new TestAssertionException();
			}

			if (result[7] != '0')
			{
				throw new TestAssertionException();
			}

			//With index

			result = test.SelectMany((x, y) => (x + y).ToString());

			if (result.Count != 8)
			{
				throw new TestAssertionException();
			}

			if (result[0] != '1')
			{
				throw new TestAssertionException();
			}

			if (result[1] != '0')
			{
				throw new TestAssertionException();
			}

			if (result[6] != '4')
			{
				throw new TestAssertionException();
			}

			if (result[7] != '3')
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void SequenceEqual_Test ()
		{
			//-----------
			// Empty list
			//-----------

			//Preparation
			List<VENP> list1 = new List<VENP>();
			List<VENP> list2 = new List<VENP>();
			IEnumerable<VENP> test1 = list1.AsEnumerable();
			IEnumerable<VENP> test2 = list2.AsEnumerable();

			//With flags

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (!test2.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.None))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.None))
			{
				throw new TestAssertionException();
			}

			if (!test2.SequenceEqual(test1, CollectionComparerFlags.None))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.ReferenceEquality))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.ReferenceEquality))
			{
				throw new TestAssertionException();
			}

			if (!test2.SequenceEqual(test1, CollectionComparerFlags.ReferenceEquality))
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.None, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.None, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (!test2.SequenceEqual(test1, CollectionComparerFlags.None, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.IgnoreOrder, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (!test2.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.None, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.None, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (!test2.SequenceEqual(test1, CollectionComparerFlags.None, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.IgnoreOrder, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (!test2.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			//---------------------------------------------
			// Non-empty lists (first filled, second empty)
			//---------------------------------------------

			//Preparation
			list1.Add(1);
			list1.Add(2);
			list1.Add(3);
			list1.Add(4);

			//With flags

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test2, CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (test2.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.None))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test2, CollectionComparerFlags.None))
			{
				throw new TestAssertionException();
			}

			if (test2.SequenceEqual(test1, CollectionComparerFlags.None))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.ReferenceEquality))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test2, CollectionComparerFlags.ReferenceEquality))
			{
				throw new TestAssertionException();
			}

			if (test2.SequenceEqual(test1, CollectionComparerFlags.ReferenceEquality))
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.None, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test2, CollectionComparerFlags.None, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (test2.SequenceEqual(test1, CollectionComparerFlags.None, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test2, CollectionComparerFlags.IgnoreOrder, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (test2.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.None, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test2, CollectionComparerFlags.None, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (test2.SequenceEqual(test1, CollectionComparerFlags.None, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test2, CollectionComparerFlags.IgnoreOrder, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (test2.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			//----------------------------------------------------------
			// Non-empty lists (first filled, second filled), same order
			//----------------------------------------------------------

			//Preparation
			list2.AddRange(list1);

			//With flags

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (!test2.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.None))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.None))
			{
				throw new TestAssertionException();
			}

			if (!test2.SequenceEqual(test1))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.ReferenceEquality | CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.ReferenceEquality | CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (!test2.SequenceEqual(test1, CollectionComparerFlags.ReferenceEquality | CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.None, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.None, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (!test2.SequenceEqual(test1, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.IgnoreOrder, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (!test2.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.None, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.None, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (!test2.SequenceEqual(test1, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.IgnoreOrder, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (!test2.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			//---------------------------------------------------------------
			// Non-empty lists (first filled, second filled), different order
			//---------------------------------------------------------------

			//Preparation
			list2.Reverse();

			//With flags

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (!test2.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.None))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test2, CollectionComparerFlags.None))
			{
				throw new TestAssertionException();
			}

			if (test2.SequenceEqual(test1))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.ReferenceEquality | CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.ReferenceEquality | CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (!test2.SequenceEqual(test1, CollectionComparerFlags.ReferenceEquality | CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.None, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test2, CollectionComparerFlags.None, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (test2.SequenceEqual(test1, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.IgnoreOrder, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (!test2.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.None, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test2, CollectionComparerFlags.None, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (test2.SequenceEqual(test1, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.IgnoreOrder, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (!test2.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			//-----------------------------------------------------------------
			// Non-empty lists (first filled, second filled), different content
			//-----------------------------------------------------------------

			//Preparation
			list2.Clear();
			list2.Add(1);
			list2.Add(2);
			list2.Add(11);
			list2.Add(12);

			//With flags

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test2, CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (test2.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.None))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test2, CollectionComparerFlags.None))
			{
				throw new TestAssertionException();
			}

			if (test2.SequenceEqual(test1))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.ReferenceEquality | CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test2, CollectionComparerFlags.ReferenceEquality | CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (test2.SequenceEqual(test1, CollectionComparerFlags.ReferenceEquality | CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.None, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test2, CollectionComparerFlags.None, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (test2.SequenceEqual(test1, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test2, CollectionComparerFlags.IgnoreOrder, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (test2.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder, new EqualityComparison<VENP>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.None, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test2, CollectionComparerFlags.None, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (test2.SequenceEqual(test1, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test2, CollectionComparerFlags.IgnoreOrder, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (test2.SequenceEqual(test1, CollectionComparerFlags.IgnoreOrder, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			//---------------------------------
			// Reference equalities, same order
			//---------------------------------

			//Preparation
			list2.Clear();
			list2.AddRange(list1);
			List<VENP> list3 = new List<VENP>
			{
				1,
				2,
				3,
				4
			};
			IEnumerable<VENP> test3 = list3.AsEnumerable();

			//Same references

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.None))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.ReferenceEquality))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.ReferenceEquality))
			{
				throw new TestAssertionException();
			}

			//Different references

			if (!test1.SequenceEqual(test3, CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test3, CollectionComparerFlags.None))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test3, CollectionComparerFlags.ReferenceEquality | CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test3, CollectionComparerFlags.ReferenceEquality))
			{
				throw new TestAssertionException();
			}

			//---------------------------------
			// Reference equalities, different order
			//---------------------------------

			//Preparation
			list2.Reverse();
			list3.Reverse();

			//Same references

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test2, CollectionComparerFlags.None))
			{
				throw new TestAssertionException();
			}

			if (!test1.SequenceEqual(test2, CollectionComparerFlags.ReferenceEquality | CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test2, CollectionComparerFlags.ReferenceEquality))
			{
				throw new TestAssertionException();
			}

			//Different references

			if (!test1.SequenceEqual(test3, CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test3, CollectionComparerFlags.None))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test3, CollectionComparerFlags.ReferenceEquality | CollectionComparerFlags.IgnoreOrder))
			{
				throw new TestAssertionException();
			}

			if (test1.SequenceEqual(test3, CollectionComparerFlags.ReferenceEquality))
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Skip_Test ()
		{
			//-----------
			// Empty list
			//-----------

			//Preparation
			List<int> list = new List<int>();
			IEnumerable<int> test = list.AsEnumerable();

			//Test

			if (test.Skip(0).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test.Skip(2).Count != 0)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list.Add(1);
			list.Add(2);
			list.Add(3);
			list.Add(4);

			//Test

			if (test.Skip(0).Count != 4)
			{
				throw new TestAssertionException();
			}

			if (test.Skip(0)[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Skip(0)[3] != 4)
			{
				throw new TestAssertionException();
			}

			if (test.Skip(2).Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Skip(2)[0] != 3)
			{
				throw new TestAssertionException();
			}

			if (test.Skip(2)[1] != 4)
			{
				throw new TestAssertionException();
			}

			if (test.Skip(4).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test.Skip(5).Count != 0)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void SkipWhile_Test ()
		{
			//-----------
			// Empty list
			//-----------

			//Preparation
			List<int> list = new List<int>();
			IEnumerable<int> test = list.AsEnumerable();

			//Without index

			if (test.SkipWhile(x => x < 3).Count != 0)
			{
				throw new TestAssertionException();
			}

			//With index

			if (test.SkipWhile((x, y) => x < 2).Count != 0)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list.Add(1);
			list.Add(2);
			list.Add(3);
			list.Add(4);

			//Without index

			if (test.SkipWhile(x => x < 3).Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test.SkipWhile(x => x < 3)[0] != 3)
			{
				throw new TestAssertionException();
			}

			if (test.SkipWhile(x => x < 3)[1] != 4)
			{
				throw new TestAssertionException();
			}

			//With index

			if (test.SkipWhile((x, y) => x < 2).Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test.SkipWhile((x, y) => x < 2)[0] != 3)
			{
				throw new TestAssertionException();
			}

			if (test.SkipWhile((x, y) => x < 2)[1] != 4)
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
			IEnumerable<int> test = list.AsEnumerable();

			//Default comparer

			List<int> result = test.Sort(false);
			if (result.Count != 0)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			result = test.Sort(false, new OrderComparison<int>((x, y) => x.CompareTo(y)));
			if (result.Count != 0)
			{
				throw new TestAssertionException();
			}

			//Custom function

			result = test.Sort(false, (x, y) => x.CompareTo(y));
			if (result.Count != 0)
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

			result = test.Sort(false);
			if (result.Count != 4)
			{
				throw new TestAssertionException();
			}

			if (result[0] != 0)
			{
				throw new TestAssertionException();
			}

			if (result[1] != 5)
			{
				throw new TestAssertionException();
			}

			if (result[2] != 10)
			{
				throw new TestAssertionException();
			}

			if (result[3] != 20)
			{
				throw new TestAssertionException();
			}

			result = test.Sort(true);
			if (result.Count != 4)
			{
				throw new TestAssertionException();
			}

			if (result[0] != 20)
			{
				throw new TestAssertionException();
			}

			if (result[1] != 10)
			{
				throw new TestAssertionException();
			}

			if (result[2] != 5)
			{
				throw new TestAssertionException();
			}

			if (result[3] != 0)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			result = test.Sort(false, new OrderComparison<int>((x, y) => x.CompareTo(y)));
			if (result.Count != 4)
			{
				throw new TestAssertionException();
			}

			if (result[0] != 0)
			{
				throw new TestAssertionException();
			}

			if (result[1] != 5)
			{
				throw new TestAssertionException();
			}

			if (result[2] != 10)
			{
				throw new TestAssertionException();
			}

			if (result[3] != 20)
			{
				throw new TestAssertionException();
			}

			result = test.Sort(true, new OrderComparison<int>((x, y) => x.CompareTo(y)));
			if (result.Count != 4)
			{
				throw new TestAssertionException();
			}

			if (result[0] != 20)
			{
				throw new TestAssertionException();
			}

			if (result[1] != 10)
			{
				throw new TestAssertionException();
			}

			if (result[2] != 5)
			{
				throw new TestAssertionException();
			}

			if (result[3] != 0)
			{
				throw new TestAssertionException();
			}

			//Custom function

			result = test.Sort(false, (x, y) => x.CompareTo(y));
			if (result.Count != 4)
			{
				throw new TestAssertionException();
			}

			if (result[0] != 0)
			{
				throw new TestAssertionException();
			}

			if (result[1] != 5)
			{
				throw new TestAssertionException();
			}

			if (result[2] != 10)
			{
				throw new TestAssertionException();
			}

			if (result[3] != 20)
			{
				throw new TestAssertionException();
			}

			result = test.Sort(true, (x, y) => x.CompareTo(y));
			if (result.Count != 4)
			{
				throw new TestAssertionException();
			}

			if (result[0] != 20)
			{
				throw new TestAssertionException();
			}

			if (result[1] != 10)
			{
				throw new TestAssertionException();
			}

			if (result[2] != 5)
			{
				throw new TestAssertionException();
			}

			if (result[3] != 0)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Take_Test ()
		{
			//-----------
			// Empty list
			//-----------

			//Preparation
			List<int> list = new List<int>();
			IEnumerable<int> test = list.AsEnumerable();

			//Test

			if (test.Take(0).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test.Take(2).Count != 0)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list.Add(1);
			list.Add(2);
			list.Add(3);
			list.Add(4);

			//Test

			if (test.Take(0).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test.Take(2).Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Take(2)[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Take(2)[1] != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Take(4).Count != 4)
			{
				throw new TestAssertionException();
			}

			if (test.Take(4)[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Take(4)[3] != 4)
			{
				throw new TestAssertionException();
			}

			if (test.Take(5).Count != 4)
			{
				throw new TestAssertionException();
			}

			if (test.Take(5)[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Take(5)[3] != 4)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void TakeWhile_Test ()
		{
			//-----------
			// Empty list
			//-----------

			//Preparation
			List<int> list = new List<int>();
			IEnumerable<int> test = list.AsEnumerable();

			//Without index

			if (test.TakeWhile(x => x < 3).Count != 0)
			{
				throw new TestAssertionException();
			}

			//With index

			if (test.TakeWhile((x, y) => x < 2).Count != 0)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list.Add(1);
			list.Add(2);
			list.Add(3);
			list.Add(4);

			//Without index

			if (test.TakeWhile(x => x < 3).Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test.TakeWhile(x => x < 3)[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (test.TakeWhile(x => x < 3)[1] != 2)
			{
				throw new TestAssertionException();
			}

			//With index

			if (test.TakeWhile((x, y) => x < 2).Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test.TakeWhile((x, y) => x < 2)[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (test.TakeWhile((x, y) => x < 2)[1] != 2)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToArray_Test ()
		{
			//-----------
			// Empty list
			//-----------

			//Preparation
			List<int> list = new List<int>();
			IEnumerable<int> test = list.AsEnumerable();

			//Unconditional

			if (test.ToArray().Length != 0)
			{
				throw new TestAssertionException();
			}

			//With index

			if (test.ToArray(1).Length != 0)
			{
				throw new TestAssertionException();
			}

			//With index and length

			if (test.ToArray(1, 2).Length != 0)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list.Add(1);
			list.Add(2);
			list.Add(3);
			list.Add(4);
			list.Add(5);

			//Unconditional

			if (test.ToArray().Length != 5)
			{
				throw new TestAssertionException();
			}

			if (test.ToArray()[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (test.ToArray()[4] != 5)
			{
				throw new TestAssertionException();
			}

			//With index

			if (test.ToArray(1).Length != 4)
			{
				throw new TestAssertionException();
			}

			if (test.ToArray(1)[0] != 2)
			{
				throw new TestAssertionException();
			}

			if (test.ToArray(1)[3] != 5)
			{
				throw new TestAssertionException();
			}

			//With index and length

			if (test.ToArray(1, 2).Length != 2)
			{
				throw new TestAssertionException();
			}

			if (test.ToArray(1, 2)[0] != 2)
			{
				throw new TestAssertionException();
			}

			if (test.ToArray(1, 2)[1] != 3)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToDictionary_Test ()
		{
			//------------
			// Empty lists
			//------------

			//Preparation
			List<int> list = new List<int>();
			IEnumerable<int> test = list.AsEnumerable();

			//Default comparer

			if (test.ToDictionary(x => new KeyValuePair<string, int>((x/10).ToString(), x)).Count != 0)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test.ToDictionary(StringComparerEx.InvariantCultureIgnoreCase, (x => new KeyValuePair<string, int>((x/10).ToString(), x))).Count != 0)
			{
				throw new TestAssertionException();
			}

			//-----------------------------------
			// Non-empty list (no key duplicates)
			//-----------------------------------

			//Preparation
			list.Add(10);
			list.Add(20);
			list.Add(30);
			list.Add(40);
			list.Add(50);
			list.Add(60);

			//Default comparer

			Dictionary<string, int> result = test.ToDictionary(x => new KeyValuePair<string, int>((x/10).ToString(), x));

			if (result.Count != 6)
			{
				throw new TestAssertionException();
			}

			if (result["1"] != 10)
			{
				throw new TestAssertionException();
			}

			if (result["6"] != 60)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			result = test.ToDictionary(StringComparerEx.InvariantCultureIgnoreCase, (x => new KeyValuePair<string, int>((x/10).ToString(), x)));

			if (result.Count != 6)
			{
				throw new TestAssertionException();
			}

			if (result["1"] != 10)
			{
				throw new TestAssertionException();
			}

			if (result["6"] != 60)
			{
				throw new TestAssertionException();
			}

			//--------------------------------
			// Non-empty list (key duplicates)
			//--------------------------------

			//Preparation
			list.Add(11);

			//Test

			try
			{
				test.ToDictionary(x => new KeyValuePair<string, int>((x/10).ToString(), x));
				throw new TestAssertionException();
			}
			catch (InvalidOperationException)
			{
			}
		}

		[TestMethod]
		public void ToDictionaryList_Test ()
		{
			//------------
			// Empty lists
			//------------

			//Preparation
			List<int> list = new List<int>();
			IEnumerable<int> test = list.AsEnumerable();

			//Default comparer

			if (test.ToDictionaryList(x => new KeyValuePair<string, int>((x/10).ToString(), x)).Count != 0)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test.ToDictionaryList(StringComparerEx.InvariantCultureIgnoreCase, (x => new KeyValuePair<string, int>((x/10).ToString(), x))).Count != 0)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list.Add(11);
			list.Add(12);
			list.Add(12);
			list.Add(21);
			list.Add(22);
			list.Add(33);

			//Default comparer

			Dictionary<string, List<int>> result = test.ToDictionaryList(x => new KeyValuePair<string, int>((x/10).ToString(), x));

			if (result.Count != 3)
			{
				throw new TestAssertionException();
			}

			if (result["1"].Count != 3)
			{
				throw new TestAssertionException();
			}

			if (result["1"][0] != 11)
			{
				throw new TestAssertionException();
			}

			if (result["1"][2] != 12)
			{
				throw new TestAssertionException();
			}

			if (result["2"].Count != 2)
			{
				throw new TestAssertionException();
			}

			if (result["2"][0] != 21)
			{
				throw new TestAssertionException();
			}

			if (result["2"][1] != 22)
			{
				throw new TestAssertionException();
			}

			if (result["3"].Count != 1)
			{
				throw new TestAssertionException();
			}

			if (result["3"][0] != 33)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			result = test.ToDictionaryList(StringComparerEx.InvariantCultureIgnoreCase, (x => new KeyValuePair<string, int>((x/10).ToString(), x)));

			if (result.Count != 3)
			{
				throw new TestAssertionException();
			}

			if (result["1"].Count != 3)
			{
				throw new TestAssertionException();
			}

			if (result["1"][0] != 11)
			{
				throw new TestAssertionException();
			}

			if (result["1"][2] != 12)
			{
				throw new TestAssertionException();
			}

			if (result["2"].Count != 2)
			{
				throw new TestAssertionException();
			}

			if (result["2"][0] != 21)
			{
				throw new TestAssertionException();
			}

			if (result["2"][1] != 22)
			{
				throw new TestAssertionException();
			}

			if (result["3"].Count != 1)
			{
				throw new TestAssertionException();
			}

			if (result["3"][0] != 33)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToDictionarySet_Test ()
		{
			//------------
			// Empty lists
			//------------

			//Preparation
			List<int> list = new List<int>();
			IEnumerable<int> test = list.AsEnumerable();

			//Default comparer

			if (test.ToDictionarySet(x => new KeyValuePair<string, int>((x/10).ToString(), x)).Count != 0)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test.ToDictionarySet(StringComparerEx.InvariantCultureIgnoreCase, EqualityComparer<int>.Default, (x => new KeyValuePair<string, int>((x/10).ToString(), x))).Count != 0)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list.Add(11);
			list.Add(12);
			list.Add(12);
			list.Add(21);
			list.Add(22);
			list.Add(33);

			//Default comparer

			Dictionary<string, HashSet<int>> result = test.ToDictionarySet(x => new KeyValuePair<string, int>((x/10).ToString(), x));

			if (result.Count != 3)
			{
				throw new TestAssertionException();
			}

			if (result["1"].Count != 2)
			{
				throw new TestAssertionException();
			}

			if (result["1"].ElementAt(0) != 11)
			{
				throw new TestAssertionException();
			}

			if (result["1"].ElementAt(1) != 12)
			{
				throw new TestAssertionException();
			}

			if (result["2"].Count != 2)
			{
				throw new TestAssertionException();
			}

			if (result["2"].ElementAt(0) != 21)
			{
				throw new TestAssertionException();
			}

			if (result["2"].ElementAt(1) != 22)
			{
				throw new TestAssertionException();
			}

			if (result["3"].Count != 1)
			{
				throw new TestAssertionException();
			}

			if (result["3"].ElementAt(0) != 33)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			result = test.ToDictionarySet(StringComparerEx.InvariantCultureIgnoreCase, EqualityComparer<int>.Default, (x => new KeyValuePair<string, int>((x/10).ToString(), x)));

			if (result.Count != 3)
			{
				throw new TestAssertionException();
			}

			if (result["1"].Count != 2)
			{
				throw new TestAssertionException();
			}

			if (result["1"].ElementAt(0) != 11)
			{
				throw new TestAssertionException();
			}

			if (result["1"].ElementAt(1) != 12)
			{
				throw new TestAssertionException();
			}

			if (result["2"].Count != 2)
			{
				throw new TestAssertionException();
			}

			if (result["2"].ElementAt(0) != 21)
			{
				throw new TestAssertionException();
			}

			if (result["2"].ElementAt(1) != 22)
			{
				throw new TestAssertionException();
			}

			if (result["3"].Count != 1)
			{
				throw new TestAssertionException();
			}

			if (result["3"].ElementAt(0) != 33)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToList_Test ()
		{
			//-----------
			// Empty list
			//-----------

			//Preparation
			List<int> list = new List<int>();
			IEnumerable<int> test = list.AsEnumerable();

			//Unconditional

			if (test.ToList().Count != 0)
			{
				throw new TestAssertionException();
			}

			//With index

			if (test.ToList(1).Count != 0)
			{
				throw new TestAssertionException();
			}

			//With index and length

			if (test.ToList(1, 2).Count != 0)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list.Add(1);
			list.Add(2);
			list.Add(3);
			list.Add(4);
			list.Add(5);

			//Unconditional

			if (test.ToList().Count != 5)
			{
				throw new TestAssertionException();
			}

			if (test.ToList()[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (test.ToList()[4] != 5)
			{
				throw new TestAssertionException();
			}

			//With index

			if (test.ToList(1).Count != 4)
			{
				throw new TestAssertionException();
			}

			if (test.ToList(1)[0] != 2)
			{
				throw new TestAssertionException();
			}

			if (test.ToList(1)[3] != 5)
			{
				throw new TestAssertionException();
			}

			//With index and length

			if (test.ToList(1, 2).Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test.ToList(1, 2)[0] != 2)
			{
				throw new TestAssertionException();
			}

			if (test.ToList(1, 2)[1] != 3)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Union_Test ()
		{
			//------------
			// Empty lists
			//------------

			//Preparation
			List<int> list1 = new List<int>();
			List<int> list2 = new List<int>();
			IEnumerable<int> test1 = list1.AsEnumerable();
			IEnumerable<int> test2 = list2.AsEnumerable();

			//Default comparer

			if (test1.Union(test2).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test2.Union(test1).Count != 0)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test1.Union(test2, new EqualityComparison<int>((x, y) => x == y)).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test2.Union(test1, new EqualityComparison<int>((x, y) => x == y)).Count != 0)
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (test1.Union(test2, (x, y) => x == y).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test2.Union(test1, (x, y) => x == y).Count != 0)
			{
				throw new TestAssertionException();
			}

			//---------------------------------------------
			// Non-empty lists (first filled, second empty)
			//---------------------------------------------

			//Preparation
			list1.Add(1);
			list1.Add(2);
			list1.Add(3);

			//Default comparer

			if (test1.Union(test2).Count != 3)
			{
				throw new TestAssertionException();
			}

			if (test2.Union(test1).Count != 3)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test1.Union(test2, new EqualityComparison<int>((x, y) => x == y)).Count != 3)
			{
				throw new TestAssertionException();
			}

			if (test2.Union(test1, new EqualityComparison<int>((x, y) => x == y)).Count != 3)
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (test1.Union(test2, (x, y) => x == y).Count != 3)
			{
				throw new TestAssertionException();
			}

			if (test2.Union(test1, (x, y) => x == y).Count != 3)
			{
				throw new TestAssertionException();
			}

			//----------------------------------------------
			// Non-empty lists (first filled, second filled)
			//----------------------------------------------

			//Preparation
			list2.Add(2);
			list2.Add(3);
			list2.Add(4);
			list2.Add(5);
			list2.Add(6);

			//Default comparer

			if (test1.Union(test2).Count != 6)
			{
				throw new TestAssertionException();
			}

			if (test2.Union(test1).Count != 6)
			{
				throw new TestAssertionException();
			}

			//Custom comparer

			if (test1.Union(test2, new EqualityComparison<int>((x, y) => x == y)).Count != 6)
			{
				throw new TestAssertionException();
			}

			if (test2.Union(test1, new EqualityComparison<int>((x, y) => x == y)).Count != 6)
			{
				throw new TestAssertionException();
			}

			//Custom function

			if (test1.Union(test2, (x, y) => x == y).Count != 6)
			{
				throw new TestAssertionException();
			}

			if (test2.Union(test1, (x, y) => x == y).Count != 6)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Where_Test ()
		{
			//------------
			// Empty lists
			//------------

			//Preparation
			List<int> list = new List<int>();
			IEnumerable<int> test = list.AsEnumerable();

			//Without index

			if (test.Where(x => x > 2).Count != 0)
			{
				throw new TestAssertionException();
			}

			//With index

			if (test.Where((x, y) => x > 2).Count != 0)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Non-empty list
			//---------------

			//Preparation
			list.Add(1);
			list.Add(2);
			list.Add(3);
			list.Add(4);
			list.Add(5);

			//Without index

			if (test.Where(x => x > 2).Count != 3)
			{
				throw new TestAssertionException();
			}

			if (test.Where(x => x > 2)[0] != 3)
			{
				throw new TestAssertionException();
			}

			if (test.Where(x => x > 2)[2] != 5)
			{
				throw new TestAssertionException();
			}

			//With index

			if (test.Where((x, y) => x > 2).Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Where((x, y) => x > 2)[0] != 4)
			{
				throw new TestAssertionException();
			}

			if (test.Where((x, y) => x > 2)[1] != 5)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Zip_Test ()
		{
			//------------
			// Empty lists
			//------------

			//Preparation
			List<int> list1 = new List<int>();
			List<int> list2 = new List<int>();
			IEnumerable<int> test1 = list1.AsEnumerable();
			IEnumerable<int> test2 = list2.AsEnumerable();

			//Without index

			if (test1.Zip(test2, (x, y) => x*y).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test2.Zip(test1, (x, y) => x*y).Count != 0)
			{
				throw new TestAssertionException();
			}

			//Wit index

			if (test1.Zip(test2, (x, y, z) => x*y*z).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test2.Zip(test1, (x, y, z) => x*y*z).Count != 0)
			{
				throw new TestAssertionException();
			}

			//---------------------------------------------
			// Non-empty lists (first filled, second empty)
			//---------------------------------------------

			//Preparation
			list1.Add(1);
			list1.Add(2);
			list1.Add(3);

			//Without index

			if (test1.Zip(test2, (x, y) => x*y).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test2.Zip(test1, (x, y) => x*y).Count != 0)
			{
				throw new TestAssertionException();
			}

			//Wit index

			if (test1.Zip(test2, (x, y, z) => x*y*z).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test2.Zip(test1, (x, y, z) => x*y*z).Count != 0)
			{
				throw new TestAssertionException();
			}

			//----------------------------------------------
			// Non-empty lists (first filled, second filled)
			//----------------------------------------------

			//Preparation
			list2.Add(10);
			list2.Add(20);
			list2.Add(30);
			list2.Add(40);
			list2.Add(50);

			//Without index

			if (test1.Zip(test2, (x, y) => x*y).Count != 3)
			{
				throw new TestAssertionException();
			}

			if (test1.Zip(test2, (x, y) => x*y)[0] != 10)
			{
				throw new TestAssertionException();
			}

			if (test2.Zip(test1, (x, y) => x*y).Count != 3)
			{
				throw new TestAssertionException();
			}

			if (test1.Zip(test2, (x, y) => x*y)[2] != 90)
			{
				throw new TestAssertionException();
			}

			//With index

			if (test1.Zip(test2, (x, y, z) => x*y*z).Count != 3)
			{
				throw new TestAssertionException();
			}

			if (test1.Zip(test2, (x, y, z) => x*y*z)[0] != 0)
			{
				throw new TestAssertionException();
			}

			if (test2.Zip(test1, (x, y, z) => x*y*z).Count != 3)
			{
				throw new TestAssertionException();
			}

			if (test1.Zip(test2, (x, y, z) => x*y*z)[2] != 180)
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
