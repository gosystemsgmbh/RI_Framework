using System;

using RI.Framework.Composition.Model;




namespace RI.Test.Framework.Composition
{
	[Export]
	public sealed class Mock_Exports_9
	{
		#region Static Methods

		[ExportCreator]
		public static Mock_Exports_9 Create (Type type)
		{
			return new Mock_Exports_9
			{
				Value = "Test123"
			};
		}

		#endregion




		#region Instance Properties/Indexer

		public string Value { get; private set; }

		#endregion
	}
}
