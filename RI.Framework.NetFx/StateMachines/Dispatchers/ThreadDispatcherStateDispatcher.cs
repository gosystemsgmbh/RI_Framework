using System;
using System.Collections.Generic;

using RI.Framework.Utilities.Threading;




namespace RI.Framework.StateMachines.Dispatchers
{
	/// <summary>
	///     Implements a state machine operation dispatcher which uses a <see cref="IThreadDispatcher" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IStateDispatcher" /> for more details.
	///     </para>
	/// </remarks>
	public sealed class ThreadDispatcherStateDispatcher : IStateDispatcher
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ThreadDispatcherStateDispatcher" />.
		/// </summary>
		/// <param name="threadDispatcher"> The used dispatcher. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="threadDispatcher" /> is null. </exception>
		public ThreadDispatcherStateDispatcher (IThreadDispatcher threadDispatcher)
		{
			if (threadDispatcher == null)
			{
				throw new ArgumentNullException(nameof(threadDispatcher));
			}

			this.ThreadDispatcher = threadDispatcher;

			this.UpdateTimers = new Dictionary<StateMachine, ThreadDispatcherTimer>();
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used dispatcher.
		/// </summary>
		/// <value>
		///     The used dispatcher.
		/// </value>
		public IThreadDispatcher ThreadDispatcher { get; private set; }

		private Dictionary<StateMachine, ThreadDispatcherTimer> UpdateTimers { get; set; }

		#endregion




		#region Interface: IStateDispatcher

		/// <inheritdoc />
		public void DispatchSignal (StateMachineSignalDelegate signalDelegate, StateSignalInfo signalInfo)
		{
			this.ThreadDispatcher.Post(new Action<StateMachineSignalDelegate, StateSignalInfo>((x, y) => x(y)), signalDelegate, signalInfo);
		}

		/// <inheritdoc />
		public void DispatchTransition (StateMachineTransientDelegate transientDelegate, StateTransientInfo transientInfo)
		{
			this.ThreadDispatcher.Post(new Action<StateMachineTransientDelegate, StateTransientInfo>((x, y) => x(y)), transientDelegate, transientInfo);
		}

		/// <inheritdoc />
		public void DispatchUpdate (StateMachineUpdateDelegate updateDelegate, StateUpdateInfo updateInfo)
		{
			if (this.UpdateTimers.ContainsKey(updateInfo.StateMachine))
			{
				this.UpdateTimers[updateInfo.StateMachine].Stop();
				this.UpdateTimers.Remove(updateInfo.StateMachine);
			}

			ThreadDispatcherTimer timer = new ThreadDispatcherTimer(this.ThreadDispatcher, ThreadDispatcherTimerMode.OneShot, this.ThreadDispatcher.DefaultPriority, updateInfo.UpdateDelay, updateDelegate, updateInfo);
			this.UpdateTimers.Add(updateInfo.StateMachine, timer);
			timer.Start();
		}

		#endregion
	}
}
