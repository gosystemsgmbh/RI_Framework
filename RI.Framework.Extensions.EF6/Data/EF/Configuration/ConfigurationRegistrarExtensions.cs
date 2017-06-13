using System;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq.Expressions;
using System.Reflection;




namespace RI.Framework.Data.EF.Configuration
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="ConfigurationRegistrar" /> type.
	/// </summary>
	public static class ConfigurationRegistrarExtensions
	{
		#region Static Methods

		/// <summary>
		///     Registers an entity configuration class instance.
		/// </summary>
		/// <param name="registrar"> The entity configuration registrar. </param>
		/// <param name="entityTypeConfiguration"> The entity configuration class instance to register. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="registrar" /> or <paramref name="entityTypeConfiguration" /> is null. </exception>
		public static void Add (this ConfigurationRegistrar registrar, IEntityConfiguration entityTypeConfiguration)
		{
			if (registrar == null)
			{
				throw new ArgumentNullException(nameof(registrar));
			}

			if (entityTypeConfiguration == null)
			{
				throw new ArgumentNullException(nameof(entityTypeConfiguration));
			}

			Expression<Func<ConfigurationRegistrar, ConfigurationRegistrar>> fakeExpression = x => x.Add((EntityTypeConfiguration<string>)null);
			MethodInfo fakeMethod = ((MethodCallExpression)fakeExpression.Body).Method;
			MethodInfo genericMethod = fakeMethod.GetGenericMethodDefinition();
			MethodInfo concreteMethod = genericMethod.MakeGenericMethod(entityTypeConfiguration.EntityType);
			concreteMethod.Invoke(registrar, new object[] {entityTypeConfiguration});
		}

		#endregion
	}
}
