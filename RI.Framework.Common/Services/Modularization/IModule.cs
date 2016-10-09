using RI.Framework.Composition.Model;




namespace RI.Framework.Services.Modularization
{
	/// <summary>
	///     Defines the interface for a module.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A module implements a self-contained functionality or service of an application which is implemented separate of other modules and managed by an <see cref="IModuleService" />.
	///     </para>
	/// </remarks>
	[Export]
	public interface IModule
	{
		/// <summary>
		///     Gets whether the module is initialized or not.
		/// </summary>
		/// <value>
		///     true if the module is initialized, false otherwise or after the module was unloaded.
		/// </value>
		bool IsInitialized { get; }

		/// <summary>
		///     Initializes the module.
		/// </summary>
		void Initialize ();

		/// <summary>
		///     Unloads the module.
		/// </summary>
		void Unload ();
	}
}
