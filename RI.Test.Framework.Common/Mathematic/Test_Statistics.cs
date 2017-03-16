using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Mathematic;
using RI.Framework.Utilities;




namespace RI.Test.Framework.Mathematic
{
	[TestClass]
	public sealed class Test_Statistics
	{
		[TestMethod]
		public void Test()
		{
			Statistics stats1 = new Statistics(new double[0]);

			if (stats1.Count != 0)
			{
				throw new TestAssertionException();
			}
			if (!stats1.Min.IsNan())
			{
				throw new TestAssertionException();
			}
			if (!stats1.Max.IsNan())
			{
				throw new TestAssertionException();
			}
			if (!stats1.Median.IsNan())
			{
				throw new TestAssertionException();
			}
			if (stats1.Average != 0.0)
			{
				throw new TestAssertionException();
			}
			if (stats1.RMS != 0.0)
			{
				throw new TestAssertionException();
			}
			if (stats1.Sigma != 0.0)
			{
				throw new TestAssertionException();
			}
			if (stats1.SquareSum != 0.0)
			{
				throw new TestAssertionException();
			}
			if (stats1.Sum != 0.0)
			{
				throw new TestAssertionException();
			}

			Statistics stats2 = new Statistics(new []{2.0});

			if (stats2.Count != 1)
			{
				throw new TestAssertionException();
			}
			if (stats2.Min != 2.0)
			{
				throw new TestAssertionException();
			}
			if (stats2.Max != 2.0)
			{
				throw new TestAssertionException();
			}
			if (!stats2.Median.IsNan())
			{
				throw new TestAssertionException();
			}
			if (stats2.Average != 2.0)
			{
				throw new TestAssertionException();
			}
			if (stats2.RMS != 2.0)
			{
				throw new TestAssertionException();
			}
			if (stats2.Sigma != 0.0)
			{
				throw new TestAssertionException();
			}
			if (stats2.SquareSum != 4.0)
			{
				throw new TestAssertionException();
			}
			if (stats2.Sum != 2.0)
			{
				throw new TestAssertionException();
			}

			Statistics stats3 = new Statistics(new[] { 1.0, 3.0 });

			if (stats3.Count != 2)
			{
				throw new TestAssertionException();
			}
			if (stats3.Min != 1.0)
			{
				throw new TestAssertionException();
			}
			if (stats3.Max != 3.0)
			{
				throw new TestAssertionException();
			}
			if (stats3.Median != 2.0)
			{
				throw new TestAssertionException();
			}
			if (stats3.Average != 2.0)
			{
				throw new TestAssertionException();
			}
			if (stats3.RMS != Math.Sqrt(5.0))
			{
				throw new TestAssertionException();
			}
			if (stats3.Sigma != 1.0)
			{
				throw new TestAssertionException();
			}
			if (stats3.SquareSum != 10.0)
			{
				throw new TestAssertionException();
			}
			if (stats3.Sum != 4.0)
			{
				throw new TestAssertionException();
			}

			Statistics stats4 = new Statistics(new[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 20.0 });

			if (stats4.Count != 10)
			{
				throw new TestAssertionException();
			}
			if (stats4.Min != 1.0)
			{
				throw new TestAssertionException();
			}
			if (stats4.Max != 20.0)
			{
				throw new TestAssertionException();
			}
			if (stats4.Median != 5.5)
			{
				throw new TestAssertionException();
			}
			if (stats4.Average != 6.5)
			{
				throw new TestAssertionException();
			}
			if (stats4.RMS != Math.Sqrt(68.5))
			{
				throw new TestAssertionException();
			}
			if (stats4.Sigma != 5.123475382979799)
			{
				throw new TestAssertionException();
			}
			if (stats4.SquareSum != 685.0)
			{
				throw new TestAssertionException();
			}
			if (stats4.Sum != 65.0)
			{
				throw new TestAssertionException();
			}
		}
	}
}
