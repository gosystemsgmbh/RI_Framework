using System;




namespace RI.Framework.Data.Repository.Views
{
	/// <summary>
	///     Event arguments for entity view events related to a single entity.
	/// </summary>
	/// <typeparam name="T"> The type of the entity. </typeparam>
	[Serializable]
	public sealed class EntityViewItemEventArgs <T> : EventArgs
		where T : class
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="EntityViewItemEventArgs{T}" />.
		/// </summary>
		/// <param name="value"> The entity. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="value" /> is null. </exception>
		public EntityViewItemEventArgs (T value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			this.Value = value;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the entity.
		/// </summary>
		/// <value>
		///     The entity.
		/// </value>
		public T Value { get; }

		#endregion
	}
}
