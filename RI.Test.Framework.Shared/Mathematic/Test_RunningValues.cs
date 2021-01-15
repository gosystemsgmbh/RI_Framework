using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Mathematic;
using RI.Framework.Mathematic.Statistics;




namespace RI.Test.Framework.Mathematic
{
	[TestClass]
	public sealed class Test_RunningValues
	{
		#region Instance Methods

		[TestMethod]
		public void Test ()
		{
			RunningValues test = new RunningValues(3);

			if (test.Capacity != 3)
			{
				throw new TestAssertionException();
			}

			if (test.Count != 0)
			{
				throw new TestAssertionException();
			}
			if (test.Duration != 0.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Sum != 0.0f)
			{
				throw new TestAssertionException();
			}
			if (test.ArithmeticMean != 0.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Difference != 0.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Last != 0.0f)
			{
				throw new TestAssertionException();
			}
			if (!test.GetHistory().SequenceEqual(new float[] { }))
			{
				throw new TestAssertionException();
			}
			if (!test.GetTimesteps().SequenceEqual(new float[] { }))
			{
				throw new TestAssertionException();
			}

			test.Add(1.0f, 1.0f);

			if (test.Count != 1)
			{
				throw new TestAssertionException();
			}
			if (test.Duration != 1.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Sum != 1.0f)
			{
				throw new TestAssertionException();
			}
			if (test.ArithmeticMean != 1.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Difference != 1.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Last != 1.0f)
			{
				throw new TestAssertionException();
			}
			if (!test.GetHistory().SequenceEqual(new float[] {1.0f}))
			{
				throw new TestAssertionException();
			}
			if (!test.GetTimesteps().SequenceEqual(new float[] {1.0f}))
			{
				throw new TestAssertionException();
			}

			test.Add(2.0f, 2.0f);

			if (test.Count != 2)
			{
				throw new TestAssertionException();
			}
			if (test.Duration != 3.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Sum != 5.0f)
			{
				throw new TestAssertionException();
			}
			if (test.ArithmeticMean != (5.0f / 3.0f))
			{
				throw new TestAssertionException();
			}
			if (test.Difference != 3.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Last != 4.0f)
			{
				throw new TestAssertionException();
			}
			if (!test.GetHistory().SequenceEqual(new float[] {1.0f, 2.0f}))
			{
				throw new TestAssertionException();
			}
			if (!test.GetTimesteps().SequenceEqual(new float[] {1.0f, 2.0f}))
			{
				throw new TestAssertionException();
			}

			test.Add(4.0f, 3.0f);

			if (test.Count != 3)
			{
				throw new TestAssertionException();
			}
			if (test.Duration != 6.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Sum != 17.0f)
			{
				throw new TestAssertionException();
			}
			if (test.ArithmeticMean != (17.0f / 6.0f))
			{
				throw new TestAssertionException();
			}
			if (test.Difference != 8.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Last != 12.0f)
			{
				throw new TestAssertionException();
			}
			if (!test.GetHistory().SequenceEqual(new float[] {1.0f, 2.0f, 4.0f}))
			{
				throw new TestAssertionException();
			}
			if (!test.GetTimesteps().SequenceEqual(new float[] {1.0f, 2.0f, 3.0f}))
			{
				throw new TestAssertionException();
			}

			test.Add(8.0f, 4.0f);

			if (test.Count != 3)
			{
				throw new TestAssertionException();
			}
			if (test.Duration != 9.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Sum != 48.0f)
			{
				throw new TestAssertionException();
			}
			if (test.ArithmeticMean != (48.0f / 9.0f))
			{
				throw new TestAssertionException();
			}
			if (test.Difference != 20.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Last != 32.0f)
			{
				throw new TestAssertionException();
			}
			if (!test.GetHistory().SequenceEqual(new float[] {2.0f, 4.0f, 8.0f}))
			{
				throw new TestAssertionException();
			}
			if (!test.GetTimesteps().SequenceEqual(new float[] {2.0f, 3.0f, 4.0f}))
			{
				throw new TestAssertionException();
			}

			StatisticValues stats = test.GetStatistics();

			if (!stats.Values.SequenceEqual(new[] {2.0, 4.0, 8.0}))
			{
				throw new TestAssertionException();
			}
			if (!stats.Timesteps.SequenceEqual(new[] {2.0, 3.0, 4.0}))
			{
				throw new TestAssertionException();
			}
			if (!stats.WeightedValues.SequenceEqual(new[] {4.0, 12.0, 32.0}))
			{
				throw new TestAssertionException();
			}

			test.Reset();

			if (test.Count != 0)
			{
				throw new TestAssertionException();
			}
			if (test.Duration != 0.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Sum != 0.0f)
			{
				throw new TestAssertionException();
			}
			if (test.ArithmeticMean != 0.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Difference != 0.0f)
			{
				throw new TestAssertionException();
			}
			if (test.Last != 0.0f)
			{
				throw new TestAssertionException();
			}
			if (!test.GetHistory().SequenceEqual(new float[] { }))
			{
				throw new TestAssertionException();
			}
			if (!test.GetTimesteps().SequenceEqual(new float[] { }))
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
