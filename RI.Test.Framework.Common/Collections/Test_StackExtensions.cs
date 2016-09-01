using System.Collections.Generic;

using RI.Framework.Collections;

#if PLATFORM_NET
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
#if PLATFORM_UNITY
using RI.Test.Framework;
#endif




namespace RI.Test.Framework.Collections
{
	[TestClass]
	public sealed class Test_StackExtensions
	{
		#region Instance Methods

		[TestMethod]
		public void PeekAll_Test ()
		{
			Stack<int> test = new Stack<int>();
			test.Push(1);
			test.Push(2);
			test.Push(3);

			List<int> result = test.PeekAll();

			if (result[0] != 3)
			{
				throw new TestAssertionException();
			}

			if (result[1] != 2)
			{
				throw new TestAssertionException();
			}

			if (result[2] != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Count != 3)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void PopAll_Test ()
		{
			Stack<int> test = new Stack<int>();
			test.Push(1);
			test.Push(2);
			test.Push(3);

			List<int> result = test.PopAll();

			if (result[0] != 3)
			{
				throw new TestAssertionException();
			}

			if (result[1] != 2)
			{
				throw new TestAssertionException();
			}

			if (result[2] != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Count != 0)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void PushRange_Test ()
		{
			Stack<int> test = new Stack<int>();

			if (test.PushRange(new[]
			                   {
				                   1, 2, 3
			                   }) != 3)
			{
				throw new TestAssertionException();
			}

			if (test.Count != 3)
			{
				throw new TestAssertionException();
			}

			if (test.Pop() != 3)
			{
				throw new TestAssertionException();
			}

			if (test.Pop() != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Pop() != 1)
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
