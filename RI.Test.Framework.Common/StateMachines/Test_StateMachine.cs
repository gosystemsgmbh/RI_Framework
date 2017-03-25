using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Composition;
using RI.Framework.StateMachines;
using RI.Framework.Utilities.Threading;




namespace RI.Test.Framework.StateMachines
{
	[TestClass]
	public sealed class Test_StateMachine
	{
		//DispatcherServiceStateDispatcher (Unity)
		//MonoState (Unity)

		[TestMethod]
		public void WithoutDispatcher_Test ()
		{
			Mock_State.TestValue = "";

			StateMachineConfiguration config = new StateMachineConfiguration();
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
		public void WithDispatcher_Test ()
		{
			Mock_State.TestValue = "";
			
			HeavyThreadDispatcher htd = new HeavyThreadDispatcher();
			ThreadDispatcherStateDispatcher dispatcher = new ThreadDispatcherStateDispatcher(htd);
			StateMachineConfiguration config = new StateMachineConfiguration();
			config.Dispatcher = dispatcher;
			StateMachine test = new StateMachine(config);

			htd.Start();

			if (test.State != null)
			{
				throw new TestAssertionException();
			}
			if (Mock_State.TestValue != "")
			{
				throw new TestAssertionException();
			}

			test.Signal("#1");
			htd.DoProcessing();

			if (Mock_State.TestValue != "")
			{
				throw new TestAssertionException();
			}

			test.Transient(typeof(Mock_State_A));
			htd.DoProcessing();

			if (!(test.State is Mock_State_A))
			{
				throw new TestAssertionException();
			}
			if (Mock_State.TestValue != "iAeA")
			{
				throw new TestAssertionException();
			}

			test.Signal("#2");
			htd.DoProcessing();

			if (Mock_State.TestValue != "iAeA#2")
			{
				throw new TestAssertionException();
			}

			test.Transient(typeof(Mock_State_B));
			htd.DoProcessing();

			if (!(test.State is Mock_State_B))
			{
				throw new TestAssertionException();
			}
			if (Mock_State.TestValue != "iAeA#2iBlAeB")
			{
				throw new TestAssertionException();
			}

			test.Signal("#3");
			htd.DoProcessing();

			if (Mock_State.TestValue != "iAeA#2iBlAeB#3")
			{
				throw new TestAssertionException();
			}

			test.Transient(typeof(Mock_State_C));
			htd.DoProcessing();

			if (!(test.State is Mock_State_C))
			{
				throw new TestAssertionException();
			}
			if (Mock_State.TestValue != "iAeA#2iBlAeB#3iClBeC")
			{
				throw new TestAssertionException();
			}

			test.Signal("#4");
			htd.DoProcessing();

			if (Mock_State.TestValue != "iAeA#2iBlAeB#3iClBeC#4")
			{
				throw new TestAssertionException();
			}

			test.Transient(null);
			htd.DoProcessing();

			if (test.State != null)
			{
				throw new TestAssertionException();
			}
			if (Mock_State.TestValue != "iAeA#2iBlAeB#3iClBeC#4lC")
			{
				throw new TestAssertionException();
			}

			test.Signal("#5");
			htd.DoProcessing();

			if (Mock_State.TestValue != "iAeA#2iBlAeB#3iClBeC#4lC")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Inherited_Test()
		{
			Mock_State.TestValue = "";

			HeavyThreadDispatcher htd = new HeavyThreadDispatcher();
			ThreadDispatcherStateDispatcher dispatcher = new ThreadDispatcherStateDispatcher(htd);
			StateMachineConfiguration config = new StateMachineConfiguration();
			config.Dispatcher = dispatcher;
			StateMachine test = new StateMachine(config);

			htd.Start();

			if (test.State != null)
			{
				throw new TestAssertionException();
			}
			if (Mock_State.TestValue != "")
			{
				throw new TestAssertionException();
			}

			test.Signal(1);
			htd.DoProcessing();

			if (Mock_State.TestValue != "")
			{
				throw new TestAssertionException();
			}

			test.Transient<Mock_State_D1>();
			htd.DoProcessing();

			if (!(test.State is Mock_State_D1))
			{
				throw new TestAssertionException();
			}
			if (Mock_State.TestValue != "iDiD1eDeD1")
			{
				throw new TestAssertionException();
			}

			test.Signal(2);
			htd.DoProcessing();

			if (Mock_State.TestValue != "iDiD1eDeD1sDsD1")
			{
				throw new TestAssertionException();
			}

			test.Transient<Mock_State_D2>();
			htd.DoProcessing();

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
			htd.DoProcessing();

			if (Mock_State.TestValue != "sDsD2***iDiD3lDlD2eDeD3###iDiD1lDlD3eDeD1")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Signals_Test()
		{
			Mock_State.TestValue = "";

			StateMachineConfiguration config = new StateMachineConfiguration();
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
		public void CachingResolver_Test()
		{
			Mock_State.TestValue = "";

			StateMachineConfiguration config = new StateMachineConfiguration();
			config.Cache = new StateCache();
			StateMachine test = new StateMachine(config);

			config.EnableAutomaticCaching = false;

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

			config.EnableAutomaticCaching = false;
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
			if (!object.ReferenceEquals(f1, f2))
			{
				throw new TestAssertionException();
			}

			config.EnableAutomaticCaching = true;
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

			config.EnableAutomaticCaching = true;
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
			config.Resolver = new StateResolver(container);
			config.EnableAutomaticCaching = false;

			test.Transient(null);

			try
			{
				test.Transient<Mock_State_A>();
				throw new TestAssertionException();
			}
			catch (StateNotFoundException)
			{
			}

			container.AddExport(typeof(Mock_State_A), "Test", false);

			try
			{
				test.Transient<Mock_State_A>();
				throw new TestAssertionException();
			}
			catch (StateNotFoundException)
			{
			}

			container.AddExport(typeof(Mock_State_A), typeof(Mock_State_A), false);

			test.Transient<Mock_State_A>();
			Mock_State_A a1 = (Mock_State_A)test.State;
			test.Transient<Mock_State_A>();
			Mock_State_A a2 = (Mock_State_A)test.State;
			if (!object.ReferenceEquals(a1, a2))
			{
				throw new TestAssertionException();
			}

			container.AddExport(typeof(Mock_State_B), typeof(Mock_State_B), true);

			test.Transient<Mock_State_B>();
			Mock_State_B b1 = (Mock_State_B)test.State;
			test.Transient<Mock_State_B>();
			Mock_State_B b2 = (Mock_State_B)test.State;
			if (object.ReferenceEquals(b1, b2))
			{
				throw new TestAssertionException();
			}
		}
	}
}
