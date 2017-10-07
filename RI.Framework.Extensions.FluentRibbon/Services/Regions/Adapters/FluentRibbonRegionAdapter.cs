using System;
using System.Collections.Generic;

using Fluent;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition.Model;




namespace RI.Framework.Services.Regions.Adapters
{
	/// <summary>
	///     Implements a region adapter which handles Fluent Ribbon controls.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The Fluent Ribbon controls which are supported by this region adapter are:
	///         <see cref="Ribbon" />, <see cref="RibbonTabItem" />.
	///         All types derived from those are also supported.
	///     </para>
	///     <para>
	///         See <see cref="IRegionAdapter" /> for more details.
	///     </para>
	/// </remarks>
	[Export]
	public sealed class FluentRibbonRegionAdapter : WpfRegionAdapterBase
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

			if ((container is RibbonTabItem) && (element is RibbonGroupBox))
			{
				((RibbonTabItem)container).Groups.Remove((RibbonGroupBox)element);
				((RibbonTabItem)container).Groups.Add((RibbonGroupBox)element);
			}
			else if ((container is Ribbon) && (element is RibbonTabItem))
			{
				((Ribbon)container).Tabs.Remove((RibbonTabItem)element);
				((Ribbon)container).Tabs.Add((RibbonTabItem)element);
			}
		}

		/// <inheritdoc />
		public override void Clear (object container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			if (container is RibbonTabItem)
			{
				((RibbonTabItem)container).Groups.Clear();
			}
			else if (container is Ribbon)
			{
				((Ribbon)container).Tabs.Clear();
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
			if ((container is RibbonTabItem) && (element is RibbonGroupBox))
			{
				contains = ((RibbonTabItem)container).Groups.Contains((RibbonGroupBox)element);
			}
			else if ((container is Ribbon) && (element is RibbonTabItem))
			{
				contains = ((Ribbon)container).Tabs.Contains((RibbonTabItem)element);
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
			if (container is RibbonTabItem)
			{
				return ((RibbonTabItem)container).Groups.Cast<object>().ToList();
			}
			else if (container is Ribbon)
			{
				return ((Ribbon)container).Tabs.Cast<object>().ToList();
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

			if ((container is RibbonTabItem) && (element is RibbonGroupBox))
			{
				((RibbonTabItem)container).Groups.Remove((RibbonGroupBox)element);
			}
			else if ((container is Ribbon) && (element is RibbonTabItem))
			{
				((Ribbon)container).Tabs.Remove((RibbonTabItem)element);
			}
		}

		/// <inheritdoc />
		protected override void GetSupportedTypes (List<Type> types)
		{
			types.Add(typeof(Ribbon));
			types.Add(typeof(RibbonTabItem));
		}

		#endregion
	}
}
