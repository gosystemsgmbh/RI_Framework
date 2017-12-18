using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Utilities;




namespace RI.Test.Framework.Utilities
{
	[TestClass]
	public sealed class Test_DateTimeExtensions
	{
		#region Instance Methods

		[TestMethod]
		public void ToIso8601_Test ()
		{
			if ((new DateTime(2016, 2, 1, 14, 30, 50, 333)).ToIso8601() != "2016-02-01T14:30:50.3330000")
			{
				throw new TestAssertionException();
			}

			if ((new DateTimeOffset(2016, 2, 1, 14, 30, 50, 333, TimeSpan.FromHours(2))).ToIso8601() != "2016-02-01T14:30:50.3330000+02:00")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToSortableString_Test ()
		{
			if ((new DateTime(2016, 2, 1, 14, 30, 50, 333)).ToSortableString() != "20160201143050333")
			{
				throw new TestAssertionException();
			}

			if ((new DateTime(2016, 2, 1, 14, 30, 50, 333)).ToSortableString('-') != "2016-02-01-14-30-50-333")
			{
				throw new TestAssertionException();
			}

			if ((new DateTime(2016, 2, 1, 14, 30, 50, 333)).ToSortableString("@@") != "2016@@02@@01@@14@@30@@50@@333")
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
