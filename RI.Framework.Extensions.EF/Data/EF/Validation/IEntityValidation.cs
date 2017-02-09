using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

using RI.Framework.Composition.Model;
using RI.Framework.Data.Repository;




namespace RI.Framework.Data.EF.Validation
{
	/// <summary>
	///     Defines the interface for entity validation classes.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Entity validation classes are used to validate an entity in the context of an <see cref="RepositoryDbContext" />.
	///     </para>
	///     <para>
	///         Entity validation classes are created during <see cref="RepositoryDbContext.OnValidatorsCreating" />.
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
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be added, false otherwise.
		/// </returns>
		bool CanAdd (RepositoryDbContext repository, object entity);

		/// <summary>
		///     Determines whether an entity can be attached.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be attached, false otherwise.
		/// </returns>
		bool CanAttach (RepositoryDbContext repository, object entity);

		/// <summary>
		///     Determines whether a new entity instance can be created.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <returns>
		///     true if a new entity instance can be created, false otherwise.
		/// </returns>
		bool CanCreate (RepositoryDbContext repository);

		/// <summary>
		///     Determines whether an entity can be deleted.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="entry"> The entity entry of the entity. </param>
		/// <returns>
		///     true if the entity can be deleted, false otherwise.
		/// </returns>
		bool CanDelete (RepositoryDbContext repository, DbEntityEntry entry);

		/// <summary>
		///     Determines whether an entity can be modified.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="entry"> The entity entry of the entity. </param>
		/// <returns>
		///     true if the entity can be modified, false otherwise.
		/// </returns>
		bool CanModify (RepositoryDbContext repository, DbEntityEntry entry);

		/// <summary>
		///     Determines whether an entity can be reloaded.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="entry"> The entity entry of the entity. </param>
		/// <returns>
		///     true if the entity can be reloaded, false otherwise.
		/// </returns>
		bool CanReload (RepositoryDbContext repository, DbEntityEntry entry);

		/// <summary>
		///     Determines whether an entity can be validated.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="entry"> The entity entry of the entity. </param>
		/// <returns>
		///     true if the entity can be validated, false otherwise.
		/// </returns>
		bool CanValidate (RepositoryDbContext repository, DbEntityEntry entry);

		/// <summary>
		///     Fixes an entity (means: modifies values) before it is saved to the database.
		/// </summary>
		/// <param name="repository"> The repository the fixed entity belongs to. </param>
		/// <param name="entry"> The entity entry of the entity to fix. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="repository" /> or <paramref name="entry" /> is null. </exception>
		void Fix (RepositoryDbContext repository, DbEntityEntry entry);

		/// <summary>
		///     Validates an entity.
		/// </summary>
		/// <param name="repository"> The repository the validated entity belongs to. </param>
		/// <param name="entry"> The entity entry of the entity to validate. </param>
		/// <returns>
		///     The validation results if the validation results in validation errors or null if the entity is valid.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="repository" /> or <paramref name="entry" /> is null. </exception>
		DbEntityValidationResult Validate (RepositoryDbContext repository, DbEntityEntry entry);

		/// <summary>
		/// Initializes a newly created entity which was created using <see cref="IRepositorySet.Create"/>.
		/// </summary>
		/// <param name="repository">The repository the newly created entity belongs to.</param>
		/// <param name="entry">The entity entry of the entity to initialize.</param>
		/// <exception cref="ArgumentNullException"> <paramref name="repository" /> or <paramref name="entry" /> is null. </exception>
		void Initialize (RepositoryDbContext repository, DbEntityEntry entry);
	}
}
