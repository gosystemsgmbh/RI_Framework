using System;
using System.Data.Entity.Validation;

using RI.Framework.Data.Repository;
using RI.Framework.Utilities;




namespace RI.Framework.Data.EF
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="DbEntityValidationResult" /> type.
	/// </summary>
	public static class DbEntityValidationResultExtensions
	{
		#region Static Methods

		/// <summary>
		/// Converts validation results to repository errors.
		/// </summary>
		/// <param name="entityValidationResult">The validation results to convert.</param>
		/// <returns>
		/// The repository errors represented by <paramref name="entityValidationResult"/> or null if no validation errors are defined.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="entityValidationResult"/> is null.</exception>
		public static RepositorySetErrors ToRepositoryErrors (this DbEntityValidationResult entityValidationResult)
		{
			if (entityValidationResult == null)
			{
				throw new ArgumentNullException(nameof(entityValidationResult));
			}

			if (entityValidationResult.ValidationErrors.Count == 0)
			{
				return null;
			}

			RepositorySetErrors errors = new RepositorySetErrors();
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
