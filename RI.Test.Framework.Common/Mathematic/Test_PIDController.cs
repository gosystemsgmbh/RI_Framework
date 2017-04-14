using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Mathematic.Controllers;




namespace RI.Test.Framework.Mathematic
{
	[TestClass]
	public sealed class Test_PIDController
	{
		#region Instance Methods

		[TestMethod]
		public void Test ()
		{
			PIDController test = new PIDController();

			if (test.Loops != 0)
			{
				throw new TestAssertionException();
			}
			if (test.KP != 1.0f)
			{
				throw new TestAssertionException();
			}
			if (test.KI != 1.0f)
			{
				throw new TestAssertionException();
			}
			if (test.KD != 0.0f)
			{
				throw new TestAssertionException();
			}
			if (test.SetPoint != 0.0f)
			{
				throw new TestAssertionException();
			}
			if (test.ProcessVariable != 0.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Output != 0.0f)
			{
				throw new TestAssertionException();
			}
			if (test.OutputUnclamped != 0.0f)
			{
				throw new TestAssertionException();
			}
			if (test.OutputMin != float.MinValue)
			{
				throw new TestAssertionException();
			}
			if (test.OutputMax != float.MaxValue)
			{
				throw new TestAssertionException();
			}
			if (test.Error != 0.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Integral != 0.0f)
			{
				throw new TestAssertionException();
			}

			test.KP = 2.0f;
			test.KI = 3.0f;
			test.KD = 4.0f;
			test.OutputMin = 1.0f;
			test.OutputMax = 10.0f;

			test.ComputeNewSetPoint(10.0f, 0.0f, 1.0f);

			if (test.Loops != 1)
			{
				throw new TestAssertionException();
			}
			if (test.KP != 2.0f)
			{
				throw new TestAssertionException();
			}
			if (test.KI != 3.0f)
			{
				throw new TestAssertionException();
			}
			if (test.KD != 4.0f)
			{
				throw new TestAssertionException();
			}
			if (test.SetPoint != 10.0f)
			{
				throw new TestAssertionException();
			}
			if (test.ProcessVariable != 0.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Output != 10.0f)
			{
				throw new TestAssertionException();
			}
			if (test.OutputUnclamped != 90.0f)
			{
				throw new TestAssertionException();
			}
			if (test.OutputMin != 1.0f)
			{
				throw new TestAssertionException();
			}
			if (test.OutputMax != 10.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Error != 10.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Integral != 10.0f)
			{
				throw new TestAssertionException();
			}

			test.ComputeNewSetPoint(10.0f, 0.0f, 2.0f);

			if (test.Loops != 2)
			{
				throw new TestAssertionException();
			}
			if (test.KP != 2.0f)
			{
				throw new TestAssertionException();
			}
			if (test.KI != 3.0f)
			{
				throw new TestAssertionException();
			}
			if (test.KD != 4.0f)
			{
				throw new TestAssertionException();
			}
			if (test.SetPoint != 10.0f)
			{
				throw new TestAssertionException();
			}
			if (test.ProcessVariable != 0.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Output != 10.0f)
			{
				throw new TestAssertionException();
			}
			if (test.OutputUnclamped != 110.0f)
			{
				throw new TestAssertionException();
			}
			if (test.OutputMin != 1.0f)
			{
				throw new TestAssertionException();
			}
			if (test.OutputMax != 10.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Error != 10.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Integral != 30.0f)
			{
				throw new TestAssertionException();
			}

			test.ComputeNewSetPoint(5.0f, 5.0f, 3.0f);

			if (test.Loops != 3)
			{
				throw new TestAssertionException();
			}
			if (test.KP != 2.0f)
			{
				throw new TestAssertionException();
			}
			if (test.KI != 3.0f)
			{
				throw new TestAssertionException();
			}
			if (test.KD != 4.0f)
			{
				throw new TestAssertionException();
			}
			if (test.SetPoint != 5.0f)
			{
				throw new TestAssertionException();
			}
			if (test.ProcessVariable != 5.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Output != 10.0f)
			{
				throw new TestAssertionException();
			}
			if (test.OutputUnclamped != 76.6666641f)
			{
				throw new TestAssertionException();
			}
			if (test.OutputMin != 1.0f)
			{
				throw new TestAssertionException();
			}
			if (test.OutputMax != 10.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Error != 0.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Integral != 30.0f)
			{
				throw new TestAssertionException();
			}

			test.ComputeNewSetPoint(5.0f, 20.0f, 0.5f);

			if (test.Loops != 4)
			{
				throw new TestAssertionException();
			}
			if (test.KP != 2.0f)
			{
				throw new TestAssertionException();
			}
			if (test.KI != 3.0f)
			{
				throw new TestAssertionException();
			}
			if (test.KD != 4.0f)
			{
				throw new TestAssertionException();
			}
			if (test.SetPoint != 5.0f)
			{
				throw new TestAssertionException();
			}
			if (test.ProcessVariable != 20.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Output != 1.0f)
			{
				throw new TestAssertionException();
			}
			if (test.OutputUnclamped != -82.5)
			{
				throw new TestAssertionException();
			}
			if (test.OutputMin != 1.0f)
			{
				throw new TestAssertionException();
			}
			if (test.OutputMax != 10.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Error != -15.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Integral != 22.5f)
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
