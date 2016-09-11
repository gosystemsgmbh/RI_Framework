using System;
using System.Collections.Generic;

using RI.Framework.Utilities.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;




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
