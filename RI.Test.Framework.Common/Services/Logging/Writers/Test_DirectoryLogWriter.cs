using System;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.IO.Paths;
using RI.Framework.Services.Logging.Writers;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Logging;

namespace RI.Test.Framework.Services.Logging.Writers
{
	[TestClass]
	public sealed class Test_DirectoryLogWriter
	{
		#region Instance Methods

		[TestMethod]
		public void Test ()
		{
			DirectoryPath path = DirectoryPath.GetTempDirectory().AppendDirectory(Guid.NewGuid().ToString("N"));
			path.Delete();

			bool isUnix = path.Type == PathType.Unix;

			DirectoryLogWriter test = new DirectoryLogWriter(path, false, "Test.txt", Encoding.UTF8, new DateTime(1, 2, 3, 4, 5, 6, 7));

			if (test.CommonDirectory != path)
			{
				throw new TestAssertionException();
			}
			if (test.CurrentDirectory != (isUnix ? (path + "/0001-02-03-04-05-06-007") : (path + "\\0001-02-03-04-05-06-007")))
			{
				throw new TestAssertionException();
			}
			if (test.CurrentFile != (isUnix ? (path + "/0001-02-03-04-05-06-007/Test.txt") : (path + "\\0001-02-03-04-05-06-007\\Test.txt")))
			{
				throw new TestAssertionException();
			}
			if (test.Encoding != Encoding.UTF8)
			{
				throw new TestAssertionException();
			}
			if (test.InitialTimestamp != new DateTime(1, 2, 3, 4, 5, 6, 7))
			{
				throw new TestAssertionException();
			}

			if (path.GetSubdirectories(false, false).Count != 1)
			{
				throw new TestAssertionException();
			}

			FilePath file = test.CurrentFile;

			test.Close();

			string text = test.CurrentFile.ReadText(Encoding.UTF8);
			if (!text.IsNullOrEmptyOrWhitespace())
			{
				throw new TestAssertionException();
			}

			DateTime now = DateTime.Now;

			test = new DirectoryLogWriter(path, false, "Test.log", Encoding.UTF32, now);

			if (test.CommonDirectory != path)
			{
				throw new TestAssertionException();
			}
			if (test.CurrentDirectory != (isUnix ? (path + "/" + now.ToSortableString('-')) : (path + "\\" + now.ToSortableString('-'))))
			{
				throw new TestAssertionException();
			}
			if (test.CurrentFile != (isUnix ? (path + "/" + now.ToSortableString('-') + "/Test.log") : (path + "\\" + now.ToSortableString('-') + "\\Test.log")))
			{
				throw new TestAssertionException();
			}
			if (test.Encoding != Encoding.UTF32)
			{
				throw new TestAssertionException();
			}
			if (test.InitialTimestamp != now)
			{
				throw new TestAssertionException();
			}

			if (path.GetSubdirectories(false, false).Count != 2)
			{
				throw new TestAssertionException();
			}

			test.Cleanup(new DateTime(2000, 2, 3, 4, 5, 6, 7));

			if (path.GetSubdirectories(false, false).Count != 1)
			{
				throw new TestAssertionException();
			}

			test.Log(new DateTime(1, 2, 3, 4, 5, 6, 7), 99, LogLevel.Fatal, "Source", "Message");

			file = test.CurrentFile;

			test.Close();

			text = test.CurrentFile.ReadText(Encoding.UTF8);
			if (text != ("# [0001-02-03-04-05-06-007] [99] [Fatal] [Source] Message" + Environment.NewLine))
			{
				throw new TestAssertionException();
			}

			path.Delete();
		}

		#endregion
	}
}
