using System;
using System.Collections.Generic;




namespace RI.Framework.Data.Repository.Views
{
	public sealed class EntityViewItemsEventArgs<T>
		where T : class
	{
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

		public IList<T> OldItems { get; }

		public IList<T> NewItems { get; }
	}
}