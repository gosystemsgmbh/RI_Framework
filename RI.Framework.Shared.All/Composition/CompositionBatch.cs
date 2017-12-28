using System;
using System.Collections.Generic;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Composition
{
	/// <summary>
	///     Provides batching functionality for managing exports and performing composition by collecting multiple composition actions, executed in one run by a <see cref="CompositionContainer" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This class provides methods for managing exports and performing composition which are similar to the methods of <see cref="CompositionContainer" /> and also have the same effect once executed by <see cref="CompositionContainer.Compose(CompositionBatch)" />.
	///     </para>
	///     <para>
	///         See <see cref="CompositionContainer" /> for details about managing exports and performing composition.
	///     </para>
	/// </remarks>
	public sealed class CompositionBatch
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="CompositionBatch" />.
		/// </summary>
		public CompositionBatch ()
		{
			this.ItemsToAdd = new List<CompositionCatalogItem>();
			this.ItemsToRemove = new List<CompositionCatalogItem>();
			this.CreatorsToAdd = new List<CompositionCreator>();
			this.CreatorsToRemove = new List<CompositionCreator>();
			this.CatalogsToAdd = new List<CompositionCatalog>();
			this.CatalogsToRemove = new List<CompositionCatalog>();
			this.ObjectsToSatisfy = new Dictionary<object, CompositionFlags>();
			this.Composition = CompositionFlags.None;
		}

		#endregion




		#region Instance Properties/Indexer

		internal List<CompositionCatalog> CatalogsToAdd { get; }

		internal List<CompositionCatalog> CatalogsToRemove { get; }

		internal CompositionFlags Composition { get; private set; }

		internal List<CompositionCreator> CreatorsToAdd { get; }

		internal List<CompositionCreator> CreatorsToRemove { get; }

		internal List<CompositionCatalogItem> ItemsToAdd { get; }

		internal List<CompositionCatalogItem> ItemsToRemove { get; }

		internal Dictionary<object, CompositionFlags> ObjectsToSatisfy { get; }

		#endregion




		#region Instance Methods

		/// <inheritdoc cref="CompositionContainer.AddCatalog" />
		public void AddCatalog (CompositionCatalog catalog)
		{
			if (catalog == null)
			{
				throw new ArgumentNullException(nameof(catalog));
			}

			this.CatalogsToAdd.Add(catalog);
			this.CatalogsToRemove.Remove(catalog);
		}

		/// <inheritdoc cref="CompositionContainer.AddCreator" />
		public void AddCreator (CompositionCreator creator)
		{
			if (creator == null)
			{
				throw new ArgumentNullException(nameof(creator));
			}

			this.CreatorsToAdd.Add(creator);
			this.CreatorsToRemove.Remove(creator);
		}

		/// <inheritdoc cref="CompositionContainer.AddFactory(Delegate, Type, bool)" />
		public void AddFactory (Delegate factory, Type exportType, bool privateExport)
		{
			if (factory == null)
			{
				throw new ArgumentNullException(nameof(factory));
			}

			if (exportType == null)
			{
				throw new ArgumentNullException(nameof(exportType));
			}

			this.AddFactory(factory, CompositionContainer.GetNameOfType(exportType), privateExport);
		}

		/// <inheritdoc cref="CompositionContainer.AddFactory(Delegate, string, bool)" />
		public void AddFactory (Delegate factory, string exportName, bool privateExport)
		{
			if (factory == null)
			{
				throw new ArgumentNullException(nameof(factory));
			}

			if (exportName == null)
			{
				throw new ArgumentNullException(nameof(exportName));
			}

			if (exportName.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(exportName));
			}

			this.ItemsToAdd.Add(new CompositionCatalogItem(exportName, factory, privateExport));
		}

		/// <inheritdoc cref="CompositionContainer.AddFactory(Func{CompositionContainer,object}, Type, bool)" />
		public void AddFactory (Func<CompositionContainer, object> factory, Type exportType, bool privateExport) => this.AddFactory((Delegate)factory, exportType, privateExport);

		/// <inheritdoc cref="CompositionContainer.AddFactory(Func{CompositionContainer,object}, string, bool)" />
		public void AddFactory (Func<CompositionContainer, object> factory, string exportName, bool privateExport) => this.AddFactory((Delegate)factory, exportName, privateExport);

		/// <inheritdoc cref="CompositionContainer.AddInstance(object, Type)" />
		public void AddInstance (object instance, Type exportType)
		{
			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			if (exportType == null)
			{
				throw new ArgumentNullException(nameof(exportType));
			}

			this.AddInstance(instance, CompositionContainer.GetNameOfType(exportType));
		}

		/// <inheritdoc cref="CompositionContainer.AddInstance(object, string)" />
		public void AddInstance (object instance, string exportName)
		{
			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			if (!CompositionContainer.ValidateExportInstance(instance))
			{
				throw new InvalidTypeArgumentException(nameof(instance));
			}

			if (exportName == null)
			{
				throw new ArgumentNullException(nameof(exportName));
			}

			if (exportName.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(exportName));
			}

			this.ItemsToAdd.Add(new CompositionCatalogItem(exportName, instance));
		}

		/// <inheritdoc cref="CompositionContainer.AddType(Type, Type, bool)" />
		public void AddType (Type type, Type exportType, bool privateExport)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (exportType == null)
			{
				throw new ArgumentNullException(nameof(exportType));
			}

			this.AddType(type, CompositionContainer.GetNameOfType(exportType), privateExport);
		}

		/// <inheritdoc cref="CompositionContainer.AddType(Type, string, bool)" />
		public void AddType (Type type, string exportName, bool privateExport)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (!CompositionContainer.ValidateExportType(type))
			{
				throw new InvalidTypeArgumentException(nameof(type));
			}

			if (exportName == null)
			{
				throw new ArgumentNullException(nameof(exportName));
			}

			if (exportName.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(exportName));
			}

			this.ItemsToAdd.Add(new CompositionCatalogItem(exportName, type, privateExport));
		}

		/// <summary>
		///     Empties this composition batch so that it does not contain any composition actions.
		/// </summary>
		public void Clear ()
		{
			this.ItemsToAdd.Clear();
			this.ItemsToRemove.Clear();
			this.CatalogsToAdd.Clear();
			this.CatalogsToRemove.Clear();
			this.ObjectsToSatisfy.Clear();
			this.Composition = CompositionFlags.None;
		}

		/// <inheritdoc cref="CompositionContainer.Recompose" />
		public void Recompose (CompositionFlags composition)
		{
			this.Composition = this.Composition | composition;
		}

		/// <inheritdoc cref="CompositionContainer.RemoveCatalog" />
		public void RemoveCatalog (CompositionCatalog catalog)
		{
			if (catalog == null)
			{
				throw new ArgumentNullException(nameof(catalog));
			}

			this.CatalogsToRemove.Add(catalog);
			this.CatalogsToAdd.Remove(catalog);
		}

		/// <inheritdoc cref="CompositionContainer.RemoveCreator" />
		public void RemoveCreator (CompositionCreator creator)
		{
			if (creator == null)
			{
				throw new ArgumentNullException(nameof(creator));
			}

			this.CreatorsToRemove.Add(creator);
			this.CreatorsToAdd.Remove(creator);
		}

		/// <inheritdoc cref="CompositionContainer.RemoveFactory(Delegate, Type)" />
		public void RemoveFactory (Delegate factory, Type exportType)
		{
			if (factory == null)
			{
				throw new ArgumentNullException(nameof(factory));
			}

			if (exportType == null)
			{
				throw new ArgumentNullException(nameof(exportType));
			}

			this.RemoveFactory(factory, CompositionContainer.GetNameOfType(exportType));
		}

		/// <inheritdoc cref="CompositionContainer.RemoveFactory(Delegate, string)" />
		public void RemoveFactory (Delegate factory, string exportName)
		{
			if (factory == null)
			{
				throw new ArgumentNullException(nameof(factory));
			}

			if (exportName == null)
			{
				throw new ArgumentNullException(nameof(exportName));
			}

			if (exportName.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(exportName));
			}

			this.ItemsToRemove.Add(new CompositionCatalogItem(exportName, factory, false));
		}

		/// <inheritdoc cref="CompositionContainer.RemoveFactory(Func{CompositionContainer,object}, Type)" />
		public void RemoveFactory (Func<CompositionContainer, object> factory, Type exportType) => this.RemoveFactory((Delegate)factory, exportType);

		/// <inheritdoc cref="CompositionContainer.RemoveFactory(Func{CompositionContainer,object}, string)" />
		public void RemoveFactory (Func<CompositionContainer, object> factory, string exportName) => this.RemoveFactory((Delegate)factory, exportName);

		/// <inheritdoc cref="CompositionContainer.RemoveInstance(object, Type)" />
		public void RemoveInstance (object instance, Type exportType)
		{
			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			if (exportType == null)
			{
				throw new ArgumentNullException(nameof(exportType));
			}

			this.RemoveInstance(instance, CompositionContainer.GetNameOfType(exportType));
		}

		/// <inheritdoc cref="CompositionContainer.RemoveInstance(object, string)" />
		public void RemoveInstance (object instance, string exportName)
		{
			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			if (exportName == null)
			{
				throw new ArgumentNullException(nameof(exportName));
			}

			if (exportName.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(exportName));
			}

			this.ItemsToRemove.Add(new CompositionCatalogItem(exportName, instance));
		}

		/// <inheritdoc cref="CompositionContainer.RemoveType(Type, Type)" />
		public void RemoveType (Type type, Type exportType)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (exportType == null)
			{
				throw new ArgumentNullException(nameof(exportType));
			}

			this.RemoveType(type, CompositionContainer.GetNameOfType(exportType));
		}

		/// <inheritdoc cref="CompositionContainer.RemoveType(Type, string)" />
		public void RemoveType (Type type, string exportName)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (exportName == null)
			{
				throw new ArgumentNullException(nameof(exportName));
			}

			if (exportName.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(exportName));
			}

			this.ItemsToRemove.Add(new CompositionCatalogItem(exportName, type, false));
		}

		/// <inheritdoc cref="CompositionContainer.ResolveImports" />
		public void ResolveImports (object obj, CompositionFlags composition)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			if (!CompositionContainer.ValidateExportInstance(obj))
			{
				throw new InvalidTypeArgumentException(nameof(obj));
			}

			if (!this.ObjectsToSatisfy.ContainsKey(obj))
			{
				this.ObjectsToSatisfy.Add(obj, composition);
			}
			else
			{
				this.ObjectsToSatisfy[obj] = this.ObjectsToSatisfy[obj] | composition;
			}
		}

		#endregion
	}
}
