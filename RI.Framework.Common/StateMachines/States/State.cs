﻿using System;
using System.Collections.Generic;

using RI.Framework.Collections;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.StateMachines.States
{
	/// <summary>
	///     Implements a base class which can be used for state implementation.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IState" /> for more details.
	///     </para>
	///     <note type="important">
	///         Becareful when using <see cref="State" /> with performance critical code!
	///         See <see cref="RegisterSignal{TSignal}" /> for details.
	///     </note>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public abstract class State : LogSource, IState
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="State" />.
		/// </summary>
		protected State ()
		{
			this.SyncRoot = new object();

			this.IsInitialized = false;
			this.UseCaching = true;
			this.UpdateInterval = null;

			this.SignalHandlers = new Dictionary<Type, Delegate>();
			this.SubMachines = new HashSet<StateMachine>();
			this.ActiveMachines = new HashSet<StateMachine>();
		}

		#endregion




		#region Instance Fields

		private bool _isInitialized;

		private int? _updateInterval;

		private bool _useCaching;

		#endregion




		#region Instance Properties/Indexer

		private HashSet<StateMachine> ActiveMachines { get; }

		private Dictionary<Type, Delegate> SignalHandlers { get; }

		private HashSet<StateMachine> SubMachines { get; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Creates a new sub state machine which is a hierarchical subordinate to this state.
		/// </summary>
		/// <param name="creator"> The creator callback which is used to create the sub state machine. </param>
		/// <returns>
		///     The created sub state machine.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         The created sub state machine is stored by this state instance.
		///         This state instance will issue a transient to null on the created sub state machine every time this state is left (<see cref="Leave" />).
		///         Further management, like keeping a reference to issue signals, transients, etc., must be done by this state itself.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="creator" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="creator" /> returned null instead of a <see cref="StateMachine" /> or derived instance. </exception>
		protected StateMachine CreateSubStateMachine (Func<StateMachine> creator)
		{
			if (creator == null)
			{
				throw new ArgumentNullException(nameof(creator));
			}

			lock (this.SyncRoot)
			{
				StateMachine subStateMachine = creator();
				if (subStateMachine == null)
				{
					throw new InvalidOperationException();
				}

				this.SubMachines.Add(subStateMachine);

				return subStateMachine;
			}
		}

		/// <summary>
		///     Registers a type-specific signal handler.
		/// </summary>
		/// <typeparam name="TSignal"> The type of the signals handled by <paramref name="handler" />. </typeparam>
		/// <param name="handler"> The delegate which handles signals of type <typeparamref name="TSignal" />. </param>
		/// <remarks>
		///     <note type="important">
		///         This is a convenience method which allows you to register type-specific signal handlers, automatically routing signals to their corresponding handler based on their type.
		///         However, this comes with a significant performance impact as the routing uses <see cref="Delegate.DynamicInvoke" />.
		///         Do not use <see cref="RegisterSignal{TSignal}" /> for highly performance critical code!
		///         Use <see cref="OnUnregisteredSignal" /> instead.
		///     </note>
		///     <para>
		///         Any handler already registered for the signal type <typeparamref name="TSignal" /> will be unregistered first.
		///         Only one handler for a particular signal type can be registered.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="handler" /> is null. </exception>
		protected void RegisterSignal <TSignal> (Action<TSignal, StateSignalInfo> handler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException(nameof(handler));
			}

			lock (this.SyncRoot)
			{
				Type type = typeof(TSignal);
				this.SignalHandlers.Remove(type);
				this.SignalHandlers.Add(type, handler);
			}
		}

		/// <summary>
		///     Unregisters the signal handler for the specified signal type.
		/// </summary>
		/// <typeparam name="TSignal"> The type of the signal. </typeparam>
		/// <remarks>
		///     <para>
		///         Nothing happens if no signal handler for the specified signal type is registered.
		///     </para>
		/// </remarks>
		protected void UnregisterSignal <TSignal> ()
		{
			lock (this.SyncRoot)
			{
				Type type = typeof(TSignal);
				this.SignalHandlers.Remove(type);
			}
		}

		#endregion




		#region Virtuals

		/// <inheritdoc cref="IState.Enter" />
		protected virtual void Enter (StateTransientInfo transientInfo)
		{
		}

		/// <inheritdoc cref="IState.Initialize" />
		protected virtual void Initialize (StateMachine stateMachine)
		{
		}

		/// <inheritdoc cref="IState.Leave" />
		protected virtual void Leave (StateTransientInfo transientInfo)
		{
		}

		/// <summary>
		///     Called by <see cref="Signal" /> for all signals which do not match with a registered signal handler.
		/// </summary>
		/// <param name="signalInfo"> The executed signal. </param>
		protected virtual void OnUnregisteredSignal (StateSignalInfo signalInfo)
		{
		}

		/// <inheritdoc cref="IState.Signal" />
		/// <remarks>
		///     <para>
		///         The default implementation checks whether signal handlers have been registered using <see cref="RegisterSignal{TSignal}" />.
		///         If a registered signal handler is found which matches the signals type, that signal handler is called.
		///         Otherwise <see cref="OnUnregisteredSignal" /> is called.
		///     </para>
		/// </remarks>
		protected virtual void Signal (StateSignalInfo signalInfo)
		{
			Type signalType = signalInfo.Signal?.GetType();

			Delegate signalHandler = null;
			if (signalType != null)
			{
				lock (this.SyncRoot)
				{
					if (this.SignalHandlers.ContainsKey(signalType))
					{
						signalHandler = this.SignalHandlers[signalType];
					}
				}
			}

			if (signalHandler != null)
			{
				signalHandler.DynamicInvoke(signalInfo.Signal, signalInfo);
			}
			else
			{
				this.OnUnregisteredSignal(signalInfo);
			}
		}

		/// <inheritdoc cref="IState.Update" />
		protected virtual void Update (StateUpdateInfo updateInfo)
		{
		}

		#endregion




		#region Interface: IState

		/// <inheritdoc />
		public bool IsActive
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.ActiveMachines.Count > 0;
				}
			}
		}

		/// <inheritdoc />
		public bool IsInitialized
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._isInitialized;
				}
			}
			protected set
			{
				lock (this.SyncRoot)
				{
					this._isInitialized = value;
				}
			}
		}

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public StateMachine StateMachine
		{
			get
			{
				lock (this.SyncRoot)
				{
					if (this.ActiveMachines.Count > 1)
					{
						throw new InvalidOperationException("An instance of the state " + this.GetType().Name + " is used in more than one state machines.");
					}
					else if (this.ActiveMachines.Count == 1)
					{
						return this.ActiveMachines.First();
					}
					else
					{
						return null;
					}
				}
			}
		}

		/// <inheritdoc />
		public object SyncRoot { get; }

		/// <inheritdoc />
		public int? UpdateInterval
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._updateInterval;
				}
			}
			protected set
			{
				lock (this.SyncRoot)
				{
					this._updateInterval = value;
				}
			}
		}

		/// <inheritdoc />
		public bool UseCaching
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._useCaching;
				}
			}
			protected set
			{
				lock (this.SyncRoot)
				{
					this._useCaching = value;
				}
			}
		}

		/// <inheritdoc />
		void IState.Enter (StateTransientInfo transientInfo)
		{
			if (transientInfo == null)
			{
				throw new ArgumentNullException(nameof(transientInfo));
			}

			lock (this.SyncRoot)
			{
				this.ActiveMachines.Add(transientInfo.StateMachine);
			}

			this.Enter(transientInfo);
		}

		/// <inheritdoc />
		public List<StateMachine> GetActiveMachines ()
		{
			lock (this.SyncRoot)
			{
				return new List<StateMachine>(this.ActiveMachines);
			}
		}

		/// <inheritdoc />
		void IState.Initialize (StateMachine stateMachine)
		{
			if (stateMachine == null)
			{
				throw new ArgumentNullException(nameof(stateMachine));
			}

			bool initialize = false;
			lock (this.SyncRoot)
			{
				if (!this.IsInitialized)
				{
					initialize = true;
					this.IsInitialized = true;
				}
			}

			if (initialize)
			{
				this.Initialize(stateMachine);
			}
		}

		/// <inheritdoc />
		void IState.Leave (StateTransientInfo transientInfo)
		{
			if (transientInfo == null)
			{
				throw new ArgumentNullException(nameof(transientInfo));
			}

			this.Leave(transientInfo);

			lock (this.SyncRoot)
			{
				this.ActiveMachines.Remove(transientInfo.StateMachine);
				this.SubMachines.ForEach(x => x.Transient(null));
			}
		}

		/// <inheritdoc />
		void IState.Signal (StateSignalInfo signalInfo)
		{
			if (signalInfo == null)
			{
				throw new ArgumentNullException(nameof(signalInfo));
			}

			this.Signal(signalInfo);
		}

		/// <inheritdoc />
		void IState.Update (StateUpdateInfo updateInfo)
		{
			if (updateInfo == null)
			{
				throw new ArgumentNullException(nameof(updateInfo));
			}

			this.Update(updateInfo);
		}

		#endregion
	}
}
