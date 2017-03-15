using System;




namespace RI.Framework.Utilities.Threading
{
	[Serializable]
	public enum ThreadDispatcherShutdownMode
	{
		None = 0,

		DiscardPending = 1,

		FinishPending = 2,
	}
}