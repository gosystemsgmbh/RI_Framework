using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Utilities.Reflection;




namespace RI.Test.Framework.Utilities.Reflection
{
	[TestClass]
	public sealed class Test_MethodCallDispatcher
	{
		#region Instance Methods

		[TestMethod]
		public void Instance_Test ()
		{
			Mock_MethodCallTarget.TestValue = "0";
			Mock_MethodCallTarget mock = new Mock_MethodCallTarget();

			MethodCallDispatcher dispatcher = MethodCallDispatcher.FromTarget(mock, nameof(Mock_MethodCallTarget.InstanceMethod));

			if (!dispatcher.CanDispatchParameter("123"))
			{
				throw new TestAssertionException();
			}

			if (!dispatcher.CanDispatchParameter(123))
			{
				throw new TestAssertionException();
			}

			if (dispatcher.CanDispatchParameter(true))
			{
				throw new TestAssertionException();
			}

			if (!dispatcher.CanDispatchType(typeof(string)))
			{
				throw new TestAssertionException();
			}

			if (!dispatcher.CanDispatchType(typeof(int)))
			{
				throw new TestAssertionException();
			}

			if (dispatcher.CanDispatchType(typeof(bool)))
			{
				throw new TestAssertionException();
			}

			if (Mock_MethodCallTarget.TestValue != "0")
			{
				throw new TestAssertionException();
			}

			if (!dispatcher.Dispatch("1", out _))
			{
				throw new TestAssertionException();
			}
			if (Mock_MethodCallTarget.TestValue != "01")
			{
				throw new TestAssertionException();
			}

			if (!dispatcher.Dispatch(2, out _))
			{
				throw new TestAssertionException();
			}
			if (Mock_MethodCallTarget.TestValue != "012")
			{
				throw new TestAssertionException();
			}

			if (dispatcher.Dispatch(false, out _))
			{
				throw new TestAssertionException();
			}
			if (Mock_MethodCallTarget.TestValue != "012")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Static_Test ()
		{
			Mock_MethodCallTarget.TestValue = "A";

			MethodCallDispatcher dispatcher = MethodCallDispatcher.FromType(typeof(Mock_MethodCallTarget), nameof(Mock_MethodCallTarget.StaticMethod));

			if (!dispatcher.CanDispatchParameter("XYZ"))
			{
				throw new TestAssertionException();
			}

			if (!dispatcher.CanDispatchParameter(123))
			{
				throw new TestAssertionException();
			}

			if (dispatcher.CanDispatchParameter(true))
			{
				throw new TestAssertionException();
			}

			if (!dispatcher.CanDispatchType(typeof(string)))
			{
				throw new TestAssertionException();
			}

			if (!dispatcher.CanDispatchType(typeof(int)))
			{
				throw new TestAssertionException();
			}

			if (dispatcher.CanDispatchType(typeof(bool)))
			{
				throw new TestAssertionException();
			}

			if (Mock_MethodCallTarget.TestValue != "A")
			{
				throw new TestAssertionException();
			}

			if (!dispatcher.Dispatch("B", out _))
			{
				throw new TestAssertionException();
			}
			if (Mock_MethodCallTarget.TestValue != "AB")
			{
				throw new TestAssertionException();
			}

			if (!dispatcher.Dispatch(0, out _))
			{
				throw new TestAssertionException();
			}
			if (Mock_MethodCallTarget.TestValue != "AB0")
			{
				throw new TestAssertionException();
			}

			if (dispatcher.Dispatch(false, out _))
			{
				throw new TestAssertionException();
			}
			if (Mock_MethodCallTarget.TestValue != "AB0")
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
