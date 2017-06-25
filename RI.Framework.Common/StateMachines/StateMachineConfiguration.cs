using System;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Services.Logging;
using RI.Framework.StateMachines.Caches;
using RI.Framework.StateMachines.Dispatchers;
using RI.Framework.StateMachines.Resolvers;
using RI.Framework.StateMachines.States;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.StateMachines
{
	/// <summary>
	///     Implements the base class which defines the configuration of a state machine.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="StateMachine" /> for more details about state machines.
	///     </para>
	///     <para>
	///         See the respective properties for their default values.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public abstract class StateMachineConfiguration : ISynchronizable, ICloneable<StateMachineConfiguration>, ICloneable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="StateMachineConfiguration" />.
		/// </summary>
		internal StateMachineConfiguration ()
		{
			this.SyncRoot = new object();

			this.IsLocked = false;

			this.Dispatcher = new DefaultStateDispatcher();
			this.Resolver = new DefaultStateResolver();
			this.Cache = new DefaultStateCache();

			this.EnableAutomaticCaching = true;
			this.LoggingEnabled = true;
		}

		#endregion




		#region Instance Fields

		private IStateCache _cache;

		private IStateDispatcher _dispatcher;

		private bool _enableAutomaticCaching;

		private bool _isLocked;

		private bool _loggingEnabled;

		private IStateResolver _resolver;

		#endregion




		#region Instance Properties/Indexer

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
		///         The default value is an instance of <see cref="DefaultStateCache" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> The property is being set to null while this instance is used in a <see cref="StateMachine" />. </exception>
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
		///         The default value is an instance of <see cref="DefaultStateDispatcher" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> The property is being set to null while this instance is used in a <see cref="StateMachine" />. </exception>
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
		///         The default value is an instance of <see cref="DefaultStateResolver" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> The property is being set to null while this instance is used in a <see cref="StateMachine" />. </exception>
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




		#region Abstracts

		internal abstract StateMachineConfiguration CloneInternal ();

		#endregion




		#region Interface: ICloneable<StateMachineConfiguration>

		/// <inheritdoc />
		StateMachineConfiguration ICloneable<StateMachineConfiguration>.Clone ()
		{
			return this.CloneInternal();
		}

		/// <inheritdoc />
		object ICloneable.Clone ()
		{
			return this.CloneInternal();
		}

		#endregion




		#region Interface: ISynchronizable

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; }

		#endregion
	}

	/// <summary>
	///     Implements the base class which defines the configuration of a state machine.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="StateMachine" /> for more details about state machines.
	///     </para>
	///     <para>
	///         See the respective properties for their default values.
	///     </para>
	/// </remarks>
	public abstract class StateMachineConfiguration <T> : StateMachineConfiguration, ICloneable<T>
		where T : StateMachineConfiguration<T>, new()
	{
		#region Virtuals

		/// <summary>
		///     Called when the current instance is to be cloned.
		/// </summary>
		/// <param name="clone"> The clone being created. </param>
		[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
		protected virtual void Clone (T clone)
		{
			clone.Dispatcher = (this.Dispatcher as ICloneable<IStateDispatcher>)?.Clone() ?? this.Dispatcher;
			clone.Resolver = (this.Resolver as ICloneable<IStateResolver>)?.Clone() ?? this.Resolver;
			clone.Cache = (this.Cache as ICloneable<IStateCache>)?.Clone() ?? this.Cache;

			clone.EnableAutomaticCaching = this.EnableAutomaticCaching;
			clone.LoggingEnabled = this.LoggingEnabled;
		}

		#endregion




		#region Overrides

		internal sealed override StateMachineConfiguration CloneInternal ()
		{
			return this.Clone();
		}

		#endregion




		#region Interface: ICloneable<T>

		/// <inheritdoc />
		public T Clone ()
		{
			lock (this.SyncRoot)
			{
				T clone = new T();
				this.Clone(clone);
				return clone;
			}
		}

		#endregion
	}
}
