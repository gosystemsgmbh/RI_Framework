using RI.Framework.Collections.Generic;

#if PLATFORM_NET
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
#if PLATFORM_UNITY
using RI.Test.Framework;
#endif




namespace RI.Test.Framework.Collections.Generic
{
	[TestClass]
	public sealed class Test_Warehouse
	{
		#region Instance Methods

		[TestMethod]
		public void Test ()
		{
			Warehouse<int> warehouse = new Warehouse<int>(10);

			if (warehouse.Size != 10)
			{
				throw new TestAssertionException();
			}

			if (warehouse.Free != 10)
			{
				throw new TestAssertionException();
			}

			if (warehouse.Storage.Length != 11)
			{
				throw new TestAssertionException();
			}

			if (warehouse.Reserve() != 10)
			{
				throw new TestAssertionException();
			}

			if (warehouse.Size != 10)
			{
				throw new TestAssertionException();
			}

			if (warehouse.Free != 9)
			{
				throw new TestAssertionException();
			}

			if (warehouse.Storage.Length != 11)
			{
				throw new TestAssertionException();
			}

			if (warehouse.Reserve() != 9)
			{
				throw new TestAssertionException();
			}

			if (warehouse.Size != 10)
			{
				throw new TestAssertionException();
			}

			if (warehouse.Free != 8)
			{
				throw new TestAssertionException();
			}

			if (warehouse.Storage.Length != 11)
			{
				throw new TestAssertionException();
			}

			warehouse.Release(10);

			if (warehouse.Size != 10)
			{
				throw new TestAssertionException();
			}

			if (warehouse.Free != 9)
			{
				throw new TestAssertionException();
			}

			if (warehouse.Storage.Length != 11)
			{
				throw new TestAssertionException();
			}

			warehouse.Release(9);

			if (warehouse.Size != 10)
			{
				throw new TestAssertionException();
			}

			if (warehouse.Free != 10)
			{
				throw new TestAssertionException();
			}

			if (warehouse.Storage.Length != 11)
			{
				throw new TestAssertionException();
			}

			if (warehouse.Reserve() != 9)
			{
				throw new TestAssertionException();
			}

			if (warehouse.Reserve() != 10)
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
