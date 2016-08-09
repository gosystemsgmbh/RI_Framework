using System.Collections.Generic;
using System.Reflection;

using RI.Framework.Collections;




namespace RI.Test.Framework
{
	public abstract class TestModule
	{
		#region Instance Methods

		public List<MethodInfo> GetTestMethods ()
		{
			List<MethodInfo> testMethods = new List<MethodInfo>(this.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public));

			testMethods.RemoveWhere(x =>
			{
				object[] attributes = x.GetCustomAttributes(typeof(TestMethodAttribute), false);
				return attributes.Length == 0;
			});

			return testMethods;
		}

		protected void Fail (string message, params object[] args)
		{
			string finalMessage = string.Format(message, args);
			throw new TestAssertionException(finalMessage);
		}

		#endregion
	}
}
