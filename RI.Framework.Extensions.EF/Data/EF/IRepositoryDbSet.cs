using System;




namespace RI.Framework.Data.EF
{
	public interface IRepositoryDbSet
	{
		Type EntityType { get; }
	}
}
