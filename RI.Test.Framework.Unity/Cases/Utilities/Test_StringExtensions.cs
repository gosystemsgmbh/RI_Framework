using System;
using System.Diagnostics.CodeAnalysis;
using RI.Framework.Utilities;

namespace RI.Test.Framework.Cases.Utilities
{
	[SuppressMessage ("ReSharper", "InconsistentNaming")]
	[SuppressMessage ("ReSharper", "UnusedMember.Global")]
	public sealed class Test_StringExtensions : TestModule
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

			if (!" ".IsEmpty())
			{
				throw new TestAssertionException();
			}

			if (!"  ".IsEmpty())
			{
				throw new TestAssertionException();
			}

			if (!"\f\r\n\t\v".IsEmpty())
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
		public void Join_Test ()
		{
			if (( new string[]
			{
			} ).Join() != string.Empty)
			{
				throw new TestAssertionException();
			}

			if (( new string[]
			{
			} ).Join('@') != string.Empty)
			{
				throw new TestAssertionException();
			}

			if (( new string[]
			{
			} ).Join("..") != string.Empty)
			{
				throw new TestAssertionException();
			}

			if (( new[]
			{
				"A"
			} ).Join() != "A")
			{
				throw new TestAssertionException();
			}

			if (( new[]
			{
				"A"
			} ).Join('@') != "A")
			{
				throw new TestAssertionException();
			}

			if (( new[]
			{
				"A"
			} ).Join("..") != "A")
			{
				throw new TestAssertionException();
			}

			if (( new[]
			{
				"A", "B"
			} ).Join() != "AB")
			{
				throw new TestAssertionException();
			}

			if (( new[]
			{
				"A", "B"
			} ).Join('@') != "A@B")
			{
				throw new TestAssertionException();
			}

			if (( new[]
			{
				"A", "B"
			} ).Join("..") != "A..B")
			{
				throw new TestAssertionException();
			}

			if (( new[]
			{
				"A", "B", "CD"
			} ).Join() != "ABCD")
			{
				throw new TestAssertionException();
			}

			if (( new[]
			{
				"A", "B", "CD"
			} ).Join('@') != "A@B@CD")
			{
				throw new TestAssertionException();
			}

			if (( new[]
			{
				"A", "B", "CD"
			} ).Join("..") != "A..B..CD")
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

			if (string.Empty.Repeat(3, '@') != "")
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

			if (string.Empty.Repeat(3, "..") != "")
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
		}

		[TestMethod]
		public void SplitWhere_Test ()
		{
			string[] result = "".SplitWhere((s, c, p, n) => ( p == 3 ) || ( n == 5 ));

			if (result.Length != 1)
			{
				throw new TestAssertionException();
			}

			if (result[0] != "")
			{
				throw new TestAssertionException();
			}

			result = "abcd fghijklmnopqrstuvwxyz".SplitWhere((s, c, p, n) => ( p == 3 ) || ( n == 5 ) || ( n == 5 ));

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

			result = "abcd fghijklmnopqrstuvwxyz".SplitWhere(StringSplitOptions.RemoveEmptyEntries, (s, c, p, n) => ( p == 3 ) || ( n == 5 ) || ( n == 5 ));

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
		}

		#endregion
	}
}
