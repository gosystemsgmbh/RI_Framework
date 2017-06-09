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
	///         <see cref="StateDispatcher" /> internally uses a <see cref="SynchronizationContext"/>, which is captured at the time of dispatching, to dispatch operations or falls back to <see cref="ThreadPool.QueueUserWorkItem(WaitCallback,object)"/> if no <see cref="SynchronizationContext"/> is available.
	///     </para>
	///     <para>
	///         See <see cref="IStateCache" /> for more details.
	///     </para>
	/// </remarks>
	public sealed class StateDispatcher : IStateDispatcher
	{
		/// <summary>
		///     Creates a new instance of <see cref="StateDispatcher" />.
		/// </summary>
		public StateDispatcher()
		{
			this.SyncRoot = new object();

			this.UpdateTimers = new Dictionary<StateMachine, Timer>();
		}

		/// <inheritdoc />
		public object SyncRoot { get; private set; }

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		private Dictionary<StateMachine, Timer> UpdateTimers { get; set; }

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

				Timer timer = new Timer(x =>
				{
					((DispatchCapture)x).Execute();
				}, capture, updateInfo.UpdateDelay, Timeout.Infinite);
				this.UpdateTimers.Add(stateMachine, timer);
			}
		}

		private sealed class DispatchCapture
		{
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

			public Delegate Action { get; private set; }

			public object Arguments { get; private set; }

			public SynchronizationContext Context { get; private set; }

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
		}
	}
}
