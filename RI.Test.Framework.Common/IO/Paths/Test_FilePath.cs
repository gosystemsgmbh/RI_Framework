using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RI.Framework.Collections;
using RI.Framework.Collections.Comparison;
using RI.Framework.IO.Paths;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;

namespace RI.Test.Framework.IO.Paths
{
	[TestClass]
	public sealed class Test_FilePath
	{
		[TestMethod]
		public void Operations_Test ()
		{
			FilePath test = FilePath.GetTemporaryFile();

			//-------------------
			// Creation, deletion
			//-------------------

			if (!test.Exists)
			{
				throw new TestAssertionException();
			}

			if (!test.Delete())
			{
				throw new TestAssertionException();
			}

			if (test.Delete())
			{
				throw new TestAssertionException();
			}

			if (test.Exists)
			{
				throw new TestAssertionException();
			}

			if (!test.Create())
			{
				throw new TestAssertionException();
			}

			if (test.Create())
			{
				throw new TestAssertionException();
			}

			if (!test.Delete())
			{
				throw new TestAssertionException();
			}

			//-------------------------
			// Writing and reading text
			//-------------------------

			if (!test.WriteText("Test 1234"))
			{
				throw new TestAssertionException();
			}

			if (test.WriteText("0123456789"))
			{
				throw new TestAssertionException();
			}

			if (test.ReadText() != "0123456789")
			{
				throw new TestAssertionException();
			}

			if (test.WriteText("0123456789", Encoding.UTF8))
			{
				throw new TestAssertionException();
			}

			if (test.ReadText(Encoding.UTF8) != "0123456789")
			{
				throw new TestAssertionException();
			}

			if (!test.Delete())
			{
				throw new TestAssertionException();
			}

			if (test.ReadText() != null)
			{
				throw new TestAssertionException();
			}

			//-------------------------
			// Writing and reading data
			//-------------------------

			byte[] data = new byte[]
			{
				0, 1, 2, 3, 4
			};

			if (!test.WriteBytes(data))
			{
				throw new TestAssertionException();
			}

			if (test.WriteBytes(data))
			{
				throw new TestAssertionException();
			}

			if(!data.SequenceEqual(test.ReadBytes(), CollectionComparerFlags.None))
			{
				throw new TestAssertionException();
			}

			if (!test.Delete())
			{
				throw new TestAssertionException();
			}

			if (test.ReadBytes() != null)
			{
				throw new TestAssertionException();
			}

			//----------------------------------
			// Files not possible for operations
			//----------------------------------

			test = new FilePath("*.tmp");

			try
			{
				bool temp = test.Exists;
			}
			catch (InvalidOperationException)
			{
			}

			try
			{
				test.Create();
			}
			catch (InvalidOperationException)
			{
			}

			try
			{
				test.Delete();
			}
			catch (InvalidOperationException)
			{
			}

			try
			{
				test.ReadBytes();
			}
			catch (InvalidOperationException)
			{
			}

			try
			{
				test.ReadText();
			}
			catch (InvalidOperationException)
			{
			}

			try
			{
				test.WriteBytes(null);
			}
			catch (InvalidOperationException)
			{
			}

			try
			{
				test.WriteText(null);
			}
			catch (InvalidOperationException)
			{
			}
		}

		[TestMethod]
		public void Properties_Test ()
		{
			//-----------------------
			// Relative, no extension
			//-----------------------

			FilePath test = new FilePath(@"test");

			if (test.Directory != null)
			{
				throw new TestAssertionException();
			}

			if (test.ExtensionWithDot != null)
			{
				throw new TestAssertionException();
			}

			if (test.ExtensionWithoutDot != null)
			{
				throw new TestAssertionException();
			}

			if (test.FileName != @"test")
			{
				throw new TestAssertionException();
			}

			if (test.FileNameWithoutExtension != @"test")
			{
				throw new TestAssertionException();
			}

			if (test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			if (test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			if (test.IsRoot)
			{
				throw new TestAssertionException();
			}

			if (test.IsRooted)
			{
				throw new TestAssertionException();
			}

			if (test.PathNormalized != @"test")
			{
				throw new TestAssertionException();
			}

			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}

			if (test.PathOriginal != @"test")
			{
				throw new TestAssertionException();
			}

			if (test.Root != null)
			{
				throw new TestAssertionException();
			}

			if (test.Type != PathProperties.GetSystemType())
			{
				throw new TestAssertionException();
			}

			//------------------------
			// Relative, one extension
			//------------------------

			test = new FilePath(@"test.tmp");

			if (test.Directory != null)
			{
				throw new TestAssertionException();
			}

			if (test.ExtensionWithDot != ".tmp")
			{
				throw new TestAssertionException();
			}

			if (test.ExtensionWithoutDot != "tmp")
			{
				throw new TestAssertionException();
			}

			if (test.FileName != @"test.tmp")
			{
				throw new TestAssertionException();
			}

			if (test.FileNameWithoutExtension != @"test")
			{
				throw new TestAssertionException();
			}

			if (test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			if (test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			if (test.IsRoot)
			{
				throw new TestAssertionException();
			}

			if (test.IsRooted)
			{
				throw new TestAssertionException();
			}

			if (test.PathNormalized != @"test.tmp")
			{
				throw new TestAssertionException();
			}

			if (test.PathResolved != @"test.tmp")
			{
				throw new TestAssertionException();
			}

			if (test.PathOriginal != @"test.tmp")
			{
				throw new TestAssertionException();
			}

			if (test.Root != null)
			{
				throw new TestAssertionException();
			}

			if (test.Type != PathProperties.GetSystemType())
			{
				throw new TestAssertionException();
			}

			//-------------------------
			// Relative, two extensions
			//-------------------------

			test = new FilePath(@"test.data.tmp");

			if (test.Directory != null)
			{
				throw new TestAssertionException();
			}

			if (test.ExtensionWithDot != ".tmp")
			{
				throw new TestAssertionException();
			}

			if (test.ExtensionWithoutDot != "tmp")
			{
				throw new TestAssertionException();
			}

			if (test.FileName != @"test.data.tmp")
			{
				throw new TestAssertionException();
			}

			if (test.FileNameWithoutExtension != @"test.data")
			{
				throw new TestAssertionException();
			}

			if (test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			if (test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			if (test.IsRoot)
			{
				throw new TestAssertionException();
			}

			if (test.IsRooted)
			{
				throw new TestAssertionException();
			}

			if (test.PathNormalized != @"test.data.tmp")
			{
				throw new TestAssertionException();
			}

			if (test.PathResolved != @"test.data.tmp")
			{
				throw new TestAssertionException();
			}

			if (test.PathOriginal != @"test.data.tmp")
			{
				throw new TestAssertionException();
			}

			if (test.Root != null)
			{
				throw new TestAssertionException();
			}

			if (test.Type != PathProperties.GetSystemType())
			{
				throw new TestAssertionException();
			}

			//-----------------------
			// Absolute, no extension
			//-----------------------

			test = new FilePath(@"c:\abcd\test");

			if (test.Directory != new DirectoryPath(@"c:\abcd"))
			{
				throw new TestAssertionException();
			}

			if (test.ExtensionWithDot != null)
			{
				throw new TestAssertionException();
			}

			if (test.ExtensionWithoutDot != null)
			{
				throw new TestAssertionException();
			}

			if (test.FileName != @"test")
			{
				throw new TestAssertionException();
			}

			if (test.FileNameWithoutExtension != @"test")
			{
				throw new TestAssertionException();
			}

			if (test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			if (test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			if (test.IsRoot)
			{
				throw new TestAssertionException();
			}

			if (!test.IsRooted)
			{
				throw new TestAssertionException();
			}

			if (test.PathNormalized != @"c:\abcd\test")
			{
				throw new TestAssertionException();
			}

			if (test.PathResolved != @"c:\abcd\test")
			{
				throw new TestAssertionException();
			}

			if (test.PathOriginal != @"c:\abcd\test")
			{
				throw new TestAssertionException();
			}

			if (test.Root != @"c:")
			{
				throw new TestAssertionException();
			}

			if (test.Type != PathProperties.GetSystemType())
			{
				throw new TestAssertionException();
			}

			//------------------------
			// Absolute, one extension
			//------------------------

			test = new FilePath(@"c:\abcd\test.tmp");

			if (test.Directory != new DirectoryPath(@"c:\abcd"))
			{
				throw new TestAssertionException();
			}

			if (test.ExtensionWithDot != ".tmp")
			{
				throw new TestAssertionException();
			}

			if (test.ExtensionWithoutDot != "tmp")
			{
				throw new TestAssertionException();
			}

			if (test.FileName != @"test.tmp")
			{
				throw new TestAssertionException();
			}

			if (test.FileNameWithoutExtension != @"test")
			{
				throw new TestAssertionException();
			}

			if (test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			if (test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			if (test.IsRoot)
			{
				throw new TestAssertionException();
			}

			if (!test.IsRooted)
			{
				throw new TestAssertionException();
			}

			if (test.PathNormalized != @"c:\abcd\test.tmp")
			{
				throw new TestAssertionException();
			}

			if (test.PathResolved != @"c:\abcd\test.tmp")
			{
				throw new TestAssertionException();
			}

			if (test.PathOriginal != @"c:\abcd\test.tmp")
			{
				throw new TestAssertionException();
			}

			if (test.Root != @"c:")
			{
				throw new TestAssertionException();
			}

			if (test.Type != PathProperties.GetSystemType())
			{
				throw new TestAssertionException();
			}

			//-------------------------
			// Absolute, two extensions
			//-------------------------

			test = new FilePath(@"c:\abcd\test.data.tmp");

			if (test.Directory != new DirectoryPath(@"c:\abcd"))
			{
				throw new TestAssertionException();
			}

			if (test.ExtensionWithDot != ".tmp")
			{
				throw new TestAssertionException();
			}

			if (test.ExtensionWithoutDot != "tmp")
			{
				throw new TestAssertionException();
			}

			if (test.FileName != @"test.data.tmp")
			{
				throw new TestAssertionException();
			}

			if (test.FileNameWithoutExtension != @"test.data")
			{
				throw new TestAssertionException();
			}

			if (test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			if (test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			if (test.IsRoot)
			{
				throw new TestAssertionException();
			}

			if (!test.IsRooted)
			{
				throw new TestAssertionException();
			}

			if (test.PathNormalized != @"c:\abcd\test.data.tmp")
			{
				throw new TestAssertionException();
			}

			if (test.PathResolved != @"c:\abcd\test.data.tmp")
			{
				throw new TestAssertionException();
			}

			if (test.PathOriginal != @"c:\abcd\test.data.tmp")
			{
				throw new TestAssertionException();
			}

			if (test.Root != @"c:")
			{
				throw new TestAssertionException();
			}

			if (test.Type != PathProperties.GetSystemType())
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Changes_Test ()
		{
			if (new FilePath(@"c:\test\1234.tmp").ChangeDirectory(@"d:\abcd\").PathNormalized != @"d:\abcd\1234.tmp")
			{
				throw new TestAssertionException();
			}

			if (new FilePath(@"c:\test\1234.tmp").ChangeExtension(@".dat").PathNormalized != @"c:\test\1234.dat")
			{
				throw new TestAssertionException();
			}

			if (new FilePath(@"c:\test\1234.tmp").ChangeExtension(@"dat").PathNormalized != @"c:\test\1234.dat")
			{
				throw new TestAssertionException();
			}

			if (new FilePath(@"c:\test\1234.tmp").ChangeExtension(@"").PathNormalized != @"c:\test\1234.")
			{
				throw new TestAssertionException();
			}

			if (new FilePath(@"c:\test\1234.tmp").ChangeExtension(null).PathNormalized != @"c:\test\1234")
			{
				throw new TestAssertionException();
			}

			if (new FilePath(@"c:\test\1234.tmp").ChangeFileName(@"abcd.dat").PathNormalized != @"c:\test\abcd.dat")
			{
				throw new TestAssertionException();
			}

			if (new FilePath(@"c:\test\1234.tmp").ChangeFileName(@"abcd.dat.xxx").PathNormalized != @"c:\test\abcd.dat.xxx")
			{
				throw new TestAssertionException();
			}

			if (new FilePath(@"c:\test\1234.tmp").ChangeFileNameWithoutExtension(@"abcd").PathNormalized != @"c:\test\abcd.tmp")
			{
				throw new TestAssertionException();
			}

			if (new FilePath(@"c:\test\1234.tmp").ChangeFileNameWithoutExtension(@"abcd.dat").PathNormalized != @"c:\test\abcd.dat.tmp")
			{
				throw new TestAssertionException();
			}

			if (new FilePath(@"c:\test\1234.tmp").ChangeFileNameWithoutExtension(@"").PathNormalized != @"c:\test\.tmp")
			{
				throw new TestAssertionException();
			}

			if (new FilePath(@"c:\test\1234.tmp").ChangeFileNameWithoutExtension(null).PathNormalized != @"c:\test\tmp")
			{
				throw new TestAssertionException();
			}

			try
			{
				new FilePath(@"c:\test\1234.tmp").ChangeDirectory(null);
				throw new TestAssertionException();
			}
			catch (ArgumentNullException)
			{
			}

			try
			{
				new FilePath(@"c:\test\1234.tmp").ChangeExtension(@"|");
				throw new TestAssertionException();
			}
			catch (InvalidPathArgumentException)
			{
			}

			try
			{
				new FilePath(@"c:\test\1234.tmp").ChangeFileName(@"|");
				throw new TestAssertionException();
			}
			catch (InvalidPathArgumentException)
			{
			}

			try
			{
				new FilePath(@"c:\test\1234.tmp").ChangeFileName(@"");
				throw new TestAssertionException();
			}
			catch (EmptyStringArgumentException)
			{
			}

			try
			{
				new FilePath(@"c:\test\1234.tmp").ChangeFileName(null);
				throw new TestAssertionException();
			}
			catch (ArgumentNullException)
			{
			}

			try
			{
				new FilePath(@"c:\test\1234.tmp").ChangeFileNameWithoutExtension(@"|");
				throw new TestAssertionException();
			}
			catch (InvalidPathArgumentException)
			{
			}
		}

		[TestMethod]
		public void Equals_Test ()
		{
			//---------
			// Operator
			//---------

			if (new FilePath("test.tmp") == (FilePath)"test.dat")
			{
				throw new TestAssertionException();
			}

			if (!(new FilePath("test.tmp") == (FilePath)"test.tmp"))
			{
				throw new TestAssertionException();
			}

			if (new FilePath("test.tmp") != (FilePath)"test.tmp")
			{
				throw new TestAssertionException();
			}

			if (!(new FilePath("test.tmp") != (FilePath)"test.dat"))
			{
				throw new TestAssertionException();
			}

			if (new FilePath("test.tmp") == (FilePath)(string)null)
			{
				throw new TestAssertionException();
			}

			if (!(new FilePath("test.tmp") != (FilePath)(string)null))
			{
				throw new TestAssertionException();
			}

			//-------
			// Static
			//-------

			if (!PathString.Equals((FilePath)null, (FilePath)null))
			{
				throw new TestAssertionException();
			}

			if (PathString.Equals((FilePath)"test.tmp", (FilePath)null))
			{
				throw new TestAssertionException();
			}

			if (PathString.Equals((FilePath)null, (FilePath)"test.tmp"))
			{
				throw new TestAssertionException();
			}

			if (!PathString.Equals((FilePath)"test.tmp", (FilePath)"test.tmp"))
			{
				throw new TestAssertionException();
			}

			if (PathString.Equals((FilePath)"test.tmp", (FilePath)"test.dat"))
			{
				throw new TestAssertionException();
			}

			//-------
			// Object
			//-------

			if (new FilePath("test.tmp").Equals((object)null))
			{
				throw new TestAssertionException();
			}

			if (!new FilePath("test.tmp").Equals((object)(FilePath)"test.tmp"))
			{
				throw new TestAssertionException();
			}

			if (!new FilePath("test.tmp").Equals((object)PathProperties.FromPath("test.tmp", false, false, PathProperties.GetSystemType())))
			{
				throw new TestAssertionException();
			}

			//---------
			// FilePath
			//---------

			if (new FilePath("test.tmp").Equals((FilePath)(string)null))
			{
				throw new TestAssertionException();
			}

			if (new FilePath("test.tmp").Equals((FilePath)"test.dat"))
			{
				throw new TestAssertionException();
			}

			if (!new FilePath("test.tmp").Equals((FilePath)"test.tmp"))
			{
				throw new TestAssertionException();
			}

			//---------------
			// PathProperties
			//---------------

			if (new FilePath("test.tmp").Equals((PathProperties)null))
			{
				throw new TestAssertionException();
			}

			if (new FilePath("test.tmp").Equals(PathProperties.FromPath("test.dat", false, false, PathProperties.GetSystemType())))
			{
				throw new TestAssertionException();
			}

			if (!new FilePath("test.tmp").Equals(PathProperties.FromPath("test.tmp", false, false, PathProperties.GetSystemType())))
			{
				throw new TestAssertionException();
			}

			//----------
			// Hash code
			//----------

			if (new FilePath("test.tmp").GetHashCode() != new FilePath("test.tmp").GetHashCode())
			{
				throw new TestAssertionException();
			}

			if (new FilePath("test.tmp").GetHashCode() == new FilePath("test.dat").GetHashCode())
			{
				throw new TestAssertionException();
			}

			//-------
			// String
			//-------

			if (new FilePath("test.tmp").ToString() != "test.tmp")
			{
				throw new TestAssertionException();
			}

			if (new FilePath(@"c:\abcd\test.tmp\").ToString() != @"c:\abcd\test.tmp\")
			{
				throw new TestAssertionException();
			}

			if (( (string)( (FilePath)null ) ) != null)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Compare_Test()
		{
			//---------
			// Operator
			//---------

			if (!(new FilePath("test.tmp") >= new FilePath("test.tmp")))
			{
				throw new TestAssertionException();
			}

			if (!(new FilePath("test.tmp") <= new FilePath("test.tmp")))
			{
				throw new TestAssertionException();
			}

			if (!(new FilePath("test1234.tmp") >= new FilePath("test.tmp")))
			{
				throw new TestAssertionException();
			}

			if (!(new FilePath("test.tmp") <= new FilePath("test1234.tmp")))
			{
				throw new TestAssertionException();
			}

			if (!(new FilePath("test1234.tmp") > new FilePath("test.tmp")))
			{
				throw new TestAssertionException();
			}

			if (!(new FilePath("test.tmp") < new FilePath("test1234.tmp")))
			{
				throw new TestAssertionException();
			}

			if (!(new FilePath("test1234.tmp") >= null))
			{
				throw new TestAssertionException();
			}

			if (new FilePath("test.tmp") <= null)
			{
				throw new TestAssertionException();
			}

			if (!(new FilePath("test1234.tmp") > null))
			{
				throw new TestAssertionException();
			}

			if (new FilePath("test.tmp") < null)
			{
				throw new TestAssertionException();
			}

			//-------
			// Static
			//-------

			if (PathString.Compare((FilePath)null, (FilePath)null) != 0)
			{
				throw new TestAssertionException();
			}

			if (PathString.Compare((FilePath)"test.tmp", (FilePath)null) != 1)
			{
				throw new TestAssertionException();
			}

			if (PathString.Compare((FilePath)null, (FilePath)"test.tmp") != -1)
			{
				throw new TestAssertionException();
			}

			if (PathString.Compare((FilePath)"test.tmp", (FilePath)"test.tmp") != 0)
			{
				throw new TestAssertionException();
			}

			if (PathString.Compare((FilePath)"test.tmp", (FilePath)"test.dat") == 0)
			{
				throw new TestAssertionException();
			}

			//-------
			// Object
			//-------

			if (new FilePath("test.tmp").CompareTo((object)null) != 1)
			{
				throw new TestAssertionException();
			}

			if (new FilePath("test.tmp").CompareTo((object)(FilePath)"test.tmp") != 0)
			{
				throw new TestAssertionException();
			}

			if (new FilePath("test.tmp").CompareTo((object)PathProperties.FromPath("test.tmp", false, false, PathProperties.GetSystemType())) != 0)
			{
				throw new TestAssertionException();
			}

			//---------
			// FilePath
			//---------

			if (new FilePath("test.tmp").CompareTo((FilePath)(string)null) != 1)
			{
				throw new TestAssertionException();
			}

			if (new FilePath("test.tmp").CompareTo((FilePath)"test.dat") == 0)
			{
				throw new TestAssertionException();
			}

			if (new FilePath("test.tmp").CompareTo((FilePath)"test.tmp") != 0)
			{
				throw new TestAssertionException();
			}

			//---------------
			// PathProperties
			//---------------

			if (new FilePath("test.tmp").CompareTo((PathProperties)null) != 1)
			{
				throw new TestAssertionException();
			}

			if (new FilePath("test.tmp").CompareTo(PathProperties.FromPath("test.dat", false, false, PathProperties.GetSystemType())) == 0)
			{
				throw new TestAssertionException();
			}

			if (new FilePath("test.tmp").CompareTo(PathProperties.FromPath("test.tmp", false, false, PathProperties.GetSystemType())) != 0)
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Clone_Test ()
		{
			ICloneable test1 = new FilePath("test.tmp");
			if ((FilePath)test1.Clone() != (FilePath)"test.tmp")
			{
				throw new TestAssertionException();
			}

			ICloneable<FilePath> test2 = new FilePath("test.tmp");
			if (test2.Clone() != (FilePath)"test.tmp")
			{
				throw new TestAssertionException();
			}

			ICloneable<PathString> test3 = new FilePath("test.tmp");
			if (test3.Clone() != (FilePath)"test.tmp")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void MakeRelativeTo_Test ()
		{
			try
			{
				new FilePath(@"c:\abcd\test.tmp").MakeRelativeTo(null);
				throw new TestAssertionException();
			}
			catch (ArgumentNullException)
			{
			}

			try
			{
				new FilePath(@"c:\abcd\test.tmp").MakeRelativeTo(@"1234\abcd");
				throw new TestAssertionException();
			}
			catch (InvalidPathArgumentException)
			{
			}

			if (new FilePath(@"abcd\test.tmp").MakeRelativeTo(@"c:\1234") != @"abcd\test.tmp")
			{
				throw new TestAssertionException();
			}

			if (new FilePath(@"c:\abcd\test.tmp").MakeRelativeTo(@"c:\abcd") != @"test.tmp")
			{
				throw new TestAssertionException();
			}

			if (new FilePath(@"c:\abcd\test.tmp").MakeRelativeTo(@"c:\abcd\1234") != @"..\test.tmp")
			{
				throw new TestAssertionException();
			}

			if (new FilePath(@"c:\abcd\test.tmp").MakeRelativeTo(@"c:\") != @"abcd\test.tmp")
			{
				throw new TestAssertionException();
			}

			if (new FilePath(@"c:\abcd\test.tmp").MakeRelativeTo(@"d:\abcd\test.tmp") != @"c:\abcd\test.tmp")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void MakeAbsoluteFrom_Test()
		{
			try
			{
				new FilePath(@"test.tmp").MakeAbsoluteFrom(null);
				throw new TestAssertionException();
			}
			catch (ArgumentNullException)
			{
			}

			try
			{
				new FilePath(@"test.tmp").MakeAbsoluteFrom(@"1234\abcd");
				throw new TestAssertionException();
			}
			catch (InvalidPathArgumentException)
			{
			}

			if (new FilePath(@"c:\test\1234.tmp").MakeAbsoluteFrom(@"c:\abcd") != @"c:\test\1234.tmp")
			{
				throw new TestAssertionException();
			}

			if (new FilePath(@"test.tmp").MakeAbsoluteFrom(@"c:\abcd") != @"c:\abcd\test.tmp")
			{
				throw new TestAssertionException();
			}

			if (new FilePath(@"..\test.tmp").MakeAbsoluteFrom(@"c:\abcd") != @"c:\test.tmp")
			{
				throw new TestAssertionException();
			}

			if (new FilePath(@"abcd\test.tmp").MakeAbsoluteFrom(@"c:\1234") != @"c:\1234\abcd\test.tmp")
			{
				throw new TestAssertionException();
			}
		}
	}
}
