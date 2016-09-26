using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;




namespace RI.Framework.Services.Regions
{
	/// <summary>
	///     Implements a region adapter for <see cref="Panel" />s.
	/// </summary>
	public sealed class PanelRegionAdapter : RegionAdapterBase<Panel>
	{
		#region Overrides

		/// <inheritdoc />
		protected override void Activate (Panel container, object element)
		{
		}

		/// <inheritdoc />
		protected override void Add (Panel container, object element)
		{
			if (!container.Children.Contains((UIElement)element))
			{
				container.Children.Add((UIElement)element);
			}
		}

		/// <inheritdoc />
		protected override void Clear (Panel container)
		{
			container.Children.Clear();
		}

		/// <inheritdoc />
		protected override void Deactivate (Panel container, object element)
		{
		}

		/// <inheritdoc />
		protected override object[] Get (Panel container)
		{
			List<object> elements = new List<object>();
			foreach (object element in container.Children)
			{
				elements.Add(element);
			}
			return elements.ToArray();
		}

		/// <inheritdoc />
		protected override void Remove (Panel container, object element)
		{
			container.Children.Remove((UIElement)element);
		}

		#endregion
	}
}
