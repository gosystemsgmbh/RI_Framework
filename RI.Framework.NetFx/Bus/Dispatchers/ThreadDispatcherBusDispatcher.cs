using System;

using RI.Framework.Composition.Model;
using RI.Framework.Threading.Dispatcher;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bus.Dispatchers
{
	/// <summary>
	///     Implements a bus dispatcher which uses <see cref="IThreadDispatcher" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IBusDispatcher" /> for more details.
	///     </para>
	///     <para>
	///         Bus operations are dispatched using <see cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])" />.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public sealed class ThreadDispatcherBusDispatcher : IBusDispatcher
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ThreadDispatcherBusDispatcher" />.
		/// </summary>
		/// <param name="threadDispatcher"> The used dispatcher. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="threadDispatcher" /> is null. </exception>
		public ThreadDispatcherBusDispatcher (IThreadDispatcher threadDispatcher)
		{
			if (threadDispatcher == null)
			{
				throw new ArgumentNullException(nameof(threadDispatcher));
			}

			this.SyncRoot = new object();
			this.ThreadDispatcher = threadDispatcher;

			this.Priority = null;
			this.Options = null;
		}

		#endregion




		#region Instance Fields

		private ThreadDispatcherOptions? _options;

		private int? _priority;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets the options used for dispatching bus operations.
		/// </summary>
		/// <value>
		///     The options used for dispatching bus operations or null if the default options of the used dispatcher should be used (<see cref="IThreadDispatcher.DefaultOptions" />).
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
		///     Gets or sets the priority used for dispatching bus operations.
		/// </summary>
		/// <value>
		///     The priority used for dispatching bus operations or null if the default priority of the used dispatcher should be used (<see cref="IThreadDispatcher.DefaultPriority" />).
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

		private ThreadDispatcherOptions UsedOptions => this.Options.GetValueOrDefault(this.ThreadDispatcher.DefaultOptions);

		private int UsedPriority => this.Priority.GetValueOrDefault(this.ThreadDispatcher.DefaultPriority);

		#endregion




		#region Interface: IBusDispatcher

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; }

		/// <inheritdoc />
		public void Dispatch (Delegate action, params object[] parameters)
		{
			lock (this.SyncRoot)
			{
				this.ThreadDispatcher.Post(this.UsedPriority, this.UsedOptions, action, parameters ?? new object[0]);
			}
		}

		/// <inheritdoc />
		public void Initialize (IDependencyResolver dependencyResolver)
		{
		}

		/// <inheritdoc />
		public void Unload ()
		{
		}

		#endregion
	}
}
