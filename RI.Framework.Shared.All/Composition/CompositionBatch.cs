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

		/// <inheritdoc cref="CompositionContainer.AddExport(object, Type)" />
		public void AddExport (object instance, Type exportType)
		{
			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			if (exportType == null)
			{
				throw new ArgumentNullException(nameof(exportType));
			}

			this.AddExport(instance, CompositionContainer.GetNameOfType(exportType));
		}

		/// <inheritdoc cref="CompositionContainer.AddExport(object, string)" />
		public void AddExport (object instance, string exportName)
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

		/// <inheritdoc cref="CompositionContainer.AddExport(Type, Type, bool)" />
		public void AddExport (Type type, Type exportType, bool privateExport)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (exportType == null)
			{
				throw new ArgumentNullException(nameof(exportType));
			}

			this.AddExport(type, CompositionContainer.GetNameOfType(exportType), privateExport);
		}

		/// <inheritdoc cref="CompositionContainer.AddExport(Type, string, bool)" />
		public void AddExport (Type type, string exportName, bool privateExport)
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

		/// <inheritdoc cref="CompositionContainer.AddExport(Delegate, Type, bool)" />
		public void AddExport (Delegate factory, Type exportType, bool privateExport)
		{
			if (factory == null)
			{
				throw new ArgumentNullException(nameof(factory));
			}

			if (exportType == null)
			{
				throw new ArgumentNullException(nameof(exportType));
			}

			this.AddExport(factory, CompositionContainer.GetNameOfType(exportType), privateExport);
		}

		/// <inheritdoc cref="CompositionContainer.AddExport(Delegate, string, bool)" />
		public void AddExport (Delegate factory, string exportName, bool privateExport)
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

		/// <inheritdoc cref="CompositionContainer.AddExport(Func{CompositionContainer,object}, Type, bool)" />
		public void AddExport (Func<CompositionContainer, object> factory, Type exportType, bool privateExport) => this.AddExport((Delegate)factory, exportType, privateExport);

		/// <inheritdoc cref="CompositionContainer.AddExport(Func{CompositionContainer,object}, string, bool)" />
		public void AddExport (Func<CompositionContainer, object> factory, string exportName, bool privateExport) => this.AddExport((Delegate)factory, exportName, privateExport);

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

		/// <inheritdoc cref="CompositionContainer.RemoveExport(object, Type)" />
		public void RemoveExport (object instance, Type exportType)
		{
			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			if (exportType == null)
			{
				throw new ArgumentNullException(nameof(exportType));
			}

			this.RemoveExport(instance, CompositionContainer.GetNameOfType(exportType));
		}

		/// <inheritdoc cref="CompositionContainer.RemoveExport(object, string)" />
		public void RemoveExport (object instance, string exportName)
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

		/// <inheritdoc cref="CompositionContainer.RemoveExport(Type, Type)" />
		public void RemoveExport (Type type, Type exportType)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (exportType == null)
			{
				throw new ArgumentNullException(nameof(exportType));
			}

			this.RemoveExport(type, CompositionContainer.GetNameOfType(exportType));
		}

		/// <inheritdoc cref="CompositionContainer.RemoveExport(Type, string)" />
		public void RemoveExport (Type type, string exportName)
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

		/// <inheritdoc cref="CompositionContainer.RemoveExport(Delegate, Type)" />
		public void RemoveExport (Delegate factory, Type exportType)
		{
			if (factory == null)
			{
				throw new ArgumentNullException(nameof(factory));
			}

			if (exportType == null)
			{
				throw new ArgumentNullException(nameof(exportType));
			}

			this.RemoveExport(factory, CompositionContainer.GetNameOfType(exportType));
		}

		/// <inheritdoc cref="CompositionContainer.RemoveExport(Delegate, string)" />
		public void RemoveExport (Delegate factory, string exportName)
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

		/// <inheritdoc cref="CompositionContainer.RemoveExport(Func{CompositionContainer,object}, Type)" />
		public void RemoveExport (Func<CompositionContainer, object> factory, Type exportType) => this.RemoveExport((Delegate)factory, exportType);

		/// <inheritdoc cref="CompositionContainer.RemoveExport(Func{CompositionContainer,object}, string)" />
		public void RemoveExport (Func<CompositionContainer, object> factory, string exportName) => this.RemoveExport((Delegate)factory, exportName);

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
