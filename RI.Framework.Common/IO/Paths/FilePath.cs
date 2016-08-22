using System;
using System.IO;

using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.IO.Paths
{
	[Serializable]
	public sealed class FilePath : PathString,
	                               ICloneable<FilePath>
	{
		#region Instance Constructor/Destructor

		public FilePath (string path)
				: base(PathProperties.FromPath(path, false, false, null))
		{
		}

		public FilePath (string path, bool allowWildcards, bool allowRelatives, PathType? assumedType)
				: base(PathProperties.FromPath(path, allowWildcards, allowRelatives, assumedType))
		{
		}

		public FilePath (PathProperties path)
				: base(path)
		{
		}

		#endregion




		#region Instance Properties/Indexer

		public DirectoryPath Directory
		{
			get
			{
				return ( new DirectoryPath(Path.GetDirectoryName(this.Path)) );
			}
		}

		public string Extension
		{
			get
			{
				if (!Path.HasExtension(this.Path))
				{
					return ( null );
				}

				return ( Path.GetExtension(this.Path) );
			}
		}

		public string FileName
		{
			get
			{
				return ( Path.GetFileName(this.Path) );
			}
		}

		public string FileNameWithoutExtension
		{
			get
			{
				return ( Path.GetFileNameWithoutExtension(this.Path) );
			}
		}

		#endregion




		#region Instance Methods

		public FilePath ChangeDirectory (DirectoryPath directory)
		{
			if (directory == null)
			{
				throw ( new ArgumentNullException(nameof(directory)) );
			}

			return ( new FilePath(Path.Combine(directory.Path, this.FileName)) );
		}

		public FilePath ChangeExtension (string extension)
		{
			if (extension == null)
			{
				throw ( new ArgumentNullException(nameof(extension)) );
			}

			if (!FilePath.IsFileExtension(extension, false))
			{
				throw ( new InvalidPathArgumentException(nameof(extension)) );
			}

			return ( new FilePath(Path.ChangeExtension(this.Path, extension)) );
		}

		public FilePath ChangeFileName (string fileName)
		{
			if (fileName == null)
			{
				throw ( new ArgumentNullException(nameof(fileName)) );
			}

			if (!FilePath.IsFilePath(fileName, false))
			{
				throw ( new InvalidPathArgumentException(nameof(fileName)) );
			}

			return ( new FilePath(Path.Combine(this.Directory.Path, fileName)) );
		}

		public FilePath ChangeFileNameWithoutExtension (string fileNameWithoutExtension)
		{
			if (fileNameWithoutExtension == null)
			{
				throw ( new ArgumentNullException(nameof(fileNameWithoutExtension)) );
			}

			if (!FilePath.IsFilePath(fileNameWithoutExtension, false))
			{
				throw ( new InvalidPathArgumentException(nameof(fileNameWithoutExtension)) );
			}

			return ( new FilePath(Path.Combine(this.Directory.Path, fileNameWithoutExtension + ( this.Extension ?? string.Empty ))) );
		}

		public FilePath ToAbsolutePath (DirectoryPath root)
		{
			if (root == null)
			{
				throw ( new ArgumentNullException(nameof(root)) );
			}

			if (!root.IsAbsolute)
			{
				throw ( new PathNotAbsoluteArgumentException(nameof(root)) );
			}

			return ( new FilePath(PathString.ToAbsolutePath(this, root)) );
		}

		public FilePath ToRelativePath (DirectoryPath root)
		{
			if (root == null)
			{
				throw ( new ArgumentNullException(nameof(root)) );
			}

			if (!root.IsAbsolute)
			{
				throw ( new PathNotAbsoluteArgumentException(nameof(root)) );
			}

			return ( new FilePath(PathString.ToRelativePath(this, root)) );
		}

		public FilePath ToRelativePath (DirectoryPath root, bool includeCurrentSpecifier)
		{
			if (root == null)
			{
				throw ( new ArgumentNullException(nameof(root)) );
			}

			if (!root.IsAbsolute)
			{
				throw ( new PathNotAbsoluteArgumentException(nameof(root)) );
			}

			return ( new FilePath(PathString.ToRelativePath(this, root, includeCurrentSpecifier)) );
		}

		#endregion




		#region Overrides

		protected override PathString CloneInternal ()
		{
			return this.Clone();
		}

		#endregion




		#region Interface: ICloneable<FilePath>

		public FilePath Clone ()
		{
			return new FilePath(this.GetPathProperties());
		}

		#endregion
	}
}
