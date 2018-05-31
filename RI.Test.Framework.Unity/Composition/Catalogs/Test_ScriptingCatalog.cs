using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.ComponentModel;




namespace RI.Test.Framework.Composition.Catalogs
{
	public sealed class Test_ScriptingCatalog : TestModule
	{
		#region Instance Methods

		[TestMethod]
		public void Test ()
		{
			object customExport = ServiceLocator.GetInstance("Mock_Export");

			if (customExport == null)
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
