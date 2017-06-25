namespace RI.Framework.Bus.Pipelines
{
	public class DefaultBusPipeline : IBusPipeline
	{
		#region Instance Methods

		public void DestroyNode (BusContext context, BusNode node)
		{
		}

		#endregion




		#region Interface: IBusPipeline

		public void InitializeNode (BusContext context, BusNode node)
		{
		}

		public void Start (BusContext context)
		{
		}

		public void Stop (BusContext context)
		{
		}

		#endregion
	}
}
