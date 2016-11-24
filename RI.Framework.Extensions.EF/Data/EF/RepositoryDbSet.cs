﻿using System;
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
	///     Implements a repository set using an Entity Frameworks <see cref="DbSet{TEntity}" />.
	/// </summary>
	/// <typeparam name="T"> The type of the entities which are represented by this repository set. </typeparam>
	/// <remarks>
	///     <para>
	///         See <see cref="IRepositorySet{T}" /> and <see cref="DbSet{TEntity}" /> for more details.
	///     </para>
	/// </remarks>
	public class RepositoryDbSet <T> : IRepositoryDbSet, IRepositorySet<T>
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
		{
			if (set == null)
			{
				throw new ArgumentNullException(nameof(set));
			}

			if (repository == null)
			{
				throw new ArgumentNullException(nameof(repository));
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
		///     Gets the underlying Entity Framework <see cref="DbSet{TEntity}" /> used by this repository set.
		/// </summary>
		/// <value>
		///     The underlying Entity Framework <see cref="DbSet{TEntity}" /> used by this repository set.
		/// </value>
		public DbSet<T> Set { get; private set; }

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




		#region Interface: IRepositoryDbSet

		/// <inheritdoc />
		public Type EntityType => typeof(T);

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

			if (!this.CanAdd(entity))
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
		public virtual bool CanCreate ()
		{
			return (((IEntityValidation)this.Repository.GetValidator<T>())?.CanCreate(this.Repository)).GetValueOrDefault(true);
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
		public T Create ()
		{
			if (!this.CanCreate())
			{
				throw new InvalidOperationException("The entity cannot be created.");
			}

			return this.Set.Create();
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
		public virtual IEnumerable<T> GetAll ()
		{
			return this.Set;
		}

		/// <inheritdoc />
		public virtual IEnumerable<T> GetFiltered (object filter, int pageIndex, int pageSize, out int entityCount, out int pageCount)
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
				throw new ArgumentOutOfRangeException(nameof(pageSize));
			}

			entityCount = this.Set.Count();
			pageCount = ((pageSize == 0) || (entityCount == 0)) ? 1 : ((entityCount / pageSize) + (((entityCount % pageSize) == 0) ? 0 : 1));

			if ((pageIndex != 0) && (pageIndex >= pageCount))
			{
				throw new ArgumentOutOfRangeException(nameof(pageIndex));
			}

			int offset = pageIndex * pageSize;

			EntityFilter<T> entityFilter = this.Repository.GetFilter<T>();
			IQueryable<T> queryable = (entityFilter?.Filter(this.Repository, this, filter) ?? (filter as IRepositoryDbSetFilter<T>)?.Filter(this)) ?? this.Set;
			return queryable.Skip(offset).Take(pageSize);
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
			return entry == null ? false : (entry.State & (EntityState.Modified | EntityState.Added | EntityState.Deleted)) != 0;
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

			DbEntityEntry<T> entry = this.Repository.Entry(entity);
			if (entry != null)
			{
				entry.State |= EntityState.Modified;
				entry.State &= ~EntityState.Unchanged;
			}
		}

		/// <inheritdoc />
		public void Reload (T entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			if (!this.CanModify(entity))
			{
				throw new InvalidOperationException("The entity cannot be reloaded.");
			}

			DbEntityEntry<T> entry = this.Repository.Entry(entity);
			if ((entry != null) && ((entry.State & (EntityState.Modified | EntityState.Deleted | EntityState.Detached)) != 0))
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

			this.Repository.ChangeTracker.DetectChanges();

			DbEntityEntry<T> entry = this.Repository.Entry(entity);
			DbEntityValidationResult results = entry?.GetValidationResult();
			return results?.ToRepositoryErrors();
		}

		#endregion
	}
}