using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

using RI.Framework.Composition.Catalogs;
using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging;




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
	[Export]
	public abstract class WpfBootstrapper <TApplication> : WindowsBootstrapper<TApplication>
		where TApplication : Application
	{
		#region Overrides

		/// <summary>
		///     Called when the used application object (<see cref="Application" />) needs to be configured.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation adds the application object (<see cref="Application" />) to the used composition container as an export using a <see cref="InstanceCatalog" />, configures exception handling of the application objects dispatcher to use <see cref="Bootstrapper.StartExceptionHandling" />, sets the WPF application objects <see cref="System.Windows.Application.ShutdownMode" /> property to <see cref="ShutdownMode.OnExplicitShutdown" />, and sets the WPF application objects <see cref="WpfApplication.Bootstrapper" /> property to this boottsrapper instance (if the application object derives from <see cref="WpfApplication" />).
		///     </note>
		/// </remarks>
		protected override void ConfigureApplication ()
		{
			base.ConfigureApplication();

			if (!this.DebuggerAttached)
			{
				this.Application.DispatcherUnhandledException += (s, a) => this.StartExceptionHandling(a.Exception);
			}

			this.Application.ShutdownMode = ShutdownMode.OnExplicitShutdown;

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
		///     Dispatches a bootstrapper-specific operation for execution after bootstrapping completed.
		/// </summary>
		/// <param name="action"> The delegate to execute. </param>
		/// <param name="args"> The optional arguments for the delegate. </param>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation posts the delegate to the application objects dispatcher.
		///     </note>
		/// </remarks>
		protected override void DispatchBeginOperations (Delegate action, params object[] args)
		{
			this.Application.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new Action<Delegate, List<object>>((x, y) => x.DynamicInvoke(y.ToArray())), action, args.ToList());
		}

		/// <summary>
		///     Dispatches a bootstrapper-specific operation for execution after bootstrapping completed.
		/// </summary>
		/// <param name="action"> The delegate to execute. </param>
		/// <param name="args"> The optional arguments for the delegate. </param>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation posts the delegate to the application objects dispatcher.
		///     </note>
		/// </remarks>
		protected override void DispatchModuleInitialization (Delegate action, params object[] args)
		{
			this.Application.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action<Delegate, List<object>>((x, y) => x.DynamicInvoke(y.ToArray())), action, args.ToList());
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
