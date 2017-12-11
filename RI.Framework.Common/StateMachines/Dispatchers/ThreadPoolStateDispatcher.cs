using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using RI.Framework.Composition.Model;
using RI.Framework.StateMachines;
using RI.Framework.StateMachines.Dispatchers;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bus.Dispatchers
{
	/// <summary>
	///     Implements a state machine operation dispatcher which uses <see cref="ThreadPool" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IStateDispatcher" /> for more details.
	///     </para>
	///     <para>
	///         State machine operations are dispatched using <see cref="ThreadPool.QueueUserWorkItem(WaitCallback)" />.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public sealed class ThreadPoolStateDispatcher : IStateDispatcher, IDisposable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ThreadPoolStateDispatcher" />.
		/// </summary>
		public ThreadPoolStateDispatcher ()
		{
			this.SyncRoot = new object();
			this.UpdateTimers = new Dictionary<StateMachine, Timer>();
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="ThreadPoolStateDispatcher" />.
		/// </summary>
		~ThreadPoolStateDispatcher ()
		{
			this.Dispose(false);
		}

		#endregion




		#region Instance Properties/Indexer

		private Dictionary<StateMachine, Timer> UpdateTimers { get; }

		#endregion




		#region Instance Methods

		[SuppressMessage("ReSharper", "UnusedParameter.Local")]
		private void Dispose (bool disposing)
		{
			lock (this.SyncRoot)
			{
				foreach (KeyValuePair<StateMachine, Timer> timer in this.UpdateTimers)
				{
					timer.Value.Dispose();
				}
				this.UpdateTimers.Clear();
			}
		}

		#endregion




		#region Interface: IDisposable

		/// <inheritdoc />
		public void Dispose ()
		{
			this.Dispose(true);
		}

		#endregion




		#region Interface: IStateDispatcher

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; }


		/// <inheritdoc />
		public void DispatchSignal (StateMachineSignalDelegate signalDelegate, StateSignalInfo signalInfo)
		{
			lock (this.SyncRoot)
			{
				ThreadPool.QueueUserWorkItem(_ => signalDelegate(signalInfo));
			}
		}

		/// <inheritdoc />
		public void DispatchTransition (StateMachineTransientDelegate transientDelegate, StateTransientInfo transientInfo)
		{
			lock (this.SyncRoot)
			{
				ThreadPool.QueueUserWorkItem(_ => transientDelegate(transientInfo));
			}
		}

		/// <inheritdoc />
		public void DispatchUpdate (StateMachineUpdateDelegate updateDelegate, StateUpdateInfo updateInfo)
		{
			StateMachine stateMachine = updateInfo.StateMachine;

			lock (this.SyncRoot)
			{
				if (this.UpdateTimers.ContainsKey(stateMachine))
				{
					this.UpdateTimers[stateMachine].Dispose();
					this.UpdateTimers.Remove(stateMachine);
				}

				Timer timer = new Timer(_ => updateDelegate(updateInfo), null, updateInfo.UpdateDelay, Timeout.Infinite);
				this.UpdateTimers.Add(stateMachine, timer);
			}
		}

		#endregion
	}
}
