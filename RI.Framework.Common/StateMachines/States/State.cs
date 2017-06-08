using System;
using System.Collections.Generic;

using RI.Framework.Composition.Model;




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
	[Export]
	public abstract class State : IState
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="State" />.
		/// </summary>
		protected State ()
		{
			this.IsInitialized = false;
			this.UseCaching = true;
			this.UpdateInterval = null;

			this.StateMachine = null;

			this.SignalHandlers = new Dictionary<Type, Delegate>();
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the state machine associated with this state.
		/// </summary>
		/// <value>
		///     The state machine associated with this state.
		/// </value>
		/// <remarks>
		///     <para>
		///         <see cref="StateMachine" /> is updated before each signal or transition.
		///     </para>
		/// </remarks>
		protected StateMachine StateMachine { get; private set; }

		private Dictionary<Type, Delegate> SignalHandlers { get; set; }

		#endregion




		#region Instance Methods

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
		protected void RegisterSignal <TSignal> (Action<TSignal> handler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException(nameof(handler));
			}

			Type type = typeof(TSignal);
			this.SignalHandlers.Remove(type);
			this.SignalHandlers.Add(type, handler);
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
			Type type = typeof(TSignal);
			this.SignalHandlers.Remove(type);
		}
		
		private void SetStateMachine (StateMachine stateMachine)
		{
			this.StateMachine = stateMachine;
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
			if ((signalType != null) && (this.SignalHandlers.ContainsKey(signalType)))
			{
				this.SignalHandlers[signalType].DynamicInvoke(signalInfo.Signal);
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
		public bool IsInitialized { get; protected set; }

		/// <inheritdoc />
		public int? UpdateInterval { get; protected set; }

		/// <inheritdoc />
		public bool UseCaching { get; protected set; }

		/// <inheritdoc />
		void IState.Enter (StateTransientInfo transientInfo)
		{
			if (transientInfo == null)
			{
				throw new ArgumentNullException(nameof(transientInfo));
			}

			this.SetStateMachine(transientInfo.StateMachine);
			this.Enter(transientInfo);
		}

		/// <inheritdoc />
		void IState.Initialize (StateMachine stateMachine)
		{
			if (stateMachine == null)
			{
				throw new ArgumentNullException(nameof(stateMachine));
			}

			if (!this.IsInitialized)
			{
				this.SetStateMachine(stateMachine);
				this.Initialize(stateMachine);
			}

			this.IsInitialized = true;
		}

		/// <inheritdoc />
		void IState.Leave (StateTransientInfo transientInfo)
		{
			if (transientInfo == null)
			{
				throw new ArgumentNullException(nameof(transientInfo));
			}

			this.SetStateMachine(transientInfo.StateMachine);
			this.Leave(transientInfo);
		}

		/// <inheritdoc />
		void IState.Signal (StateSignalInfo signalInfo)
		{
			if (signalInfo == null)
			{
				throw new ArgumentNullException(nameof(signalInfo));
			}

			this.SetStateMachine(signalInfo.StateMachine);
			this.Signal(signalInfo);
		}

		/// <inheritdoc />
		void IState.Update (StateUpdateInfo updateInfo)
		{
			if (updateInfo == null)
			{
				throw new ArgumentNullException(nameof(updateInfo));
			}

			this.SetStateMachine(updateInfo.StateMachine);
			this.Update(updateInfo);
		}

		#endregion
	}
}
