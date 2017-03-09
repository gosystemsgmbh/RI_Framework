using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Utilities;




namespace RI.Test.Framework.Utilities
{
	[TestClass]
	public sealed class Test_SingleExtensions
	{
		[TestMethod]
		public void Test()
		{
			//--------------
			// Numeric value
			//--------------

			if (1.2f.IsInfinity())
			{
				throw new TestAssertionException();
			}

			if (1.2f.IsNan())
			{
				throw new TestAssertionException();
			}

			if (1.2f.IsNanOrInfinity())
			{
				throw new TestAssertionException();
			}

			if (!1.2f.IsNumber())
			{
				throw new TestAssertionException();
			}

			//----
			// NaN
			//----

			if (float.NaN.IsInfinity())
			{
				throw new TestAssertionException();
			}

			if (!float.NaN.IsNan())
			{
				throw new TestAssertionException();
			}

			if (!float.NaN.IsNanOrInfinity())
			{
				throw new TestAssertionException();
			}

			if (float.NaN.IsNumber())
			{
				throw new TestAssertionException();
			}

			//----------
			// +infinity
			//----------

			if (!float.PositiveInfinity.IsInfinity())
			{
				throw new TestAssertionException();
			}

			if (float.PositiveInfinity.IsNan())
			{
				throw new TestAssertionException();
			}

			if (!float.PositiveInfinity.IsNanOrInfinity())
			{
				throw new TestAssertionException();
			}

			if (float.PositiveInfinity.IsNumber())
			{
				throw new TestAssertionException();
			}

			//----------
			// -infinity
			//----------

			if (!float.NegativeInfinity.IsInfinity())
			{
				throw new TestAssertionException();
			}

			if (float.NegativeInfinity.IsNan())
			{
				throw new TestAssertionException();
			}

			if (!float.NegativeInfinity.IsNanOrInfinity())
			{
				throw new TestAssertionException();
			}

			if (float.NegativeInfinity.IsNumber())
			{
				throw new TestAssertionException();
			}

			//--------------
			// Default value
			//--------------

			if (1.2f.GetValueOrDefault() != 1.2f)
			{
				throw new TestAssertionException();
			}

			if (float.NaN.GetValueOrDefault() != 0.0f)
			{
				throw new TestAssertionException();
			}

			if (1.2f.GetValueOrDefault(1.5f) != 1.2f)
			{
				throw new TestAssertionException();
			}

			if (float.NaN.GetValueOrDefault(1.5f) != 1.5f)
			{
				throw new TestAssertionException();
			}
		}
	}
}
