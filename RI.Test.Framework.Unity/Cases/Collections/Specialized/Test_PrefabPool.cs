using RI.Framework.Collections.Specialized;

using UnityEngine;

using Microsoft.VisualStudio.TestTools.UnitTesting;




namespace RI.Test.Framework.Cases.Collections.Specialized
{
	public sealed class Test_PrefabPool : TestModule
	{
		#region Instance Methods

		[TestMethod]
		public void Test ()
		{
			GameObject prefab = new GameObject();
			PrefabPool pool = new PrefabPool(prefab);

			if (pool.AutoActivate)
			{
				throw new TestAssertionException();
			}

			if (pool.AutoDeactivate)
			{
				throw new TestAssertionException();
			}

			if (pool.Count != 0)
			{
				throw new TestAssertionException();
			}

			if (!object.ReferenceEquals(prefab, pool.Prefab))
			{
				throw new TestAssertionException();
			}

			if (object.ReferenceEquals(prefab, pool.Take()))
			{
				throw new TestAssertionException();
			}

			pool = new PrefabPool(prefab, 10);

			pool.AutoActivate = true;
			pool.AutoDeactivate = true;

			if (pool.Count != 10)
			{
				throw new TestAssertionException();
			}

			GameObject test = pool.Take();

			if (!test.activeSelf)
			{
				throw new TestAssertionException();
			}

			pool.Return(test);

			if (test.activeSelf)
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
