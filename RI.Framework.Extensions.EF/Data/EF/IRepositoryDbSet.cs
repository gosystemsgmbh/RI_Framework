using System;




namespace RI.Framework.Data.EF
{
	/// <summary>
	///     Defines the non-generic interface for an Entity Framework repository set.
	/// </summary>
	public interface IRepositoryDbSet
	{
		/// <summary>
		///     Gets the type of entities this repository set manages.
		/// </summary>
		/// <value>
		///     The type of entities this repository set manages.
		/// </value>
		Type EntityType { get; }
	}
}
