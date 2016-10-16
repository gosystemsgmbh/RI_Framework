using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.IO.INI;
using RI.Framework.IO.INI.Elements;




namespace RI.Test.Framework.IO.INI
{
	[TestClass]
	public class Test_IniDocument
	{
		#region Constants

		private const string Data1 = "\nText\n;Comment\nName=Value\n[Header1]\n\nText1\n;Comment1\nName1=Value1";

		private const string Data2 = "\nText\n;Comment\nName=Value\n[Header1]\n\nText1\n;Comment1\nName1=Value1\n";

		private const string Data3 = "\nText\n;Comment\nName=Value\n[Header1]\n\nText1\n;Comment1\nName1=Value1\n\n";

		private const string Data4 = "";

		#endregion




		#region Instance Methods

		[TestMethod]
		public void Load_Test ()
		{
			IniDocument test = new IniDocument();

			using (StringReader sr = new StringReader(Test_IniDocument.Data1))
			{
				using (IniReader ir = new IniReader(sr))
				{
					test.Load(ir);
				}
			}
			this.VerifyData1(test);

			test.Clear();

			test.Load(Test_IniDocument.Data1);
			this.VerifyData1(test);
		}

		private void VerifyData1 (IniDocument document)
		{
			if (document.Count != 7)
			{
				throw new TestAssertionException();
			}

			if (((TextIniElement)document.Elements[0]).Text != Environment.NewLine + "Text")
			{
				throw new TestAssertionException();
			}

			if (((CommentIniElement)document.Elements[1]).Comment != "Comment")
			{
				throw new TestAssertionException();
			}

			if (((ValueIniElement)document.Elements[2]).Name != "Name")
			{
				throw new TestAssertionException();
			}

			if (((ValueIniElement)document.Elements[2]).Value != "Value")
			{
				throw new TestAssertionException();
			}

			if (((SectionIniElement)document.Elements[3]).SectionName != "Header1")
			{
				throw new TestAssertionException();
			}

			if (((TextIniElement)document.Elements[4]).Text != Environment.NewLine + "Text1")
			{
				throw new TestAssertionException();
			}

			if (((CommentIniElement)document.Elements[5]).Comment != "Comment1")
			{
				throw new TestAssertionException();
			}

			if (((ValueIniElement)document.Elements[6]).Name != "Name1")
			{
				throw new TestAssertionException();
			}

			if (((ValueIniElement)document.Elements[6]).Value != "Value1")
			{
				throw new TestAssertionException();
			}
		}

		private void VerifyData2 (IniDocument document)
		{
			if (document.Count != 7)
			{
				throw new TestAssertionException();
			}

			if (((TextIniElement)document.Elements[0]).Text != Environment.NewLine + "Text")
			{
				throw new TestAssertionException();
			}

			if (((CommentIniElement)document.Elements[1]).Comment != "Comment")
			{
				throw new TestAssertionException();
			}

			if (((ValueIniElement)document.Elements[2]).Name != "Name")
			{
				throw new TestAssertionException();
			}

			if (((ValueIniElement)document.Elements[2]).Value != "Value")
			{
				throw new TestAssertionException();
			}

			if (((SectionIniElement)document.Elements[3]).SectionName != "Header1")
			{
				throw new TestAssertionException();
			}

			if (((TextIniElement)document.Elements[4]).Text != Environment.NewLine + "Text1")
			{
				throw new TestAssertionException();
			}

			if (((CommentIniElement)document.Elements[5]).Comment != "Comment1")
			{
				throw new TestAssertionException();
			}

			if (((ValueIniElement)document.Elements[6]).Name != "Name1")
			{
				throw new TestAssertionException();
			}

			if (((ValueIniElement)document.Elements[6]).Value != "Value1")
			{
				throw new TestAssertionException();
			}
		}

		private void VerifyData3 (IniDocument document)
		{
			if (document.Count != 8)
			{
				throw new TestAssertionException();
			}

			if (((TextIniElement)document.Elements[0]).Text != Environment.NewLine + "Text")
			{
				throw new TestAssertionException();
			}

			if (((CommentIniElement)document.Elements[1]).Comment != "Comment")
			{
				throw new TestAssertionException();
			}

			if (((ValueIniElement)document.Elements[2]).Name != "Name")
			{
				throw new TestAssertionException();
			}

			if (((ValueIniElement)document.Elements[2]).Value != "Value")
			{
				throw new TestAssertionException();
			}

			if (((SectionIniElement)document.Elements[3]).SectionName != "Header1")
			{
				throw new TestAssertionException();
			}

			if (((TextIniElement)document.Elements[4]).Text != Environment.NewLine + "Text1")
			{
				throw new TestAssertionException();
			}

			if (((CommentIniElement)document.Elements[5]).Comment != "Comment1")
			{
				throw new TestAssertionException();
			}

			if (((ValueIniElement)document.Elements[6]).Name != "Name1")
			{
				throw new TestAssertionException();
			}

			if (((ValueIniElement)document.Elements[6]).Value != "Value1")
			{
				throw new TestAssertionException();
			}

			if (((TextIniElement)document.Elements[7]).Text != "")
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
