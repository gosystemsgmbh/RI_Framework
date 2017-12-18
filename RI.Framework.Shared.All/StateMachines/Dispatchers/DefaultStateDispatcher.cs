using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using RI.Framework.Threading;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.StateMachines.Dispatchers
{
	/// <summary>
	///     Implements a default state machine operation dispatcher.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IStateDispatcher" /> for more details.
	///     </para>
	///     <para>
	///         <see cref="DefaultStateDispatcher" /> uses <see cref="SynchronizationContext" />, which is captured at the time of dispatching, to dispatch operations or falls back to <see cref="ThreadPool.QueueUserWorkItem(WaitCallback,object)" /> if no <see cref="SynchronizationContext" /> is available.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class DefaultStateDispatcher : IStateDispatcher, IDisposable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DefaultStateDispatcher" />.
		/// </summary>
		public DefaultStateDispatcher ()
		{
			this.SyncRoot = new object();
			this.UpdateTimers = new Dictionary<StateMachine, Timer>();
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="DefaultStateDispatcher" />.
		/// </summary>
		~DefaultStateDispatcher ()
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
				DispatchCapture capture = new DispatchCapture(signalDelegate, signalInfo);
				capture.Execute();
			}
		}

		/// <inheritdoc />
		public void DispatchTransition (StateMachineTransientDelegate transientDelegate, StateTransientInfo transientInfo)
		{
			lock (this.SyncRoot)
			{
				DispatchCapture capture = new DispatchCapture(transientDelegate, transientInfo);
				capture.Execute();
			}
		}

		/// <inheritdoc />
		public void DispatchUpdate (StateMachineUpdateDelegate updateDelegate, StateUpdateInfo updateInfo)
		{
			StateMachine stateMachine = updateInfo.StateMachine;
			DispatchCapture capture = new DispatchCapture(updateDelegate, updateInfo);

			lock (this.SyncRoot)
			{
				if (this.UpdateTimers.ContainsKey(stateMachine))
				{
					this.UpdateTimers[stateMachine].Dispose();
					this.UpdateTimers.Remove(stateMachine);
				}

				Timer timer = new Timer(x => { ((DispatchCapture)x).Execute(); }, capture, updateInfo.UpdateDelay, Timeout.Infinite);
				this.UpdateTimers.Add(stateMachine, timer);
			}
		}

		#endregion
	}
}
