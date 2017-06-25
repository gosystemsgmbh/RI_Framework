using System;
using System.Collections.Generic;
using System.Reflection;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition.Model;
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
	/// <threadsafety static="true" instance="true" />
	/// TODO: ExportAllTypes setting in bootstrapper
	/// TODO: Prevent from loading the same assembly twice
	public sealed class ScriptingCatalog : CompositionCatalog
	{
		#region Constants

		private const string ScriptingAssemblyName = "Assembly-CSharp";

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ScriptingCatalog" />.
		/// </summary>
		/// <remarks>
		///     <para>
		///         true is used for <see cref="ExportAllTypes" />.
		///     </para>
		/// </remarks>
		public ScriptingCatalog ()
			: this(true)
		{
		}

		/// <summary>
		/// </summary>
		/// <param name="exportAllTypes"> Specifies whether all types should be exported (see <see cref="ExportAllTypes" /> for details). </param>
		public ScriptingCatalog (bool exportAllTypes)
		{
			this.ExportAllTypes = exportAllTypes;
		}

		#endregion




		#region Instance Fields

		private bool _exportAllTypes;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets whether all types should be exported.
		/// </summary>
		/// <value>
		///     true if all types should be exported, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         If all types are exported, the exports will consist of all non-abstract, non-static types, even those without an <see cref="ExportAttribute" />.
		///     </para>
		/// </remarks>
		public bool ExportAllTypes
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._exportAllTypes;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._exportAllTypes = value;
				}
			}
		}

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

			lock (this.SyncRoot)
			{
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
								bool privateExport = CompositionContainer.IsExportPrivate(type).GetValueOrDefault(false);
								HashSet<string> names = CompositionContainer.GetExportsOfType(type, this.ExportAllTypes);
								foreach (string name in names)
								{
									if (!this.Items.ContainsKey(name))
									{
										this.Items.Add(name, new List<CompositionCatalogItem>());
									}

									if (!this.Items[name].Any(x => x.Type == type))
									{
										this.Items[name].Add(new CompositionCatalogItem(name, type, privateExport));
									}
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
