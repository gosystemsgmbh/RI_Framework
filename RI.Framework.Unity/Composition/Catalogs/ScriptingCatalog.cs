using System;
using System.Collections.Generic;
using System.Reflection;

using RI.Framework.Collections.Linq;
using RI.Framework.Services.Logging;




namespace RI.Framework.Composition.Catalogs
{
	/// <summary>
	///     Implements a composition catalog which contains all types of a Unity projects scripting assemblies.
	/// </summary>
	/// <remarks>
	///     <para>
	///         All assemblies of the Unity project which match the pattern <c> Assembly-CSharp*.dll </c> are searched for composable types.
	///     </para>
	///     <para>
	///         The types of the scripting assemblies are used for type exporting (see <see cref="CompositionContainer" /> for more details).
	///     </para>
	///     <para>
	///         See <see cref="CompositionCatalog" /> for more details about composition catalogs.
	///     </para>
	/// </remarks>
	public sealed class ScriptingCatalog : CompositionCatalog
	{
		#region Constants

		private const string ScriptingAssemblyName = "Assembly-CSharp";

		#endregion




		#region Instance Methods

		/// <summary>
		///     Ensures that all the types in the Unity projects scripting assemblies are loaded.
		/// </summary>
		public void Reload ()
		{
			this.RequestRecompose();
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected internal override void UpdateItems ()
		{
			base.UpdateItems();

			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				AssemblyName assemblyName = assembly.GetName();
				if (assemblyName.Name.StartsWith(ScriptingCatalog.ScriptingAssemblyName, StringComparison.InvariantCultureIgnoreCase))
				{
					this.Log(LogLevel.Debug, "Loading assembly: {0}", assemblyName.FullName);
					Type[] types = assembly.GetTypes();
					foreach (Type type in types)
					{
						if (CompositionContainer.ValidateExportType(type))
						{
							HashSet<string> names = CompositionContainer.GetExportsOfType(type);
							foreach (string name in names)
							{
								if (!this.Items.ContainsKey(name))
								{
									this.Items.Add(name, new List<CompositionCatalogItem>());
								}

								if (!this.Items[name].Any(x => x.Type == type))
								{
									this.Items[name].Add(new CompositionCatalogItem(name, type));
								}
							}
						}
					}
				}
			}
		}

		#endregion
	}
}
