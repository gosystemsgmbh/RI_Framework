using System;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Services;
using RI.Framework.Services.Dispatcher;




namespace RI.Test.Framework.Cases.Services.Dispatcher
{
	public sealed class Test_DispatcherOperation : TestModule
	{
		#region Instance Properties/Indexer

		private Action TestContinuation { get; set; }

		#endregion




		#region Instance Methods

		[TestMethod]
		public void Test_1 ()
		{
			IDispatcherService test = ServiceLocator.GetInstance<IDispatcherService>();

			IDispatcherOperation op1 = test.Dispatch(() => { throw new TestAssertionException(); });
			IDispatcherOperation op2 = test.Dispatch(() => { throw new TestAssertionException(); });
			IDispatcherOperation op3 = test.Dispatch(() => { throw new TestAssertionException(); });

			if (op1.Status != DispatcherStatus.Queued)
			{
				throw new TestAssertionException();
			}
			if (op2.Status != DispatcherStatus.Queued)
			{
				throw new TestAssertionException();
			}
			if (op3.Status != DispatcherStatus.Queued)
			{
				throw new TestAssertionException();
			}

			test.CancelAllOperations();

			if (op1.Status != DispatcherStatus.Canceled)
			{
				throw new TestAssertionException();
			}
			if (op2.Status != DispatcherStatus.Canceled)
			{
				throw new TestAssertionException();
			}
			if (op3.Status != DispatcherStatus.Canceled)
			{
				throw new TestAssertionException();
			}

			op1 = test.DispatchFunc(DispatcherPriority.Now, () => "abc");
			if (op1.Status != DispatcherStatus.Processed)
			{
				throw new TestAssertionException();
			}
			if ((string)op1.Result != "abc")
			{
				throw new TestAssertionException();
			}

			string testValue = string.Empty;

			DateTime start = DateTime.UtcNow;
			op1 = test.Dispatch(DispatcherPriority.Frame, () =>
			{
				DateTime now = DateTime.UtcNow;
				if (now.Subtract(start).TotalMilliseconds < 1000)
				{
					throw new TestAssertionException();
				}

				op2 = test.Dispatch(DispatcherPriority.Frame, () => { });
				op2.Reschedule(1000).Timeout(100).OnFinished((x, y) =>
				{
					testValue += x.Status;
					if (testValue != "ProcessedTimeout")
					{
						throw new TestAssertionException();
					}
					this.TestContinuation();
				});
			});
			op1.Reschedule(1100).OnFinished((x, y) => { testValue += x.Status; });
		}

		[TestMethod]
		public void Test_2 ()
		{
			IDispatcherService test = ServiceLocator.GetInstance<IDispatcherService>();

			IDispatcherOperation op = test.Dispatch(DispatcherPriority.Later, () => { });
			op.OnFinished((x, y) =>
			{
				if (x.Status != DispatcherStatus.Canceled)
				{
					throw new TestAssertionException();
				}
				this.TestContinuation();
			});
			test.Dispatch(DispatcherPriority.Frame, () => { op.Cancel(); });
		}

		[TestMethod]
		public void Test_3 ()
		{
			IDispatcherService test = ServiceLocator.GetInstance<IDispatcherService>();

			string testValue = string.Empty;

			test.Dispatch(DispatcherPriority.Idle, () => { testValue += "A"; }).Reschedule(1000);
			test.Dispatch(DispatcherPriority.Later, () => { testValue += "B"; }).Reschedule(1000);
			test.Dispatch(DispatcherPriority.Frame, () => { testValue += "C"; }).Reschedule(1000);

			test.Dispatch(DispatcherPriority.Idle, () =>
			{
				if (testValue != "CBA")
				{
					throw new TestAssertionException();
				}
				this.TestContinuation();
			}).Reschedule(1000);
		}

		#endregion




		#region Overrides

		public override void InvokeTestMethod (MethodInfo method, Action testContinuation)
		{
			this.TestContinuation = testContinuation;
			base.InvokeTestMethod(method, null);
		}

		#endregion
	}
}
