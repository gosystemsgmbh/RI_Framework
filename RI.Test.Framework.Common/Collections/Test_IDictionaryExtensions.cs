using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Collections;
using RI.Framework.Collections.Linq;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Comparison;




namespace RI.Test.Framework.Collections
{
	[TestClass]
	public sealed class Test_IDictionaryExtensions
	{
		#region Instance Methods

		[TestMethod]
		public void AddOrReplace_Test ()
		{
			Dictionary<string, int> dict = new Dictionary<string, int>();
			IDictionary<string, int> test = dict.AsDictionary();

			if (!test.AddOrReplace("1", 1))
			{
				throw new TestAssertionException();
			}

			if (test["1"] != 1)
			{
				throw new TestAssertionException();
			}

			if (test.AddOrReplace("1", 2))
			{
				throw new TestAssertionException();
			}

			if (test["1"] != 2)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ContainsKey_Test ()
		{
			Dictionary<string, int> dict = new Dictionary<string, int>();
			IDictionary<string, int> test = dict.AsDictionary();

			if (test.ContainsKey("a", new EqualityComparison<string>((x, y) => x.StartsWith(y, StringComparison.Ordinal))))
			{
				throw new TestAssertionException();
			}

			if (test.ContainsKey("a", (x, y) => x.StartsWith(y, StringComparison.Ordinal)))
			{
				throw new TestAssertionException();
			}

			dict.Add("a", 1);
			dict.Add("b", 2);
			dict.Add("c", 3);
			dict.Add("dd", 4);
			dict.Add("ee", 5);
			dict.Add("ff", 5);

			if (!test.ContainsKey("f", new EqualityComparison<string>((x, y) => x.StartsWith(y, StringComparison.Ordinal))))
			{
				throw new TestAssertionException();
			}

			if (!test.ContainsKey("f", (x, y) => x.StartsWith(y, StringComparison.Ordinal)))
			{
				throw new TestAssertionException();
			}

			if (test.ContainsKey("g", new EqualityComparison<string>((x, y) => x.StartsWith(y, StringComparison.Ordinal))))
			{
				throw new TestAssertionException();
			}

			if (test.ContainsKey("g", (x, y) => x.StartsWith(y, StringComparison.Ordinal)))
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ContainsValue_Test ()
		{
			Dictionary<string, int> dict = new Dictionary<string, int>();
			IDictionary<string, int> test = dict.AsDictionary();

			if (test.ContainsValue(1, new EqualityComparison<int>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (test.ContainsValue(1, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			dict.Add("a", 1);
			dict.Add("b", 1);
			dict.Add("c", 1);
			dict.Add("dd", 2);
			dict.Add("ee", 2);
			dict.Add("ff", 3);

			if (!test.ContainsValue(1))
			{
				throw new TestAssertionException();
			}

			if (!test.ContainsValue(2, new EqualityComparison<int>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (!test.ContainsValue(3, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}

			if (test.ContainsValue(4))
			{
				throw new TestAssertionException();
			}

			if (test.ContainsValue(4, new EqualityComparison<int>((x, y) => x == y)))
			{
				throw new TestAssertionException();
			}

			if (test.ContainsValue(4, (x, y) => x == y))
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void GetKeys_Test ()
		{
			Dictionary<string, int> dict = new Dictionary<string, int>();
			IDictionary<string, int> test = dict.AsDictionary();

			if (test.GetKeys().Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test.GetKeys(x => x.Key.ToInt32().HasValue).Count != 0)
			{
				throw new TestAssertionException();
			}

			dict.Add("1", 1);
			dict.Add("2", 2);
			dict.Add("3", 3);
			dict.Add("a", 4);
			dict.Add("b", 5);
			dict.Add("c", 6);

			if (test.GetKeys().Count != 6)
			{
				throw new TestAssertionException();
			}

			if (test.GetKeys()[0] != "1")
			{
				throw new TestAssertionException();
			}

			if (test.GetKeys()[5] != "c")
			{
				throw new TestAssertionException();
			}

			if (test.GetKeys(x => x.Key.ToInt32().HasValue).Count != 3)
			{
				throw new TestAssertionException();
			}

			if (test.GetKeys(x => x.Key.ToInt32().HasValue)[0] != "1")
			{
				throw new TestAssertionException();
			}

			if (test.GetKeys(x => x.Key.ToInt32().HasValue)[2] != "3")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void GetValues_Test ()
		{
			Dictionary<string, int> dict = new Dictionary<string, int>();
			IDictionary<string, int> test = dict.AsDictionary();

			if (test.GetValues().Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test.GetValues(x => x.Value > 3).Count != 0)
			{
				throw new TestAssertionException();
			}

			dict.Add("1", 1);
			dict.Add("2", 2);
			dict.Add("3", 3);
			dict.Add("a", 4);
			dict.Add("b", 5);
			dict.Add("c", 6);

			if (test.GetValues().Count != 6)
			{
				throw new TestAssertionException();
			}

			if (test.GetValues()[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (test.GetValues()[5] != 6)
			{
				throw new TestAssertionException();
			}

			if (test.GetValues(x => x.Value > 3).Count != 3)
			{
				throw new TestAssertionException();
			}

			if (test.GetValues(x => x.Value > 3)[0] != 4)
			{
				throw new TestAssertionException();
			}

			if (test.GetValues(x => x.Value > 3)[2] != 6)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void RemoveRange_Test ()
		{
			Dictionary<string, int> dict = new Dictionary<string, int>();
			IDictionary<string, int> test = dict.AsDictionary();

			if (test.RemoveRange(new[]
			                     {
				                     "1", "2"
			                     }) != 0)
			{
				throw new TestAssertionException();
			}

			dict.Add("1", 1);
			dict.Add("2", 2);
			dict.Add("3", 3);
			dict.Add("a", 4);
			dict.Add("b", 5);
			dict.Add("c", 6);

			if (test.RemoveRange(new[]
			                     {
				                     "1", "2"
			                     }) != 2)
			{
				throw new TestAssertionException();
			}

			if (dict.Count != 4)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void RemoveWhere_Test ()
		{
			Dictionary<string, int> dict = new Dictionary<string, int>();
			IDictionary<string, int> test = dict.AsDictionary();

			if (test.RemoveWhere(x => x.Key.ToInt32().HasValue || x.Value > 5).Count != 0)
			{
				throw new TestAssertionException();
			}

			dict.Add("1", 1);
			dict.Add("2", 2);
			dict.Add("3", 3);
			dict.Add("a", 4);
			dict.Add("b", 5);
			dict.Add("c", 6);

			List<KeyValuePair<string, int>> result = test.RemoveWhere(x => x.Key.ToInt32().HasValue || x.Value > 5);

			if (result.Count != 4)
			{
				throw new TestAssertionException();
			}

			if (result[0].Key != "1")
			{
				throw new TestAssertionException();
			}

			if (result[3].Key != "c")
			{
				throw new TestAssertionException();
			}

			if (dict["a"] != 4)
			{
				throw new TestAssertionException();
			}

			if (dict["b"] != 5)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Transfrom_Test ()
		{
			Dictionary<string, int> dict = new Dictionary<string, int>();
			IDictionary<string, int> test = dict.AsDictionary();

			if (test.Transform(x => x.Value + 10) != 0)
			{
				throw new TestAssertionException();
			}

			dict.Add("1", 1);
			dict.Add("2", 2);
			dict.Add("3", 3);
			dict.Add("a", 4);
			dict.Add("b", 5);
			dict.Add("c", 6);

			if (test.Transform(x => x.Value*10) != 6)
			{
				throw new TestAssertionException();
			}

			if (dict["1"] != 10)
			{
				throw new TestAssertionException();
			}

			if (dict["c"] != 60)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void TryAdd_Test ()
		{
			Dictionary<string, int> dict = new Dictionary<string, int>();
			IDictionary<string, int> test = dict.AsDictionary();

			if (!test.TryAdd("1", 1))
			{
				throw new TestAssertionException();
			}

			if (test["1"] != 1)
			{
				throw new TestAssertionException();
			}

			if (test.TryAdd("1", 2))
			{
				throw new TestAssertionException();
			}

			if (test["1"] != 1)
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
