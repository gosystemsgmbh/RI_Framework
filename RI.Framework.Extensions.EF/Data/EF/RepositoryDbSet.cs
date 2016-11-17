using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

using RI.Framework.Data.Repository;
using RI.Framework.Services;
using RI.Framework.Services.Logging;




namespace RI.Framework.Data.EF
{
	public class RepositoryDbSet <T> : IRepositoryDbSet, IRepositorySet<T>
		where T : class
	{
		#region Instance Constructor/Destructor

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

		public RepositoryDbContext Repository { get; private set; }

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

		public Type EntityType => typeof(T);

		#endregion




		#region Interface: IRepositorySet<T>

		public virtual void Add (T entity)
		{
			throw new NotImplementedException();
		}

		public virtual bool CanAdd (T entity)
		{
			throw new NotImplementedException();
		}

		public virtual bool CanCreate ()
		{
			throw new NotImplementedException();
		}

		public virtual bool CanDelete (T entity)
		{
			throw new NotImplementedException();
		}

		public virtual bool CanModify (T entity)
		{
			throw new NotImplementedException();
		}

		public virtual bool CanReload (T entity)
		{
			throw new NotImplementedException();
		}

		public virtual T Create ()
		{
			throw new NotImplementedException();
		}

		public virtual void Delete (T entity)
		{
			throw new NotImplementedException();
		}

		public virtual IEnumerable<T> GetAll ()
		{
			throw new NotImplementedException();
		}

		public virtual IEnumerable<T> GetFiltered (object filter, int pageIndex, int pageSize, out int entityCount, out int pageCount)
		{
			throw new NotImplementedException();
		}

		public virtual bool IsModified (T entity)
		{
			throw new NotImplementedException();
		}

		public virtual void Modify (T entity)
		{
			throw new NotImplementedException();
		}

		public virtual void Reload (T entity)
		{
			throw new NotImplementedException();
		}

		public virtual RepositoryErrors Validate (T entity)
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
