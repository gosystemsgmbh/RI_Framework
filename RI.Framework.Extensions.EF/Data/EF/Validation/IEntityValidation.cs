using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

using RI.Framework.Composition.Model;




namespace RI.Framework.Data.EF.Validation
{
	/// <summary>
	/// Defines the interface for entity validation classes.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Entity validation classes are used to validate an entity in the context of an <see cref="RepositoryDbContext"/>.
	/// </para>
	/// <para>
	/// Entity validation classes are created during <see cref="RepositoryDbContext.OnValidatorsCreating"/>.
	/// </para>
	/// </remarks>
	[Export]
	public interface IEntityValidation
	{
		/// <summary>
		/// Gets the type of entities this entity validator validates.
		/// </summary>
		/// <value>
		/// The type of entities this entity validator validates.
		/// </value>
		Type EntityType { get; }

		/// <summary>
		/// Validates an entity.
		/// </summary>
		/// <param name="repository">The repository the validated entity belongs to.</param>
		/// <param name="entry">The entity entry of the entity to validate.</param>
		/// <returns>
		/// The validation results if the validation results in validation errors or null if the entity is valid.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="repository"/> or <paramref name="entry"/> is null.</exception>
		DbEntityValidationResult Validate (RepositoryDbContext repository, DbEntityEntry entry);
	}
}
