using System;




namespace RI.Framework.Data.Repository.Views
{
	/// <summary>
	///     Describes the update strategy used for updating entity collections of <see cref="EntityView{TEntity,TViewObject}" />.
	/// </summary>
	[Serializable]
	public enum EntityViewCollectionUpdateStrategy
	{
		/// <summary>
		///     New instances of the collections are created for each update.
		/// </summary>
		Recreate = 0,

		/// <summary>
		///     The collection instances are kept but are being cleared/updated on each update.
		/// </summary>
		Update = 1,
	}
}
