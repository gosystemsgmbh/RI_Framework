using System.Collections.Generic;




namespace RI.Framework.Data
{
	public interface IRepositoryErrors
	{
		IList<string> EntityErrors { get; }

		IDictionary<string, IList<string>> PropertyErrors { get; }
	}
}