using System;
using System.Collections.Generic;
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
		#region Virtuals

		/// <summary>
		///     Called when it is to be determined whether an entity can be added.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="set"> The set. </param>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be added, false otherwise.
		/// </returns>
		public virtual bool CanAdd (RepositoryDbContext repository, RepositoryDbSet<T> set, T entity)
		{
			return true;
		}

		/// <summary>
		///     Called when it is to be determined whether an entity can be attached.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="set"> The set. </param>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be attached, false otherwise.
		/// </returns>
		public virtual bool CanAttach (RepositoryDbContext repository, RepositoryDbSet<T> set, T entity)
		{
			return true;
		}

		/// <summary>
		///     Called when it is to be determined whether a new instance can be created,
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="set"> The set. </param>
		/// <returns>
		///     true if a new instance can be created, false otherwise.
		/// </returns>
		public virtual bool CanCreate (RepositoryDbContext repository, RepositoryDbSet<T> set)
		{
			return true;
		}

		/// <summary>
		///     Called when it is to be determined whether an entity can be deleted.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="set"> The set. </param>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be deleted, false otherwise.
		/// </returns>
		public virtual bool CanDelete (RepositoryDbContext repository, RepositoryDbSet<T> set, T entity)
		{
			return true;
		}

		/// <summary>
		///     Called when it is to be determined whether an entity can be modified.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="set"> The set. </param>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be modified, false otherwise.
		/// </returns>
		public virtual bool CanModify (RepositoryDbContext repository, RepositoryDbSet<T> set, T entity)
		{
			return true;
		}

		/// <summary>
		///     Called when it is to be determined whether an entity can be reloaded.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="set"> The set. </param>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be reloaded, false otherwise.
		/// </returns>
		public virtual bool CanReload (RepositoryDbContext repository, RepositoryDbSet<T> set, T entity)
		{
			return true;
		}

		/// <summary>
		///     Called when it is to be determined whether an entity can be validated.
		/// </summary>
		/// <param name="repository"> The repository. </param>
		/// <param name="set"> The set. </param>
		/// <param name="entity"> The entity. </param>
		/// <returns>
		///     true if the entity can be validated, false otherwise.
		/// </returns>
		public virtual bool CanValidate (RepositoryDbContext repository, RepositoryDbSet<T> set, T entity)
		{
			return true;
		}

		/// <summary>
		///     Called when an entity is to be fixed.
		/// </summary>
		/// <param name="repository"> The repository the fixed entity belongs to. </param>
		/// <param name="set"> The set the fixed entity belongs to. </param>
		/// <param name="entity"> The entity to fix. </param>
		protected virtual void Fix (RepositoryDbContext repository, RepositoryDbSet<T> set, T entity)
		{
		}

		/// <summary>
		///     Called when an entity is to be initialized.
		/// </summary>
		/// <param name="repository"> The repository the initialized entity belongs to. </param>
		/// <param name="set"> The set the initialized entity belongs to. </param>
		/// <param name="entity"> The entity to initialize. </param>
		protected virtual void Initialize (RepositoryDbContext repository, RepositoryDbSet<T> set, T entity)
		{
		}

		/// <summary>
		///     Called when an entity is to be validated.
		/// </summary>
		/// <param name="repository"> The repository the validated entity belongs to. </param>
		/// <param name="set"> The set the validated entity belongs to. </param>
		/// <param name="entity"> The entity to validate. </param>
		/// <param name="errors"> The list which is to be populated with the validation errors (if any). </param>
		protected virtual void Validate (RepositoryDbContext repository, RepositoryDbSet<T> set, T entity, List<DbValidationError> errors)
		{
		}

		#endregion




		#region Interface: IEntityValidation

		/// <inheritdoc />
		Type IEntityValidation.EntityType => typeof(T);

		/// <inheritdoc />
		bool IEntityValidation.CanAdd (RepositoryDbContext repository, RepositoryDbSet set, object entity)
		{
			if (repository == null)
			{
				throw new ArgumentNullException(nameof(repository));
			}

			if (set == null)
			{
				throw new ArgumentNullException(nameof(set));
			}

			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			return this.CanAdd(repository, (RepositoryDbSet<T>)set, (T)entity);
		}

		/// <inheritdoc />
		bool IEntityValidation.CanAttach (RepositoryDbContext repository, RepositoryDbSet set, object entity)
		{
			if (repository == null)
			{
				throw new ArgumentNullException(nameof(repository));
			}

			if (set == null)
			{
				throw new ArgumentNullException(nameof(set));
			}

			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			return this.CanAttach(repository, (RepositoryDbSet<T>)set, (T)entity);
		}

		/// <inheritdoc />
		bool IEntityValidation.CanCreate (RepositoryDbContext repository, RepositoryDbSet set)
		{
			if (repository == null)
			{
				throw new ArgumentNullException(nameof(repository));
			}

			if (set == null)
			{
				throw new ArgumentNullException(nameof(set));
			}

			return this.CanCreate(repository, (RepositoryDbSet<T>)set);
		}

		/// <inheritdoc />
		bool IEntityValidation.CanDelete (RepositoryDbContext repository, RepositoryDbSet set, object entity)
		{
			if (repository == null)
			{
				throw new ArgumentNullException(nameof(repository));
			}

			if (set == null)
			{
				throw new ArgumentNullException(nameof(set));
			}

			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			return this.CanDelete(repository, (RepositoryDbSet<T>)set, (T)entity);
		}

		/// <inheritdoc />
		bool IEntityValidation.CanModify (RepositoryDbContext repository, RepositoryDbSet set, object entity)
		{
			if (repository == null)
			{
				throw new ArgumentNullException(nameof(repository));
			}

			if (set == null)
			{
				throw new ArgumentNullException(nameof(set));
			}

			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			return this.CanModify(repository, (RepositoryDbSet<T>)set, (T)entity);
		}

		/// <inheritdoc />
		bool IEntityValidation.CanReload (RepositoryDbContext repository, RepositoryDbSet set, object entity)
		{
			if (repository == null)
			{
				throw new ArgumentNullException(nameof(repository));
			}

			if (set == null)
			{
				throw new ArgumentNullException(nameof(set));
			}

			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			return this.CanReload(repository, (RepositoryDbSet<T>)set, (T)entity);
		}

		/// <inheritdoc />
		bool IEntityValidation.CanValidate (RepositoryDbContext repository, RepositoryDbSet set, object entity)
		{
			if (repository == null)
			{
				throw new ArgumentNullException(nameof(repository));
			}

			if (set == null)
			{
				throw new ArgumentNullException(nameof(set));
			}

			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			return this.CanValidate(repository, (RepositoryDbSet<T>)set, (T)entity);
		}

		/// <inheritdoc />
		void IEntityValidation.Fix (RepositoryDbContext repository, RepositoryDbSet set, object entity)
		{
			if (repository == null)
			{
				throw new ArgumentNullException(nameof(repository));
			}

			if (set == null)
			{
				throw new ArgumentNullException(nameof(set));
			}

			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			this.Fix(repository, (RepositoryDbSet<T>)set, (T)entity);
		}

		/// <inheritdoc />
		void IEntityValidation.Initialize (RepositoryDbContext repository, RepositoryDbSet set, object entity)
		{
			if (repository == null)
			{
				throw new ArgumentNullException(nameof(repository));
			}

			if (set == null)
			{
				throw new ArgumentNullException(nameof(set));
			}

			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			this.Initialize(repository, (RepositoryDbSet<T>)set, (T)entity);
		}

		/// <inheritdoc />
		DbEntityValidationResult IEntityValidation.Validate (RepositoryDbContext repository, RepositoryDbSet set, object entity)
		{
			if (repository == null)
			{
				throw new ArgumentNullException(nameof(repository));
			}

			if (set == null)
			{
				throw new ArgumentNullException(nameof(set));
			}

			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			List<DbValidationError> errors = new List<DbValidationError>();
			this.Validate(repository, (RepositoryDbSet<T>)set, (T)entity, errors);
			return errors.Count == 0 ? null : new DbEntityValidationResult(repository.Entry(entity), errors);
		}

		#endregion
	}
}
