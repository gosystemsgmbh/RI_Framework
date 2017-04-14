using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using RI.Framework.Composition;
using RI.Framework.Composition.Catalogs;
using RI.Framework.Composition.Model;
using RI.Framework.IO.Paths;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Reflection;
using RI.Framework.Utilities.Text;




namespace RI.Framework.Services
{
	/// <summary>
	///     Implements a bootstrapper for Windows Forms applications.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The following sequence is performed when <see cref="Run" /> is called:
	///     </para>
	///     <list type="number">
	///         <item>
	///             <para>
	///                 <see cref="State" /> is set to <see cref="WinFormsBootstrapperState.Bootstrapping" />.
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
	///                 <see cref="DetermineProcessCommandLine" /> is called and <see cref="ProcessCommandLine" /> is set.
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
	///                 <see cref="CreateApplication" /> is called and <see cref="Application" /> is set.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="ConfigureApplication" /> is called.
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
	///                 <see cref="State" /> is set to <see cref="WinFormsBootstrapperState.Running" />.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="BeginRun" /> is called.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="Application" />.<see cref="System.Windows.Forms.Application.Run()" /> is called. The application is now running until <see cref="WinFormsBootstrapper.Shutdown" /> is called.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="FinishRun" /> is called.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="State" /> is set to <see cref="WinFormsBootstrapperState.ShuttingDown" />.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="BeginShutdown" /> is called.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 Remaining operations in the applications dispatcher are processed.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="FinishShutdown" /> is called.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="State" /> is set to <see cref="WinFormsBootstrapperState.ShutDown" />.
	///             </para>
	///         </item>
	///         <item>
	///             <para>
	///                 <see cref="Run" /> returns.
	///             </para>
	///         </item>
	///     </list>
	/// </remarks>
	[Export]
	public abstract class WinFormsBootstrapper : IBootstrapper
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="WinFormsBootstrapper" />.
		/// </summary>
		protected WinFormsBootstrapper ()
		{
			this.State = WinFormsBootstrapperState.Uninitialized;
			this.ShutdownInitiated = false;

			this.Container = null;
			this.Application = null;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used Windows Forms application object.
		/// </summary>
		/// <value>
		///     The used Windows Forms application object.
		/// </value>
		public ApplicationContext Application { get; private set; }

		/// <summary>
		///     Gets the main assembly of the application.
		/// </summary>
		/// <value>
		///     The main assembly of the application.
		/// </value>
		public Assembly ApplicationAssembly { get; private set; }

		/// <summary>
		///     Gets the associated company name of the application.
		/// </summary>
		/// <value>
		///     The associated company name of the application.
		/// </value>
		public string ApplicationCompanyName { get; private set; }

		/// <summary>
		///     Gets the copyright statement of the application.
		/// </summary>
		/// <value>
		///     The copyright statement of the application.
		/// </value>
		public string ApplicationCopyright { get; private set; }

		/// <summary>
		///     Gets the read- and writeable directory associated with the application used to store persistent data.
		/// </summary>
		/// <value>
		///     The read- and writeable directory associated with the application used to store persistent data.
		/// </value>
		public DirectoryPath ApplicationDataDirectory { get; private set; }

		/// <summary>
		///     Gets the read-only directory where the applications executable files are stored.
		/// </summary>
		/// <value>
		///     The read-only directory where the applications executable files are stored.
		/// </value>
		public DirectoryPath ApplicationExecutableDirectory { get; private set; }

		/// <summary>
		///     Gets the GUID of the application which is application version dependent.
		/// </summary>
		/// <value>
		///     The GUID of the application which is application version dependent.
		/// </value>
		public Guid ApplicationIdVersionDependent { get; private set; }

		/// <summary>
		///     Gets the GUID of the application which is application version independent.
		/// </summary>
		/// <value>
		///     The GUID of the application which is application version independent.
		/// </value>
		public Guid ApplicationIdVersionIndependent { get; private set; }

		/// <summary>
		///     Gets the product name of the application.
		/// </summary>
		/// <value>
		///     The product name of the application.
		/// </value>
		public string ApplicationProductName { get; private set; }

		/// <summary>
		///     Gets the version of the application.
		/// </summary>
		/// <value>
		///     The version of the application.
		/// </value>
		public Version ApplicationVersion { get; private set; }

		/// <summary>
		///     Gets the used composition container.
		/// </summary>
		/// <value>
		///     The used composition container.
		/// </value>
		public CompositionContainer Container { get; private set; }

		/// <summary>
		///     Gets the command line which was used for the current process.
		/// </summary>
		/// <value>
		///     The command line which was used for the current process.
		/// </value>
		public CommandLine ProcessCommandLine { get; private set; }

		/// <summary>
		///     Gets the GUID of the current session.
		/// </summary>
		/// <value>
		///     The GUID of the current session.
		/// </value>
		public Guid SessionId { get; private set; }

		/// <summary>
		///     Gets the timestamp of the current session.
		/// </summary>
		/// <value>
		///     The timestamp of the current session.
		/// </value>
		public DateTime SessionTimestamp { get; private set; }

		/// <summary>
		///     Gets the current state of the bootstrapper.
		/// </summary>
		/// <value>
		///     The current state of the bootstrapper.
		/// </value>
		public WinFormsBootstrapperState State { get; private set; }

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

		[SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
		private void HandleExceptionInternal (Exception exception)
		{
			try
			{
				if (exception == null)
				{
					return;
				}

				string message = exception.ToDetailedString();

				try
				{
					this.Log(LogLevel.Fatal, "EXCEPTION: {0}", message);
				}
				catch
				{
				}

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

				Environment.FailFast(message, exception);
			}
			catch
			{
			}
		}

		#endregion




		#region Virtuals

		/// <summary>
		///     Hides the splash screen.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation does nothing.
		///     </note>
		///     <note type="implement">
		///         This method is not called from <see cref="Run" />, it must be called by the application itself when it is desired to hide the splash screen.
		///     </note>
		/// </remarks>
		public virtual void HideSplashScreen ()
		{
		}

		/// <summary>
		///     Called before the application begins running after the bootstrapping is completed.
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
		///     Called before the application begins shutting down.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation does nothing.
		///     </note>
		/// </remarks>
		protected virtual void BeginShutdown ()
		{
		}

		/// <summary>
		///     Called when the used Windows Forms application object (<see cref="Application" />) needs to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation does nothing.
		///     </note>
		/// </remarks>
		protected virtual void ConfigureApplication ()
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
		///         The default implementation adds the container to itself.
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
		///         The default implementation calls <see cref="ServiceLocator.BindToCompositionContainer" /> using the used composition container (<see cref="Container" />).
		///     </note>
		/// </remarks>
		protected virtual void ConfigureServiceLocator ()
		{
			ServiceLocator.BindToCompositionContainer(this.Container);
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
		///     Called when the Windows Forms application object needs to be created.
		/// </summary>
		/// <returns>
		///     The Windows Forms application object to be used.
		///     Can be null if a default <see cref="System.Windows.Forms.ApplicationContext" /> is to be used.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation returns null so a default <see cref="System.Windows.Forms.ApplicationContext" /> will be created and used.
		///     </note>
		/// </remarks>
		protected virtual ApplicationContext CreateApplication ()
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
			return this.ApplicationAssembly.GetGuid(true, true);
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
		///     Called before the application begins shutting down after the application was running.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation does nothing.
		///     </note>
		/// </remarks>
		protected virtual void FinishRun ()
		{
		}

		/// <summary>
		///     Called after the application finished shutting down.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation does nothing.
		///     </note>
		/// </remarks>
		protected virtual void FinishShutdown ()
		{
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

		#endregion




		#region Interface: IBootstrapper

		/// <inheritdoc />
		public void Run ()
		{
			if (this.State != WinFormsBootstrapperState.Uninitialized)
			{
				throw new InvalidOperationException();
			}

			this.Log(LogLevel.Debug, "Bootstrapping");
			this.State = WinFormsBootstrapperState.Bootstrapping;

			if (!Debugger.IsAttached)
			{
				System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);
				System.Windows.Forms.Application.ThreadException += (s, a) => this.HandleExceptionInternal(a.Exception);
				AppDomain.CurrentDomain.UnhandledException += (s, a) => this.HandleExceptionInternal(a.ExceptionObject as Exception);
			}

			this.ApplicationAssembly = this.DetermineApplicationAssembly();
			this.ApplicationProductName = this.DetermineApplicationProductName();
			this.ApplicationCompanyName = this.DetermineApplicationCompanyName();
			this.ApplicationCopyright = this.DetermineApplicationCopyright();
			this.ApplicationVersion = this.DetermineApplicationVersion();
			this.ApplicationIdVersionIndependent = this.DetermineApplicationIdVersionIndependent();
			this.ApplicationIdVersionDependent = this.DetermineApplicationIdVersionDependent();

			this.ProcessCommandLine = this.DetermineProcessCommandLine();

			this.SessionTimestamp = this.DetermineSessionTimestamp();
			this.SessionId = this.DetermineSessionId();

			this.ApplicationExecutableDirectory = this.DetermineApplicationExecutableDirectory();
			this.ApplicationDataDirectory = this.DetermineApplicationDataDirectory();

			this.Log(LogLevel.Debug, "Creating container");
			this.Container = this.CreateContainer() ?? new CompositionContainer();

			this.Log(LogLevel.Debug, "Configuring service locator");
			this.ConfigureServiceLocator();

			this.Log(LogLevel.Debug, "Configuring bootstrapper");
			this.ConfigureBootstrapper();

			this.Log(LogLevel.Debug, "Configuring logging");
			this.ConfigureLogging();

			this.Log(LogLevel.Debug, "Creating application");
			this.Application = this.CreateApplication() ?? new ApplicationContext();

			this.Log(LogLevel.Debug, "Configuring application");
			this.ConfigureApplication();

			this.Log(LogLevel.Debug, "Showing splash screen");
			this.ShowSplashScreen();

			this.Log(LogLevel.Debug, "Configuring container");
			this.ConfigureContainer();

			this.Log(LogLevel.Debug, "Configuring services");
			this.ConfigureServices();

			this.Log(LogLevel.Debug, "Configuring modularization");
			this.ConfigureModularization();

			this.Log(LogLevel.Debug, "Running");
			this.State = WinFormsBootstrapperState.Running;

			this.Log(LogLevel.Debug, "Beginning run");
			this.BeginRun();

			this.Log(LogLevel.Debug, "Handing over to Windows Forms application object");
			System.Windows.Forms.Application.Run(this.Application);

			this.Log(LogLevel.Debug, "Finishing run");
			this.FinishRun();

			this.Log(LogLevel.Debug, "Shutting down");
			this.State = WinFormsBootstrapperState.ShuttingDown;

			this.Log(LogLevel.Debug, "Beginning shutdown");
			this.BeginShutdown();

			this.Log(LogLevel.Debug, "Processing remaining operations");
			System.Windows.Forms.Application.DoEvents();

			this.Log(LogLevel.Debug, "Finishing shutdown");
			this.FinishShutdown();

			this.Log(LogLevel.Debug, "Shut down");
			this.State = WinFormsBootstrapperState.ShutDown;
		}

		/// <inheritdoc />
		public void Shutdown ()
		{
			if (this.State != WinFormsBootstrapperState.Running)
			{
				throw new InvalidOperationException();
			}

			if (this.ShutdownInitiated)
			{
				return;
			}

			this.ShutdownInitiated = true;

			this.Log(LogLevel.Debug, "Initiating shutdown");
			System.Windows.Forms.Application.Exit();
		}

		#endregion
	}
}
