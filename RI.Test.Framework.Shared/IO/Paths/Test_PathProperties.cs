using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RI.Framework.IO.Paths;
using RI.Framework.Utilities.Exceptions;




namespace RI.Test.Framework.IO.Paths
{
	[TestClass]
	public sealed class Test_PathProperties
	{
		#region Instance Methods

		[TestMethod]
		public void General_Test ()
		{
			PathProperties test = null;

			//---------
			// FromPath
			//---------

			try
			{
				test = PathProperties.FromPath(null);
				throw new TestAssertionException();
			}
			catch (ArgumentNullException)
			{
			}

			test = PathProperties.FromPath(@"");
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.Empty)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test");
			if (test.Type != PathProperties.GetSystemType())
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test");
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}

			try
			{
				test = PathProperties.FromPath(null, false, false, null);
				throw new TestAssertionException();
			}
			catch (ArgumentNullException)
			{
			}

			test = PathProperties.FromPath(@"", false, false, null);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.Empty)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test", false, false, null);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.AmbiguousType)
			{
				throw new TestAssertionException();
			}

			try
			{
				test = PathProperties.FromPath(null, false, false, PathType.Windows);
				throw new TestAssertionException();
			}
			catch (ArgumentNullException)
			{
			}

			test = PathProperties.FromPath(@"", false, false, PathType.Windows);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.Empty)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test", false, false, PathType.Windows);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}

			try
			{
				test = PathProperties.FromPath(@"c:\test", false, false, PathType.Invalid);
				throw new TestAssertionException();
			}
			catch (ArgumentOutOfRangeException)
			{
			}

			test = PathProperties.FromPath("@test", false, false, PathType.Windows);
			if (!test.Equals(test.Clone()))
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath("@test", false, false, PathType.Windows);
			if (!test.Equals(((ICloneable)test).Clone()))
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\test\", false, false, PathType.Windows);
			if (test.ToString() != @"\test\")
			{
				throw new TestAssertionException();
			}

			//-------------
			// MakeRelative
			//-------------

			try
			{
				test = PathProperties.MakeRelative(null, PathProperties.FromPath(@"c:\test", false, false, null));
				throw new TestAssertionException();
			}
			catch (ArgumentNullException)
			{
			}

			try
			{
				test = PathProperties.MakeRelative(PathProperties.FromPath(@"c:\test", false, false, null), null);
				throw new TestAssertionException();
			}
			catch (ArgumentNullException)
			{
			}

			try
			{
				test = PathProperties.MakeRelative(PathProperties.FromPath(@"test", false, false, null), PathProperties.FromPath(@"c:\test", false, false, null));
				throw new TestAssertionException();
			}
			catch (InvalidPathArgumentException)
			{
			}

			try
			{
				test = PathProperties.MakeRelative(PathProperties.FromPath(@"c:\test", false, false, null), PathProperties.FromPath(@"test", false, false, null));
				throw new TestAssertionException();
			}
			catch (InvalidPathArgumentException)
			{
			}

			try
			{
				test = PathProperties.MakeRelative(PathProperties.FromPath(@"test", false, false, PathType.Windows), PathProperties.FromPath(@"c:\test", false, false, PathType.Windows));
				throw new TestAssertionException();
			}
			catch (InvalidPathArgumentException)
			{
			}

			try
			{
				test = PathProperties.MakeRelative(PathProperties.FromPath(@"c:\test", false, false, PathType.Windows), PathProperties.FromPath(@"test", false, false, PathType.Unix));
				throw new TestAssertionException();
			}
			catch (InvalidPathArgumentException)
			{
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"c:\test", false, false, PathType.Windows), PathProperties.FromPath(@"abcd", false, false, PathType.Windows));
			if (!test.IsValid)
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"abcd")
			{
				throw new TestAssertionException();
			}

			//-------------
			// MakeAbsolute
			//-------------

			try
			{
				test = PathProperties.MakeAbsolute(null, PathProperties.FromPath(@"c:\test", false, false, null));
				throw new TestAssertionException();
			}
			catch (ArgumentNullException)
			{
			}

			try
			{
				test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"c:\test", false, false, null), null);
				throw new TestAssertionException();
			}
			catch (ArgumentNullException)
			{
			}

			try
			{
				test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"test", false, false, null), PathProperties.FromPath(@"c:\test", false, false, null));
				throw new TestAssertionException();
			}
			catch (InvalidPathArgumentException)
			{
			}

			try
			{
				test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"c:\test", false, false, null), PathProperties.FromPath(@"test", false, false, null));
				throw new TestAssertionException();
			}
			catch (InvalidPathArgumentException)
			{
			}

			try
			{
				test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"test", false, false, PathType.Windows), PathProperties.FromPath(@"c:\test", false, false, PathType.Windows));
				throw new TestAssertionException();
			}
			catch (InvalidPathArgumentException)
			{
			}

			try
			{
				test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"c:\test", false, false, PathType.Windows), PathProperties.FromPath(@"test", false, false, PathType.Unix));
				throw new TestAssertionException();
			}
			catch (InvalidPathArgumentException)
			{
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"c:\test", false, false, PathType.Windows), PathProperties.FromPath(@"c:\abcd", false, false, PathType.Windows));
			if (!test.IsValid)
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"c:\abcd")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Unc_Test ()
		{
			PathProperties test = null;

			//-----------
			// Path types
			//-----------

			test = PathProperties.FromPath(@"test", false, false, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.IsValid)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test", false, false, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.IsValid)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test", false, false, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.IsValid)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test", false, false, PathType.Unix);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.WrongType)
			{
				throw new TestAssertionException();
			}
			if (test.IsValid)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test", false, false, PathType.Windows);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.WrongType)
			{
				throw new TestAssertionException();
			}
			if (test.IsValid)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test\abcd", false, false, PathType.Unix);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.WrongType)
			{
				throw new TestAssertionException();
			}
			if (test.IsValid)
			{
				throw new TestAssertionException();
			}

			//----------
			// Wildcards
			//----------

			test = PathProperties.FromPath(@"test", false, false, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test", false, false, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"*.tmp", false, false, PathType.Unc);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.WildcardsNotAllowed)
			{
				throw new TestAssertionException();
			}
			if (!test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\*.tmp", false, false, null);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.WildcardsNotAllowed)
			{
				throw new TestAssertionException();
			}
			if (!test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"*.tmp", true, false, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\*.tmp", true, false, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\*.tmp", true, false, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			//----------
			// Relatives
			//----------

			test = PathProperties.FromPath(@"test", false, false, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test", false, false, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"..\test", false, false, PathType.Unc);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.RelativesNotAllowed)
			{
				throw new TestAssertionException();
			}
			if (!test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\..\abcd", false, false, null);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.RelativesNotAllowed)
			{
				throw new TestAssertionException();
			}
			if (!test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"..\test", false, true, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\..\abcd", false, true, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\..\abcd", false, true, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			//-----
			// Root
			//-----

			test = PathProperties.FromPath(@"test", false, false, PathType.Unc);
			if (test.Type != PathType.Unc)
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
			if (test.Root != null)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server", false, false, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (!test.IsRoot)
			{
				throw new TestAssertionException();
			}
			if (!test.IsRooted)
			{
				throw new TestAssertionException();
			}
			if (test.Root != @"\\server")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\", false, false, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (!test.IsRoot)
			{
				throw new TestAssertionException();
			}
			if (!test.IsRooted)
			{
				throw new TestAssertionException();
			}
			if (test.Root != @"\\server")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test", false, false, null);
			if (test.Type != PathType.Unc)
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
			if (test.Root != @"\\server")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\", false, false, null);
			if (test.Type != PathType.Unc)
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
			if (test.Root != @"\\server")
			{
				throw new TestAssertionException();
			}

			//-------
			// Parent
			//-------

			test = PathProperties.FromPath(@"test", false, false, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Parent != null)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server", false, false, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Parent != null)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\", false, false, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Parent != null)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test", false, false, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Parent != @"\\server")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\", false, false, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Parent != @"\\server")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\abcd", false, false, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Parent != @"\\server\test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\abcd\", false, false, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Parent != @"\\server\test")
			{
				throw new TestAssertionException();
			}

			//-----
			// Name
			//-----

			test = PathProperties.FromPath(@"test", false, false, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Name != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server", false, false, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Name != @"server")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\", false, false, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Name != @"server")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test", false, false, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Name != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\", false, false, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Name != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\abcd", false, false, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Name != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\abcd\", false, false, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.Name != @"abcd")
			{
				throw new TestAssertionException();
			}

			//-------
			// Errors
			//-------

			test = PathProperties.FromPath(@"\\server\test|abcd", false, false, PathType.Unc);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.InvalidCharacter)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test:abcd", false, false, PathType.Unc);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.InvalidCharacter)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test/abcd", false, false, PathType.Unc);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.InvalidDirectorySeparator)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test/abcd", false, false, null);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.InvalidDirectorySeparator)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\\abcd", false, false, PathType.Unc);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.RepeatedDirectorySeparator)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@" \test", false, false, PathType.Unc);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.EmptyName)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\ \abcd", false, false, PathType.Unc);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.EmptyName)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\..\..\abcd", false, true, PathType.Unc);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.RelativeGoesBeyondRoot)
			{
				throw new TestAssertionException();
			}

			//--------------
			// Normalization
			//--------------

			// Relative

			test = PathProperties.FromPath(@"test", false, false, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\test", false, false, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test\", false, false, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test\")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\test\", false, false, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\test\")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\test\abcd\", false, false, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\test\abcd\")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"test\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[0] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[1] != @"abcd")
			{
				throw new TestAssertionException();
			}

			// Absolute

			test = PathProperties.FromPath(@"\\server\test\abcd", false, false, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\\server\test\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"\\server\test\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[1] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[2] != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\abcd\", false, false, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\\server\test\abcd\")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"\\server\test\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[1] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[2] != @"abcd")
			{
				throw new TestAssertionException();
			}

			//----------
			// Resolving
			//----------

			// General

			test = PathProperties.FromPath(@".", false, true, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @".")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @".")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @".")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"..", false, true, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"..")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test", false, true, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\test\", false, true, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\test\")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test", false, true, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\\server\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"\\server\test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\", false, true, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\\server\test\")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"\\server\test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}

			// Relative/Same

			test = PathProperties.FromPath(@"\.\test", false, true, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\.\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@".\.\test", false, true, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @".\.\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test\.\", false, true, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test\.\")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test\.\.", false, true, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test\.\.")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@".\test\.", false, true, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @".\test\.")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"test")
			{
				throw new TestAssertionException();
			}

			// Relative/Up

			test = PathProperties.FromPath(@"\..\test", false, true, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\..\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"..\test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"..\..\test", false, true, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"..\..\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"..\..\test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[2] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test\..\", false, true, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test\..\")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @".")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @".")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test\..\..", false, true, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test\..\..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"..")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"..\test\..", false, true, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"..\test\..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"..")
			{
				throw new TestAssertionException();
			}

			// Absolute/Same

			test = PathProperties.FromPath(@"\\server\test\abcd\.", false, true, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\\server\test\abcd\.")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"\\server\test\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[2] != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\abcd\.\.", false, true, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\\server\test\abcd\.\.")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"\\server\test\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[2] != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\.\abcd", false, true, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\\server\test\.\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"\\server\test\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[2] != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\.\.\abcd", false, true, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\\server\test\.\.\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"\\server\test\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[2] != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\.\abcd\.", false, true, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\\server\test\.\abcd\.")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"\\server\test\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[2] != @"abcd")
			{
				throw new TestAssertionException();
			}

			// Absolute/Up

			test = PathProperties.FromPath(@"\\server\test\abcd\..", false, true, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\\server\test\abcd\..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"\\server\test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\abcd\..\..", false, true, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\\server\test\abcd\..\..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"\\server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\..\abcd", false, true, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\\server\test\..\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"\\server\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\1234\..\..\abcd", false, true, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\\server\test\1234\..\..\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"\\server\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\test\..\abcd\..", false, true, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\\server\test\..\abcd\..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"\\server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}

			// Mixed

			test = PathProperties.FromPath(@"..\test\..\.\abcd\..", false, true, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"..\test\..\.\abcd\..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"..")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"..\test\..\.\abcd\..\1234", false, true, PathType.Unc);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"..\test\..\.\abcd\..\1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"..\1234")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"1234")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\.\test\.\..\abcd\..", false, true, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\\server\.\test\.\..\abcd\..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"\\server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\\server\.\test\.\..\abcd\..\1234", false, true, null);
			if (test.Type != PathType.Unc)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\\server\.\test\.\..\abcd\..\1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"\\server\1234")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"1234")
			{
				throw new TestAssertionException();
			}

			//---------
			// Equality
			//---------

			if (PathProperties.FromPath(@"\\server\test", false, false, null).Equals(1234))
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"\\server\test", false, false, null).Equals(PathProperties.FromPath(@"\\server\test\abcd", false, false, null)))
			{
				throw new TestAssertionException();
			}

			if (!PathProperties.FromPath(@"\\server\test", false, false, null).Equals(PathProperties.FromPath(@"\\server\test\", false, false, null)))
			{
				throw new TestAssertionException();
			}

			if (!PathProperties.FromPath(@"\\server\test", false, false, null).Equals(PathProperties.FromPath(@"\\server\TEST", false, false, null)))
			{
				throw new TestAssertionException();
			}

			if (!PathProperties.FromPath(@"\\server\test", false, false, null).Equals(PathProperties.FromPath(@"\\SERVER\TEST\", false, false, null)))
			{
				throw new TestAssertionException();
			}

			//---------
			// Hashcode
			//---------

			if (PathProperties.FromPath(@"\\server\test", false, false, null).GetHashCode() == PathProperties.FromPath(@"\\server\test\abcd", false, false, null).GetHashCode())
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"\\server\test", false, false, null).GetHashCode() != PathProperties.FromPath(@"\\server\test\", false, false, null).GetHashCode())
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"\\server\test", false, false, null).GetHashCode() != PathProperties.FromPath(@"\\server\TEST", false, false, null).GetHashCode())
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"\\server\test", false, false, null).GetHashCode() != PathProperties.FromPath(@"\\SERVER\TEST\", false, false, null).GetHashCode())
			{
				throw new TestAssertionException();
			}

			//-----------
			// Comparison
			//-----------

			if (PathProperties.FromPath(@"\\server\test", false, false, null).CompareTo(1234) != 1)
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"\\server\test", false, false, null).CompareTo(PathProperties.FromPath(@"\\server\test\abcd", false, false, null)) >= 0)
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"\\server\test\abcd", false, false, null).CompareTo(PathProperties.FromPath(@"\\server\test", false, false, null)) <= 0)
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"\\server\test", false, false, null).CompareTo(PathProperties.FromPath(@"\\server\test\", false, false, null)) != 0)
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"\\server\test", false, false, null).CompareTo(PathProperties.FromPath(@"\\server\TEST", false, false, null)) != 0)
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"\\server\test", false, false, null).CompareTo(PathProperties.FromPath(@"\\SERVER\TEST\", false, false, null)) != 0)
			{
				throw new TestAssertionException();
			}

			//--------------
			// Make relative
			//--------------

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"\\server\", false, true, null), PathProperties.FromPath(@"test", false, true, PathType.Unc));
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"\\server\", false, true, null), PathProperties.FromPath(@"\\server\", false, true, null));
			if (test.PathResolved != @".")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @".")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"\\server\test", false, true, null), PathProperties.FromPath(@"\\server\test", false, true, null));
			if (test.PathResolved != @".")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @".")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"\\server\", false, true, null), PathProperties.FromPath(@"\\test\", false, true, null));
			if (test.PathResolved != @"\\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"\\test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"\\server\test", false, true, null), PathProperties.FromPath(@"\\abcd\test", false, true, null));
			if (test.PathResolved != @"\\abcd\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"\\abcd\test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"\\server\test\abcd", false, true, null), PathProperties.FromPath(@"\\server\test\1234", false, true, null));
			if (test.PathResolved != @"..\1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"..\1234")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"\\server\test\abcd", false, true, null), PathProperties.FromPath(@"\\server\abcd\1234", false, true, null));
			if (test.PathResolved != @"..\..\abcd\1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"..\..\abcd\1234")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"\\server\test", false, true, null), PathProperties.FromPath(@"\\server\test\abcd", false, true, null));
			if (test.PathResolved != @"abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"\\server\test\abcd", false, true, null), PathProperties.FromPath(@"\\server\test", false, true, null));
			if (test.PathResolved != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"..")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"\\server\test\.\..\abcd\.\", false, true, null), PathProperties.FromPath(@"\\server\.\test\..\1234\", false, true, null));
			if (test.PathResolved != @"..\1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"..\1234")
			{
				throw new TestAssertionException();
			}

			//--------------
			// Make absolute
			//--------------

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"\\server\", false, true, null), PathProperties.FromPath(@"\\server\", false, true, null));
			if (test.PathResolved != @"\\server")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"\\server")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"\\server\test", false, true, null), PathProperties.FromPath(@"\\abcd\test", false, true, null));
			if (test.PathResolved != @"\\abcd\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"\\abcd\test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"\\server\", false, true, null), PathProperties.FromPath(@"test", false, true, PathType.Unc));
			if (test.PathResolved != @"\\server\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"\\server\test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"\\server\abcd", false, true, null), PathProperties.FromPath(@"test", false, true, PathType.Unc));
			if (test.PathResolved != @"\\server\abcd\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"\\server\abcd\test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"\\server\", false, true, null), PathProperties.FromPath(@"test\abcd", false, true, PathType.Unc));
			if (test.PathResolved != @"\\server\test\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"\\server\test\abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"\\server\1234", false, true, null), PathProperties.FromPath(@"test\abcd", false, true, PathType.Unc));
			if (test.PathResolved != @"\\server\1234\test\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"\\server\1234\test\abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"\\server\test\abcd\", false, true, null), PathProperties.FromPath(@"..\1234", false, true, PathType.Unc));
			if (test.PathResolved != @"\\server\test\1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"\\server\test\1234")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"\\server\test\..\abcd\", false, true, null), PathProperties.FromPath(@"1234\test", false, true, PathType.Unc));
			if (test.PathResolved != @"\\server\abcd\1234\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"\\server\abcd\1234\test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"\\server\test\..\abcd\", false, true, null), PathProperties.FromPath(@"..\1234", false, true, PathType.Unc));
			if (test.PathResolved != @"\\server\1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"\\server\1234")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"\\server\test\.\..\abcd\.\", false, true, null), PathProperties.FromPath(@"\.\test\..\1234\", false, true, PathType.Unc));
			if (test.PathResolved != @"\\server\abcd\1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"\\server\abcd\1234")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Unix_Test ()
		{
			PathProperties test = null;

			//-----------
			// Path types
			//-----------

			test = PathProperties.FromPath(@"test", false, false, PathType.Unix);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.IsValid)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.IsValid)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test", false, false, PathType.Unix);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.IsValid)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test", false, false, PathType.Unc);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.WrongType)
			{
				throw new TestAssertionException();
			}
			if (test.IsValid)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test", false, false, PathType.Windows);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.WrongType)
			{
				throw new TestAssertionException();
			}
			if (test.IsValid)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test/abcd", false, false, PathType.Windows);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.WrongType)
			{
				throw new TestAssertionException();
			}
			if (test.IsValid)
			{
				throw new TestAssertionException();
			}

			//----------
			// Wildcards
			//----------

			test = PathProperties.FromPath(@"test", false, false, PathType.Unix);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"*.tmp", false, false, PathType.Unix);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.WildcardsNotAllowed)
			{
				throw new TestAssertionException();
			}
			if (!test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/*.tmp", false, false, null);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.WildcardsNotAllowed)
			{
				throw new TestAssertionException();
			}
			if (!test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"*.tmp", true, false, PathType.Unix);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/*.tmp", true, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/*.tmp", true, false, PathType.Unix);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			//----------
			// Relatives
			//----------

			test = PathProperties.FromPath(@"test", false, false, PathType.Unix);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"../test", false, false, PathType.Unix);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.RelativesNotAllowed)
			{
				throw new TestAssertionException();
			}
			if (!test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/../abcd", false, false, null);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.RelativesNotAllowed)
			{
				throw new TestAssertionException();
			}
			if (!test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"../test", false, true, PathType.Unix);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/../abcd", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/../abcd", false, true, PathType.Unix);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			//-----
			// Root
			//-----

			test = PathProperties.FromPath(@"test", false, false, PathType.Unix);
			if (test.Type != PathType.Unix)
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
			if (test.Root != null)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (!test.IsRoot)
			{
				throw new TestAssertionException();
			}
			if (!test.IsRooted)
			{
				throw new TestAssertionException();
			}
			if (test.Root != @"/server")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (!test.IsRoot)
			{
				throw new TestAssertionException();
			}
			if (!test.IsRooted)
			{
				throw new TestAssertionException();
			}
			if (test.Root != @"/server")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test", false, false, null);
			if (test.Type != PathType.Unix)
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
			if (test.Root != @"/server")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/", false, false, null);
			if (test.Type != PathType.Unix)
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
			if (test.Root != @"/server")
			{
				throw new TestAssertionException();
			}

			//-------
			// Parent
			//-------

			test = PathProperties.FromPath(@"test", false, false, PathType.Unix);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Parent != null)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Parent != null)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Parent != null)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Parent != @"/server")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Parent != @"/server")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/abcd", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Parent != @"/server/test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/abcd/", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Parent != @"/server/test")
			{
				throw new TestAssertionException();
			}

			//-----
			// Name
			//-----

			test = PathProperties.FromPath(@"test", false, false, PathType.Unix);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Name != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Name != @"server")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Name != @"server")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Name != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Name != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/abcd", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Name != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/abcd/", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.Name != @"abcd")
			{
				throw new TestAssertionException();
			}

			//-------
			// Errors
			//-------

			test = PathProperties.FromPath(@"/server/test|abcd", false, false, PathType.Unix);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.InvalidCharacter)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test:abcd", false, false, PathType.Unix);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.InvalidCharacter)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test\abcd", false, false, PathType.Unix);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.InvalidDirectorySeparator)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test\abcd", false, false, null);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.InvalidDirectorySeparator)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test//abcd", false, false, PathType.Unix);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.RepeatedDirectorySeparator)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@" /test", false, false, PathType.Unix);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.EmptyName)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/ /abcd", false, false, PathType.Unix);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.EmptyName)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/../../abcd", false, true, PathType.Unix);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.RelativeGoesBeyondRoot)
			{
				throw new TestAssertionException();
			}

			//--------------
			// Normalization
			//--------------

			// Relative

			test = PathProperties.FromPath(@"test", false, false, PathType.Unix);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/test", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"/test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"/test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test/", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test/")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/test/", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"/test/")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"/test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test/abcd", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test/abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"test/abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[0] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[1] != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test/abcd/", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test/abcd/")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"test/abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[0] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[1] != @"abcd")
			{
				throw new TestAssertionException();
			}

			// Absolute

			test = PathProperties.FromPath(@"/server/test/abcd", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"/server/test/abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"/server/test/abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[1] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[2] != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/abcd/", false, false, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"/server/test/abcd/")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"/server/test/abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[1] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[2] != @"abcd")
			{
				throw new TestAssertionException();
			}

			//----------
			// Resolving
			//----------

			// General

			test = PathProperties.FromPath(@".", false, true, PathType.Unix);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @".")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @".")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @".")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"..", false, true, PathType.Unix);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"..")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test", false, true, PathType.Unix);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test/", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test/")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"/server/test")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"/server/test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"/server/test/")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"/server/test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}

			// Relative/Same

			test = PathProperties.FromPath(@"./test", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"./test")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"././test", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"././test")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test/./", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test/./")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test/./.", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test/./.")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"./test/.", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"./test/.")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"test")
			{
				throw new TestAssertionException();
			}

			// Relative/Up

			test = PathProperties.FromPath(@"../test", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"../test")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"../test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"../../test", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"../../test")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"../../test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[2] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test/../", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test/../")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @".")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @".")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test/../..", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test/../..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"..")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"../test/..", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"../test/..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"..")
			{
				throw new TestAssertionException();
			}

			// Absolute/Same

			test = PathProperties.FromPath(@"/server/test/abcd/.", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"/server/test/abcd/.")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"/server/test/abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[2] != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/abcd/./.", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"/server/test/abcd/./.")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"/server/test/abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[2] != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/./abcd", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"/server/test/./abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"/server/test/abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[2] != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/././abcd", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"/server/test/././abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"/server/test/abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[2] != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/./abcd/.", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"/server/test/./abcd/.")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"/server/test/abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[2] != @"abcd")
			{
				throw new TestAssertionException();
			}

			// Absolute/Up

			test = PathProperties.FromPath(@"/server/test/abcd/..", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"/server/test/abcd/..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"/server/test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/abcd/../..", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"/server/test/abcd/../..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"/server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/../abcd", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"/server/test/../abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"/server/abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/1234/../../abcd", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"/server/test/1234/../../abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"/server/abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/test/../abcd/..", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"/server/test/../abcd/..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"/server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}

			// Mixed

			test = PathProperties.FromPath(@"../test/.././abcd/..", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"../test/.././abcd/..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"..")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"../test/.././abcd/../1234", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"../test/.././abcd/../1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"../1234")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"1234")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/./test/./../abcd/..", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"/server/./test/./../abcd/..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"/server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"/server/./test/./../abcd/../1234", false, true, null);
			if (test.Type != PathType.Unix)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"/server/./test/./../abcd/../1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"/server/1234")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"server")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"1234")
			{
				throw new TestAssertionException();
			}

			//---------
			// Equality
			//---------

			if (PathProperties.FromPath(@"/server/test", false, false, null).Equals(1234))
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"/server/test", false, false, null).Equals(PathProperties.FromPath(@"/server/test/abcd", false, false, null)))
			{
				throw new TestAssertionException();
			}

			if (!PathProperties.FromPath(@"/server/test", false, false, null).Equals(PathProperties.FromPath(@"/server/test/", false, false, null)))
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"/server/test", false, false, null).Equals(PathProperties.FromPath(@"/server/TEST", false, false, null)))
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"/server/test", false, false, null).Equals(PathProperties.FromPath(@"/SERVER/TEST/", false, false, null)))
			{
				throw new TestAssertionException();
			}

			//---------
			// Hashcode
			//---------

			if (PathProperties.FromPath(@"/server/test", false, false, null).GetHashCode() == PathProperties.FromPath(@"/server/test/abcd", false, false, null).GetHashCode())
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"/server/test", false, false, null).GetHashCode() != PathProperties.FromPath(@"/server/test/", false, false, null).GetHashCode())
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"/server/test", false, false, null).GetHashCode() == PathProperties.FromPath(@"/server/TEST", false, false, null).GetHashCode())
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"/server/test", false, false, null).GetHashCode() == PathProperties.FromPath(@"/SERVER\TEST/", false, false, null).GetHashCode())
			{
				throw new TestAssertionException();
			}

			//-----------
			// Comparison
			//-----------

			if (PathProperties.FromPath(@"/server/test", false, false, null).CompareTo(1234) != 1)
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"/server/test", false, false, null).CompareTo(PathProperties.FromPath(@"/server/test/abcd", false, false, null)) >= 0)
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"/server/test/abcd", false, false, null).CompareTo(PathProperties.FromPath(@"/server/test", false, false, null)) <= 0)
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"/server/test", false, false, null).CompareTo(PathProperties.FromPath(@"/server/test/", false, false, null)) != 0)
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"/server/test", false, false, null).CompareTo(PathProperties.FromPath(@"/server/TEST", false, false, null)) == 0)
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"/server/test", false, false, null).CompareTo(PathProperties.FromPath(@"/SERVER/TEST/", false, false, null)) == 0)
			{
				throw new TestAssertionException();
			}

			//--------------
			// Make relative
			//--------------

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"/server/", false, true, null), PathProperties.FromPath(@"test", false, true, PathType.Unix));
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"/server/", false, true, null), PathProperties.FromPath(@"/server/", false, true, null));
			if (test.PathResolved != @".")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @".")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"/server/test", false, true, null), PathProperties.FromPath(@"/server/test", false, true, null));
			if (test.PathResolved != @".")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @".")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"/server/", false, true, null), PathProperties.FromPath(@"/test/", false, true, null));
			if (test.PathResolved != @"/test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"/test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"/server/test", false, true, null), PathProperties.FromPath(@"/abcd/test", false, true, null));
			if (test.PathResolved != @"/abcd/test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"/abcd/test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"/server/test/abcd", false, true, null), PathProperties.FromPath(@"/server/test/1234", false, true, null));
			if (test.PathResolved != @"../1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"../1234")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"/server/test/abcd", false, true, null), PathProperties.FromPath(@"/server/abcd/1234", false, true, null));
			if (test.PathResolved != @"../../abcd/1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"../../abcd/1234")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"/server/test", false, true, null), PathProperties.FromPath(@"/server/test/abcd", false, true, null));
			if (test.PathResolved != @"abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"/server/test/abcd", false, true, null), PathProperties.FromPath(@"/server/test", false, true, null));
			if (test.PathResolved != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"..")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"/server/test/./../abcd/./", false, true, null), PathProperties.FromPath(@"/server/./test/../1234/", false, true, null));
			if (test.PathResolved != @"../1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"../1234")
			{
				throw new TestAssertionException();
			}

			//--------------
			// Make absolute
			//--------------

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"/server/", false, true, null), PathProperties.FromPath(@"/server/", false, true, null));
			if (test.PathResolved != @"/server")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"/server")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"/server/test", false, true, null), PathProperties.FromPath(@"/abcd/test", false, true, null));
			if (test.PathResolved != @"/abcd/test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"/abcd/test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"/server/", false, true, null), PathProperties.FromPath(@"test", false, true, PathType.Unix));
			if (test.PathResolved != @"/server/test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"/server/test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"/server/abcd", false, true, null), PathProperties.FromPath(@"test", false, true, PathType.Unix));
			if (test.PathResolved != @"/server/abcd/test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"/server/abcd/test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"/server/", false, true, null), PathProperties.FromPath(@"test/abcd", false, true, PathType.Unix));
			if (test.PathResolved != @"/server/test/abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"/server/test/abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"/server/1234", false, true, null), PathProperties.FromPath(@"test/abcd", false, true, PathType.Unix));
			if (test.PathResolved != @"/server/1234/test/abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"/server/1234/test/abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"/server/test/abcd/", false, true, null), PathProperties.FromPath(@"../1234", false, true, PathType.Unix));
			if (test.PathResolved != @"/server/test/1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"/server/test/1234")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"/server/test/../abcd/", false, true, null), PathProperties.FromPath(@"1234/test", false, true, PathType.Unix));
			if (test.PathResolved != @"/server/abcd/1234/test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"/server/abcd/1234/test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"/server/test/../abcd/", false, true, null), PathProperties.FromPath(@"../1234", false, true, PathType.Unix));
			if (test.PathResolved != @"/server/1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"/server/1234")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"/server/test/./../abcd/./", false, true, null), PathProperties.FromPath(@"./test/../1234/", false, true, PathType.Unix));
			if (test.PathResolved != @"/server/abcd/1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"/server/abcd/1234")
			{
				throw new TestAssertionException();
			}
		}

		[TestMethod]
		public void Windows_Test ()
		{
			PathProperties test = null;

			//-----------
			// Path types
			//-----------

			test = PathProperties.FromPath(@"test", false, false, PathType.Windows);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.IsValid)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.IsValid)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test", false, false, PathType.Windows);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.IsValid)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test", false, false, PathType.Unix);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.WrongType)
			{
				throw new TestAssertionException();
			}
			if (test.IsValid)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test", false, false, PathType.Unc);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.WrongType)
			{
				throw new TestAssertionException();
			}
			if (test.IsValid)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test\abcd", false, false, PathType.Unix);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.WrongType)
			{
				throw new TestAssertionException();
			}
			if (test.IsValid)
			{
				throw new TestAssertionException();
			}

			//----------
			// Wildcards
			//----------

			test = PathProperties.FromPath(@"test", false, false, PathType.Windows);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"*.tmp", false, false, PathType.Windows);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.WildcardsNotAllowed)
			{
				throw new TestAssertionException();
			}
			if (!test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\*.tmp", false, false, null);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.WildcardsNotAllowed)
			{
				throw new TestAssertionException();
			}
			if (!test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"*.tmp", true, false, PathType.Windows);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\*.tmp", true, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\*.tmp", true, false, PathType.Windows);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.HasWildcards)
			{
				throw new TestAssertionException();
			}

			//----------
			// Relatives
			//----------

			test = PathProperties.FromPath(@"test", false, false, PathType.Windows);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"..\test", false, false, PathType.Windows);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.RelativesNotAllowed)
			{
				throw new TestAssertionException();
			}
			if (!test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\..\abcd", false, false, null);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.RelativesNotAllowed)
			{
				throw new TestAssertionException();
			}
			if (!test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"..\test", false, true, PathType.Windows);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\..\abcd", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\..\abcd", false, true, PathType.Windows);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.None)
			{
				throw new TestAssertionException();
			}
			if (!test.HasRelatives)
			{
				throw new TestAssertionException();
			}

			//-----
			// Root
			//-----

			test = PathProperties.FromPath(@"test", false, false, PathType.Windows);
			if (test.Type != PathType.Windows)
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
			if (test.Root != null)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"x:", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (!test.IsRoot)
			{
				throw new TestAssertionException();
			}
			if (!test.IsRooted)
			{
				throw new TestAssertionException();
			}
			if (test.Root != @"x:")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"x:\", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (!test.IsRoot)
			{
				throw new TestAssertionException();
			}
			if (!test.IsRooted)
			{
				throw new TestAssertionException();
			}
			if (test.Root != @"x:")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test", false, false, null);
			if (test.Type != PathType.Windows)
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
			if (test.Root != @"c:")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\", false, false, null);
			if (test.Type != PathType.Windows)
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
			if (test.Root != @"c:")
			{
				throw new TestAssertionException();
			}

			//-------
			// Parent
			//-------

			test = PathProperties.FromPath(@"test", false, false, PathType.Windows);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Parent != null)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"x:", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Parent != null)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"x:\", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Parent != null)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Parent != @"c:")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Parent != @"c:")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\abcd", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Parent != @"c:\test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\abcd\", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Parent != @"c:\test")
			{
				throw new TestAssertionException();
			}

			//-----
			// Name
			//-----

			test = PathProperties.FromPath(@"test", false, false, PathType.Windows);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Name != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"x:", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Name != @"x:")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"x:\", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Name != @"x:")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Name != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Name != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\abcd", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Name != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\abcd\", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.Name != @"abcd")
			{
				throw new TestAssertionException();
			}

			//-------
			// Errors
			//-------

			test = PathProperties.FromPath(@"c:\test|abcd", false, false, PathType.Windows);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.InvalidCharacter)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test:abcd", false, false, PathType.Windows);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.InvalidCharacter)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test/abcd", false, false, PathType.Windows);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.InvalidDirectorySeparator)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test/abcd", false, false, null);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.InvalidDirectorySeparator)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\\abcd", false, false, PathType.Windows);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.RepeatedDirectorySeparator)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@" \test", false, false, PathType.Windows);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.EmptyName)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\ \abcd", false, false, PathType.Windows);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.EmptyName)
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\..\..\abcd", false, true, PathType.Windows);
			if (test.Type != PathType.Invalid)
			{
				throw new TestAssertionException();
			}
			if (test.Error != PathError.RelativeGoesBeyondRoot)
			{
				throw new TestAssertionException();
			}

			//--------------
			// Normalization
			//--------------

			// Relative

			test = PathProperties.FromPath(@"test", false, false, PathType.Windows);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\test", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test\", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test\")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\test\", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\test\")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\test\abcd\", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\test\abcd\")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"test\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[0] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[1] != @"abcd")
			{
				throw new TestAssertionException();
			}

			// Absolute

			test = PathProperties.FromPath(@"c:\test\abcd", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"c:\test\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"c:\test\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[0] != @"c:")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[1] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[2] != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\abcd\", false, false, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"c:\test\abcd\")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"c:\test\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[0] != @"c:")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[1] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsNormalized[2] != @"abcd")
			{
				throw new TestAssertionException();
			}

			//----------
			// Resolving
			//----------

			// General

			test = PathProperties.FromPath(@".", false, true, PathType.Windows);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @".")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @".")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @".")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"..", false, true, PathType.Windows);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"..")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test", false, true, PathType.Windows);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"\test\", false, true, PathType.Windows);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\test\")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"c:\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"c:\test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"c:")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"c:\test\")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"c:\test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"c:")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}

			// Relative/Same

			test = PathProperties.FromPath(@"\.\test", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\.\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@".\.\test", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @".\.\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test\.\", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test\.\")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test\.\.", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test\.\.")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@".\test\.", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @".\test\.")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"test")
			{
				throw new TestAssertionException();
			}

			// Relative/Up

			test = PathProperties.FromPath(@"\..\test", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"\..\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"..\test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"..\..\test", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"..\..\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"..\..\test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[2] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test\..\", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test\..\")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @".")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @".")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"test\..\..", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"test\..\..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"..")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"..\test\..", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"..\test\..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"..")
			{
				throw new TestAssertionException();
			}

			// Absolute/Same

			test = PathProperties.FromPath(@"c:\test\abcd\.", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"c:\test\abcd\.")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"c:\test\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"c:")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[2] != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\abcd\.\.", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"c:\test\abcd\.\.")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"c:\test\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"c:")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[2] != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\.\abcd", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"c:\test\.\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"c:\test\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"c:")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[2] != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\.\.\abcd", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"c:\test\.\.\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"c:\test\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"c:")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[2] != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\.\abcd\.", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"c:\test\.\abcd\.")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"c:\test\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 3)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"c:")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[2] != @"abcd")
			{
				throw new TestAssertionException();
			}

			// Absolute/Up

			test = PathProperties.FromPath(@"c:\test\abcd\..", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"c:\test\abcd\..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"c:\test")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"c:")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\abcd\..\..", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"c:\test\abcd\..\..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"c:")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"c:")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\..\abcd", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"c:\test\..\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"c:\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"c:")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\1234\..\..\abcd", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"c:\test\1234\..\..\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"c:\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"c:")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\test\..\abcd\..", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"c:\test\..\abcd\..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"c:")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"c:")
			{
				throw new TestAssertionException();
			}

			// Mixed

			test = PathProperties.FromPath(@"..\test\..\.\abcd\..", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"..\test\..\.\abcd\..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"..")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"..\test\..\.\abcd\..\1234", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"..\test\..\.\abcd\..\1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"..\1234")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"1234")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\.\test\.\..\abcd\..", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"c:\.\test\.\..\abcd\..")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"c:")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 1)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"c:")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.FromPath(@"c:\.\test\.\..\abcd\..\1234", false, true, null);
			if (test.Type != PathType.Windows)
			{
				throw new TestAssertionException();
			}
			if (test.PathOriginal != @"c:\.\test\.\..\abcd\..\1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathResolved != @"c:\1234")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved.Length != 2)
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[0] != @"c:")
			{
				throw new TestAssertionException();
			}
			if (test.PartsResolved[1] != @"1234")
			{
				throw new TestAssertionException();
			}

			//---------
			// Equality
			//---------

			if (PathProperties.FromPath(@"c:\test", false, false, null).Equals(1234))
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"c:\test", false, false, null).Equals(PathProperties.FromPath(@"c:\test\abcd", false, false, null)))
			{
				throw new TestAssertionException();
			}

			if (!PathProperties.FromPath(@"c:\test", false, false, null).Equals(PathProperties.FromPath(@"c:\test\", false, false, null)))
			{
				throw new TestAssertionException();
			}

			if (!PathProperties.FromPath(@"c:\test", false, false, null).Equals(PathProperties.FromPath(@"c:\TEST", false, false, null)))
			{
				throw new TestAssertionException();
			}

			if (!PathProperties.FromPath(@"c:\test", false, false, null).Equals(PathProperties.FromPath(@"c:\TEST\", false, false, null)))
			{
				throw new TestAssertionException();
			}

			//---------
			// Hashcode
			//---------

			if (PathProperties.FromPath(@"c:\test", false, false, null).GetHashCode() == PathProperties.FromPath(@"c:\test\abcd", false, false, null).GetHashCode())
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"c:\test", false, false, null).GetHashCode() != PathProperties.FromPath(@"c:\test\", false, false, null).GetHashCode())
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"c:\test", false, false, null).GetHashCode() != PathProperties.FromPath(@"c:\TEST", false, false, null).GetHashCode())
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"c:\test", false, false, null).GetHashCode() != PathProperties.FromPath(@"c:\TEST\", false, false, null).GetHashCode())
			{
				throw new TestAssertionException();
			}

			//-----------
			// Comparison
			//-----------

			if (PathProperties.FromPath(@"c:\test", false, false, null).CompareTo(1234) != 1)
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"c:\test", false, false, null).CompareTo(PathProperties.FromPath(@"c:\test\abcd", false, false, null)) >= 0)
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"c:\test\abcd", false, false, null).CompareTo(PathProperties.FromPath(@"c:\test", false, false, null)) <= 0)
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"c:\test", false, false, null).CompareTo(PathProperties.FromPath(@"c:\test\", false, false, null)) != 0)
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"c:\test", false, false, null).CompareTo(PathProperties.FromPath(@"c:\TEST", false, false, null)) != 0)
			{
				throw new TestAssertionException();
			}

			if (PathProperties.FromPath(@"c:\test", false, false, null).CompareTo(PathProperties.FromPath(@"c:\TEST\", false, false, null)) != 0)
			{
				throw new TestAssertionException();
			}

			//--------------
			// Make relative
			//--------------

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"c:\", false, true, null), PathProperties.FromPath(@"test", false, true, PathType.Windows));
			if (test.PathResolved != @"test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"c:\", false, true, null), PathProperties.FromPath(@"c:\", false, true, null));
			if (test.PathResolved != @".")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @".")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"c:\test", false, true, null), PathProperties.FromPath(@"c:\test", false, true, null));
			if (test.PathResolved != @".")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @".")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"x:\", false, true, null), PathProperties.FromPath(@"y:\", false, true, null));
			if (test.PathResolved != @"y:")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"y:")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"x:\test", false, true, null), PathProperties.FromPath(@"y:\test", false, true, null));
			if (test.PathResolved != @"y:\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"y:\test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"c:\test\abcd", false, true, null), PathProperties.FromPath(@"c:\test\1234", false, true, null));
			if (test.PathResolved != @"..\1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"..\1234")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"c:\test\abcd", false, true, null), PathProperties.FromPath(@"c:\abcd\1234", false, true, null));
			if (test.PathResolved != @"..\..\abcd\1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"..\..\abcd\1234")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"c:\test", false, true, null), PathProperties.FromPath(@"c:\test\abcd", false, true, null));
			if (test.PathResolved != @"abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"c:\test\abcd", false, true, null), PathProperties.FromPath(@"c:\test", false, true, null));
			if (test.PathResolved != @"..")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"..")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeRelative(PathProperties.FromPath(@"c:\test\.\..\abcd\.\", false, true, null), PathProperties.FromPath(@"c:\.\test\..\1234\", false, true, null));
			if (test.PathResolved != @"..\1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"..\1234")
			{
				throw new TestAssertionException();
			}

			//--------------
			// Make absolute
			//--------------

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"c:\", false, true, null), PathProperties.FromPath(@"c:\", false, true, null));
			if (test.PathResolved != @"c:")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"c:")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"x:\test", false, true, null), PathProperties.FromPath(@"y:\test", false, true, null));
			if (test.PathResolved != @"y:\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"y:\test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"c:\", false, true, null), PathProperties.FromPath(@"test", false, true, PathType.Windows));
			if (test.PathResolved != @"c:\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"c:\test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"c:\abcd", false, true, null), PathProperties.FromPath(@"test", false, true, PathType.Windows));
			if (test.PathResolved != @"c:\abcd\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"c:\abcd\test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"c:\", false, true, null), PathProperties.FromPath(@"test\abcd", false, true, PathType.Windows));
			if (test.PathResolved != @"c:\test\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"c:\test\abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"c:\1234", false, true, null), PathProperties.FromPath(@"test\abcd", false, true, PathType.Windows));
			if (test.PathResolved != @"c:\1234\test\abcd")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"c:\1234\test\abcd")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"c:\test\abcd\", false, true, null), PathProperties.FromPath(@"..\1234", false, true, PathType.Windows));
			if (test.PathResolved != @"c:\test\1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"c:\test\1234")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"c:\test\..\abcd\", false, true, null), PathProperties.FromPath(@"1234\test", false, true, PathType.Windows));
			if (test.PathResolved != @"c:\abcd\1234\test")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"c:\abcd\1234\test")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"c:\test\..\abcd\", false, true, null), PathProperties.FromPath(@"..\1234", false, true, PathType.Windows));
			if (test.PathResolved != @"c:\1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"c:\1234")
			{
				throw new TestAssertionException();
			}

			test = PathProperties.MakeAbsolute(PathProperties.FromPath(@"c:\test\.\..\abcd\.\", false, true, null), PathProperties.FromPath(@"\.\test\..\1234\", false, true, PathType.Windows));
			if (test.PathResolved != @"c:\abcd\1234")
			{
				throw new TestAssertionException();
			}
			if (test.PathNormalized != @"c:\abcd\1234")
			{
				throw new TestAssertionException();
			}
		}

		#endregion
	}
}
