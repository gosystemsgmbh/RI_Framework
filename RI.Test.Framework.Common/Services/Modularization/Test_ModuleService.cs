using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Services.Modularization;




namespace RI.Test.Framework.Services.Modularization
{
	[TestClass]
	public sealed class Test_ModuleService
	{
		#region Instance Methods

		[TestMethod]
		public void Test ()
		{
			ModuleService test = new ModuleService();
			Mock_Module module1 = new Mock_Module();
			Mock_Module module2 = new Mock_Module();

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

		#endregion
	}
}
