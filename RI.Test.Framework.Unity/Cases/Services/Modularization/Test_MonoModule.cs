using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Services;
using RI.Framework.Services.Modularization;




namespace RI.Test.Framework.Cases.Services.Modularization
{
	public sealed class Test_MonoModule : TestModule
	{
		[TestMethod]
		public void Test ()
		{
			ModuleService test = new ModuleService();
			Mock_ModuleA module1 = ServiceLocator.GetInstance<Mock_ModuleA>();
			Mock_ModuleB module2 = ServiceLocator.GetInstance<Mock_ModuleB>();

			if (test.Modules.Count() != 0)
			{
				throw new TestAssertionException();
			}
			if (test.IsInitialized)
			{
				throw new TestAssertionException();
			}

			test.AddModule(module1);

			if (test.Modules.Count() != 1)
			{
				throw new TestAssertionException();
			}
			if (!test.Modules.Contains(module1))
			{
				throw new TestAssertionException();
			}
			if (test.IsInitialized)
			{
				throw new TestAssertionException();
			}
			if (module1.IsInitialized)
			{
				throw new TestAssertionException();
			}

			test.Initialize();

			if (test.Modules.Count() != 1)
			{
				throw new TestAssertionException();
			}
			if (!test.Modules.Contains(module1))
			{
				throw new TestAssertionException();
			}
			if (!test.IsInitialized)
			{
				throw new TestAssertionException();
			}
			if (!module1.IsInitialized)
			{
				throw new TestAssertionException();
			}

			test.AddModule(module2);

			if (test.Modules.Count() != 2)
			{
				throw new TestAssertionException();
			}
			if (!test.Modules.Contains(module1))
			{
				throw new TestAssertionException();
			}
			if (!test.Modules.Contains(module2))
			{
				throw new TestAssertionException();
			}
			if (!test.IsInitialized)
			{
				throw new TestAssertionException();
			}
			if (!module1.IsInitialized)
			{
				throw new TestAssertionException();
			}
			if (!module2.IsInitialized)
			{
				throw new TestAssertionException();
			}

			test.RemoveModule(module2);

			if (test.Modules.Count() != 1)
			{
				throw new TestAssertionException();
			}
			if (!test.Modules.Contains(module1))
			{
				throw new TestAssertionException();
			}
			if (!test.IsInitialized)
			{
				throw new TestAssertionException();
			}
			if (!module1.IsInitialized)
			{
				throw new TestAssertionException();
			}
			if (module2.IsInitialized)
			{
				throw new TestAssertionException();
			}

			test.Unload();

			if (test.Modules.Count() != 1)
			{
				throw new TestAssertionException();
			}
			if (!test.Modules.Contains(module1))
			{
				throw new TestAssertionException();
			}
			if (test.IsInitialized)
			{
				throw new TestAssertionException();
			}
			if (module1.IsInitialized)
			{
				throw new TestAssertionException();
			}
		}
	}
}