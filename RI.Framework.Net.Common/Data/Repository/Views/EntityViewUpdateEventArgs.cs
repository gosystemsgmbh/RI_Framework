using System;




namespace RI.Framework.Data.Repository.Views
{
	/// <summary>
	///     Event arguments for the entity views <see cref="EntityView{TEntity,TViewObject}.Updating" /> and <see cref="EntityView{TEntity,TViewObject}.Updated" /> events.
	/// </summary>
	[Serializable]
	public sealed class EntityViewUpdateEventArgs : EventArgs
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="EntityViewUpdateEventArgs" />.
		/// </summary>
		/// <param name="resetPageNumber"> Specifies whether <see cref="EntityView{TEntity,TViewObject}.PageNumber" /> is reset to 1. </param>
		public EntityViewUpdateEventArgs (bool resetPageNumber)
		{
			this.ResetPageNumber = resetPageNumber;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets whether <see cref="EntityView{TEntity,TViewObject}.PageNumber" /> is reset to 1.
		/// </summary>
		/// <value>
		///     true if <see cref="EntityView{TEntity,TViewObject}.PageNumber" /> is reset to 1, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         See <see cref="EntityView{TEntity,TViewObject}.PageNumber" /> for more details.
		///     </para>
		/// </remarks>
		public bool ResetPageNumber { get; }

		#endregion
	}
}
