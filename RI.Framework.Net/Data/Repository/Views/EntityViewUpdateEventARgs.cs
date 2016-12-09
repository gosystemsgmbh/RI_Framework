using System;




namespace RI.Framework.Data.Repository.Views
{
	[Serializable]
	public sealed class EntityViewUpdateEventArgs : EventArgs
	{
		#region Instance Constructor/Destructor

		public EntityViewUpdateEventArgs (bool resetPageNumber)
		{
			this.ResetPageNumber = resetPageNumber;
		}

		#endregion




		#region Instance Properties/Indexer

		public bool ResetPageNumber { get; }

		#endregion
	}
}
