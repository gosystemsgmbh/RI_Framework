using System;
using System.Collections.Generic;
using System.Reflection;

using RI.Framework.Collections;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition.Model;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.Runtime;

namespace RI.Framework.Composition.Catalogs
{
	/// <summary>
	///     Implements a composition catalog which gets exports from all assemblies and types of the current application domain.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="CompositionCatalog" /> for more details about composition catalogs.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class AppDomainCatalog : CompositionCatalog
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="AppDomainCatalog" />.
		/// </summary>
		/// <remarks>
		///     <para>
		///         true is used for <see cref="ExportAllTypes" /> and <see cref="AutoUpdate" />, false is used for <see cref="IgnoreFrameworkTypes"/>.
		///     </para>
		/// </remarks>
		public AppDomainCatalog ()
			: this(true, true, false)
		{
		}

		/// <summary>
		/// </summary>
		/// <param name="exportAllTypes"> Specifies whether all types should be exported (see <see cref="ExportAllTypes" /> for details). </param>
		/// <param name="autoUpdate"> Specifies whether the exports are automatically updated when a new assembly is loaded (see <see cref="AutoUpdate" /> for details). </param>
		/// <param name="ignoreFrameworkTypes"> Specifies whether framework-provided types should be exported (see <see cref="IgnoreFrameworkTypes" /> for details). </param>
		public AppDomainCatalog (bool exportAllTypes, bool autoUpdate, bool ignoreFrameworkTypes)
		{
			this.ExportAllTypes = exportAllTypes;
			this.AutoUpdate = autoUpdate;
			this.IgnoreFrameworkTypes = ignoreFrameworkTypes;

			this.LoadedAssemblies = new HashSet<Assembly>();
			this.AssemblyLoadEventHandler = this.AssemblyLoadEvent;

			AppDomain.CurrentDomain.AssemblyLoad += this.AssemblyLoadEventHandler;
		}

		#endregion




		#region Instance Fields

		private bool _autoUpdate;
		private bool _exportAllTypes;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets whether the exports are automatically updated when a new assembly is loaded.
		/// </summary>
		/// <value>
		///     true if exports are automatically updated when a new assembly is loaded, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         If exports are automatically updated, this catalog issues a recomposition every time an assembly is loaded into the application domain.
		///     </para>
		/// </remarks>
		public bool AutoUpdate
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._autoUpdate;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._autoUpdate = value;
				}
			}
		}

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

		private bool _ignoreFrameworkTypes;

		/// <summary>
		/// Gets whether types provided by the framework itself are ignored.
		/// </summary>
		/// <value>
		/// true if types provided by the framework itself are ignored, false otherwise.
		/// </value>
		/// <remarks>
		/// <para>
		/// If framework-provided types are ignored, they are not exported by default.
		/// However, you can still export those types by explicit export them, e.g. through <see cref="TypeCatalog"/> or <see cref="InstanceCatalog"/>.
		/// </para>
		/// </remarks>
		public bool IgnoreFrameworkTypes
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._ignoreFrameworkTypes;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._ignoreFrameworkTypes = value;
				}
			}
		}

		private AssemblyLoadEventHandler AssemblyLoadEventHandler { get; }

		private HashSet<Assembly> LoadedAssemblies { get; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Checks the associated directory for new assemblies and loads them.
		/// </summary>
		public void Reload ()
		{
			this.RequestRecompose();
		}

		private void AssemblyLoadEvent (object sender, AssemblyLoadEventArgs args)
		{
			if (this.AutoUpdate)
			{
				this.Reload();
			}
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected override void Dispose (bool disposing)
		{
			lock (this.SyncRoot)
			{
				AppDomain.CurrentDomain.AssemblyLoad -= this.AssemblyLoadEventHandler;
			}

			base.Dispose(disposing);
		}

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
					this.Log(LogLevel.Debug, "Loading assembly: {0}", newAssembly.FullName);
					List<Type> types = newAssembly.GetTypes().Where(x => x.IsClass && x.IsPublic && (!x.IsAbstract)).ToList();
					foreach (Type type in types)
					{
						if (CompositionContainer.ValidateExportType(type))
						{
							bool isFrameworkType = FrameworkTypeUtility.IsFrameworkType(type);
							if (isFrameworkType && this.IgnoreFrameworkTypes)
							{
								this.Log(LogLevel.Debug, "Framework-provided type filtered: {0}", type.FullName);
								continue;
							}

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

				this.LoadedAssemblies.AddRange(newAssemblies);
			}
		}

		#endregion
	}
}
