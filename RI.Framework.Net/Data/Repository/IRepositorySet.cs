using System;
using System.Collections.Generic;
using System.Linq.Expressions;




namespace RI.Framework.Data.Repository
{
	/// <summary>
	///     Defines the interface for a non-generic repository set.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A repository set is used together with an <see cref="IRepositoryContext" /> and is used to expose one specific type of entities of that repository context.
	///     </para>
	/// </remarks>
	public interface IRepositorySet
	{
		/// <summary>
		///     Gets the type of entities this repository set manages.
		/// </summary>
		/// <value>
		///     The type of entities this repository set manages.
		/// </value>
		Type EntityType { get; }
		
		/// <summary>
		///     Adds a new entity to the set.
		/// </summary>
		/// <param name="entity"> The entity to add to the set. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> <paramref name="entity" /> is of an incompatible type. </exception>
		/// <exception cref="InvalidOperationException"> The entity cannot be added. </exception>
		void Add (object entity);

		/// <summary>
		///     Attaches an existing entity to the set.
		/// </summary>
		/// <param name="entity"> The entity to attach to the set. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> <paramref name="entity" /> is of an incompatible type. </exception>
		/// <exception cref="InvalidOperationException"> The entity cannot be added. </exception>
		void Attach (object entity);

		/// <summary>
		///     Creates a new entity.
		/// </summary>
		/// <returns>
		///     The newly created entity.
		/// </returns>
		/// <exception cref="InvalidOperationException"> No new entities can be created. </exception>
		object Create ();

		/// <summary>
		///     Deletes an entity from the set.
		/// </summary>
		/// <param name="entity"> The entity to delete from the set. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> <paramref name="entity" /> is of an incompatible type. </exception>
		/// <exception cref="InvalidOperationException"> The entity cannot be deleted. </exception>
		void Delete (object entity);
		
		/// <summary>
		///     Determines whether an entity has any pending changes.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity has pending changes, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> <paramref name="entity" /> is of an incompatible type. </exception>
		/// <exception cref="InvalidOperationException"> The entitys modification status cannot be determined. </exception>
		bool IsModified (object entity);

		/// <summary>
		///     Determines whether an entity has validation errors.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity has validation errors, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> <paramref name="entity" /> is of an incompatible type. </exception>
		/// <exception cref="InvalidOperationException"> The entitys validity status cannot be determined. </exception>
		bool IsValid (object entity);

		/// <summary>
		///     Explicitly marks an entity as modified or having pending changes respectively.
		/// </summary>
		/// <param name="entity"> The entity to explicitly mark as modified. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> <paramref name="entity" /> is of an incompatible type. </exception>
		/// <exception cref="InvalidOperationException"> The entity cannot be modified. </exception>
		void Modify (object entity);

		/// <summary>
		///     Reloads an entity from the database, discarding all its pending changes and validation errors.
		/// </summary>
		/// <param name="entity"> The entity to reload from the database. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> <paramref name="entity" /> is of an incompatible type. </exception>
		/// <exception cref="InvalidOperationException"> The entity cannot be reloaded. </exception>
		void Reload (object entity);

		/// <summary>
		///     Validates an entity and returns all ist validation errors.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     The validation errors or null if the entity is valid.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> <paramref name="entity" /> is of an incompatible type. </exception>
		/// <exception cref="InvalidOperationException"> The entity cannot be validated. </exception>
		RepositorySetErrors Validate (object entity);

		/// <summary>
		///     Gets a sequence of all entities of this sets type.
		/// </summary>
		/// <returns>
		///     The sequence of all entities of this sets type.
		/// </returns>
		IEnumerable<object> GetAll ();

		/// <summary>
		///     Gets the number of all entities of this sets type.
		/// </summary>
		/// <returns>
		///     The number of all entities of this sets type.
		/// </returns>
		int GetCount ();

		/// <summary>
		///     Checks whether an entity can be added to the set.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be added, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> <paramref name="entity" /> is of an incompatible type. </exception>
		bool CanAdd (object entity);

		/// <summary>
		///     Checks whether an entity can be attached to the set.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be attached, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> <paramref name="entity" /> is of an incompatible type. </exception>
		bool CanAttach (object entity);

		/// <summary>
		///     Checks whether new entities can be created by the set.
		/// </summary>
		/// <returns>
		///     true if new entities can be created, false otherwise.
		/// </returns>
		bool CanCreate ();

		/// <summary>
		///     Checks whether an entity can be deleted from the set.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be deleted, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> <paramref name="entity" /> is of an incompatible type. </exception>
		bool CanDelete (object entity);

		/// <summary>
		///     Checks whether an entity can be modified.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be modified, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> <paramref name="entity" /> is of an incompatible type. </exception>
		bool CanModify (object entity);

		/// <summary>
		///     Checks whether an entity can be reloaded into the set.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be reloaded, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> <paramref name="entity" /> is of an incompatible type. </exception>
		bool CanReload (object entity);

		/// <summary>
		///     Checks whether an entity can be validated.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be validated, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> <paramref name="entity" /> is of an incompatible type. </exception>
		bool CanValidate (object entity);

		/// <summary>
		///     Gets a sequence of filtered entities from this set.
		/// </summary>
		/// <param name="filter"> An implementation-defined filter object which is used for filtering or null if no filter is to be used. </param>
		/// <param name="sorter"> The sorter which is used to sort the entities after filtering or null if no sorting is to be performed. </param>
		/// <param name="pageIndex"> The zero-based index of the page of filtered entities to retrieve with the returned sequence. </param>
		/// <param name="pageSize"> The size of one page or 0 if no paging is to be used. </param>
		/// <param name="entityCount"> The total count of all filtered entities over all pages. </param>
		/// <param name="pageCount"> The total count of pages available with the filter applied. </param>
		/// <returns>
		///     The sequence of filtered types of this sets type.
		/// </returns>
		/// <remarks>
		///     <note type="important">
		///         The number of elements in the sequence is not necessarily <paramref name="pageSize" /> but guaranteed to be less or equal.
		///     </note>
		///     <note type="important">
		///         The first page (of index zero) can always be retrieved, even if <paramref name="entityCount" /> or <paramref name="pageCount" /> is zero (resulting in returning an empty sequence).
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentException"> <paramref name="filter" /> is an invalid filter object or <paramref name="sorter" /> is an invalid sorter. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="pageIndex" /> or <paramref name="pageSize" /> is less than zero, <paramref name="pageIndex" /> is not zero when <paramref name="pageSize" /> is zero, or <paramref name="pageIndex" /> points to a page which does not exist. </exception>
		IEnumerable<object> GetFiltered (object filter, IComparer sorter, int pageIndex, int pageSize, out int entityCount, out int pageCount);

		/// <summary>
		///     Gets a sequence of filtered entities from an existing sequence of entities.
		/// </summary>
		/// <param name="entities"> The sequence of existing entities to filter. </param>
		/// <param name="filter"> An implementation-defined filter object which is used for filtering or null if no filter is to be used. </param>
		/// <param name="sorter"> The sorter which is used to sort the entities after filtering or null if no sorting is to be performed. </param>
		/// <param name="pageIndex"> The zero-based index of the page of filtered entities to retrieve with the returned sequence. </param>
		/// <param name="pageSize"> The size of one page or 0 if no paging is to be used. </param>
		/// <param name="entityCount"> The total count of all filtered entities over all pages. </param>
		/// <param name="pageCount"> The total count of pages available with the filter applied. </param>
		/// <returns>
		///     The sequence of filtered types of this sets type.
		/// </returns>
		/// <remarks>
		///     <note type="important">
		///         The number of elements in the sequence is not necessarily <paramref name="pageSize" /> but guaranteed to be less or equal.
		///     </note>
		///     <note type="important">
		///         The first page (of index zero) can always be retrieved, even if <paramref name="entityCount" /> or <paramref name="pageCount" /> is zero (resulting in returning an empty sequence).
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentException"> <paramref name="filter" /> is an invalid filter object or <paramref name="sorter" /> is an invalid sorter. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="pageIndex" /> or <paramref name="pageSize" /> is less than zero, <paramref name="pageIndex" /> is not zero when <paramref name="pageSize" /> is zero, or <paramref name="pageIndex" /> points to a page which does not exist. </exception>		new IEnumerable<T> GetFiltered <TKey> (IEnumerable<T> entities, object filter, Expression<Func<T, TKey>> sorter, int pageIndex, int pageSize, out int entityCount, out int pageCount);
		IEnumerable<object> GetFiltered (IEnumerable entities, object filter, IComparer sorter, int pageIndex, int pageSize, out int entityCount, out int pageCount);
	}

	/// <summary>
	///     Defines the interface for a generic repository set.
	/// </summary>
	/// <typeparam name="T"> The type of the entities which are represented by this repository set. </typeparam>
	/// <remarks>
	///     <para>
	///         A repository set is used together with an <see cref="IRepositoryContext" /> and is used to expose one specific type of entities of that repository context.
	///     </para>
	/// </remarks>
	public interface IRepositorySet <T> : IRepositorySet
		where T : class
	{
		/// <summary>
		///     Adds a new entity to the set.
		/// </summary>
		/// <param name="entity"> The entity to add to the set. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The entity cannot be added. </exception>
		new void Add (T entity);

		/// <summary>
		///     Attaches an existing entity to the set.
		/// </summary>
		/// <param name="entity"> The entity to attach to the set. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The entity cannot be added. </exception>
		new void Attach (T entity);

		/// <summary>
		///     Checks whether an entity can be added to the set.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be added, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		new bool CanAdd (T entity);

		/// <summary>
		///     Checks whether an entity can be attached to the set.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be attached, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		new bool CanAttach (T entity);

		/// <summary>
		///     Checks whether an entity can be deleted from the set.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be deleted, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		new bool CanDelete (T entity);

		/// <summary>
		///     Checks whether an entity can be modified.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be modified, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		new bool CanModify (T entity);

		/// <summary>
		///     Checks whether an entity can be reloaded into the set.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be reloaded, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		new bool CanReload (T entity);

		/// <summary>
		///     Checks whether an entity can be validated.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be validated, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		new bool CanValidate (T entity);

		/// <summary>
		///     Creates a new entity.
		/// </summary>
		/// <returns>
		///     The newly created entity.
		/// </returns>
		/// <exception cref="InvalidOperationException"> No new entities can be created. </exception>
		new T Create ();

		/// <summary>
		///     Deletes an entity from the set.
		/// </summary>
		/// <param name="entity"> The entity to delete from the set. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The entity cannot be deleted. </exception>
		new void Delete (T entity);

		/// <summary>
		///     Gets a sequence of all entities of this sets type.
		/// </summary>
		/// <returns>
		///     The sequence of all entities of this sets type.
		/// </returns>
		new IEnumerable<T> GetAll ();

		/// <summary>
		///     Gets a sequence of filtered entities from this set.
		/// </summary>
		/// <param name="filter"> An implementation-defined filter object which is used for filtering or null if no filter is to be used. </param>
		/// <param name="sorter"> The sorter which is used to sort the entities after filtering or null if no sorting is to be performed. </param>
		/// <param name="pageIndex"> The zero-based index of the page of filtered entities to retrieve with the returned sequence. </param>
		/// <param name="pageSize"> The size of one page or 0 if no paging is to be used. </param>
		/// <param name="entityCount"> The total count of all filtered entities over all pages. </param>
		/// <param name="pageCount"> The total count of pages available with the filter applied. </param>
		/// <returns>
		///     The sequence of filtered types of this sets type.
		/// </returns>
		/// <remarks>
		///     <note type="important">
		///         The number of elements in the sequence is not necessarily <paramref name="pageSize" /> but guaranteed to be less or equal.
		///     </note>
		///     <note type="important">
		///         The first page (of index zero) can always be retrieved, even if <paramref name="entityCount" /> or <paramref name="pageCount" /> is zero (resulting in returning an empty sequence).
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentException"> <paramref name="filter" /> is an invalid filter object or <paramref name="sorter" /> is an invalid sorter. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="pageIndex" /> or <paramref name="pageSize" /> is less than zero, <paramref name="pageIndex" /> is not zero when <paramref name="pageSize" /> is zero, or <paramref name="pageIndex" /> points to a page which does not exist. </exception>
		new IEnumerable<T> GetFiltered (object filter, IComparer<T> sorter, int pageIndex, int pageSize, out int entityCount, out int pageCount);

		/// <summary>
		///     Gets a sequence of filtered entities from an existing sequence of entities.
		/// </summary>
		/// <param name="entities"> The sequence of existing entities to filter. </param>
		/// <param name="filter"> An implementation-defined filter object which is used for filtering or null if no filter is to be used. </param>
		/// <param name="sorter"> The sorter which is used to sort the entities after filtering or null if no sorting is to be performed. </param>
		/// <param name="pageIndex"> The zero-based index of the page of filtered entities to retrieve with the returned sequence. </param>
		/// <param name="pageSize"> The size of one page or 0 if no paging is to be used. </param>
		/// <param name="entityCount"> The total count of all filtered entities over all pages. </param>
		/// <param name="pageCount"> The total count of pages available with the filter applied. </param>
		/// <returns>
		///     The sequence of filtered types of this sets type.
		/// </returns>
		/// <remarks>
		///     <note type="important">
		///         The number of elements in the sequence is not necessarily <paramref name="pageSize" /> but guaranteed to be less or equal.
		///     </note>
		///     <note type="important">
		///         The first page (of index zero) can always be retrieved, even if <paramref name="entityCount" /> or <paramref name="pageCount" /> is zero (resulting in returning an empty sequence).
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentException"> <paramref name="filter" /> is an invalid filter object or <paramref name="sorter" /> is an invalid sorter. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="pageIndex" /> or <paramref name="pageSize" /> is less than zero, <paramref name="pageIndex" /> is not zero when <paramref name="pageSize" /> is zero, or <paramref name="pageIndex" /> points to a page which does not exist. </exception>
		new IEnumerable<T> GetFiltered (IEnumerable<T> entities, object filter, IComparer<T> sorter, int pageIndex, int pageSize, out int entityCount, out int pageCount);

		/// <summary>
		///     Determines whether an entity has any pending changes.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity has pending changes, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The entitys modification status cannot be determined. </exception>
		new bool IsModified (T entity);

		/// <summary>
		///     Determines whether an entity has validation errors.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity has validation errors, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The entitys validity status cannot be determined. </exception>
		new bool IsValid (T entity);

		/// <summary>
		///     Explicitly marks an entity as modified or having pending changes respectively.
		/// </summary>
		/// <param name="entity"> The entity to explicitly mark as modified. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The entity cannot be modified. </exception>
		new void Modify (T entity);

		/// <summary>
		///     Reloads an entity from the database, discarding all its pending changes and validation errors.
		/// </summary>
		/// <param name="entity"> The entity to reload from the database. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The entity cannot be reloaded. </exception>
		new void Reload (T entity);

		/// <summary>
		///     Validates an entity and returns all ist validation errors.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     The validation errors or null if the entity is valid.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The entity cannot be validated. </exception>
		new RepositorySetErrors Validate (T entity);
	}
}
