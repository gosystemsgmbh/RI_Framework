using System;
using System.Data.Entity.ModelConfiguration;




namespace RI.Framework.Data.EF.Configuration
{
	public abstract class EntityConfiguration <T> : EntityTypeConfiguration<T>, IEntityConfiguration
		where T : class
	{
		#region Interface: IEntityConfiguration

		public Type EntityType => typeof(T);

		#endregion
	}
}
