using System.Windows.Controls;




namespace RI.Framework.Services.Regions
{
	/// <summary>
	///     Implements a region adapter for <see cref="ContentControl" />s.
	/// </summary>
	public sealed class ContentControlRegionAdapter : RegionAdapterBase<ContentControl>
	{
		#region Overrides

		/// <inheritdoc />
		protected override void Activate (ContentControl container, object element)
		{
		}

		/// <inheritdoc />
		protected override void Add (ContentControl container, object element)
		{
			container.Content = element;
		}

		/// <inheritdoc />
		protected override void Clear (ContentControl container)
		{
			container.Content = null;
		}

		/// <inheritdoc />
		protected override void Deactivate (ContentControl container, object element)
		{
		}

		/// <inheritdoc />
		protected override object[] Get (ContentControl container)
		{
			return new object[] {container.Content};
		}

		/// <inheritdoc />
		protected override void Remove (ContentControl container, object element)
		{
			if (object.ReferenceEquals(container.Content, element))
			{
				container.Content = null;
			}
		}

		#endregion
	}
}
