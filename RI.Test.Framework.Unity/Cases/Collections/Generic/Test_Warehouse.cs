using System.Diagnostics.CodeAnalysis;

using RI.Framework.Collections.Generic;




namespace RI.Test.Framework.Cases.Collections.Generic
{
	[SuppressMessage ("ReSharper", "InconsistentNaming")]
	[SuppressMessage ("ReSharper", "UnusedMember.Global")]
	public sealed class Test_Warehouse : TestModule
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
