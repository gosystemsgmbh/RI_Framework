﻿using System;

using RI.Framework.Data.Repository.Entities;




namespace RI.Framework.Data.Repository
{
	/// <summary>
	///     Event arguments for the entity views event which resolves the currently used change tracking context to be used with <see cref="IEntityChangeTracking" />.
	/// </summary>
	[Serializable]
	public sealed class ChangeTrackingContextResolveEventArgs : EventArgs
	{
		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets the change tracking context.
		/// </summary>
		/// <value>
		///     The change tracking context or null if no change tracking context is available.
		/// </value>
		public object ChangeTrackingContext { get; set; }

		#endregion
	}
}
