using System;
using System.Collections;
using System.Collections.Generic;

using RI.Framework.Utilities.Reflection;




namespace RI.Framework.Services.Regions
{
	/// <summary>
	///     Implements a base class for region adapters which provides some commonly used base functionality.
	/// </summary>
	public abstract class RegionAdapterBase : IRegionAdapter
	{
		#region Abstracts

		/// <summary>
		///     Fills a list of types which are supported by this region adapter.
		/// </summary>
		/// <param name="types"> The list which is filled with the supported types by this region adapter. </param>
		protected abstract void GetSupportedTypes (List<Type> types);

		#endregion




		#region Virtuals

		/// <summary>
		///     Creates a list of elements where the elements are sorted according to their sort index.
		/// </summary>
		/// <param name="elements"> The sequence of elements to sort. </param>
		/// <returns>
		///     The list of sorted elements.
		///     An empty list is returned if the sequence contains no elements.
		/// </returns>
		/// <remarks>
		///     <para>
		///         <see cref="GetSortIndex" /> is used to sort the elements.
		///     </para>
		/// </remarks>
		protected virtual List<object> GetSortedElements (IEnumerable elements)
		{
			List<object> sorted = new List<object>();
			foreach (object element in elements)
			{
				sorted.Add(element);
			}
			sorted.Sort((x, y) => this.GetSortIndex(x).CompareTo(this.GetSortIndex(y)));
			return sorted;
		}

		/// <summary>
		///     Gets the sort index of an element.
		/// </summary>
		/// <param name="element"> The element. </param>
		/// <returns>
		///     The sort index of the element.
		///     <see cref="int.MaxValue" /> is returned if the element does not provide a sort index in any way.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The sort index of an element is retrieved using <see cref="IRegionElement" />.<see cref="IRegionElement.SortIndex" /> (higher priority) and <see cref="RegionElementSortHintAttribute" /> (lower priority).
		///     </para>
		/// </remarks>
		protected virtual int GetSortIndex (object element)
		{
			if (element is IRegionElement)
			{
				IRegionElement regionElement = (IRegionElement)element;
				if (regionElement.SortIndex.HasValue)
				{
					return regionElement.SortIndex.Value;
				}
			}

			object[] attributes = element.GetType().GetCustomAttributes(typeof(RegionElementSortHintAttribute), true);

			if (attributes.Length == 0)
			{
				return int.MaxValue;
			}

			return ((RegionElementSortHintAttribute)attributes[0]).Index;
		}

		#endregion




		#region Interface: IRegionAdapter

		/// <inheritdoc />
		public virtual void Activate (object container, object element)
		{
			if (element is IRegionElement)
			{
				IRegionElement regionElement = (IRegionElement)element;
				regionElement.Activated();
			}
		}

		/// <inheritdoc />
		public abstract void Add (object container, object element);

		/// <inheritdoc />
		public abstract void Clear (object container);

		/// <inheritdoc />
		public virtual void Deactivate (object container, object element)
		{
			if (element is IRegionElement)
			{
				IRegionElement regionElement = (IRegionElement)element;
				regionElement.Deactivated();
			}
		}

		/// <inheritdoc />
		public abstract List<object> Get (object container);

		/// <inheritdoc />
		public virtual bool IsCompatibleContainer (Type type, out int inheritanceDepth)
		{
			Type matchingType = null;

			List<Type> supportedTypes = new List<Type>();
			this.GetSupportedTypes(supportedTypes);

			return type.GetBestMatchingType(out matchingType, out inheritanceDepth, supportedTypes.ToArray());
		}

		/// <inheritdoc />
		public abstract void Remove (object container, object element);

		/// <inheritdoc />
		public abstract void Sort (object container);

		#endregion
	}
}
