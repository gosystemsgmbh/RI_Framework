﻿using System;
using System.Threading;

using RI.Framework.Threading;




namespace RI.Test.Framework.Threading
{
	public sealed class Mock_HeavyThread : HeavyThread
	{
		#region Instance Constructor/Destructor

		public Mock_HeavyThread (Action action)
		{
			this.Action = action;
			this.TestValue = string.Empty;
		}

		#endregion




		#region Instance Properties/Indexer

		public Action Action { get; private set; }

		public new ManualResetEvent StopEvent => base.StopEvent;

		public string TestValue { get; private set; }

		#endregion




		#region Overrides

		protected override void Dispose (bool disposing)
		{
			base.Dispose(disposing);
			this.TestValue += "Dispose";
		}

		protected override void OnBegin ()
		{
			base.OnBegin();
			this.TestValue += "Begin";
		}

		protected override void OnEnd ()
		{
			base.OnEnd();
			this.TestValue += "End";
		}

		protected override void OnException (Exception exception, bool canContinue)
		{
			base.OnException(exception, canContinue);
			this.TestValue += "Exception";
		}

		protected override void OnRun ()
		{
			base.OnRun();
			this.Action();
		}

		protected override void OnStarted (bool withLock)
		{
			base.OnStarted(withLock);
			this.TestValue += withLock ? "Started1" : "Started2";
		}

		protected override void OnStarting ()
		{
			base.OnStarting();
			this.TestValue += "Starting";
		}

		protected override void OnStopping ()
		{
			base.OnStopping();
			this.TestValue += "Stop";
		}

		#endregion
	}
}
