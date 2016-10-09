using System;
using System.Collections.Generic;

using RI.Framework.Composition.Model;




namespace RI.Framework.Services.Modularization
{
	/// <summary>
	///     Defines the interface for a modularization service.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A modularization service manages the current <see cref="IModule" /> instances of an application, including initialization and unloading.
	///     </para>
	/// </remarks>
	[Export]
	public interface IModuleService
	{
		/// <summary>
		///     Gets whether the modules are initialized or not.
		/// </summary>
		/// <value>
		///     true if the modules are initialized, false otherwise or after the modules were unloaded.
		/// </value>
		bool IsInitialized { get; }

		/// <summary>
		///     Gets all currently available modules.
		/// </summary>
		/// <value>
		///     All currently available modules.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		IEnumerable<IModule> Modules { get; }

		/// <summary>
		///     Adds a module and initializes it if <see cref="Initialize" /> was called before.
		/// </summary>
		/// <param name="module"> The module to add. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already added module should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="module" /> is null. </exception>
		void AddModule (IModule module);

		/// <summary>
		///     Initializes all modules.
		/// </summary>
		void Initialize ();

		/// <summary>
		///     Removes a module and unloads it if <see cref="Initialize" /> was called before.
		/// </summary>
		/// <param name="module"> The module to remove. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already removed module should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="module" /> is null. </exception>
		void RemoveModule (IModule module);

		/// <summary>
		///     Unloads all modules.
		/// </summary>
		void Unload ();
	}
}
