using System;
using System.IO;

using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.IO.Paths
{
	[Serializable]
	public sealed class DirectoryPath : PathString,
	                                    ICloneable<DirectoryPath>
	{
		#region Instance Constructor/Destructor

		public DirectoryPath (string path)
				: base(PathProperties.FromPath(path, false, false, null))
		{
		}

		public DirectoryPath (string path, bool allowWildcards, bool allowRelatives, PathType? assumedType)
				: base(PathProperties.FromPath(path, allowWildcards, allowRelatives, assumedType))
		{
		}

		public DirectoryPath (PathProperties path)
				: base(path)
		{
		}

		#endregion




		#region Instance Properties/Indexer

		public string DirectoryName
		{
			get
			{
				if (this.IsRoot)
				{
					return ( this.Path );
				}

				string[] pieces = this.Path.Split(new[]
				{
					Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar
				}, StringSplitOptions.None);

				return ( pieces[pieces.Length - 1] );
			}
		}

		public DirectoryPath Parent
		{
			get
			{
				if (this.IsRoot)
				{
					return ( null );
				}

				string[] pieces = this.Path.Split(new[]
				{
					Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar
				}, StringSplitOptions.None);

				string parent = string.Join(new string(Path.DirectorySeparatorChar, 1), pieces, 0, pieces.Length - 1);

				return ( new DirectoryPath(parent) );
			}
		}

		#endregion




		#region Instance Methods

		public DirectoryPath Append (params DirectoryPath[] directories)
		{
			directories = directories ?? new DirectoryPath[0];

			for (int i1 = 0; i1 < directories.Length; i1++)
			{
				if (directories[i1].IsAbsolute)
				{
					throw ( new PathNotRelativeArgumentException(nameof(directories)) );
				}
			}

			string path = this.Path;

			for (int i1 = 0; i1 < directories.Length; i1++)
			{
				path = Path.Combine(path, directories[i1].Path);
			}

			return ( new DirectoryPath(path) );
		}

		public FilePath Append (FilePath file)
		{
			if (file == null)
			{
				throw ( new ArgumentNullException(nameof(file)) );
			}

			if (file.IsAbsolute)
			{
				throw ( new PathNotRelativeArgumentException(nameof(file)) );
			}

			return ( new FilePath(Path.Combine(this.Path, file.Path)) );
		}

		public DirectoryPath ChangeDirectoryName (string directoryName)
		{
			if (directoryName == null)
			{
				throw ( new ArgumentNullException(nameof(directoryName)) );
			}

			if (!DirectoryPath.IsDirectoryPath(directoryName, false))
			{
				throw ( new InvalidPathArgumentException(nameof(directoryName)) );
			}

			if (this.IsRoot)
			{
				return ( new DirectoryPath(directoryName + ":") );
			}

			return ( new DirectoryPath(Path.Combine(this.Parent.Path, directoryName)) );
		}

		public DirectoryPath ChangeParent (DirectoryPath newParent)
		{
			if (newParent == null)
			{
				throw ( new ArgumentNullException(nameof(newParent)) );
			}

			return ( newParent.Append(new DirectoryPath(this.DirectoryName)) );
		}

		public DirectoryPath ToAbsolutePath (DirectoryPath root)
		{
			if (root == null)
			{
				throw ( new ArgumentNullException(nameof(root)) );
			}

			if (!root.IsAbsolute)
			{
				throw ( new PathNotAbsoluteArgumentException(nameof(root)) );
			}

			return ( new DirectoryPath(PathString.ToAbsolutePath(this, root)) );
		}

		public DirectoryPath ToRelativePath (DirectoryPath root)
		{
			if (root == null)
			{
				throw ( new ArgumentNullException(nameof(root)) );
			}

			if (!root.IsAbsolute)
			{
				throw ( new PathNotAbsoluteArgumentException(nameof(root)) );
			}

			return ( new DirectoryPath(PathString.ToRelativePath(this, root)) );
		}

		public DirectoryPath ToRelativePath (DirectoryPath root, bool includeCurrentSpecifier)
		{
			if (root == null)
			{
				throw ( new ArgumentNullException(nameof(root)) );
			}

			if (!root.IsAbsolute)
			{
				throw ( new PathNotAbsoluteArgumentException(nameof(root)) );
			}

			return ( new DirectoryPath(PathString.ToRelativePath(this, root, includeCurrentSpecifier)) );
		}

		#endregion




		#region Overrides

		protected override PathString CloneInternal ()
		{
			return this.Clone();
		}

		#endregion




		#region Interface: ICloneable<DirectoryPath>

		public DirectoryPath Clone ()
		{
			return new DirectoryPath(this.GetPathProperties());
		}

		#endregion
	}
}
