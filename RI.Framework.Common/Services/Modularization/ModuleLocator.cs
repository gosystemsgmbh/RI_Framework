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
		/// <summary>
		///     Gets all currently available modules.
		/// </summary>
		/// <value>
		///     All currently available modules or null if no module service is available.
		/// </value>
		public static IEnumerable<IModule> Modules
		{
			get
			{
				IModuleService moduleService = ServiceLocator.GetInstance<IModuleService>();
				return moduleService?.Modules;
			}
		}

		/// <summary>
		///     Initializes all modules.
		/// </summary>
		public static void Initialize ()
		{
			IModuleService moduleService = ServiceLocator.GetInstance<IModuleService>();
			moduleService?.Initialize();
		}

		/// <summary>
		///     Unloads all modules.
		/// </summary>
		public static void Unload ()
		{
			IModuleService moduleService = ServiceLocator.GetInstance<IModuleService>();
			moduleService?.Unload();
		}
	}
}
