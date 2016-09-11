using System;

using RI.Framework.Utilities;

using Microsoft.VisualStudio.TestTools.UnitTesting;




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
