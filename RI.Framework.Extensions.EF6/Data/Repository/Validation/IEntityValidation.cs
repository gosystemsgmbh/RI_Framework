using System;
using System.Data.Entity.Validation;

using RI.Framework.Composition.Model;




namespace RI.Framework.Data.Repository.Validation
{
	/// <summary>
	///     Defines the interface for entity validation classes.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Entity validation classes are used to validate an entity in the context of an <see cref="DbRepositoryContext" />.
	///     </para>
	///     <para>
	///         Entity validation classes are created during <see cref="DbRepositoryContext.OnValidatorsCreating" />.
	///     </para>
	/// </remarks>
	[Export]
	public interface IEntityValidation
	{
		/// <summary>
		///     Gets the type of entities this entity validator validates.
		/// </summary>
		/// <value>
		///     The type of entities this entity validator validates.
		/// </value>
		Type EntityType { get; }

		/// <summary>
		///     Determines whether an entity can be added.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="set"> The set. </param>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be added, false otherwise.
		/// </returns>
		bool CanAdd (DbRepositoryContext repository, DbRepositorySet set, object entity);

		/// <summary>
		///     Determines whether an entity can be attached.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="set"> The set. </param>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be attached, false otherwise.
		/// </returns>
		bool CanAttach (DbRepositoryContext repository, DbRepositorySet set, object entity);

		/// <summary>
		///     Determines whether a new entity instance can be created.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="set"> The set. </param>
		/// <returns>
		///     true if a new entity instance can be created, false otherwise.
		/// </returns>
		bool CanCreate (DbRepositoryContext repository, DbRepositorySet set);

		/// <summary>
		///     Determines whether an entity can be deleted.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="set"> The set. </param>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be deleted, false otherwise.
		/// </returns>
		bool CanDelete (DbRepositoryContext repository, DbRepositorySet set, object entity);

		/// <summary>
		///     Determines whether an entity can be modified.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="set"> The set. </param>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be modified, false otherwise.
		/// </returns>
		bool CanModify (DbRepositoryContext repository, DbRepositorySet set, object entity);

		/// <summary>
		///     Determines whether an entity can be reloaded.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="set"> The set. </param>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be reloaded, false otherwise.
		/// </returns>
		bool CanReload (DbRepositoryContext repository, DbRepositorySet set, object entity);

		/// <summary>
		///     Determines whether an entity can be validated.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="set"> The set. </param>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be validated, false otherwise.
		/// </returns>
		bool CanValidate (DbRepositoryContext repository, DbRepositorySet set, object entity);

		/// <summary>
		///     Handles the dematerialization of an entity before it is saved to the database.
		/// </summary>
		/// <param name="repository"> The repository the entity belongs to. </param>
		/// <param name="set"> The set. </param>
		/// <param name="entity"> The entity. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="repository" />, <paramref name="set" />, or <paramref name="entity" /> is null. </exception>
		void Dematerialize (DbRepositoryContext repository, DbRepositorySet set, object entity);

		/// <summary>
		///     Fixes an entity (means: modifies values) before it is validated or saved to the database.
		/// </summary>
		/// <param name="repository"> The repository the fixed entity belongs to. </param>
		/// <param name="set"> The set. </param>
		/// <param name="entity"> The entity. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="repository" />, <paramref name="set" />, or <paramref name="entity" /> is null. </exception>
		void Fix (DbRepositoryContext repository, DbRepositorySet set, object entity);

		/// <summary>
		///     Initializes a newly created entity which was created using <see cref="IRepositorySet.Create" />.
		/// </summary>
		/// <param name="repository"> The repository the newly created entity belongs to. </param>
		/// <param name="set"> The set. </param>
		/// <param name="entity"> The entity. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="repository" />, <paramref name="set" />, or <paramref name="entity" /> is null. </exception>
		void Initialize (DbRepositoryContext repository, DbRepositorySet set, object entity);

		/// <summary>
		///     Handles the materialization of an entity after it was loaded from the database.
		/// </summary>
		/// <param name="repository"> The repository the entity belongs to. </param>
		/// <param name="set"> The set. </param>
		/// <param name="entity"> The entity. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="repository" />, <paramref name="set" />, or <paramref name="entity" /> is null. </exception>
		void Materialize (DbRepositoryContext repository, DbRepositorySet set, object entity);

		/// <summary>
		///     Validates an entity.
		/// </summary>
		/// <param name="repository"> The repository the validated entity belongs to. </param>
		/// <param name="set"> The set. </param>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     The validation results if the validation results in validation errors or null if the entity is valid.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="repository" />, <paramref name="set" />, or <paramref name="entity" /> is null. </exception>
		DbEntityValidationResult Validate (DbRepositoryContext repository, DbRepositorySet set, object entity);
	}
}
