using System;
using System.Runtime.Serialization;




namespace RI.Test.Framework
{
	[Serializable]
	public sealed class TestAssertionException : Exception
	{
		#region Instance Constructor/Destructor

		public TestAssertionException ()
				: base("Test failed.")
		{
		}

		public TestAssertionException (string failure)
				: base(failure)
		{
		}

		private TestAssertionException (SerializationInfo info, StreamingContext context)
				: base(info, context)
		{
		}

		#endregion
	}
}
