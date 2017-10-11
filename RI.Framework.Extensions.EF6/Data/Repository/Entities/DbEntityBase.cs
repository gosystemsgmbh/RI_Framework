using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Reflection;

using RI.Framework.Data.EF;
using RI.Framework.Utilities;

namespace RI.Framework.Data.Repository.Entities
{
	/// <summary>
	/// Implements a base class for entities used with <see cref="DbRepositorySet{T}"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="EntityBase"/> for more details.
	/// </para>
	/// </remarks>
	/// TODO: Extract helpers
	public abstract class DbEntityBase : EntityBase
	{
		/// <summary>
		/// Creates a DTO from this entity.
		/// </summary>
		/// <param name="repository">The associated repository context.</param>
		/// <returns>
		/// An instance of this entities type as a DTO.
		/// </returns>
		/// <remarks>
		/// <note type="note">
		/// The created DTO is not a deep copy, the values are copied shallow.
		/// </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="repository"/> is null.</exception>
		public DbEntityBase ToDto (DbRepositoryContext repository)
		{
			if (repository == null)
			{
				throw new ArgumentNullException(nameof(repository));
			}

			Type type = this.GetEntityType();
			DbEntityBase instance = (DbEntityBase)Activator.CreateInstance(type);

			this.CopyValues(this, instance, true, repository);

			return instance;
		}

		/// <summary>
		/// Creates an entity from this DTO.
		/// </summary>
		/// <param name="repository">The associated repository context.</param>
		/// <returns>
		/// An instance of this DTOs type as an entity.
		/// </returns>
		/// <remarks>
		/// <note type="note">
		/// The created entity is not a deep copy, the values are copied shallow.
		/// </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="repository"/> is null.</exception>
		public DbEntityBase ToEntity (DbRepositoryContext repository)
		{
			if (repository == null)
			{
				throw new ArgumentNullException(nameof(repository));
			}

			Type type = this.GetEntityType();
			IRepositorySet set = repository.GetSet(type);
			DbEntityBase instance = (DbEntityBase)set.Create();

			this.CopyValues(this, instance, false, repository);

			return instance;
		}

		private void CopyValues (DbEntityBase source, DbEntityBase target, bool toDto, DbRepositoryContext repository)
		{
			Type sourceType = source.GetEntityType();
			Dictionary<string, Tuple<MethodInfo, MethodInfo>> sourceProperties = DbEntityBase.GetCopyProperties(sourceType);

			Type targetType = target.GetEntityType();
			Dictionary<string, Tuple<MethodInfo, MethodInfo>> targetProperties = DbEntityBase.GetCopyProperties(targetType);

			List<string> filteredProperties = toDto ? this.FilterPropertiesToDto(sourceProperties.Keys.ToList(), repository) : this.FilterPropertiesToEntity(sourceProperties.Keys.ToList(), repository);
			foreach (string property in filteredProperties)
			{
				MethodInfo getMethod = sourceProperties[property].Item1;
				MethodInfo setMethod = targetProperties[property].Item2;

				object value = getMethod.Invoke(source, null);
				setMethod.Invoke(target, new[]
				{
					value
				});
			}
		}

		private static object GlobalSyncRoot { get; set; }

		private static Dictionary<Type, Dictionary<string, Tuple<MethodInfo, MethodInfo>>> CopyProperties { get; set; }

		static DbEntityBase ()
		{
			DbEntityBase.GlobalSyncRoot = new object();
			DbEntityBase.CopyProperties = new Dictionary<Type, Dictionary<string, Tuple<MethodInfo, MethodInfo>>>();
		}

		private static Dictionary<string, Tuple<MethodInfo, MethodInfo>> GetCopyProperties (Type type)
		{
			lock (DbEntityBase.GlobalSyncRoot)
			{
				if (!DbEntityBase.CopyProperties.ContainsKey(type))
				{
					Dictionary<string, Tuple<MethodInfo, MethodInfo>> dictionary = new Dictionary<string, Tuple<MethodInfo, MethodInfo>>(StringComparerEx.OrdinalIgnoreCase);
					PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					foreach (PropertyInfo property in properties)
					{
						MethodInfo getMethod = property.GetGetMethod(true);
						MethodInfo setMethod = property.GetSetMethod(true);
						dictionary.Add(property.Name, new Tuple<MethodInfo, MethodInfo>(getMethod, setMethod));
					}
					DbEntityBase.CopyProperties.Add(type, dictionary);
				}
				return DbEntityBase.CopyProperties[type];
			}
		}

		/// <summary>
		/// Filters the properties which should not be copied from an entity to a DTO.
		/// </summary>
		/// <param name="properties">The list of properties to copy.</param>
		/// <param name="repository">The used repository.</param>
		/// <returns>
		/// The list of filtered properties.
		/// This must never be null, an empty list is to be returned if no properties shall be copied.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The default implementation filters out all navigation properties, using <see cref="DbContextExtensions.GetNavigationPropertiesForType"/>.
		/// </para>
		/// </remarks>
		protected virtual List<string> FilterPropertiesToDto (List<string> properties, DbRepositoryContext repository)
		{
			this.FilterNavigationProperties(properties, repository);
			return properties;
		}

		/// <summary>
		/// Filters the properties which should not be copied from a DTO to an entity.
		/// </summary>
		/// <param name="properties">The list of properties to copy.</param>
		/// <param name="repository">The used repository.</param>
		/// <returns>
		/// The list of filtered properties.
		/// This must never be null, an empty list is to be returned if no properties shall be copied.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The default implementation filters out all navigation properties, using <see cref="DbContextExtensions.GetNavigationPropertiesForType"/>.
		/// </para>
		/// </remarks>
		protected virtual List<string> FilterPropertiesToEntity (List<string> properties, DbRepositoryContext repository)
		{
			this.FilterNavigationProperties(properties, repository);
			return properties;
		}

		private void FilterNavigationProperties (List<string> properties, DbRepositoryContext repository)
		{
			Type entityType = this.GetEntityType();
			List<string> navigationProperties = repository.GetNavigationPropertiesForType(entityType);
			properties.RemoveAll(x => navigationProperties.Contains(x, StringComparerEx.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Gets the entity type of this entity.
		/// </summary>
		/// <returns>
		/// The entity type of this entity.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The entity type is the actual POCO type you declared.
		/// If it is an EF proxy type, its underlying POCO type is returned.
		/// </para>
		/// </remarks>
		public Type GetEntityType () => ObjectContext.GetObjectType(this.GetType());

		/// <summary>
		/// Checks whether this entity is a dynamic proxy.
		/// </summary>
		/// <returns>
		/// true if the entity is a dynamic proxy, false otherwise.
		/// </returns>
		public bool IsProxyType () => this.GetType() != this.GetEntityType();
	}
}
