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

			if (assembly.GetCopyright() != "Copyright (c) 2015-2016 Roten Informatik")
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
			Assembly assembly = Assembly.GetExecutingAssembly();

			if (!assembly.GetFile().EndsWith(assembly.GetTitle() + ".dll"))
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void GetGuid_Test ()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			Guid testGuid = new Guid("92AD84FB-E1BE-4908-9D1E-77D02DFF5A5B");

			if (assembly.GetGuid(false, false) != testGuid)
			{
				throw new TestAssertionException();
			}

			if (assembly.GetGuid(false, true) != testGuid)
			{
				throw new TestAssertionException();
			}

			if (assembly.GetGuid(true, false) == assembly.GetGuid(true, true))
			{
				throw new TestAssertionException();
			}

			if (assembly.GetGuid(true, false) == testGuid)
			{
				throw new TestAssertionException();
			}

			if (assembly.GetGuid(true, true) == testGuid)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void GetProduct_Test ()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			if (assembly.GetProduct() != "Utility Framework")
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
