using System;




namespace RI.Framework.Data.Repository.Views
{
	public sealed class EntityViewItemEventArgs <T> : EventArgs
		where T : class
	{
		#region Instance Constructor/Destructor

		public EntityViewItemEventArgs (T value)
		{
			this.Value = value;
		}

		#endregion




		#region Instance Properties/Indexer

		public T Value { get; }

		#endregion
	}
}
