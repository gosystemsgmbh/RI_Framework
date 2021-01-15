using RI.Framework.Composition.Model;




namespace RI.Test.Framework.Composition
{
	[Export]
	public sealed class Mock_Exports_11
	{
		#region Instance Properties/Indexer

		[Import]
		public Mock_Exports_10 Test10 { get; set; }

		[Import]
		public Mock_Exports_11 Test11 { get; set; }

		#endregion
	}
}
