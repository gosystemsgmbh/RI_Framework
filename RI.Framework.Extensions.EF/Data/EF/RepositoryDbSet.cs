﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;

using RI.Framework.Data.EF.Filter;
using RI.Framework.Data.EF.Validation;
using RI.Framework.Data.Repository;
using RI.Framework.Services;
using RI.Framework.Services.Logging;




namespace RI.Framework.Data.EF
{
	/// <summary>
	///     Implements a non-generic repository set using an Entity Frameworks <see cref="DbSet{TEntity}" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This is only a non-generic base class inherited by <see cref="RepositoryDbSet{T}" /> in order to provide an implementation of <see cref="IRepositorySet" />.
	///         This implementation simply forwards everything to <see cref="RepositoryDbSet{T}" />.
	///         You cannot inherit from this class.
	///         See <see cref="RepositoryDbSet{T}" /> for more details.
	///     </para>
	/// </remarks>
	public abstract class RepositoryDbSet : IRepositorySet
	{
		#region Instance Constructor/Destructor

		internal RepositoryDbSet (RepositoryDbContext repository, DbSet set)
		{
			if (repository == null)
			{
				throw new ArgumentNullException(nameof(repository));
			}

			if (set == null)
			{
				throw new ArgumentNullException(nameof(set));
			}

			this.Repository = repository;
			this.Set = set;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the repository this repository set belongs to.
		/// </summary>
		/// <value>
		///     The repository this repository set belongs to.
		/// </value>
		public RepositoryDbContext Repository { get; private set; }

		/// <summary>
		///     Gets the underlying Entity Framework <see cref="DbSet" /> used by this repository set.
		/// </summary>
		/// <value>
		///     The underlying Entity Framework <see cref="DbSet" /> used by this repository set.
		/// </value>
		public DbSet Set { get; private set; }

		#endregion




		#region Abstracts

		/// <inheritdoc cref="IRepositorySet.EntityType" />
		protected abstract Type EntityTypeInternal { get; }

		/// <inheritdoc cref="IRepositorySet.Add" />
		protected abstract void AddInternal (object entity);

		/// <inheritdoc cref="IRepositorySet.Attach" />
		protected abstract void AttachInternal (object entity);

		/// <inheritdoc cref="IRepositorySet.CanAdd" />
		protected abstract bool CanAddInternal (object entity);

		/// <inheritdoc cref="IRepositorySet.CanAttach" />
		protected abstract bool CanAttachInternal (object entity);

		/// <inheritdoc cref="IRepositorySet.CanCreate" />
		protected abstract bool CanCreateInternal ();

		/// <inheritdoc cref="IRepositorySet.CanDelete" />
		protected abstract bool CanDeleteInternal (object entity);

		/// <inheritdoc cref="IRepositorySet.CanModify" />
		protected abstract bool CanModifyInternal (object entity);

		/// <inheritdoc cref="IRepositorySet.CanReload" />
		protected abstract bool CanReloadInternal (object entity);

		/// <inheritdoc cref="IRepositorySet.CanValidate" />
		protected abstract bool CanValidateInternal (object entity);

		/// <inheritdoc cref="IRepositorySet.Create" />
		protected abstract object CreateInternal ();

		/// <inheritdoc cref="IRepositorySet.Delete" />
		protected abstract void DeleteInternal (object entity);

		/// <inheritdoc cref="IRepositorySet.GetAll" />
		protected abstract IEnumerable<object> GetAllInternal ();

		/// <inheritdoc cref="IRepositorySet.GetCount" />
		protected abstract int GetCountInternal ();

		/// <inheritdoc cref="IRepositorySet.GetFiltered(object,int,int,out int,out int,out int)" />
		protected abstract IEnumerable<object> GetFilteredInternal (object filter, int pageIndex, int pageSize, out int totalCount, out int filteredCount, out int pageCount);

		/// <inheritdoc cref="IRepositorySet.GetFiltered(IEnumerable,object,int,int,out int,out int,out int)" />
		protected abstract IEnumerable<object> GetFilteredInternal (IEnumerable entities, object filter, int pageIndex, int pageSize, out int totalCount, out int filteredCount, out int pageCount);

		/// <inheritdoc cref="IRepositorySet.IsModified" />
		protected abstract bool IsModifiedInternal (object entity);

		/// <inheritdoc cref="IRepositorySet.IsValid" />
		protected abstract bool IsValidInternal (object entity);

		/// <inheritdoc cref="IRepositorySet.Modify" />
		protected abstract void ModifyInternal (object entity);

		/// <inheritdoc cref="IRepositorySet.Reload" />
		protected abstract void ReloadInternal (object entity);

		/// <inheritdoc cref="IRepositorySet.Validate" />
		protected abstract RepositorySetErrors ValidateInternal (object entity);

		#endregion




		#region Interface: IRepositorySet

		/// <inheritdoc />
		public Type EntityType => this.EntityTypeInternal;

		/// <inheritdoc />
		public void Add (object entity)
		{
			this.AddInternal(entity);
		}

		/// <inheritdoc />
		public void Attach (object entity)
		{
			this.AttachInternal(entity);
		}

		/// <inheritdoc />
		public bool CanAdd (object entity)
		{
			return this.CanAddInternal(entity);
		}

		/// <inheritdoc />
		public bool CanAttach (object entity)
		{
			return this.CanAttachInternal(entity);
		}

		/// <inheritdoc />
		public bool CanCreate ()
		{
			return this.CanCreateInternal();
		}

		/// <inheritdoc />
		public bool CanDelete (object entity)
		{
			return this.CanDeleteInternal(entity);
		}

		/// <inheritdoc />
		public bool CanModify (object entity)
		{
			return this.CanModifyInternal(entity);
		}

		/// <inheritdoc />
		public bool CanReload (object entity)
		{
			return this.CanReloadInternal(entity);
		}

		/// <inheritdoc />
		public bool CanValidate (object entity)
		{
			return this.CanValidateInternal(entity);
		}

		/// <inheritdoc />
		public object Create ()
		{
			return this.CreateInternal();
		}

		/// <inheritdoc />
		public void Delete (object entity)
		{
			this.DeleteInternal(entity);
		}

		/// <inheritdoc />
		public IEnumerable<object> GetAll ()
		{
			return this.GetAllInternal();
		}

		/// <inheritdoc />
		public int GetCount ()
		{
			return this.GetCountInternal();
		}

		/// <inheritdoc />
		public IEnumerable<object> GetFiltered (object filter, int pageIndex, int pageSize, out int totalCount, out int filteredCount, out int pageCount)
		{
			return this.GetFilteredInternal(filter, pageIndex, pageSize, out totalCount, out filteredCount, out pageCount);
		}

		/// <inheritdoc />
		public IEnumerable<object> GetFiltered (IEnumerable entities, object filter, int pageIndex, int pageSize, out int totalCount, out int filteredCount, out int pageCount)
		{
			return this.GetFilteredInternal(entities, filter, pageIndex, pageSize, out totalCount, out filteredCount, out pageCount);
		}

		/// <inheritdoc />
		public bool IsModified (object entity)
		{
			return this.IsModifiedInternal(entity);
		}

		/// <inheritdoc />
		public bool IsValid (object entity)
		{
			return this.IsValidInternal(entity);
		}

		/// <inheritdoc />
		public void Modify (object entity)
		{
			this.ModifyInternal(entity);
		}

		/// <inheritdoc />
		public void Reload (object entity)
		{
			this.ReloadInternal(entity);
		}

		/// <inheritdoc />
		public RepositorySetErrors Validate (object entity)
		{
			return this.ValidateInternal(entity);
		}

		#endregion
	}

	/// <summary>
	///     Implements a generic repository set using an Entity Frameworks <see cref="DbSet{TEntity}" />.
	/// </summary>
	/// <typeparam name="T"> The type of the entities which are represented by this repository set. </typeparam>
	/// <remarks>
	///     <para>
	///         See <see cref="IRepositorySet{T}" /> and <see cref="DbSet{TEntity}" /> for more details.
	///     </para>
	/// </remarks>
	public class RepositoryDbSet <T> : RepositoryDbSet, IRepositorySet<T>
		where T : class
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="RepositoryDbSet{T}" />.
		/// </summary>
		/// <param name="repository"> The repository this repository set belongs to. </param>
		/// <param name="set"> The underlying Entity Framework <see cref="DbSet{TEntity}" /> used by this repository set. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="repository" /> or <paramref name="set" /> is null. </exception>
		public RepositoryDbSet (RepositoryDbContext repository, DbSet<T> set)
			: base(repository, set)
		{
			if (set == null)
			{
				throw new ArgumentNullException(nameof(set));
			}

			this.Set = set;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the underlying Entity Framework <see cref="DbSet{TEntity}" /> used by this repository set.
		/// </summary>
		/// <value>
		///     The underlying Entity Framework <see cref="DbSet{TEntity}" /> used by this repository set.
		/// </value>
		public new DbSet<T> Set { get; private set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Logs a message.
		/// </summary>
		/// <param name="severity"> The severity of the message. </param>
		/// <param name="format"> The message. </param>
		/// <param name="args"> The arguments which will be expanded into the message (comparable to <see cref="string.Format(string, object[])" />). </param>
		/// <remarks>
		///     <para>
		///         <see cref="ILogService" /> is used, obtained through <see cref="ServiceLocator" />.
		///         If no <see cref="ILogService" /> is available, no logging is performed.
		///     </para>
		/// </remarks>
		protected void Log (LogLevel severity, string format, params object[] args)
		{
			LogLocator.Log(severity, this.GetType().Name, format, args);
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected sealed override Type EntityTypeInternal => typeof(T);

		/// <inheritdoc />
		protected sealed override void AddInternal (object entity)
		{
			this.Add((T)entity);
		}

		/// <inheritdoc />
		protected sealed override void AttachInternal (object entity)
		{
			this.Attach((T)entity);
		}

		/// <inheritdoc />
		protected sealed override bool CanAddInternal (object entity)
		{
			return this.CanAdd((T)entity);
		}

		/// <inheritdoc />
		protected sealed override bool CanAttachInternal (object entity)
		{
			return this.CanAttach((T)entity);
		}

		/// <inheritdoc />
		protected sealed override bool CanCreateInternal ()
		{
			return (((IEntityValidation)this.Repository.GetValidator<T>())?.CanCreate(this.Repository)).GetValueOrDefault(true);
		}

		/// <inheritdoc />
		protected sealed override bool CanDeleteInternal (object entity)
		{
			return this.CanDelete((T)entity);
		}

		/// <inheritdoc />
		protected sealed override bool CanModifyInternal (object entity)
		{
			return this.CanModify((T)entity);
		}

		/// <inheritdoc />
		protected sealed override bool CanReloadInternal (object entity)
		{
			return this.CanReload((T)entity);
		}

		/// <inheritdoc />
		protected sealed override bool CanValidateInternal (object entity)
		{
			return this.CanValidate((T)entity);
		}

		/// <inheritdoc />
		protected sealed override object CreateInternal ()
		{
			return this.Create();
		}

		/// <inheritdoc />
		protected sealed override void DeleteInternal (object entity)
		{
			this.Delete((T)entity);
		}

		/// <inheritdoc />
		protected sealed override IEnumerable<object> GetAllInternal ()
		{
			return this.GetAll();
		}

		/// <inheritdoc />
		protected sealed override int GetCountInternal ()
		{
			return this.Set.Count();
		}

		/// <inheritdoc />
		protected sealed override IEnumerable<object> GetFilteredInternal (object filter, int pageIndex, int pageSize, out int totalCount, out int filteredCount, out int pageCount)
		{
			return this.GetFiltered(filter, pageIndex, pageSize, out totalCount, out filteredCount, out pageCount);
		}

		/// <inheritdoc />
		protected sealed override IEnumerable<object> GetFilteredInternal (IEnumerable entities, object filter, int pageIndex, int pageSize, out int totalCount, out int filteredCount, out int pageCount)
		{
			return this.GetFiltered(entities.Cast<T>(), filter, pageIndex, pageSize, out totalCount, out filteredCount, out pageCount);
		}

		/// <inheritdoc />
		protected sealed override bool IsModifiedInternal (object entity)
		{
			return this.IsModified((T)entity);
		}

		/// <inheritdoc />
		protected sealed override bool IsValidInternal (object entity)
		{
			return this.IsValid((T)entity);
		}

		/// <inheritdoc />
		protected sealed override void ModifyInternal (object entity)
		{
			this.Modify((T)entity);
		}

		/// <inheritdoc />
		protected sealed override void ReloadInternal (object entity)
		{
			this.Reload((T)entity);
		}

		/// <inheritdoc />
		protected sealed override RepositorySetErrors ValidateInternal (object entity)
		{
			return this.Validate((T)entity);
		}

		#endregion




		#region Interface: IRepositorySet<T>

		/// <inheritdoc />
		public void Add (T entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			if (!this.CanAdd(entity))
			{
				throw new InvalidOperationException("The entity cannot be added.");
			}

			this.Set.Add(entity);
		}

		/// <inheritdoc />
		public void Attach (T entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			if (!this.CanAttach(entity))
			{
				throw new InvalidOperationException("The entity cannot be attached.");
			}

			this.Set.Attach(entity);
		}

		/// <inheritdoc />
		public virtual bool CanAdd (T entity)
		{
			return (((IEntityValidation)this.Repository.GetValidator<T>())?.CanAdd(this.Repository, entity)).GetValueOrDefault(true);
		}

		/// <inheritdoc />
		public virtual bool CanAttach (T entity)
		{
			return (((IEntityValidation)this.Repository.GetValidator<T>())?.CanAttach(this.Repository, entity)).GetValueOrDefault(true);
		}

		/// <inheritdoc />
		public virtual bool CanDelete (T entity)
		{
			return (((IEntityValidation)this.Repository.GetValidator<T>())?.CanDelete(this.Repository, this.Repository.Entry(entity))).GetValueOrDefault(true);
		}

		/// <inheritdoc />
		public virtual bool CanModify (T entity)
		{
			return (((IEntityValidation)this.Repository.GetValidator<T>())?.CanModify(this.Repository, this.Repository.Entry(entity))).GetValueOrDefault(true);
		}

		/// <inheritdoc />
		public virtual bool CanReload (T entity)
		{
			return (((IEntityValidation)this.Repository.GetValidator<T>())?.CanReload(this.Repository, this.Repository.Entry(entity))).GetValueOrDefault(true);
		}

		/// <inheritdoc />
		public virtual bool CanValidate (T entity)
		{
			return (((IEntityValidation)this.Repository.GetValidator<T>())?.CanValidate(this.Repository, this.Repository.Entry(entity))).GetValueOrDefault(true);
		}

		/// <inheritdoc />
		public new T Create ()
		{
			if (!this.CanCreate())
			{
				throw new InvalidOperationException("New entities cannot be created.");
			}

			T entity = this.Set.Create();
			((IEntityValidation)this.Repository.GetValidator<T>())?.Initialize(this.Repository, this.Repository.Entry(entity));
			return entity;
		}

		/// <inheritdoc />
		public void Delete (T entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			if (!this.CanDelete(entity))
			{
				throw new InvalidOperationException("The entity cannot be deleted.");
			}

			this.Set.Remove(entity);
		}

		/// <inheritdoc />
		public new virtual IEnumerable<T> GetAll ()
		{
			return this.Set;
		}

		/// <inheritdoc />
		public new virtual IEnumerable<T> GetFiltered (object filter, int pageIndex, int pageSize, out int totalCount, out int filteredCount, out int pageCount)
		{
			return this.GetFiltered(null, filter, pageIndex, pageSize, out totalCount, out filteredCount, out pageCount);
		}

		/// <inheritdoc />
		public virtual IEnumerable<T> GetFiltered (IEnumerable<T> entities, object filter, int pageIndex, int pageSize, out int totalCount, out int filteredCount, out int pageCount)
		{
			if (pageIndex < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(pageIndex));
			}

			if (pageSize < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(pageSize));
			}

			if ((pageSize == 0) && (pageIndex != 0))
			{
				throw new ArgumentOutOfRangeException(nameof(pageIndex));
			}

			EntityFilter<T> entityFilter = this.Repository.GetFilter<T>();
			if (entityFilter == null)
			{
				throw new InvalidOperationException("No entity filter available.");
			}

			IOrderedQueryable<T> queryable = entityFilter.Filter(this.Repository, this, entities, filter);

			totalCount = this.GetCount();
			filteredCount = queryable.Count();
			pageCount = ((pageSize == 0) || (filteredCount == 0)) ? 1 : ((filteredCount / pageSize) + (((filteredCount % pageSize) == 0) ? 0 : 1));

			if ((pageIndex != 0) && (pageIndex >= pageCount))
			{
				throw new ArgumentOutOfRangeException(nameof(pageIndex));
			}

			if (pageSize == 0)
			{
				return queryable;
			}

			int offset = pageIndex * pageSize;
			IQueryable<T> pagedQueryable = queryable.Skip(offset).Take(pageSize);

			return pagedQueryable;
		}

		/// <inheritdoc />
		public bool IsModified (T entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			this.Repository.ChangeTracker.DetectChanges();

			DbEntityEntry<T> entry = this.Repository.Entry(entity);
			return entry == null ? false : ((entry.State & (EntityState.Modified | EntityState.Added | EntityState.Deleted)) != 0);
		}

		/// <inheritdoc />
		public bool IsValid (T entity)
		{
			return this.Validate(entity) == null;
		}

		/// <inheritdoc />
		public void Modify (T entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			if (!this.CanModify(entity))
			{
				throw new InvalidOperationException("The entity cannot be modified.");
			}

			this.Repository.ChangeTracker.DetectChanges();

			DbEntityEntry<T> entry = this.Repository.Entry(entity);
			if ((entry != null) && ((entry.State & (EntityState.Unchanged)) != 0))
			{
				entry.State = EntityState.Modified;
			}
		}

		/// <inheritdoc />
		public void Reload (T entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			if (!this.CanReload(entity))
			{
				throw new InvalidOperationException("The entity cannot be reloaded.");
			}

			this.Repository.ChangeTracker.DetectChanges();

			DbEntityEntry<T> entry = this.Repository.Entry(entity);
			if ((entry != null) && ((entry.State & (EntityState.Modified)) != 0))
			{
				entry.Reload();
			}
		}

		/// <inheritdoc />
		public RepositorySetErrors Validate (T entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			if (!this.CanValidate(entity))
			{
				throw new InvalidOperationException("The entity cannot be validated.");
			}

			this.Repository.ChangeTracker.DetectChanges();

			DbEntityEntry<T> entry = this.Repository.Entry(entity);
			DbEntityValidationResult results = entry?.GetValidationResult();
			return results?.ToRepositoryErrors();
		}

		#endregion
	}
}
