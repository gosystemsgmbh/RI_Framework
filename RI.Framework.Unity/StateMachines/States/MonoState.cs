using System;
using System.Collections.Generic;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition;
using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging;
using RI.Framework.StateMachines.Resolvers;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;

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
	///         Instances of <see cref="MonoState" />s can only be used with a <see cref="IStateResolver" /> which uses a <see cref="CompositionContainer" /> (e.g. <see cref="CompositionContainerStateResolver" />) because <see cref="MonoState" />s cannot be instantiated directly (see note above).
	///     </note>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public abstract class MonoState : MonoBehaviour, IState, ILogSource
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
			gameObject.name = type.Name;
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
			this.SyncRoot = new object();

			this.IsInitialized = false;
			this.UseCaching = true;
			this.UpdateInterval = null;

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

		/// <inheritdoc cref="IState.Update" />
		protected virtual void Update (StateUpdateInfo updateInfo)
		{
		}

		#endregion




		#region Interface: ILogSource

		/// <inheritdoc />
		public LogLevel LogFilter { get; set; } = LogLevel.Debug;

		/// <inheritdoc />
		public Utilities.Logging.ILogger Logger { get; set; } = LogLocator.Logger;

		/// <inheritdoc />
		public bool LoggingEnabled { get; set; } = true;

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
