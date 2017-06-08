using System;
using System.Collections.Generic;
using System.Threading;




namespace RI.Framework.StateMachines.Dispatchers
{
	/// <summary>
	///     Implements a state machine operation dispatcher which uses <see cref="SynchronizationContext" />.
	/// </summary>
	/// <remarks>
	///     <note type="note">
	///         The current <see cref="SynchronizationContext" /> to be used is not captured during construction of <see cref="SynchronizationContextStateDispatcher" /> but rather at the moment an operation is dispatched.
	///     </note>
	///     <note type="note">
	///         If no <see cref="SynchronizationContext" /> can be captured, <see cref="ThreadPool" />.<see cref="ThreadPool.QueueUserWorkItem(WaitCallback,object)" /> is used.
	///     </note>
	///     <para>
	///         See <see cref="IStateDispatcher" /> for more details.
	///     </para>
	/// </remarks>
	public sealed class SynchronizationContextStateDispatcher : IStateDispatcher
	{
		#region Instance Properties/Indexer

		private Dictionary<StateMachine, Timer> UpdateTimers { get; set; }

		#endregion




		#region Instance Methods

		private void PostDelegate (SynchronizationContext synchronizationContext, Delegate action, object args)
		{
			PostState state = new PostState();
			state.Delegate = action;
			state.Arguments = args;

			if (synchronizationContext == null)
			{
				ThreadPool.QueueUserWorkItem(x =>
				{
					PostState postState = (PostState)x;
					postState.Delegate.DynamicInvoke(postState.Arguments);
				}, state);
			}
			else
			{
				synchronizationContext.Post(x =>
				{
					PostState postState = (PostState)x;
					postState.Delegate.DynamicInvoke(postState.Arguments);
				}, state);
			}
		}

		#endregion




		#region Interface: IStateDispatcher

		/// <inheritdoc />
		public void DispatchSignal (StateMachineSignalDelegate signalDelegate, StateSignalInfo signalInfo)
		{
			this.PostDelegate(SynchronizationContext.Current, signalDelegate, signalInfo);
		}

		/// <inheritdoc />
		public void DispatchTransition (StateMachineTransientDelegate transientDelegate, StateTransientInfo transientInfo)
		{
			this.PostDelegate(SynchronizationContext.Current, transientDelegate, transientInfo);
		}

		/// <inheritdoc />
		public void DispatchUpdate (StateMachineUpdateDelegate updateDelegate, StateUpdateInfo updateInfo)
		{
			if (this.UpdateTimers.ContainsKey(updateInfo.StateMachine))
			{
				this.UpdateTimers[updateInfo.StateMachine].Dispose();
				this.UpdateTimers.Remove(updateInfo.StateMachine);
			}

			UpdateTimerState state = new UpdateTimerState();
			state.UpdateDelegate = updateDelegate;
			state.UpdateInfo = updateInfo;
			state.SynchronizationContext = SynchronizationContext.Current;

			Timer timer = new Timer(x =>
			{
				UpdateTimerState intervalState = (UpdateTimerState)x;
				this.PostDelegate(intervalState.SynchronizationContext, intervalState.UpdateDelegate, intervalState.UpdateInfo);
				intervalState.Timer.Dispose();
			}, state, updateInfo.UpdateDelay, Timeout.Infinite);
			state.Timer = timer;
			this.UpdateTimers.Add(updateInfo.StateMachine, timer);
		}

		#endregion




		#region Type: PostState

		private sealed class PostState
		{
			#region Instance Properties/Indexer

			public object Arguments { get; set; }
			public Delegate Delegate { get; set; }

			#endregion
		}

		#endregion




		#region Type: UpdateTimerState

		private sealed class UpdateTimerState
		{
			#region Instance Properties/Indexer

			public SynchronizationContext SynchronizationContext { get; set; }
			public Timer Timer { get; set; }

			public StateMachineUpdateDelegate UpdateDelegate { get; set; }

			public StateUpdateInfo UpdateInfo { get; set; }

			#endregion
		}

		#endregion
	}
}
