using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

using RI.Framework.Composition.Model;




namespace RI.Framework.Data.EF.Validation
{
	[Export]
	public interface IEntityValidation
	{
		Type EntityType { get; }

		DbEntityValidationResult Validate (RepositoryDbContext repository, DbEntityEntry entry);
	}
}
