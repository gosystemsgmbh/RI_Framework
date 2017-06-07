using System;
using System.Collections.Generic;

using RI.Framework.Collections;
using RI.Framework.Composition;
using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Bus
{
	/// <summary>
	///     Implements a default bus service which is suitable for most scenarios.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This bus service manages <see cref="IBusMessageDispatcher" />s  and <see cref="IBusConnection" />s from two sources.
	///         One are the explicitly specified dispatchers and connections added through <see cref="AddDispatcher" /> and <see cref="AddConnection" />.
	///         The second is a <see cref="CompositionContainer" /> if this <see cref="BusService" /> is added as an export (the dispatchers and connections are then imported through composition).
	///         <see cref="Dispatchers" /> gives the sequence containing all message dispatchers from all sources and <see cref="Connections" /> gives the sequence containing all message receivers from all sources.
	///     </para>
	///     <para>
	///         See <see cref="IBusService" /> for more details.
	///     </para>
	///     <note type="note">
	///         The first created instance of <see cref="BusService" /> is set as the singleton instance for <see cref="Singleton{T}" />
	///     </note>
	/// </remarks>
	public sealed class BusService : IBusService, ILogSource
	{
		/// <summary>
		/// Creates a new instance of <see cref="BusService"/>.
		/// </summary>
		public BusService ()
		{
			this.DispatchersManual = new List<IBusMessageDispatcher>();
			this.ConnectionsManual = new List<IBusConnection>();

			this.LocalNodeInternal = null;

			Singleton<BusService>.Ensure(() => this);
			Singleton<IBusService>.Ensure(() => this);
		}




		#region Instance Properties/Indexer

		[Import(typeof(IBusMessageDispatcher), Recomposable = true)]
		private Import DispatchersImported { get; set; }

		private List<IBusMessageDispatcher> DispatchersManual { get; set; }

		[Import(typeof(IBusConnection), Recomposable = true)]
		private Import ConnectionsImported { get; set; }

		private List<IBusConnection> ConnectionsManual { get; set; }

		#endregion




		#region Interface: IMessageService

		private BusNode LocalNodeInternal { get; set; }

		/// <inheritdoc cref="IBusService.LocalNode"/>
		public IBusNode LocalNode => this.LocalNodeInternal;

		/// <inheritdoc />
		public IEnumerable<IBusMessageDispatcher> Dispatchers
		{
			get
			{
				foreach (IBusMessageDispatcher dispatcher in this.DispatchersManual)
				{
					yield return dispatcher;
				}

				foreach (IBusMessageDispatcher dispatcher in this.DispatchersImported.Values<IBusMessageDispatcher>())
				{
					yield return dispatcher;
				}
			}
		}

		/// <inheritdoc />
		public IEnumerable<IBusConnection> Connections
		{
			get
			{
				foreach (IBusConnection connection in this.ConnectionsManual)
				{
					yield return connection;
				}

				foreach (IBusConnection connection in this.ConnectionsImported.Values<IBusConnection>())
				{
					yield return connection;
				}
			}
		}

		/// <inheritdoc />
		public void AddDispatcher(IBusMessageDispatcher messageDispatcher)
		{
			if (messageDispatcher == null)
			{
				throw new ArgumentNullException(nameof(messageDispatcher));
			}

			if (this.DispatchersManual.Contains(messageDispatcher))
			{
				return;
			}

			this.DispatchersManual.Add(messageDispatcher);
		}

		/// <inheritdoc />
		public void AddConnection (IBusConnection busConnection)
		{
			if (busConnection == null)
			{
				throw new ArgumentNullException(nameof(busConnection));
			}

			if (this.ConnectionsManual.Contains(busConnection))
			{
				return;
			}

			this.ConnectionsManual.Add(busConnection);
		}

		/// <inheritdoc />
		public void Start (Guid localNodeId)
		{
			this.Stop();

			bool success = false;
			try
			{
				this.LocalNodeInternal = new BusNode(localNodeId, this.Dispatchers, this.Connections);
				this.LocalNodeInternal.Start();
				success = true;
			}
			finally
			{
				if (!success)
				{
					try
					{
						this.Stop();
					}
					catch
					{
					}
				}
			}
		}

		/// <inheritdoc />
		public void Stop ()
		{
			this.LocalNodeInternal?.Stop();
			this.LocalNodeInternal = null;
		}

		/// <inheritdoc />
		public void RemoveDispatcher(IBusMessageDispatcher messageDispatcher)
		{
			if (messageDispatcher == null)
			{
				throw new ArgumentNullException(nameof(messageDispatcher));
			}

			if (!this.DispatchersManual.Contains(messageDispatcher))
			{
				return;
			}

			this.DispatchersManual.RemoveAll(messageDispatcher);
		}

		/// <inheritdoc />
		public void RemoveConnection (IBusConnection busConnection)
		{
			if (busConnection == null)
			{
				throw new ArgumentNullException(nameof(busConnection));
			}

			if (!this.ConnectionsManual.Contains(busConnection))
			{
				return;
			}

			this.ConnectionsManual.RemoveAll(busConnection);
		}

		#endregion
	}
}
