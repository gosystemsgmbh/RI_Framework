using System;

using RI.Framework.Services.Dispatcher;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.StateMachines.Dispatchers
{
	/// <summary>
	///     Implements a default state machine operation dispatcher which uses <see cref="RI.Framework.Services.Dispatcher.IDispatcherService" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IStateDispatcher" /> for more details.
	///     </para>
	///     <note type="note">
	///         Signals and transitions are dispatched using the <see cref="DispatcherPriority.Default" /> priority.
	///     </note>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class DispatcherServiceStateDispatcher : IStateDispatcher
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DispatcherServiceStateDispatcher" />.
		/// </summary>
		/// <param name="dispatcherService"> The used dispatcher service. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcherService" /> is null. </exception>
		public DispatcherServiceStateDispatcher (IDispatcherService dispatcherService)
		{
			if (dispatcherService == null)
			{
				throw new ArgumentNullException(nameof(dispatcherService));
			}

			this.SyncRoot = new object();
			this.DispatcherService = dispatcherService;
			this.Priority = DispatcherPriority.Default;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used dispatcher service.
		/// </summary>
		/// <value>
		///     The used dispatcher service.
		/// </value>
		public IDispatcherService DispatcherService { get; }

		private DispatcherPriority _priority;

		/// <summary>
		/// Gets or sets the priority used for dispatching state machine operations.
		/// </summary>
		/// <value>
		/// The priority used for dispatching state machine operations.
		/// </value>
		/// <remarks>
		/// <para>
		/// The default value is <see cref="DispatcherPriority.Default"/>.
		/// </para>
		/// </remarks>
		public DispatcherPriority Priority
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._priority;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._priority = value;
				}
			}
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
			this.DispatcherService.Dispatch(this.Priority, (x, y) => x(y), signalDelegate, signalInfo);
		}

		/// <inheritdoc />
		public void DispatchTransition (StateMachineTransientDelegate transientDelegate, StateTransientInfo transientInfo)
		{
			this.DispatcherService.Dispatch(this.Priority, (x, y) => x(y), transientDelegate, transientInfo);
		}

		/// <inheritdoc />
		public void DispatchUpdate (StateMachineUpdateDelegate updateDelegate, StateUpdateInfo updateInfo)
		{
			this.DispatcherService.Dispatch(this.Priority, (x, y) => x(y), updateDelegate, updateInfo).Reschedule(updateInfo.UpdateDelay);
		}

		#endregion
	}
}
