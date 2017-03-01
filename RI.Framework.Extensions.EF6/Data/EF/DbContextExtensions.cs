using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;




namespace RI.Framework.Data.EF
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="DbContext" /> type.
	/// </summary>
	public static class DbContextExtensions
	{
		#region Static Methods

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

			ReadOnlyCollection<EntityContainerMapping> storageMetadata = ((IObjectContextAdapter)dbContext).ObjectContext.MetadataWorkspace.GetItems<EntityContainerMapping>(DataSpace.CSSpace);
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
