using System;




namespace RI.Framework.StateMachines
{
	[Serializable]
	public sealed class StateMachineTransientEventArgs : EventArgs
	{
		public StateTransientInfo TransientInfo { get; private set; }

		public StateMachineTransientEventArgs (StateTransientInfo transientInfo)
		{
			if (transientInfo == null)
			{
				throw new ArgumentNullException(nameof(transientInfo));
			}

			this.TransientInfo = transientInfo;
		}
	}
}