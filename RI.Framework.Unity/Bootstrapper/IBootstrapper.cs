using System;

using RI.Framework.Composition;
using RI.Framework.Composition.Model;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bootstrapper
{
	/// <summary>
	///     Defines the interface for a bootstrapper.
	/// </summary>
	/// <remarks>
	///     <para>
	///         An application and service bootstrapper sets up the composition, initializes the services, and then runs the application.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public interface IBootstrapper : ISynchronizable
	{
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
		///     Gets the used composition container.
		/// </summary>
		/// <value>
		///     The used composition container.
		/// </value>
		CompositionContainer Container { get; }

		/// <summary>
		///     Gets the current state of the bootstrapper.
		/// </summary>
		/// <value>
		///     The current state of the bootstrapper.
		/// </value>
		BootstrapperState State { get; }

		#endregion
	}
}
