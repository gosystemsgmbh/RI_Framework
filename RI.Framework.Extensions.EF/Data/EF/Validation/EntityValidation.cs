using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;




namespace RI.Framework.Data.EF.Validation
{
	public abstract class EntityValidation <T> : IEntityValidation
		where T : class
	{
		#region Virtuals

		public virtual void Validate (RepositoryDbContext repository, DbEntityEntry entry, T entity, List<DbValidationError> errors)
		{
		}

		#endregion




		#region Interface: IEntityValidation

		public Type EntityType => typeof(T);

		public DbEntityValidationResult Validate (RepositoryDbContext repository, DbEntityEntry entry)
		{
			List<DbValidationError> errors = new List<DbValidationError>();
			this.Validate(repository, entry, (T)entry.Entity, errors);
			return errors.Count == 0 ? null : new DbEntityValidationResult(entry, errors);
		}

		#endregion
	}
}
