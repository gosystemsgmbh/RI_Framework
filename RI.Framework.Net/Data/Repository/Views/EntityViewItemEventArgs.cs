using System;




namespace RI.Framework.Data.Repository.Views
{
	public sealed class EntityViewItemEventArgs<T> : EventArgs
		where T : class
	{
		public EntityViewItemEventArgs(T value)
		{
			this.Value = value;
		}

		public T Value { get; }
	}
}