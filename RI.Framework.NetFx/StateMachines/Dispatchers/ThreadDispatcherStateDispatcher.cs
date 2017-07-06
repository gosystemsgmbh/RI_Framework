using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Threading.Dispatcher;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.StateMachines.Dispatchers
{
	/// <summary>
	///     Implements a state machine operation dispatcher which uses <see cref="IThreadDispatcher" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IStateDispatcher" /> for more details.
	///     </para>
	///     <para>
	///         State machine operations are dispatched using <see cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])" />.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class ThreadDispatcherStateDispatcher : IStateDispatcher, IDisposable
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

			this.Priority = null;
			this.Options = null;
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="ThreadDispatcherStateDispatcher" />.
		/// </summary>
		~ThreadDispatcherStateDispatcher ()
		{
			this.Dispose(false);
		}

		#endregion




		#region Instance Fields

		private ThreadDispatcherOptions? _options;

		private int? _priority;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets the options used for dispatching state machine operations.
		/// </summary>
		/// <value>
		///     The options used for dispatching state machine operations or null if the default options of the used dispatcher should be used (<see cref="IThreadDispatcher.DefaultOptions" />).
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is null.
		///     </para>
		/// </remarks>
		public ThreadDispatcherOptions? Options
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._options;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._options = value;
				}
			}
		}

		/// <summary>
		///     Gets or sets the priority used for dispatching state machine operations.
		/// </summary>
		/// <value>
		///     The priority used for dispatching state machine operations or null if the default priority of the used dispatcher should be used (<see cref="IThreadDispatcher.DefaultPriority" />).
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is null.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="value" /> is less than zero. </exception>
		public int? Priority
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
				if (value.HasValue)
				{
					if (value.Value < 0)
					{
						throw new ArgumentOutOfRangeException(nameof(value));
					}
				}

				lock (this.SyncRoot)
				{
					this._priority = value;
				}
			}
		}

		/// <summary>
		///     Gets the used dispatcher.
		/// </summary>
		/// <value>
		///     The used dispatcher.
		/// </value>
		public IThreadDispatcher ThreadDispatcher { get; }

		private Dictionary<StateMachine, ThreadDispatcherTimer> UpdateTimers { get; }

		private ThreadDispatcherOptions UsedOptions => this.Options.GetValueOrDefault(this.ThreadDispatcher.DefaultOptions);

		private int UsedPriority => this.Priority.GetValueOrDefault(this.ThreadDispatcher.DefaultPriority);

		#endregion




		#region Instance Methods

		[SuppressMessage("ReSharper", "UnusedParameter.Local")]
		private void Dispose (bool disposing)
		{
			lock (this.SyncRoot)
			{
				foreach (KeyValuePair<StateMachine, ThreadDispatcherTimer> timer in this.UpdateTimers)
				{
					timer.Value.Stop();
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
				this.ThreadDispatcher.Post(this.UsedPriority, this.UsedOptions, signalDelegate, signalInfo);
			}
		}

		/// <inheritdoc />
		public void DispatchTransition (StateMachineTransientDelegate transientDelegate, StateTransientInfo transientInfo)
		{
			lock (this.SyncRoot)
			{
				this.ThreadDispatcher.Post(this.UsedPriority, this.UsedOptions, transientDelegate, transientInfo);
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
					this.UpdateTimers[stateMachine].Stop();
					this.UpdateTimers.Remove(stateMachine);
				}

				ThreadDispatcherTimer timer = this.ThreadDispatcher.PostDelayed(updateInfo.UpdateDelay, this.UsedPriority, this.UsedOptions, updateDelegate, updateInfo);
				this.UpdateTimers.Add(stateMachine, timer);
			}
		}

		#endregion
	}
}
