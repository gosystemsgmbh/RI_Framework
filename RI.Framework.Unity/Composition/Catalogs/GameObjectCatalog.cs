using System.Collections.Generic;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Utilities;

using UnityEngine;




namespace RI.Framework.Composition.Catalogs
{
	/// <summary>
	///     Implements a composition catalog which contains all currently active <c> GameObject </c>s.
	/// </summary>
	/// <remarks>
	///     <para>
	///         All active loaded <c> GameObject </c>s of the active scene are used.
	///     </para>
	///     <para>
	///         The active <c> GameObject </c>s are used for object exporting (see <see cref="CompositionContainer" /> for more details).
	///     </para>
	///     <para>
	///         See <see cref="CompositionCatalog" /> for more details about composition catalogs.
	///     </para>
	///     <note type="important">
	///         <see cref="GameObjectCatalog" /> has huge performance implications as for each composition, all game objects are searched and re-added as exports!
	///         Therefore, only use <see cref="GameObjectCatalog" /> if you do not perform periodic compositions (e.g. only during startup or scene load to bring everything in place) and also if the number of game objects during composition is reasonable to have them all as exports in a <see cref="CompositionContainer" />.
	///         Furthermore, as the game objects change continuously, <see cref="GameObjectCatalog" /> is not updated automatically (the performance hit would be atrocious!).
	///         It is only updated when resolving is performed by <see cref="CompositionContainer" /> or <see cref="Reload" /> is called.
	///     </note>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class GameObjectCatalog : CompositionCatalog
	{
		#region Instance Methods

		/// <summary>
		///     Checks the associated directory for new assemblies and loads them.
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
				this.Items.Clear();

				GameObject[] gameObjects = Object.FindObjectsOfType<GameObject>();
				foreach (GameObject gameObject in gameObjects)
				{
					HashSet<string> names = CompositionContainer.GetExportsOfType(gameObject.GetType(), true);
					string gameObjectName = gameObject.name;
					if (!gameObjectName.IsNullOrEmptyOrWhitespace())
					{
						names.Add(gameObjectName);
					}
					foreach (string name in names)
					{
						if (!this.Items.ContainsKey(name))
						{
							this.Items.Add(name, new List<CompositionCatalogItem>());
						}

						if (!this.Items[name].Any(x => object.ReferenceEquals(x.Value, gameObject)))
						{
							this.Items[name].Add(new CompositionCatalogItem(name, gameObject));
						}
					}
				}
			}
		}

		#endregion
	}
}
