using System;




namespace RI.Framework.StateMachines
{
	[Serializable]
	public sealed class StateMachineSignalEventArgs : EventArgs
	{
		public StateSignalInfo SignalInfo { get; private set; }

		public StateMachineSignalEventArgs(StateSignalInfo signalInfo)
		{
			if (signalInfo == null)
			{
				throw new ArgumentNullException(nameof(signalInfo));
			}

			this.SignalInfo = signalInfo;
		}
	}
}