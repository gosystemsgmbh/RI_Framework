using System;

using RI.Framework.Composition.Model;




namespace RI.Framework.Data.EF.Filter
{
	/// <summary>
	///     Defines the interface for entity filter classes.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Entity filter classes are used to filter entities in the context of an <see cref="RepositoryDbSet{T}" />.
	///     </para>
	///     <para>
	///         Entity filter classes are created during <see cref="RepositoryDbContext.OnFiltersCreating" />.
	///     </para>
	/// </remarks>
	[Export]
	public interface IEntityFilter
	{
		/// <summary>
		///     Gets the type of entities this entity filter filters.
		/// </summary>
		/// <value>
		///     The type of entities this entity filter filters.
		/// </value>
		Type EntityType { get; }
	}
}
