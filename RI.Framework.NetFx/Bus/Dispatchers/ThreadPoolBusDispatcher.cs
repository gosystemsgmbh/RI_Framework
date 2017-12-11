using System;
using System.Threading;

using RI.Framework.ComponentModel;
using RI.Framework.Composition.Model;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bus.Dispatchers
{
	/// <summary>
	///     Implements a bus dispatcher which uses <see cref="ThreadPool" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IBusDispatcher" /> for more details.
	///     </para>
	///     <para>
	///         Bus operations are dispatched using <see cref="ThreadPool.QueueUserWorkItem(WaitCallback)" />.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public sealed class ThreadPoolBusDispatcher : IBusDispatcher
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ThreadPoolBusDispatcher" />.
		/// </summary>
		public ThreadPoolBusDispatcher ()
		{
			this.SyncRoot = new object();
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
				ThreadPool.QueueUserWorkItem(_ => action.DynamicInvoke(parameters));
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
