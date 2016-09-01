using System;

using RI.Framework.Collections.Specialized;

#if PLATFORM_NET
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
#if PLATFORM_UNITY
using RI.Test.Framework;
#endif




namespace RI.Test.Framework.Collections.Specialized
{
	[TestClass]
	public sealed class Test_ClonePool
	{
		#region Instance Methods

		[TestMethod]
		public void Test ()
		{
			Version prototype = new Version(1, 2, 3, 4);
			ClonePool<Version> pool = new ClonePool<Version>(prototype);

			if (pool.Count != 0)
			{
				throw new TestAssertionException();
			}

			if (!object.ReferenceEquals(prototype, pool.Prototype))
			{
				throw new TestAssertionException();
			}

			if (object.ReferenceEquals(prototype, pool.Take()))
			{
				throw new TestAssertionException();
			}

			if (!pool.Take().Equals(prototype))
			{
				throw new TestAssertionException();
			}

			pool = new ClonePool<Version>(prototype, 10);

			if (pool.Count != 10)
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
