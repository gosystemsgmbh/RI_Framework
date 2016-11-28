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
	///         This interface is currently only used to allow a non-generic abstraction of <see cref="IRepositorySet{T}" /> but does not provide actual functionality.
	///     </para>
	///     <para>
	///         See <see cref="IRepositorySet{T}" /> for more details.
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
	}

	/// <summary>
	///     Defines the interface for a repository set.
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
		void Add (T entity);

		/// <summary>
		///     Attaches an existing entity to the set.
		/// </summary>
		/// <param name="entity"> The entity to attach to the set. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The entity cannot be added. </exception>
		void Attach (T entity);

		/// <summary>
		///     Checks whether an entity can be added to the set.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be added, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		bool CanAdd (T entity);

		/// <summary>
		///     Checks whether an entity can be attached to the set.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be attached, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		bool CanAttach (T entity);

		/// <summary>
		///     Checks whether can be created by the set.
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
		bool CanDelete (T entity);

		/// <summary>
		///     Checks whether an entity can be modified.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be modified, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		bool CanModify (T entity);

		/// <summary>
		///     Checks whether an entity can be reloaded into the set.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be reloaded, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		bool CanReload (T entity);

		/// <summary>
		///     Creates a new entity.
		/// </summary>
		/// <returns>
		///     The newly created entity.
		/// </returns>
		/// <exception cref="InvalidOperationException"> No new entities can be created. </exception>
		T Create ();

		/// <summary>
		///     Deletes an entity from the set.
		/// </summary>
		/// <param name="entity"> The entity to delete from the set. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The entity cannot be deleted. </exception>
		void Delete (T entity);

		/// <summary>
		///     Gets a sequence of all entities of this sets type.
		/// </summary>
		/// <returns>
		///     The sequence of all entities of this sets type.
		/// </returns>
		IEnumerable<T> GetAll ();

		/// <summary>
		///     Gets the number of all entities of this sets type.
		/// </summary>
		/// <returns>
		///     The number of all entities of this sets type.
		/// </returns>
		int GetCount ();

		/// <summary>
		///     Gets a sequence of filtered entities from this set.
		/// </summary>
		/// <param name="filter"> An implementation-defined filter object which is used for filtering or null if no filter is to be used. </param>
		/// <param name="sorter"> The sorter expression or function which is used to sort the entities. </param>
		/// <param name="pageIndex"> The zero-based index of the page of filtered entities to retrieve with the returned sequence. </param>
		/// <param name="pageSize"> The size of one page or 0 if no paging is to be used. </param>
		/// <param name="entityCount"> The total count of all entities over all pages with the filter applied. </param>
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
		/// <exception cref="ArgumentNullException"> <paramref name="sorter" /> is null. </exception>
		/// <exception cref="ArgumentException"> <paramref name="filter" /> is an invalid filter object. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="pageIndex" /> or <paramref name="pageSize" /> is less than zero, <paramref name="pageIndex" /> is not zero when <paramref name="pageSize" /> is zero, or <paramref name="pageIndex" /> points to a page which does not exist. </exception>
		IEnumerable<T> GetFiltered <TKey> (object filter, Expression<Func<T, TKey>> sorter, int pageIndex, int pageSize, out int entityCount, out int pageCount);

		/// <summary>
		///     Gets a sequence of filtered entities from an existing sequence of entities.
		/// </summary>
		/// <param name="entities"> The sequence of existing entities to filter. </param>
		/// <param name="filter"> An implementation-defined filter object which is used for filtering or null if no filter is to be used. </param>
		/// <param name="sorter"> The sorter expression or function which is used to sort the entities. </param>
		/// <param name="pageIndex"> The zero-based index of the page of filtered entities to retrieve with the returned sequence. </param>
		/// <param name="pageSize"> The size of one page or 0 if no paging is to be used. </param>
		/// <param name="entityCount"> The total count of all entities over all pages with the filter applied. </param>
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
		/// <exception cref="ArgumentNullException"> <paramref name="entities" /> or <paramref name="sorter" /> is null. </exception>
		/// <exception cref="ArgumentException"> <paramref name="filter" /> is an invalid filter object. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="pageIndex" /> or <paramref name="pageSize" /> is less than zero, <paramref name="pageIndex" /> is not zero when <paramref name="pageSize" /> is zero, or <paramref name="pageIndex" /> points to a page which does not exist. </exception>
		IEnumerable<T> GetFiltered <TKey> (IEnumerable<T> entities, object filter, Expression<Func<T, TKey>> sorter, int pageIndex, int pageSize, out int entityCount, out int pageCount);

		/// <summary>
		///     Determines whether an entity has any pending changes.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity has pending changes, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		bool IsModified (T entity);

		/// <summary>
		///     Explicitly marks an entity as modified or having pending changes respectively.
		/// </summary>
		/// <param name="entity"> The entity to explicitly mark as modified. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The entity cannot be modified. </exception>
		void Modify (T entity);

		/// <summary>
		///     Reloads an entity from the database, discarding all its pending changes and validation errors.
		/// </summary>
		/// <param name="entity"> The entity to reload from the database. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The entity cannot be reloaded. </exception>
		void Reload (T entity);

		/// <summary>
		///     Determines whether an entity has validation errors.
		/// </summary>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     The validation errors or null if the entity is valid.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="entity" /> is null. </exception>
		RepositorySetErrors Validate (T entity);
	}
}
