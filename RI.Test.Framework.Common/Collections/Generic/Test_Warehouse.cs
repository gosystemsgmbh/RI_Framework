using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Collections.Generic;
using RI.Framework.Collections.Linq;




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

			if (warehouse.Reserve() != 1)
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

			if (warehouse.Reserve() != 2)
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

			warehouse.Release(1);

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

			warehouse.Release(2);

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

			if (warehouse.Reserve() != 2)
			{
				throw new TestAssertionException();
			}

			if (warehouse.Reserve() != 1)
			{
				throw new TestAssertionException();
			}

			warehouse = new Warehouse<int>(1);

			if (warehouse.Reserve() != 1)
			{
				throw new TestAssertionException();
			}

			if (warehouse.Reserve() != 0)
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
