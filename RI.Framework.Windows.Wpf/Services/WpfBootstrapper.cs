using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

using RI.Framework.Bootstrapping;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition.Catalogs;
using RI.Framework.Utilities.Logging;




namespace RI.Framework.Services
{
	/// <summary>
	///     Implements a WPF application bootstrapper.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="WindowsBootstrapper{TApplication}" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public abstract class WpfBootstrapper <TApplication> : WindowsBootstrapper<TApplication>
		where TApplication : Application
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="WpfBootstrapper{TApplication}" />
		/// </summary>
		protected WpfBootstrapper ()
		{
			this.DispatcherExceptionHandler = this.HandleDispatcherException;
		}

		#endregion




		#region Instance Properties/Indexer

		private DispatcherUnhandledExceptionEventHandler DispatcherExceptionHandler { get; }

		#endregion




		#region Instance Methods

		private void HandleDispatcherException (object sender, DispatcherUnhandledExceptionEventArgs args) => this.StartExceptionHandling(args.Exception);

		#endregion




		#region Overrides

		/// <summary>
		///     Called when the used application object (<see cref="Application" />) needs to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation adds the application object (<see cref="Application" />) to the used composition container as an export using a <see cref="InstanceCatalog" />, configures exception handling of the application objects dispatcher to use <see cref="Bootstrapper.StartExceptionHandling" />, sets the WPF application objects <see cref="System.Windows.Application.ShutdownMode" /> property to <see cref="System.Windows.ShutdownMode.OnExplicitShutdown" />, and sets the WPF application objects <see cref="WpfApplication.Bootstrapper" /> property to this boottsrapper instance (if the application object derives from <see cref="WpfApplication" />).
		///     </note>
		/// </remarks>
		protected override void ConfigureApplication ()
		{
			base.ConfigureApplication();

			if (!this.DebuggerAttached)
			{
				this.Application.DispatcherUnhandledException += this.DispatcherExceptionHandler;
			}

			this.Application.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;

			WpfApplication wpfApplication = this.Application as WpfApplication;
			if (wpfApplication != null)
			{
				wpfApplication.Bootstrapper = this;
			}
		}

		/// <summary>
		///     Called when a default application object needs to be created.
		/// </summary>
		/// <returns>
		///     The default application object.
		///     Can be null if the use of an application object is not applicable.
		/// </returns>
		/// <remarks>
		///     <para>
		///         <see cref="CreateDefaultApplication" /> is called if <see cref="Bootstrapper.CreateApplication" /> returns null to use a default application object.
		///     </para>
		///     <note type="implement">
		///         The default implementation returns a new instance of <see cref="WpfApplication" />.
		///     </note>
		/// </remarks>
		protected override object CreateDefaultApplication ()
		{
			return new WpfApplication();
		}

		/// <summary>
		///     Used to dispatch <see cref="Bootstrapper.BeginOperations" /> for execution after bootstrapping completed.
		/// </summary>
		/// <param name="action"> The delegate to execute. </param>
		/// <param name="args"> The optional arguments for the delegate. </param>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation executes the delegate with the applications dispatcher.
		///     </note>
		/// </remarks>
		protected override void DispatchBeginOperations (Delegate action, params object[] args)
		{
			this.Application.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action<Delegate, List<object>>((x, y) => x.DynamicInvoke(y.ToArray())), action, args.ToList());
		}

		/// <summary>
		///     Used to dispatch module initialization.
		/// </summary>
		/// <param name="action"> The delegate to execute. </param>
		/// <param name="args"> The optional arguments for the delegate. </param>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation executes the delegate with the applications dispatcher.
		///     </note>
		/// </remarks>
		protected override void DispatchModuleInitialization (Delegate action, params object[] args)
		{
			this.Application.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action<Delegate, List<object>>((x, y) => x.DynamicInvoke(y.ToArray())), action, args.ToList());
		}

		/// <summary>
		///     Used to dispatch <see cref="Bootstrapper.StopOperations" /> for execution before shutdown starts.
		/// </summary>
		/// <param name="action"> The delegate to execute. </param>
		/// <param name="args"> The optional arguments for the delegate. </param>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation executes the delegate with the applications dispatcher.
		///     </note>
		/// </remarks>
		protected override void DispatchStopOperations (Delegate action, params object[] args)
		{
			this.Application.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new Action<Delegate, List<object>>((x, y) => x.DynamicInvoke(y.ToArray())), action, args.ToList());
		}

		/// <summary>
		///     Called after shutdown to cleanup all bootstrapper resources.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation calls <see cref="IDisposable.Dispose" /> on <see cref="Bootstrapper.Application" /> and then <see cref="Bootstrapper.Container" />.
		///     </note>
		/// </remarks>
		protected override void DoCleanup ()
		{
			if (this.Application != null)
			{
				this.Application.DispatcherUnhandledException -= this.DispatcherExceptionHandler;
			}

			base.DoCleanup();
		}

		/// <summary>
		///     Instructs the application to start running.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation calls <see cref="System.Windows.Application.Run()" /> and then waits for the application to shutdown.
		///     </note>
		/// </remarks>
		protected override void InitiateRun ()
		{
			this.Log(LogLevel.Debug, "Handing over to WPF application object");
			this.Application.Run();
		}

		/// <summary>
		///     Instructs the application to shutdown the application and return from <see cref="InitiateRun" />.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation calls <see cref="System.Windows.Application.Shutdown()" /> and then returns immediately.
		///     </note>
		/// </remarks>
		protected override void InitiateShutdown ()
		{
			this.Log(LogLevel.Debug, "Triggering WPF application object to shutdown");
			this.Application.Shutdown();
		}

		#endregion
	}
}
