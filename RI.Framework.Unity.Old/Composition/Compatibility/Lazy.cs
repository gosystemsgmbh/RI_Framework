using System;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Composition.Compatibility
{
	/// <summary>
	///     Implements a placeholder for lazy resolving an import.
	/// </summary>
	/// <typeparam name="T"> The type being imported. </typeparam>
	public sealed class Lazy <T> : ISynchronizable
	{
		#region Instance Constructor/Destructor

		internal Lazy (Func<T> resolver)
		{
			if (resolver == null)
			{
				throw new ArgumentNullException(nameof(resolver));
			}

			this.SyncRoot = new object();

			this.Resolver = resolver;

			this.ValueCreated = false;
			this.ValueInternal = default(T);
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets whether the imported value is resolved or not.
		/// </summary>
		/// <value>
		///     true if the value is already resolved, false otherwise.
		/// </value>
		public bool IsValueCreated
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.ValueCreated;
				}
			}
		}

		/// <summary>
		///     Gets the imported value.
		/// </summary>
		/// <value>
		///     The imported value.
		/// </value>
		/// <remarks>
		///     <para>
		///         The value will be resolved the first time <see cref="Value" /> is used.
		///     </para>
		/// </remarks>
		public T Value
		{
			get
			{
				lock (this.SyncRoot)
				{
					if (!this.ValueCreated)
					{
						this.ValueInternal = this.Resolver();
						this.ValueCreated = true;
					}

					return this.ValueInternal;
				}
			}
		}

		private Func<T> Resolver { get; }

		private bool ValueCreated { get; set; }

		private T ValueInternal { get; set; }

		#endregion




		#region Interface: ISynchronizable

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; }

		#endregion
	}
}
