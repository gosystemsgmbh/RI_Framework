using System;




namespace RI.Framework.Data.Repository.Views
{
	public sealed class EntityViewUpdateEventArgs : EventArgs
	{
		public EntityViewUpdateEventArgs (bool resetPageNumber)
		{
			this.ResetPageNumber = resetPageNumber;
		}

		public bool ResetPageNumber { get; }
	}
}