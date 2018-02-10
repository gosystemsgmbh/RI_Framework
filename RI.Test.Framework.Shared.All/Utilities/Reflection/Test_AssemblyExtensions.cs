using System;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Utilities.Reflection;




namespace RI.Test.Framework.Utilities.Reflection
{
	[TestClass]
	public sealed class Test_AssemblyExtensions
	{
		#region Instance Methods

		[TestMethod]
		public void GetCompany_Test ()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			if (assembly.GetCompany() != "Roten Informatik")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void GetCopyright_Test ()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			if (assembly.GetCopyright() != "Copyright (c) 2011-2018 Roten Informatik")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void GetDescription_Test ()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			if (!assembly.GetDescription().StartsWith("RI.Test.Framework."))
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void GetFile_Test ()
		{
#if PLATFORM_NETFX
			Assembly assembly = Assembly.GetExecutingAssembly();

			if (!assembly.GetFile().EndsWith(assembly.GetTitle() + ".dll"))
			{
				throw new TestAssertionException();
			}
#endif
		}

		[TestMethod]
		public void GetGuid_Test ()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			Guid testGuid1 = new Guid("C87E32C3-CDD1-51F8-6D52-981B2724C0BC");
			Guid testGuid2 = new Guid("B97C77BD-B9D9-2EEC-4672-616D65776F72");
			Guid testGuid3 = new Guid("7527DDB6-406E-4896-8ECA-A017ECD7B5E7");

#if PLATFORM_NETFX

			if (assembly.GetGuid(true, false) != testGuid1)
			{
				throw new TestAssertionException();
			}

			if (assembly.GetGuid(true, true) != testGuid2)
			{
				throw new TestAssertionException();
			}

			if (assembly.GetGuid(true, false) == assembly.GetGuid(true, true))
			{
				throw new TestAssertionException();
			}

#endif

			if (assembly.GetGuid(false, false) != testGuid3)
			{
				throw new TestAssertionException();
			}

			if (assembly.GetGuid(false, true) != testGuid3)
			{
				throw new TestAssertionException();
			}

			if (assembly.GetGuid(false, false) != assembly.GetGuid(false, true))
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void GetProduct_Test ()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			if (assembly.GetProduct() != "Decoupling & Utilities Framework")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void GetTitle_Test ()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			if (!assembly.GetTitle().StartsWith("RI.Test.Framework."))
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void GetVersion_Test ()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			if (assembly.GetAssemblyVersion() != assembly.GetFileVersion())
			{
				throw new TestAssertionException();
			}

			if (assembly.GetAssemblyVersion() != assembly.GetInformationalVersion())
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
