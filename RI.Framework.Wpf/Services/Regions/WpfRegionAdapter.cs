using System;
using System.Collections.Generic;




namespace RI.Framework.Services.Regions
{
	public sealed class WpfRegionAdapter : IRegionAdapter
	{
		#region Instance Methods

		public bool Contains (object container, object element)
		{
			throw new NotImplementedException();
		}

		public void Set (object container, object element)
		{
			throw new NotImplementedException();
		}

		#endregion




		#region Interface: IRegionAdapter

		public void Activate (object container, object element)
		{
			throw new NotImplementedException();
		}

		public void Add (object container, object element)
		{
			throw new NotImplementedException();
		}

		public void Clear (object container)
		{
			throw new NotImplementedException();
		}

		public void Deactivate (object container, object element)
		{
			throw new NotImplementedException();
		}

		public List<object> Get (object container)
		{
			throw new NotImplementedException();
		}

		public bool IsCompatibleContainer (Type type, out int inheritanceDepth)
		{
			throw new NotImplementedException();
		}

		public void Remove (object container, object element)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
