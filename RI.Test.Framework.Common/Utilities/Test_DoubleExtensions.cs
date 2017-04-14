using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Utilities;




namespace RI.Test.Framework.Utilities
{
	[TestClass]
	public sealed class Test_DoubleExtensions
	{
		#region Instance Methods

		[TestMethod]
		public void Test ()
		{
			//--------------
			// Numeric value
			//--------------

			if (1.2.IsInfinity())
			{
				throw new TestAssertionException();
			}

			if (1.2.IsNan())
			{
				throw new TestAssertionException();
			}

			if (1.2.IsNanOrInfinity())
			{
				throw new TestAssertionException();
			}

			if (!1.2.IsNumber())
			{
				throw new TestAssertionException();
			}

			//----
			// NaN
			//----

			if (double.NaN.IsInfinity())
			{
				throw new TestAssertionException();
			}

			if (!double.NaN.IsNan())
			{
				throw new TestAssertionException();
			}

			if (!double.NaN.IsNanOrInfinity())
			{
				throw new TestAssertionException();
			}

			if (double.NaN.IsNumber())
			{
				throw new TestAssertionException();
			}

			//----------
			// +infinity
			//----------

			if (!double.PositiveInfinity.IsInfinity())
			{
				throw new TestAssertionException();
			}

			if (double.PositiveInfinity.IsNan())
			{
				throw new TestAssertionException();
			}

			if (!double.PositiveInfinity.IsNanOrInfinity())
			{
				throw new TestAssertionException();
			}

			if (double.PositiveInfinity.IsNumber())
			{
				throw new TestAssertionException();
			}

			//----------
			// -infinity
			//----------

			if (!double.NegativeInfinity.IsInfinity())
			{
				throw new TestAssertionException();
			}

			if (double.NegativeInfinity.IsNan())
			{
				throw new TestAssertionException();
			}

			if (!double.NegativeInfinity.IsNanOrInfinity())
			{
				throw new TestAssertionException();
			}

			if (double.NegativeInfinity.IsNumber())
			{
				throw new TestAssertionException();
			}

			//--------------
			// Default value
			//--------------

			if (1.2.GetValueOrDefault() != 1.2)
			{
				throw new TestAssertionException();
			}

			if (double.NaN.GetValueOrDefault() != 0.0)
			{
				throw new TestAssertionException();
			}

			if (1.2.GetValueOrDefault(1.5) != 1.2)
			{
				throw new TestAssertionException();
			}

			if (double.NaN.GetValueOrDefault(1.5) != 1.5)
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
