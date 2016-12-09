using System;
using System.Collections.Generic;




namespace RI.Framework.Data.Repository.Views
{
	[Serializable]
	public sealed class EntityViewItemsEventArgs <T> : EventArgs
		where T : class
	{
		#region Instance Constructor/Destructor

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

		public IList<T> NewItems { get; }

		public IList<T> OldItems { get; }

		#endregion
	}
}
