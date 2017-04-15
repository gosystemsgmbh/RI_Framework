using System;
using System.Collections;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Utilities.Threading;

using UnityEngine;




namespace RI.Test.Framework.Utilities.Threading
{
	public sealed class Test_ThreadMover : TestModule
	{
		#region Instance Properties/Indexer

		private Action TestContinuation { get; set; }

		#endregion




		#region Instance Methods

		[TestMethod]
		public void Test ()
		{
			ThreadMover.BeginTask(this.TestCoroutine1());
			ThreadMover.BeginTask(this.TestCoroutine2());
			ThreadMover.BeginTask(this.TestCoroutine2());
		}

		private IEnumerator TestCoroutine1 ()
		{
			yield return null;
			yield return new WaitForSeconds(2);
			yield return new ToBackground();
			yield return new ToBackground();
			yield return new ToForeground();
			yield return new ToForeground();
			yield return new ToHeavyThread();
			yield return new ToHeavyThread();
			yield return new ToDispatcher();
			yield return new ToDispatcher();
			yield return new ToDispatcher();
			yield return new ToDispatcher();

			yield return null;
			yield return new WaitForSeconds(2);
			yield return new ToBackground();
			yield return new WaitForSeconds(2);
			yield return new ToBackground();
			yield return new WaitForEndOfFrame();
			yield return new ToHeavyThread();
			yield return new WaitForEndOfFrame();
			yield return new ToDispatcher();
			yield return new WaitForEndOfFrame();
			yield return new ToDispatcher();
			yield return new WaitForEndOfFrame();
			yield return new ToForeground();
			yield return new ToBackground();
			yield return new ToHeavyThread();
			yield return new ToDispatcher();
			yield return new ToDispatcher();

			this.TestContinuation();
		}

		private IEnumerator TestCoroutine2 ()
		{
			yield return null;
			yield return new WaitForSeconds(2);
			yield return new ToBackground();
			yield return new ToBackground();
			yield return new ToForeground();
			yield return new ToForeground();
			yield return new ToHeavyThread();
			yield return new ToHeavyThread();
			yield return new ToDispatcher();
			yield return new ToDispatcher();
			yield return new ToDispatcher();
			yield return new ToDispatcher();

			yield return null;
			yield return new WaitForSeconds(2);
			yield return new ToBackground();
			yield return new WaitForSeconds(2);
			yield return new ToBackground();
			yield return new WaitForEndOfFrame();
			yield return new ToHeavyThread();
			yield return new WaitForEndOfFrame();
			yield return new ToDispatcher();
			yield return new WaitForEndOfFrame();
			yield return new ToDispatcher();
			yield return new WaitForEndOfFrame();
			yield return new ToForeground();
			yield return new ToBackground();
			yield return new ToHeavyThread();
			yield return new ToDispatcher();
			yield return new ToDispatcher();
		}

		#endregion




		#region Overrides

		public override void InvokeTestMethod (MethodInfo method, Action testContinuation)
		{
			this.TestContinuation = testContinuation;
			base.InvokeTestMethod(method, null);
		}

		#endregion
	}
}
