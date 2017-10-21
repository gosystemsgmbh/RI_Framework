using System;
using System.Windows;
using System.Windows.Threading;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bus.Dispatchers
{
	/// <summary>
	///     Implements a bus operation dispatcher which uses <see cref="System.Windows.Threading.Dispatcher" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IBusDispatcher" /> for more details.
	///     </para>
	///     <para>
	///         Bus operations are dispatched using <see cref="System.Windows.Threading.Dispatcher.BeginInvoke(Delegate,DispatcherPriority,object[])" />.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class WpfBusDispatcher : IBusDispatcher
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="WpfBusDispatcher" />.
		/// </summary>
		/// <param name="application"> The application object to get the dispatcher from. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="application" /> is null. </exception>
		public WpfBusDispatcher (Application application)
		{
			if (application == null)
			{
				throw new ArgumentNullException(nameof(application));
			}

			this.SyncRoot = new object();
			this.Dispatcher = application.Dispatcher;

			this.Priority = DispatcherPriority.Normal;
		}

		/// <summary>
		///     Creates a new instance of <see cref="WpfBusDispatcher" />.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher to use. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
		public WpfBusDispatcher (Dispatcher dispatcher)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			this.SyncRoot = new object();
			this.Dispatcher = dispatcher;

			this.Priority = DispatcherPriority.Normal;
		}

		#endregion




		#region Instance Fields

		private DispatcherPriority _priority;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used dispatcher.
		/// </summary>
		/// <value>
		///     The used dispatcher.
		/// </value>
		public Dispatcher Dispatcher { get; }

		/// <summary>
		///     Gets or sets the priority used for dispatching messages.
		/// </summary>
		/// <value>
		///     The priority used for dispatching messages.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is <see cref="DispatcherPriority.Normal" />.
		///     </para>
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
				this.Dispatcher.BeginInvoke(action, this.Priority, parameters ?? new object[0]);
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
