using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities.Reflection;




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
	public sealed class WpfRegionAdapter : IRegionAdapter
	{
		#region Interface: IRegionAdapter

		/// <inheritdoc />
		public void Activate (object container, object element)
		{
		}

		/// <inheritdoc />
		public void Add (object container, object element)
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
		public void Clear (object container)
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
		public void Deactivate (object container, object element)
		{
		}

		/// <inheritdoc />
		public List<object> Get (object container)
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
		public bool IsCompatibleContainer (Type type, out int inheritanceDepth)
		{
			Type matchingType = null;
			return type.GetBestMatchingType(out matchingType, out inheritanceDepth, typeof(ContentControl), typeof(ItemsControl), typeof(Panel));
		}

		/// <inheritdoc />
		public void Remove (object container, object element)
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

		#endregion
	}
}
