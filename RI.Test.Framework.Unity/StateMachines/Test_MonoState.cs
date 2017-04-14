using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Services;
using RI.Framework.Services.Dispatcher;
using RI.Framework.StateMachines;




namespace RI.Test.Framework.Cases.StateMachines
{
	public sealed class Test_MonoState : TestModule
	{
		#region Instance Methods

		[TestMethod]
		public void Test ()
		{
			Mock_State.TestValue = "";

			DefaultStateMachineConfiguration config = new DefaultStateMachineConfiguration();
			StateMachine test = new StateMachine(config);

			if (test.State != null)
			{
				throw new TestAssertionException();
			}
			if (Mock_State.TestValue != "")
			{
				throw new TestAssertionException();
			}

			test.Signal("#1");

			if (Mock_State.TestValue != "")
			{
				throw new TestAssertionException();
			}

			test.Transient(typeof(Mock_State_A));

			ServiceLocator.GetInstance<IDispatcherService>().Dispatch(() =>
			{
				if (!(test.State is Mock_State_A))
				{
					throw new TestAssertionException();
				}
				if (Mock_State.TestValue != "iAeA")
				{
					throw new TestAssertionException();
				}

				test.Signal("#2");

				ServiceLocator.GetInstance<IDispatcherService>().Dispatch(() =>
				{
					if (Mock_State.TestValue != "iAeA#2")
					{
						throw new TestAssertionException();
					}

					test.Transient(typeof(Mock_State_B));

					ServiceLocator.GetInstance<IDispatcherService>().Dispatch(() =>
					{
						if (!(test.State is Mock_State_B))
						{
							throw new TestAssertionException();
						}
						if (Mock_State.TestValue != "iAeA#2iBlAeB")
						{
							throw new TestAssertionException();
						}

						test.Signal("#3");

						ServiceLocator.GetInstance<IDispatcherService>().Dispatch(() =>
						{
							if (Mock_State.TestValue != "iAeA#2iBlAeB#3")
							{
								throw new TestAssertionException();
							}

							test.Transient(typeof(Mock_State_C));

							ServiceLocator.GetInstance<IDispatcherService>().Dispatch(() =>
							{
								if (!(test.State is Mock_State_C))
								{
									throw new TestAssertionException();
								}
								if (Mock_State.TestValue != "iAeA#2iBlAeB#3iClBeC")
								{
									throw new TestAssertionException();
								}

								test.Signal("#4");

								ServiceLocator.GetInstance<IDispatcherService>().Dispatch(() =>
								{
									if (Mock_State.TestValue != "iAeA#2iBlAeB#3iClBeC#4")
									{
										throw new TestAssertionException();
									}

									test.Transient(null);

									ServiceLocator.GetInstance<IDispatcherService>().Dispatch(() =>
									{
										if (test.State != null)
										{
											throw new TestAssertionException();
										}
										if (Mock_State.TestValue != "iAeA#2iBlAeB#3iClBeC#4lC")
										{
											throw new TestAssertionException();
										}

										test.Signal("#5");

										ServiceLocator.GetInstance<IDispatcherService>().Dispatch(() =>
										{
											if (Mock_State.TestValue != "iAeA#2iBlAeB#3iClBeC#4lC")
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
