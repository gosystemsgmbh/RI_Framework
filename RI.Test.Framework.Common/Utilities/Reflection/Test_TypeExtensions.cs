using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Utilities.Reflection;




namespace RI.Test.Framework.Utilities.Reflection
{
	[TestClass]
	public sealed class Test_TypeExtensions
	{
		#region Instance Methods

		[TestMethod]
		public void GetBestMatchingType_Test ()
		{
			Type matchingType = null;
			int inheritanceDepth = -1;

			if (!typeof(object).GetBestMatchingType(out matchingType, out inheritanceDepth, typeof(object)))
			{
				throw new TestAssertionException();
			}
			if (matchingType != typeof(object))
			{
				throw new TestAssertionException();
			}
			if (inheritanceDepth != 0)
			{
				throw new TestAssertionException();
			}

			if (!typeof(object).GetBestMatchingType(out matchingType, out inheritanceDepth, typeof(object), typeof(int), typeof(string)))
			{
				throw new TestAssertionException();
			}
			if (matchingType != typeof(object))
			{
				throw new TestAssertionException();
			}
			if (inheritanceDepth != 0)
			{
				throw new TestAssertionException();
			}

			if (typeof(object).GetBestMatchingType(out matchingType, out inheritanceDepth, typeof(int), typeof(string)))
			{
				throw new TestAssertionException();
			}
			if (matchingType != null)
			{
				throw new TestAssertionException();
			}
			if (inheritanceDepth != -1)
			{
				throw new TestAssertionException();
			}

			if (!typeof(string).GetBestMatchingType(out matchingType, out inheritanceDepth, typeof(string)))
			{
				throw new TestAssertionException();
			}
			if (matchingType != typeof(string))
			{
				throw new TestAssertionException();
			}
			if (inheritanceDepth != 0)
			{
				throw new TestAssertionException();
			}

			if (!typeof(string).GetBestMatchingType(out matchingType, out inheritanceDepth, typeof(string), typeof(int), typeof(object)))
			{
				throw new TestAssertionException();
			}
			if (matchingType != typeof(string))
			{
				throw new TestAssertionException();
			}
			if (inheritanceDepth != 0)
			{
				throw new TestAssertionException();
			}

			if (!typeof(string).GetBestMatchingType(out matchingType, out inheritanceDepth, typeof(int), typeof(object)))
			{
				throw new TestAssertionException();
			}
			if (matchingType != typeof(object))
			{
				throw new TestAssertionException();
			}
			if (inheritanceDepth != 1)
			{
				throw new TestAssertionException();
			}

			if (typeof(string).GetBestMatchingType(out matchingType, out inheritanceDepth, typeof(int)))
			{
				throw new TestAssertionException();
			}
			if (matchingType != null)
			{
				throw new TestAssertionException();
			}
			if (inheritanceDepth != -1)
			{
				throw new TestAssertionException();
			}
		}

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
