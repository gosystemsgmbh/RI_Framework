﻿using System;
using System.Collections.Generic;
using System.Reflection;

using RI.Framework.Collections.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;




namespace RI.Test.Framework.Cases
{
	public sealed  class Test_Common : TestModule
	{
		public override List<MethodInfo> GetTestMethods ()
		{
			//TODO: Re-activate
			return new List<MethodInfo>();

			List<MethodInfo> result = new List<MethodInfo>(base.GetTestMethods());

			Assembly assembly = Assembly.GetExecutingAssembly();
			Type[] types = assembly.GetTypes();
			Type[] testTypes = (from x in types where this.TestType(x) select x).ToArray();
			MethodInfo[] methods = testTypes.SelectMany(x => x.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)).ToArray();
			MethodInfo[] testMethods = (from x in methods where this.TestMethod(x) select x).ToArray();
			result.AddRange(testMethods);

			return result;
		}

		public override void InvokeTestMethod (MethodInfo method, Action testContinuation)
		{
			object instance = Activator.CreateInstance(method.DeclaringType);
			method.Invoke(instance, null);
			testContinuation.Invoke();
		}

		private bool TestType (Type type)
		{
			return type.GetCustomAttributes(typeof(TestClassAttribute), false).Length > 0;
		}

		private bool TestMethod (MethodInfo method)
		{
			return method.GetCustomAttributes(typeof(TestMethodAttribute), false).Length > 0;
		}
	}
}