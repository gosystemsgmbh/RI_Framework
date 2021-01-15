using System;
using System.Collections.Generic;
using System.Text;

namespace RI.Test.Framework.Utilities.Reflection
{
	public sealed class Mock_MethodCallTarget
	{
		public static string TestValue { get; set; }

		public static void StaticMethod (string value)
		{
			Mock_MethodCallTarget.TestValue += value;
		}

		public static void StaticMethod (int value)
		{
			Mock_MethodCallTarget.TestValue += value;
		}

		public void InstanceMethod (string value)
		{
			Mock_MethodCallTarget.TestValue += value;
		}

		public void InstanceMethod (int value)
		{
			Mock_MethodCallTarget.TestValue += value;
		}
	}
}
