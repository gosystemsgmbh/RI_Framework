using System;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.ComponentModel;
using RI.Framework.Services.Dispatcher;




namespace RI.Test.Framework.Services.Dispatcher
{
	public sealed class Test_DispatcherService : TestModule
	{
		#region Instance Properties/Indexer

		private string BroadcastTestValue { get; set; }
		private Action TestContinuation { get; set; }

		#endregion




		#region Instance Methods

		[TestMethod]
		public void Test_Broadcast ()
		{
			DispatcherService test = ServiceLocator.GetInstance<DispatcherService>();

			this.BroadcastTestValue = string.Empty;

			test.Broadcast(DispatcherPriority.Now, "0");
			test.Broadcast(DispatcherPriority.Idle, "1");

			test.RegisterReceiver<string>(this.BroadcastTestReceiver);

			test.Broadcast(DispatcherPriority.Idle, "2");
			test.Broadcast(DispatcherPriority.Later, "3");
			test.Broadcast(DispatcherPriority.Frame, "4");
			test.Broadcast(DispatcherPriority.Now, "5");

			test.Dispatch(DispatcherPriority.Idle, () => { test.UnregisterReceiver<string>(this.BroadcastTestReceiver); });

			test.Broadcast(DispatcherPriority.Now, "6");
			test.Broadcast(DispatcherPriority.Idle, "7");

			test.Dispatch(DispatcherPriority.Idle, () =>
			{
				if (this.BroadcastTestValue != "564312")
				{
					throw new TestAssertionException();
				}
				this.TestContinuation();
			});
		}

		[TestMethod]
		public void Test_Cascading ()
		{
			DispatcherService test = ServiceLocator.GetInstance<DispatcherService>();

			string testValue = string.Empty;

			test.Dispatch(DispatcherPriority.Now, () =>
			{
				testValue += "#";
				test.Dispatch(DispatcherPriority.Frame, (a) =>
				{
					testValue += a.ToString();
					test.Dispatch(DispatcherPriority.Later, (b, c) =>
					{
						testValue += b.ToString() + c.ToString();
						test.Dispatch(DispatcherPriority.Idle, (d, e, f) =>
						{
							testValue += d.ToString() + e.ToString() + f.ToString();
							test.Dispatch(DispatcherPriority.Background, (g, h, i, j) =>
							{
								testValue += g.ToString() + h.ToString() + i.ToString() + j.ToString();
								test.DispatchFunc(DispatcherPriority.Background, () =>
								{
									testValue += "@";
									test.DispatchFunc(DispatcherPriority.Idle, (k) =>
									{
										testValue += k.ToString();
										test.DispatchFunc(DispatcherPriority.Later, (l, m) =>
										{
											testValue += l.ToString() + m.ToString();
											test.DispatchFunc(DispatcherPriority.Frame, (n, o, p) =>
											{
												testValue += n.ToString() + o.ToString() + p.ToString();
												test.DispatchFunc(DispatcherPriority.Now, (q, r, s, t) =>
												{
													testValue += q.ToString() + r.ToString() + s.ToString() + t.ToString();
													if (testValue != "#12345678910@11121314151617181920")
													{
														throw new TestAssertionException();
													}
													this.TestContinuation();
													return "E";
												}, 17, 18, 19, 20);
												return "D";
											}, 14, 15, 16);
											return "C";
										}, 12, 13);
										return "B";
									}, 11);
									return "A";
								});
							}, 7, 8, 9, 10);
						}, 4, 5, 6);
					}, 2, 3);
				}, 1);
			});

			if (testValue != "#")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Test_Stacking ()
		{
			DispatcherService test = ServiceLocator.GetInstance<DispatcherService>();

			string testValue = string.Empty;

			test.Dispatch(DispatcherPriority.Idle, () =>
			{
				testValue += "#";
				if (testValue != "456231#")
				{
					throw new TestAssertionException();
				}
				this.TestContinuation();
			});

			test.Dispatch(DispatcherPriority.Later, (a) => { testValue += a.ToString(); }, 1);

			test.Dispatch(DispatcherPriority.Frame, (b, c) => { testValue += b.ToString() + c.ToString(); }, 2, 3);

			test.Dispatch(DispatcherPriority.Now, (d, e, f) => { testValue += d.ToString() + e.ToString() + f.ToString(); }, 4, 5, 6);
		}

		private void BroadcastTestReceiver (string value)
		{
			this.BroadcastTestValue += value;
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
