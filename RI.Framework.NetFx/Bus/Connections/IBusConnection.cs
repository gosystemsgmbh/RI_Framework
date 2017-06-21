namespace RI.Framework.Bus.Connections
{
	public interface IBusConnection
	{
		void Start (BusContext context);

		void Stop (BusContext context);
	}
}
