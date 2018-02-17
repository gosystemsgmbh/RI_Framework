using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Collections.Comparison;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.IO.CSV;
using RI.Framework.Utilities;



namespace RI.Test.Framework.IO.CSV
{
	[TestClass]
	public class Test_CsvDocument
	{
		private const string NoQuotes_NoBreaks_NoEmpties = "A;BB;CCC\r\nD;EE;FFF";

		private const string NoQuotes_NoBreaks_WithEmpties = ";A;;BB;;CCC;;\r\n;D;;EE;;FFF;;";

		private const string NoQuotes_WithBreaks_NoEmpties = "A;\"B\nB\";CCC\r\nD;\"E\nE\";FFF";

		private const string NoQuotes_WithBreaks_WithEmpties = ";A;;\"B\nB\";;CCC;;\r\n;D;;\"E\nE\";;FFF;;";

		private const string WithQuotes_NoBreaks_NoEmpties = "\"A\";\"BB\";\"CCC\"\r\n\"D\";\"EE\";\"FFF\"";

		private const string WithQuotes_NoBreaks_WithEmpties = "\"\";\"A\";\"\";\"BB\";\"\";\"CCC\";\"\";\"\"\r\n\"\";\"D\";\"\";\"EE\";\"\";\"FFF\";\"\";\"\"";

		private const string WithQuotes_WithBreaks_NoEmpties = "\"A\";\"B\nB\";\"CCC\"\r\n\"D\";\"E\nE\";\"FFF\"";

		private const string WithQuotes_WithBreaks_WithEmpties = "\"\";\"A\";\"\";\"B\nB\";\"\";\"CCC\";\"\";\r\n\"\";\"D\";\"\";\"E\nE\";\"\";\"FFF\";\"\";\"\"";

		//TODO: #12: Other Tests: Different value separators
		//TODO: #12: Other Tests: Different line breaks
		//TODO: #12: Other Tests: With/without empty lines (start, in the middle, at the end)

		private void Verify_Value (CsvDocument doc, int row, int column, string expected)
		{
			string value = doc.Data[row][column];
			if (!string.Equals(value, expected, StringComparison.Ordinal))
			{
				throw new TestAssertionException("(" + row + "," + column + ") should be \"" + expected + "\" but is \"" + value + "\" inside \"" + doc.AsString() + "\".");
			}
		}

		private void Verify_NoQuotes_NoBreaks_NoEmpties (CsvDocument doc)
		{
			this.Verify_Value(doc, 0, 0, "A");
			this.Verify_Value(doc, 0, 1, "BB");
			this.Verify_Value(doc, 0, 2, "CCC");

			this.Verify_Value(doc, 1, 0, "D");
			this.Verify_Value(doc, 1, 1, "EE");
			this.Verify_Value(doc, 1, 2, "FFF");
		}

		private void Verify_NoQuotes_NoBreaks_WithEmpties (CsvDocument doc)
		{
			this.Verify_Value(doc, 0, 0, "");
			this.Verify_Value(doc, 0, 1, "A");
			this.Verify_Value(doc, 0, 2, "");
			this.Verify_Value(doc, 0, 3, "BB");
			this.Verify_Value(doc, 0, 4, "");
			this.Verify_Value(doc, 0, 5, "CCC");
			this.Verify_Value(doc, 0, 6, "");
			this.Verify_Value(doc, 0, 7, "");

			this.Verify_Value(doc, 1, 0, "");
			this.Verify_Value(doc, 1, 1, "D");
			this.Verify_Value(doc, 1, 2, "");
			this.Verify_Value(doc, 1, 3, "EE");
			this.Verify_Value(doc, 1, 4, "");
			this.Verify_Value(doc, 1, 5, "FFF");
			this.Verify_Value(doc, 1, 6, "");
			this.Verify_Value(doc, 1, 7, "");
		}

		private void Verify_NoQuotes_WithBreaks_NoEmpties (CsvDocument doc)
		{
			this.Verify_Value(doc, 0, 0, "A");
			this.Verify_Value(doc, 0, 1, "B\r\nB");
			this.Verify_Value(doc, 0, 2, "CCC");

			this.Verify_Value(doc, 1, 0, "D");
			this.Verify_Value(doc, 1, 1, "E\r\nE");
			this.Verify_Value(doc, 1, 2, "FFF");
		}

		private void Verify_NoQuotes_WithBreaks_WithEmpties (CsvDocument doc)
		{
			this.Verify_Value(doc, 0, 0, "");
			this.Verify_Value(doc, 0, 1, "A");
			this.Verify_Value(doc, 0, 2, "");
			this.Verify_Value(doc, 0, 3, "B\r\nB");
			this.Verify_Value(doc, 0, 4, "");
			this.Verify_Value(doc, 0, 5, "CCC");
			this.Verify_Value(doc, 0, 6, "");
			this.Verify_Value(doc, 0, 7, "");

			this.Verify_Value(doc, 1, 0, "");
			this.Verify_Value(doc, 1, 1, "D");
			this.Verify_Value(doc, 1, 2, "");
			this.Verify_Value(doc, 1, 3, "E\r\nE");
			this.Verify_Value(doc, 1, 4, "");
			this.Verify_Value(doc, 1, 5, "FFF");
			this.Verify_Value(doc, 1, 6, "");
			this.Verify_Value(doc, 1, 7, "");
		}

		private void Verify_WithQuotes_NoBreaks_NoEmpties (CsvDocument doc)
		{
			this.Verify_Value(doc, 0, 0, "A");
			this.Verify_Value(doc, 0, 1, "BB");
			this.Verify_Value(doc, 0, 2, "CCC");

			this.Verify_Value(doc, 1, 0, "D");
			this.Verify_Value(doc, 1, 1, "EE");
			this.Verify_Value(doc, 1, 2, "FFF");
		}

		private void Verify_WithQuotes_NoBreaks_WithEmpties (CsvDocument doc)
		{
			this.Verify_Value(doc, 0, 0, "");
			this.Verify_Value(doc, 0, 1, "A");
			this.Verify_Value(doc, 0, 2, "");
			this.Verify_Value(doc, 0, 3, "BB");
			this.Verify_Value(doc, 0, 4, "");
			this.Verify_Value(doc, 0, 5, "CCC");
			this.Verify_Value(doc, 0, 6, "");
			this.Verify_Value(doc, 0, 7, "");

			this.Verify_Value(doc, 1, 0, "");
			this.Verify_Value(doc, 1, 1, "D");
			this.Verify_Value(doc, 1, 2, "");
			this.Verify_Value(doc, 1, 3, "EE");
			this.Verify_Value(doc, 1, 4, "");
			this.Verify_Value(doc, 1, 5, "FFF");
			this.Verify_Value(doc, 1, 6, "");
			this.Verify_Value(doc, 1, 7, "");
		}

		private void Verify_WithQuotes_WithBreaks_NoEmpties (CsvDocument doc)
		{
			this.Verify_Value(doc, 0, 0, "A");
			this.Verify_Value(doc, 0, 1, "B\r\nB");
			this.Verify_Value(doc, 0, 2, "CCC");

			this.Verify_Value(doc, 1, 0, "D");
			this.Verify_Value(doc, 1, 1, "E\r\nE");
			this.Verify_Value(doc, 1, 2, "FFF");
		}

		private void Verify_WithQuotes_WithBreaks_WithEmpties (CsvDocument doc)
		{
			this.Verify_Value(doc, 0, 0, "");
			this.Verify_Value(doc, 0, 1, "A");
			this.Verify_Value(doc, 0, 2, "");
			this.Verify_Value(doc, 0, 3, "B\r\nB");
			this.Verify_Value(doc, 0, 4, "");
			this.Verify_Value(doc, 0, 5, "CCC");
			this.Verify_Value(doc, 0, 6, "");
			this.Verify_Value(doc, 0, 7, "");

			this.Verify_Value(doc, 1, 0, "");
			this.Verify_Value(doc, 1, 1, "D");
			this.Verify_Value(doc, 1, 2, "");
			this.Verify_Value(doc, 1, 3, "E\r\nE");
			this.Verify_Value(doc, 1, 4, "");
			this.Verify_Value(doc, 1, 5, "FFF");
			this.Verify_Value(doc, 1, 6, "");
			this.Verify_Value(doc, 1, 7, "");
		}

		[TestMethod]
		public void Values_Test ()
		{
			CsvDocument doc = new CsvDocument();

			doc.Load(Test_CsvDocument.NoQuotes_NoBreaks_NoEmpties);
			this.Verify_NoQuotes_NoBreaks_NoEmpties(doc);

			doc.Load(Test_CsvDocument.NoQuotes_NoBreaks_WithEmpties);
			this.Verify_NoQuotes_NoBreaks_WithEmpties(doc);

			doc.Load(Test_CsvDocument.NoQuotes_WithBreaks_NoEmpties);
			this.Verify_NoQuotes_WithBreaks_NoEmpties(doc);

			doc.Load(Test_CsvDocument.NoQuotes_WithBreaks_WithEmpties);
			this.Verify_NoQuotes_WithBreaks_WithEmpties(doc);

			doc.Load(Test_CsvDocument.WithQuotes_NoBreaks_NoEmpties);
			this.Verify_WithQuotes_NoBreaks_NoEmpties(doc);

			doc.Load(Test_CsvDocument.WithQuotes_NoBreaks_WithEmpties);
			this.Verify_WithQuotes_NoBreaks_WithEmpties(doc);

			doc.Load(Test_CsvDocument.WithQuotes_WithBreaks_NoEmpties);
			this.Verify_WithQuotes_WithBreaks_NoEmpties(doc);

			doc.Load(Test_CsvDocument.WithQuotes_WithBreaks_WithEmpties);
			this.Verify_WithQuotes_WithBreaks_WithEmpties(doc);
		}

		[TestMethod]
		public void LoadSave_Test()
		{
			//TODO: #12: Implement tests
		}
	}
}
