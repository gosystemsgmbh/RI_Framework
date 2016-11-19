using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;




namespace RI.Framework.Data.EF.Validation
{
	/// <summary>
	///     Implements a base class for entity validators.
	/// </summary>
	/// <typeparam name="T"> The type of entity this validator validates. </typeparam>
	/// <remarks>
	///     <para>
	///         See <see cref="IEntityValidation" /> for more details.
	///     </para>
	/// </remarks>
	public abstract class EntityValidation <T> : IEntityValidation
		where T : class
	{
		#region Abstracts

		/// <summary>
		///     Called when an entity is to be validated.
		/// </summary>
		/// <param name="repository"> The repository the validated entity belongs to. </param>
		/// <param name="entry"> The entity entry of the entity to validate. </param>
		/// <param name="entity"> The entity to validate. </param>
		/// <param name="errors"> The list which is to be populated with the validation errors (if any). </param>
		protected abstract void Validate (RepositoryDbContext repository, DbEntityEntry entry, T entity, List<DbValidationError> errors);

		#endregion




		#region Interface: IEntityValidation

		/// <inheritdoc />
		public Type EntityType => typeof(T);

		/// <inheritdoc />
		public DbEntityValidationResult Validate (RepositoryDbContext repository, DbEntityEntry entry)
		{
			if (repository == null)
			{
				throw new ArgumentNullException(nameof(repository));
			}

			if (entry == null)
			{
				throw new ArgumentNullException(nameof(entry));
			}
			
			List<DbValidationError> errors = new List<DbValidationError>();
			this.Validate(repository, entry, (T)entry.Entity, errors);
			return errors.Count == 0 ? null : new DbEntityValidationResult(entry, errors);
		}

		#endregion
	}
}
