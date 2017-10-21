﻿using System;
using System.Threading;

using RI.Framework.Threading;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bus.Dispatchers
{
	/// <summary>
	///     Implements a default bus dispatcher.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IBusDispatcher" /> for more details.
	///     </para>
	///     <para>
	///         <see cref="DefaultBusDispatcher" /> uses <see cref="SynchronizationContext" />, which is captured at the time of dispatching, to dispatch operations or falls back to <see cref="ThreadPool.QueueUserWorkItem(WaitCallback,object)" /> if no <see cref="SynchronizationContext" /> is available.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class DefaultBusDispatcher : IBusDispatcher
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DefaultBusDispatcher" />.
		/// </summary>
		public DefaultBusDispatcher ()
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
				DispatchCapture capture = new DispatchCapture(action, parameters ?? new object[0]);
				capture.Execute();
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
