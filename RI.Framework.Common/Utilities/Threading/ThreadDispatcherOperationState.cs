using System;




namespace RI.Framework.Utilities.Threading
{
	[Serializable]
	public enum ThreadDispatcherOperationState
	{
		Waiting = 0,

		Executing = 1,

		Finished = 2,

		Canceled = 3,

		Exception = 4,
	}
}