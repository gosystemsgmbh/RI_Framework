using System;

using RI.Framework.Composition.Model;




namespace RI.Framework.Data.EF.Configuration
{
	[Export]
	public interface IEntityConfiguration
	{
		Type EntityType { get; }
	}
}
