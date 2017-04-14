using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Utilities;




namespace RI.Test.Framework.Utilities
{
	[TestClass]
	public sealed class Test_StringExtensions
	{
		#region Instance Methods

		[TestMethod]
		public void Contains_Test ()
		{
			if (string.Empty.Contains("Test2", StringComparison.Ordinal))
			{
				throw new TestAssertionException();
			}

			if (string.Empty.Contains("Test2", StringComparison.OrdinalIgnoreCase))
			{
				throw new TestAssertionException();
			}

			if (!"Test1".Contains(string.Empty, StringComparison.Ordinal))
			{
				throw new TestAssertionException();
			}

			if (!"Test1".Contains(string.Empty, StringComparison.OrdinalIgnoreCase))
			{
				throw new TestAssertionException();
			}

			if ("Test1".Contains("Test2", StringComparison.Ordinal))
			{
				throw new TestAssertionException();
			}

			if ("Test1".Contains("Test2", StringComparison.OrdinalIgnoreCase))
			{
				throw new TestAssertionException();
			}

			if (!"Test1".Contains("Test1", StringComparison.Ordinal))
			{
				throw new TestAssertionException();
			}

			if (!"Test1".Contains("Test1", StringComparison.OrdinalIgnoreCase))
			{
				throw new TestAssertionException();
			}

			if (!"Test1".Contains("Test", StringComparison.Ordinal))
			{
				throw new TestAssertionException();
			}

			if (!"Test1".Contains("Test", StringComparison.OrdinalIgnoreCase))
			{
				throw new TestAssertionException();
			}

			if (!"Test1".Contains("T", StringComparison.Ordinal))
			{
				throw new TestAssertionException();
			}

			if (!"Test1".Contains("T", StringComparison.OrdinalIgnoreCase))
			{
				throw new TestAssertionException();
			}

			if ("Test1".Contains("test", StringComparison.Ordinal))
			{
				throw new TestAssertionException();
			}

			if (!"Test1".Contains("test", StringComparison.OrdinalIgnoreCase))
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ContainsWhitespace_Test ()
		{
			if ("".ContainsWhitespace())
			{
				throw new TestAssertionException();
			}

			if ("a".ContainsWhitespace())
			{
				throw new TestAssertionException();
			}

			if ("abc".ContainsWhitespace())
			{
				throw new TestAssertionException();
			}

			if (!" ".ContainsWhitespace())
			{
				throw new TestAssertionException();
			}

			if (!"\r".ContainsWhitespace())
			{
				throw new TestAssertionException();
			}

			if (!"\n".ContainsWhitespace())
			{
				throw new TestAssertionException();
			}

			if (!"\t".ContainsWhitespace())
			{
				throw new TestAssertionException();
			}

			if (!" abcd ".ContainsWhitespace())
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void DoubleOccurrence_Test ()
		{
			if (string.Empty.DoubleOccurrence('A') != string.Empty)
			{
				throw new TestAssertionException();
			}

			if ("ABC".DoubleOccurrence('A') != "AABC")
			{
				throw new TestAssertionException();
			}

			if ("AABC".DoubleOccurrence('A') != "AAAABC")
			{
				throw new TestAssertionException();
			}

			if ("BCD".DoubleOccurrence('A') != "BCD")
			{
				throw new TestAssertionException();
			}

			if ("A".DoubleOccurrence('A') != "AA")
			{
				throw new TestAssertionException();
			}

			if ("AA".DoubleOccurrence('A') != "AAAA")
			{
				throw new TestAssertionException();
			}

			if ("AAA".DoubleOccurrence('A') != "AAAAAA")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void EndsWithCount_Test ()
		{
			if (string.Empty.EndsWithCount('A', StringComparison.Ordinal) != 0)
			{
				throw new TestAssertionException();
			}

			if (string.Empty.EndsWithCount('A', StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new TestAssertionException();
			}

			if ("BCD".EndsWithCount('A', StringComparison.Ordinal) != 0)
			{
				throw new TestAssertionException();
			}

			if ("BCD".EndsWithCount('A', StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new TestAssertionException();
			}

			if ("ABC".EndsWithCount('A', StringComparison.Ordinal) != 0)
			{
				throw new TestAssertionException();
			}

			if ("ABC".EndsWithCount('A', StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new TestAssertionException();
			}

			if ("AABC".EndsWithCount('A', StringComparison.Ordinal) != 0)
			{
				throw new TestAssertionException();
			}

			if ("AABC".EndsWithCount('A', StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new TestAssertionException();
			}

			if ("CBA".EndsWithCount('A', StringComparison.Ordinal) != 1)
			{
				throw new TestAssertionException();
			}

			if ("CBA".EndsWithCount('A', StringComparison.OrdinalIgnoreCase) != 1)
			{
				throw new TestAssertionException();
			}

			if ("CBAA".EndsWithCount('A', StringComparison.Ordinal) != 2)
			{
				throw new TestAssertionException();
			}

			if ("CBAA".EndsWithCount('A', StringComparison.OrdinalIgnoreCase) != 2)
			{
				throw new TestAssertionException();
			}

			if ("A".EndsWithCount('A', StringComparison.Ordinal) != 1)
			{
				throw new TestAssertionException();
			}

			if ("A".EndsWithCount('A', StringComparison.OrdinalIgnoreCase) != 1)
			{
				throw new TestAssertionException();
			}

			if ("AA".EndsWithCount('A', StringComparison.Ordinal) != 2)
			{
				throw new TestAssertionException();
			}

			if ("AA".EndsWithCount('A', StringComparison.OrdinalIgnoreCase) != 2)
			{
				throw new TestAssertionException();
			}

			if ("AAA".EndsWithCount('A', StringComparison.Ordinal) != 3)
			{
				throw new TestAssertionException();
			}

			if ("AAA".EndsWithCount('A', StringComparison.OrdinalIgnoreCase) != 3)
			{
				throw new TestAssertionException();
			}

			if ("AA".EndsWithCount('a', StringComparison.Ordinal) != 0)
			{
				throw new TestAssertionException();
			}

			if ("AA".EndsWithCount('a', StringComparison.OrdinalIgnoreCase) != 2)
			{
				throw new TestAssertionException();
			}

			if ("abc@@".EndsWithCount("@@", StringComparison.OrdinalIgnoreCase) != 1)
			{
				throw new TestAssertionException();
			}

			if ("abc@@@".EndsWithCount("@@", StringComparison.OrdinalIgnoreCase) != 1)
			{
				throw new TestAssertionException();
			}

			if ("abc@@@@".EndsWithCount("@@", StringComparison.OrdinalIgnoreCase) != 2)
			{
				throw new TestAssertionException();
			}

			if ("@@abc".EndsWithCount("@@", StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new TestAssertionException();
			}

			if ("@@@abc".EndsWithCount("@@", StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new TestAssertionException();
			}

			if ("@@@@abc".EndsWithCount("@@", StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Escape_Test ()
		{
			if (string.Empty.Escape() != string.Empty)
			{
				throw new TestAssertionException();
			}

			if ("ABC".Escape() != "ABC")
			{
				throw new TestAssertionException();
			}

			if ("\a\b\f\n\r\t\v\\\'\"".Escape() != "\\a\\b\\f\\n\\r\\t\\v\\\\\\\'\\\"")
			{
				throw new TestAssertionException();
			}

			if ("\\".Escape() != "\\\\")
			{
				throw new TestAssertionException();
			}

			if ("\\\\".Escape() != "\\\\\\\\")
			{
				throw new TestAssertionException();
			}

			if ("\\\\\\".Escape() != "\\\\\\\\\\\\")
			{
				throw new TestAssertionException();
			}

			if ("AB\\CD".Escape() != "AB\\\\CD")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void HalveOccurrence_Test ()
		{
			if (string.Empty.HalveOccurrence('A') != string.Empty)
			{
				throw new TestAssertionException();
			}

			if ("AABC".HalveOccurrence('A') != "ABC")
			{
				throw new TestAssertionException();
			}

			if ("ABC".HalveOccurrence('A') != "BC")
			{
				throw new TestAssertionException();
			}

			if ("BCD".HalveOccurrence('A') != "BCD")
			{
				throw new TestAssertionException();
			}

			if ("A".HalveOccurrence('A') != string.Empty)
			{
				throw new TestAssertionException();
			}

			if ("AA".HalveOccurrence('A') != "A")
			{
				throw new TestAssertionException();
			}

			if ("AAA".HalveOccurrence('A') != "A")
			{
				throw new TestAssertionException();
			}

			if ("AAAA".HalveOccurrence('A') != "AA")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void IsEmpty_Test ()
		{
			if (!string.Empty.IsEmpty())
			{
				throw new TestAssertionException();
			}

			if (!"".IsEmpty())
			{
				throw new TestAssertionException();
			}

			if (" ".IsEmpty())
			{
				throw new TestAssertionException();
			}

			if ("  ".IsEmpty())
			{
				throw new TestAssertionException();
			}

			if ("\f\r\n\t\v".IsEmpty())
			{
				throw new TestAssertionException();
			}

			if ("A".IsEmpty())
			{
				throw new TestAssertionException();
			}

			if ("ABC".IsEmpty())
			{
				throw new TestAssertionException();
			}

			if (" A ".IsEmpty())
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void IsEmptyOrWhitespace_Test ()
		{
			if (!string.Empty.IsEmptyOrWhitespace())
			{
				throw new TestAssertionException();
			}

			if (!"".IsEmptyOrWhitespace())
			{
				throw new TestAssertionException();
			}

			if (!" ".IsEmptyOrWhitespace())
			{
				throw new TestAssertionException();
			}

			if (!"  ".IsEmptyOrWhitespace())
			{
				throw new TestAssertionException();
			}

			if (!"\f\r\n\t\v".IsEmptyOrWhitespace())
			{
				throw new TestAssertionException();
			}

			if ("A".IsEmptyOrWhitespace())
			{
				throw new TestAssertionException();
			}

			if ("ABC".IsEmptyOrWhitespace())
			{
				throw new TestAssertionException();
			}

			if (" A ".IsEmptyOrWhitespace())
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void IsNull_Test ()
		{
			if (!((string)null).IsNull())
			{
				throw new TestAssertionException();
			}

			if (string.Empty.IsNull())
			{
				throw new TestAssertionException();
			}

			if (" ".IsNull())
			{
				throw new TestAssertionException();
			}

			if ("test".IsNull())
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void IsNullOrEmpty_Test ()
		{
			if (!((string)null).IsNullOrEmpty())
			{
				throw new TestAssertionException();
			}

			if (!string.Empty.IsNullOrEmpty())
			{
				throw new TestAssertionException();
			}

			if (!"".IsNullOrEmpty())
			{
				throw new TestAssertionException();
			}

			if (" ".IsNullOrEmpty())
			{
				throw new TestAssertionException();
			}

			if ("  ".IsNullOrEmpty())
			{
				throw new TestAssertionException();
			}

			if ("\f\r\n\t\v".IsNullOrEmpty())
			{
				throw new TestAssertionException();
			}

			if ("A".IsNullOrEmpty())
			{
				throw new TestAssertionException();
			}

			if ("ABC".IsNullOrEmpty())
			{
				throw new TestAssertionException();
			}

			if (" A ".IsEmptyOrWhitespace())
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void IsNullOrEmptyOrWhitespace_Test ()
		{
			if (!((string)null).IsNullOrEmptyOrWhitespace())
			{
				throw new TestAssertionException();
			}

			if (!string.Empty.IsNullOrEmptyOrWhitespace())
			{
				throw new TestAssertionException();
			}

			if (!"".IsNullOrEmptyOrWhitespace())
			{
				throw new TestAssertionException();
			}

			if (!" ".IsNullOrEmptyOrWhitespace())
			{
				throw new TestAssertionException();
			}

			if (!"  ".IsNullOrEmptyOrWhitespace())
			{
				throw new TestAssertionException();
			}

			if (!"\f\r\n\t\v".IsNullOrEmptyOrWhitespace())
			{
				throw new TestAssertionException();
			}

			if ("A".IsNullOrEmptyOrWhitespace())
			{
				throw new TestAssertionException();
			}

			if ("ABC".IsNullOrEmptyOrWhitespace())
			{
				throw new TestAssertionException();
			}

			if (" A ".IsNullOrEmptyOrWhitespace())
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void IsNullOrWhitespace_Test ()
		{
			if (!((string)null).IsNullOrWhitespaces())
			{
				throw new TestAssertionException();
			}

			if (string.Empty.IsNullOrWhitespaces())
			{
				throw new TestAssertionException();
			}

			if ("".IsNullOrWhitespaces())
			{
				throw new TestAssertionException();
			}

			if (!" ".IsNullOrWhitespaces())
			{
				throw new TestAssertionException();
			}

			if (!"  ".IsNullOrWhitespaces())
			{
				throw new TestAssertionException();
			}

			if (!"\f\r\n\t\v".IsNullOrWhitespaces())
			{
				throw new TestAssertionException();
			}

			if ("A".IsNullOrWhitespaces())
			{
				throw new TestAssertionException();
			}

			if ("ABC".IsNullOrWhitespaces())
			{
				throw new TestAssertionException();
			}

			if (" A ".IsNullOrWhitespaces())
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void IsWhitespace_Test ()
		{
			if (string.Empty.IsWhitespace())
			{
				throw new TestAssertionException();
			}

			if ("".IsWhitespace())
			{
				throw new TestAssertionException();
			}

			if (!" ".IsWhitespace())
			{
				throw new TestAssertionException();
			}

			if (!"  ".IsWhitespace())
			{
				throw new TestAssertionException();
			}

			if (!"\f\r\n\t\v".IsWhitespace())
			{
				throw new TestAssertionException();
			}

			if ("A".IsWhitespace())
			{
				throw new TestAssertionException();
			}

			if ("ABC".IsWhitespace())
			{
				throw new TestAssertionException();
			}

			if (" A ".IsWhitespace())
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Join_Test ()
		{
			if ((new string[] { }).Join() != string.Empty)
			{
				throw new TestAssertionException();
			}

			if ((new string[] { }).Join('@') != string.Empty)
			{
				throw new TestAssertionException();
			}

			if ((new string[] { }).Join("..") != string.Empty)
			{
				throw new TestAssertionException();
			}

			if ((new[] {"A"}).Join() != "A")
			{
				throw new TestAssertionException();
			}

			if ((new[] {"A"}).Join('@') != "A")
			{
				throw new TestAssertionException();
			}

			if ((new[] {"A"}).Join("..") != "A")
			{
				throw new TestAssertionException();
			}

			if ((new[] {"A", "B"}).Join() != "AB")
			{
				throw new TestAssertionException();
			}

			if ((new[] {"A", "B"}).Join('@') != "A@B")
			{
				throw new TestAssertionException();
			}

			if ((new[] {"A", "B"}).Join("..") != "A..B")
			{
				throw new TestAssertionException();
			}

			if ((new[] {"A", "B", "CD"}).Join() != "ABCD")
			{
				throw new TestAssertionException();
			}

			if ((new[] {"A", "B", "CD"}).Join('@') != "A@B@CD")
			{
				throw new TestAssertionException();
			}

			if ((new[] {"A", "B", "CD"}).Join("..") != "A..B..CD")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ModifyOccurrence_Test ()
		{
			if (string.Empty.ModifyOccurrence('A', 1.0, 0) != string.Empty)
			{
				throw new TestAssertionException();
			}

			if (string.Empty.ModifyOccurrence('A', 3.0, 0) != string.Empty)
			{
				throw new TestAssertionException();
			}

			if (string.Empty.ModifyOccurrence('A', 1.0, 1) != string.Empty)
			{
				throw new TestAssertionException();
			}

			if (string.Empty.ModifyOccurrence('A', 3.0, 1) != string.Empty)
			{
				throw new TestAssertionException();
			}

			if ("A".ModifyOccurrence('A', 1.0, 0) != "A")
			{
				throw new TestAssertionException();
			}

			if ("A".ModifyOccurrence('A', 3.0, 0) != "AAA")
			{
				throw new TestAssertionException();
			}

			if ("A".ModifyOccurrence('A', 1.0, 1) != "AA")
			{
				throw new TestAssertionException();
			}

			if ("A".ModifyOccurrence('A', 3.0, 1) != "AAAA")
			{
				throw new TestAssertionException();
			}

			if ("AA".ModifyOccurrence('A', 1.0, 0) != "AA")
			{
				throw new TestAssertionException();
			}

			if ("AA".ModifyOccurrence('A', 3.0, 0) != "AAAAAA")
			{
				throw new TestAssertionException();
			}

			if ("AA".ModifyOccurrence('A', 1.0, 1) != "AAA")
			{
				throw new TestAssertionException();
			}

			if ("AA".ModifyOccurrence('A', 3.0, 1) != "AAAAAAA")
			{
				throw new TestAssertionException();
			}

			if ("AAA".ModifyOccurrence('A', 1.0, 0) != "AAA")
			{
				throw new TestAssertionException();
			}

			if ("AAA".ModifyOccurrence('A', 3.0, 0) != "AAAAAAAAA")
			{
				throw new TestAssertionException();
			}

			if ("AAA".ModifyOccurrence('A', 1.0, 1) != "AAAA")
			{
				throw new TestAssertionException();
			}

			if ("AAA".ModifyOccurrence('A', 3.0, 1) != "AAAAAAAAAA")
			{
				throw new TestAssertionException();
			}

			if ("AAAA".ModifyOccurrence('A', 1.0, 0) != "AAAA")
			{
				throw new TestAssertionException();
			}

			if ("AAAA".ModifyOccurrence('A', 3.0, 0) != "AAAAAAAAAAAA")
			{
				throw new TestAssertionException();
			}

			if ("AAAA".ModifyOccurrence('A', 1.0, 1) != "AAAAA")
			{
				throw new TestAssertionException();
			}

			if ("AAAA".ModifyOccurrence('A', 3.0, 1) != "AAAAAAAAAAAAA")
			{
				throw new TestAssertionException();
			}

			if ("AAAA".ModifyOccurrence('A', 0.25, 0) != "A")
			{
				throw new TestAssertionException();
			}

			if ("AAAA".ModifyOccurrence('A', 1.0, -2) != "AA")
			{
				throw new TestAssertionException();
			}

			if ("AAAA".ModifyOccurrence('A', 0.25, -2) != string.Empty)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void NormalizeLineBreaks_Test ()
		{
			if ("\r\r\n\n".NormalizeLineBreaks() != ("\r" + Environment.NewLine + Environment.NewLine))
			{
				throw new TestAssertionException();
			}

			if ("\r\r\nabc\n123".NormalizeLineBreaks() != ("\r" + Environment.NewLine + "abc" + Environment.NewLine + "123"))
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Repeat_Test ()
		{
			if (string.Empty.Repeat(0) != "")
			{
				throw new TestAssertionException();
			}

			if (string.Empty.Repeat(1) != "")
			{
				throw new TestAssertionException();
			}

			if (string.Empty.Repeat(3) != "")
			{
				throw new TestAssertionException();
			}

			if (string.Empty.Repeat(0, '@') != "")
			{
				throw new TestAssertionException();
			}

			if (string.Empty.Repeat(1, '@') != "")
			{
				throw new TestAssertionException();
			}

			if (string.Empty.Repeat(3, '@') != "@@")
			{
				throw new TestAssertionException();
			}

			if (string.Empty.Repeat(0, "..") != "")
			{
				throw new TestAssertionException();
			}

			if (string.Empty.Repeat(1, "..") != "")
			{
				throw new TestAssertionException();
			}

			if (string.Empty.Repeat(3, "..") != "....")
			{
				throw new TestAssertionException();
			}

			if ('A'.Repeat(0) != "")
			{
				throw new TestAssertionException();
			}

			if ('A'.Repeat(1) != "A")
			{
				throw new TestAssertionException();
			}

			if ('A'.Repeat(3) != "AAA")
			{
				throw new TestAssertionException();
			}

			if ('A'.Repeat(0, '@') != "")
			{
				throw new TestAssertionException();
			}

			if ('A'.Repeat(1, '@') != "A")
			{
				throw new TestAssertionException();
			}

			if ('A'.Repeat(3, '@') != "A@A@A")
			{
				throw new TestAssertionException();
			}

			if ('A'.Repeat(0, "..") != "")
			{
				throw new TestAssertionException();
			}

			if ('A'.Repeat(1, "..") != "A")
			{
				throw new TestAssertionException();
			}

			if ('A'.Repeat(3, "..") != "A..A..A")
			{
				throw new TestAssertionException();
			}

			if ("abc".Repeat(0) != "")
			{
				throw new TestAssertionException();
			}

			if ("abc".Repeat(1) != "abc")
			{
				throw new TestAssertionException();
			}

			if ("abc".Repeat(3) != "abcabcabc")
			{
				throw new TestAssertionException();
			}

			if ("abc".Repeat(0, '@') != "")
			{
				throw new TestAssertionException();
			}

			if ("abc".Repeat(1, '@') != "abc")
			{
				throw new TestAssertionException();
			}

			if ("abc".Repeat(3, '@') != "abc@abc@abc")
			{
				throw new TestAssertionException();
			}

			if ("abc".Repeat(0, "..") != "")
			{
				throw new TestAssertionException();
			}

			if ("abc".Repeat(1, "..") != "abc")
			{
				throw new TestAssertionException();
			}

			if ("abc".Repeat(3, "..") != "abc..abc..abc")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ReplaceSingleStart_Test ()
		{
			if ("abcd".ReplaceSingleStart("", "", StringComparison.Ordinal) != "abcd")
			{
				throw new TestAssertionException();
			}

			if ("abxcd".ReplaceSingleStart("|X", "@", StringComparison.Ordinal) != "abxcd")
			{
				throw new TestAssertionException();
			}

			if ("abxcd".ReplaceSingleStart("|X", "@", StringComparison.OrdinalIgnoreCase) != "abxcd")
			{
				throw new TestAssertionException();
			}

			if ("ab|cd".ReplaceSingleStart("|X", "@", StringComparison.Ordinal) != "ab|cd")
			{
				throw new TestAssertionException();
			}

			if ("ab|cd".ReplaceSingleStart("|X", "@", StringComparison.OrdinalIgnoreCase) != "ab|cd")
			{
				throw new TestAssertionException();
			}

			if ("ab|xcd".ReplaceSingleStart("|X", "@", StringComparison.Ordinal) != "ab|xcd")
			{
				throw new TestAssertionException();
			}

			if ("ab|xcd".ReplaceSingleStart("|X", "@", StringComparison.OrdinalIgnoreCase) != "ab@cd")
			{
				throw new TestAssertionException();
			}

			if ("ab||xcd".ReplaceSingleStart("|X", "@", StringComparison.Ordinal) != "ab||xcd")
			{
				throw new TestAssertionException();
			}

			if ("ab||xcd".ReplaceSingleStart("|X", "@", StringComparison.OrdinalIgnoreCase) != "ab||xcd")
			{
				throw new TestAssertionException();
			}

			if ("ab|||xcd".ReplaceSingleStart("|X", "@", StringComparison.OrdinalIgnoreCase) != "ab|||xcd")
			{
				throw new TestAssertionException();
			}

			if ("abc".ReplaceSingleStart("c", "x", StringComparison.OrdinalIgnoreCase) != "abx")
			{
				throw new TestAssertionException();
			}

			if ("abcd".ReplaceSingleStart("c", "x", StringComparison.OrdinalIgnoreCase) != "abxd")
			{
				throw new TestAssertionException();
			}

			if ("abcd".ReplaceSingleStart("c", "", StringComparison.OrdinalIgnoreCase) != "abd")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Split_Test ()
		{
			string[] result = "abcHALLOxyzCIAOtestHALLOCIAO ".Split("HALLO", "CIAO");
			if (result.Length != 5)
			{
				throw new TestAssertionException();
			}
			if (result[0] != "abc")
			{
				throw new TestAssertionException();
			}
			if (result[1] != "xyz")
			{
				throw new TestAssertionException();
			}
			if (result[2] != "test")
			{
				throw new TestAssertionException();
			}
			if (result[3] != "")
			{
				throw new TestAssertionException();
			}
			if (result[4] != " ")
			{
				throw new TestAssertionException();
			}

			result = "abcHALLOxyzCIAOtestHALLOCIAO ".Split(StringSplitOptions.None, "HALLO", "CIAO");
			if (result.Length != 5)
			{
				throw new TestAssertionException();
			}
			if (result[0] != "abc")
			{
				throw new TestAssertionException();
			}
			if (result[1] != "xyz")
			{
				throw new TestAssertionException();
			}
			if (result[2] != "test")
			{
				throw new TestAssertionException();
			}
			if (result[3] != "")
			{
				throw new TestAssertionException();
			}
			if (result[4] != " ")
			{
				throw new TestAssertionException();
			}

			result = "abcHALLOxyzCIAOtestHALLOCIAO ".Split(StringSplitOptions.RemoveEmptyEntries, "HALLO", "CIAO");
			if (result.Length != 4)
			{
				throw new TestAssertionException();
			}
			if (result[0] != "abc")
			{
				throw new TestAssertionException();
			}
			if (result[1] != "xyz")
			{
				throw new TestAssertionException();
			}
			if (result[2] != "test")
			{
				throw new TestAssertionException();
			}
			if (result[3] != " ")
			{
				throw new TestAssertionException();
			}

			result = "abc#xyz@test@# ".Split(StringSplitOptions.None, '#', '@');
			if (result.Length != 5)
			{
				throw new TestAssertionException();
			}
			if (result[0] != "abc")
			{
				throw new TestAssertionException();
			}
			if (result[1] != "xyz")
			{
				throw new TestAssertionException();
			}
			if (result[2] != "test")
			{
				throw new TestAssertionException();
			}
			if (result[3] != "")
			{
				throw new TestAssertionException();
			}
			if (result[4] != " ")
			{
				throw new TestAssertionException();
			}

			result = "abc#xyz@test@# ".Split(StringSplitOptions.RemoveEmptyEntries, '#', '@');
			if (result.Length != 4)
			{
				throw new TestAssertionException();
			}
			if (result[0] != "abc")
			{
				throw new TestAssertionException();
			}
			if (result[1] != "xyz")
			{
				throw new TestAssertionException();
			}
			if (result[2] != "test")
			{
				throw new TestAssertionException();
			}
			if (result[3] != " ")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void SplitLines_Test ()
		{
			string[] test = null;

			//----------------
			// Without options
			//----------------

			test = "".SplitLines();

			if (test.Length != 1)
			{
				throw new TestAssertionException();
			}

			if (test[0] != "")
			{
				throw new TestAssertionException();
			}

			test = " ".SplitLines();

			if (test.Length != 1)
			{
				throw new TestAssertionException();
			}

			if (test[0] != " ")
			{
				throw new TestAssertionException();
			}

			test = "\r\r\n\n".SplitLines();

			if (test.Length != 3)
			{
				throw new TestAssertionException();
			}

			if (test[0] != "\r")
			{
				throw new TestAssertionException();
			}

			if (test[1] != "")
			{
				throw new TestAssertionException();
			}

			if (test[2] != "")
			{
				throw new TestAssertionException();
			}

			test = "\r\r\nabc\n123".SplitLines();

			if (test.Length != 3)
			{
				throw new TestAssertionException();
			}

			if (test[0] != "\r")
			{
				throw new TestAssertionException();
			}

			if (test[1] != "abc")
			{
				throw new TestAssertionException();
			}

			if (test[2] != "123")
			{
				throw new TestAssertionException();
			}

			//-------------
			// With options
			//-------------

			test = "".SplitLines(StringSplitOptions.RemoveEmptyEntries);

			if (test.Length != 0)
			{
				throw new TestAssertionException();
			}

			test = " ".SplitLines();

			if (test.Length != 1)
			{
				throw new TestAssertionException();
			}

			if (test[0] != " ")
			{
				throw new TestAssertionException();
			}

			test = "\r\r\n\n".SplitLines(StringSplitOptions.RemoveEmptyEntries);

			if (test.Length != 1)
			{
				throw new TestAssertionException();
			}

			if (test[0] != "\r")
			{
				throw new TestAssertionException();
			}

			test = "\r\r\nabc\n123".SplitLines(StringSplitOptions.RemoveEmptyEntries);

			if (test.Length != 3)
			{
				throw new TestAssertionException();
			}

			if (test[0] != "\r")
			{
				throw new TestAssertionException();
			}

			if (test[1] != "abc")
			{
				throw new TestAssertionException();
			}

			if (test[2] != "123")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void SplitWhere_Test ()
		{
			string[] result = "".SplitWhere((s, c, p, n) => (p == 3) || (n == 5));

			if (result.Length != 1)
			{
				throw new TestAssertionException();
			}

			if (result[0] != "")
			{
				throw new TestAssertionException();
			}

			result = "abcd fghijklmnopqrstuvwxyz".SplitWhere((s, c, p, n) => (p == 3) || (n == 5) || (n == 5));

			if (result.Length != 3)
			{
				throw new TestAssertionException();
			}

			if (result[0] != "abcd")
			{
				throw new TestAssertionException();
			}

			if (result[1] != " ")
			{
				throw new TestAssertionException();
			}

			if (result[2] != "fghijklmnopqrstuvwxyz")
			{
				throw new TestAssertionException();
			}

			result = "abcd fghijklmnopqrstuvwxyz".SplitWhere(StringSplitOptions.RemoveEmptyEntries, (s, c, p, n) => (p == 3) || (n == 5) || (n == 5));

			if (result.Length != 2)
			{
				throw new TestAssertionException();
			}

			if (result[0] != "abcd")
			{
				throw new TestAssertionException();
			}

			if (result[1] != "fghijklmnopqrstuvwxyz")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void StartsWithCount_Test ()
		{
			if (string.Empty.StartsWithCount('A', StringComparison.Ordinal) != 0)
			{
				throw new TestAssertionException();
			}

			if (string.Empty.StartsWithCount('A', StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new TestAssertionException();
			}

			if ("BCD".StartsWithCount('A', StringComparison.Ordinal) != 0)
			{
				throw new TestAssertionException();
			}

			if ("BCD".StartsWithCount('A', StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new TestAssertionException();
			}

			if ("ABC".StartsWithCount('A', StringComparison.Ordinal) != 1)
			{
				throw new TestAssertionException();
			}

			if ("ABC".StartsWithCount('A', StringComparison.OrdinalIgnoreCase) != 1)
			{
				throw new TestAssertionException();
			}

			if ("AABC".StartsWithCount('A', StringComparison.Ordinal) != 2)
			{
				throw new TestAssertionException();
			}

			if ("AABC".StartsWithCount('A', StringComparison.OrdinalIgnoreCase) != 2)
			{
				throw new TestAssertionException();
			}

			if ("CBA".StartsWithCount('A', StringComparison.Ordinal) != 0)
			{
				throw new TestAssertionException();
			}

			if ("CBA".StartsWithCount('A', StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new TestAssertionException();
			}

			if ("CBAA".StartsWithCount('A', StringComparison.Ordinal) != 0)
			{
				throw new TestAssertionException();
			}

			if ("CBAA".StartsWithCount('A', StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new TestAssertionException();
			}

			if ("A".StartsWithCount('A', StringComparison.Ordinal) != 1)
			{
				throw new TestAssertionException();
			}

			if ("A".StartsWithCount('A', StringComparison.OrdinalIgnoreCase) != 1)
			{
				throw new TestAssertionException();
			}

			if ("AA".StartsWithCount('A', StringComparison.Ordinal) != 2)
			{
				throw new TestAssertionException();
			}

			if ("AA".StartsWithCount('A', StringComparison.OrdinalIgnoreCase) != 2)
			{
				throw new TestAssertionException();
			}

			if ("AAA".StartsWithCount('A', StringComparison.Ordinal) != 3)
			{
				throw new TestAssertionException();
			}

			if ("AAA".StartsWithCount('A', StringComparison.OrdinalIgnoreCase) != 3)
			{
				throw new TestAssertionException();
			}

			if ("AA".StartsWithCount('a', StringComparison.Ordinal) != 0)
			{
				throw new TestAssertionException();
			}

			if ("AA".StartsWithCount('a', StringComparison.OrdinalIgnoreCase) != 2)
			{
				throw new TestAssertionException();
			}

			if ("abc@@".StartsWithCount("@@", StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new TestAssertionException();
			}

			if ("abc@@@".StartsWithCount("@@", StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new TestAssertionException();
			}

			if ("abc@@@@".StartsWithCount("@@", StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new TestAssertionException();
			}

			if ("@@abc".StartsWithCount("@@", StringComparison.OrdinalIgnoreCase) != 1)
			{
				throw new TestAssertionException();
			}

			if ("@@@abc".StartsWithCount("@@", StringComparison.OrdinalIgnoreCase) != 1)
			{
				throw new TestAssertionException();
			}

			if ("@@@@abc".StartsWithCount("@@", StringComparison.OrdinalIgnoreCase) != 2)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToBoolean_Test ()
		{
			//----------
			// Has value
			//----------

			if (string.Empty.ToBoolean().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("test".ToBoolean().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"true".ToBoolean().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"TRUE".ToBoolean().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"false".ToBoolean().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"FALSE".ToBoolean().HasValue)
			{
				throw new TestAssertionException();
			}

			//-----
			// True
			//-----

			if (!"true".ToBoolean().Value)
			{
				throw new TestAssertionException();
			}

			if (!"TRUE".ToBoolean().Value)
			{
				throw new TestAssertionException();
			}

			if (!"yes".ToBoolean().Value)
			{
				throw new TestAssertionException();
			}

			if (!"YES".ToBoolean().Value)
			{
				throw new TestAssertionException();
			}

			if (!"on".ToBoolean().Value)
			{
				throw new TestAssertionException();
			}

			if (!"ON".ToBoolean().Value)
			{
				throw new TestAssertionException();
			}

			if (!"1".ToBoolean().Value)
			{
				throw new TestAssertionException();
			}

			//------
			// False
			//------

			if ("false".ToBoolean().Value)
			{
				throw new TestAssertionException();
			}

			if ("FALSE".ToBoolean().Value)
			{
				throw new TestAssertionException();
			}

			if ("no".ToBoolean().Value)
			{
				throw new TestAssertionException();
			}

			if ("NO".ToBoolean().Value)
			{
				throw new TestAssertionException();
			}

			if ("off".ToBoolean().Value)
			{
				throw new TestAssertionException();
			}

			if ("OFF".ToBoolean().Value)
			{
				throw new TestAssertionException();
			}

			if ("0".ToBoolean().Value)
			{
				throw new TestAssertionException();
			}

			//----------
			// Untrimmed
			//----------

			if (!" true".ToBoolean().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"true ".ToBoolean().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!" true ".ToBoolean().HasValue)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToByte_Test ()
		{
			//----------
			// Has value
			//----------

			if (string.Empty.ToByte().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("test".ToByte().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("-1".ToByte().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("-1".ToByteInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("256".ToByte().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("256".ToByteInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("1.2".ToByteInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"0".ToByte().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"0".ToByteInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"255".ToByte().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"255".ToByteInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			//-------
			// Values
			//-------

			if ("0".ToByte().Value != 0)
			{
				throw new TestAssertionException();
			}

			if ("0".ToByteInvariant().Value != 0)
			{
				throw new TestAssertionException();
			}

			if ("255".ToByte().Value != 255)
			{
				throw new TestAssertionException();
			}

			if ("255".ToByteInvariant().Value != 255)
			{
				throw new TestAssertionException();
			}

			//----------
			// Untrimmed
			//----------

			if (!" 0".ToByteInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"0 ".ToByteInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!" 0 ".ToByteInvariant().HasValue)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToDateTimeFromSortable_Test ()
		{
			//---------------
			// Invalid values
			//---------------

			if (string.Empty.ToDateTimeFromSortable().HasValue)
			{
				throw new TestAssertionException();
			}

			if (string.Empty.ToDateTimeFromSortable('-').HasValue)
			{
				throw new TestAssertionException();
			}

			if (string.Empty.ToDateTimeFromSortable("###").HasValue)
			{
				throw new TestAssertionException();
			}

			if ("test".ToDateTimeFromSortable().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("test".ToDateTimeFromSortable('-').HasValue)
			{
				throw new TestAssertionException();
			}

			if ("test".ToDateTimeFromSortable("###").HasValue)
			{
				throw new TestAssertionException();
			}

			if ("20001303040506007".ToDateTimeFromSortable().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("2000-13-03-04-05-06-007".ToDateTimeFromSortable('-').HasValue)
			{
				throw new TestAssertionException();
			}

			if ("2000###13###03###04###05###06###007".ToDateTimeFromSortable("###").HasValue)
			{
				throw new TestAssertionException();
			}

			//-------------
			// Valid values
			//-------------

			if ("20000203040506007".ToDateTimeFromSortable().Value != new DateTime(2000, 2, 3, 4, 5, 6, 7))
			{
				throw new TestAssertionException();
			}

			if ("2000-02-03-04-05-06-007".ToDateTimeFromSortable('-').Value != new DateTime(2000, 2, 3, 4, 5, 6, 7))
			{
				throw new TestAssertionException();
			}

			if ("2000###02###03###04###05###06###007".ToDateTimeFromSortable("###").Value != new DateTime(2000, 2, 3, 4, 5, 6, 7))
			{
				throw new TestAssertionException();
			}

			if (" 20000203040506007".ToDateTimeFromSortable().Value != new DateTime(2000, 2, 3, 4, 5, 6, 7))
			{
				throw new TestAssertionException();
			}

			if ("20000203040506007 ".ToDateTimeFromSortable().Value != new DateTime(2000, 2, 3, 4, 5, 6, 7))
			{
				throw new TestAssertionException();
			}

			if (" 20000203040506007 ".ToDateTimeFromSortable().Value != new DateTime(2000, 2, 3, 4, 5, 6, 7))
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToDecimal_Test ()
		{
			//----------
			// Has value
			//----------

			if (string.Empty.ToDecimal().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("test".ToDecimal().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("-79228162514264337593543950336".ToDecimalInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("79228162514264337593543950336".ToDecimalInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"1.2".ToDecimalInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"-79228162514264337593543950335".ToDecimalInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"79228162514264337593543950335".ToDecimalInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			//-------
			// Values
			//-------

			if ("-79228162514264337593543950335".ToDecimalInvariant().Value != -79228162514264337593543950335m)
			{
				throw new TestAssertionException();
			}

			if ("79228162514264337593543950335".ToDecimalInvariant().Value != 79228162514264337593543950335m)
			{
				throw new TestAssertionException();
			}

			if ("0".ToDecimal().Value != 0m)
			{
				throw new TestAssertionException();
			}

			if ("-1".ToDecimal().Value != -1m)
			{
				throw new TestAssertionException();
			}

			if ("1".ToDecimal().Value != 1m)
			{
				throw new TestAssertionException();
			}

			if ("0".ToDecimalInvariant().Value != 0.0m)
			{
				throw new TestAssertionException();
			}

			if ("0.0".ToDecimalInvariant().Value != 0.0m)
			{
				throw new TestAssertionException();
			}

			//----------
			// Untrimmed
			//----------

			if (!" 0".ToDecimalInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"0 ".ToDecimalInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!" 0 ".ToDecimalInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!" 0.0".ToDecimalInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"0.0 ".ToDecimalInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!" 0.0 ".ToDecimalInvariant().HasValue)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToDouble_Test ()
		{
			//----------
			// Has value
			//----------

			if (string.Empty.ToDouble().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("test".ToDouble().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("-1.79769313486232e308".ToDoubleInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("1.79769313486232e308".ToDoubleInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"1.2".ToDoubleInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"-1.79769313486231e308".ToDoubleInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"1.79769313486231e308".ToDoubleInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			//-------
			// Values
			//-------

			if ("-1.79769313486231e308".ToDoubleInvariant().Value != -1.79769313486231e308)
			{
				throw new TestAssertionException();
			}

			if ("1.79769313486231e308".ToDoubleInvariant().Value != 1.79769313486231e308)
			{
				throw new TestAssertionException();
			}

			if ("0".ToDouble().Value != 0.0)
			{
				throw new TestAssertionException();
			}

			if ("-1".ToDouble().Value != -1.0)
			{
				throw new TestAssertionException();
			}

			if ("1".ToDouble().Value != 1.0)
			{
				throw new TestAssertionException();
			}

			if ("0".ToDoubleInvariant().Value != 0.0)
			{
				throw new TestAssertionException();
			}

			if ("0.0".ToDoubleInvariant().Value != 0.0)
			{
				throw new TestAssertionException();
			}

			//----------
			// Untrimmed
			//----------

			if (!" 0".ToDoubleInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"0 ".ToDoubleInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!" 0 ".ToDoubleInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!" 0.0".ToDoubleInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"0.0 ".ToDoubleInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!" 0.0 ".ToDoubleInvariant().HasValue)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToEmptyIfNullOrEmpty_Test ()
		{
			if (((string)null).ToEmptyIfNullOrEmpty() != string.Empty)
			{
				throw new TestAssertionException();
			}

			if (string.Empty.ToEmptyIfNullOrEmpty() != string.Empty)
			{
				throw new TestAssertionException();
			}

			if (" ".ToEmptyIfNullOrEmpty() == string.Empty)
			{
				throw new TestAssertionException();
			}

			if ("test".ToEmptyIfNullOrEmpty() == string.Empty)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToEmptyIfNullOrEmptyOrWhitespace_Test ()
		{
			if (((string)null).ToEmptyIfNullOrEmptyOrWhitespace() != string.Empty)
			{
				throw new TestAssertionException();
			}

			if (string.Empty.ToEmptyIfNullOrEmptyOrWhitespace() != string.Empty)
			{
				throw new TestAssertionException();
			}

			if (" ".ToEmptyIfNullOrEmptyOrWhitespace() != string.Empty)
			{
				throw new TestAssertionException();
			}

			if ("test".ToEmptyIfNullOrEmptyOrWhitespace() == string.Empty)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToEnum_Test ()
		{
			//----------
			// Has value
			//----------

			if (string.Empty.ToEnum<UriFormat>().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("test".ToEnum<UriFormat>().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"UriEscaped".ToEnum<UriFormat>().HasValue)
			{
				throw new TestAssertionException();
			}

			if (string.Empty.ToEnum(typeof(UriFormat)) != null)
			{
				throw new TestAssertionException();
			}

			if ("test".ToEnum(typeof(UriFormat)) != null)
			{
				throw new TestAssertionException();
			}

			if ("UriEscaped".ToEnum(typeof(UriFormat)) == null)
			{
				throw new TestAssertionException();
			}

			//--------
			// Generic
			//--------

			if ("UriEscaped".ToEnum<UriFormat>().Value != UriFormat.UriEscaped)
			{
				throw new TestAssertionException();
			}

			if ("1".ToEnum<UriFormat>().Value != UriFormat.UriEscaped)
			{
				throw new TestAssertionException();
			}

			//-----
			// Type
			//-----

			if ((UriFormat)"UriEscaped".ToEnum(typeof(UriFormat)) != UriFormat.UriEscaped)
			{
				throw new TestAssertionException();
			}

			if ((UriFormat)"1".ToEnum(typeof(UriFormat)) != UriFormat.UriEscaped)
			{
				throw new TestAssertionException();
			}

			//----------
			// Untrimmed
			//----------

			if (!" UriEscaped".ToEnum<UriFormat>().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"UriEscaped ".ToEnum<UriFormat>().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!" UriEscaped ".ToEnum<UriFormat>().HasValue)
			{
				throw new TestAssertionException();
			}

			if (" UriEscaped".ToEnum(typeof(UriFormat)) == null)
			{
				throw new TestAssertionException();
			}

			if ("UriEscaped ".ToEnum(typeof(UriFormat)) == null)
			{
				throw new TestAssertionException();
			}

			if (" UriEscaped ".ToEnum(typeof(UriFormat)) == null)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToFloat_Test ()
		{
			//----------
			// Has value
			//----------

			if (string.Empty.ToFloat().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("test".ToFloat().HasValue)
			{
				throw new TestAssertionException();
			}

#if PLATFORM_NETFX
			if ("-3.402824e38".ToFloatInvariant().HasValue)
			{
				throw new TestAssertionException();
			}
#else
			if ("-3.402824e38".ToFloatInvariant().Value != float.NegativeInfinity)
			{
				throw new TestAssertionException();
			}
#endif

			if ("3.402824e38".ToFloatInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"1.2".ToFloatInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"-3.402823e38".ToFloatInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"3.402823e38".ToFloatInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			//-------
			// Values
			//-------

			if ("-3.402823e38".ToFloatInvariant().Value != -3.402823e38f)
			{
				throw new TestAssertionException();
			}

			if ("3.402823e38".ToFloatInvariant().Value != 3.402823e38f)
			{
				throw new TestAssertionException();
			}

			if ("0".ToFloat().Value != 0.0f)
			{
				throw new TestAssertionException();
			}

			if ("-1".ToFloat().Value != -1.0f)
			{
				throw new TestAssertionException();
			}

			if ("1".ToFloat().Value != 1.0f)
			{
				throw new TestAssertionException();
			}

			if ("0".ToFloatInvariant().Value != 0.0f)
			{
				throw new TestAssertionException();
			}

			if ("0.0".ToFloatInvariant().Value != 0.0f)
			{
				throw new TestAssertionException();
			}

			//----------
			// Untrimmed
			//----------

			if (!" 0".ToFloatInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"0 ".ToFloatInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!" 0 ".ToFloatInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!" 0.0".ToFloatInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"0.0 ".ToFloatInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!" 0.0 ".ToFloatInvariant().HasValue)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToGuid_Test ()
		{
			if (string.Empty.ToGuid().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("test".ToGuid().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("F8DB57DCA314421F8CA1E88F6B73126".ToGuid().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("F8DB57DCA314421F8CA1E88F6B73126Z".ToGuid().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"{17F87CCD-C3D0-4211-8D3B-6C5F8FBE394A}".ToGuid().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"{17f87ccd-c3d0-4211-8d3b-6c5f8fbe394a}".ToGuid().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"71B759B7-C14C-411B-9234-434A6AC46112".ToGuid().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"71b759b7-c14c-411b-9234-434a6ac46112".ToGuid().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"F8DB57DCA314421F8CA1E88F6B73126E".ToGuid().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"f8db57dca314421f8ca1e88f6b73126e".ToGuid().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"(4CD3826B-47AE-48D1-95CE-5F3EA533222E)".ToGuid().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"(4cd3826b-47ae-48d1-95ce-5f3ea533222e)".ToGuid().HasValue)
			{
				throw new TestAssertionException();
			}
#if PLATFORM_NETFX
			if (!"{0XCC5BE5A0,0X79A5,0X410F,{0XB8,0XDF,0XBF,0X74,0X4C,0X48,0XD5,0X18}}".ToGuid().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"{0xcc5be5a0,0x79a5,0x410f,{0xb8,0xdf,0xbf,0x74,0x4c,0x48,0xd5,0x18}}".ToGuid().HasValue)
			{
				throw new TestAssertionException();
			}
#endif
		}

		[TestMethod]
		public void ToInt16_Test ()
		{
			//----------
			// Has value
			//----------

			if (string.Empty.ToInt16().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("test".ToInt16().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("-32769".ToInt16().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("-32769".ToInt16Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("32768".ToInt16().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("32768".ToInt16Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("1.2".ToInt16Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"-32768".ToInt16().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"-32768".ToInt16Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"32767".ToInt16().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"32767".ToInt16Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			//-------
			// Values
			//-------

			if ("-32768".ToInt16().Value != -32768)
			{
				throw new TestAssertionException();
			}

			if ("-32768".ToInt16Invariant().Value != -32768)
			{
				throw new TestAssertionException();
			}

			if ("32767".ToInt16().Value != 32767)
			{
				throw new TestAssertionException();
			}

			if ("32767".ToInt16Invariant().Value != 32767)
			{
				throw new TestAssertionException();
			}

			if ("0".ToInt16().Value != 0)
			{
				throw new TestAssertionException();
			}

			if ("0".ToInt16Invariant().Value != 0)
			{
				throw new TestAssertionException();
			}

			//----------
			// Untrimmed
			//----------

			if (!" 0".ToInt16Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"0 ".ToInt16Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!" 0 ".ToInt16Invariant().HasValue)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToInt32_Test ()
		{
			//----------
			// Has value
			//----------

			if (string.Empty.ToInt32().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("test".ToInt32().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("-2147483649".ToInt32().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("-2147483649".ToInt32Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("2147483648".ToInt32().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("2147483648".ToInt32Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("1.2".ToInt32Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"-2147483648".ToInt32().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"-2147483648".ToInt32Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"2147483647".ToInt32().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"2147483647".ToInt32Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			//-------
			// Values
			//-------

			if ("-2147483648".ToInt32().Value != -2147483648)
			{
				throw new TestAssertionException();
			}

			if ("-2147483648".ToInt32Invariant().Value != -2147483648)
			{
				throw new TestAssertionException();
			}

			if ("2147483647".ToInt32().Value != 2147483647)
			{
				throw new TestAssertionException();
			}

			if ("2147483647".ToInt32Invariant().Value != 2147483647)
			{
				throw new TestAssertionException();
			}

			if ("0".ToInt32().Value != 0)
			{
				throw new TestAssertionException();
			}

			if ("0".ToInt32Invariant().Value != 0)
			{
				throw new TestAssertionException();
			}

			//----------
			// Untrimmed
			//----------

			if (!" 0".ToInt32Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"0 ".ToInt32Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!" 0 ".ToInt32Invariant().HasValue)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToInt64_Test ()
		{
			//----------
			// Has value
			//----------

			if (string.Empty.ToInt64().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("test".ToInt64().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("-9223372036854775809".ToInt64().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("-9223372036854775809".ToInt64Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("9223372036854775808".ToInt64().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("9223372036854775808".ToInt64Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("1.2".ToInt64Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"-9223372036854775808".ToInt64().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"-9223372036854775808".ToInt64Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"9223372036854775807".ToInt64().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"9223372036854775807".ToInt64Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			//-------
			// Values
			//-------

			if ("-9223372036854775808".ToInt64().Value != -9223372036854775808)
			{
				throw new TestAssertionException();
			}

			if ("-9223372036854775808".ToInt64Invariant().Value != -9223372036854775808)
			{
				throw new TestAssertionException();
			}

			if ("9223372036854775807".ToInt64().Value != 9223372036854775807)
			{
				throw new TestAssertionException();
			}

			if ("9223372036854775807".ToInt64Invariant().Value != 9223372036854775807)
			{
				throw new TestAssertionException();
			}

			if ("0".ToInt64().Value != 0)
			{
				throw new TestAssertionException();
			}

			if ("0".ToInt64Invariant().Value != 0)
			{
				throw new TestAssertionException();
			}

			//----------
			// Untrimmed
			//----------

			if (!" 0".ToInt64Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"0 ".ToInt64Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!" 0 ".ToInt64Invariant().HasValue)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToNullIfNullOrEmpty_Test ()
		{
			if (((string)null).ToNullIfNullOrEmpty() != null)
			{
				throw new TestAssertionException();
			}

			if (string.Empty.ToNullIfNullOrEmpty() != null)
			{
				throw new TestAssertionException();
			}

			if (" ".ToNullIfNullOrEmpty() == null)
			{
				throw new TestAssertionException();
			}

			if ("test".ToNullIfNullOrEmpty() == null)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToNullIfNullOrEmptyOrWhitespace_Test ()
		{
			if (((string)null).ToNullIfNullOrEmptyOrWhitespace() != null)
			{
				throw new TestAssertionException();
			}

			if (string.Empty.ToNullIfNullOrEmptyOrWhitespace() != null)
			{
				throw new TestAssertionException();
			}

			if (" ".ToNullIfNullOrEmptyOrWhitespace() != null)
			{
				throw new TestAssertionException();
			}

			if ("test".ToNullIfNullOrEmptyOrWhitespace() == null)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToSByte_Test ()
		{
			//----------
			// Has value
			//----------

			if (string.Empty.ToSByte().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("test".ToSByte().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("-129".ToSByte().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("-129".ToSByteInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("128".ToSByte().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("128".ToSByteInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("1.2".ToSByteInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"-128".ToSByte().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"-128".ToSByteInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"127".ToSByte().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"127".ToSByteInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			//-------
			// Values
			//-------

			if ("-128".ToSByte().Value != -128)
			{
				throw new TestAssertionException();
			}

			if ("-128".ToSByteInvariant().Value != -128)
			{
				throw new TestAssertionException();
			}

			if ("127".ToSByte().Value != 127)
			{
				throw new TestAssertionException();
			}

			if ("127".ToSByteInvariant().Value != 127)
			{
				throw new TestAssertionException();
			}

			if ("0".ToSByte().Value != 0)
			{
				throw new TestAssertionException();
			}

			if ("0".ToSByteInvariant().Value != 0)
			{
				throw new TestAssertionException();
			}

			//----------
			// Untrimmed
			//----------

			if (!" 0".ToSByteInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"0 ".ToSByteInvariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!" 0 ".ToSByteInvariant().HasValue)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToTimeSpanFromSortable_Test ()
		{
			//---------------
			// Invalid values
			//---------------

			if (string.Empty.ToTimeSpanFromSortable().HasValue)
			{
				throw new TestAssertionException();
			}

			if (string.Empty.ToTimeSpanFromSortable('-').HasValue)
			{
				throw new TestAssertionException();
			}

			if (string.Empty.ToTimeSpanFromSortable("###").HasValue)
			{
				throw new TestAssertionException();
			}

			if ("test".ToTimeSpanFromSortable().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("test".ToTimeSpanFromSortable('-').HasValue)
			{
				throw new TestAssertionException();
			}

			if ("test".ToTimeSpanFromSortable("###").HasValue)
			{
				throw new TestAssertionException();
			}

			if ("1240304005".ToTimeSpanFromSortable().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("1-24-03-04-005".ToTimeSpanFromSortable('-').HasValue)
			{
				throw new TestAssertionException();
			}

			if ("1###24###03###04###005".ToTimeSpanFromSortable("###").HasValue)
			{
				throw new TestAssertionException();
			}

			//-------------
			// Valid values
			//-------------

			if ("1020304005".ToTimeSpanFromSortable().Value != new TimeSpan(1, 2, 3, 4, 5))
			{
				throw new TestAssertionException();
			}

			if ("1-02-03-04-005".ToTimeSpanFromSortable('-').Value != new TimeSpan(1, 2, 3, 4, 5))
			{
				throw new TestAssertionException();
			}

			if ("1###02###03###04###005".ToTimeSpanFromSortable("###").Value != new TimeSpan(1, 2, 3, 4, 5))
			{
				throw new TestAssertionException();
			}

			if ("-1020304005".ToTimeSpanFromSortable().Value != new TimeSpan(-1, -2, -3, -4, -5))
			{
				throw new TestAssertionException();
			}

			if ("-1-02-03-04-005".ToTimeSpanFromSortable('-').Value != new TimeSpan(-1, -2, -3, -4, -5))
			{
				throw new TestAssertionException();
			}

			if ("-1###02###03###04###005".ToTimeSpanFromSortable("###").Value != new TimeSpan(-1, -2, -3, -4, -5))
			{
				throw new TestAssertionException();
			}

			if ("01020304005".ToTimeSpanFromSortable().Value != new TimeSpan(1, 2, 3, 4, 5))
			{
				throw new TestAssertionException();
			}

			if ("01-02-03-04-005".ToTimeSpanFromSortable('-').Value != new TimeSpan(1, 2, 3, 4, 5))
			{
				throw new TestAssertionException();
			}

			if ("01###02###03###04###005".ToTimeSpanFromSortable("###").Value != new TimeSpan(1, 2, 3, 4, 5))
			{
				throw new TestAssertionException();
			}

			if ("1000020304005".ToTimeSpanFromSortable().Value != new TimeSpan(1000, 2, 3, 4, 5))
			{
				throw new TestAssertionException();
			}

			if ("1000-02-03-04-005".ToTimeSpanFromSortable('-').Value != new TimeSpan(1000, 2, 3, 4, 5))
			{
				throw new TestAssertionException();
			}

			if ("1000###02###03###04###005".ToTimeSpanFromSortable("###").Value != new TimeSpan(1000, 2, 3, 4, 5))
			{
				throw new TestAssertionException();
			}

			if (" 1020304005".ToTimeSpanFromSortable().Value != new TimeSpan(1, 2, 3, 4, 5))
			{
				throw new TestAssertionException();
			}

			if ("1020304005 ".ToTimeSpanFromSortable().Value != new TimeSpan(1, 2, 3, 4, 5))
			{
				throw new TestAssertionException();
			}

			if (" 1020304005 ".ToTimeSpanFromSortable().Value != new TimeSpan(1, 2, 3, 4, 5))
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToUInt16_Test ()
		{
			//----------
			// Has value
			//----------

			if (string.Empty.ToUInt16().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("test".ToUInt16().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("-1".ToUInt16().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("-1".ToUInt16Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("65536".ToUInt16().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("65536".ToUInt16Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("1.2".ToUInt16Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"0".ToUInt16().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"0".ToUInt16Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"65535".ToUInt16().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"65535".ToUInt16Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			//-------
			// Values
			//-------

			if ("0".ToUInt16().Value != 0)
			{
				throw new TestAssertionException();
			}

			if ("0".ToUInt16Invariant().Value != 0)
			{
				throw new TestAssertionException();
			}

			if ("65535".ToUInt16().Value != 65535)
			{
				throw new TestAssertionException();
			}

			if ("65535".ToUInt16Invariant().Value != 65535)
			{
				throw new TestAssertionException();
			}

			//----------
			// Untrimmed
			//----------

			if (!" 0".ToUInt16Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"0 ".ToUInt16Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!" 0 ".ToUInt16Invariant().HasValue)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToUInt32_Test ()
		{
			//----------
			// Has value
			//----------

			if (string.Empty.ToUInt32().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("test".ToUInt32().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("-1".ToUInt32().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("-1".ToUInt32Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("4294967296".ToUInt32().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("4294967296".ToUInt32Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("1.2".ToUInt32Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"0".ToUInt32().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"0".ToUInt32Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"4294967295".ToUInt32().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"4294967295".ToUInt32Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			//-------
			// Values
			//-------

			if ("0".ToUInt32().Value != 0)
			{
				throw new TestAssertionException();
			}

			if ("0".ToUInt32Invariant().Value != 0)
			{
				throw new TestAssertionException();
			}

			if ("4294967295".ToUInt32().Value != 4294967295)
			{
				throw new TestAssertionException();
			}

			if ("4294967295".ToUInt32Invariant().Value != 4294967295)
			{
				throw new TestAssertionException();
			}

			//----------
			// Untrimmed
			//----------

			if (!" 0".ToUInt32Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"0 ".ToUInt32Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!" 0 ".ToUInt32Invariant().HasValue)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToUInt64_Test ()
		{
			//----------
			// Has value
			//----------

			if (string.Empty.ToUInt64().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("test".ToUInt64().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("-1".ToUInt64().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("-1".ToUInt64Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("18446744073709551616".ToUInt64().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("18446744073709551616".ToUInt64Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if ("1.2".ToUInt64Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"0".ToUInt64().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"0".ToUInt64Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"18446744073709551615".ToUInt64().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"18446744073709551615".ToUInt64Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			//-------
			// Values
			//-------

			if ("0".ToUInt64().Value != 0)
			{
				throw new TestAssertionException();
			}

			if ("0".ToUInt64Invariant().Value != 0)
			{
				throw new TestAssertionException();
			}

			if ("18446744073709551615".ToUInt64().Value != 18446744073709551615)
			{
				throw new TestAssertionException();
			}

			if ("18446744073709551615".ToUInt64Invariant().Value != 18446744073709551615)
			{
				throw new TestAssertionException();
			}

			//----------
			// Untrimmed
			//----------

			if (!" 0".ToUInt64Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!"0 ".ToUInt64Invariant().HasValue)
			{
				throw new TestAssertionException();
			}

			if (!" 0 ".ToUInt64Invariant().HasValue)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void ToVersion_Test ()
		{
			if (string.Empty.ToVersion() != null)
			{
				throw new TestAssertionException();
			}

			if ("test".ToVersion() != null)
			{
				throw new TestAssertionException();
			}

			if ("1.2.3.4.5".ToVersion() != null)
			{
				throw new TestAssertionException();
			}

			if ("1".ToVersion() != null)
			{
				throw new TestAssertionException();
			}

			if ("1.2".ToVersion() != new Version(1, 2))
			{
				throw new TestAssertionException();
			}

			if ("1.2.3".ToVersion() != new Version(1, 2, 3))
			{
				throw new TestAssertionException();
			}

			if ("1.2.3.4".ToVersion() != new Version(1, 2, 3, 4))
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Unescape_Test ()
		{
			if (string.Empty.Unescape() != string.Empty)
			{
				throw new TestAssertionException();
			}

			if ("ABC".Unescape() != "ABC")
			{
				throw new TestAssertionException();
			}

			if ("\\a\\b\\f\\n\\r\\t\\v\\\\\\\'\\\"".Unescape() != "\a\b\f\n\r\t\v\\\'\"")
			{
				throw new TestAssertionException();
			}

			if ("\\\\".Unescape() != "\\")
			{
				throw new TestAssertionException();
			}

			if ("\\\\\\\\".Unescape() != "\\\\")
			{
				throw new TestAssertionException();
			}

			if ("\\\\\\\\\\\\".Unescape() != "\\\\\\")
			{
				throw new TestAssertionException();
			}

			if ("AB\\\\CD".Unescape() != "AB\\CD")
			{
				throw new TestAssertionException();
			}

			if ("1\\x2".Unescape() != "1x2")
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
