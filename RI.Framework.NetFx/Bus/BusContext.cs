using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using RI.Framework.Bus.Connections;
using RI.Framework.Bus.Containers;
using RI.Framework.Bus.Dispatchers;
using RI.Framework.Bus.Pipelines;
using RI.Framework.Bus.Serialization;
using RI.Framework.Collections;
using RI.Framework.Services.Logging;
using RI.Framework.Threading;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Bus
{
	//TODO: Document
	//TODO: Logging
	//TODO: Dispatcher configuration
	public sealed class BusContext : ISynchronizable, IDisposable, ILogSource
	{
		#region Constants

		public static readonly IEqualityComparer<string> BusNameComparer = StringComparerEx.TrimmedInvariantCultureIgnoreCase;
		public static readonly IEqualityComparer<string> NodeIdComparer = StringComparerEx.TrimmedInvariantCultureIgnoreCase;

		#endregion




		#region Static Constructor/Destructor

		static BusContext ()
		{
			BusContext.StartStopSyncRoot = new object();
		}

		#endregion




		#region Static Properties/Indexer

		private static object StartStopSyncRoot { get; set; }

		#endregion




		#region Instance Constructor/Destructor

		public BusContext ()
		{
			this.SyncRoot = new object();

			this.Nodes = new NodeCollection();
			this.Connections = new Dictionary<string, HashSet<IBusConnection>>(BusContext.BusNameComparer);
			this.Serializers = new HashSet<IBusSerializer>();
			this.Containers = new HashSet<IBusContainer>();
			this.Dispatchers = new HashSet<IBusDispatcher>();
			this.Pipelines = new HashSet<IBusPipeline>();

			this.DispatcherInternal = new HeavyThreadDispatcher();
			this.SessionId = null;
		}

		~BusContext ()
		{
			this.Stop();
		}

		#endregion




		#region Instance Fields

		private Guid? _sessionId;

		#endregion




		#region Instance Properties/Indexer

		public IThreadDispatcher ContextDispatcher => this.DispatcherInternal;
		public bool IsStarted => this.SessionId.HasValue;

		public Guid? SessionId
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._sessionId;
				}
			}
			private set
			{
				lock (this.SyncRoot)
				{
					this._sessionId = value;
				}
			}
		}

		private Dictionary<string, HashSet<IBusConnection>> Connections { get; }
		private HashSet<IBusContainer> Containers { get; }

		private HeavyThreadDispatcher DispatcherInternal { get; }
		private HashSet<IBusDispatcher> Dispatchers { get; }

		private NodeCollection Nodes { get; }
		private HashSet<IBusPipeline> Pipelines { get; }
		private HashSet<IBusSerializer> Serializers { get; }

		#endregion




		#region Instance Methods

		public void AddConnection (IBusConnection connection, params string[] busNames)
		{
			this.AddConnection(connection, (IEnumerable<string>)busNames);
		}

		public void AddConnection (IBusConnection connection, IEnumerable<string> busNames)
		{
			if (connection == null)
			{
				throw new ArgumentNullException(nameof(connection));
			}

			if (busNames == null)
			{
				throw new ArgumentNullException(nameof(busNames));
			}

			HashSet<string> enumeratedBusNames = new HashSet<string>(busNames.Select(x => x.Trim()), BusContext.BusNameComparer);

			if (enumeratedBusNames.Count == 0)
			{
				throw new ArgumentException("No bus names specified.", nameof(busNames));
			}

			if (enumeratedBusNames.Any(x => x.IsNullOrEmptyOrWhitespace()))
			{
				throw new EmptyStringArgumentException(nameof(busNames));
			}

			lock (this.SyncRoot)
			{
				this.VerifyNotStarted();

				foreach (string busName in enumeratedBusNames)
				{
					if (!this.Connections.ContainsKey(busName))
					{
						this.Connections.Add(busName, new HashSet<IBusConnection>());
					}
					this.Connections[busName].Add(connection);
				}
			}
		}

		public void AddContainer (IBusContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			lock (this.SyncRoot)
			{
				this.VerifyNotStarted();

				this.Containers.Add(container);
			}
		}

		public void AddDispatcher (IBusDispatcher dispatcher)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			lock (this.SyncRoot)
			{
				this.VerifyNotStarted();

				this.Dispatchers.Add(dispatcher);
			}
		}

		public void AddPipeline (IBusPipeline pipeline)
		{
			if (pipeline == null)
			{
				throw new ArgumentNullException(nameof(pipeline));
			}

			lock (this.SyncRoot)
			{
				this.VerifyNotStarted();

				this.Pipelines.Add(pipeline);
			}
		}

		public void AddSerializer (IBusSerializer serializer)
		{
			if (serializer == null)
			{
				throw new ArgumentNullException(nameof(serializer));
			}

			lock (this.SyncRoot)
			{
				this.VerifyNotStarted();

				this.Serializers.Add(serializer);
			}
		}

		public BusNode CreateNode (string nodeName)
		{
			if (nodeName == null)
			{
				throw new ArgumentNullException(nameof(nodeName));
			}

			if (nodeName.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(nodeName));
			}

			lock (BusContext.StartStopSyncRoot)
			{
				IThreadDispatcher dispatcher = null;
				HashSet<IBusPipeline> pipelines = null;

				lock (this.SyncRoot)
				{
					this.VerifyStarted();

					if (this.Nodes.Contains(nodeName))
					{
						return this.Nodes[nodeName];
					}

					dispatcher = this.DispatcherInternal;
					pipelines = this.GetPipelines();
				}

				BusNode node = dispatcher.Send(new Func<string, BusNode>(x => new BusNode(this, x)), nodeName) as BusNode;
				if (node == null)
				{
					throw new InvalidOperationException("Node creation failed.");
				}

				dispatcher.Send(new Action<BusNode>(x =>
				{
					foreach (IBusPipeline pipeline in pipelines)
					{
						pipeline.InitializeNode(this, node);
					}
				}), node);

				lock (this.SyncRoot)
				{
					this.Nodes.Add(node);
					return node;
				}
			}
		}

		public HashSet<IBusConnection> GetConnections ()
		{
			lock (this.SyncRoot)
			{
				HashSet<IBusConnection> connections = new HashSet<IBusConnection>();
				connections.AddRange(this.Connections.SelectMany(x => x.Value));
				return connections;
			}
		}

		public HashSet<IBusContainer> GetContainers ()
		{
			lock (this.SyncRoot)
			{
				HashSet<IBusContainer> containers = new HashSet<IBusContainer>();
				containers.AddRange(this.Containers);
				containers.AddRange(this.Resolve<IBusContainer>());
				return containers;
			}
		}

		public HashSet<IBusDispatcher> GetDispatchers ()
		{
			lock (this.SyncRoot)
			{
				HashSet<IBusDispatcher> dispatchers = new HashSet<IBusDispatcher>();
				dispatchers.AddRange(this.Dispatchers);
				dispatchers.AddRange(this.Resolve<IBusDispatcher>());
				return dispatchers;
			}
		}


		public HashSet<BusNode> GetNodes ()
		{
			lock (this.SyncRoot)
			{
				HashSet<BusNode> nodes = new HashSet<BusNode>();
				nodes.AddRange(this.Nodes);
				return nodes;
			}
		}

		public HashSet<IBusPipeline> GetPipelines ()
		{
			lock (this.SyncRoot)
			{
				HashSet<IBusPipeline> pipelines = new HashSet<IBusPipeline>();
				pipelines.AddRange(this.Pipelines);
				pipelines.AddRange(this.Resolve<IBusPipeline>());
				return pipelines;
			}
		}

		public HashSet<IBusSerializer> GetSerializers ()
		{
			lock (this.SyncRoot)
			{
				HashSet<IBusSerializer> serializers = new HashSet<IBusSerializer>();
				serializers.AddRange(this.Serializers);
				serializers.AddRange(this.Resolve<IBusSerializer>());
				return serializers;
			}
		}

		public HashSet<T> Resolve <T> ()
		{
			lock (this.SyncRoot)
			{
				HashSet<IBusContainer> containers = new HashSet<IBusContainer>(this.Containers);
				foreach (IBusContainer container in this.Containers)
				{
					containers.AddRange(container.Resolve(typeof(IBusContainer)).OfType<IBusContainer>());
				}

				HashSet<T> instances = new HashSet<T>();
				foreach (IBusContainer container in containers)
				{
					instances.AddRange(container.Resolve(typeof(T)).OfType<T>());
				}
				return instances;
			}
		}


		public void Start ()
		{
			lock (BusContext.StartStopSyncRoot)
			{
				lock (this.SyncRoot)
				{
					this.VerifyNotStarted();

					GC.ReRegisterForFinalize(this);

					Guid sessionId = Guid.NewGuid();
					this.Log(LogLevel.Information, "Starting bus context; Session ID: {0}", sessionId);

					HashSet<IBusConnection> connections = this.GetConnections();
					HashSet<IBusSerializer> serializers = this.GetSerializers();
					HashSet<IBusDispatcher> dispatchers = this.GetDispatchers();
					HashSet<IBusPipeline> pipelines = this.GetPipelines();

					if (connections.Count == 0)
					{
						throw new InvalidBusConfigurationException("No connections configured.");
					}
					if (serializers.Count == 0)
					{
						throw new InvalidBusConfigurationException("No serializers configured.");
					}
					if (dispatchers.Count == 0)
					{
						throw new InvalidBusConfigurationException("No dispatchers configured.");
					}
					if (pipelines.Count == 0)
					{
						throw new InvalidBusConfigurationException("No pipelines configured.");
					}

					bool success = false;
					try
					{
						this.DispatcherInternal.Start();
						this.Nodes.Clear();
						this.SessionId = sessionId;

						this.DispatcherInternal.Post(new Action(() =>
						{
							foreach (IBusPipeline pipeline in pipelines)
							{
								pipeline.Start(this);
							}
						}));

						success = true;
					}
					finally
					{
						if (!success)
						{
							this.Stop();
						}
					}
				}
			}
		}

		public void Stop ()
		{
			lock (BusContext.StartStopSyncRoot)
			{
				IThreadDispatcher dispatcher = null;
				HashSet<IBusPipeline> pipelines = null;

				lock (this.SyncRoot)
				{
					if (!this.IsStarted)
					{
						return;
					}

					this.Log(LogLevel.Information, "Stopping bus context; Session ID: {0}", this.SessionId.Value);

					dispatcher = this.DispatcherInternal;
					pipelines = this.GetPipelines();
				}

				dispatcher.Send(new Action(() =>
				{
					foreach (IBusPipeline pipeline in pipelines)
					{
						pipeline.Stop(this);
					}
				}));

				dispatcher.Shutdown(true);
				dispatcher.DoProcessing();

				lock (this.SyncRoot)
				{
					this.DispatcherInternal.Stop(true);
					this.Nodes.Clear();
					this.SessionId = null;

					GC.SuppressFinalize(this);
				}
			}
		}

		private void VerifyNotStarted ()
		{
			if (this.IsStarted)
			{
				throw new InvalidOperationException("The bus context is started.");
			}
		}

		private void VerifyStarted ()
		{
			if (!this.IsStarted)
			{
				throw new InvalidOperationException("The bus context is not started.");
			}
		}

		#endregion




		#region Interface: IDisposable

		void IDisposable.Dispose ()
		{
			this.Stop();
		}

		#endregion




		#region Interface: ISynchronizable

		bool ISynchronizable.IsSynchronized => true;

		public object SyncRoot { get; }

		#endregion




		#region Type: NodeCollection

		private sealed class NodeCollection : KeyedCollection<string, BusNode>
		{
			#region Instance Constructor/Destructor

			public NodeCollection ()
				: base(BusContext.NodeIdComparer)
			{
			}

			#endregion




			#region Overrides

			protected override string GetKeyForItem (BusNode item) => item.Id;

			#endregion
		}

		#endregion
	}
}
