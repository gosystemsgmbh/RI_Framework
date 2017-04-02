using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.Services.Logging;
using RI.Test.Framework.Mocks;

namespace RI.Test.Framework.Services.Logging
{
	[TestClass]
	public sealed class Test_LogService
	{
		[TestMethod]
		public void Test ()
		{
			LogService test = new LogService();
			Mock_Writer writer1 = new Mock_Writer();
			Mock_Writer writer2 = new Mock_Writer();

			if (test.Writers.Count() != 0)
			{
				throw new TestAssertionException();
			}

			test.Log(LogLevel.Debug, "Test", "{0}{1}", 1, "abc");

			test.AddWriter(writer1);

			if (test.Writers.Count() != 1)
			{
				throw new TestAssertionException();
			}
			if (!test.Writers.Contains(writer1))
			{
				throw new TestAssertionException();
			}

			test.Log(new DateTime(1, 2, 3, 4, 5, 6, 7), 99, LogLevel.Fatal, "Source", "{0}{1}", 1, "abc");

			if (writer1.Entries.Count != 1)
			{
				throw new TestAssertionException();
			}
			if (!this.TestEntry(writer1.Entries[writer1.Entries.Count - 1], new DateTime(1, 2, 3, 4, 5, 6, 7), 99, LogLevel.Fatal, "Source", "1abc"))
			{
				throw new TestAssertionException();
			}

			test.AddWriter(writer1);

			if (test.Writers.Count() != 1)
			{
				throw new TestAssertionException();
			}
			if (!test.Writers.Contains(writer1))
			{
				throw new TestAssertionException();
			}

			test.Log(new DateTime(1, 2, 3, 4, 5, 6, 7), 99, LogLevel.Fatal, "Source", "{0}{1}", 1, "abc");

			if (writer1.Entries.Count != 2)
			{
				throw new TestAssertionException();
			}
			if (!this.TestEntry(writer1.Entries[writer1.Entries.Count - 1], new DateTime(1, 2, 3, 4, 5, 6, 7), 99, LogLevel.Fatal, "Source", "1abc"))
			{
				throw new TestAssertionException();
			}

			test.AddWriter(writer2);

			if (test.Writers.Count() != 2)
			{
				throw new TestAssertionException();
			}
			if (!test.Writers.Contains(writer1))
			{
				throw new TestAssertionException();
			}
			if (!test.Writers.Contains(writer2))
			{
				throw new TestAssertionException();
			}

			test.Log(new DateTime(1, 2, 3, 4, 5, 6, 7), 99, LogLevel.Fatal, "Source", "{0}{1}", 1, "abc");

			if (writer1.Entries.Count != 3)
			{
				throw new TestAssertionException();
			}
			if (!this.TestEntry(writer1.Entries[writer1.Entries.Count - 1], new DateTime(1, 2, 3, 4, 5, 6, 7), 99, LogLevel.Fatal, "Source", "1abc"))
			{
				throw new TestAssertionException();
			}
			if (writer2.Entries.Count != 1)
			{
				throw new TestAssertionException();
			}
			if (!this.TestEntry(writer2.Entries[writer2.Entries.Count - 1], new DateTime(1, 2, 3, 4, 5, 6, 7), 99, LogLevel.Fatal, "Source", "1abc"))
			{
				throw new TestAssertionException();
			}

			test.RemoveWriter(writer1);

			if (test.Writers.Count() != 1)
			{
				throw new TestAssertionException();
			}
			if (!test.Writers.Contains(writer2))
			{
				throw new TestAssertionException();
			}

			test.Log(new DateTime(1, 2, 3, 4, 5, 6, 7), 99, LogLevel.Fatal, "Source", "{0}{1}", 1, "abc");

			if (writer2.Entries.Count != 2)
			{
				throw new TestAssertionException();
			}
			if (!this.TestEntry(writer2.Entries[writer2.Entries.Count - 1], new DateTime(1, 2, 3, 4, 5, 6, 7), 99, LogLevel.Fatal, "Source", "1abc"))
			{
				throw new TestAssertionException();
			}

			test.Log(new DateTime(3000, 2, 3, 4, 5, 6, 7), 99, LogLevel.Fatal, "Source", "{0}{1}", 1, "abc");

			if (writer2.Entries.Count != 3)
			{
				throw new TestAssertionException();
			}
			if (!this.TestEntry(writer2.Entries[writer2.Entries.Count - 1], new DateTime(3000, 2, 3, 4, 5, 6, 7), 99, LogLevel.Fatal, "Source", "1abc"))
			{
				throw new TestAssertionException();
			}

			test.Cleanup(DateTime.Now);

			if (writer2.Entries.Count != 1)
			{
				throw new TestAssertionException();
			}
			if (!this.TestEntry(writer2.Entries[writer2.Entries.Count - 1], new DateTime(3000, 2, 3, 4, 5, 6, 7), 99, LogLevel.Fatal, "Source", "1abc"))
			{
				throw new TestAssertionException();
			}
		}

		private bool TestEntry (MockTuple<DateTime, int, LogLevel, string, string> entry, DateTime timestamp, int threadId, LogLevel severity, string source, string message)
		{
			if (entry.Item1 != timestamp)
			{
				return false;
			}

			if (entry.Item2 != threadId)
			{
				return false;
			}

			if (entry.Item3 != severity)
			{
				return false;
			}

			if (entry.Item4 != source)
			{
				return false;
			}

			if (entry.Item5 != message)
			{
				return false;
			}

			return true;
		}
	}
}
