using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Services;
using RI.Framework.Services.Dispatcher;
using RI.Framework.StateMachines;




namespace RI.Test.Framework.StateMachines.States
{
	public sealed class Test_MonoState : TestModule
	{
		#region Instance Methods

		[TestMethod]
		public void Test ()
		{
			Mock_MonoState.TestValue = "";

			DefaultStateMachineConfiguration config = new DefaultStateMachineConfiguration();
			StateMachine test = new StateMachine(config);

			if (test.State != null)
			{
				throw new TestAssertionException();
			}
			if (Mock_MonoState.TestValue != "")
			{
				throw new TestAssertionException();
			}

			test.Signal("#1");

			if (Mock_MonoState.TestValue != "")
			{
				throw new TestAssertionException();
			}

			test.Transient(typeof(Mock_MonoState_A));

			ServiceLocator.GetInstance<DispatcherService>().Dispatch(() =>
			{
				if (!(test.State is Mock_MonoState_A))
				{
					throw new TestAssertionException();
				}
				if (Mock_MonoState.TestValue != "iAeA")
				{
					throw new TestAssertionException();
				}

				test.Signal("#2");

				ServiceLocator.GetInstance<DispatcherService>().Dispatch(() =>
				{
					if (Mock_MonoState.TestValue != "iAeA#2")
					{
						throw new TestAssertionException();
					}

					test.Transient(typeof(Mock_MonoState_B));

					ServiceLocator.GetInstance<DispatcherService>().Dispatch(() =>
					{
						if (!(test.State is Mock_MonoState_B))
						{
							throw new TestAssertionException();
						}
						if (Mock_MonoState.TestValue != "iAeA#2iBlAeB")
						{
							throw new TestAssertionException();
						}

						test.Signal("#3");

						ServiceLocator.GetInstance<DispatcherService>().Dispatch(() =>
						{
							if (Mock_MonoState.TestValue != "iAeA#2iBlAeB#3")
							{
								throw new TestAssertionException();
							}

							test.Transient(typeof(Mock_MonoState_C));

							ServiceLocator.GetInstance<DispatcherService>().Dispatch(() =>
							{
								if (!(test.State is Mock_MonoState_C))
								{
									throw new TestAssertionException();
								}
								if (Mock_MonoState.TestValue != "iAeA#2iBlAeB#3iClBeC")
								{
									throw new TestAssertionException();
								}

								test.Signal("#4");

								ServiceLocator.GetInstance<DispatcherService>().Dispatch(() =>
								{
									if (Mock_MonoState.TestValue != "iAeA#2iBlAeB#3iClBeC#4")
									{
										throw new TestAssertionException();
									}

									test.Transient(null);

									ServiceLocator.GetInstance<DispatcherService>().Dispatch(() =>
									{
										if (test.State != null)
										{
											throw new TestAssertionException();
										}
										if (Mock_MonoState.TestValue != "iAeA#2iBlAeB#3iClBeC#4lC")
										{
											throw new TestAssertionException();
										}

										test.Signal("#5");

										ServiceLocator.GetInstance<DispatcherService>().Dispatch(() =>
										{
											if (Mock_MonoState.TestValue != "iAeA#2iBlAeB#3iClBeC#4lC")
											{
												throw new TestAssertionException();
											}
										});
									});
								});
							});
						});
					});
				});
			});
		}

		#endregion
	}
}
