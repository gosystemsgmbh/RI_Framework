namespace RI.Framework.Bus.Pipelines
{
	public interface IBusPipeline
	{
		void InitializeNode (BusContext context, BusNode node);
		void Start (BusContext context);

		void Stop (BusContext context);
	}
}
