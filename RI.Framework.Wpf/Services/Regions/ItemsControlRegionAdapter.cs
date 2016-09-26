using System.Collections.Generic;
using System.Windows.Controls;




namespace RI.Framework.Services.Regions
{
	/// <summary>
	///     Implements a region adapter for <see cref="ItemsControl" />s.
	/// </summary>
	public sealed class ItemsControlRegionAdapter : RegionAdapterBase<ItemsControl>
	{
		#region Overrides

		/// <inheritdoc />
		protected override void Activate (ItemsControl container, object element)
		{
		}

		/// <inheritdoc />
		protected override void Add (ItemsControl container, object element)
		{
			if (!container.Items.Contains(element))
			{
				container.Items.Add(element);
			}
		}

		/// <inheritdoc />
		protected override void Clear (ItemsControl container)
		{
			container.Items.Clear();
		}

		/// <inheritdoc />
		protected override void Deactivate (ItemsControl container, object element)
		{
		}

		/// <inheritdoc />
		protected override object[] Get (ItemsControl container)
		{
			List<object> elements = new List<object>();
			foreach (object element in container.Items)
			{
				elements.Add(element);
			}
			return elements.ToArray();
		}

		/// <inheritdoc />
		protected override void Remove (ItemsControl container, object element)
		{
			container.Items.Remove(element);
		}

		#endregion
	}
}
