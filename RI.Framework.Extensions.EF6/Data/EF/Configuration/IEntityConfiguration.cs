using System;

using RI.Framework.Composition.Model;




namespace RI.Framework.Data.EF.Configuration
{
	/// <summary>
	///     Defines the interface for entity configuration classes.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Entity validation classes are used to describe the configuration of an entity in the context of an <see cref="RepositoryDbContext" />.
	///         This includes a description of what a certain entity actually is and how it is mapped to the database used by the corresponding <see cref="RepositoryDbContext" />.
	///     </para>
	///     <para>
	///         Entity validation classes are created during <see cref="RepositoryDbContext.OnConfigurationCreating" />.
	///     </para>
	/// </remarks>
	[Export]
	public interface IEntityConfiguration
	{
		/// <summary>
		///     Gets the type of entities this entity configuration configures.
		/// </summary>
		/// <value>
		///     The type of entities this entity configuration configures.
		/// </value>
		Type EntityType { get; }
	}
}
