using RI.Framework.Composition;
using RI.Framework.Composition.Model;

namespace RI.Framework.Services
{
	/// <summary>
	/// Defines the interface which makes types bootstrapper operations aware.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="Bootstrapper"/> uses its <see cref="CompositionContainer"/> to discover <see cref="IBootstrapperOperations"/>.
	/// The methods of all found instances of <see cref="IBootstrapperOperations"/> are then called as the bootstrapping process continues.
	/// </para>
	/// </remarks>
	[Export]
	public interface IBootstrapperOperations
	{
		/// <summary>
		/// Called when all bootstrapping and initialization is done and actual application operations begin.
		/// </summary>
		void BeginOperations ();

		/// <summary>
		///     Called before the bootstrapper starts shutting down and everything is still initialized and available.
		/// </summary>
		void StopOperations ();
	}
}
