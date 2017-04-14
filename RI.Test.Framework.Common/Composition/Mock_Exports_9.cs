using System;

using RI.Framework.Composition.Model;




namespace RI.Test.Framework.Composition
{
	[Export]
	public sealed class Mock_Exports_9
	{
		[ExportCreator]
		public static Mock_Exports_9 Create (Type type)
		{
			return new Mock_Exports_9
			{
				Value = "Test123"
			};
		}

		public string Value { get; private set; }
	}
}