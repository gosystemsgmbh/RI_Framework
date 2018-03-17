using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.StateMachines.Dispatchers
{
	/// <summary>
	///     Implements a state machine operation dispatcher which uses <see cref="SynchronizationContext" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IStateDispatcher" /> for more details.
	///     </para>
	///     <para>
	///         State machine operations are dispatched using <see cref="System.Threading.SynchronizationContext.Post(SendOrPostCallback,object)" />.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public sealed class SynchronizationContextStateDispatcher : IStateDispatcher, IDisposable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="SynchronizationContextStateDispatcher" />.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The synchronization context which is the current context when this instance is created will be used, using <see cref="System.Threading.SynchronizationContext.Current" />.
		///     </para>
		/// </remarks>
		/// <exception cref="InvalidOperationException"> There is no current synchronization context available. </exception>
		public SynchronizationContextStateDispatcher ()
		{
			SynchronizationContext context = SynchronizationContext.Current;
			if (context == null)
			{
				throw new InvalidOperationException("No current synchronization context available.");
			}

			this.SyncRoot = new object();
			this.SynchronizationContext = context;
			this.UpdateTimers = new Dictionary<StateMachine, Timer>();
		}

		/// <summary>
		///     Creates a new instance of <see cref="SynchronizationContextStateDispatcher" />.
		/// </summary>
		/// <param name="synchronizationContext"> The used synchronization context. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="synchronizationContext" /> is null. </exception>
		public SynchronizationContextStateDispatcher (SynchronizationContext synchronizationContext)
		{
			if (synchronizationContext == null)
			{
				throw new ArgumentNullException(nameof(synchronizationContext));
			}

			this.SyncRoot = new object();
			this.SynchronizationContext = synchronizationContext;
			this.UpdateTimers = new Dictionary<StateMachine, Timer>();
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="SynchronizationContextStateDispatcher" />.
		/// </summary>
		~SynchronizationContextStateDispatcher ()
		{
			this.Dispose(false);
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used synchronization context.
		/// </summary>
		/// <value>
		///     The used synchronization context.
		/// </value>
		public SynchronizationContext SynchronizationContext { get; }

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
				this.SynchronizationContext.Post(_ => signalDelegate.Invoke(signalInfo), null);
			}
		}

		/// <inheritdoc />
		public void DispatchTransition (StateMachineTransientDelegate transientDelegate, StateTransientInfo transientInfo)
		{
			lock (this.SyncRoot)
			{
				this.SynchronizationContext.Post(_ => transientDelegate.Invoke(transientInfo), null);
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
