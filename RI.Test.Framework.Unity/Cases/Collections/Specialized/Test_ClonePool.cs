using System;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Collections.Specialized;




namespace RI.Test.Framework.Cases.Collections.Specialized
{
	[SuppressMessage ("ReSharper", "InconsistentNaming")]
	[SuppressMessage ("ReSharper", "UnusedMember.Global")]
	public sealed class Test_ClonePool : TestModule
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
