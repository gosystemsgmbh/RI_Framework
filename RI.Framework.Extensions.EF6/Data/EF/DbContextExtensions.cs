using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;




namespace RI.Framework.Data.EF
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="DbContext" /> type.
	/// </summary>
	public static class DbContextExtensions
	{
		#region Static Methods

		/// <summary>
		///     Gets all navigation properties of a mapped type.
		/// </summary>
		/// <param name="dbContext"> The <see cref="DbContext" />. </param>
		/// <param name="type"> The type. </param>
		/// <returns>
		///     The list of navigation property names or null if the type is not mapped.
		///     If the type is mapped but has no navigation properties, an empty list is returned.
		/// </returns>
		/// <remarks>
		///     <para>
		///         Only the names of the actual navigation properties are returned, foreign key properties are not included.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> or <paramref name="dbContext" /> is null. </exception>
		public static List<string> GetNavigationPropertiesForType (this DbContext dbContext, Type type)
		{
			if (dbContext == null)
			{
				throw new ArgumentNullException(nameof(dbContext));
			}

			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			MetadataWorkspace workspace = ((IObjectContextAdapter)dbContext).ObjectContext.MetadataWorkspace;
			ObjectItemCollection itemCollection = (ObjectItemCollection)(workspace.GetItemCollection(DataSpace.OSpace));
			EntityType entityType = itemCollection.OfType<EntityType>().FirstOrDefault(e => itemCollection.GetClrType(e) == type);

			if (entityType == null)
			{
				return null;
			}

			List<string> navigationProperties = (from x in entityType.NavigationProperties select x.Name).ToList();

			return navigationProperties;
		}

		/// <summary>
		///     Gets the name of the table a type is mapped to.
		/// </summary>
		/// <param name="dbContext"> The <see cref="DbContext" />. </param>
		/// <param name="type"> The type. </param>
		/// <returns>
		///     The name of the table or null if the type is not mapped.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> or <paramref name="dbContext" /> is null. </exception>
		public static string GetTableNameForType (this DbContext dbContext, Type type)
		{
			if (dbContext == null)
			{
				throw new ArgumentNullException(nameof(dbContext));
			}

			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			MetadataWorkspace workspace = ((IObjectContextAdapter)dbContext).ObjectContext.MetadataWorkspace;
			ReadOnlyCollection<EntityContainerMapping> storageMetadata = workspace.GetItems<EntityContainerMapping>(DataSpace.CSSpace);
			foreach (EntityContainerMapping mapping in storageMetadata)
			{
				EntitySet entitySet;
				if (mapping.StoreEntityContainer.TryGetEntitySetByName(type.Name, false, out entitySet))
				{
					return entitySet.Table;
				}
			}

			return null;
		}

		#endregion
	}
}
