using System;

using RI.Framework.Services.Logging;
using RI.Framework.StateMachines.Caches;
using RI.Framework.StateMachines.Dispatchers;
using RI.Framework.StateMachines.Resolvers;
using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.StateMachines
{
	/// <summary>
	///     Defines the configuration of a state machine.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="StateMachine" /> for more details about state machines.
	///     </para>
	///     <para>
	///         See the respective properties for their default values.
	///     </para>
	/// </remarks>
	public abstract class StateMachineConfiguration : ISynchronizable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="StateMachineConfiguration" />.
		/// </summary>
		protected StateMachineConfiguration ()
		{
			this.SyncRoot = new object();

			this.IsLocked = false;

			this.Dispatcher = new StateDispatcher();
			this.Resolver = new StateResolver();
			this.Cache = new StateCache();

			this.EnableAutomaticCaching = true;
			this.LoggingEnabled = true;
		}

		#endregion




		#region Instance Properties/Indexer

		private IStateCache _cache;

		/// <summary>
		///     Gets or sets the used state instance cache.
		/// </summary>
		/// <value>
		///     The used state instance cache.
		/// </value>
		/// <remarks>
		///     <note type="important">
		///         Implementations of <see cref="IStateCache" /> should not be shared among multiple state machines unless the cached <see cref="IState" /> instances know how to behave if they are the current state of more than one state machine at the same time.
		///     </note>
		///     <note type="note">
		///         The state cache of a state machine should not be changed while the current state is not null.
		///     </note>
		///     <para>
		///         The default value is an instance of <see cref="StateCache" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException">The property is being set to null while this instance is used in a <see cref="StateMachine"/>.</exception>
		public IStateCache Cache
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._cache;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					if (this.IsLocked && (value == null))
					{
						throw new ArgumentNullException(nameof(value));
					}

					this._cache = value;
				}
			}
		}

		private IStateDispatcher _dispatcher;

		/// <summary>
		///     Gets or sets the used dispatcher.
		/// </summary>
		/// <value>
		///     The used dispatcher.
		/// </value>
		/// <remarks>
		///     <para>
		///         Implementations of <see cref="IStateDispatcher" /> can be shared among multiple state machines.
		///     </para>
		///     <note type="note">
		///         The dispatcher of a state machine should not be changed while the current state is not null.
		///     </note>
		///     <para>
		///         The default value is an instance of <see cref="StateDispatcher" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException">The property is being set to null while this instance is used in a <see cref="StateMachine"/>.</exception>
		public IStateDispatcher Dispatcher
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._dispatcher;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					if (this.IsLocked && (value == null))
					{
						throw new ArgumentNullException(nameof(value));
					}

					this._dispatcher = value;
				}
			}
		}

		private bool _enableAutomaticCaching;

		/// <summary>
		///     Gets or sets whether state instances are automatically added to the cache by <see cref="StateMachine" /> after they became the current state for the first time.
		/// </summary>
		/// <value>
		///     true if a state instance is added to the cache after it became the current state for the first time, false if the state instances must be added manually.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is true.
		///     </para>
		/// </remarks>
		public bool EnableAutomaticCaching
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._enableAutomaticCaching;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._enableAutomaticCaching = value;
				}
			}
		}

		private bool _loggingEnabled;

		/// <summary>
		///     Gets or sets whether logging, using <see cref="LogLocator" />, is enabled.
		/// </summary>
		/// <value>
		///     true if logging is enabled, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is true.
		///     </para>
		/// </remarks>
		public bool LoggingEnabled
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._loggingEnabled;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._loggingEnabled = value;
				}
			}
		}

		private IStateResolver _resolver;

		/// <summary>
		///     Gets or sets the used state resolver.
		/// </summary>
		/// <value>
		///     The used state resolver.
		/// </value>
		/// <remarks>
		///     <para>
		///         Implementations of <see cref="IStateResolver" /> can be shared among multiple state machines.
		///     </para>
		///     <note type="note">
		///         The resolver of a state machine can be changed at any time.
		///     </note>
		///     <para>
		///         The default value is an instance of <see cref="StateResolver" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException">The property is being set to null while this instance is used in a <see cref="StateMachine"/>.</exception>
		public IStateResolver Resolver
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._resolver;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					if (this.IsLocked && (value == null))
					{
						throw new ArgumentNullException(nameof(value));
					}

					this._resolver = value;
				}
			}
		}

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; private set; }

		private bool _isLocked;

		internal bool IsLocked
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._isLocked;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._isLocked = value;
				}
			}
		}

		#endregion
	}
}
