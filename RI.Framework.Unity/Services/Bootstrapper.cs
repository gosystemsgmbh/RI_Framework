using System;

using RI.Framework.Composition;
using RI.Framework.Composition.Catalogs;
using RI.Framework.Composition.Model;
using RI.Framework.Services.Dispatcher;
using RI.Framework.Services.Logging;
using RI.Framework.Services.Modularization;

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
		///     Specifies whether default services should be used or not.
		/// </summary>
		/// <remarks>
		///     <para>
		///         Default services, added to the used composition container, when this field is true are the following types: <see cref="LogService" />, <see cref="LogWriter" />, <see cref="ModuleService" />, <see cref="DispatcherService" />.
		///     </para>
		///     <note type="important">
		///         If this property is true, nothing will be added to the composition container and you must add all services manually.
		///     </note>
		/// </remarks>
		public bool UseDefaultServices = true;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used composition container.
		/// </summary>
		/// <value>
		///     The used composition container.
		/// </value>
		public CompositionContainer Container { get; private set; }

		/// <summary>
		///     Gets the current state of the bootstrapper.
		/// </summary>
		/// <value>
		///     The current state of the bootstrapper.
		/// </value>
		public BootstrapperState State { get; private set; }

		private bool ShutdownFinished { get; set; }

		private bool ShutdownInitiated { get; set; }

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
			this.State = BootstrapperState.Uninitialized;
			this.ShutdownInitiated = false;
			this.ShutdownFinished = false;

			this.Container = null;

			((IBootstrapper)this).Run();
		}

		private void OnApplicationQuit ()
		{
			if (this.ShutdownFinished)
			{
				return;
			}

			this.ShutdownFinished = true;

			this.Log(LogLevel.Debug, "Ending run");
			this.EndRun();

			this.Log(LogLevel.Debug, "Shutting down");
			this.State = BootstrapperState.ShuttingDown;

			this.Log(LogLevel.Debug, "Doing shutdown");
			this.DoShutdown();

			this.Log(LogLevel.Debug, "Shut down");
			this.State = BootstrapperState.ShutDown;
		}

		#endregion




		#region Virtuals

		/// <summary>
		///     Called before the game begins running after the bootstrapping is completed.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation does nothing.
		///     </note>
		/// </remarks>
		protected virtual void BeginRun ()
		{
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
		///         The default implementation adds a single <see cref="ScriptingCatalog" /> to the composition container as well as the container itself.
		///     </note>
		/// </remarks>
		protected virtual void ConfigureContainer ()
		{
			this.Container.AddCatalog(new InstanceCatalog(this.Container));
			this.Container.AddCatalog(new ScriptingCatalog());
		}

		/// <summary>
		///     Called when the logging needs to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation adds <see cref="LogService" /> and <see cref="LogWriter" /> to the composition container if <see cref="UseDefaultServices" /> is true, otherwise it does nothing.
		///     </note>
		/// </remarks>
		protected virtual void ConfigureLogging ()
		{
			if (this.UseDefaultServices)
			{
				this.Container.AddCatalog(new TypeCatalog(typeof(LogService), typeof(LogWriter)));
			}
		}

		/// <summary>
		///     Called when the modularization needs to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation adds <see cref="ModuleService" /> to the composition container if <see cref="UseDefaultServices" /> is true, otherwise it does nothing.
		///     </note>
		/// </remarks>
		protected virtual void ConfigureModularization ()
		{
			if (this.UseDefaultServices)
			{
				this.Container.AddCatalog(new TypeCatalog(typeof(ModuleService)));
			}
		}

		/// <summary>
		///     Called when the service locator (<see cref="ServiceLocator" />) needs to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation calls <see cref="ServiceLocator.BindToCompositionContainer" /> using the used composition container (<see cref="Container" />).
		///     </note>
		/// </remarks>
		protected virtual void ConfigureServiceLocator ()
		{
			ServiceLocator.BindToCompositionContainer(this.Container);
		}

		/// <summary>
		///     Called when all the other services of the game need to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation adds <see cref="DispatcherService" /> to the composition container if <see cref="UseDefaultServices" /> is true, otherwise it does nothing.
		///     </note>
		/// </remarks>
		protected virtual void ConfigureServices ()
		{
			if (this.UseDefaultServices)
			{
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
		///         The default implementation does nothing.
		///     </note>
		/// </remarks>
		protected virtual void EndRun ()
		{
		}

		#endregion




		#region Interface: IBootstrapper

		/// <inheritdoc />
		void IBootstrapper.Run ()
		{
			if (this.State != BootstrapperState.Uninitialized)
			{
				throw new InvalidOperationException();
			}

			this.Log(LogLevel.Debug, "Bootstrapping");
			this.State = BootstrapperState.Bootstrapping;

			Object.DontDestroyOnLoad(this.gameObject);

			this.Log(LogLevel.Debug, "Creating container");
			this.Container = this.CreateContainer() ?? new CompositionContainer();

			this.Log(LogLevel.Debug, "Configuring service locator");
			this.ConfigureServiceLocator();

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

			this.Log(LogLevel.Debug, "Running");
			this.State = BootstrapperState.Running;

			this.Log(LogLevel.Debug, "Beginning run");
			this.BeginRun();
		}

		/// <inheritdoc />
		public void Shutdown ()
		{
			if (this.State != BootstrapperState.Running)
			{
				throw new InvalidOperationException();
			}

			if (this.ShutdownInitiated)
			{
				return;
			}

			this.ShutdownInitiated = true;

			this.Log(LogLevel.Debug, "Initiating shutdown");
			Application.Quit();
		}

		#endregion
	}
}
