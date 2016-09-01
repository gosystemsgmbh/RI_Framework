using System;

using RI.Framework.Utilities;

#if PLATFORM_NET
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
#if PLATFORM_UNITY
using RI.Test.Framework;
#endif




namespace RI.Test.Framework.Utilities
{
	[TestClass]
	public sealed class Test_TimeSpanExtensions
	{
		#region Instance Methods

		[TestMethod]
		public void ToSortableString_Test ()
		{
			if (( new TimeSpan(1, 14, 30, 50, 333) ).ToSortableString() != "1143050333")
			{
				throw new TestAssertionException();
			}

			if (( new TimeSpan(1, 14, 30, 50, 333) ).ToSortableString('-') != "1-14-30-50-333")
			{
				throw new TestAssertionException();
			}

			if (( new TimeSpan(1, 14, 30, 50, 333) ).ToSortableString("@@") != "1@@14@@30@@50@@333")
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
