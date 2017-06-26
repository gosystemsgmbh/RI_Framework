using System;
using System.Collections.Generic;

using RI.Framework.StateMachines.States;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.StateMachines.Caches
{
	/// <summary>
	///     Implements a default state cache suitable for most scenarios.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="DefaultStateCache" /> internally uses a simple dictionary with the state type as key.
	///     </para>
	///     <para>
	///         See <see cref="IStateCache" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class DefaultStateCache : IStateCache
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DefaultStateCache" />.
		/// </summary>
		public DefaultStateCache ()
		{
			this.SyncRoot = new object();

			this.States = new Dictionary<Type, IState>();
		}

		#endregion




		#region Instance Properties/Indexer

		private Dictionary<Type, IState> States { get; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Gets a set of states cached in this cache.
		/// </summary>
		/// <returns>
		///     The set of states cached in this cache.
		///     An empty set is returned if no states are cached in this cache.
		/// </returns>
		public HashSet<IState> GetStates ()
		{
			lock (this.SyncRoot)
			{
				return new HashSet<IState>(this.States.Values);
			}
		}

		#endregion




		#region Interface: IStateCache

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; }

		/// <inheritdoc />
		public void AddState (IState state)
		{
			if (state == null)
			{
				throw new ArgumentNullException(nameof(state));
			}

			lock (this.SyncRoot)
			{
				Type type = state.GetType();
				if (this.States.ContainsKey(type))
				{
					this.States[type] = state;
				}
				else
				{
					this.States.Add(type, state);
				}
			}
		}

		/// <inheritdoc />
		public void Clear ()
		{
			lock (this.SyncRoot)
			{
				this.States.Clear();
			}
		}

		/// <inheritdoc />
		public bool ContainsState (Type state)
		{
			if (state == null)
			{
				throw new ArgumentNullException(nameof(state));
			}

			lock (this.SyncRoot)
			{
				return this.States.ContainsKey(state);
			}
		}

		/// <inheritdoc />
		public IState GetState (Type state)
		{
			if (state == null)
			{
				throw new ArgumentNullException(nameof(state));
			}

			lock (this.SyncRoot)
			{
				if (!this.States.ContainsKey(state))
				{
					throw new KeyNotFoundException("The state of type " + state.Name + " could not be found in the cache.");
				}

				return this.States[state];
			}
		}

		/// <inheritdoc />
		public void RemoveState (IState state)
		{
			if (state == null)
			{
				throw new ArgumentNullException(nameof(state));
			}

			lock (this.SyncRoot)
			{
				Type type = state.GetType();
				this.States.Remove(type);
			}
		}

		#endregion
	}
}
