using System;
using System.Reflection;

using RI.Framework.Composition;
using RI.Framework.Composition.Model;
using RI.Framework.IO.Paths;
using RI.Framework.Utilities.Text;




namespace RI.Framework.Services
{
	/// <summary>
	///     Defines the interface for a bootstrapper.
	/// </summary>
	/// <remarks>
	///     <para>
	///         An application and service bootstrapper sets up the composition, initializes the services, and then runs the application.
	///     </para>
	/// </remarks>
	[Export]
	public interface IBootstrapper
	{
		/// <summary>
		///     Gets the main assembly of the application.
		/// </summary>
		/// <value>
		///     The main assembly of the application.
		/// </value>
		Assembly ApplicationAssembly { get; }

		/// <summary>
		///     Gets the associated company name of the application.
		/// </summary>
		/// <value>
		///     The associated company name of the application.
		/// </value>
		string ApplicationCompanyName { get; }

		/// <summary>
		///     Gets the copyright statement of the application.
		/// </summary>
		/// <value>
		///     The copyright statement of the application.
		/// </value>
		string ApplicationCopyright { get; }

		/// <summary>
		///     Gets the read- and writeable directory associated with the application used to store persistent data.
		/// </summary>
		/// <value>
		///     The read- and writeable directory associated with the application used to store persistent data.
		/// </value>
		DirectoryPath ApplicationDataDirectory { get; }

		/// <summary>
		///     Gets the read-only directory where the applications executable files are stored.
		/// </summary>
		/// <value>
		///     The read-only directory where the applications executable files are stored.
		/// </value>
		DirectoryPath ApplicationExecutableDirectory { get; }

		/// <summary>
		///     Gets the GUID of the application which is application version dependent.
		/// </summary>
		/// <value>
		///     The GUID of the application which is application version dependent.
		/// </value>
		Guid ApplicationIdVersionDependent { get; }

		/// <summary>
		///     Gets the GUID of the application which is application version independent.
		/// </summary>
		/// <value>
		///     The GUID of the application which is application version independent.
		/// </value>
		Guid ApplicationIdVersionIndependent { get; }

		/// <summary>
		///     Gets the product name of the application.
		/// </summary>
		/// <value>
		///     The product name of the application.
		/// </value>
		string ApplicationProductName { get; }

		/// <summary>
		///     Gets the version of the application.
		/// </summary>
		/// <value>
		///     The version of the application.
		/// </value>
		Version ApplicationVersion { get; }

		/// <summary>
		///     Gets the used composition container.
		/// </summary>
		/// <value>
		///     The used composition container.
		/// </value>
		CompositionContainer Container { get; }

		/// <summary>
		///     Gets whether a debugger is attached to the current process or not.
		/// </summary>
		/// <value>
		///     true if a debugger is attached to the current process, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         The value of <see cref="DebuggerAttached" /> reflects the situation during bootstrapping.
		///     </para>
		/// </remarks>
		bool DebuggerAttached { get; }

		/// <summary>
		///     Gets the anonymized GUID of the domain this machine belongs to.
		/// </summary>
		/// <value>
		///     The anonymized GUID of the domain this machine belongs to.
		/// </value>
		Guid DomainId { get; }

		/// <summary>
		///     Gets whether the program is executed on a 64 bit machine.
		/// </summary>
		/// <value>
		///     true if executed on a 64 bit machine, false otherwise.
		/// </value>
		bool Machine64Bit { get; }

		/// <summary>
		///     Gets the anonymized GUID of the local machine.
		/// </summary>
		/// <value>
		///     The anonymized GUID of the local machine.
		/// </value>
		Guid MachineId { get; }

		/// <summary>
		///     Gets the command line which was used for the current process.
		/// </summary>
		/// <value>
		///     The command line which was used for the current process.
		/// </value>
		CommandLine ProcessCommandLine { get; }

		/// <summary>
		///     Gets whether the program is executed in a 64 bit process.
		/// </summary>
		/// <value>
		///     true if executed in a 64 bit process, false otherwise.
		/// </value>
		bool Session64Bit { get; }

		/// <summary>
		///     Gets the GUID of the current session.
		/// </summary>
		/// <value>
		///     The GUID of the current session.
		/// </value>
		Guid SessionId { get; }

		/// <summary>
		///     Gets the timestamp of the current session.
		/// </summary>
		/// <value>
		///     The timestamp of the current session.
		/// </value>
		DateTime SessionTimestamp { get; }

		/// <summary>
		///     Gets the current state of the bootstrapper.
		/// </summary>
		/// <value>
		///     The current state of the bootstrapper.
		/// </value>
		BootstrapperState State { get; }

		/// <summary>
		///     Gets the anonymized GUID of the current user.
		/// </summary>
		/// <value>
		///     The anonymized GUID of the current user.
		/// </value>
		Guid UserId { get; }




		#region Abstracts

		/// <summary>
		///     Starts the bootstrapping and runs the application.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         <see cref="Run" /> must only be called once.
		///     </note>
		/// </remarks>
		/// <exception cref="InvalidOperationException"> <see cref="Run" /> was called repeatedly. </exception>
		void Run ();

		/// <summary>
		///     Initiates the shutdown of the application.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         It must be possible to call <see cref="Shutdown" /> multiple times.
		///     </note>
		/// </remarks>
		/// <exception cref="InvalidOperationException"> <see cref="Run" /> was not called before. </exception>
		void Shutdown ();

		/// <summary>
		///     Hides the splash screen.
		/// </summary>
		/// <remarks>
		///     <note type="implement">
		///         It must be possible to call <see cref="HideSplashScreen" /> multiple times.
		///     </note>
		/// </remarks>
		void HideSplashScreen ();

		/// <summary>
		///     Starts the handling of an application-level exception.
		/// </summary>
		/// <param name="exception"> The exception to be handled. </param>
		/// <remarks>
		///     <note type="note">
		///         This method is called to start the exception handling, not to do custom exception handling.
		///     </note>
		///     <note type="note">
		///         <see cref="StartExceptionHandling" /> does never return and terminates the current process.
		///     </note>
		/// </remarks>
		void StartExceptionHandling (Exception exception);

		/// <summary>
		///     Starts the first chance handling of an application-level exception.
		/// </summary>
		/// <param name="exception"> The exception to be handled. </param>
		/// <remarks>
		///     <note type="note">
		///         This method is called to start the exception handling, not to do custom exception handling.
		///     </note>
		///     <note type="note">
		///         <see cref="StartFirstChanceExceptionHandling" /> always returns and does not terminate the current process.
		///     </note>
		/// </remarks>
		void StartFirstChanceExceptionHandling (Exception exception);

		#endregion
	}
}
