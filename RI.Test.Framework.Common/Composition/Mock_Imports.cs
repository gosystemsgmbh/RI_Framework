using System;
using System.Collections.Generic;
using System.Text;

using RI.Framework.Composition.Model;




namespace RI.Test.Framework.Composition
{
	[Export]
	public sealed class Mock_Imports
	{
		[Import]
		public Mock_Exports_1 Import_1 { get; private set; }

		[Import(typeof(Mock_Exports_2))]
		public Import Import_2 { get; private set; }

		[Import("E3")]
		public object Import_3 { get; private set; }

		[Import("E4")]
		public Import Import_4 { get; private set; }

		[Import(typeof(Mock_Exports_5))]
		public Import Import_5 { get; private set; }

		[Import(typeof(Mock_Exports_6))]
		public Import Import_6 { get; private set; }

		[Import(typeof(Mock_Exports_1), Recomposable = false)]
		public Import Import_7 { get; private set; }

		[Import(typeof(Mock_Exports_1), Recomposable = true)]
		public Import Import_8 { get; private set; }
	}
}
