using System;
using System.Collections.Generic;

using RI.Framework.Utilities.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.IO;

namespace RI.Test.Framework.Utilities.Text
{
	[TestClass]
	public sealed class Test_IndentedTextWriter
	{
		[TestMethod]
		public void Write_Test ()
		{
			StringBuilder sb = new StringBuilder();
			using (StringWriter sw = new StringWriter(sb))
			{
				using (IndentedTextWriter iw = new IndentedTextWriter(sw))
				{
					iw.NewLine = "\n";
					iw.IndentString = "@=";
					iw.IndentEmptyLines = false;
					iw.IndentLevel = 0;

					iw.Flush();
					if (sb.ToString() != "")
					{
						throw new TestAssertionException();
					}

					//-------------------------
					// Empty lines not indented
					//-------------------------

					sb.Remove(0, sb.Length);
					iw.Reset();
					iw.IndentEmptyLines = false;
					iw.IndentLevel = 0;

					iw.Write("test1");
					iw.Flush();
					if (sb.ToString() != "test1")
					{
						throw new TestAssertionException();
					}

					iw.Write("\r\n");
					iw.Flush();
					if (sb.ToString() != "test1\n")
					{
						throw new TestAssertionException();
					}

					iw.Write("test2");
					iw.Flush();
					if (sb.ToString() != "test1\ntest2")
					{
						throw new TestAssertionException();
					}

					iw.Write("\n");
					iw.Flush();
					if (sb.ToString() != "test1\ntest2\n")
					{
						throw new TestAssertionException();
					}

					iw.Write(iw.NewLine);
					iw.Flush();
					if (sb.ToString() != "test1\ntest2\n\n")
					{
						throw new TestAssertionException();
					}

					sb.Remove(0, sb.Length);
					iw.Reset();
					iw.IndentEmptyLines = false;
					iw.IndentLevel = 1;

					iw.Write("test1");
					iw.Flush();
					if (sb.ToString() != "@=test1")
					{
						throw new TestAssertionException();
					}

					iw.Write("\r\n");
					iw.Flush();
					if (sb.ToString() != "@=test1\n")
					{
						throw new TestAssertionException();
					}

					iw.Write("test2");
					iw.Flush();
					if (sb.ToString() != "@=test1\n@=test2")
					{
						throw new TestAssertionException();
					}

					iw.Write("\n");
					iw.Flush();
					if (sb.ToString() != "@=test1\n@=test2\n")
					{
						throw new TestAssertionException();
					}

					iw.Write(iw.NewLine);
					iw.Flush();
					if (sb.ToString() != "@=test1\n@=test2\n\n")
					{
						throw new TestAssertionException();
					}

					sb.Remove(0, sb.Length);
					iw.Reset();
					iw.IndentEmptyLines = false;
					iw.IndentLevel = 2;

					iw.Write("test1");
					iw.Flush();
					if (sb.ToString() != "@=@=test1")
					{
						throw new TestAssertionException();
					}

					iw.Write("\r\n");
					iw.Flush();
					if (sb.ToString() != "@=@=test1\n")
					{
						throw new TestAssertionException();
					}

					iw.Write("test2");
					iw.Flush();
					if (sb.ToString() != "@=@=test1\n@=@=test2")
					{
						throw new TestAssertionException();
					}

					iw.Write("\n");
					iw.Flush();
					if (sb.ToString() != "@=@=test1\n@=@=test2\n")
					{
						throw new TestAssertionException();
					}

					iw.Write(iw.NewLine);
					iw.Flush();
					if (sb.ToString() != "@=@=test1\n@=@=test2\n\n")
					{
						throw new TestAssertionException();
					}

					//---------------------
					// Empty lines indented
					//---------------------

					sb.Remove(0, sb.Length);
					iw.Reset();
					iw.IndentEmptyLines = true;
					iw.IndentLevel = 0;

					iw.Write("test1");
					iw.Flush();
					if (sb.ToString() != "test1")
					{
						throw new TestAssertionException();
					}

					iw.Write("\r\n");
					iw.Flush();
					if (sb.ToString() != "test1\n")
					{
						throw new TestAssertionException();
					}

					iw.Write("test2");
					iw.Flush();
					if (sb.ToString() != "test1\ntest2")
					{
						throw new TestAssertionException();
					}

					iw.Write("\n");
					iw.Flush();
					if (sb.ToString() != "test1\ntest2\n")
					{
						throw new TestAssertionException();
					}

					iw.Write(iw.NewLine);
					iw.Flush();
					if (sb.ToString() != "test1\ntest2\n\n")
					{
						throw new TestAssertionException();
					}

					sb.Remove(0, sb.Length);
					iw.Reset();
					iw.IndentEmptyLines = true;
					iw.IndentLevel = 1;

					iw.Write("test1");
					iw.Flush();
					if (sb.ToString() != "@=test1")
					{
						throw new TestAssertionException();
					}

					iw.Write("\r\n");
					iw.Flush();
					if (sb.ToString() != "@=test1\n")
					{
						throw new TestAssertionException();
					}

					iw.Write("test2");
					iw.Flush();
					if (sb.ToString() != "@=test1\n@=test2")
					{
						throw new TestAssertionException();
					}

					iw.Write("\n");
					iw.Flush();
					if (sb.ToString() != "@=test1\n@=test2\n")
					{
						throw new TestAssertionException();
					}

					iw.Write(iw.NewLine);
					iw.Flush();
					if (sb.ToString() != "@=test1\n@=test2\n@=\n")
					{
						throw new TestAssertionException();
					}

					sb.Remove(0, sb.Length);
					iw.Reset();
					iw.IndentEmptyLines = true;
					iw.IndentLevel = 2;

					iw.Write("test1");
					iw.Flush();
					if (sb.ToString() != "@=@=test1")
					{
						throw new TestAssertionException();
					}

					iw.Write("\r\n");
					iw.Flush();
					if (sb.ToString() != "@=@=test1\n")
					{
						throw new TestAssertionException();
					}

					iw.Write("test2");
					iw.Flush();
					if (sb.ToString() != "@=@=test1\n@=@=test2")
					{
						throw new TestAssertionException();
					}

					iw.Write("\n");
					iw.Flush();
					if (sb.ToString() != "@=@=test1\n@=@=test2\n")
					{
						throw new TestAssertionException();
					}

					iw.Write(iw.NewLine);
					iw.Flush();
					if (sb.ToString() != "@=@=test1\n@=@=test2\n@=@=\n")
					{
						throw new TestAssertionException();
					}

					//--------------
					// Special cases
					//--------------

					sb.Remove(0, sb.Length);
					iw.Reset();
					iw.IndentEmptyLines = false;
					iw.IndentLevel = 1;

					iw.IndentString = " ";
					iw.NewLine = "@";

					iw.Write("test1");
					iw.Flush();
					if (sb.ToString() != " test1")
					{
						throw new TestAssertionException();
					}

					iw.Write("\r\n");
					iw.Flush();
					if (sb.ToString() != " test1@")
					{
						throw new TestAssertionException();
					}

					iw.Write("test2");
					iw.Flush();
					if (sb.ToString() != " test1@ test2")
					{
						throw new TestAssertionException();
					}

					iw.Write("\n");
					iw.Flush();
					if (sb.ToString() != " test1@ test2@")
					{
						throw new TestAssertionException();
					}

					iw.Write(iw.NewLine);
					iw.Flush();
					if (sb.ToString() != " test1@ test2@@")
					{
						throw new TestAssertionException();
					}

					sb.Remove(0, sb.Length);
					iw.Reset();
					iw.IndentEmptyLines = false;
					iw.IndentLevel = 1;

					iw.IndentString = null;
					iw.NewLine = "@";

					iw.Write("test1");
					iw.Flush();
					if (sb.ToString() != "test1")
					{
						throw new TestAssertionException();
					}

					iw.Write("\r\n");
					iw.Flush();
					if (sb.ToString() != "test1@")
					{
						throw new TestAssertionException();
					}

					iw.Write("test2");
					iw.Flush();
					if (sb.ToString() != "test1@test2")
					{
						throw new TestAssertionException();
					}

					iw.Write("\n");
					iw.Flush();
					if (sb.ToString() != "test1@test2@")
					{
						throw new TestAssertionException();
					}

					iw.Write(iw.NewLine);
					iw.Flush();
					if (sb.ToString() != "test1@test2@@")
					{
						throw new TestAssertionException();
					}

					iw.Close();
					iw.Close();
					iw.Dispose();
					iw.Dispose();

					try
					{
						iw.Write("");
						throw new TestAssertionException();
					}
					catch (InvalidOperationException)
					{
					}
				}
			}
		}

		[TestMethod]
		public void WriteLine_Test()
		{
			StringBuilder sb = new StringBuilder();
			using (StringWriter sw = new StringWriter(sb))
			{
				using (IndentedTextWriter iw = new IndentedTextWriter(sw))
				{
					iw.NewLine = "\n";
					iw.IndentString = "@=";
					iw.IndentEmptyLines = false;
					iw.IndentLevel = 0;

					iw.Flush();
					if (sb.ToString() != "")
					{
						throw new TestAssertionException();
					}

					//-------------------------
					// Empty lines not indented
					//-------------------------

					sb.Remove(0, sb.Length);
					iw.Reset();
					iw.IndentEmptyLines = false;
					iw.IndentLevel = 0;

					iw.WriteLine("test1");
					iw.Flush();
					if (sb.ToString() != "test1")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("\r\n");
					iw.Flush();
					if (sb.ToString() != "test1\n")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("test2");
					iw.Flush();
					if (sb.ToString() != "test1\ntest2")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("\n");
					iw.Flush();
					if (sb.ToString() != "test1\ntest2\n")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine(iw.NewLine);
					iw.Flush();
					if (sb.ToString() != "test1\ntest2\n\n")
					{
						throw new TestAssertionException();
					}

					sb.Remove(0, sb.Length);
					iw.Reset();
					iw.IndentEmptyLines = false;
					iw.IndentLevel = 1;

					iw.WriteLine("test1");
					iw.Flush();
					if (sb.ToString() != "@=test1")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("\r\n");
					iw.Flush();
					if (sb.ToString() != "@=test1\n")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("test2");
					iw.Flush();
					if (sb.ToString() != "@=test1\n@=test2")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("\n");
					iw.Flush();
					if (sb.ToString() != "@=test1\n@=test2\n")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine(iw.NewLine);
					iw.Flush();
					if (sb.ToString() != "@=test1\n@=test2\n\n")
					{
						throw new TestAssertionException();
					}

					sb.Remove(0, sb.Length);
					iw.Reset();
					iw.IndentEmptyLines = false;
					iw.IndentLevel = 2;

					iw.WriteLine("test1");
					iw.Flush();
					if (sb.ToString() != "@=@=test1")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("\r\n");
					iw.Flush();
					if (sb.ToString() != "@=@=test1\n")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("test2");
					iw.Flush();
					if (sb.ToString() != "@=@=test1\n@=@=test2")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("\n");
					iw.Flush();
					if (sb.ToString() != "@=@=test1\n@=@=test2\n")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine(iw.NewLine);
					iw.Flush();
					if (sb.ToString() != "@=@=test1\n@=@=test2\n\n")
					{
						throw new TestAssertionException();
					}

					//---------------------
					// Empty lines indented
					//---------------------

					sb.Remove(0, sb.Length);
					iw.Reset();
					iw.IndentEmptyLines = true;
					iw.IndentLevel = 0;

					iw.WriteLine("test1");
					iw.Flush();
					if (sb.ToString() != "test1")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("\r\n");
					iw.Flush();
					if (sb.ToString() != "test1\n")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("test2");
					iw.Flush();
					if (sb.ToString() != "test1\ntest2")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("\n");
					iw.Flush();
					if (sb.ToString() != "test1\ntest2\n")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine(iw.NewLine);
					iw.Flush();
					if (sb.ToString() != "test1\ntest2\n\n")
					{
						throw new TestAssertionException();
					}

					sb.Remove(0, sb.Length);
					iw.Reset();
					iw.IndentEmptyLines = true;
					iw.IndentLevel = 1;

					iw.WriteLine("test1");
					iw.Flush();
					if (sb.ToString() != "@=test1")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("\r\n");
					iw.Flush();
					if (sb.ToString() != "@=test1\n")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("test2");
					iw.Flush();
					if (sb.ToString() != "@=test1\n@=test2")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("\n");
					iw.Flush();
					if (sb.ToString() != "@=test1\n@=test2\n")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine(iw.NewLine);
					iw.Flush();
					if (sb.ToString() != "@=test1\n@=test2\n@=\n")
					{
						throw new TestAssertionException();
					}

					sb.Remove(0, sb.Length);
					iw.Reset();
					iw.IndentEmptyLines = true;
					iw.IndentLevel = 2;

					iw.WriteLine("test1");
					iw.Flush();
					if (sb.ToString() != "@=@=test1")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("\r\n");
					iw.Flush();
					if (sb.ToString() != "@=@=test1\n")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("test2");
					iw.Flush();
					if (sb.ToString() != "@=@=test1\n@=@=test2")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("\n");
					iw.Flush();
					if (sb.ToString() != "@=@=test1\n@=@=test2\n")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine(iw.NewLine);
					iw.Flush();
					if (sb.ToString() != "@=@=test1\n@=@=test2\n@=@=\n")
					{
						throw new TestAssertionException();
					}

					//--------------
					// Special cases
					//--------------

					sb.Remove(0, sb.Length);
					iw.Reset();
					iw.IndentEmptyLines = false;
					iw.IndentLevel = 1;

					iw.IndentString = " ";
					iw.NewLine = "@";

					iw.WriteLine("test1");
					iw.Flush();
					if (sb.ToString() != " test1")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("\r\n");
					iw.Flush();
					if (sb.ToString() != " test1@")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("test2");
					iw.Flush();
					if (sb.ToString() != " test1@ test2")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("\n");
					iw.Flush();
					if (sb.ToString() != " test1@ test2@")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine(iw.NewLine);
					iw.Flush();
					if (sb.ToString() != " test1@ test2@@")
					{
						throw new TestAssertionException();
					}

					sb.Remove(0, sb.Length);
					iw.Reset();
					iw.IndentEmptyLines = false;
					iw.IndentLevel = 1;

					iw.IndentString = null;
					iw.NewLine = "@";

					iw.WriteLine("test1");
					iw.Flush();
					if (sb.ToString() != "test1")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("\r\n");
					iw.Flush();
					if (sb.ToString() != "test1@")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("test2");
					iw.Flush();
					if (sb.ToString() != "test1@test2")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine("\n");
					iw.Flush();
					if (sb.ToString() != "test1@test2@")
					{
						throw new TestAssertionException();
					}

					iw.WriteLine(iw.NewLine);
					iw.Flush();
					if (sb.ToString() != "test1@test2@@")
					{
						throw new TestAssertionException();
					}

					iw.Close();
					iw.Close();
					iw.Dispose();
					iw.Dispose();

					try
					{
						iw.WriteLine("");
						throw new TestAssertionException();
					}
					catch (InvalidOperationException)
					{
					}
				}
			}
		}
	}
}
