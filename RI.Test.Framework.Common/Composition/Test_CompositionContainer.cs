using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Collections.Comparison;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition;
using RI.Framework.Composition.Catalogs;
using RI.Framework.Composition.Model;




namespace RI.Test.Framework.Composition
{
	[TestClass]
	public sealed class Test_CompositionContainer
	{
		[TestMethod]
		public void ModelExportImport_Test()
		{
			CompositionContainer test = new CompositionContainer();
			test.AddCatalog(new AssemblyCatalog(Assembly.GetExecutingAssembly()));

			Mock_Imports imports1 = new Mock_Imports();
			test.ResolveImports(imports1, CompositionFlags.Normal);

			if (imports1.Import_1 == null)
			{
				throw new TestAssertionException();
			}

			if (imports1.Import_2.ToList<Mock_Exports_2>().Count != 1)
			{
				throw new TestAssertionException();
			}
			if (imports1.Import_2.ToArray<Mock_Exports_2>().Length != 1)
			{
				throw new TestAssertionException();
			}
			if (imports1.Import_2.Value<Mock_Exports_2>() == null)
			{
				throw new TestAssertionException();
			}
			if (imports1.Import_2.Values<Mock_Exports_2>().Count() != 1)
			{
				throw new TestAssertionException();
			}

			if ((!(imports1.Import_3 is Mock_Exports_3A)) && (!(imports1.Import_3 is Mock_Exports_3A)))
			{
				throw new TestAssertionException();
			}

			if (imports1.Import_4.ToList<object>().Count != 2)
			{
				throw new TestAssertionException();
			}
			if (imports1.Import_4.ToArray<object>().Length != 2)
			{
				throw new TestAssertionException();
			}
			if (imports1.Import_4.Value<Mock_Exports_4A>() == null)
			{
				throw new TestAssertionException();
			}
			if (imports1.Import_4.Value<Mock_Exports_4B>() == null)
			{
				throw new TestAssertionException();
			}
			if (imports1.Import_4.Values<Mock_Exports_4A>().Count() != 1)
			{
				throw new TestAssertionException();
			}
			if (imports1.Import_4.Values<Mock_Exports_4B>().Count() != 1)
			{
				throw new TestAssertionException();
			}

			Mock_Imports imports2 = test.GetExport<Mock_Imports>();

			if (imports2.Import_1 == null)
			{
				throw new TestAssertionException();
			}

			if (imports2.Import_2.ToList<Mock_Exports_2>().Count != 1)
			{
				throw new TestAssertionException();
			}
			if (imports2.Import_2.ToArray<Mock_Exports_2>().Length != 1)
			{
				throw new TestAssertionException();
			}
			if (imports2.Import_2.Value<Mock_Exports_2>() == null)
			{
				throw new TestAssertionException();
			}
			if (imports2.Import_2.Values<Mock_Exports_2>().Count() != 1)
			{
				throw new TestAssertionException();
			}

			if ((!(imports2.Import_3 is Mock_Exports_3A)) && (!(imports2.Import_3 is Mock_Exports_3A)))
			{
				throw new TestAssertionException();
			}

			if (imports2.Import_4.ToList<object>().Count != 2)
			{
				throw new TestAssertionException();
			}
			if (imports2.Import_4.ToArray<object>().Length != 2)
			{
				throw new TestAssertionException();
			}
			if (imports2.Import_4.Value<Mock_Exports_4A>() == null)
			{
				throw new TestAssertionException();
			}
			if (imports2.Import_4.Value<Mock_Exports_4B>() == null)
			{
				throw new TestAssertionException();
			}
			if (imports2.Import_4.Values<Mock_Exports_4A>().Count() != 1)
			{
				throw new TestAssertionException();
			}
			if (imports2.Import_4.Values<Mock_Exports_4B>().Count() != 1)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ManualExportImport_Test()
		{
			CompositionContainer test = new CompositionContainer();

			test.AddExport(typeof(Mock_Exports_1), typeof(Mock_Exports_1), false);
			test.AddExport(new Mock_Exports_2(), typeof(Mock_Exports_2));
			test.AddExport(typeof(Mock_Exports_3A), "E3", false);
			test.AddExport(new Mock_Exports_3B(), "E3");
			test.AddExport(typeof(Mock_Exports_4A), "E4", false);
			test.AddExport(new Mock_Exports_4B(), "E4");

			if (test.GetExports<object>().Count != 0)
			{
				throw new TestAssertionException();
			}
			if (test.GetExports<object>(typeof(object)).Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test.GetExport<Mock_Exports_1>() == null)
			{
				throw new TestAssertionException();
			}
			if (test.GetExport<object>(typeof(Mock_Exports_1)) == null)
			{
				throw new TestAssertionException();
			}
			if (test.GetExports<Mock_Exports_1>().Count != 1)
			{
				throw new TestAssertionException();
			}
			if (test.GetExports<object>(typeof(Mock_Exports_1)).Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.GetExport<Mock_Exports_2>() == null)
			{
				throw new TestAssertionException();
			}
			if (test.GetExport<object>(typeof(Mock_Exports_2)) == null)
			{
				throw new TestAssertionException();
			}
			if (test.GetExports<Mock_Exports_2>().Count != 1)
			{
				throw new TestAssertionException();
			}
			if (test.GetExports<object>(typeof(Mock_Exports_2)).Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.GetExport<object>("E3") == null)
			{
				throw new TestAssertionException();
			}
			if (test.GetExports<object>("E3").Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test.GetExport<object>("E4") == null)
			{
				throw new TestAssertionException();
			}
			if (test.GetExports<object>("E4").Count != 2)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Inheritance_Test()
		{
			CompositionContainer test = new CompositionContainer();
			test.AddCatalog(new AssemblyCatalog(Assembly.GetExecutingAssembly()));

			Mock_Imports imports = test.GetExport<Mock_Imports>();

			if (imports.Import_5.Values<Mock_Exports_5>().Count() != 0)
			{
				throw new TestAssertionException();
			}

			if (imports.Import_6.Values<Mock_Exports_6>().Count() != 2)
			{
				throw new TestAssertionException();
			}
			if (imports.Import_6.Value<Mock_Exports_6A>() == null)
			{
				throw new TestAssertionException();
			}
			if (imports.Import_6.Value<Mock_Exports_6B>() == null)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void SharedPrivate_Test()
		{
			CompositionContainer test = new CompositionContainer();

			test.AddExport(typeof(Mock_Exports_1), typeof(Mock_Exports_1), false);
			test.AddExport(typeof(Mock_Exports_2), typeof(Mock_Exports_2), true);

			if (!object.ReferenceEquals(test.GetExport<Mock_Exports_1>(), test.GetExport<Mock_Exports_1>()))
			{
				throw new TestAssertionException();
			}

			if (object.ReferenceEquals(test.GetExport<Mock_Exports_2>(), test.GetExport<Mock_Exports_2>()))
			{
				throw new TestAssertionException();
			}

			Mock_Exports_1 instance1 = new Mock_Exports_1();
			Mock_Exports_2 instance2 = new Mock_Exports_2();

			test.AddExport(instance1, typeof(Mock_Exports_1));
			test.AddExport(instance2, typeof(Mock_Exports_2));

			if (object.ReferenceEquals(test.GetExports<Mock_Exports_1>()[0], test.GetExports<Mock_Exports_1>()[1]))
			{
				throw new TestAssertionException();
			}

			if (object.ReferenceEquals(test.GetExports<Mock_Exports_2>()[0], test.GetExports<Mock_Exports_2>()[1]))
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ConstructorCreator_Test()
		{
			CompositionContainer test = new CompositionContainer();
			test.AddCatalog(new AssemblyCatalog(Assembly.GetExecutingAssembly()));

			Mock_Exports_7 m7 = test.GetExport<Mock_Exports_7>();
			if (m7 == null)
			{
				throw new TestAssertionException();
			}

			Mock_Exports_8 m8 = test.GetExport<Mock_Exports_8>();
			if (m8 == null)
			{
				throw new TestAssertionException();
			}
			if (!object.ReferenceEquals(m8.Value, m7))
			{
				throw new TestAssertionException();
			}

			Mock_Exports_9 m9 = test.GetExport<Mock_Exports_9>();
			if (m9 == null)
			{
				throw new TestAssertionException();
			}
			if (m9.Value != "Test123")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Recomposition_Test ()
		{
			CompositionContainer test = new CompositionContainer();
			test.AddExport(typeof(Mock_Imports), typeof(Mock_Imports), false);
			Mock_Imports imports = test.GetExport<Mock_Imports>();

			if (imports.Import_7.Values<Mock_Exports_1>().Count() != 0)
			{
				throw new TestAssertionException();
			}
			if (imports.Import_8.Values<Mock_Exports_1>().Count() != 0)
			{
				throw new TestAssertionException();
			}

			test.AddExport(typeof(Mock_Exports_1), typeof(Mock_Exports_1), false);

			if (imports.Import_7.Values<Mock_Exports_1>().Count() != 1)
			{
				throw new TestAssertionException();
			}
			if (imports.Import_8.Values<Mock_Exports_1>().Count() != 1)
			{
				throw new TestAssertionException();
			}

			test.AddExport(new Mock_Exports_1(), typeof(Mock_Exports_1));

			if (imports.Import_7.Values<Mock_Exports_1>().Count() != 1)
			{
				throw new TestAssertionException();
			}
			if (imports.Import_8.Values<Mock_Exports_1>().Count() != 2)
			{
				throw new TestAssertionException();
			}

			test.Recompose(CompositionFlags.All);

			if (imports.Import_7.Values<Mock_Exports_1>().Count() != 2)
			{
				throw new TestAssertionException();
			}
			if (imports.Import_8.Values<Mock_Exports_1>().Count() != 2)
			{
				throw new TestAssertionException();
			}

			test.RemoveExport(typeof(Mock_Exports_1), typeof(Mock_Exports_1));

			if (imports.Import_7.Values<Mock_Exports_1>().Count() != 2)
			{
				throw new TestAssertionException();
			}
			if (imports.Import_8.Values<Mock_Exports_1>().Count() != 1)
			{
				throw new TestAssertionException();
			}
		}
	}
}
