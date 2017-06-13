using RI.Framework.Bus.Connections;
using RI.Framework.Bus.Nodes;
using RI.Framework.Bus.Serialization;




namespace RI.Framework.Bus.Context
{
	public class BusContext
	{
		#region Instance Methods

		public void AddConnection (IBusConnection busConnection, params string[] busNames)
		{
		}

		public void AddSerializer (IBusSerializer busSerializer)
		{
		}

		public BusNode CreateNode ()
		{
			return null;
		}

		#endregion
	}
}
