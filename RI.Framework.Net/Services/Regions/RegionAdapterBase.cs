using System;
using System.Collections.Generic;

using RI.Framework.Utilities.Reflection;




namespace RI.Framework.Services.Regions
{
	/// <summary>
	///     Implements a base class for region adapters.
	/// </summary>
	/// <typeparam name="T"> The type of the container which is handled by this region adapter. </typeparam>
	public abstract class RegionAdapterBase <T> : IRegionAdapter
		where T : class
	{
		#region Abstracts

		/// <inheritdoc cref="IRegionAdapter.Activate" />
		protected abstract void Activate (T container, object element);

		/// <inheritdoc cref="IRegionAdapter.Add" />
		protected abstract void Add (T container, object element);

		/// <inheritdoc cref="IRegionAdapter.Clear" />
		protected abstract void Clear (T container);

		/// <inheritdoc cref="IRegionAdapter.Deactivate" />
		protected abstract void Deactivate (T container, object element);

		/// <inheritdoc cref="IRegionAdapter.Get" />
		protected abstract object[] Get (T container);

		/// <inheritdoc cref="IRegionAdapter.Remove" />
		protected abstract void Remove (T container, object element);

		#endregion




		#region Virtuals

		/// <inheritdoc cref="IRegionAdapter.Contains" />
		protected virtual bool Contains (T container, object element)
		{
			object[] elements = this.Get(container);
			foreach (object currentElement in elements)
			{
				if (object.ReferenceEquals(currentElement, element))
				{
					return true;
				}
			}
			return false;
		}

		/// <inheritdoc cref="IRegionAdapter.IsCompatibleContainer" />
		protected virtual bool IsCompatibleContainer (Type type, out int inheritanceDepth)
		{
			List<Type> inheritance = type.GetInheritance(true);
			inheritance.Reverse();
			for (int i1 = 0; i1 < inheritance.Count; i1++)
			{
				if (type.Equals(inheritance[i1]))
				{
					inheritanceDepth = i1;
					return true;
				}
			}

			inheritanceDepth = -1;
			return false;
		}

		/// <inheritdoc cref="IRegionAdapter.Set" />
		protected virtual void Set (T container, object element)
		{
			this.Clear(container);
			this.Add(container, element);
		}

		#endregion




		#region Interface: IRegionAdapter

		/// <inheritdoc />
		void IRegionAdapter.Activate (object container, object element)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			this.Activate((T)container, element);
		}

		/// <inheritdoc />
		void IRegionAdapter.Add (object container, object element)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			this.Add((T)container, element);
		}

		/// <inheritdoc />
		void IRegionAdapter.Clear (object container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			this.Clear((T)container);
		}

		/// <inheritdoc />
		bool IRegionAdapter.Contains (object container, object element)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			return this.Contains((T)container, element);
		}

		/// <inheritdoc />
		void IRegionAdapter.Deactivate (object container, object element)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			this.Deactivate((T)container, element);
		}

		/// <inheritdoc />
		object[] IRegionAdapter.Get (object container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			return this.Get((T)container);
		}

		/// <inheritdoc />
		bool IRegionAdapter.IsCompatibleContainer (Type type, out int inheritanceDepth)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			return this.IsCompatibleContainer(type, out inheritanceDepth);
		}

		/// <inheritdoc />
		void IRegionAdapter.Remove (object container, object element)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			this.Remove((T)container, element);
		}

		/// <inheritdoc />
		void IRegionAdapter.Set (object container, object element)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			this.Set((T)container, element);
		}

		#endregion
	}
}
