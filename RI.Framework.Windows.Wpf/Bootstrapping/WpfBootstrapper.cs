using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Utilities.Logging;




namespace RI.Framework.Bootstrapping
{
	/// <summary>
	///     Implements a default WPF application bootstrapper.
	/// </summary>
	/// <typeparam name="TApplication"> The type of the used application object. </typeparam>
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

		/// <inheritdoc />
		protected override void ConfigureApplication ()
		{
			base.ConfigureApplication();

			if (!this.DebuggerAttached)
			{
				this.Application.DispatcherUnhandledException += this.DispatcherExceptionHandler;
			}

			this.Application.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
		}

		/// <summary>
		///     Called when a default application object needs to be created.
		/// </summary>
		/// <returns>
		///     The default application object.
		/// </returns>
		/// <remarks>
		///     <para>
		///         <see cref="CreateDefaultApplication" /> is called if <see cref="Bootstrapper.CreateApplication" /> returns null to use a default application object.
		///     </para>
		///     <note type="implement">
		///         The default implementation returns a new instance of <see cref="WpfApplication" />.
		///     </note>
		///     <note type="implement">
		///         This method must never return null.
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
		///         The default implementation executes the delegate with the applications dispatcher using the <see cref="DispatcherPriority.ApplicationIdle"/> priority.
		///     </note>
		/// </remarks>
		/// TODO: Return awaitable
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
		///         The default implementation executes the delegate with the applications dispatcher using the <see cref="DispatcherPriority.Normal"/> priority.
		///     </note>
		/// </remarks>
		/// TODO: Return awaitable
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
		///         The default implementation executes the delegate with the applications dispatcher using the <see cref="DispatcherPriority.SystemIdle"/> priority.
		///     </note>
		/// </remarks>
		/// TODO: Return awaitable
		protected override void DispatchStopOperations (Delegate action, params object[] args)
		{
			this.Application.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new Action<Delegate, List<object>>((x, y) => x.DynamicInvoke(y.ToArray())), action, args.ToList());
		}

		/// <inheritdoc />
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
		///     Instructs the application to shutdown.
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
