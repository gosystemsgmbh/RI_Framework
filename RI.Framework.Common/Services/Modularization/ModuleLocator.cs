using System.Collections.Generic;




namespace RI.Framework.Services.Modularization
{
	/// <summary>
	///     Provides a centralized and global module provider.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="ModuleLocator" /> is merely a convenience utility as it uses <see cref="ServiceLocator" /> to retrieve and use a <see cref="IModuleService" />.
	///     </para>
	/// </remarks>
	public static class ModuleLocator
	{
		#region Static Properties/Indexer

		/// <summary>
		///     Gets whether a module service is available and can be used by <see cref="ModuleLocator" />.
		/// </summary>
		/// <value>
		///     true if a module service is available and can be used by <see cref="ModuleLocator" />, false otherwise.
		/// </value>
		public static bool IsAvailable => ModuleLocator.Service != null;

		/// <inheritdoc cref="IModuleService.IsInitialized" />
		public static bool IsInitialized => (ModuleLocator.Service?.IsInitialized).GetValueOrDefault(false);

		/// <inheritdoc cref="IModuleService.Modules" />
		public static IEnumerable<IModule> Modules => ModuleLocator.Service?.Modules ?? new IModule[0];

		/// <summary>
		///     Gets the available module service.
		/// </summary>
		/// <value>
		///     The available module service or null if no module service is available.
		/// </value>
		public static IModuleService Service => ServiceLocator.GetInstance<IModuleService>();

		#endregion




		#region Static Methods

		/// <inheritdoc cref="IModuleService.Initialize" />
		public static void Initialize () => ModuleLocator.Service?.Initialize();

		/// <inheritdoc cref="IModuleService.Unload" />
		public static void Unload () => ModuleLocator.Service?.Unload();

		#endregion
	}
}
