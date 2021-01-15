﻿using System;

using RI.Framework.ComponentModel;
using RI.Framework.Composition;
using RI.Framework.Composition.Catalogs;
using RI.Framework.Composition.Creators;
using RI.Framework.Composition.Model;
using RI.Framework.Services.Dispatcher;
using RI.Framework.Services.Logging;
using RI.Framework.Services.Logging.Writers;
using RI.Framework.Services.Modularization;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;

using UnityEngine;

using Object = UnityEngine.Object;




namespace RI.Framework.Bootstrapping
{
	/// <summary>
	///     Implements a default bootstrapper which is suitable for most scenarios.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The following is the sequence implemented by this bootstrapper:
	///     </para>
	///     <list type="number">
	///         <item>
	///             <para>
	///                 This <c> MonoBehaviour </c> receives the <c> Awake </c> message and calls <see cref="IBootstrapper.Run" /> on itself, which sets the following in motion:
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="State" /> is set to <see cref="BootstrapperState.Bootstrapping" />.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <c> Object.DontDestroyOnLoad </c> is called for the game object this <c> MonoBehaviour </c> is attached to.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="CreateContainer" /> is called and <see cref="Container" /> is set.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="ConfigureServiceLocator" /> is called.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="ConfigureBootstrapper" /> is called.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="ConfigureLogging" /> is called.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="ConfigureSingletons" /> is called.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="ConfigureContainer" /> is called.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="ConfigureServices" /> is called.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="ConfigureModularization" /> is called.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="State" /> is set to <see cref="BootstrapperState.Running" />.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="BeginRun" /> is called.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="IBootstrapper.Run" /> returns and the game is considered &quot;running&quot;.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 This <c> MonoBehaviour </c> receives the <c> OnApplicationQuit </c> message, which continues the sequence:
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="EndRun" /> is called.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="State" /> is set to <see cref="BootstrapperState.ShuttingDown" />.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="DoShutdown" /> is called.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="State" /> is set to <see cref="BootstrapperState.ShutDown" />.
	///             </para>
	///         </item>
	///     </list>
	///     <note type="note">
	///         <para>
	///             The sequence as described above runs only once, including the shutdown part.
	///             In cases where <c> Application.CancelQuit </c> is called during the shutdown, the shutdown procedure will complete but not run again when the game is finally exiting.
	///         </para>
	///     </note>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public class Bootstrapper : MonoBehaviour, IBootstrapper, ILogSource
	{
		#region Instance Fields

		/// <summary>
		///     Specifies whether the default dispatcher service should be used or not.
		/// </summary>
		/// <remarks>
		///     <para>
		///         If true, <see cref="Services.Dispatcher.DispatcherService" /> is added automatically, providing a default dispatcher service.
		///     </para>
		///     <para>
		///         The default value is true.
		///     </para>
		/// </remarks>
		public bool DispatcherService = true;

		/// <summary>
		///     Specifies whether the <see cref="Container" /> should be disposed during shutdown.
		/// </summary>
		/// <remarks>
		///     <para>
		///         If true, <see cref="Container" /> is disposed using <see cref="CompositionContainer.Dispose" /> during <see cref="DoShutdown" />.
		///     </para>
		///     <para>
		///         The default value is true.
		///     </para>
		/// </remarks>
		public bool DisposeContainer = true;

		/// <summary>
		///     Specifies whether the default logging service should be used or not.
		/// </summary>
		/// <remarks>
		///     <para>
		///         If true, <see cref="LogService" /> and <see cref="DebugLogWriter" /> are added automatically, providing logging through Unitys logging mechanism.
		///     </para>
		///     <para>
		///         The default value is false.
		///     </para>
		/// </remarks>
		public bool LoggingService = false;

		/// <summary>
		///     Specifies whether the used module service should initialize all modules.
		/// </summary>
		/// <remarks>
		///     <para>
		///         If true, the used <see cref="IModuleService" />, if any, initializes all modules during <see cref="BeginRun" /> (using <see cref="IModuleService.Initialize" />).
		///     </para>
		///     <para>
		///         The default value is true.
		///     </para>
		/// </remarks>
		public bool ModuleInitialization = true;

		/// <summary>
		///     Specifies whether the default module service should be used or not.
		/// </summary>
		/// <remarks>
		///     <para>
		///         If true, <see cref="Services.Modularization.ModuleService" /> is added automatically, providing a default modularization service using <see cref="IModule" /> (<see cref="MonoModuleBase" />, <see cref="ModuleBase" />, or a custom implementation of <see cref="IModule" />).
		///     </para>
		///     <para>
		///         The default value is true.
		///     </para>
		/// </remarks>
		public bool ModuleService = true;

		/// <summary>
		///     Specifies whether the used module service should unload all modules.
		/// </summary>
		/// <remarks>
		///     <para>
		///         If true, the used <see cref="IModuleService" />, if any, unloads all modules during <see cref="EndRun" /> (using <see cref="IModuleService.Unload" />).
		///     </para>
		///     <para>
		///         The default value is true.
		///     </para>
		/// </remarks>
		public bool ModuleUnloading = true;

		/// <summary>
		///     Specifies whether the <c> MonoBehaviour </c> composition creator should be used or not.
		/// </summary>
		/// <remarks>
		///     <para>
		///         If true, <see cref="Composition.Creators.MonoBehaviourCreator" /> is added automatically to <see cref="Container" />, making it possible to retrieve properly constructed instances of <c> MonoBehaviour </c>s from the container.
		///     </para>
		///     <para>
		///         The default value is true.
		///     </para>
		/// </remarks>
		public bool MonoBehaviourCreator = true;

		/// <summary>
		///     Specifies whether the default scripting catalog should be used or not.
		/// </summary>
		/// <remarks>
		///     <para>
		///         If true, <see cref="Composition.Catalogs.ScriptingCatalog" /> is added automatically to <see cref="Container" />, adding all eligible types from the scripting assembly to the container.
		///     </para>
		///     <para>
		///         The default value is true.
		///     </para>
		/// </remarks>
		public bool ScriptingCatalog = true;

		/// <summary>
		///     Specifies whether the default scripting catalog, if used, should export all types.
		/// </summary>
		/// <remarks>
		///     <para>
		///         If true, all non-abstract, non-static types, even those without an <see cref="ExportAttribute" />, are exported by the default scripting catalog (see <see cref="ScriptingCatalog"/>).
		///     </para>
		///     <para>
		///         The default value is true.
		///     </para>
		/// </remarks>
		public bool ScriptingCatalogWithAllTypes = true;

		/// <summary>
		///     Specifies whether the <see cref="ServiceLocator" /> should be bound to <see cref="Container" /> or not.
		/// </summary>
		/// <remarks>
		///     <para>
		///         If true, <see cref="Container" /> is bound to <see cref="ServiceLocator" /> using <see cref="ServiceLocator.BindToDependencyResolver" />.
		///     </para>
		///     <para>
		///         The default value is true.
		///     </para>
		/// </remarks>
		public bool ServiceLocatorBinding = true;

		private BootstrapperState _state = BootstrapperState.Uninitialized;

		#endregion




		#region Instance Properties/Indexer

		private bool ShutdownFinished { get; set; } = false;

		#endregion




		#region Instance Methods

		private void Awake ()
		{
			((IBootstrapper)this).Run();
		}

		private void OnApplicationQuit ()
		{
			if (this.ShutdownFinished)
			{
				return;
			}

			this.ShutdownFinished = true;

			if (this.State != BootstrapperState.Running)
			{
				return;
			}

			this.Log(LogLevel.Debug, "Ending run");
			this.EndRun();

			this.Log(LogLevel.Debug, "State: Shutting down");
			this.State = BootstrapperState.ShuttingDown;

			this.Log(LogLevel.Debug, "Doing shutdown");
			this.DoShutdown();

			this.Log(LogLevel.Debug, "State: Shut down");
			this.State = BootstrapperState.ShutDown;
		}

		#endregion




		#region Virtuals

		/// <summary>
		///     Called before the game begins running after the bootstrapping is completed.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation calls <see cref="IModuleService.Initialize" /> of the used <see cref="IModuleService" /> if <see cref="ModuleInitialization" /> is true, otherwise it does nothing.
		///     </note>
		/// </remarks>
		protected virtual void BeginRun ()
		{
			if (this.ModuleInitialization)
			{
				this.Log(LogLevel.Debug, "Automatically initialize modules");
				this.Container.GetExport<IModuleService>()?.Initialize();
			}
		}

		/// <summary>
		///     Called when the bootstrapper itself needs to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation adds this bootstrapper instance to the used composition container as an export using a <see cref="InstanceCatalog" />.
		///     </note>
		/// </remarks>
		protected virtual void ConfigureBootstrapper ()
		{
			CompositionBatch batch = new CompositionBatch();

			batch.AddInstance(this, typeof(IBootstrapper));
			batch.AddInstance(this, typeof(Bootstrapper));
			batch.AddInstance(this, this.GetType());

			batch.AddInstance(this.Container, typeof(IDependencyResolver));
			batch.AddInstance(this.Container, typeof(IServiceProvider));
			batch.AddInstance(this.Container, typeof(CompositionContainer));

			this.Container.Compose(batch);
		}

		/// <summary>
		///     Called when the used composition container (<see cref="Container" />) needs to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation adds a single <see cref="Composition.Catalogs.ScriptingCatalog" /> to the composition container as well as the container itself.
		///     </note>
		/// </remarks>
		protected virtual void ConfigureContainer ()
		{
			if (this.MonoBehaviourCreator)
			{
				this.Log(LogLevel.Debug, "Using default MonoBehaviour composition creator");
				this.Container.AddCreator(new MonoBehaviourCreator());
			}

			if (this.ScriptingCatalog)
			{
				this.Log(LogLevel.Debug, "Using default scripting catalog ({0})", this.ScriptingCatalogWithAllTypes ? "all types" : "explicitly exported types only");
				this.Container.AddCatalog(new ScriptingCatalog(this.ScriptingCatalogWithAllTypes));
			}
		}

		/// <summary>
		///     Called when the logging needs to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation adds <see cref="LogService" /> and <see cref="DebugLogWriter" /> to the composition container if <see cref="LoggingService" /> is true, otherwise it does nothing.
		///     </note>
		/// </remarks>
		protected virtual void ConfigureLogging ()
		{
			if (this.LoggingService)
			{
				this.Log(LogLevel.Debug, "Using default logging service");
				this.Container.AddCatalog(new TypeCatalog(typeof(LogService), typeof(DebugLogWriter)));
			}
		}

		/// <summary>
		///     Called when the modularization needs to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation adds <see cref="Services.Modularization.ModuleService" /> to the composition container if <see cref="ModuleService" /> is true, otherwise it does nothing.
		///     </note>
		/// </remarks>
		protected virtual void ConfigureModularization ()
		{
			if (this.ModuleService)
			{
				this.Log(LogLevel.Debug, "Using default module service");
				this.Container.AddCatalog(new TypeCatalog(typeof(ModuleService)));
			}
		}

		/// <summary>
		///     Called when the service locator (<see cref="ServiceLocator" />) needs to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation calls <see cref="ServiceLocator.BindToDependencyResolver" /> using the used composition container (<see cref="Container" />) if <see cref="ServiceLocatorBinding" /> is true, otherwise it does nothing.
		///     </note>
		/// </remarks>
		protected virtual void ConfigureServiceLocator ()
		{
			if (this.ServiceLocatorBinding)
			{
				this.Log(LogLevel.Debug, "Using default service locator binding");
				ServiceLocator.BindToDependencyResolver(this.Container);
			}
		}

		/// <summary>
		///     Called when all the other services of the game need to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation adds <see cref="Services.Dispatcher.DispatcherService" /> to the composition container if <see cref="DispatcherService" /> is true, otherwise it does nothing.
		///     </note>
		/// </remarks>
		protected virtual void ConfigureServices ()
		{
			if (this.DispatcherService)
			{
				this.Log(LogLevel.Debug, "Using default dispatcher service");
				this.Container.AddCatalog(new TypeCatalog(typeof(DispatcherService)));
			}
		}

		/// <summary>
		///     Called when the bootstrapper singletons are to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation sets the singleton instance for <see cref="Bootstrapper" /> and <see cref="CompositionContainer" /> (<see cref="Container" />) using <see cref="Singleton{T}" />.
		///     </note>
		/// </remarks>
		protected virtual void ConfigureSingletons ()
		{
			Singleton<IBootstrapper>.Ensure(() => this);
			Singleton<Bootstrapper>.Ensure(() => this);

			Singleton<IDependencyResolver>.Ensure(() => this.Container);
			Singleton<IServiceProvider>.Ensure(() => this.Container);
			Singleton<CompositionContainer>.Ensure(() => this.Container);
		}

		/// <summary>
		///     Called when the composition container needs to be created.
		/// </summary>
		/// <returns>
		///     The composition container to be used.
		///     Can be null if a default <see cref="CompositionContainer" /> is to be used.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation returns null so a default <see cref="CompositionContainer" /> will be created and used.
		///     </note>
		/// </remarks>
		protected virtual CompositionContainer CreateContainer ()
		{
			return null;
		}

		/// <summary>
		///     Called when the game is shut down.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation calls <see cref="CompositionContainer.Dispose" /> of the used <see cref="CompositionContainer" /> if <see cref="DisposeContainer" /> is true, otherwise it does nothing.
		///     </note>
		/// </remarks>
		protected virtual void DoShutdown ()
		{
			if (this.DisposeContainer)
			{
				this.Log(LogLevel.Debug, "Disposing container");
				this.Container?.Dispose();
			}
		}

		/// <summary>
		///     Called before the game begins shutting down after the game was running.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation calls <see cref="IModuleService.Unload" /> of the used <see cref="IModuleService" /> if <see cref="ModuleUnloading" /> is true, otherwise it does nothing.
		///     </note>
		/// </remarks>
		protected virtual void EndRun ()
		{
			if (this.ModuleUnloading)
			{
				this.Log(LogLevel.Debug, "Automatically unload modules");
				this.Container?.GetExport<IModuleService>()?.Unload();
			}
		}

		#endregion




		#region Interface: IBootstrapper

		/// <inheritdoc />
		public CompositionContainer Container { get; private set; } = null;

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public BootstrapperState State
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._state;
				}
			}
			private set
			{
				lock (this.SyncRoot)
				{
					this._state = value;
				}
			}
		}

		/// <inheritdoc />
		public object SyncRoot { get; } = new object();

		/// <inheritdoc />
		void IBootstrapper.Run ()
		{
			lock (this.SyncRoot)
			{
				if (this.State != BootstrapperState.Uninitialized)
				{
					throw new InvalidOperationException(this.GetType().Name + " is already running.");
				}

				this.Log(LogLevel.Debug, "State: Bootstrapping");
				this.State = BootstrapperState.Bootstrapping;
			}

			Object.DontDestroyOnLoad(this.gameObject);
			this.gameObject.name = this.GetType().Name;

			this.Log(LogLevel.Debug, "Creating container");
			this.Container = this.CreateContainer() ?? new CompositionContainer();

			this.Log(LogLevel.Debug, "Configuring service locator");
			this.ConfigureServiceLocator();

			this.Log(LogLevel.Debug, "Configuring bootstrapper");
			this.ConfigureBootstrapper();

			this.Log(LogLevel.Debug, "Configuring logging");
			this.ConfigureLogging();

			this.Log(LogLevel.Debug, "Configuring bootstrapper singletons");
			this.ConfigureSingletons();

			this.Log(LogLevel.Debug, "Configuring container");
			this.ConfigureContainer();

			this.Log(LogLevel.Debug, "Configuring services");
			this.ConfigureServices();

			this.Log(LogLevel.Debug, "Configuring modularization");
			this.ConfigureModularization();

			this.Log(LogLevel.Debug, "State: Running");
			this.State = BootstrapperState.Running;

			this.Log(LogLevel.Debug, "Beginning run");
			this.BeginRun();
		}

		/// <inheritdoc />
		public void Shutdown ()
		{
			this.Log(LogLevel.Debug, "Initiating shutdown");
			Application.Quit();
		}

		#endregion




		#region Interface: ILogSource

		/// <inheritdoc />
		public LogLevel LogFilter { get; set; } = LogLevel.Debug;

		/// <inheritdoc />
		public Utilities.Logging.ILogger Logger { get; set; } = LogLocator.Logger;

		/// <inheritdoc />
		public bool LoggingEnabled { get; set; } = true;

		#endregion
	}
}
