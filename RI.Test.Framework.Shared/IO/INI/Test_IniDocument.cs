﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.Collections.Comparison;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.IO.INI;
using RI.Framework.IO.INI.Elements;
using RI.Framework.IO.Paths;
using RI.Framework.Utilities;




namespace RI.Test.Framework.IO.INI
{
	[TestClass]
	public class Test_IniDocument
	{
		#region Constants

		private const string Data1 = "\nText\n;Comment\nName=Value\n[Header1]\n\nText1\n;Comment1\nName1=Value1";

		private const string Data10 = "H9N1=VA\nH9N1=VB\nH9N2=VC\nH9N2=VD\n[H3]\nH3N1=V1\nH3N1=V2\n[H4]\nH4N1=V3\nH4N1=V4\n[H3]\nH3N1=V5\nH3N2=V6\n[H4]\nH4N1=V7\nH4N2=V8";

		private const string Data11 = "H9N1=VA\nH9N1=VB\nH9N2=VC\nH9N2=VD\n[H3]\nH3N1=V1\nH3N1=V2\n[H3]\nH3N1=V5\nH3N2=V6\n[H4]\nH4N1=V3\nH4N1=V4\n[H4]\nH4N1=V7\nH4N2=V8";

		private const string Data12 = "H9N1=VA\nH9N1=VB\nH9N2=VC\nH9N2=VD\n[H3]\nH3N1=V1\nH3N1=V2\nH3N1=V5\nH3N2=V6\n[H4]\nH4N1=V3\nH4N1=V4\nH4N1=V7\nH4N2=V8";

		private const string Data13 = "H0N1=VA\nH0N2=VB\n[H1]\nH1N1=V1\nH1N2=V2\nH1N3=V5\nH1N4=V6";

		private const string Data14 = "H0N1=VA\nH0N2=VB\n[H1]\nH1N1=V1\nH1N2=V2\nH1N3=V5\nH1N4=V6\n[H2]";

		private const string Data15 = "H0N1=VA\nH0N2=VB\n[H1]\nH1N1=V1\nH1N2=V2\nH1N3=V5\nH1N4=V6\n[H2]\n;Comment1\n;Comment2";

		private const string Data16 = "H0N1=VA\nH0N2=VB\n[H1]\nH1N1=V1\nH1N2=V2\nH1N3=V5\nH1N4=V6\n[H2]\nText1\nText2";

		private const string Data17 = "H0N1=VA\nH0N2=VB\n[H1]\nH1N1=V1\nH1N2=V2\nH1N3=V5\nH1N4=V6\n[H2]\n;Comment1\n;Comment2\nText1\nText2";

		private const string Data18 = "H0N1=VA\nH0N2=VB\n[H1]\nH1N1=V1\nH1N2=V2\n[H2]\n[H1]\nH1N3=V5\nH1N4=V6\n[H2]";

		private const string Data19 = "H0N1=VA\nH0N2=VB\n[H1]\nH1N1=V11\nH1N2=V22\nH1N3=V55\nH1N4=V66\n[H2]\nH2N1=V3\nH2N2=V4\nH2N3=V7\nH2N4=V8";

		private const string Data2 = "\nText\n;Comment\nName=Value\n[Header1]\n\nText1\n;Comment1\nName1=Value1\n";

		private const string Data20 = "H0N1=VA\nH0N2=VB\n[H1]\nH1N1=V11\nH1N2=V22\n[H2]\nH2N1=V3\nH2N2=V4\n[H1]\nH1N3=V55\nH1N4=V66\n[H2]\nH2N3=V7\nH2N4=V8";

		private const string Data21 = "H0N1=VA\nH0N2=VB\n[H1]\nH1N1=V11\nH1N2=V22\nH1N3=V55\nH1N4=V66\n[H2]\nH2N1=V33\nH2N2=V44\nH2N3=V77\nH2N4=V88";

		private const string Data22 = "H0N1=VA\nH0N2=VB\n[H1]\nH1N2=V2\nH1N1=V1\n[H2]\nH2N2=V4\nH2N1=V3\n[H1]\nH1N4=V6\nH1N3=V5\n[H2]\nH2N4=V8\nH2N3=V7";

		private const string Data23 = "H0N1=VA\nH0N2=VB\n[H1]\nH1N1=V1\nH1N2=V2\n[H2]\nH2N2=V4\nH2N1=V3\n[H1]\nH1N3=V5\nH1N4=V6\n[H2]\nH2N4=V8\nH2N3=V7";

		private const string Data24 = "H0N1=VA\nH0N2=VB\n[H1]\nH1N2=V2\nH1N1=V1\n[H2]\nH2N1=V3\nH2N2=V4\n[H1]\nH1N4=V6\nH1N3=V5\n[H2]\nH2N3=V7\nH2N4=V8";

		private const string Data25 = "[H1]\nH1N1=V1\nH1N2=V2\nH1N3=V5\nH1N4=V6";

		private const string Data26 = "%!Name#Value%n%#\n(Header1%n%#)\n!!Comment";

		private const string Data27 = "!Name=Value|n#\n[Header1|n#]\n;!Comment";

		private const string Data28 = "|;Name=Value|n|=\n[Header1|n|=]\n;;Comment";

		private const string Data29 = ";Name#Value%n=\n(Header1%n=)\n!;Comment";

		private const string Data3 = "\nText\n;Comment\nName=Value\n[Header1]\n\nText1\n;Comment1\nName1=Value1\n\n";

		private const string Data4 = "";

		private const string Data5 = " ";

		private const string Data6 = "\nText\n;Comment\nName=Value\n\n[Header1]\n\nText1\n;Comment1\nName1=Value1";

		private const string Data7 = "H0N1=VA\nH0N2=VB\n[H1]\nH1N1=V1\nH1N2=V2\n[H2]\nH2N1=V3\nH2N2=V4\n[H1]\nH1N3=V5\nH1N4=V6\n[H2]\nH2N3=V7\nH2N4=V8";

		private const string Data8 = "H0N1=VA\nH0N2=VB\n[H1]\nH1N1=V1\nH1N2=V2\n[H1]\nH1N3=V5\nH1N4=V6\n[H2]\nH2N1=V3\nH2N2=V4\n[H2]\nH2N3=V7\nH2N4=V8";

		private const string Data9 = "H0N1=VA\nH0N2=VB\n[H1]\nH1N1=V1\nH1N2=V2\nH1N3=V5\nH1N4=V6\n[H2]\nH2N1=V3\nH2N2=V4\nH2N3=V7\nH2N4=V8";

		private static readonly IDictionary<string, string> H0 = new Dictionary<string, string>()
		{
			{"H0N1", "VA"},
			{"H0N2", "VB"},
		};

		private static readonly IDictionary<string, string> H1 = new Dictionary<string, string>()
		{
			{"H1N1", "V1"},
			{"H1N2", "V2"},
			{"H1N3", "V5"},
			{"H1N4", "V6"},
		};

		private static readonly IDictionary<string, string> H2 = new Dictionary<string, string>()
		{
			{"H2N1", "V3"},
			{"H2N2", "V4"},
			{"H2N3", "V7"},
			{"H2N4", "V8"},
		};

		private static readonly IDictionary<string, IList<string>> H3 = new Dictionary<string, IList<string>>()
		{
			{
				"H3N1", new List<string>
				{
					"V1",
					"V2",
					"V5"
				}
			},
			{
				"H3N2", new List<string>
				{
					"V6"
				}
			},
		};

		private static readonly IDictionary<string, IList<string>> H4 = new Dictionary<string, IList<string>>()
		{
			{
				"H4N1", new List<string>
				{
					"V3",
					"V4",
					"V7"
				}
			},
			{
				"H4N2", new List<string>
				{
					"V8"
				}
			},
		};

		private static readonly IDictionary<string, IList<string>> H9 = new Dictionary<string, IList<string>>()
		{
			{
				"H9N1", new List<string>
				{
					"VA",
					"VB"
				}
			},
			{
				"H9N2", new List<string>
				{
					"VC",
					"VD"
				}
			},
		};

		private static readonly IDictionary<string, string> V1 = new Dictionary<string, string>
		{
			{"H1N1", "V1"},
			{"H1N2", "V2"},
			{"H1N3", "V5"},
			{"H1N4", "V6"},
		};

		private static readonly IDictionary<string, IDictionary<string, string>> V12 = new Dictionary<string, IDictionary<string, string>>
		{
			{
				string.Empty, new Dictionary<string, string>
				{
					{"H0N1", "VA"},
					{"H0N2", "VB"},
				}
			},
			{
				"H1", new Dictionary<string, string>
				{
					{"H1N1", "V1"},
					{"H1N2", "V2"},
					{"H1N3", "V5"},
					{"H1N4", "V6"},
				}
			},
			{
				"H2", new Dictionary<string, string>
				{
					{"H2N1", "V3"},
					{"H2N2", "V4"},
					{"H2N3", "V7"},
					{"H2N4", "V8"},
				}
			}
		};

		private static readonly IDictionary<string, string> V2 = new Dictionary<string, string>
		{
			{"H2N1", "V3"},
			{"H2N2", "V4"},
			{"H2N3", "V7"},
			{"H2N4", "V8"},
		};

		private static readonly IDictionary<string, IDictionary<string, IList<string>>> V34 = new Dictionary<string, IDictionary<string, IList<string>>>
		{
			{string.Empty, Test_IniDocument.H9},
			{"H3", Test_IniDocument.H3},
			{"H4", Test_IniDocument.H4}
		};

		#endregion




		#region Instance Methods

		[TestMethod]
		public void Elements_Test ()
		{
			//------
			// Empty
			//------

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

			//-------------
			// Add elements
			//-------------

			test.Clear();
			test.AddText(string.Empty);
			test.AddText("Text");
			test.AddComment("Comment");
			test.AddValue("Name", "Value");
			test.AddSectionHeader("Header1");
			test.AddText(string.Empty);
			test.AddText("Text1");
			test.AddComment("Comment1");
			test.AddValue("Name1", "Value1");

			if (test.AsString(new IniWriterSettings()).Replace("\r", string.Empty) != Test_IniDocument.Data1)
			{
				throw new TestAssertionException();
			}

			test.Clear();
			test.AddText(string.Empty + Environment.NewLine + "Text");
			test.AddComment("Comment");
			test.AddValue("Name", "Value");
			test.AddSectionHeader("Header1");
			test.AddText(string.Empty + Environment.NewLine + "Text1");
			test.AddComment("Comment1");
			test.AddValue("Name1", "Value1");

			if (test.AsString(new IniWriterSettings()).Replace("\r", string.Empty) != Test_IniDocument.Data1)
			{
				throw new TestAssertionException();
			}

			//-----------------------------
			// Add sections (single values)
			//-----------------------------

			Dictionary<string, string> h0_1 = new Dictionary<string, string>()
			{
				{"H0N1", "VA"},
			};

			Dictionary<string, string> h0_2 = new Dictionary<string, string>()
			{
				{"H0N2", "VB"},
			};

			Dictionary<string, string> h1_1 = new Dictionary<string, string>()
			{
				{"H1N1", "V1"},
				{"H1N2", "V2"},
			};

			Dictionary<string, string> h1_2 = new Dictionary<string, string>()
			{
				{"H1N3", "V5"},
				{"H1N4", "V6"},
			};

			Dictionary<string, string> h2_1 = new Dictionary<string, string>()
			{
				{"H2N1", "V3"},
				{"H2N2", "V4"},
			};

			Dictionary<string, string> h2_2 = new Dictionary<string, string>()
			{
				{"H2N3", "V7"},
				{"H2N4", "V8"},
			};

			test.Clear();
			test.AddSection(null, IniSectionAddMode.AppendEnd, h0_1);
			test.AddSection("H1", IniSectionAddMode.AppendEnd, h1_1);
			test.AddSection("H2", IniSectionAddMode.AppendEnd, h2_1);
			test.AddSection("H1", IniSectionAddMode.AppendEnd, h1_2);
			test.AddSection("H2", IniSectionAddMode.AppendEnd, h2_2);
			test.AddSection(null, IniSectionAddMode.AppendEnd, h0_2);

			this.VerifyH0(test);
			this.VerifyH1(test);
			this.VerifyH2(test);

			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data7)
			{
				throw new TestAssertionException();
			}

			test.Clear();
			test.AddSection(null, IniSectionAddMode.AppendSame, h0_1);
			test.AddSection("H1", IniSectionAddMode.AppendSame, h1_1);
			test.AddSection("H2", IniSectionAddMode.AppendSame, h2_1);
			test.AddSection("H1", IniSectionAddMode.AppendSame, h1_2);
			test.AddSection("H2", IniSectionAddMode.AppendSame, h2_2);
			test.AddSection(null, IniSectionAddMode.AppendSame, h0_2);

			this.VerifyH0(test);
			this.VerifyH1(test);
			this.VerifyH2(test);

			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data8)
			{
				throw new TestAssertionException();
			}

			test.Clear();
			test.AddSection(null, IniSectionAddMode.AppendEnd, h0_1);
			test.AddSection(null, IniSectionAddMode.AppendEnd, h0_2);
			test.AddSection("H1", IniSectionAddMode.AppendEnd, h1_1);
			test.AddSection("H1", IniSectionAddMode.AppendEnd, h1_2);
			test.AddSection("H2", IniSectionAddMode.AppendEnd, h2_1);
			test.AddSection("H2", IniSectionAddMode.AppendEnd, h2_2);

			this.VerifyH0(test);
			this.VerifyH1(test);
			this.VerifyH2(test);

			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data8)
			{
				throw new TestAssertionException();
			}

			test.Clear();
			test.AddSection(null, IniSectionAddMode.AppendSame, h0_1);
			test.AddSection(null, IniSectionAddMode.AppendSame, h0_2);
			test.AddSection("H1", IniSectionAddMode.AppendSame, h1_1);
			test.AddSection("H1", IniSectionAddMode.AppendSame, h1_2);
			test.AddSection("H2", IniSectionAddMode.AppendSame, h2_1);
			test.AddSection("H2", IniSectionAddMode.AppendSame, h2_2);

			this.VerifyH0(test);
			this.VerifyH1(test);
			this.VerifyH2(test);

			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data8)
			{
				throw new TestAssertionException();
			}

			test.Clear();
			test.AddSection(null, IniSectionAddMode.AppendSame, h0_1);
			test.AddSection("H1", IniSectionAddMode.MergeSame, h1_1);
			test.AddSection("H2", IniSectionAddMode.MergeSame, h2_1);
			test.AddSection("H1", IniSectionAddMode.MergeSame, h1_2);
			test.AddSection("H2", IniSectionAddMode.MergeSame, h2_2);
			test.AddSection(null, IniSectionAddMode.AppendSame, h0_2);

			this.VerifyH0(test);
			this.VerifyH1(test);
			this.VerifyH2(test);

			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data9)
			{
				throw new TestAssertionException();
			}

			test.Clear();
			test.AddSection(null, IniSectionAddMode.AppendSame, h0_1);
			test.AddSection(null, IniSectionAddMode.AppendSame, h0_2);
			test.AddSection("H1", IniSectionAddMode.MergeSame, h1_1);
			test.AddSection("H1", IniSectionAddMode.MergeSame, h1_2);
			test.AddSection("H2", IniSectionAddMode.MergeSame, h2_1);
			test.AddSection("H2", IniSectionAddMode.MergeSame, h2_2);

			this.VerifyH0(test);
			this.VerifyH1(test);
			this.VerifyH2(test);

			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data9)
			{
				throw new TestAssertionException();
			}

			//-------------------------------
			// Add sections (multiple values)
			//-------------------------------

			Dictionary<string, IList<string>> h9_1 = new Dictionary<string, IList<string>>()
			{
				{
					"H9N1", new List<string>
					{
						"VA",
						"VB"
					}
				},
			};

			Dictionary<string, IList<string>> h9_2 = new Dictionary<string, IList<string>>()
			{
				{
					"H9N2", new List<string>
					{
						"VC",
						"VD"
					}
				},
			};

			Dictionary<string, IList<string>> h3_1 = new Dictionary<string, IList<string>>()
			{
				{
					"H3N1", new List<string>
					{
						"V1",
						"V2"
					}
				},
			};

			Dictionary<string, IList<string>> h3_2 = new Dictionary<string, IList<string>>()
			{
				{
					"H3N1", new List<string>
					{
						"V5"
					}
				},
				{
					"H3N2", new List<string>
					{
						"V6"
					}
				},
			};

			Dictionary<string, IList<string>> h4_1 = new Dictionary<string, IList<string>>()
			{
				{
					"H4N1", new List<string>
					{
						"V3",
						"V4"
					}
				},
			};

			Dictionary<string, IList<string>> h4_2 = new Dictionary<string, IList<string>>()
			{
				{
					"H4N1", new List<string>
					{
						"V7"
					}
				},
				{
					"H4N2", new List<string>
					{
						"V8"
					}
				},
			};

			test.Clear();
			test.AddSection(null, IniSectionAddMode.AppendEnd, h9_1);
			test.AddSection("H3", IniSectionAddMode.AppendEnd, h3_1);
			test.AddSection("H4", IniSectionAddMode.AppendEnd, h4_1);
			test.AddSection("H3", IniSectionAddMode.AppendEnd, h3_2);
			test.AddSection("H4", IniSectionAddMode.AppendEnd, h4_2);
			test.AddSection(null, IniSectionAddMode.AppendEnd, h9_2);

			this.VerifyH9(test);
			this.VerifyH3(test);
			this.VerifyH4(test);

			string temp = test.AsString().Replace("\r", string.Empty);
			if (temp != Test_IniDocument.Data10)
			{
				throw new TestAssertionException();
			}

			test.Clear();
			test.AddSection(null, IniSectionAddMode.AppendSame, h9_1);
			test.AddSection("H3", IniSectionAddMode.AppendSame, h3_1);
			test.AddSection("H4", IniSectionAddMode.AppendSame, h4_1);
			test.AddSection("H3", IniSectionAddMode.AppendSame, h3_2);
			test.AddSection("H4", IniSectionAddMode.AppendSame, h4_2);
			test.AddSection(null, IniSectionAddMode.AppendSame, h9_2);

			this.VerifyH9(test);
			this.VerifyH3(test);
			this.VerifyH4(test);

			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data11)
			{
				throw new TestAssertionException();
			}

			test.Clear();
			test.AddSection(null, IniSectionAddMode.AppendEnd, h9_1);
			test.AddSection(null, IniSectionAddMode.AppendEnd, h9_2);
			test.AddSection("H3", IniSectionAddMode.AppendEnd, h3_1);
			test.AddSection("H3", IniSectionAddMode.AppendEnd, h3_2);
			test.AddSection("H4", IniSectionAddMode.AppendEnd, h4_1);
			test.AddSection("H4", IniSectionAddMode.AppendEnd, h4_2);

			this.VerifyH9(test);
			this.VerifyH3(test);
			this.VerifyH4(test);

			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data11)
			{
				throw new TestAssertionException();
			}

			test.Clear();
			test.AddSection(null, IniSectionAddMode.AppendSame, h9_1);
			test.AddSection(null, IniSectionAddMode.AppendSame, h9_2);
			test.AddSection("H3", IniSectionAddMode.AppendSame, h3_1);
			test.AddSection("H3", IniSectionAddMode.AppendSame, h3_2);
			test.AddSection("H4", IniSectionAddMode.AppendSame, h4_1);
			test.AddSection("H4", IniSectionAddMode.AppendSame, h4_2);

			this.VerifyH9(test);
			this.VerifyH3(test);
			this.VerifyH4(test);

			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data11)
			{
				throw new TestAssertionException();
			}

			test.Clear();
			test.AddSection(null, IniSectionAddMode.MergeSame, h9_1);
			test.AddSection("H3", IniSectionAddMode.MergeSame, h3_1);
			test.AddSection("H4", IniSectionAddMode.MergeSame, h4_1);
			test.AddSection("H3", IniSectionAddMode.MergeSame, h3_2);
			test.AddSection("H4", IniSectionAddMode.MergeSame, h4_2);
			test.AddSection(null, IniSectionAddMode.MergeSame, h9_2);

			this.VerifyH9(test);
			this.VerifyH3(test);
			this.VerifyH4(test);

			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data12)
			{
				throw new TestAssertionException();
			}

			test.Clear();
			test.AddSection(null, IniSectionAddMode.MergeSame, h9_1);
			test.AddSection(null, IniSectionAddMode.MergeSame, h9_2);
			test.AddSection("H3", IniSectionAddMode.MergeSame, h3_1);
			test.AddSection("H3", IniSectionAddMode.MergeSame, h3_2);
			test.AddSection("H4", IniSectionAddMode.MergeSame, h4_1);
			test.AddSection("H4", IniSectionAddMode.MergeSame, h4_2);

			this.VerifyH9(test);
			this.VerifyH3(test);
			this.VerifyH4(test);

			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data12)
			{
				throw new TestAssertionException();
			}

			//----------------
			// Remove sections
			//----------------

			test.Load(Test_IniDocument.Data9);
			test.RemoveSections("H2");
			this.VerifyH0(test);
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data13)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data13);
			test.RemoveSections("H2");
			test.RemoveSections("H3");
			test.RemoveSections("H4");
			this.VerifyH0(test);
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data13)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data13);
			test.RemoveSections(null);
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data25)
			{
				throw new TestAssertionException();
			}

			//----------------------
			// Remove empty sections
			//----------------------

			test.Load(Test_IniDocument.Data14);
			test.RemoveEmptySections();
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data13)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data14);
			test.RemoveEmptySections(false, false);
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data13)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data14);
			test.RemoveEmptySections(false, true);
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data13)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data14);
			test.RemoveEmptySections(true, false);
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data13)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data14);
			test.RemoveEmptySections(true, true);
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data13)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data15);
			test.RemoveEmptySections();
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data13)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data15);
			test.RemoveEmptySections(false, false);
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data13)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data15);
			test.RemoveEmptySections(false, true);
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data15)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data15);
			test.RemoveEmptySections(true, false);
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data13)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data15);
			test.RemoveEmptySections(true, true);
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data15)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data16);
			test.RemoveEmptySections();
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data13)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data16);
			test.RemoveEmptySections(false, false);
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data13)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data16);
			test.RemoveEmptySections(false, true);
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data13)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data16);
			test.RemoveEmptySections(true, false);
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data16)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data16);
			test.RemoveEmptySections(true, true);
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data16)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data17);
			test.RemoveEmptySections();
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data13)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data17);
			test.RemoveEmptySections(false, false);
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data13)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data17);
			test.RemoveEmptySections(false, true);
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data17)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data17);
			test.RemoveEmptySections(true, false);
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data17)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data17);
			test.RemoveEmptySections(true, true);
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data17)
			{
				throw new TestAssertionException();
			}

			//-------------
			// Get sections
			//-------------

			test.Load(Test_IniDocument.Data9);

			if (!this.VerifySection(new List<Dictionary<string, string>>
			{
				test.GetSection(null)
			}, Test_IniDocument.H0.ToList()))
			{
				throw new TestAssertionException();
			}

			if (!this.VerifySection(new List<Dictionary<string, string>>
			{
				test.GetSection("H1")
			}, Test_IniDocument.H1.ToList()))
			{
				throw new TestAssertionException();
			}

			if (!this.VerifySection(new List<Dictionary<string, string>>
			{
				test.GetSection("H2")
			}, Test_IniDocument.H2.ToList()))
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data12);

			if (!this.VerifySection(new List<Dictionary<string, List<string>>>
			{
				test.GetSectionAll(null)
			}, Test_IniDocument.H9.ToList()))
			{
				throw new TestAssertionException();
			}

			if (!this.VerifySection(new List<Dictionary<string, List<string>>>
			{
				test.GetSectionAll("H3")
			}, Test_IniDocument.H3.ToList()))
			{
				throw new TestAssertionException();
			}

			if (!this.VerifySection(new List<Dictionary<string, List<string>>>
			{
				test.GetSectionAll("H4")
			}, Test_IniDocument.H4.ToList()))
			{
				throw new TestAssertionException();
			}

			//---------------
			// Merge sections
			//---------------

			test.Load(Test_IniDocument.Data7);
			test.RemoveSections("H2");
			test.MergeSections();
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data13)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data7);
			test.MergeSections();
			this.VerifyH1(test);
			this.VerifyH2(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data9)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data18);
			test.RemoveEmptySections(false, false);
			test.MergeSections();
			this.VerifyH1(test);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data13)
			{
				throw new TestAssertionException();
			}

			//-----------------------------
			// Removal of text and comments
			//-----------------------------

			test.Load(Test_IniDocument.Data17);
			test.RemoveComments();
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data16)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data17);
			test.RemoveText();
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data15)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data17);
			test.RemoveTextAndComments();
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data14)
			{
				throw new TestAssertionException();
			}

			//-----------
			// Set values
			//-----------

			test.Clear();

			test.SetValue(null, "H0N1", "VA");
			test.SetValue(null, "H0N2", "VB");
			test.SetValue("H1", "H1N1", "V1");
			test.SetValue("H1", "H1N2", "V2");
			test.SetValue("H1", "H1N3", "V5");
			test.SetValue("H1", "H1N4", "V6");
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data13)
			{
				throw new TestAssertionException();
			}

			test.SetValue(null, "H0N1", "VA");
			test.SetValue(null, "H0N2", "VB");
			test.SetValue("H2", "H2N1", "V3");
			test.SetValue("H2", "H2N2", "V4");
			test.SetValue("H2", "H2N3", "V7");
			test.SetValue("H2", "H2N4", "V8");
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data9)
			{
				throw new TestAssertionException();
			}

			test.SetValue("H1", "H1N1", "V11");
			test.SetValue("H1", "H1N2", "V22");
			test.SetValue("H1", "H1N3", "V55");
			test.SetValue("H1", "H1N4", "V66");
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data19)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data7);
			test.SetValue("H1", "H1N1", "V11");
			test.SetValue("H1", "H1N2", "V22");
			test.SetValue("H1", "H1N3", "V55");
			test.SetValue("H1", "H1N4", "V66");
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data20)
			{
				throw new TestAssertionException();
			}

			Dictionary<string, string> v1 = new Dictionary<string, string>
			{
				{"H1N1", "V1"},
				{"H1N2", "V2"},
				{"H1N3", "V5"},
				{"H1N4", "V6"},
			};

			test.SetValues("H1", v1);
			test.MergeSections();
			test.SortSections();
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data9)
			{
				throw new TestAssertionException();
			}

			Dictionary<string, IDictionary<string, string>> v2 = new Dictionary<string, IDictionary<string, string>>
			{
				{
					"H1", new Dictionary<string, string>
					{
						{"H1N1", "V11"},
						{"H1N2", "V22"},
						{"H1N3", "V55"},
						{"H1N4", "V66"},
					}
				},
				{
					"H2", new Dictionary<string, string>
					{
						{"H2N1", "V33"},
						{"H2N2", "V44"},
						{"H2N3", "V77"},
						{"H2N4", "V88"},
					}
				}
			};

			test.SetValues(v2);
			test.MergeSections();
			test.SortSections();
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data21)
			{
				throw new TestAssertionException();
			}

			//-----------
			// Get values
			//-----------

			test.Load(Test_IniDocument.Data7);
			if (test.GetValue(null, "Test") != null)
			{
				throw new TestAssertionException();
			}
			if (test.GetValue("H1", "Test") != null)
			{
				throw new TestAssertionException();
			}
			if (test.GetValue("Test", "H1N1") != null)
			{
				throw new TestAssertionException();
			}
			if (test.GetValue(null, "H0N1") != "VA")
			{
				throw new TestAssertionException();
			}
			if (test.GetValue(null, "H0N2") != "VB")
			{
				throw new TestAssertionException();
			}
			if (test.GetValue("H1", "H1N1") != "V1")
			{
				throw new TestAssertionException();
			}
			if (test.GetValue("H1", "H1N2") != "V2")
			{
				throw new TestAssertionException();
			}
			if (test.GetValue("H1", "H1N3") != "V5")
			{
				throw new TestAssertionException();
			}
			if (test.GetValue("H1", "H1N4") != "V6")
			{
				throw new TestAssertionException();
			}
			if (test.GetValue("H2", "H2N1") != "V3")
			{
				throw new TestAssertionException();
			}
			if (test.GetValue("H2", "H2N2") != "V4")
			{
				throw new TestAssertionException();
			}
			if (test.GetValue("H2", "H2N3") != "V7")
			{
				throw new TestAssertionException();
			}
			if (test.GetValue("H2", "H2N4") != "V8")
			{
				throw new TestAssertionException();
			}

			if (!this.VerifySection(test.GetValues(), Test_IniDocument.V12))
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data10);
			if (!this.VerifySection(test.GetValuesAll(), Test_IniDocument.V34))
			{
				throw new TestAssertionException();
			}

			//--------------
			// Delete values
			//--------------

			test.Load(Test_IniDocument.Data7);

			if (test.DeleteValue(null, "H1N1"))
			{
				throw new TestAssertionException();
			}

			if (test.DeleteValue("Test", "H1N1"))
			{
				throw new TestAssertionException();
			}

			if (test.DeleteValue("H1", "Test"))
			{
				throw new TestAssertionException();
			}

			if (!test.DeleteValue(null, "H0N1"))
			{
				throw new TestAssertionException();
			}

			if (!test.DeleteValue(null, "H0N2"))
			{
				throw new TestAssertionException();
			}

			if (!test.DeleteValue("H2", "H2N1"))
			{
				throw new TestAssertionException();
			}

			if (!test.DeleteValue("H2", "H2N2"))
			{
				throw new TestAssertionException();
			}

			if (!test.DeleteValue("H2", "H2N3"))
			{
				throw new TestAssertionException();
			}

			if (!test.DeleteValue("H2", "H2N4"))
			{
				throw new TestAssertionException();
			}

			test.RemoveEmptySections(false, false);
			test.MergeSections();

			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data25)
			{
				throw new TestAssertionException();
			}

			//----------------
			// Sorting regions
			//----------------

			test.Load(Test_IniDocument.Data7);
			test.SortSections();
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data8)
			{
				throw new TestAssertionException();
			}

			//-----------------
			// Sorting Elements
			//-----------------

			test.Load(Test_IniDocument.Data22);
			test.SortElements();
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data7)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data22);
			test.SortElements("H1");
			test.SortElements("H2");
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data7)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data22);
			test.SortElements("H1");
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data23)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data22);
			test.SortElements("H2");
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data24)
			{
				throw new TestAssertionException();
			}
		}

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
		public void Settings_Test ()
		{
			IniDocument test = new IniDocument();

			//----------------
			// Writer settings
			//----------------

			test.Load(Test_IniDocument.Data1);
			if (test.AsString(new IniWriterSettings
			{
				EmptyLineBeforeSectionHeader = false
			}).Replace("\r", string.Empty) != Test_IniDocument.Data1)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data1);
			if (test.AsString(new IniWriterSettings
			{
				EmptyLineBeforeSectionHeader = true
			}).Replace("\r", string.Empty) != Test_IniDocument.Data6)
			{
				throw new TestAssertionException();
			}

			//-----------------
			// General settings
			//-----------------

			IniReaderSettings s1 = new IniReaderSettings
			{
				CommentStart = '!',
				EscapeCharacter = '%',
				NameValueSeparator = '#',
				SectionEnd = ')',
				SectionStart = '('
			};

			IniWriterSettings s2 = new IniWriterSettings
			{
				CommentStart = '!',
				EscapeCharacter = '%',
				NameValueSeparator = '#',
				SectionEnd = ')',
				SectionStart = '('
			};

			test.Load(Test_IniDocument.Data26, s1);
			if (test.AsString().Replace("\r", string.Empty) != Test_IniDocument.Data27)
			{
				throw new TestAssertionException();
			}

			test.Load(Test_IniDocument.Data28);
			string temp = test.AsString(s2).Replace("\r", string.Empty);
			if (temp != Test_IniDocument.Data29)
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

			if (((TextIniElement)document.Elements[0]).Text != (Environment.NewLine + "Text"))
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

			if (((TextIniElement)document.Elements[4]).Text != (Environment.NewLine + "Text1"))
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

			if (((TextIniElement)document.Elements[0]).Text != (Environment.NewLine + "Text"))
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

			if (((TextIniElement)document.Elements[4]).Text != (Environment.NewLine + "Text1"))
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

			if (((TextIniElement)document.Elements[0]).Text != (Environment.NewLine + "Text"))
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

			if (((TextIniElement)document.Elements[4]).Text != (Environment.NewLine + "Text1"))
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

		private void VerifyData4 (IniDocument document)
		{
			if (document.Count != 0)
			{
				throw new TestAssertionException();
			}
		}

		private void VerifyData5 (IniDocument document)
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

		private void VerifyData6 (IniDocument document)
		{
			if (document.Count != 8)
			{
				throw new TestAssertionException();
			}

			if (((TextIniElement)document.Elements[0]).Text != (Environment.NewLine + "Text"))
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

			if (((TextIniElement)document.Elements[3]).Text != string.Empty)
			{
				throw new TestAssertionException();
			}

			if (((SectionIniElement)document.Elements[4]).SectionName != "Header1")
			{
				throw new TestAssertionException();
			}

			if (((TextIniElement)document.Elements[5]).Text != (Environment.NewLine + "Text1"))
			{
				throw new TestAssertionException();
			}

			if (((CommentIniElement)document.Elements[6]).Comment != "Comment1")
			{
				throw new TestAssertionException();
			}

			if (((ValueIniElement)document.Elements[7]).Name != "Name1")
			{
				throw new TestAssertionException();
			}

			if (((ValueIniElement)document.Elements[6]).Value != "Value1")
			{
				throw new TestAssertionException();
			}
		}

		private void VerifyH0 (IniDocument document)
		{
			List<Dictionary<string, string>> section = document.GetSections(null).ToList();
			List<KeyValuePair<string, string>> h0 = Test_IniDocument.H0.ToList();

			if (!this.VerifySection(section, h0))
			{
				throw new TestAssertionException();
			}
		}

		private void VerifyH1 (IniDocument document)
		{
			List<Dictionary<string, string>> section = document.GetSections("H1").ToList();
			List<KeyValuePair<string, string>> h1 = Test_IniDocument.H1.ToList();

			if (!this.VerifySection(section, h1))
			{
				throw new TestAssertionException();
			}
		}

		private void VerifyH2 (IniDocument document)
		{
			List<Dictionary<string, string>> section = document.GetSections("H2").ToList();
			List<KeyValuePair<string, string>> h2 = Test_IniDocument.H2.ToList();

			if (!this.VerifySection(section, h2))
			{
				throw new TestAssertionException();
			}
		}

		private void VerifyH3 (IniDocument document)
		{
			List<Dictionary<string, List<string>>> section = document.GetSectionsAll("H3").ToList();
			List<KeyValuePair<string, IList<string>>> h3 = Test_IniDocument.H3.ToList();

			if (!this.VerifySection(section, h3))
			{
				throw new TestAssertionException();
			}
		}

		private void VerifyH4 (IniDocument document)
		{
			List<Dictionary<string, List<string>>> section = document.GetSectionsAll("H4").ToList();
			List<KeyValuePair<string, IList<string>>> h4 = Test_IniDocument.H4.ToList();

			if (!this.VerifySection(section, h4))
			{
				throw new TestAssertionException();
			}
		}

		private void VerifyH9 (IniDocument document)
		{
			List<Dictionary<string, List<string>>> section = document.GetSectionsAll(null).ToList();
			List<KeyValuePair<string, IList<string>>> h9 = Test_IniDocument.H9.ToList();

			if (!this.VerifySection(section, h9))
			{
				throw new TestAssertionException();
			}
		}

		private bool VerifySection (List<Dictionary<string, string>> sections, List<KeyValuePair<string, string>> elements)
		{
			foreach (KeyValuePair<string, string> element in elements)
			{
				bool found = false;
				foreach (Dictionary<string, string> section in sections)
				{
					if (!section.ContainsKey(element.Key))
					{
						continue;
					}

					if (!string.Equals(section[element.Key], element.Value, StringComparison.Ordinal))
					{
						continue;
					}

					found = true;
				}

				if (!found)
				{
					return false;
				}
			}

			return true;
		}

		private bool VerifySection (List<Dictionary<string, List<string>>> sections, List<KeyValuePair<string, IList<string>>> elements)
		{
			CollectionComparer<string> comparer = new CollectionComparer<string>((x, y) => string.Equals(x, y, StringComparison.Ordinal));

			foreach (KeyValuePair<string, IList<string>> element in elements)
			{
				bool found = false;
				foreach (Dictionary<string, List<string>> section in sections)
				{
					if (!section.ContainsKey(element.Key))
					{
						continue;
					}

					foreach (string value in section[element.Key])
					{
						if (element.Value.Contains(value, StringComparerEx.Ordinal))
						{
							found = true;
						}
					}
				}

				if (!found)
				{
					return false;
				}
			}

			return true;
		}

		private bool VerifySection (Dictionary<string, Dictionary<string, string>> sections, IDictionary<string, IDictionary<string, string>> elements)
		{
			foreach (KeyValuePair<string, IDictionary<string, string>> elementSection in elements)
			{
				if (!sections.ContainsKey(elementSection.Key))
				{
					return false;
				}
				foreach (KeyValuePair<string, string> elementValue in elementSection.Value)
				{
					if (!sections[elementSection.Key].ContainsKey(elementValue.Key))
					{
						return false;
					}
					if (!string.Equals(sections[elementSection.Key][elementValue.Key], elementValue.Value, StringComparison.Ordinal))
					{
						return false;
					}
				}
			}

			foreach (KeyValuePair<string, Dictionary<string, string>> section in sections)
			{
				if (!elements.ContainsKey(section.Key))
				{
					return false;
				}
				foreach (KeyValuePair<string, string> value in section.Value)
				{
					if (!elements[section.Key].ContainsKey(value.Key))
					{
						return false;
					}
					if (!string.Equals(elements[section.Key][value.Key], value.Value, StringComparison.Ordinal))
					{
						return false;
					}
				}
			}

			return true;
		}

		private bool VerifySection (Dictionary<string, Dictionary<string, List<string>>> sections, IDictionary<string, IDictionary<string, IList<string>>> elements)
		{
			foreach (KeyValuePair<string, IDictionary<string, IList<string>>> elementSection in elements)
			{
				if (!sections.ContainsKey(elementSection.Key))
				{
					return false;
				}
				foreach (KeyValuePair<string, IList<string>> elementValue in elementSection.Value)
				{
					if (!sections[elementSection.Key].ContainsKey(elementValue.Key))
					{
						return false;
					}
					if (!sections[elementSection.Key][elementValue.Key].SequenceEqual(elementValue.Value, CollectionComparerFlags.None, StringComparerEx.Ordinal))
					{
						return false;
					}
				}
			}

			foreach (KeyValuePair<string, Dictionary<string, List<string>>> section in sections)
			{
				if (!elements.ContainsKey(section.Key))
				{
					return false;
				}
				foreach (KeyValuePair<string, List<string>> value in section.Value)
				{
					if (!elements[section.Key].ContainsKey(value.Key))
					{
						return false;
					}
					if (!elements[section.Key][value.Key].SequenceEqual(value.Value, CollectionComparerFlags.None, StringComparerEx.Ordinal))
					{
						return false;
					}
				}
			}

			return true;
		}

		#endregion
	}
}
