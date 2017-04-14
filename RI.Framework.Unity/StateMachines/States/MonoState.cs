using System;

using RI.Framework.Composition;
using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging;

using UnityEngine;

using Object = UnityEngine.Object;




namespace RI.Framework.StateMachines.States
{
	/// <summary>
	///     Implements a base class which can be used for <c> MonoBehaviour </c> based state implementation.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IState" /> for more details.
	///     </para>
	///     <note type="note">
	///         Instances of <see cref="MonoState" />s are not created using their constructor (as this would be the wrong way how to instantiate anything <c> MonoBehaviour </c>). Instead, <see cref="CreateInstance" /> is used.
	///     </note>
	///     <note type="important">
	///         Instances of <see cref="MonoState" />s can only be used with a <see cref="IStateResolver" /> which uses a <see cref="CompositionContainer" /> (e.g. <see cref="StateResolver" />) because <see cref="MonoState" />s cannot be instantiated directly (see note above).
	///     </note>
	/// </remarks>
	[Export]
	public abstract class MonoState : MonoBehaviour, IState
	{
		#region Static Methods

		/// <summary>
		///     Creates an instance of the specified <see cref="MonoState" /> type.
		/// </summary>
		/// <param name="type"> The type of which an instance is to be created. </param>
		/// <returns> The created instance. </returns>
		/// <remarks>
		///     <para>
		///         To instantiate a <see cref="MonoState" />, a new <c> GameObject </c> is created to which the <see cref="MonoState" /> is added as a component using <c> AddComponent </c>.
		///         The created <c> GameObject </c> has also called <c> Object.DontDestroyOnLoad </c> on it.
		///     </para>
		/// </remarks>
		/// <exception cref="System.ArgumentNullException"> <paramref name="type" /> is null. </exception>
		[ExportCreator]
		public static MonoState CreateInstance (Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			GameObject gameObject = new GameObject();
			MonoState instance = gameObject.AddComponent(type) as MonoState;
			Object.DontDestroyOnLoad(gameObject);
			return instance;
		}

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="MonoState" />.
		/// </summary>
		protected MonoState ()
		{
			this.IsInitialized = false;
			this.UseCaching = true;

			this.StateMachine = null;
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

		#endregion




		#region Instance Methods

		/// <summary>
		///     Logs a message.
		/// </summary>
		/// <param name="severity"> The severity of the message. </param>
		/// <param name="format"> The message. </param>
		/// <param name="args"> The arguments which will be expanded into the message (comparable to <see cref="string.Format(string, object[])" />). </param>
		/// <remarks>
		///     <para>
		///         <see cref="ILogService" /> is used, obtained through <see cref="LogLocator" />.
		///         If no <see cref="ILogService" /> is available, no logging is performed.
		///     </para>
		/// </remarks>
		protected void Log (LogLevel severity, string format, params object[] args)
		{
			LogLocator.Log(severity, this.GetType().Name, format, args);
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

		/// <inheritdoc cref="IState.Signal" />
		protected virtual void Signal (StateSignalInfo signalInfo)
		{
		}

		#endregion




		#region Interface: IState

		/// <inheritdoc />
		public bool IsInitialized { get; protected set; }

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

		#endregion
	}
}
