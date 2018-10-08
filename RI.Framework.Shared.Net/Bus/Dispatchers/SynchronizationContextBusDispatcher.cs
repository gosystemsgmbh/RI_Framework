using System;
using System.Threading;

using RI.Framework.ComponentModel;
using RI.Framework.Composition.Model;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bus.Dispatchers
{
	/// <summary>
	///     Implements a bus dispatcher which uses <see cref="SynchronizationContext" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IBusDispatcher" /> for more details.
	///     </para>
	///     <para>
	///         Bus operations are dispatched using <see cref="System.Threading.SynchronizationContext.Post(SendOrPostCallback,object)" />.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public sealed class SynchronizationContextBusDispatcher : IBusDispatcher
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="SynchronizationContextBusDispatcher" />.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The synchronization context which is the current context when this instance is created will be used, using <see cref="System.Threading.SynchronizationContext.Current" />.
		///     </para>
		/// </remarks>
		/// <exception cref="InvalidOperationException"> There is no current synchronization context available. </exception>
		public SynchronizationContextBusDispatcher ()
		{
			SynchronizationContext context = SynchronizationContext.Current;
			if (context == null)
			{
				throw new InvalidOperationException("No current synchronization context available.");
			}

			this.SyncRoot = new object();
			this.SynchronizationContext = context;
		}

		/// <summary>
		///     Creates a new instance of <see cref="SynchronizationContextBusDispatcher" />.
		/// </summary>
		/// <param name="synchronizationContext"> The used synchronization context. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="synchronizationContext" /> is null. </exception>
		public SynchronizationContextBusDispatcher (SynchronizationContext synchronizationContext)
		{
			if (synchronizationContext == null)
			{
				throw new ArgumentNullException(nameof(synchronizationContext));
			}

			this.SyncRoot = new object();
			this.SynchronizationContext = synchronizationContext;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used synchronization context.
		/// </summary>
		/// <value>
		///     The used synchronization context.
		/// </value>
		public SynchronizationContext SynchronizationContext { get; }

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
				this.SynchronizationContext.Post(_ => action.DynamicInvoke(parameters), null);
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
