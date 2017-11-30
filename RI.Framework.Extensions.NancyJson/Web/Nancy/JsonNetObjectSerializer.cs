using Nancy;

using Newtonsoft.Json;




namespace RI.Framework.Web.Nancy
{
	/// <summary>
	///     Implements an object serializer which uses JSON.
	/// </summary>
	public sealed class JsonNetObjectSerializer : IObjectSerializer
	{
		#region Interface: IObjectSerializer

		/// <inheritdoc />
		public object Deserialize (string sourceString)
		{
			try
			{
				//Deserialize with default settings
				return JsonConvert.DeserializeObject(sourceString);
			}
			catch
			{
				//The source string to deserialize is considered of "uncontrolled" origin (e.g. the clients web browser) and therefore we can either successfully deserialize, or we can't... but we do not want crashes from garbage sent by the browser (or an attacker...)
				return null;
			}
		}

		/// <inheritdoc />
		public string Serialize (object sourceObject)
		{
			//Serialize with default settings
			return JsonConvert.SerializeObject(sourceObject);
		}

		#endregion
	}
}
