using System;
using System.Collections.Generic;
using System.Reflection;

using RI.Framework.Collections;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition.Model;
using RI.Framework.Utilities.Logging;




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

			this.LoadedAssemblies = new HashSet<Assembly>();
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
        ///         If all types are exported, the exports will consist of all public, non-abstract, non-static types, even those without an <see cref="ExportAttribute" />.
        ///     </para>
        ///     <note type="note">
        ///         Already exported types will not be affected when this property is changed.
        ///     </note>
        ///     <para>
        ///         Changing this property will not automatically reload the assemblies/types.
        ///         Use <see cref="Reload"/> to apply new settings to this property.
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

		private HashSet<Assembly> LoadedAssemblies { get; }

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
				HashSet<Assembly> allAssemblies = new HashSet<Assembly>(AppDomain.CurrentDomain.GetAssemblies(), this.LoadedAssemblies.Comparer);
				HashSet<Assembly> newAssemblies = allAssemblies.Except(this.LoadedAssemblies, this.LoadedAssemblies.Comparer);

				foreach (Assembly newAssembly in newAssemblies)
				{
					AssemblyName assemblyName = newAssembly.GetName();
					if (assemblyName.Name.StartsWith(ScriptingCatalog.ScriptingAssemblyName, StringComparison.InvariantCultureIgnoreCase))
					{
						this.Log(LogLevel.Debug, "Loading assembly: {0}", assemblyName.FullName);
						List<Type> types = newAssembly.GetTypes().Where(x => x.IsClass && x.IsPublic && (!x.IsAbstract)).ToList();
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

				this.LoadedAssemblies.AddRange(newAssemblies);
			}
		}

		#endregion
	}
}
