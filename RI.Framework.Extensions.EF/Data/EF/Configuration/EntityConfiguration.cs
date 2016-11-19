using System;
using System.Data.Entity.ModelConfiguration;




namespace RI.Framework.Data.EF.Configuration
{
	/// <summary>
	/// Implements a base class for entity configurations.
	/// </summary>
	/// <typeparam name="T">The type of entity this configuration configures.</typeparam>
	/// <remarks>
	/// <para>
	/// See <see cref="IEntityConfiguration"/> for more details.
	/// </para>
	/// </remarks>
	public abstract class EntityConfiguration <T> : EntityTypeConfiguration<T>, IEntityConfiguration
		where T : class
	{
		#region Interface: IEntityConfiguration

		/// <inheritdoc />
		public Type EntityType => typeof(T);

		#endregion
	}
}
