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
		public virtual bool CanAdd (DbRepositoryContext repository, DbRepositorySet<T> set, T entity)
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
		public virtual bool CanAttach (DbRepositoryContext repository, DbRepositorySet<T> set, T entity)
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
		public virtual bool CanCreate (DbRepositoryContext repository, DbRepositorySet<T> set)
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
		public virtual bool CanDelete (DbRepositoryContext repository, DbRepositorySet<T> set, T entity)
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
		public virtual bool CanModify (DbRepositoryContext repository, DbRepositorySet<T> set, T entity)
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
		public virtual bool CanReload (DbRepositoryContext repository, DbRepositorySet<T> set, T entity)
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
		public virtual bool CanValidate (DbRepositoryContext repository, DbRepositorySet<T> set, T entity)
		{
			return true;
		}

		/// <summary>
		///     Called when the dematerialization of an entity is to be handled before it is saved to the database.
		/// </summary>
		/// <param name="repository"> The repository the entity belongs to. </param>
		/// <param name="set"> The set. </param>
		/// <param name="entity"> The entity. </param>
		protected virtual void Dematerialize (DbRepositoryContext repository, DbRepositorySet<T> set, T entity)
		{
		}

		/// <summary>
		///     Called when an entity is to be fixed.
		/// </summary>
		/// <param name="repository"> The repository the fixed entity belongs to. </param>
		/// <param name="set"> The set the fixed entity belongs to. </param>
		/// <param name="entity"> The entity to fix. </param>
		protected virtual void Fix (DbRepositoryContext repository, DbRepositorySet<T> set, T entity)
		{
		}

		/// <summary>
		///     Called when an entity is to be initialized.
		/// </summary>
		/// <param name="repository"> The repository the initialized entity belongs to. </param>
		/// <param name="set"> The set the initialized entity belongs to. </param>
		/// <param name="entity"> The entity to initialize. </param>
		protected virtual void Initialize (DbRepositoryContext repository, DbRepositorySet<T> set, T entity)
		{
		}

		/// <summary>
		///     Called when the materialization of an entity is to be handled after it was loaded from the database.
		/// </summary>
		/// <param name="repository"> The repository the entity belongs to. </param>
		/// <param name="set"> The set. </param>
		/// <param name="entity"> The entity. </param>
		protected virtual void Materialize (DbRepositoryContext repository, DbRepositorySet<T> set, T entity)
		{
		}

		/// <summary>
		///     Called when an entity is to be validated.
		/// </summary>
		/// <param name="repository"> The repository the validated entity belongs to. </param>
		/// <param name="set"> The set the validated entity belongs to. </param>
		/// <param name="entity"> The entity to validate. </param>
		/// <param name="errors"> The list which is to be populated with the validation errors (if any). </param>
		protected virtual void Validate (DbRepositoryContext repository, DbRepositorySet<T> set, T entity, List<DbValidationError> errors)
		{
		}

		#endregion




		#region Interface: IEntityValidation

		/// <inheritdoc />
		Type IEntityValidation.EntityType => typeof(T);

		/// <inheritdoc />
		bool IEntityValidation.CanAdd (DbRepositoryContext repository, DbRepositorySet set, object entity)
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

			return this.CanAdd(repository, (DbRepositorySet<T>)set, (T)entity);
		}

		/// <inheritdoc />
		bool IEntityValidation.CanAttach (DbRepositoryContext repository, DbRepositorySet set, object entity)
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

			return this.CanAttach(repository, (DbRepositorySet<T>)set, (T)entity);
		}

		/// <inheritdoc />
		bool IEntityValidation.CanCreate (DbRepositoryContext repository, DbRepositorySet set)
		{
			if (repository == null)
			{
				throw new ArgumentNullException(nameof(repository));
			}

			if (set == null)
			{
				throw new ArgumentNullException(nameof(set));
			}

			return this.CanCreate(repository, (DbRepositorySet<T>)set);
		}

		/// <inheritdoc />
		bool IEntityValidation.CanDelete (DbRepositoryContext repository, DbRepositorySet set, object entity)
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

			return this.CanDelete(repository, (DbRepositorySet<T>)set, (T)entity);
		}

		/// <inheritdoc />
		bool IEntityValidation.CanModify (DbRepositoryContext repository, DbRepositorySet set, object entity)
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

			return this.CanModify(repository, (DbRepositorySet<T>)set, (T)entity);
		}

		/// <inheritdoc />
		bool IEntityValidation.CanReload (DbRepositoryContext repository, DbRepositorySet set, object entity)
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

			return this.CanReload(repository, (DbRepositorySet<T>)set, (T)entity);
		}

		/// <inheritdoc />
		bool IEntityValidation.CanValidate (DbRepositoryContext repository, DbRepositorySet set, object entity)
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

			return this.CanValidate(repository, (DbRepositorySet<T>)set, (T)entity);
		}

		/// <inheritdoc />
		void IEntityValidation.Dematerialize (DbRepositoryContext repository, DbRepositorySet set, object entity)
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

			this.Dematerialize(repository, (DbRepositorySet<T>)set, (T)entity);
		}

		/// <inheritdoc />
		void IEntityValidation.Fix (DbRepositoryContext repository, DbRepositorySet set, object entity)
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

			this.Fix(repository, (DbRepositorySet<T>)set, (T)entity);
		}

		/// <inheritdoc />
		void IEntityValidation.Initialize (DbRepositoryContext repository, DbRepositorySet set, object entity)
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

			this.Initialize(repository, (DbRepositorySet<T>)set, (T)entity);
		}

		/// <inheritdoc />
		void IEntityValidation.Materialize (DbRepositoryContext repository, DbRepositorySet set, object entity)
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

			this.Materialize(repository, (DbRepositorySet<T>)set, (T)entity);
		}

		/// <inheritdoc />
		DbEntityValidationResult IEntityValidation.Validate (DbRepositoryContext repository, DbRepositorySet set, object entity)
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
			this.Validate(repository, (DbRepositorySet<T>)set, (T)entity, errors);
			return errors.Count == 0 ? null : new DbEntityValidationResult(repository.Entry(entity), errors);
		}

		#endregion
	}
}
