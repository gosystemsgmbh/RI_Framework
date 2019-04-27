using System;
using System.Collections.Generic;




namespace RI.Framework.Data.Repository.Views
{
    /// <summary>
    ///     Event arguments for entity view events related to multiple entities.
    /// </summary>
    /// <typeparam name="T"> The type of the entities. </typeparam>
    /// <threadsafety static="false" instance="false" />
    [Serializable]
	public sealed class EntityViewItemsEventArgs <T> : EventArgs
		where T : class
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="EntityViewItemsEventArgs{T}" />.
		/// </summary>
		/// <param name="oldItems"> The old entities. </param>
		/// <param name="newItems"> The new entities. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="oldItems" /> or <paramref name="newItems" /> is null. </exception>
		public EntityViewItemsEventArgs (IList<T> oldItems, IList<T> newItems)
		{
			if (oldItems == null)
			{
				throw new ArgumentNullException(nameof(oldItems));
			}

			if (newItems == null)
			{
				throw new ArgumentNullException(nameof(newItems));
			}

			this.OldItems = oldItems;
			this.NewItems = newItems;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the new items.
		/// </summary>
		/// <value>
		///     The new items.
		/// </value>
		public IList<T> NewItems { get; }

		/// <summary>
		///     Gets the old items.
		/// </summary>
		/// <value>
		///     The old items.
		/// </value>
		public IList<T> OldItems { get; }

		#endregion
	}
}
