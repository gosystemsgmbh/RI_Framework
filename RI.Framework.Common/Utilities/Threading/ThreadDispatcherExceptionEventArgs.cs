using System;

namespace RI.Framework.Utilities.Threading
{
	public sealed class ThreadDispatcherExceptionEventArgs : EventArgs
	{
		public Exception Exception { get; }

		public bool CanContinue { get; }


		public ThreadDispatcherExceptionEventArgs (Exception exception, bool canContinue)
		{
			this.Exception = exception;
			this.CanContinue = canContinue;
		}
	}
}
