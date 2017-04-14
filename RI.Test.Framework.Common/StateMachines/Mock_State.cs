using System;
using System.Collections.Generic;
using System.Text;

using RI.Framework.StateMachines;




namespace RI.Test.Framework.StateMachines
{
	public abstract class Mock_State : State
	{
		public static string TestValue { get; set; }
	}
}
