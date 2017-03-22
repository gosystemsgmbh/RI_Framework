using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Collections.Virtualization;

namespace RI.Test.Framework.Collections.Virtualization
{
	[TestClass]
	public sealed class Test_VirtualizationCollection
	{
		[TestMethod]
		public void General_Test ()
		{
			Mock_IItemsProvider mock = new Mock_IItemsProvider();
			VirtualizationCollection<int> test = new VirtualizationCollection<int>(3, 100, mock);

			if (test.CacheTime != 100)
			{
				throw new TestAssertionException();
			}

			if (test.PageSize != 3)
			{
				throw new TestAssertionException();
			}

			if (test.Count != 10)
			{
				throw new TestAssertionException();
			}

			if (mock.GetCounter != 0)
			{
				throw new TestAssertionException();
			}

			test.ClearCache();

			if (test.Count != 10)
			{
				throw new TestAssertionException();
			}

			if (mock.GetCounter != 0)
			{
				throw new TestAssertionException();
			}

			int temp = test[0];

			if (mock.GetCounter != 2)
			{
				throw new TestAssertionException();
			}

			temp = test[9];

			if (mock.GetCounter != 5)
			{
				throw new TestAssertionException();
			}

			temp = test[5];

			if (mock.GetCounter != 5)
			{
				throw new TestAssertionException();
			}

			temp = test[5];

			if (mock.GetCounter != 5)
			{
				throw new TestAssertionException();
			}

			test.ClearCache();
			
			temp = test[5];

			if (mock.GetCounter != 8)
			{
				throw new TestAssertionException();
			}

			test.Dispose();

			try
			{
				test.ClearCache();
				throw new TestAssertionException();
			}
			catch (ObjectDisposedException)
			{
			}
		}

		[TestMethod]
		public void Timeout_Test ()
		{
			Mock_IItemsProvider mock = new Mock_IItemsProvider();
			VirtualizationCollection<int> test = new VirtualizationCollection<int>(3, 100, mock);

			int temp = test[0];

			if (mock.GetCounter != 2)
			{
				throw new TestAssertionException();
			}

			temp = test[0];

			if (mock.GetCounter != 2)
			{
				throw new TestAssertionException();
			}

			Thread.Sleep(200);

			temp = test[0];

			if (mock.GetCounter != 4)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void List_Test ()
		{
			IList<int> test = new VirtualizationCollection<int>(3, 0, new Mock_IItemsProvider());

			if (test.Count != 10)
			{
				throw new TestAssertionException();
			}

			if (!test.IsReadOnly)
			{
				throw new TestAssertionException();
			}

			if (test[0] != 1)
			{
				throw new TestAssertionException();
			}

			if (test[9] != 10)
			{
				throw new TestAssertionException();
			}

			if (!test.Contains(1))
			{
				throw new TestAssertionException();
			}

			if (!test.Contains(10))
			{
				throw new TestAssertionException();
			}

			if (test.Contains(0))
			{
				throw new TestAssertionException();
			}

			if (test.IndexOf(0) != -1)
			{
				throw new TestAssertionException();
			}

			if (test.IndexOf(1) != 0)
			{
				throw new TestAssertionException();
			}

			if (test.IndexOf(10) != 9)
			{
				throw new TestAssertionException();
			}

			try
			{
				test.Add(0);
				throw new TestAssertionException();
			}
			catch (NotSupportedException)
			{
			}

			try
			{
				test.Insert(0, 0);
				throw new TestAssertionException();
			}
			catch (NotSupportedException)
			{
			}

			try
			{
				test.Clear();
				throw new TestAssertionException();
			}
			catch (NotSupportedException)
			{
			}

			try
			{
				test.Remove(0);
				throw new TestAssertionException();
			}
			catch (NotSupportedException)
			{
			}

			try
			{
				test.Remove(1);
				throw new TestAssertionException();
			}
			catch (NotSupportedException)
			{
			}

			try
			{
				test.RemoveAt(0);
				throw new TestAssertionException();
			}
			catch (NotSupportedException)
			{
			}

			try
			{
				test.RemoveAt(99);
				throw new TestAssertionException();
			}
			catch (NotSupportedException)
			{
			}

			try
			{
				test[0] = 0;
				throw new TestAssertionException();
			}
			catch (NotSupportedException)
			{
			}
		}
	}
}
