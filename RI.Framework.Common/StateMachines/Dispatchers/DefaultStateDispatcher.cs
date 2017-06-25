using System;
using System.Collections.Generic;
using System.Threading;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.StateMachines.Dispatchers
{
	/// <summary>
	///     Implements a default state machine operation dispatcher suitable for most scenarios.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="DefaultStateDispatcher" /> internally uses a <see cref="SynchronizationContext" />, which is captured at the time of dispatching, to dispatch operations or falls back to <see cref="ThreadPool.QueueUserWorkItem(WaitCallback,object)" /> if no <see cref="SynchronizationContext" /> is available.
	///     </para>
	///     <para>
	///         See <see cref="IStateCache" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class DefaultStateDispatcher : IStateDispatcher
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

		#endregion




		#region Instance Properties/Indexer

		private Dictionary<StateMachine, Timer> UpdateTimers { get; }

		#endregion




		#region Interface: IStateDispatcher

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; }

		/// <inheritdoc />
		public void DispatchSignal (StateMachineSignalDelegate signalDelegate, StateSignalInfo signalInfo)
		{
			DispatchCapture capture = new DispatchCapture(signalDelegate, signalInfo);
			capture.Execute();
		}

		/// <inheritdoc />
		public void DispatchTransition (StateMachineTransientDelegate transientDelegate, StateTransientInfo transientInfo)
		{
			DispatchCapture capture = new DispatchCapture(transientDelegate, transientInfo);
			capture.Execute();
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




		#region Type: DispatchCapture

		private sealed class DispatchCapture
		{
			#region Instance Constructor/Destructor

			public DispatchCapture (Delegate action, object arguments)
			{
				if (action == null)
				{
					throw new ArgumentNullException(nameof(action));
				}

				if (arguments == null)
				{
					throw new ArgumentNullException(nameof(arguments));
				}

				this.Action = action;
				this.Arguments = arguments;
				this.Context = SynchronizationContext.Current;
			}

			#endregion




			#region Instance Properties/Indexer

			public Delegate Action { get; private set; }

			public object Arguments { get; private set; }

			public SynchronizationContext Context { get; private set; }

			#endregion




			#region Instance Methods

			public void Execute ()
			{
				if (this.Context != null)
				{
					this.Context.Post(x =>
					{
						DispatchCapture capture = ((DispatchCapture)x);
						capture.Action.DynamicInvoke(capture.Arguments);
					}, this);
				}
				else
				{
					ThreadPool.QueueUserWorkItem(x =>
					{
						DispatchCapture capture = ((DispatchCapture)x);
						capture.Action.DynamicInvoke(capture.Arguments);
					}, this);
				}
			}

			#endregion
		}

		#endregion
	}
}
