using System;
using System.Collections.Generic;
using System.Reflection;




namespace RI.Framework.Composition.Catalogs
{
	/// <summary>
	///     Implements a composition catalog which contains the types of assemblies.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The types of the assemblies are used for type exporting (see <see cref="CompositionContainer" /> for more details).
	///     </para>
	///     <para>
	///         See <see cref="CompositionCatalog" /> for more details about composition catalogs.
	///     </para>
	/// </remarks>
	public class AssemblyCatalog : TypeCatalog
	{
		#region Static Methods

		private static IEnumerable<Type> GetAssemblyTypes (IEnumerable<Assembly> assemblies)
		{
			List<Type> types = new List<Type>();

			if (assemblies != null)
			{
				foreach (Assembly assembly in assemblies)
				{
					if (assembly != null)
					{
						types.AddRange(assembly.GetTypes());
					}
				}
			}

			return types;
		}

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="AssemblyCatalog" />.
		/// </summary>
		/// <param name="assemblies"> The sequence of assemblies whose types are used for composition. </param>
		/// <remarks>
		///     <para>
		///         <paramref name="assemblies" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		public AssemblyCatalog (IEnumerable<Assembly> assemblies)
			: base(AssemblyCatalog.GetAssemblyTypes(assemblies))
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="AssemblyCatalog" />.
		/// </summary>
		/// <param name="assemblies"> The array of assemblies whose types are used for composition. </param>
		public AssemblyCatalog (params Assembly[] assemblies)
			: base(AssemblyCatalog.GetAssemblyTypes(assemblies))
		{
		}

		#endregion
	}
}
