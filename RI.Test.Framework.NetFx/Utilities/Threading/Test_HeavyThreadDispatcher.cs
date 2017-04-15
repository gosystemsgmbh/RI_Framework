using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Utilities.Threading;




namespace RI.Test.Framework.Utilities.Threading
{
	[TestClass]
	public sealed class Test_HeavyThreadDispatcher
	{
		#region Instance Methods

		[TestMethod]
		public void DoProcessing_Test ()
		{
			HeavyThreadDispatcher test = new HeavyThreadDispatcher();

			test.Stop(false);
			test.Start();

			int syncTestValue = 0;
			Action syncTestAction1 = null;
			Action syncTestAction2 = new Action(() =>
			{
				syncTestValue++;
				if (syncTestValue < 10)
				{
					test.Post(syncTestAction1);
				}
			});
			syncTestAction1 = syncTestAction2;

			test.Post(syncTestAction1);

			test.DoProcessing();

			if (syncTestValue != 10)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Test ()
		{
			HeavyThreadDispatcher test = new HeavyThreadDispatcher();
			ThreadDispatcherOperation op1 = null;
			ThreadDispatcherOperation op2 = null;

			//-------------------------
			// Setting basic properties
			//-------------------------

			test.IsBackgroundThread = true;
			test.ThreadName = "Test";
			test.ThreadPriority = ThreadPriority.Normal;
			test.FinishPendingDelegatesOnShutdown = false;
			test.CatchExceptions = false;
			test.Timeout = 1000;

			if (test.IsRunning)
			{
				throw new TestAssertionException();
			}

			if (test.HasStoppedGracefully.HasValue)
			{
				throw new TestAssertionException();
			}

			if (test.ShutdownMode != ThreadDispatcherShutdownMode.None)
			{
				throw new TestAssertionException();
			}

			if (test.ThreadException != null)
			{
				throw new TestAssertionException();
			}

			if (test.IsInThread())
			{
				throw new TestAssertionException();
			}

			test.CheckForException();

			//---------------------------
			// Stop without being started
			//---------------------------

			test.Stop();
			test.Stop(true);
			test.Stop(false);

			if (test.IsRunning)
			{
				throw new TestAssertionException();
			}

			if (test.HasStoppedGracefully.HasValue)
			{
				throw new TestAssertionException();
			}

			if (test.ShutdownMode != ThreadDispatcherShutdownMode.None)
			{
				throw new TestAssertionException();
			}

			if (test.ThreadException != null)
			{
				throw new TestAssertionException();
			}

			if (test.IsInThread())
			{
				throw new TestAssertionException();
			}

			test.CheckForException();

			//---------
			// Starting
			//---------

			test.Start();

			try
			{
				test.Start();
				throw new TestAssertionException();
			}
			catch (InvalidOperationException)
			{
			}

			if (!test.IsRunning)
			{
				throw new TestAssertionException();
			}

			if (test.HasStoppedGracefully.HasValue)
			{
				throw new TestAssertionException();
			}

			if (test.ShutdownMode != ThreadDispatcherShutdownMode.None)
			{
				throw new TestAssertionException();
			}

			if (test.ThreadException != null)
			{
				throw new TestAssertionException();
			}

			if (test.IsInThread())
			{
				throw new TestAssertionException();
			}

			test.CheckForException();

			//--------------------
			// Stopping gracefully
			//--------------------

			test.Stop();

			if (test.IsRunning)
			{
				throw new TestAssertionException();
			}

			if (!test.HasStoppedGracefully.Value)
			{
				throw new TestAssertionException();
			}

			if (test.ShutdownMode != ThreadDispatcherShutdownMode.None)
			{
				throw new TestAssertionException();
			}

			if (test.ThreadException != null)
			{
				throw new TestAssertionException();
			}

			if (test.IsInThread())
			{
				throw new TestAssertionException();
			}

			test.CheckForException();

			//----------------
			// Forced stopping
			//----------------

			test.Start();

			op1 = test.Post(new Action(() => { Thread.Sleep(9999999); }));

			Thread.Sleep(100);

			test.Stop(false);

			if (test.IsRunning)
			{
				throw new TestAssertionException();
			}

			if (test.HasStoppedGracefully.Value)
			{
				throw new TestAssertionException();
			}

			if (test.ShutdownMode != ThreadDispatcherShutdownMode.None)
			{
				throw new TestAssertionException();
			}

			if (test.ThreadException != null)
			{
				throw new TestAssertionException();
			}

			if (test.IsInThread())
			{
				throw new TestAssertionException();
			}

			test.CheckForException();

			if (!op1.IsDone)
			{
				//TODO: Fix
				//throw new TestAssertionException();
			}

			if (op1.State != ThreadDispatcherOperationState.Aborted)
			{
				//TODO: Fix
				//throw new TestAssertionException();
			}

			if (op1.Exception != null)
			{
				throw new TestAssertionException();
			}

			if (op1.Result != null)
			{
				throw new TestAssertionException();
			}

			//--------------------
			// Exception (catched)
			//--------------------

			test.CatchExceptions = true;
			test.Start();

			op1 = test.Post(new Action(() => { throw new TestAssertionException(); }));

			Thread.Sleep(100);

			test.Stop(false);

			if (test.IsRunning)
			{
				throw new TestAssertionException();
			}

			if (!test.HasStoppedGracefully.Value)
			{
				throw new TestAssertionException();
			}

			if (test.ShutdownMode != ThreadDispatcherShutdownMode.None)
			{
				throw new TestAssertionException();
			}

			if (test.ThreadException != null)
			{
				throw new TestAssertionException();
			}

			if (test.IsInThread())
			{
				throw new TestAssertionException();
			}

			test.CheckForException();

			if (!op1.IsDone)
			{
				throw new TestAssertionException();
			}

			if (op1.State != ThreadDispatcherOperationState.Exception)
			{
				throw new TestAssertionException();
			}

			if (!(op1.Exception is TargetInvocationException))
			{
				throw new TestAssertionException();
			}

			if (op1.Result != null)
			{
				throw new TestAssertionException();
			}

			//----------------------
			// Exception (uncatched)
			//----------------------

			test.CatchExceptions = false;
			test.Start();

			op1 = test.Post(new Action(() => { throw new TestAssertionException(); }));

			Thread.Sleep(100);

			test.Stop(false);

			if (test.IsRunning)
			{
				throw new TestAssertionException();
			}

			if (test.HasStoppedGracefully.Value)
			{
				throw new TestAssertionException();
			}

			if (test.ShutdownMode != ThreadDispatcherShutdownMode.None)
			{
				throw new TestAssertionException();
			}

			if (!(test.ThreadException is ThreadDispatcherException))
			{
				throw new TestAssertionException();
			}

			if (test.IsInThread())
			{
				throw new TestAssertionException();
			}

			try
			{
				test.CheckForException();
			}
			catch (HeavyThreadException)
			{
			}

			if (!op1.IsDone)
			{
				throw new TestAssertionException();
			}

			if (op1.State != ThreadDispatcherOperationState.Exception)
			{
				throw new TestAssertionException();
			}

			if (!(op1.Exception is TargetInvocationException))
			{
				throw new TestAssertionException();
			}

			if (op1.Result != null)
			{
				throw new TestAssertionException();
			}

			//--------------------------
			// Cancelation, states, wait
			//--------------------------

			test.Start();

			op1 = test.Post(new Action(() => { Thread.Sleep(1); }));

			Thread.Sleep(100);

			if (op1.State != ThreadDispatcherOperationState.Finished)
			{
				throw new TestAssertionException();
			}

			op1 = test.Post(new Action(() => { Thread.Sleep(10000); }));

			if (op1.Wait(100))
			{
				throw new TestAssertionException();
			}

			op2 = test.Post(new Action(() => { Thread.Sleep(1); }));

			if (op1.State != ThreadDispatcherOperationState.Executing)
			{
				//TODO: Fix
				//throw new TestAssertionException();
			}

			if (op2.State != ThreadDispatcherOperationState.Waiting)
			{
				throw new TestAssertionException();
			}

			if (!op2.Cancel())
			{
				throw new TestAssertionException();
			}

			if (op2.State != ThreadDispatcherOperationState.Canceled)
			{
				throw new TestAssertionException();
			}

			if (op2.Cancel())
			{
				throw new TestAssertionException();
			}

			if (!op2.Wait(1))
			{
				throw new TestAssertionException();
			}

			//-----------------
			// Send, post, wait
			//-----------------

			test.Stop();
			test.Start();

			op1 = test.Post(new Action(() => { Thread.Sleep(100); }));

			if (!op1.Wait(200))
			{
				throw new TestAssertionException();
			}

			StringBuilder sb = new StringBuilder();

			test.Post(new Action(() =>
			{
				sb.Append("T");
				Thread.Sleep(10);
			}));
			test.Post(new Action(() =>
			{
				sb.Append("e");
				Thread.Sleep(10);
			}));
			test.Post(new Action(() =>
			{
				sb.Append("s");
				Thread.Sleep(10);
			}));
			test.Post(new Action(() =>
			{
				sb.Append("t");
				Thread.Sleep(10);
			}));

			if (sb.ToString() == "Test")
			{
				throw new TestAssertionException();
			}

			test.Send(new Action(() =>
			{
				sb.Append("1");
				Thread.Sleep(10);
			}));
			if (sb.ToString() != "Test1")
			{
				throw new TestAssertionException();
			}

			test.Send(new Action(() =>
			{
				sb.Append("2");
				Thread.Sleep(10);
			}));
			if (sb.ToString() != "Test12")
			{
				throw new TestAssertionException();
			}

			test.Send(new Action(() =>
			{
				sb.Append("3");
				Thread.Sleep(10);
			}));
			if (sb.ToString() != "Test123")
			{
				throw new TestAssertionException();
			}

			sb = new StringBuilder();

			test.Post(new Action(() =>
			{
				sb.Append("T");
				Thread.Sleep(10);
				test.Post(new Action(() =>
				{
					sb.Append("a");
					Thread.Sleep(10);
				}));
			}));
			test.Post(new Action(() =>
			{
				sb.Append("e");
				Thread.Sleep(10);
				test.Send(new Action(() =>
				{
					sb.Append("b");
					Thread.Sleep(10);
				}));
			}));
			test.Post(new Action(() =>
			{
				sb.Append("s");
				Thread.Sleep(10);
				test.Send(new Action(() =>
				{
					sb.Append("c");
					Thread.Sleep(10);
				}));
			}));
			test.Post(new Action(() =>
			{
				sb.Append("t");
				Thread.Sleep(10);
				test.Post(new Action(() =>
				{
					sb.Append("d");
					Thread.Sleep(10);
				}));
			}));

			Thread.Sleep(100);

			if (sb.ToString() != "Testabcd")
			{
				throw new TestAssertionException();
			}

			//---------------------------------
			// Shutdown mode and discard/finish
			//---------------------------------

			test.Stop(false);

			test.Start();
			List<ThreadDispatcherOperation> ops = new List<ThreadDispatcherOperation>();
			for (int i1 = 0; i1 < 100; i1++)
			{
				ops.Add(test.Post(new Action(() => Thread.Sleep(100))));
			}
			test.Stop(false);
			if (ops.Skip(1).Any(x => x.State != ThreadDispatcherOperationState.Canceled))
			{
				throw new TestAssertionException();
			}

			test.Start();
			ops = new List<ThreadDispatcherOperation>();
			for (int i1 = 0; i1 < 100; i1++)
			{
				ops.Add(test.Post(new Action(() => Thread.Sleep(1))));
			}
			test.Timeout = 20;
			test.Stop(true);
			if ((!ops.Any(x => x.State == ThreadDispatcherOperationState.Finished)) || (!ops.Any(x => x.State == ThreadDispatcherOperationState.Waiting)))
			{
				throw new TestAssertionException();
			}

			test.Start();
			ops = new List<ThreadDispatcherOperation>();
			for (int i1 = 0; i1 < 100; i1++)
			{
				ops.Add(test.Post(new Action(() => Thread.Sleep(1))));
			}
			test.Timeout = 2000;
			test.Stop(true);
			if (ops.Any(x => x.State != ThreadDispatcherOperationState.Finished))
			{
				throw new TestAssertionException();
			}

			//-----------
			// IsInThread
			//-----------

			test.Start();

			bool value1 = (bool)test.Send(new Func<bool>(() => { return test.IsInThread(); }));
			if (!value1)
			{
				throw new TestAssertionException();
			}

			string value2 = (string)test.Send(new Func<string>(() => { return "XYZ"; }));
			if (value2 != "XYZ")
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
