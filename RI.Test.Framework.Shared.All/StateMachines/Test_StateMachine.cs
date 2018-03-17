#if PLATFORM_NETFX

using RI.Framework.Threading.Dispatcher;
#endif
using System;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.ComponentModel;
using RI.Framework.Composition;
using RI.Framework.StateMachines;
using RI.Framework.StateMachines.Dispatchers;
using RI.Framework.StateMachines.Resolvers;
using RI.Framework.Utilities.ObjectModel;
using RI.Test.Framework.StateMachines.Dispatchers;
using RI.Test.Framework.StateMachines.States;
#if PLATFORM_UNITY
using RI.Framework.Services.Dispatcher;
using RI.Framework.Services;




#endif


namespace RI.Test.Framework.StateMachines
{
	[TestClass]
	public sealed class Test_StateMachine
	{
		#region Instance Methods

		[TestMethod]
		public void CachingResolver_Test ()
		{
			Mock_State.TestValue = "";

			StateMachineConfiguration config = new DefaultStateMachineConfiguration();
			config.Dispatcher = new Mock_Dispatcher();

			StateMachine test = new StateMachine(config);

			config.CachingEnabled = false;

			test.Transient<Mock_State_F>();
			Mock_State_F f1 = (Mock_State_F)test.State;
			test.Transient<Mock_State_G>();
			Mock_State_G g1 = (Mock_State_G)test.State;
			test.Transient<Mock_State_F>();
			Mock_State_F f2 = (Mock_State_F)test.State;
			test.Transient<Mock_State_G>();
			Mock_State_G g2 = (Mock_State_G)test.State;

			if (object.ReferenceEquals(g1, g2))
			{
				throw new TestAssertionException();
			}
			if (object.ReferenceEquals(f1, f2))
			{
				throw new TestAssertionException();
			}

			config.CachingEnabled = false;
			config.Cache.AddState(new Mock_State_F());

			test.Transient<Mock_State_F>();
			f1 = (Mock_State_F)test.State;
			test.Transient<Mock_State_G>();
			g1 = (Mock_State_G)test.State;
			test.Transient<Mock_State_F>();
			f2 = (Mock_State_F)test.State;
			test.Transient<Mock_State_G>();
			g2 = (Mock_State_G)test.State;

			if (object.ReferenceEquals(g1, g2))
			{
				throw new TestAssertionException();
			}
			if (object.ReferenceEquals(f1, f2))
			{
				throw new TestAssertionException();
			}

			config.CachingEnabled = true;
			config.Cache.Clear();

			test.Transient<Mock_State_F>();
			f1 = (Mock_State_F)test.State;
			test.Transient<Mock_State_G>();
			g1 = (Mock_State_G)test.State;
			test.Transient<Mock_State_F>();
			f2 = (Mock_State_F)test.State;
			test.Transient<Mock_State_G>();
			g2 = (Mock_State_G)test.State;

			if (!object.ReferenceEquals(g1, g2))
			{
				throw new TestAssertionException();
			}
			if (object.ReferenceEquals(f1, f2))
			{
				throw new TestAssertionException();
			}

			config.CachingEnabled = true;
			config.Cache.Clear();
			config.Cache.AddState(new Mock_State_F());

			test.Transient<Mock_State_F>();
			f1 = (Mock_State_F)test.State;
			test.Transient<Mock_State_G>();
			g1 = (Mock_State_G)test.State;
			test.Transient<Mock_State_F>();
			f2 = (Mock_State_F)test.State;
			test.Transient<Mock_State_G>();
			g2 = (Mock_State_G)test.State;

			if (!object.ReferenceEquals(g1, g2))
			{
				throw new TestAssertionException();
			}
			if (!object.ReferenceEquals(f1, f2))
			{
				throw new TestAssertionException();
			}

			CompositionContainer container = new CompositionContainer();
			config.Resolver = new CompositionContainerStateResolver(container);
			config.CachingEnabled = false;

			test.Transient(null);

			try
			{
				test.Transient<Mock_State_A>();
				throw new TestAssertionException();
			}
			catch (StateNotFoundException)
			{
			}

			container.AddType(typeof(Mock_State_A), "Test", false);

			try
			{
				test.Transient<Mock_State_A>();
				throw new TestAssertionException();
			}
			catch (StateNotFoundException)
			{
			}

			container.AddType(typeof(Mock_State_A), typeof(Mock_State_A), false);

			test.Transient<Mock_State_A>();
			Mock_State_A a1 = (Mock_State_A)test.State;
			test.Transient<Mock_State_A>();
			Mock_State_A a2 = (Mock_State_A)test.State;
			if (!object.ReferenceEquals(a1, a2))
			{
				throw new TestAssertionException();
			}

			container.AddType(typeof(Mock_State_B), typeof(Mock_State_B), true);

			test.Transient<Mock_State_B>();
			Mock_State_B b1 = (Mock_State_B)test.State;
			test.Transient<Mock_State_B>();
			Mock_State_B b2 = (Mock_State_B)test.State;
			if (object.ReferenceEquals(b1, b2))
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Inherited_Test ()
		{
			Mock_State.TestValue = "";

#if PLATFORM_NETFX
			HeavyThreadDispatcher htd = new HeavyThreadDispatcher();
			ThreadDispatcherStateDispatcher dispatcher = new ThreadDispatcherStateDispatcher(htd);

			htd.Start();

			Action<Action> cont = new Action<Action>(x =>
			{
				htd.DoProcessing();
				x();
			});
#endif
#if PLATFORM_UNITY
			DispatcherServiceStateDispatcher dispatcher = new DispatcherServiceStateDispatcher(ServiceLocator.GetInstance<IDispatcherService>());

			Action<Action> cont = new Action<Action>(x => dispatcher.DispatcherService.Dispatch(DispatcherPriority.Idle, x));
#endif

			StateMachineConfiguration config = new DefaultStateMachineConfiguration();
			config.Dispatcher = dispatcher;
			config.CachingEnabled = false;
			StateMachine test = new StateMachine(config);

			if (test.State != null)
			{
				throw new TestAssertionException();
			}
			if (Mock_State.TestValue != "")
			{
				throw new TestAssertionException();
			}

			test.Signal(1);
			cont(() =>
			{
				if (Mock_State.TestValue != "")
				{
					throw new TestAssertionException();
				}

				test.Transient<Mock_State_D1>();
				cont(() =>
				{
					if (!(test.State is Mock_State_D1))
					{
						throw new TestAssertionException();
					}
					if (Mock_State.TestValue != "iDiD1eDeD1")
					{
						throw new TestAssertionException();
					}

					test.Signal(2);

					cont(() =>
					{
						if (Mock_State.TestValue != "iDiD1eDeD1sDsD1")
						{
							throw new TestAssertionException();
						}

						test.Transient<Mock_State_D2>();

						cont(() =>
						{
							if (!(test.State is Mock_State_D2))
							{
								throw new TestAssertionException();
							}
							if (Mock_State.TestValue != "iDiD1eDeD1sDsD1iDiD2lDlD1eDeD2")
							{
								throw new TestAssertionException();
							}

							Mock_State.TestValue = "";

							test.Signal(3);

							cont(() =>
							{
								if (Mock_State.TestValue != "sDsD2***iDiD3lDlD2eDeD3###iDiD1lDlD3eDeD1")
								{
									throw new TestAssertionException();
								}
							});
						});
					});
				});
			});
		}

		[TestMethod]
		public void Signals_Test ()
		{
			Mock_State.TestValue = "";

			StateMachineConfiguration config = new DefaultStateMachineConfiguration();
			config.Dispatcher = new Mock_Dispatcher();

			StateMachine test = new StateMachine(config);

			test.Transient<Mock_State_E>();

			if (!(test.State is Mock_State_E))
			{
				throw new TestAssertionException();
			}
			if (Mock_State.TestValue != "")
			{
				throw new TestAssertionException();
			}

			test.Signal("1");

			if (Mock_State.TestValue != "1")
			{
				throw new TestAssertionException();
			}

			test.Signal("2");

			if (Mock_State.TestValue != "12")
			{
				throw new TestAssertionException();
			}

			test.Signal("3");

			if (Mock_State.TestValue != "123")
			{
				throw new TestAssertionException();
			}

			test.Signal("X");

			if (Mock_State.TestValue != "123X")
			{
				throw new TestAssertionException();
			}

			test.Signal("4");

			if (Mock_State.TestValue != "123X")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void WithDispatcher_Test ()
		{
			Mock_State.TestValue = "";

#if PLATFORM_NETFX
			HeavyThreadDispatcher htd = new HeavyThreadDispatcher();
			ThreadDispatcherStateDispatcher dispatcher = new ThreadDispatcherStateDispatcher(htd);

			htd.Start();

			Action<Action> cont = new Action<Action>(x =>
			{
				htd.DoProcessing();
				x();
			});
#endif
#if PLATFORM_UNITY
			DispatcherServiceStateDispatcher dispatcher = new DispatcherServiceStateDispatcher(ServiceLocator.GetInstance<IDispatcherService>());

			Action<Action> cont = new Action<Action>(x => dispatcher.DispatcherService.Dispatch(DispatcherPriority.Idle, x));
#endif

			StateMachineConfiguration config = new DefaultStateMachineConfiguration();
			config.Dispatcher = dispatcher;

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

			cont(() =>
			{
				if (Mock_State.TestValue != "")
				{
					throw new TestAssertionException();
				}

				test.Transient(typeof(Mock_State_A));

				cont(() =>
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

					cont(() =>
					{
						if (Mock_State.TestValue != "iAeA#2")
						{
							throw new TestAssertionException();
						}

						test.Transient(typeof(Mock_State_B));

						cont(() =>
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

							cont(() =>
							{
								if (Mock_State.TestValue != "iAeA#2iBlAeB#3")
								{
									throw new TestAssertionException();
								}

								test.Transient(typeof(Mock_State_C));

								cont(() =>
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

									cont(() =>
									{
										if (Mock_State.TestValue != "iAeA#2iBlAeB#3iClBeC#4")
										{
											throw new TestAssertionException();
										}

										test.Transient(null);

										cont(() =>
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

											cont(() =>
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
			});
		}

		[TestMethod]
		public void WithoutDispatcher_Test ()
		{
			Mock_State.TestValue = "";

			StateMachineConfiguration config = new DefaultStateMachineConfiguration();
			config.Dispatcher = new Mock_Dispatcher();

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

			if (!(test.State is Mock_State_A))
			{
				throw new TestAssertionException();
			}
			if (Mock_State.TestValue != "iAeA")
			{
				throw new TestAssertionException();
			}

			test.Signal("#2");

			if (Mock_State.TestValue != "iAeA#2")
			{
				throw new TestAssertionException();
			}

			test.Transient(typeof(Mock_State_B));

			if (!(test.State is Mock_State_B))
			{
				throw new TestAssertionException();
			}
			if (Mock_State.TestValue != "iAeA#2iBlAeB")
			{
				throw new TestAssertionException();
			}

			test.Signal("#3");

			if (Mock_State.TestValue != "iAeA#2iBlAeB#3")
			{
				throw new TestAssertionException();
			}

			test.Transient(typeof(Mock_State_C));

			if (!(test.State is Mock_State_C))
			{
				throw new TestAssertionException();
			}
			if (Mock_State.TestValue != "iAeA#2iBlAeB#3iClBeC")
			{
				throw new TestAssertionException();
			}

			test.Signal("#4");

			if (Mock_State.TestValue != "iAeA#2iBlAeB#3iClBeC#4")
			{
				throw new TestAssertionException();
			}

			test.Transient(null);

			if (test.State != null)
			{
				throw new TestAssertionException();
			}
			if (Mock_State.TestValue != "iAeA#2iBlAeB#3iClBeC#4lC")
			{
				throw new TestAssertionException();
			}

			test.Signal("#5");

			if (Mock_State.TestValue != "iAeA#2iBlAeB#3iClBeC#4lC")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void StateVariables_Test ()
		{
			StateMachineConfiguration config = new DefaultStateMachineConfiguration();
			config.Dispatcher = new Mock_Dispatcher();
			config.CachingEnabled = true;

			StateMachine test = new StateMachine(config);

			test.Transient<Mock_State_H1>();

			if (((Mock_State_H1)test.State).TestValue1 != null)
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_H1)test.State).TestValue2 != null)
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_H1)test.State).TestValue3 != null)
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_H1)test.State).TestValue4 != null)
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_H1)test.State).TestValue5 != null)
			{
				throw new TestAssertionException();
			}

			((Mock_State_H1)test.State).TestValue1 = "T11";
			((Mock_State_H1)test.State).TestValue2 = "T12";
			((Mock_State_H1)test.State).TestValue3 = "T13";
			((Mock_State_H1)test.State).TestValue4 = "T14";
			((Mock_State_H1)test.State).TestValue5 = "T15";

			if (((Mock_State_H1)test.State).TestValue1 != "T11")
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_H1)test.State).TestValue2 != "T12")
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_H1)test.State).TestValue3 != "T13")
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_H1)test.State).TestValue4 != "T14")
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_H1)test.State).TestValue5 != "T15")
			{
				throw new TestAssertionException();
			}

			test.Transient<Mock_State_H2>();

			if (((Mock_State_H2)test.State).TestValue1 != null)
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_H2)test.State).TestValue2 != "T12")
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_H2)test.State).TestValue3 != null)
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_H2)test.State).TestValue4 != "T14")
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_H2)test.State).TestValue5 != null)
			{
				throw new TestAssertionException();
			}

			((Mock_State_H2)test.State).TestValue1 = "T21";
			((Mock_State_H2)test.State).TestValue3 = "T23";
			((Mock_State_H2)test.State).TestValue5 = "T25";

			test.Transient<Mock_State_I>();

			if (((Mock_State_I)test.State).TestValue1 != null)
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_I)test.State).TestValue2 != "T12")
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_I)test.State).TestValue3 != null)
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_I)test.State).TestValue4 != "T14")
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_I)test.State).TestValue5 != null)
			{
				throw new TestAssertionException();
			}

			((Mock_State_I)test.State).TestValue1 = "T31";
			((Mock_State_I)test.State).TestValue2 = "T32";
			((Mock_State_I)test.State).TestValue3 = "T33";
			((Mock_State_I)test.State).TestValue4 = "T34";
			((Mock_State_I)test.State).TestValue5 = "T35";

			test.Transient<Mock_State_H1>();

			if (((Mock_State_H1)test.State).TestValue1 != "T11")
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_H1)test.State).TestValue2 != "T32")
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_H1)test.State).TestValue3 != "T13")
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_H1)test.State).TestValue4 != "T34")
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_H1)test.State).TestValue5 != null)
			{
				throw new TestAssertionException();
			}

			test.Transient<Mock_State_H2>();

			if (((Mock_State_H2)test.State).TestValue1 != "T21")
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_H2)test.State).TestValue2 != "T32")
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_H2)test.State).TestValue3 != "T23")
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_H2)test.State).TestValue4 != "T34")
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_H2)test.State).TestValue5 != null)
			{
				throw new TestAssertionException();
			}

			test.Transient<Mock_State_I>();

			if (((Mock_State_I)test.State).TestValue1 != null)
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_I)test.State).TestValue2 != "T32")
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_I)test.State).TestValue3 != null)
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_I)test.State).TestValue4 != "T34")
			{
				throw new TestAssertionException();
			}
			if (((Mock_State_I)test.State).TestValue5 != null)
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
