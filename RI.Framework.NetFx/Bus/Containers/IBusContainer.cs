using System;
using System.Collections;
using System.Collections.Generic;

namespace RI.Framework.Bus.Containers
{
	public interface IBusContainer
	{
		IEnumerable<object> Resolve (Type type);
	}
}
