using System;
using System.Collections.Generic;

using RI.Framework.Utilities.Text;

#if PLATFORM_NET
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
#if PLATFORM_UNITY
using RI.Test.Framework;
#endif




namespace RI.Test.Framework.Utilities.Text
{
	[TestClass]
	public sealed class Test_CommandLine
	{
		#region Instance Methods

		[TestMethod]
		public void Parse_Test()
		{
			//--------
			// Nothing
			//--------

			CommandLine test = CommandLine.Parse("", true);

			if (test.Executable != null)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters.Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test.Literals.Count != 0)
			{
				throw new TestAssertionException();
			}

			test = CommandLine.Parse("", false);

			if (test.Executable != null)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters.Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test.Literals.Count != 0)
			{
				throw new TestAssertionException();
			}

			//---------------------------------------------------
			// Executable, parameter, literal (executable parsed)
			//---------------------------------------------------

			test = CommandLine.Parse("test.exe -n1=v1 literal1", true);

			if (test.Executable != "test.exe")
			{
				throw new TestAssertionException();
			}

			if (test.Parameters.Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"].Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"][0] != "v1")
			{
				throw new TestAssertionException();
			}

			if (test.Literals.Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Literals[0] != "literal1")
			{
				throw new TestAssertionException();
			}

			//----------------------------------------------------
			// Executable, parameter, literal (executable ignored)
			//----------------------------------------------------

			test = CommandLine.Parse("test.exe -n1=v1 literal1", false);

			if (test.Executable != null)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters.Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"].Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"][0] != "v1")
			{
				throw new TestAssertionException();
			}

			if (test.Literals.Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Literals[0] != "test.exe")
			{
				throw new TestAssertionException();
			}

			if (test.Literals[1] != "literal1")
			{
				throw new TestAssertionException();
			}

			//---------------------------------------
			// Parameter, literal (executable parsed)
			//---------------------------------------

			test = CommandLine.Parse("-n1=v1 literal1", true);

			if (test.Executable != null)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters.Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"].Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"][0] != "v1")
			{
				throw new TestAssertionException();
			}

			if (test.Literals.Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Literals[0] != "literal1")
			{
				throw new TestAssertionException();
			}

			test = CommandLine.Parse("literal1 -n1=v1", true);

			if (test.Executable != "literal1")
			{
				throw new TestAssertionException();
			}

			if (test.Parameters.Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"].Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"][0] != "v1")
			{
				throw new TestAssertionException();
			}

			if (test.Literals.Count != 0)
			{
				throw new TestAssertionException();
			}

			//----------------------------------------
			// Parameter, literal (executable ignored)
			//----------------------------------------

			test = CommandLine.Parse("-n1=v1 literal1", false);

			if (test.Executable != null)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters.Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"].Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"][0] != "v1")
			{
				throw new TestAssertionException();
			}

			if (test.Literals.Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Literals[0] != "literal1")
			{
				throw new TestAssertionException();
			}
			
			//------------------------------
			// Mixed parameters and literals
			//------------------------------

			test = CommandLine.Parse("test.exe -n1=v1 literal1 -n2=v2 literal2", true);

			if (test.Executable != "test.exe")
			{
				throw new TestAssertionException();
			}

			if (test.Parameters.Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"].Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"][0] != "v1")
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n2"].Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n2"][0] != "v2")
			{
				throw new TestAssertionException();
			}

			if (test.Literals.Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Literals[0] != "literal1")
			{
				throw new TestAssertionException();
			}

			if (test.Literals[1] != "literal2")
			{
				throw new TestAssertionException();
			}

			//---------------------------------
			// Multiple parameters and literals
			//---------------------------------

			test = CommandLine.Parse("-n1=v1 -n2=v2 literal1 literal2", false);

			if (test.Executable != null)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters.Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"].Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"][0] != "v1")
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n2"].Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n2"][0] != "v2")
			{
				throw new TestAssertionException();
			}

			if (test.Literals.Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Literals[0] != "literal1")
			{
				throw new TestAssertionException();
			}

			if (test.Literals[1] != "literal2")
			{
				throw new TestAssertionException();
			}

			//----------------
			// Executable only
			//----------------

			test = CommandLine.Parse("test.exe", true);

			if (test.Executable != "test.exe")
			{
				throw new TestAssertionException();
			}

			if (test.Parameters.Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test.Literals.Count != 0)
			{
				throw new TestAssertionException();
			}

			test = CommandLine.Parse("test.exe", false);

			if (test.Executable != null)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters.Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test.Literals.Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Literals[0] != "test.exe")
			{
				throw new TestAssertionException();
			}

			//----------------
			// Parameters only
			//----------------

			test = CommandLine.Parse("-n1=v1 -n2=v2", false);

			if (test.Executable != null)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters.Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"].Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"][0] != "v1")
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n2"].Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n2"][0] != "v2")
			{
				throw new TestAssertionException();
			}

			if (test.Literals.Count != 0)
			{
				throw new TestAssertionException();
			}

			//--------------
			// Literals only
			//--------------

			test = CommandLine.Parse("literal1 literal2", false);

			if (test.Executable != null)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters.Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test.Literals.Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Literals[0] != "literal1")
			{
				throw new TestAssertionException();
			}

			if (test.Literals[1] != "literal2")
			{
				throw new TestAssertionException();
			}

			//-----------------------------
			// Same parameters and literals
			//-----------------------------

			test = CommandLine.Parse("-n1=v1 -n1=v2 literal1 literal1", false);

			if (test.Executable != null)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters.Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"].Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"][0] != "v1")
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"][1] != "v2")
			{
				throw new TestAssertionException();
			}

			if (test.Literals.Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Literals[0] != "literal1")
			{
				throw new TestAssertionException();
			}

			if (test.Literals[1] != "literal1")
			{
				throw new TestAssertionException();
			}

			//----------------------------
			// With whitespaces and quotes
			//----------------------------

			test = CommandLine.Parse("\"dir \\\" exe\" -n1=\"v1 v2 v3\" -n1=\"\" -n1= -n1 \"lit 1\" -\"name 1\"=\"value \\\" 1\" \"lit \\\"\" lit 3", true);

			if (test.Executable != "dir \" exe")
			{
				throw new TestAssertionException();
			}

			if (test.Parameters.Count != 2)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"].Count != 4)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"][0] != "v1 v2 v3")
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"][1] != "")
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"][2] != "")
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"][3] != "")
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["name 1"].Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["name 1"][0] != "value \" 1")
			{
				throw new TestAssertionException();
			}

			if (test.Literals.Count != 4)
			{
				throw new TestAssertionException();
			}

			if (test.Literals[0] != "lit 1")
			{
				throw new TestAssertionException();
			}

			if (test.Literals[1] != "lit \"")
			{
				throw new TestAssertionException();
			}

			if (test.Literals[2] != "lit")
			{
				throw new TestAssertionException();
			}

			if (test.Literals[3] != "3")
			{
				throw new TestAssertionException();
			}

			//-------------------------------------
			// Premature end of string (executable)
			//-------------------------------------

			test = CommandLine.Parse("\"test", true);

			if (test.Executable != "test")
			{
				throw new TestAssertionException();
			}

			if (test.Parameters.Count != 0)
			{
				throw new TestAssertionException();
			}

			if (test.Literals.Count != 0)
			{
				throw new TestAssertionException();
			}

			//------------------------------------
			// Premature end of string (parameter)
			//------------------------------------

			test = CommandLine.Parse("\"test.exe\" -n1=\"v1", true);

			if (test.Executable != "test.exe")
			{
				throw new TestAssertionException();
			}

			if (test.Parameters.Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"].Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"][0] != "v1")
			{
				throw new TestAssertionException();
			}

			if (test.Literals.Count != 0)
			{
				throw new TestAssertionException();
			}

			//----------------------------------
			// Premature end of string (literal)
			//----------------------------------

			test = CommandLine.Parse("\"test.exe\" -n1=\"v1\" \"literal1", true);

			if (test.Executable != "test.exe")
			{
				throw new TestAssertionException();
			}

			if (test.Parameters.Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"].Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Parameters["n1"][0] != "v1")
			{
				throw new TestAssertionException();
			}

			if (test.Literals.Count != 1)
			{
				throw new TestAssertionException();
			}

			if (test.Literals[0] != "literal1")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Build_Test ()
		{
			//--------
			// Nothing
			//--------

			CommandLine test = new CommandLine();

			if (test.ToString() != "")
			{
				throw new TestAssertionException();
			}

			//----------------
			// Executable only
			//----------------

			test = new CommandLine();

			test.Executable = "test.exe";
			if (test.ToString() != "test.exe")
			{
				throw new TestAssertionException();
			}

			test.Executable = "test exe";
			if (test.ToString() != "\"test exe\"")
			{
				throw new TestAssertionException();
			}

			test.Executable = "test\"exe";
			if (test.ToString() != "\"test\\\"exe\"")
			{
				throw new TestAssertionException();
			}

			//----------------
			// Parameters only
			//----------------

			test = new CommandLine();

			test.Parameters.Add("n1", new List<string>
			                    {
				                    "v1", "v2"
			                    });
			if (test.ToString() != "-n1=v1 -n1=v2")
			{
				throw new TestAssertionException();
			}

			test.Parameters.Add("n 1", new List<string>
								{
									"v 1", "v 2"
								});
			if (test.ToString() != "-n1=v1 -n1=v2 -\"n 1\"=\"v 1\" -\"n 1\"=\"v 2\"")
			{
				throw new TestAssertionException();
			}

			test.Parameters.Add("n\"1", new List<string>
								{
									"v\"1", "v\"2"
								});
			if (test.ToString() != "-n1=v1 -n1=v2 -\"n 1\"=\"v 1\" -\"n 1\"=\"v 2\" -\"n\\\"1\"=\"v\\\"1\" -\"n\\\"1\"=\"v\\\"2\"")
			{
				throw new TestAssertionException();
			}

			//--------------
			// Literals only
			//--------------

			test = new CommandLine();

			test.Literals.Add("literal1");
			if (test.ToString() != "literal1")
			{
				throw new TestAssertionException();
			}

			test.Literals.Add("literal 1");
			if (test.ToString() != "literal1 \"literal 1\"")
			{
				throw new TestAssertionException();
			}

			test.Literals.Add("literal\"1");
			if (test.ToString() != "literal1 \"literal 1\" \"literal\\\"1\"")
			{
				throw new TestAssertionException();
			}

			//-----------
			// Everything
			//-----------

			test = new CommandLine();

			test.Executable = "test.exe";

			test.Literals.Add("literal 1");

			test.Parameters.Add("n1", new List<string>
								{
									"v1", "v2"
								});

			test.Parameters.Add("n2", new List<string>
								{
									"v3"
								});

			if (test.ToString() != "test.exe -n1=v1 -n1=v2 -n2=v3 \"literal 1\"")
			{
				throw new TestAssertionException();
			}

			//-----------------------
			// Special parameter list
			//-----------------------
		}

		#endregion
	}
}
