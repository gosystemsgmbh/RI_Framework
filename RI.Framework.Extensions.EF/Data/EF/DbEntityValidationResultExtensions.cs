using System;
using System.Data.Entity.Validation;

using RI.Framework.Data.Repository;
using RI.Framework.Utilities;




namespace RI.Framework.Data.EF
{
	public static class DbEntityValidationResultExtensions
	{
		#region Static Methods

		public static RepositoryErrors ToRepositoryErrors (this DbEntityValidationResult entityValidationResult)
		{
			if (entityValidationResult == null)
			{
				throw new ArgumentNullException(nameof(entityValidationResult));
			}

			RepositoryErrors errors = new RepositoryErrors();
			foreach (DbValidationError result in entityValidationResult.ValidationErrors)
			{
				if (result.PropertyName.IsNullOrEmpty())
				{
					errors.AddEntityError(result.ErrorMessage);
				}
				else
				{
					errors.AddPropertyError(result.PropertyName, result.ErrorMessage);
				}
			}
			return errors;
		}

		#endregion
	}
}
