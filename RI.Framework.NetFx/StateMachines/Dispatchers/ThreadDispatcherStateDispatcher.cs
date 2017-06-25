using System;
using System.Collections.Generic;

using RI.Framework.Utilities.ObjectModel;
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
	///     <note type="note">
	///         Signals and transitions are dispatched using the <see cref="IThreadDispatcher.DefaultPriority" /> priority.
	///     </note>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
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

			this.SyncRoot = new object();

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
		public IThreadDispatcher ThreadDispatcher { get; }

		private Dictionary<StateMachine, ThreadDispatcherTimer> UpdateTimers { get; }

		#endregion




		#region Interface: IStateDispatcher

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; }

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
			StateMachine stateMachine = updateInfo.StateMachine;

			lock (this.SyncRoot)
			{
				if (this.UpdateTimers.ContainsKey(stateMachine))
				{
					this.UpdateTimers[stateMachine].Stop();
					this.UpdateTimers.Remove(stateMachine);
				}

				ThreadDispatcherTimer timer = this.ThreadDispatcher.PostDelayed(updateInfo.UpdateDelay, this.ThreadDispatcher.DefaultPriority, updateDelegate, updateInfo);
				this.UpdateTimers.Add(stateMachine, timer);
				timer.Start();
			}
		}

		#endregion
	}
}
