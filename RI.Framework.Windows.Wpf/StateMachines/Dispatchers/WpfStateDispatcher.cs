using System;
using System.Windows;
using System.Windows.Threading;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.StateMachines.Dispatchers
{
	/// <summary>
	///     Implements a state machine operation dispatcher which uses <see cref="System.Windows.Threading.Dispatcher" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IStateDispatcher" /> for more details.
	///     </para>
	///     <para>
	///         State machine operations are dispatched using <see cref="System.Windows.Threading.Dispatcher.BeginInvoke(DispatcherPriority,Delegate,object)" />.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class WpfStateDispatcher : IStateDispatcher
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="WpfStateDispatcher" />.
		/// </summary>
		/// <param name="application"> The application object to get the dispatcher from. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="application" /> is null. </exception>
		public WpfStateDispatcher(Application application)
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
		///     Creates a new instance of <see cref="WpfStateDispatcher" />.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher to use. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
		public WpfStateDispatcher(Dispatcher dispatcher)
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




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used dispatcher.
		/// </summary>
		/// <value>
		///     The used dispatcher.
		/// </value>
		public Dispatcher Dispatcher { get; }

		private DispatcherPriority _priority;

		/// <summary>
		/// Gets or sets the priority used for dispatching messages.
		/// </summary>
		/// <value>
		/// The priority used for dispatching messages.
		/// </value>
		/// <remarks>
		/// <para>
		/// The default value is <see cref="DispatcherPriority.Normal"/>.
		/// </para>
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




		#region Interface: IStateDispatcher

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; }

		/// <inheritdoc />
		public void DispatchSignal(StateMachineSignalDelegate signalDelegate, StateSignalInfo signalInfo)
		{
			lock (this.SyncRoot)
			{
				this.Dispatcher.BeginInvoke(this.Priority, signalDelegate, signalInfo);
			}
		}

		/// <inheritdoc />
		public void DispatchTransition(StateMachineTransientDelegate transientDelegate, StateTransientInfo transientInfo)
		{
			lock (this.SyncRoot)
			{
				this.Dispatcher.BeginInvoke(this.Priority, transientDelegate, transientInfo);
			}
		}

		/// <inheritdoc />
		public void DispatchUpdate(StateMachineUpdateDelegate updateDelegate, StateUpdateInfo updateInfo)
		{
			lock (this.SyncRoot)
			{
				this.Dispatcher.BeginInvoke(this.Priority, updateDelegate, updateInfo);
			}
		}

		#endregion
	}
}
