using RI.Framework.Composition.Model;




namespace RI.Test.Framework.Composition
{
	[Export]
	public sealed class Mock_Exports_8
	{
		#region Instance Constructor/Destructor

		[ExportConstructor]
		public Mock_Exports_8 (Mock_Exports_7 value)
		{
			this.Value = value;
		}

		public Mock_Exports_8 (string value)
		{
		}

		#endregion




		#region Instance Properties/Indexer

		public Mock_Exports_7 Value { get; }

		#endregion
	}
}
