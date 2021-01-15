﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using RI.Framework.Composition.Model;




namespace RI.Framework.Services.Regions.Adapters
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
    ///     <para>
    ///         See <see cref="IRegionAdapter" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
    public sealed class WpfRegionAdapter : WpfRegionAdapterBase
    {
        #region Overrides

        /// <inheritdoc />
        public override void Add (object container, object element)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (container is ContentControl)
            {
                ContentControl contentControl = (ContentControl)container;
                if (contentControl.Content != null)
                {
                    throw new NotSupportedException("The container of type" + container.GetType().Name + " already has an element and does not support multiple elements.");
                }

                contentControl.Content = element;
            }
            else if (container is ItemsControl)
            {
                ItemsControl itemsControl = (ItemsControl)container;
                itemsControl.Items.Remove(element);
                itemsControl.Items.Add(element);
            }
            else if ((container is Panel) && (element is UIElement))
            {
                Panel panel = (Panel)container;
                panel.Children.Remove((UIElement)element);
                panel.Children.Add((UIElement)element);
            }
        }

        /// <inheritdoc />
        public override void Clear (object container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

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
        public override bool Contains (object container, object element)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            bool contains = false;
            if (container is ContentControl)
            {
                ContentControl contentControl = (ContentControl)container;
                contains = object.ReferenceEquals(contentControl.Content, element);
            }
            else if (container is ItemsControl)
            {
                ItemsControl itemsControl = (ItemsControl)container;
                contains = itemsControl.Items.Contains(element);
            }
            else if ((container is Panel) && (element is UIElement))
            {
                Panel panel = (Panel)container;
                contains = panel.Children.Contains((UIElement)element);
            }

            return contains;
        }

        /// <inheritdoc />
        public override List<object> Get (object container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

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
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (container is ContentControl)
            {
                ContentControl contentControl = (ContentControl)container;
                if (object.ReferenceEquals(contentControl.Content, element))
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
        protected override void GetSupportedTypes (List<Type> types)
        {
            types.Add(typeof(ContentControl));
            types.Add(typeof(ItemsControl));
            types.Add(typeof(Panel));
        }

        #endregion
    }
}
