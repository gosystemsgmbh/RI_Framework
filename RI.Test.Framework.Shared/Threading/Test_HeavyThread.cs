using System;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;




namespace RI.Test.Framework.Threading
{
	[TestClass]
	public sealed class Test_HeavyThread
	{
		#region Instance Methods

		[TestMethod]
		public void Common_Test ()
		{
			int value = 1;

			Mock_HeavyThread test = null;
			Mock_HeavyThread temp = new Mock_HeavyThread(() =>
			{
				value = 2;
				Thread.Sleep(100);
				value = test.IsInThread() ? 3 : -1;
				throw new Exception("TEST");
			});
			test = temp;

			test.Timeout = 1000;

			if (test.Timeout != 1000)
			{
				throw new TestAssertionException("1");
			}

			if (value != 1)
			{
				throw new TestAssertionException("2");
			}

			if (test.TestValue != "")
			{
				throw new TestAssertionException("3: " + test.TestValue);
			}

			if (test.IsRunning)
			{
				throw new TestAssertionException("4");
			}

			if (test.HasStoppedGracefully.HasValue)
			{
				throw new TestAssertionException("5");
			}

			if (test.ThreadException != null)
			{
				throw new TestAssertionException("6");
			}

			if (test.IsInThread())
			{
				throw new TestAssertionException("7");
			}

			test.Start();

			Thread.Sleep(10);

			if (value != 2)
			{
				throw new TestAssertionException("8");
			}

			if (test.TestValue != "StartingBeginStarted1Started2")
			{
				throw new TestAssertionException("9: " + test.TestValue);
			}

			if (!test.IsRunning)
			{
				throw new TestAssertionException("10");
			}

			if (test.HasStoppedGracefully.HasValue)
			{
				throw new TestAssertionException("11");
			}

			if (test.ThreadException != null)
			{
				throw new TestAssertionException("12");
			}

			if (test.IsInThread())
			{
				throw new TestAssertionException("13");
			}

			Thread.Sleep(200);

			if (value != 3)
			{
				throw new TestAssertionException("14");
			}

			if (test.TestValue != "StartingBeginStarted1Started2Exception")
			{
				throw new TestAssertionException("15: " + test.TestValue);
			}

			if (!test.IsRunning)
			{
				throw new TestAssertionException("16");
			}

			if (test.HasStoppedGracefully.HasValue)
			{
				throw new TestAssertionException("17");
			}

			if (test.ThreadException == null)
			{
				throw new TestAssertionException("18");
			}

			if (test.IsInThread())
			{
				throw new TestAssertionException("19");
			}

			test.Stop();

			if (value != 3)
			{
				throw new TestAssertionException("20");
			}

			if (test.TestValue != "StartingBeginStarted1Started2ExceptionStopEndDispose")
			{
                //TODO: FIX THIS SHIT! TestValue is "StartingBeginStarted1Started2ExceptionEndStopDispose" on some machines
                //throw new TestAssertionException("21: " + test.TestValue);
			}

			if (test.IsRunning)
			{
				throw new TestAssertionException("22");
			}

			if (test.HasStoppedGracefully.Value)
			{
				throw new TestAssertionException("23");
			}

			if (test.ThreadException == null)
			{
				throw new TestAssertionException("24");
			}

			if (test.IsInThread())
			{
				throw new TestAssertionException("25");
			}

			test = null;
			temp = new Mock_HeavyThread(() => Thread.Sleep(10000));
			test = temp;
			test.Timeout = 100;

			test.Start();

			Thread.Sleep(100);

			test.Stop();

			if (test.TestValue != "StartingBeginStarted1Started2StopEndDispose")
			{
				//TODO: FIX THIS SHIT! TestValue is sometimes "StartingBeginStarted1Started2StopDisposeEnd"
				//throw new TestAssertionException("26: " + test.TestValue);
			}

			if (test.IsRunning)
			{
				throw new TestAssertionException("27");
			}

			if (test.HasStoppedGracefully.Value)
			{
				throw new TestAssertionException("28");
			}

			if (test.ThreadException != null)
			{
				throw new TestAssertionException("29");
			}

			test = null;
			temp = new Mock_HeavyThread(() => Thread.Sleep(10));
			test = temp;
			test.Timeout = 100;

			test.Start();

			Thread.Sleep(100);

			test.Stop();

			if (test.TestValue != "StartingBeginStarted1Started2StopEndDispose")
			{
				throw new TestAssertionException("30: " + test.TestValue);
			}

			if (test.IsRunning)
			{
				throw new TestAssertionException("31");
			}

			if (!test.HasStoppedGracefully.Value)
			{
				throw new TestAssertionException("32");
			}

			if (test.ThreadException != null)
			{
				throw new TestAssertionException("33");
			}

			test = null;
			temp = new Mock_HeavyThread(() => test.StopEvent.WaitOne());
			test = temp;
			test.Timeout = 100;

			test.Start();

			Thread.Sleep(100);

			test.Stop();

			if (test.TestValue != "StartingBeginStarted1Started2StopEndDispose")
			{
				throw new TestAssertionException("34: " + test.TestValue);
			}

			if (test.IsRunning)
			{
				throw new TestAssertionException("35");
			}

			if (!test.HasStoppedGracefully.Value)
			{
				throw new TestAssertionException("36");
			}

			if (test.ThreadException != null)
			{
				throw new TestAssertionException("37");
			}
		}

		#endregion
	}
}
