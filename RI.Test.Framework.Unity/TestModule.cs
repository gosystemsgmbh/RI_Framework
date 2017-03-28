using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Collections;
using RI.Framework.Composition.Model;

namespace RI.Test.Framework
{
	[Export]
	public abstract class TestModule
	{
		#region Instance Methods

		public virtual List<MethodInfo> GetTestMethods ()
		{
			List<MethodInfo> testMethods = new List<MethodInfo>(this.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public));

			testMethods.RemoveWhere(x =>
			                        {
				                        object[] attributes = x.GetCustomAttributes(typeof(TestMethodAttribute), false);
				                        return attributes.Length == 0;
			                        });

			return testMethods;
		}

		public virtual void InvokeTestMethod (MethodInfo method, Action testContinuation)
		{
			method.Invoke(this, null);
			testContinuation?.Invoke();
		}

		protected void Fail()
		{
			throw new TestAssertionException();
		}

		protected void Fail (string message, params object[] args)
		{
			string finalMessage = string.Format(message, args);
			throw new TestAssertionException(finalMessage);
		}

		#endregion
	}
}
