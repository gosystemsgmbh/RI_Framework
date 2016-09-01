using System;
using System.Collections.Generic;

using RI.Framework.Utilities.Reflection;

#if PLATFORM_NET
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
#if PLATFORM_UNITY
using RI.Test.Framework;
#endif




namespace RI.Test.Framework.Utilities.Reflection
{
	[TestClass]
	public sealed class Test_TypeExtensions
	{
		#region Instance Methods

		[TestMethod]
		public void GetInheritance_Test ()
		{
			List<Type> inheritance = typeof(int).GetInheritance(false);

			if (inheritance.Count != 2)
			{
				throw new TestAssertionException();
			}

			if (inheritance[0] != typeof(object))
			{
				throw new TestAssertionException();
			}

			if (inheritance[1] != typeof(ValueType))
			{
				throw new TestAssertionException();
			}

			inheritance = typeof(int).GetInheritance(true);

			if (inheritance.Count != 3)
			{
				throw new TestAssertionException();
			}

			if (inheritance[0] != typeof(object))
			{
				throw new TestAssertionException();
			}

			if (inheritance[1] != typeof(ValueType))
			{
				throw new TestAssertionException();
			}

			if (inheritance[2] != typeof(int))
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
