using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;

using RI.Framework.Composition;
using RI.Framework.Composition.Catalogs;
using RI.Framework.Composition.Model;
using RI.Framework.IO.Paths;
using RI.Framework.Services.Logging;
using RI.Framework.Services.Modularization;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;
using RI.Framework.Utilities.Reflection;
using RI.Framework.Utilities.Text;




namespace RI.Framework.Services
{
	/// <summary>
	///     Implements a generic bootstrapper for applications.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The following sequence is performed when <see cref="Run" /> is called:
	///     </para>
	///     <list type="number">
	///         <item>
	///             <para>
	///                 <see cref="State" /> is set to <see cref="BootstrapperState.Bootstrapping" />.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="DetermineDebuggerAttached" /> is called and <see cref="DebuggerAttached" /> is set.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="StartListeningForFirstChanceExceptions" /> is called (if <see cref="DebuggerAttached" /> is false).
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="StartupCulture" /> is set to <see cref="CultureInfo.CurrentCulture" />.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="StartupUICulture" /> is set to <see cref="CultureInfo.CurrentUICulture" />.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="Machine64Bit" /> is set to <see cref="Environment.Is64BitOperatingSystem" />.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="Session64Bit" /> is set to <see cref="Environment.Is64BitProcess" />.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="DetermineProcessCommandLine" /> is called and <see cref="ProcessCommandLine" /> is set.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="DetermineDomainId" /> is called and <see cref="DomainId" /> is set.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="DetermineMachineId" /> is called and <see cref="MachineId" /> is set.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="DetermineUserId" /> is called and <see cref="UserId" /> is set.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="DetermineApplicationAssembly" /> is called and <see cref="ApplicationAssembly" /> is set.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="DetermineApplicationProductName" /> is called and <see cref="ApplicationProductName" /> is set.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="DetermineApplicationCompanyName" /> is called and <see cref="ApplicationCompanyName" /> is set.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="DetermineApplicationCopyright" /> is called and <see cref="ApplicationCopyright" /> is set.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="DetermineApplicationVersion" /> is called and <see cref="ApplicationVersion" /> is set.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="DetermineApplicationIdVersionIndependent" /> is called and <see cref="ApplicationIdVersionIndependent" /> is set.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="DetermineApplicationIdVersionDependent" /> is called and <see cref="ApplicationIdVersionDependent" /> is set.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="DetermineSessionTimestamp" /> is called and <see cref="SessionTimestamp" /> is set.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="DetermineSessionId" /> is called and <see cref="SessionId" /> is set.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="DetermineInstanceId" /> is called and <see cref="InstanceId" /> is set.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="DetermineApplicationExecutableDirectory" /> is called and <see cref="ApplicationExecutableDirectory" /> is set.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="DetermineApplicationDataDirectory" /> is called and <see cref="ApplicationDataDirectory" /> is set.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="DeterminePreviousVersion" /> is called and <see cref="PreviousVersion" /> is set.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="DetermineFirstStart" /> is called and <see cref="FirstStart" /> is set.
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
	///                 <see cref="CreateApplication" /> and, if necessary, <see cref="CreateDefaultApplication" /> are called and <see cref="Application" /> is set.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="ConfigureApplication" /> is called.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="ConfigureSingletons" /> is called.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="ShowSplashScreen" /> is called.
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
	///                 <see cref="LogVariables" /> is called.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="State" /> is set to <see cref="BootstrapperState.Running" />.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="BeginRun" /> is called, triggering <see cref="DispatchModuleInitialization" /> and <see cref="DispatchBeginOperations" />.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="InitiateRun" /> is called. The application is now running and <see cref="Run" /> blocks until <see cref="Shutdown" /> is called.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 The application calls <see cref="Shutdown" />.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="BeginShutdown" /> is called, triggering <see cref="DispatchStopOperations" />.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="InitiateShutdown" /> is called.
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
	///         <item>
	///             <para>
	///                 <see cref="Run" /> returns.
	///             </para>
	///         </item>
	///     </list>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public abstract class Bootstrapper : LogSource, IBootstrapper
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="Bootstrapper" />.
		/// </summary>
		protected Bootstrapper ()
		{
			this.SyncRoot = new object();

			this.State = BootstrapperState.Uninitialized;
			this.HostContext = null;
			this.ShutdownInitiated = false;
			this.ShutdownInfo = null;

			this.Container = null;
			this.Application = null;

			this.FirstChanceExceptionHandler = (s, e) => this.StartFirstChanceExceptionHandling(e.Exception);
			this.ExceptionHandler = (s, a) => this.StartExceptionHandling(a.ExceptionObject as Exception);
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used application object.
		/// </summary>
		/// <value>
		///     The used application object.
		/// </value>
		protected object Application { get; private set; }

		private UnhandledExceptionEventHandler ExceptionHandler { get; }

		private EventHandler<FirstChanceExceptionEventArgs> FirstChanceExceptionHandler { get; }

		private bool ShutdownInitiated { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Logs a separator to allow quick visual distinguishing of application bootstrapping states in the a file.
		/// </summary>
		/// <remarks>
		///     <para>
		///         <see cref="ILogService" /> is used, obtained through <see cref="ServiceLocator" />.
		///         If no <see cref="ILogService" /> is available, no logging is performed.
		///     </para>
		///     <para>
		///         A separator consists of 200 dashes (<c> - </c>).
		///     </para>
		/// </remarks>
		protected void LogSeperator (string stage)
		{
			this.Log(LogLevel.Information, new string('-', 50) + (stage.IsNullOrEmptyOrWhitespace() ? string.Empty : (" " + stage + " ")) + new string('-', 150));
		}

		/// <summary>
		///     Starts listening for and handling of first chance exceptions.
		/// </summary>
		/// <remarks>
		///     <value>
		///         See <see cref="HandleFirstChanceException" /> for details.
		///     </value>
		/// </remarks>
		protected void StartListeningForFirstChanceExceptions ()
		{
			this.StopListeningForFirstChanceExceptions();
			AppDomain.CurrentDomain.FirstChanceException += this.FirstChanceExceptionHandler;
		}

		/// <summary>
		///     Stops listening for and handling of first chance exceptions.
		/// </summary>
		/// <remarks>
		///     <value>
		///         See <see cref="HandleFirstChanceException" /> for details.
		///     </value>
		/// </remarks>
		protected void StopListeningForFirstChanceExceptions ()
		{
			AppDomain.CurrentDomain.FirstChanceException -= this.FirstChanceExceptionHandler;
		}

		#endregion




		#region Abstracts

		/// <summary>
		///     Called when a default application object needs to be created.
		/// </summary>
		/// <returns>
		///     The default application object.
		///     Can be null if the use of an application object is not applicable.
		/// </returns>
		/// <remarks>
		///     <para>
		///         <see cref="CreateDefaultApplication" /> is called if <see cref="CreateApplication" /> returns null. to use a default application object.
		///     </para>
		/// </remarks>
		protected abstract object CreateDefaultApplication ();

		/// <summary>
		///     Called to determine the GUID of the domain this machine belongs to.
		/// </summary>
		/// <returns>
		///     The GUID of the the domain this machine belongs to.
		/// </returns>
		protected abstract Guid DetermineDomainId ();

		/// <summary>
		///     Called to determine the GUID of the local machine.
		/// </summary>
		/// <returns>
		///     The GUID of the local machine.
		/// </returns>
		protected abstract Guid DetermineMachineId ();

		/// <summary>
		///     Called to determine the GUID of the current user.
		/// </summary>
		/// <returns>
		///     The GUID of the current user.
		/// </returns>
		protected abstract Guid DetermineUserId ();

		/// <summary>
		///     Instructs the application to start running.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         <see cref="InitiateRun" /> must not return as long as the application is running (until <see cref="Shutdown" /> is called).
		///     </note>
		/// </remarks>
		protected abstract void InitiateRun ();

		/// <summary>
		///     Instructs the application to shutdown the application and return from <see cref="InitiateRun" />.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         Whether <see cref="InitiateShutdown" /> returns immediately or after the shutdown is completed depends on the actual bootstrapper implementation.
		///     </note>
		/// </remarks>
		protected abstract void InitiateShutdown ();

		#endregion




		#region Virtuals

		/// <summary>
		///     Called when all bootstrapping and initialization is done and actual application operations begin.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation uses the composition container to discover all implementations of <see cref="IBootstrapperOperations" /> and calls <see cref="IBootstrapperOperations.BeginOperations" /> on them.
		///     </note>
		/// </remarks>
		protected virtual void BeginOperations ()
		{
			foreach (IBootstrapperOperations ops in this.Container.GetExports<IBootstrapperOperations>())
			{
				ops.BeginOperations();
			}
		}

		/// <summary>
		///     Called before the application begins running after the bootstrapping is completed.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation performs module initialization (<see cref="IModuleService.Initialize" />), if available, and then dispatches <see cref="BeginOperations" /> using <see cref="DispatchBeginOperations" />.
		///     </note>
		/// </remarks>
		protected virtual void BeginRun ()
		{
			this.LogSeperator("BEGIN RUN");

			this.Log(LogLevel.Debug, "Dispatching initialize modules");
			this.DispatchModuleInitialization(new Action(() =>
			{
				this.LogSeperator("INITIALIZE MODULES");
				this.Log(LogLevel.Debug, "Initializing modules");
				this.Container.GetExport<IModuleService>()?.Initialize();
			}));

			this.Log(LogLevel.Debug, "Dispatching begin operations");
			this.DispatchBeginOperations(new Action(() =>
			{
				this.LogSeperator("BEGIN OPERATIONS");
				this.Log(LogLevel.Debug, "Beginning operations");
				this.BeginOperations();
			}));
		}

		/// <summary>
		///     Called before the application begins shutdown.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation dispatches <see cref="StopOperations" /> using <see cref="DispatchStopOperations" />.
		///     </note>
		/// </remarks>
		protected virtual void BeginShutdown ()
		{
			this.LogSeperator("BEGIN SHUTDOWN");

			this.Log(LogLevel.Debug, "Shutdown mode: {0}", this.ShutdownInfo.Mode);
			this.Log(LogLevel.Debug, "Exit code:     {0}", this.ShutdownInfo.ExitCode);
			this.Log(LogLevel.Debug, "Script file:   {0}", this.ShutdownInfo.ScriptFile);

			this.Log(LogLevel.Debug, "Dispatching stop operations");
			this.DispatchStopOperations(new Action(() =>
			{
				this.LogSeperator("STOP OPERATIONS");
				this.Log(LogLevel.Debug, "Stopping operations");
				this.StopOperations();
			}));
		}

		/// <summary>
		///     Called when the used application object (<see cref="Application" />) needs to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation adds the application object (<see cref="Application" />) to the used composition container as an export using a <see cref="InstanceCatalog" />.
		///     </note>
		/// </remarks>
		protected virtual void ConfigureApplication ()
		{
			if (this.Application != null)
			{
				this.Container.AddCatalog(new InstanceCatalog(this.Application));
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
		///         The default implementation adds the container (<see cref="Container" />) to itself as an export using a <see cref="InstanceCatalog" />.
		///     </note>
		/// </remarks>
		protected virtual void ConfigureContainer ()
		{
			this.Container.AddCatalog(new InstanceCatalog(this.Container));
		}

		/// <summary>
		///     Called when the logging needs to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation does nothing.
		///     </note>
		/// </remarks>
		protected virtual void ConfigureLogging ()
		{
		}

		/// <summary>
		///     Called when the modularization needs to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation does nothing.
		///     </note>
		/// </remarks>
		protected virtual void ConfigureModularization ()
		{
		}

		/// <summary>
		///     Called when the service locator (<see cref="ServiceLocator" />) needs to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation calls <see cref="ServiceLocator.BindToDependencyResolver" /> using the used composition container (<see cref="Container" />).
		///     </note>
		/// </remarks>
		protected virtual void ConfigureServiceLocator ()
		{
			ServiceLocator.BindToDependencyResolver(this.Container);
		}

		/// <summary>
		///     Called when all the other services of the application need to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation does nothing.
		///     </note>
		/// </remarks>
		protected virtual void ConfigureServices ()
		{
		}

		/// <summary>
		///     Called when the bootstrapper singletons are to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation sets the singleton instance for <see cref="Bootstrapper" /> and <see cref="CompositionContainer" /> (<see cref="Container" />).
		///     </note>
		/// </remarks>
		protected virtual void ConfigureSingletons ()
		{
			Singleton<Bootstrapper>.Ensure(() => this);
			Singleton<IBootstrapper>.Ensure(() => this);

			Singleton<CompositionContainer>.Ensure(() => this.Container);
			Singleton<IDependencyResolver>.Ensure(() => this.Container);

			if (this.Application != null)
			{
				Singleton.Set(this.Application.GetType(), this.Application);
			}
		}

		/// <summary>
		///     Creates a dictionary which contains anonymous data which can be used as additional data for crash reports.
		/// </summary>
		/// <returns>
		///     The dictionary which contains additional data for crash reports.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The dictionary uses <see cref="StringComparerEx.InvariantCultureIgnoreCase" /> for its keys.
		///     </para>
		/// </remarks>
		protected virtual Dictionary<string, string> CreateAdditionalDataForCrashReport ()
		{
			Dictionary<string, string> additionalData = new Dictionary<string, string>(StringComparerEx.InvariantCultureIgnoreCase);
			additionalData.Add(nameof(this.DebuggerAttached), this.DebuggerAttached.ToString());
			additionalData.Add(nameof(this.StartupCulture), this.StartupCulture?.ToString() ?? "[null]");
			additionalData.Add(nameof(this.StartupUICulture), this.StartupUICulture?.ToString() ?? "[null]");
			additionalData.Add(nameof(this.Machine64Bit), this.Machine64Bit.ToString());
			additionalData.Add(nameof(this.Session64Bit), this.Session64Bit.ToString());
			additionalData.Add(nameof(this.ProcessCommandLine), this.ProcessCommandLine?.ToString() ?? "[null]");
			additionalData.Add(nameof(this.DomainId), this.DomainId.ToString("N", CultureInfo.InvariantCulture));
			additionalData.Add(nameof(this.MachineId), this.MachineId.ToString("N", CultureInfo.InvariantCulture));
			additionalData.Add(nameof(this.UserId), this.UserId.ToString("N", CultureInfo.InvariantCulture));
			additionalData.Add(nameof(this.ApplicationAssembly), this.ApplicationAssembly.FullName);
			additionalData.Add(nameof(this.ApplicationProductName), this.ApplicationProductName ?? "[null]");
			additionalData.Add(nameof(this.ApplicationVersion), this.ApplicationVersion?.ToString() ?? "[null]");
			additionalData.Add(nameof(this.ApplicationIdVersionIndependent), this.ApplicationIdVersionIndependent.ToString("N", CultureInfo.InvariantCulture));
			additionalData.Add(nameof(this.ApplicationIdVersionDependent), this.ApplicationIdVersionDependent.ToString("N", CultureInfo.InvariantCulture));
			additionalData.Add(nameof(this.SessionTimestamp), this.SessionTimestamp.ToSortableString('-'));
			additionalData.Add(nameof(this.SessionId), this.SessionId.ToString("N", CultureInfo.InvariantCulture));
			additionalData.Add(nameof(this.InstanceId), this.InstanceId ?? "[null]");
			additionalData.Add(nameof(this.PreviousVersion), this.PreviousVersion?.ToString() ?? "[null]");
			additionalData.Add(nameof(this.FirstStart), this.FirstStart?.ToString() ?? "[null]");

			return additionalData;
		}

		/// <summary>
		///     Called when the application object needs to be created.
		/// </summary>
		/// <returns>
		///     The application object to be used.
		///     Can be null if a default application object is to be used.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation returns null so <see cref="CreateDefaultApplication" /> is used to create the application object.
		///     </note>
		/// </remarks>
		protected virtual object CreateApplication ()
		{
			return null;
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
		///     Called to determine the main assembly of the application (<see cref="ApplicationAssembly" />).
		/// </summary>
		/// <returns>
		///     The main assembly of the application.
		///     Cannot be null.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation uses <see cref="Assembly.GetEntryAssembly" /> to determine the main assembly.
		///     </note>
		/// </remarks>
		protected virtual Assembly DetermineApplicationAssembly ()
		{
			return Assembly.GetEntryAssembly();
		}

		/// <summary>
		///     Called to determine the associated company name of the application (<see cref="ApplicationCompanyName" />).
		/// </summary>
		/// <returns>
		///     The associated company name of the application.
		///     Cannot be null.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation uses <see cref="AssemblyExtensions.GetCompany" /> on <see cref="ApplicationAssembly" />.
		///     </note>
		/// </remarks>
		protected virtual string DetermineApplicationCompanyName ()
		{
			return this.ApplicationAssembly.GetCompany();
		}

		/// <summary>
		///     Called to determine the copyright of the application (<see cref="ApplicationCopyright" />).
		/// </summary>
		/// <returns>
		///     The copyright of the application.
		///     Cannot be null.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation uses <see cref="AssemblyExtensions.GetCopyright" /> on <see cref="ApplicationAssembly" />.
		///     </note>
		/// </remarks>
		protected virtual string DetermineApplicationCopyright ()
		{
			return this.ApplicationAssembly.GetCopyright();
		}

		/// <summary>
		///     Called to determine the read- and writeable directory associated with the application used to store persistent data (<see cref="ApplicationDataDirectory" />).
		/// </summary>
		/// <returns>
		///     The read- and writeable directory associated with the application used to store persistent data.
		///     Cannot be null.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation uses the directory returned by <see cref="Environment.GetFolderPath(Environment.SpecialFolder)" /> using <see cref="Environment.SpecialFolder.LocalApplicationData" /> appended with <see cref="ApplicationCompanyName" /> and <see cref="ApplicationProductName" />.
		///         The directory is just determined but not accessed in any way.
		///     </note>
		/// </remarks>
		protected virtual DirectoryPath DetermineApplicationDataDirectory ()
		{
			return new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)).AppendDirectory(this.ApplicationCompanyName, this.ApplicationProductName);
		}

		/// <summary>
		///     Called to determine the read-only directory where the applications executable files are stored (<see cref="ApplicationExecutableDirectory" />).
		/// </summary>
		/// <returns>
		///     The read-only directory where the applications executable files are stored.
		///     Cannot be null.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation uses <see cref="AssemblyExtensions.GetFile" /> on <see cref="ApplicationAssembly" />.
		///         The directory is just determined but not accessed in any way.
		///     </note>
		/// </remarks>
		protected virtual DirectoryPath DetermineApplicationExecutableDirectory ()
		{
			return new FilePath(this.ApplicationAssembly.GetFile()).Directory;
		}

		/// <summary>
		///     Called to determine the GUID of the application which is application version dependent (<see cref="ApplicationIdVersionDependent" />).
		/// </summary>
		/// <returns>
		///     The GUID of the application which is application version dependent.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation uses <see cref="AssemblyExtensions.GetGuid" /> on <see cref="ApplicationAssembly" />.
		///     </note>
		/// </remarks>
		protected virtual Guid DetermineApplicationIdVersionDependent ()
		{
			return this.ApplicationAssembly.GetGuid(true, false);
		}

		/// <summary>
		///     Called to determine the GUID of the application which is application version independent (<see cref="ApplicationIdVersionIndependent" />).
		/// </summary>
		/// <returns>
		///     The GUID of the application which is application version independent.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation uses <see cref="AssemblyExtensions.GetGuid" /> on <see cref="ApplicationAssembly" />.
		///     </note>
		/// </remarks>
		protected virtual Guid DetermineApplicationIdVersionIndependent ()
		{
			return this.ApplicationAssembly.GetGuid(false, true);
		}

		/// <summary>
		///     Called to determine the product name of the application (<see cref="ApplicationProductName" />).
		/// </summary>
		/// <returns>
		///     The product name of the application.
		///     Cannot be null.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation uses <see cref="AssemblyExtensions.GetProduct" /> on <see cref="ApplicationAssembly" />.
		///     </note>
		/// </remarks>
		protected virtual string DetermineApplicationProductName ()
		{
			return this.ApplicationAssembly.GetProduct();
		}

		/// <summary>
		///     Called to determine the version of the application (<see cref="ApplicationVersion" />).
		/// </summary>
		/// <returns>
		///     The version of the application.
		///     Cannot be null.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation uses <see cref="AssemblyExtensions.GetAssemblyVersion" />, <see cref="AssemblyExtensions.GetFileVersion" />, and <see cref="AssemblyExtensions.GetInformationalVersion" /> on <see cref="ApplicationAssembly" /> (in that order, whichever returns a valid version first).
		///     </note>
		/// </remarks>
		protected virtual Version DetermineApplicationVersion ()
		{
			return (this.ApplicationAssembly.GetAssemblyVersion() ?? this.ApplicationAssembly.GetFileVersion()) ?? this.ApplicationAssembly.GetInformationalVersion();
		}

		/// <summary>
		///     Called to determine whether a debugger is attached to the process or not.
		/// </summary>
		/// <returns>
		///     true if a debugger is attached to the process, false otherwise.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation uses <see cref="Debugger.IsAttached" /> to determine whether a debugger is attached to the process or not.
		///     </note>
		/// </remarks>
		protected virtual bool DetermineDebuggerAttached ()
		{
			return Debugger.IsAttached;
		}

		/// <summary>
		///     Called to determine the ID of the currently running instance of the application (<see cref="InstanceId" />).
		/// </summary>
		/// <returns>
		///     The ID of the currently running instance of the application.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation returns null and therefore indicates that multiple instances at the same time are not supported.
		///     </note>
		/// </remarks>
		protected virtual string DetermineInstanceId ()
		{
			return null;
		}

		/// <summary>
		///     Called to determine the command line of the current process (<see cref="ProcessCommandLine" />).
		/// </summary>
		/// <returns>
		///     The command line of the current process.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation uses <see cref="RI.Framework.Utilities.Text.CommandLine.Parse(string,bool,IEqualityComparer{string})" /> with <see cref="Environment" />.<see cref="Environment.CommandLine" />.
		///     </note>
		/// </remarks>
		protected virtual CommandLine DetermineProcessCommandLine ()
		{
			return CommandLine.Parse(Environment.CommandLine, true, StringComparerEx.InvariantCultureIgnoreCase);
		}

		/// <summary>
		///     Called to determine the GUID of the current session (<see cref="SessionId" />).
		/// </summary>
		/// <returns>
		///     The GUID of the current session.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation simply creates a new <see cref="Guid" /> using <see cref="Guid.NewGuid" />.
		///     </note>
		/// </remarks>
		protected virtual Guid DetermineSessionId ()
		{
			return Guid.NewGuid();
		}

		/// <summary>
		///     Called to determine the timestamp of the current session (<see cref="SessionTimestamp" />).
		/// </summary>
		/// <returns>
		///     The timestamp of the current session.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation simply returns the value of <see cref="DateTime.Now" /> at the time of calling.
		///     </note>
		/// </remarks>
		protected virtual DateTime DetermineSessionTimestamp ()
		{
			return DateTime.Now;
		}

		/// <summary>
		///     Used to dispatch <see cref="BeginOperations" /> for execution after bootstrapping completed.
		/// </summary>
		/// <param name="action"> The delegate to execute. </param>
		/// <param name="args"> The optional arguments for the delegate. </param>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation executes the delegate immediately before returning.
		///     </note>
		/// </remarks>
		protected virtual void DispatchBeginOperations (Delegate action, params object[] args)
		{
			action.DynamicInvoke(args);
		}

		/// <summary>
		///     Used to dispatch module initialization.
		/// </summary>
		/// <param name="action"> The delegate to execute. </param>
		/// <param name="args"> The optional arguments for the delegate. </param>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation executes the delegate immediately before returning.
		///     </note>
		/// </remarks>
		protected virtual void DispatchModuleInitialization (Delegate action, params object[] args)
		{
			action.DynamicInvoke(args);
		}

		/// <summary>
		///     Used to dispatch <see cref="StopOperations" /> for execution before shutdown starts.
		/// </summary>
		/// <param name="action"> The delegate to execute. </param>
		/// <param name="args"> The optional arguments for the delegate. </param>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation executes the delegate immediately before returning.
		///     </note>
		/// </remarks>
		protected virtual void DispatchStopOperations (Delegate action, params object[] args)
		{
			action.DynamicInvoke(args);
		}

		/// <summary>
		///     Called when the application is shut down.
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
		///     Called before the application begins shutting down after the application was running.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation unloads all modules and calls <see cref="HideSplashScreen" />.
		///     </note>
		/// </remarks>
		protected virtual void EndRun ()
		{
			this.LogSeperator("END RUN");

			this.Log(LogLevel.Debug, "Unloading modules");
			this.Container.GetExport<IModuleService>()?.Unload();

			this.Log(LogLevel.Debug, "Hiding splash screen");
			this.HideSplashScreen();
		}

		/// <summary>
		///     Called when an unhandled exception occurs in the application.
		/// </summary>
		/// <param name="exception"> The unhandled exception. </param>
		/// <remarks>
		///     <para>
		///         The default implementation does nothing.
		///     </para>
		///     <para>
		///         Whatever you do in this method, the application is terminated using <see cref="Environment.FailFast(string,Exception)" /> after this method returns.
		///     </para>
		/// </remarks>
		protected virtual void HandleException (Exception exception)
		{
		}

		/// <summary>
		///     Called when an exception occurs in the application.
		/// </summary>
		/// <param name="exception"> The exception. </param>
		/// <remarks>
		///     <para>
		///         The default implementation does nothing.
		///     </para>
		///     <para>
		///         First chance exceptions are exceptions which are handled immediately when they are thrown, regardless whether they are handled or not.
		///     </para>
		/// </remarks>
		protected virtual void HandleFirstChanceException (Exception exception)
		{
		}

		/// <summary>
		///     Logs some relevant bootstrapper-determined variables.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation logs the following variables using <see cref="Log" />: <see cref="ApplicationExecutableDirectory" />, <see cref="ApplicationDataDirectory" />, <see cref="ApplicationAssembly" />, <see cref="ApplicationIdVersionDependent" />, <see cref="ApplicationIdVersionIndependent" />, <see cref="ApplicationVersion" />, <see cref="SessionId" />, <see cref="SessionTimestamp" />, <see cref="ProcessCommandLine" />.
		///     </note>
		/// </remarks>
		protected virtual void LogVariables ()
		{
			this.Log(LogLevel.Debug, "Debugger attached:     {0}", this.DebuggerAttached);
			this.Log(LogLevel.Debug, "Startup culture:       {0}", this.StartupCulture?.Name ?? "[null]");
			this.Log(LogLevel.Debug, "Startup UI culture:    {0}", this.StartupUICulture?.Name ?? "[null]");
			this.Log(LogLevel.Debug, "Machine 64 bit:        {0}", this.Machine64Bit.ToString());
			this.Log(LogLevel.Debug, "Session 64 bit:        {0}", this.Session64Bit.ToString());
			this.Log(LogLevel.Debug, "Command line:          {0}", this.ProcessCommandLine?.ToString() ?? "[null]");
			this.Log(LogLevel.Debug, "Domain ID:             {0}", this.DomainId.ToString("N", CultureInfo.InvariantCulture));
			this.Log(LogLevel.Debug, "Machine ID:            {0}", this.MachineId.ToString("N", CultureInfo.InvariantCulture));
			this.Log(LogLevel.Debug, "User ID:               {0}", this.UserId.ToString("N", CultureInfo.InvariantCulture));
			this.Log(LogLevel.Debug, "Application assembly:  {0}", this.ApplicationAssembly.FullName);
			this.Log(LogLevel.Debug, "Application product:   {0}", this.ApplicationProductName ?? "[null]");
			this.Log(LogLevel.Debug, "Application company:   {0}", this.ApplicationCompanyName ?? "[null]");
			this.Log(LogLevel.Debug, "Application copyright: {0}", this.ApplicationCopyright ?? "[null]");
			this.Log(LogLevel.Debug, "Application version:   {0}", this.ApplicationVersion?.ToString() ?? "[null]");
			this.Log(LogLevel.Debug, "Application ID (-V):   {0}", this.ApplicationIdVersionIndependent.ToString("N", CultureInfo.InvariantCulture));
			this.Log(LogLevel.Debug, "Application ID (+V):   {0}", this.ApplicationIdVersionDependent.ToString("N", CultureInfo.InvariantCulture));
			this.Log(LogLevel.Debug, "Session timestamp:     {0}", this.SessionTimestamp.ToSortableString('-'));
			this.Log(LogLevel.Debug, "Session ID:            {0}", this.SessionId.ToString("N", CultureInfo.InvariantCulture));
			this.Log(LogLevel.Debug, "Instance ID:           {0}", this.InstanceId ?? "[null]");
			this.Log(LogLevel.Debug, "Executable directory:  {0}", this.ApplicationExecutableDirectory?.PathResolved ?? "[null]");
			this.Log(LogLevel.Debug, "Data directory:        {0}", this.ApplicationDataDirectory?.PathResolved ?? "[null]");
			this.Log(LogLevel.Debug, "Application object:    {0}", this.Application?.ToString() ?? "[null]");
			this.Log(LogLevel.Debug, "First start:           {0}", this.FirstStart?.ToString() ?? "[null]");
			this.Log(LogLevel.Debug, "Previous version:      {0}", this.PreviousVersion?.ToString() ?? "[null]");
		}

		/// <summary>
		///     Called when the splash screen can be created and shown.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation does nothing.
		///     </note>
		/// </remarks>
		protected virtual void ShowSplashScreen ()
		{
		}

		/// <summary>
		///     Called before the bootstrapper starts shutting down and everything is still initialized and available.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation uses the composition container to discover all implementations of <see cref="IBootstrapperOperations" /> and calls <see cref="IBootstrapperOperations.StopOperations" /> on them.
		///     </note>
		/// </remarks>
		protected virtual void StopOperations ()
		{
			foreach (IBootstrapperOperations ops in this.Container.GetExports<IBootstrapperOperations>())
			{
				ops.StopOperations();
			}
		}

		/// <summary>
		/// Called after shutdown to cleanup all bootstrapper resources.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default application first calls <see cref="IDisposable.Dispose"/>
		///     </note>
		/// </remarks>
		protected virtual void DoCleanup ()
		{
			IDisposable application = this.Application as IDisposable;
			if (application != null)
			{
				this.Log(LogLevel.Debug, "Disposing application");
				application.Dispose();
			}

			if (this.Container != null)
			{
				this.Log(LogLevel.Debug, "Disposing container");
				this.Container.Dispose();
			}
		}

		/// <summary>
		/// Implements the reset of the previous version information.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Whatever this method does, <see cref="PreviousVersion"/> is reset anyway.
		/// </para>
		///     <note type="implement">
		///         The default implementation does nothing.
		///     </note>
		/// </remarks>
		protected virtual void ResetPreviousVersionImpl ()
		{
		}

		/// <summary>
		/// Implements the reset of the first start indication.
		/// </summary>
		/// <param name="indicators">The indicators to reset.</param>
		/// <remarks>
		/// <para>
		/// Whatever this method does, <see cref="FirstStart"/> is reset anyway.
		/// </para>
		///     <note type="implement">
		///         The default implementation does nothing.
		///     </note>
		/// </remarks>
		protected virtual void ResetFirstStartImpl(FirstStart indicators)
		{
		}

		/// <summary>
		///     Called to determine whether and how the application is started for the first time (<see cref="FirstStart"/>).
		/// </summary>
		/// <returns>
		///     The first start indicators or null if the information is not available or first start indication is not used.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation does nothing and returns null.
		///     </note>
		/// </remarks>
		protected virtual FirstStart? DetermineFirstStart ()
		{
			return null;
		}

		/// <summary>
		///     Called to determine the timestamp of the current session (<see cref="SessionTimestamp" />).
		/// </summary>
		/// <returns>
		///     The previous application version or null if the application is started for the first time on this machine, the information is not vailable, or the previous version information is not used.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation does nothing and returns null.
		///     </note>
		/// </remarks>
		protected virtual Version DeterminePreviousVersion ()
		{
			return null;
		}

		#endregion




		#region Interface: IBootstrapper

		/// <inheritdoc />
		public Assembly ApplicationAssembly { get; private set; }

		/// <inheritdoc />
		public string ApplicationCompanyName { get; private set; }

		/// <inheritdoc />
		public string ApplicationCopyright { get; private set; }

		/// <inheritdoc />
		public DirectoryPath ApplicationDataDirectory { get; private set; }

		/// <inheritdoc />
		public DirectoryPath ApplicationExecutableDirectory { get; private set; }

		/// <inheritdoc />
		public Guid ApplicationIdVersionDependent { get; private set; }

		/// <inheritdoc />
		public Guid ApplicationIdVersionIndependent { get; private set; }

		/// <inheritdoc />
		public string ApplicationProductName { get; private set; }

		/// <inheritdoc />
		public Version ApplicationVersion { get; private set; }

		/// <inheritdoc />
		public CompositionContainer Container { get; private set; }

		/// <inheritdoc />
		public bool DebuggerAttached { get; private set; }

		/// <inheritdoc />
		public Guid DomainId { get; private set; }

		/// <inheritdoc />
		public HostContext HostContext { get; private set; }

		/// <inheritdoc />
		public string InstanceId { get; private set; }

		/// <inheritdoc />
		public bool Machine64Bit { get; private set; }

		/// <inheritdoc />
		public Guid MachineId { get; private set; }

		/// <inheritdoc />
		public CommandLine ProcessCommandLine { get; private set; }

		/// <inheritdoc />
		public bool Session64Bit { get; private set; }

		/// <inheritdoc />
		public Guid SessionId { get; private set; }

		/// <inheritdoc />
		public DateTime SessionTimestamp { get; private set; }

		private ShutdownInfo _shutdownInfo;

		/// <inheritdoc />
		public ShutdownInfo ShutdownInfo
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._shutdownInfo;
				}
			}
			private set
			{
				lock (this.SyncRoot)
				{
					this._shutdownInfo = value;
				}
			}
		}

		/// <inheritdoc />
		public CultureInfo StartupCulture { get; private set; }

		/// <inheritdoc />
		public CultureInfo StartupUICulture { get; private set; }

		private BootstrapperState _state;

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
		public Guid UserId { get; private set; }

		private FirstStart? _firstStart;

		/// <inheritdoc />
		public FirstStart? FirstStart
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._firstStart;
				}
			}
			private set
			{
				lock (this.SyncRoot)
				{
					this._firstStart = value;
				}
			}
		}

		private Version _previousVersion;

		/// <inheritdoc />
		public Version PreviousVersion
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._previousVersion;
				}
			}
			private set
			{
				lock (this.SyncRoot)
				{
					this._previousVersion = value;
				}
			}
		}

		/// <inheritdoc />
		public void ResetFirstStart (FirstStart? indicators)
		{
			FirstStart usedIndicators = indicators ?? Services.FirstStart.All;
			this.ResetFirstStartImpl(usedIndicators);
			this.FirstStart = this.FirstStart.HasValue ? (this.FirstStart & (~usedIndicators)) : null;
		}

		/// <inheritdoc />
		public void ResetFirstStart () => this.ResetFirstStart(null);

		/// <inheritdoc />
		public void ResetPreviousVersion ()
		{
			this.ResetPreviousVersionImpl();
			this.PreviousVersion = null;
		}

		/// <inheritdoc />
		public virtual void HideSplashScreen ()
		{
		}

		/// <inheritdoc />
		[SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
		public ShutdownInfo Run (HostContext hostContext)
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

			try
			{
				this.HostContext = hostContext;

				this.DebuggerAttached = this.DetermineDebuggerAttached();
				if (!this.DebuggerAttached)
				{
					AppDomain.CurrentDomain.UnhandledException += this.ExceptionHandler;
					this.StartListeningForFirstChanceExceptions();
				}

				this.StartupCulture = CultureInfo.CurrentCulture;
				this.StartupUICulture = CultureInfo.CurrentUICulture;

				this.Machine64Bit = Environment.Is64BitOperatingSystem;
				this.Session64Bit = Environment.Is64BitProcess;

				this.ProcessCommandLine = this.DetermineProcessCommandLine();

				this.DomainId = this.DetermineDomainId();
				this.MachineId = this.DetermineMachineId();
				this.UserId = this.DetermineUserId();

				this.ApplicationAssembly = this.DetermineApplicationAssembly();
				this.ApplicationProductName = this.DetermineApplicationProductName();
				this.ApplicationCompanyName = this.DetermineApplicationCompanyName();
				this.ApplicationCopyright = this.DetermineApplicationCopyright();
				this.ApplicationVersion = this.DetermineApplicationVersion();
				this.ApplicationIdVersionIndependent = this.DetermineApplicationIdVersionIndependent();
				this.ApplicationIdVersionDependent = this.DetermineApplicationIdVersionDependent();

				this.SessionTimestamp = this.DetermineSessionTimestamp();
				this.SessionId = this.DetermineSessionId();

				this.InstanceId = this.DetermineInstanceId();

				this.ApplicationExecutableDirectory = this.DetermineApplicationExecutableDirectory();
				this.ApplicationDataDirectory = this.DetermineApplicationDataDirectory();

				this.PreviousVersion = this.DeterminePreviousVersion();
				this.FirstStart = this.DetermineFirstStart();

				this.Log(LogLevel.Debug, "Creating container");
				this.Container = this.CreateContainer() ?? new CompositionContainer();

				this.Log(LogLevel.Debug, "Configuring service locator");
				this.ConfigureServiceLocator();

				this.Log(LogLevel.Debug, "Configuring bootstrapper");
				this.ConfigureBootstrapper();

				this.Log(LogLevel.Debug, "Configuring logging");
				this.ConfigureLogging();

				this.Log(LogLevel.Debug, "Creating application");
				this.Application = this.CreateApplication() ?? this.CreateDefaultApplication();

				this.Log(LogLevel.Debug, "Configuring application");
				this.ConfigureApplication();

				this.Log(LogLevel.Debug, "Configuring singletons");
				this.ConfigureSingletons();

				this.Log(LogLevel.Debug, "Showing splash screen");
				this.ShowSplashScreen();

				this.Log(LogLevel.Debug, "Configuring container");
				this.ConfigureContainer();

				this.Log(LogLevel.Debug, "Configuring services");
				this.ConfigureServices();

				this.Log(LogLevel.Debug, "Configuring modularization");
				this.ConfigureModularization();

				this.Log(LogLevel.Debug, "Logging variables");
				this.LogVariables();

				this.Log(LogLevel.Debug, "State: Running");
				this.State = BootstrapperState.Running;

				this.Log(LogLevel.Debug, "Beginning run");
				this.BeginRun();

				this.Log(LogLevel.Debug, "Initiating run");
				this.InitiateRun();

				this.Log(LogLevel.Debug, "Ending run");
				this.EndRun();

				this.Log(LogLevel.Debug, "State: Shutting down");
				this.State = BootstrapperState.ShuttingDown;

				this.Log(LogLevel.Debug, "Doing shutdown");
				this.DoShutdown();

				this.Log(LogLevel.Debug, "Doing cleanup");
				this.DoCleanup();

				this.Log(LogLevel.Debug, "State: Shut down");
				this.State = BootstrapperState.ShutDown;

				return this.ShutdownInfo;
			}
			finally
			{
				try
				{
					this.StopListeningForFirstChanceExceptions();
				}
				catch
				{
				}

				try
				{
					AppDomain.CurrentDomain.UnhandledException -= this.ExceptionHandler;
				}
				catch
				{
				}
			}
		}

		/// <inheritdoc />
		public void Shutdown (ShutdownInfo shutdownInfo)
		{
			lock (this.SyncRoot)
			{
				if (this.State != BootstrapperState.Running)
				{
					throw new InvalidOperationException(this.GetType().Name + " is not running.");
				}

				if (this.ShutdownInitiated)
				{
					return;
				}

				this.ShutdownInitiated = true;

				this.ShutdownInfo = shutdownInfo ?? new ShutdownInfo();
			}

			this.Log(LogLevel.Debug, "Beginning shutdown");
			this.BeginShutdown();

			this.Log(LogLevel.Debug, "Initiating shutdown");
			this.InitiateShutdown();
		}

		/// <inheritdoc />
		[SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
		public void StartExceptionHandling (Exception exception)
		{
			try
			{
				if (exception == null)
				{
					return;
				}

				string message = "[EXCEPTION]";
				try
				{
					message = exception.ToDetailedString();
				}
				catch
				{
				}

				try
				{
					this.Log(LogLevel.Fatal, "EXCEPTION: {0}", message);
				}
				catch
				{
				}

				lock (this.SyncRoot)
				{
					if (this.ShutdownInitiated && (exception is ThreadAbortException))
					{
						return;
					}

					try
					{
						this.HandleException(exception);
					}
					catch
					{
					}
				}

				Environment.FailFast(message, exception);
			}
			catch
			{
			}
		}

		/// <inheritdoc />
		[SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
		public void StartFirstChanceExceptionHandling (Exception exception)
		{
			try
			{
				if (exception == null)
				{
					return;
				}

				string message = "[FIRST CHANCE EXCEPTION]";
				try
				{
					message = exception.ToDetailedString();
				}
				catch
				{
				}

				try
				{
					this.Log(LogLevel.Warning, "FIRST CHANCE EXCEPTION: {0}", message);
				}
				catch
				{
				}

				lock (this.SyncRoot)
				{
					if (this.ShutdownInitiated && (exception is ThreadAbortException))
					{
						return;
					}

					try
					{
						this.HandleFirstChanceException(exception);
					}
					catch
					{
					}
				}
			}
			catch
			{
			}
		}

		#endregion


		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; }
	}

	/// <inheritdoc cref="Bootstrapper" />
	/// <typeparam name="TApplication"> The type of the used application object. </typeparam>
	public abstract class Bootstrapper <TApplication> : Bootstrapper
		where TApplication : class
	{
		#region Instance Properties/Indexer

		/// <inheritdoc cref="Bootstrapper.Application" />
		public new TApplication Application => (TApplication)base.Application;

		#endregion




		#region Overrides

		/// <summary>
		///     Called when the bootstrapper singletons are to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation sets the singleton instance for <see cref="Bootstrapper" />, <see cref="CompositionContainer" /> (<see cref="Bootstrapper.Container" />), and the application object (<see cref="Bootstrapper.Application" />) using <see cref="Singleton{T}" />.
		///     </note>
		/// </remarks>
		protected override void ConfigureSingletons ()
		{
			base.ConfigureSingletons();

			if (this.Application != null)
			{
				Singleton<TApplication>.Ensure(() => this.Application);
			}
		}

		#endregion
	}
}
