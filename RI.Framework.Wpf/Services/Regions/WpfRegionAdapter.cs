using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using RI.Framework.Collections;
using RI.Framework.Collections.Comparison;
using RI.Framework.Composition.Model;




namespace RI.Framework.Services.Regions
{
	/// <summary>
	///     Implements a region adapter which handles common WPF controls.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The common WPF controls which are supported by this region adapter are:
	///         <see cref="ContentControl" />, <see cref="ItemsControl" />, <see cref="Panel" />.
	///         All types derived from those are also supported.
	///     </para>
	/// </remarks>
	[Export]
	public sealed class WpfRegionAdapter : RegionAdapterBase
	{
		#region Overrides

		/// <inheritdoc />
		public override void Activate (object container, object element)
		{
			base.Activate(container, element);

			if (element is FrameworkElement)
			{
				FrameworkElement frameworkElement = (FrameworkElement)element;
				if (frameworkElement.DataContext is IRegionElement)
				{
					IRegionElement regionElement = (IRegionElement)frameworkElement.DataContext;
					regionElement.Activated();
				}
			}
		}

		/// <inheritdoc />
		public override void Add (object container, object element)
		{
			if (container is ContentControl)
			{
				ContentControl contentControl = (ContentControl)container;
				if (contentControl.Content != null)
				{
					throw new InvalidOperationException();
				}
				contentControl.Content = element;
			}
			else if (container is ItemsControl)
			{
				ItemsControl itemsControl = (ItemsControl)container;
				itemsControl.Items.Add(element);
			}
			else if ((container is Panel) && (element is UIElement))
			{
				Panel panel = (Panel)container;
				panel.Children.Add((UIElement)element);
			}
		}

		/// <inheritdoc />
		public override void Clear (object container)
		{
			if (container is ContentControl)
			{
				ContentControl contentControl = (ContentControl)container;
				contentControl.Content = null;
			}
			else if (container is ItemsControl)
			{
				ItemsControl itemsControl = (ItemsControl)container;
				itemsControl.Items.Clear();
			}
			else if (container is Panel)
			{
				Panel panel = (Panel)container;
				panel.Children.Clear();
			}
		}

		/// <inheritdoc />
		public override void Deactivate (object container, object element)
		{
			base.Deactivate(container, element);

			if (element is FrameworkElement)
			{
				FrameworkElement frameworkElement = (FrameworkElement)element;
				if (frameworkElement.DataContext is IRegionElement)
				{
					IRegionElement regionElement = (IRegionElement)frameworkElement.DataContext;
					regionElement.Deactivated();
				}
			}
		}

		/// <inheritdoc />
		public override List<object> Get (object container)
		{
			List<object> elements = new List<object>();
			if (container is ContentControl)
			{
				ContentControl contentControl = (ContentControl)container;
				elements.Add(contentControl.Content);
			}
			else if (container is ItemsControl)
			{
				ItemsControl itemsControl = (ItemsControl)container;
				foreach (object element in itemsControl.Items)
				{
					elements.Add(element);
				}
			}
			else if (container is Panel)
			{
				Panel panel = (Panel)container;
				foreach (object element in panel.Children)
				{
					elements.Add(element);
				}
			}
			return elements;
		}

		/// <inheritdoc />
		public override void Remove (object container, object element)
		{
			if (container is ContentControl)
			{
				ContentControl contentControl = (ContentControl)container;
				if (object.Equals(contentControl.Content, element))
				{
					contentControl.Content = null;
				}
			}
			else if (container is ItemsControl)
			{
				ItemsControl itemsControl = (ItemsControl)container;
				itemsControl.Items.Remove(element);
			}
			else if ((container is Panel) && (element is UIElement))
			{
				Panel panel = (Panel)container;
				panel.Children.Remove((UIElement)element);
			}
		}

		/// <inheritdoc />
		public override void Sort (object container)
		{
			if (container is ItemsControl)
			{
				ItemsControl itemsControl = (ItemsControl)container;
				List<object> sortedElements = this.GetSortedElements(itemsControl.Items);
				List<object> existingElements = itemsControl.Items.ToList();
				if (!sortedElements.SequenceEqual(existingElements, CollectionComparerFlags.ReferenceEquality))
				{
					itemsControl.Items.Clear();
					foreach (object sortedElement in sortedElements)
					{
						itemsControl.Items.Add(sortedElement);
					}
				}
			}
			else if (container is Panel)
			{
				Panel panel = (Panel)container;
				List<object> sortedElements = this.GetSortedElements(panel.Children);
				List<object> existingElements = panel.Children.ToList();
				if (!sortedElements.SequenceEqual(existingElements, CollectionComparerFlags.ReferenceEquality))
				{
					panel.Children.Clear();
					foreach (object sortedElement in sortedElements)
					{
						panel.Children.Add((UIElement)sortedElement);
					}
				}
			}
		}

		/// <inheritdoc />
		protected override void GetSupportedTypes (List<Type> types)
		{
			types.Add(typeof(ContentControl));
			types.Add(typeof(ItemsControl));
			types.Add(typeof(Panel));
		}

		#endregion
	}
}
