namespace RI.Framework.Bus.Pipelines
{
	public interface IBusPipeline
	{
		void Start (BusContext context);

		void Stop (BusContext context);

		void InitializeNode (BusContext context, BusNode node);
	}
}
