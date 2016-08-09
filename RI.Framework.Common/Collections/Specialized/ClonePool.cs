using System;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Collections.ObjectModel;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Collections.Specialized
{
	/// <summary>
	///     Implements a pool which creates items by cloning from a prototype.
	/// </summary>
	/// <typeparam name="T"> The type of objects which can be stored and recycled by the pool. </typeparam>
	/// <remarks>
	///     <para>
	///         The prototype can be any object which implements <see cref="ICloneable" /> or <see cref="ICloneable{T}" />.
	///         <see cref="ICloneable.Clone" /> is used to create new items from the prototype.
	///     </para>
	///     <para>
	///         The prototype itself is never used as an item or taken from the pool respectively, it is only used for cloning.
	///     </para>
	///     <para>
	///         This pool implementation supports <see cref="IPoolAware" />.
	///         See <see cref="PoolBase{T}" /> for more details.
	///     </para>
	/// </remarks>
	[SuppressMessage ("ReSharper", "CompareNonConstrainedGenericWithNull")]
	public sealed class ClonePool <T> : PoolBase<T>
			where T : ICloneable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ClonePool{T}" />.
		/// </summary>
		/// <param name="prototype"> The prototype object the items of this pool are cloned from. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="prototype" /> is null. </exception>
		public ClonePool (T prototype)
		{
			if (prototype == null)
			{
				throw new ArgumentNullException(nameof(prototype));
			}

			this.Initialize(prototype);
		}

		/// <summary>
		///     Creates a new instance of <see cref="ClonePool{T}" />.
		/// </summary>
		/// <param name="prototype"> The prototype object the items of this pool are cloned from. </param>
		/// <param name="count"> The amount of initial free items in the pool. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="prototype" /> is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="count" /> is less than zero. </exception>
		public ClonePool (T prototype, int count)
				: base(count)
		{
			if (prototype == null)
			{
				throw new ArgumentNullException(nameof(prototype));
			}

			if (count < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(count));
			}

			this.Initialize(prototype);

			this.Ensure(count);
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the original prototype from which all the items of this pool are cloned from.
		/// </summary>
		/// <value>
		///     The original prototype from which all the items of this pool are cloned from.
		/// </value>
		/// <remarks>
		///     <note type="important">
		///         Be careful when manipulating the prototype.
		///         Already cloned items are not affected, only items made after the manipulation.
		///     </note>
		/// </remarks>
		public T Prototype { get; private set; }

		private ICloneable GenericPrototype { get; set; }

		private ICloneable<T> TypeSpecificPrototype { get; set; }

		#endregion




		#region Instance Methods

		private void Initialize (T prototype)
		{
			this.Prototype = prototype;
			this.GenericPrototype = prototype;
			this.TypeSpecificPrototype = prototype as ICloneable<T>;
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected override T Create ()
		{
			if (this.TypeSpecificPrototype != null)
			{
				return this.TypeSpecificPrototype.Clone();
			}

			return (T)this.GenericPrototype.Clone();
		}

		#endregion
	}
}
