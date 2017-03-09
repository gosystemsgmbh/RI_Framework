using System;
using System.IO;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.IO.INI;
using RI.Framework.IO.INI.Elements;
using RI.Framework.IO.Paths;




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

		private const string Data5 = " ";

		#endregion




		#region Instance Methods

		[TestMethod]
		public void LoadSave_Test ()
		{
			IniDocument test = new IniDocument();
			FilePath file = FilePath.GetTempFile();

			//---------------------
			// Load from INI reader
			//---------------------

			using (StringReader sr = new StringReader(Test_IniDocument.Data1))
			{
				using (IniReader ir = new IniReader(sr))
				{
					test.Load(ir);
				}
			}
			this.VerifyData1(test);

			using (StringReader sr = new StringReader(Test_IniDocument.Data1))
			{
				using (IniReader ir = new IniReader(sr, new IniReaderSettings()))
				{
					test.Load(ir);
				}
			}
			this.VerifyData1(test);

			//-----------------
			// Load from string
			//-----------------

			test.Load(Test_IniDocument.Data1);
			this.VerifyData1(test);

			test.Load(Test_IniDocument.Data2);
			this.VerifyData2(test);

			test.Load(Test_IniDocument.Data3);
			this.VerifyData3(test);

			test.Load(Test_IniDocument.Data1, new IniReaderSettings());
			this.VerifyData1(test);

			test.Load(Test_IniDocument.Data2, new IniReaderSettings());
			this.VerifyData2(test);

			test.Load(Test_IniDocument.Data3, new IniReaderSettings());
			this.VerifyData3(test);

			//------------------
			// Loading from file
			//------------------

			file.WriteText(Test_IniDocument.Data1, Encoding.UTF8);
			test.Load(file, Encoding.UTF8);
			this.VerifyData1(test);

			file.WriteText(Test_IniDocument.Data2, Encoding.UTF8);
			test.Load(file, Encoding.UTF8);
			this.VerifyData2(test);

			file.WriteText(Test_IniDocument.Data3, Encoding.UTF8);
			test.Load(file, Encoding.UTF8);
			this.VerifyData3(test);

			file.WriteText(Test_IniDocument.Data1, Encoding.UTF8);
			test.Load(file, Encoding.UTF8, new IniReaderSettings());
			this.VerifyData1(test);

			file.WriteText(Test_IniDocument.Data2, Encoding.UTF8);
			test.Load(file, Encoding.UTF8, new IniReaderSettings());
			this.VerifyData2(test);

			file.WriteText(Test_IniDocument.Data3, Encoding.UTF8);
			test.Load(file, Encoding.UTF8, new IniReaderSettings());
			this.VerifyData3(test);

			//---------------------
			// Saving to INI writer
			//---------------------

			test.Load(Test_IniDocument.Data1);
			using (StringWriter sw = new StringWriter())
			{
				using (IniWriter iw = new IniWriter(sw))
				{
					test.Save(iw);
				}
				string temp = sw.ToString();
				if (temp.Replace("\r", string.Empty) != Test_IniDocument.Data1)
				{
					throw new TestAssertionException();
				}
			}

			test.Load(Test_IniDocument.Data1);
			using (StringWriter sw = new StringWriter())
			{
				using (IniWriter iw = new IniWriter(sw, new IniWriterSettings()))
				{
					test.Save(iw);
				}
				string temp = sw.ToString();
				if (temp.Replace("\r", string.Empty) != Test_IniDocument.Data1)
				{
					throw new TestAssertionException();
				}
			}

			//-----------------
			// Saving to string
			//-----------------

			test.Load(Test_IniDocument.Data1);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data1)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data2);
			if ((test.AsString().Replace("\r", string.Empty) + "\n") != Test_IniDocument.Data2)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data3);
			if ((test.AsString().Replace("\r", string.Empty) + "\n") != Test_IniDocument.Data3)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data1);
			if (test.AsString(new IniWriterSettings()).Replace("\r", string.Empty) != Test_IniDocument.Data1)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data2);
			if ((test.AsString(new IniWriterSettings()).Replace("\r", string.Empty) + "\n") != Test_IniDocument.Data2)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data3);
			if ((test.AsString(new IniWriterSettings()).Replace("\r", string.Empty) + "\n") != Test_IniDocument.Data3)
			{
				throw new TestAssertionException();
			}

			//---------------
			// Saving to file
			//---------------

			test.Load(Test_IniDocument.Data1);
			test.Save(file, Encoding.UTF8);
			test.Load(file, Encoding.UTF8);
			this.VerifyData1(test);

			test.Load(Test_IniDocument.Data2);
			test.Save(file, Encoding.UTF8);
			test.Load(file, Encoding.UTF8);
			this.VerifyData2(test);

			test.Load(Test_IniDocument.Data3 + "\n");
			test.Save(file, Encoding.UTF8);
			test.Load(file, Encoding.UTF8);
			this.VerifyData3(test);

			test.Load(Test_IniDocument.Data1);
			test.Save(file, Encoding.UTF8, new IniWriterSettings());
			test.Load(file, Encoding.UTF8);
			this.VerifyData1(test);

			test.Load(Test_IniDocument.Data2);
			test.Save(file, Encoding.UTF8, new IniWriterSettings());
			test.Load(file, Encoding.UTF8);
			this.VerifyData2(test);

			test.Load(Test_IniDocument.Data3 + "\n");
			test.Save(file, Encoding.UTF8, new IniWriterSettings());
			test.Load(file, Encoding.UTF8);
			this.VerifyData3(test);
		}

		[TestMethod]
		public void Elements_Test ()
		{
			IniDocument test = new IniDocument();

			test.Load(Test_IniDocument.Data4);
			this.VerifyData4(test);

			test.Load(Test_IniDocument.Data5);
			this.VerifyData5(test);

			test.Clear();
			if (test.Count != 0)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Settings_Test()
		{
			//Test: IniReaderSettings
			//Test: IniWriterSettings

			IniDocument test = new IniDocument();

			test.Clear();
			if (test.Count != 0)
			{
				throw new TestAssertionException();
			}
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

		private void VerifyData4(IniDocument document)
		{
			if (document.Count != 0)
			{
				throw new TestAssertionException();
			}
		}

		private void VerifyData5(IniDocument document)
		{
			if (document.Count != 1)
			{
				throw new TestAssertionException();
			}

			if (((TextIniElement)document.Elements[0]).Text != " ")
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
