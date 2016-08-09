using System;




namespace RI.Framework.Collections.Comparison
{
	/// <summary>
	///     Specifies comparison options when using <see cref="CollectionComparer{T}" /> to compare two collections.
	/// </summary>
	[Serializable]
	[Flags]
	public enum CollectionComparerFlags
	{
		/// <summary>
		///     No options.
		/// </summary>
		None = 0x00,

		/// <summary>
		///     Two collections are also considered equal if their elements are equal but in different orders.
		/// </summary>
		IgnoreOrder = 0x01,

		/// <summary>
		///     Two collections are only considered equal if their elements are of the same reference, regardless of the elements own behaviour regarding equality.
		/// </summary>
		ReferenceEquality = 0x02,
	}
}
