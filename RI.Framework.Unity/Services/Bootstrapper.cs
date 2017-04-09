using System;

using RI.Framework.Composition;
using RI.Framework.Composition.Catalogs;
using RI.Framework.Composition.Model;
using RI.Framework.Services.Dispatcher;
using RI.Framework.Services.Logging;
using RI.Framework.Services.Modularization;
using RI.Framework.Utilities.ObjectModel;

using UnityEngine;

using Object = UnityEngine.Object;




namespace RI.Framework.Services
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
	///                 <see cref="ConfigureBootstrapperSingletons" /> is called.
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
	[Export]
	public class Bootstrapper : MonoBehaviour,
	                            IBootstrapper
	{
		#region Instance Fields

		/// <summary>
		///     Specifies whether the default logging service should be used or not.
		/// </summary>
		/// <remarks>
		///     <para>
		/// If true, <see cref="LogService"/> and <see cref="LogWriter"/> are added automatically, providing logging through Unitys logging mechanism.
		///     </para>
		/// <para>
		/// The default value is false.
		/// </para>
		/// </remarks>
		public bool LoggingService = false;

		/// <summary>
		///     Specifies whether the default module service should be used or not.
		/// </summary>
		/// <remarks>
		///     <para>
		/// If true, <see cref="Modularization.ModuleService"/> is added automatically, providing a default modularization service using <see cref="IModule"/> or <see cref="MonoModule"/>.
		///     </para>
		/// <para>
		/// The default value is true.
		/// </para>
		/// </remarks>
		public bool ModuleService = true;

		/// <summary>
		///     Specifies whether the used module service should initialize all modules.
		/// </summary>
		/// <remarks>
		///     <para>
		/// If true, the used <see cref="IModuleService"/> initializes all modules during <see cref="BeginRun"/> (using <see cref="IModuleService.Initialize"/>).
		///     </para>
		/// <para>
		/// The default value is true.
		/// </para>
		/// </remarks>
		public bool ModuleInitialization = true;

		/// <summary>
		///     Specifies whether the used module service should unload all modules.
		/// </summary>
		/// <remarks>
		///     <para>
		/// If true, the used <see cref="IModuleService"/> unloads all modules during <see cref="EndRun"/> (using <see cref="IModuleService.Unload"/>).
		///     </para>
		/// <para>
		/// The default value is true.
		/// </para>
		/// </remarks>
		public bool ModuleUnloading = true;

		/// <summary>
		///     Specifies whether the default dispatcher service should be used or not.
		/// </summary>
		/// <remarks>
		///     <para>
		/// If true, <see cref="Dispatcher.DispatcherService"/> is added automatically, providing a default dispatcher service.
		///     </para>
		/// <para>
		/// The default value is true.
		/// </para>
		/// </remarks>
		public bool DispatcherService = true;

		/// <summary>
		///     Specifies whether the default scripting container should be used or not.
		/// </summary>
		/// <remarks>
		///     <para>
		/// If true, <see cref="Composition.Catalogs.ScriptingCatalog"/> is added automatically to <see cref="Container"/>, adding all eligible types from the scripting assembly to the container.
		///     </para>
		/// <para>
		/// The default value is true.
		/// </para>
		/// </remarks>
		public bool ScriptingCatalog = true;

		/// <summary>
		///     Specifies whether the <see cref="ServiceLocator"/> should be bound to <see cref="Container"/> or not.
		/// </summary>
		/// <remarks>
		///     <para>
		/// If true, <see cref="Container"/> is bound to <see cref="ServiceLocator"/> using <see cref="ServiceLocator.BindToCompositionContainer"/>.
		///     </para>
		/// <para>
		/// The default value is true.
		/// </para>
		/// </remarks>
		public bool ServiceLocatorBinding = true;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used composition container.
		/// </summary>
		/// <value>
		///     The used composition container.
		/// </value>
		public CompositionContainer Container { get; private set; } = null;

		/// <summary>
		///     Gets the current state of the bootstrapper.
		/// </summary>
		/// <value>
		///     The current state of the bootstrapper.
		/// </value>
		public BootstrapperState State { get; private set; } = BootstrapperState.Uninitialized;

		private bool ShutdownFinished { get; set; } = false;

		#endregion




		#region Instance Methods

		/// <summary>
		///     Logs a message.
		/// </summary>
		/// <param name="severity"> The severity of the message. </param>
		/// <param name="format"> The message. </param>
		/// <param name="args"> The arguments which will be expanded into the message (comparable to <see cref="string.Format(string, object[])" />). </param>
		/// <remarks>
		///     <para>
		///         <see cref="ILogService" /> is used, obtained through <see cref="ServiceLocator" />.
		///         If no <see cref="ILogService" /> is available, no logging is performed.
		///     </para>
		/// </remarks>
		protected void Log (LogLevel severity, string format, params object[] args)
		{
			LogLocator.Log(severity, this.GetType().Name, format, args);
		}

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
		///         The default implementation calls <see cref="IModuleService.Initialize"/> of the used <see cref="IModuleService"/> if <see cref="ModuleInitialization"/> is true, otherwise it does nothing.
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
			this.Container.AddCatalog(new InstanceCatalog(this));
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
			this.Container.AddCatalog(new InstanceCatalog(this.Container));

			if (this.ScriptingCatalog)
			{
				this.Log(LogLevel.Debug, "Using default scripting catalog");
				this.Container.AddCatalog(new ScriptingCatalog());
			}
		}

		/// <summary>
		///     Called when the logging needs to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation adds <see cref="LogService" /> and <see cref="LogWriter" /> to the composition container if <see cref="LoggingService" /> is true, otherwise it does nothing.
		///     </note>
		/// </remarks>
		protected virtual void ConfigureLogging ()
		{
			if (this.LoggingService)
			{
				this.Log(LogLevel.Debug, "Using default logging service");
				this.Container.AddCatalog(new TypeCatalog(typeof(LogService), typeof(LogWriter)));
			}
		}

		/// <summary>
		///     Called when the modularization needs to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation adds <see cref="Modularization.ModuleService" /> to the composition container if <see cref="ModuleService" /> is true, otherwise it does nothing.
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
		///         The default implementation calls <see cref="ServiceLocator.BindToCompositionContainer" /> using the used composition container (<see cref="Container" />) if <see cref="ServiceLocatorBinding"/> is true, otherwise it does nothing.
		///     </note>
		/// </remarks>
		protected virtual void ConfigureServiceLocator ()
		{
			if (this.ServiceLocatorBinding)
			{
				ServiceLocator.BindToCompositionContainer(this.Container);
			}
		}

		/// <summary>
		/// Called when the bootstrapper singletons are to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation sets the singleton instance for <see cref="Bootstrapper"/> and <see cref="CompositionContainer"/> (<see cref="Container"/>) using <see cref="Singleton{T}"/>.
		///     </note>
		/// </remarks>
		protected virtual void ConfigureBootstrapperSingletons()
		{
			Singleton<Bootstrapper>.Ensure(() => this);
			Singleton<CompositionContainer>.Ensure(() => this.Container);
		}

		/// <summary>
		///     Called when all the other services of the game need to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation adds <see cref="Dispatcher.DispatcherService" /> to the composition container if <see cref="DispatcherService" /> is true, otherwise it does nothing.
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
		///         The default implementation does nothing.
		///     </note>
		/// </remarks>
		protected virtual void DoShutdown ()
		{
		}

		/// <summary>
		///     Called before the game begins shutting down after the game was running.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation calls <see cref="IModuleService.Unload"/> of the used <see cref="IModuleService"/> if <see cref="ModuleUnloading"/> is true, otherwise it does nothing.
		///     </note>
		/// </remarks>
		protected virtual void EndRun ()
		{
			if (this.ModuleUnloading)
			{
				this.Log(LogLevel.Debug, "Automatically unload modules");
				this.Container.GetExport<IModuleService>()?.Unload();
			}
		}

		#endregion




		#region Interface: IBootstrapper

		/// <inheritdoc />
		void IBootstrapper.Run ()
		{
			if (this.State != BootstrapperState.Uninitialized)
			{
				throw new InvalidOperationException(this.GetType().Name + " is already running.");
			}

			this.Log(LogLevel.Debug, "State: Bootstrapping");
			this.State = BootstrapperState.Bootstrapping;

			Object.DontDestroyOnLoad(this.gameObject);
			this.gameObject.name = this.GetType().Name;

			this.Log(LogLevel.Debug, "Creating container");
			this.Container = this.CreateContainer() ?? new CompositionContainer();

			this.Log(LogLevel.Debug, "Configuring service locator");
			this.ConfigureServiceLocator();

			this.Log(LogLevel.Debug, "Configuring bootstrapper singletons");
			this.ConfigureBootstrapperSingletons();

			this.Log(LogLevel.Debug, "Configuring bootstrapper");
			this.ConfigureBootstrapper();

			this.Log(LogLevel.Debug, "Configuring logging");
			this.ConfigureLogging();

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
	}
}
