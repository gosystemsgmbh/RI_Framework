using System;
using System.IO;

using RI.Framework.Collections;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.IO.Paths
{
	/// <summary>
	///     Describes a path to a file.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="FilePath" /> uses <see cref="PathProperties" /> to extract and store path information.
	///         See <see cref="PathProperties" /> for more details about the supported types of file paths.
	///     </para>
	///     <para>
	///         <see cref="FilePath" /> provides more file path specific functionalities compared to <see cref="string" /> and offers a more consistent way of working with paths than <see cref="Path" />.
	///         It can be implicitly converted to a <see cref="string" /> to work seamless with APIs using <see cref="string" /> for paths.
	///     </para>
	/// </remarks>
	/// TODO: Example
	[Serializable]
	public sealed class FilePath : PathString,
	                               ICloneable<FilePath>
	{
		#region Static Methods

		/// <summary>
		///     Implicit conversion of a <see cref="string" /> to <see cref="FilePath" />.
		/// </summary>
		/// <param name="path"> The path to convert to a file path. </param>
		public static implicit operator FilePath (string path)
		{
			if (path == null)
			{
				return null;
			}

			return new FilePath(path);
		}

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="FilePath" />.
		/// </summary>
		/// <param name="path"> The path. </param>
		/// <remarks>
		///     <para>
		///         Using this constructor, wildcards and relative paths are not allowed and the type of the path must be clearly determinable.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="path" /> is null. </exception>
		public FilePath (string path)
			: this(PathProperties.FromPath(path, false, false, null))
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="FilePath" />.
		/// </summary>
		/// <param name="path"> The path. </param>
		/// <param name="allowWildcards"> Specifies whether wildcards are allowed or not. </param>
		/// <param name="allowRelatives"> Specifies whether relative directory names are allowed or not. </param>
		/// <param name="assumedType"> Optionally specifies the type of the path which is assumed if the type cannot be clearly determined through analysis of <paramref name="path" />. </param>
		/// <remarks>
		///     <para>
		///         See <see cref="PathProperties.FromPath" /> for more details about the parameters, especially <paramref name="assumedType" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="path" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="path" /> is not a valid file path. </exception>
		public FilePath (string path, bool allowWildcards, bool allowRelatives, PathType? assumedType)
			: this(PathProperties.FromPath(path, allowWildcards, allowRelatives, assumedType))
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="FilePath" />.
		/// </summary>
		/// <param name="path"> The <see cref="PathProperties" /> object which describes the path. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="path" /> is null. </exception>
		public FilePath (PathProperties path)
			: base(path)
		{
			this.FileNameParts = path.Name.Split(PathProperties.FileExtensionSeparator);
			this.FileName = path.Name;

			if (this.FileNameParts.Length == 1)
			{
				this.ExtensionWithoutDot = null;
				this.ExtensionWithDot = null;
				this.FileNameWithoutExtension = this.FileName;
			}
			else
			{
				this.ExtensionWithoutDot = this.FileNameParts[this.FileNameParts.Length - 1];
				this.ExtensionWithDot = PathProperties.FileExtensionSeparator + this.ExtensionWithoutDot;
				this.FileNameWithoutExtension = this.FileNameParts.ToList(0, this.FileNameParts.Length - 1).Join(PathProperties.FileExtensionSeparator);
			}
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the directory path of the file.
		/// </summary>
		/// <value>
		///     The directory path of the file or null if the file path does not specify a directory.
		/// </value>
		public DirectoryPath Directory
		{
			get
			{
				string parent = this.PathInternal.Parent;
				if (parent == null)
				{
					return null;
				}
				return new DirectoryPath(parent, this.PathInternal.AllowWildcards, this.PathInternal.AllowRelatives, this.Type);
			}
		}

		/// <summary>
		///     Gets the extension of the file name (with the dot).
		/// </summary>
		/// <value>
		///     The extension of the file name (with the dot) or null if the file name does not have an extension.
		/// </value>
		/// <remarks>
		///     <para>
		///         If the file name ends with a dot, this property has the value of an empty string.
		///     </para>
		/// </remarks>
		public string ExtensionWithDot { get; private set; }

		/// <summary>
		///     Gets the extension of the file name (without the dot).
		/// </summary>
		/// <value>
		///     The extension of the file name (without the dot) or null if the file name does not have an extension.
		/// </value>
		/// <remarks>
		///     <para>
		///         If the file name ends with a dot, this property has the value of an empty string.
		///     </para>
		/// </remarks>
		public string ExtensionWithoutDot { get; private set; }

		/// <summary>
		///     Gets the file name of the file path.
		/// </summary>
		/// <value>
		///     The file name of the file path, including name and extension.
		/// </value>
		public string FileName { get; private set; }

		/// <summary>
		///     Gets the file name without its extension.
		/// </summary>
		/// <value>
		///     The file name without its extension.
		/// </value>
		public string FileNameWithoutExtension { get; private set; }

		private string[] FileNameParts { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Creates a new file path with this file name but another directory.
		/// </summary>
		/// <param name="directory"> The new directory path. </param>
		/// <returns>
		///     The new file path.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="directory" /> is null. </exception>
		public FilePath ChangeDirectory (DirectoryPath directory)
		{
			if (directory == null)
			{
				throw new ArgumentNullException(nameof(directory));
			}

			return directory.Append(new FilePath(this.FileName, this.PathInternal.AllowWildcards, this.PathInternal.AllowRelatives, this.Type));
		}

		/// <summary>
		///     Creates a new file path with this file name and directory but another extension.
		/// </summary>
		/// <param name="extension"> The new extension. </param>
		/// <returns>
		///     The new file path.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="extension" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> The existing file name (without extension) plus <paramref name="extension" /> do not form a valid new file name. </exception>
		public FilePath ChangeExtension (string extension)
		{
			if (extension == null)
			{
				throw new ArgumentNullException(nameof(extension));
			}

			try
			{
				return this.Directory.Append(new FilePath(this.FileNameWithoutExtension + PathProperties.FileExtensionSeparator + extension, this.PathInternal.AllowWildcards, this.PathInternal.AllowRelatives, this.Type));
			}
			catch (InvalidPathArgumentException exception)
			{
				throw new InvalidPathArgumentException(nameof(extension), exception.Message);
			}
		}

		/// <summary>
		///     Creates a new file path with this directory but another file name (including extension).
		/// </summary>
		/// <param name="fileName"> The new file name including its extension. </param>
		/// <returns>
		///     The new file path.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="fileName" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="fileName" /> is not a valid new file name. </exception>
		public FilePath ChangeFileName (string fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException(nameof(fileName));
			}

			try
			{
				return this.Directory.Append(new FilePath(fileName, this.PathInternal.AllowWildcards, this.PathInternal.AllowRelatives, this.Type));
			}
			catch (InvalidPathArgumentException exception)
			{
				throw new InvalidPathArgumentException(nameof(fileName), exception.Message);
			}
		}

		/// <summary>
		///     Creates a new file path with this directory but another file name (keeping this extension).
		/// </summary>
		/// <param name="fileNameWithoutExtension"> The new file name without its extension. </param>
		/// <returns>
		///     The new file path.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="fileNameWithoutExtension" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="fileNameWithoutExtension" /> plus the existing extension do not form a valid new file name. </exception>
		public FilePath ChangeFileNameWithoutExtension (string fileNameWithoutExtension)
		{
			if (fileNameWithoutExtension == null)
			{
				throw new ArgumentNullException(nameof(fileNameWithoutExtension));
			}

			try
			{
				return this.Directory.Append(new FilePath(fileNameWithoutExtension + PathProperties.FileExtensionSeparator + this.ExtensionWithoutDot, this.PathInternal.AllowWildcards, this.PathInternal.AllowRelatives, this.Type));
			}
			catch (InvalidPathArgumentException exception)
			{
				throw new InvalidPathArgumentException(nameof(fileNameWithoutExtension), exception.Message);
			}
		}

		/// <summary>
		///     Creates an absolute file path out of this file path relative to a specified root path.
		/// </summary>
		/// <param name="root"> The root path. </param>
		/// <returns>
		///     The absolute file path using <paramref name="root" />.
		/// </returns>
		/// <remarks>
		///     <para>
		///         If this file path is already absolute, nothing is done and the same file path is returned.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="root" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="root" /> is not a rooted path. </exception>
		public FilePath ToAbsolutePath (DirectoryPath root)
		{
			if (root == null)
			{
				throw new ArgumentNullException(nameof(root));
			}

			if (!root.IsRooted)
			{
				throw new InvalidPathArgumentException(nameof(root));
			}

			if (this.IsRooted)
			{
				return this;
			}

			return new FilePath(PathProperties.MakeAbsolute(root.PathInternal, this.PathInternal));
		}

		/// <summary>
		///     Creates a relative file path out of this file path relative to a specified root path.
		/// </summary>
		/// <param name="root"> The root path. </param>
		/// <returns>
		///     The relative file path relative to <paramref name="root" />.
		/// </returns>
		/// <remarks>
		///     <para>
		///         If this file path is already relative, nothing is done and the same file path is returned.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="root" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="root" /> is not a rooted path. </exception>
		public FilePath ToRelativePath (DirectoryPath root)
		{
			if (root == null)
			{
				throw new ArgumentNullException(nameof(root));
			}

			if (!root.IsRooted)
			{
				throw new InvalidPathArgumentException(nameof(root));
			}

			if (!this.IsRooted)
			{
				return this;
			}

			return new FilePath(PathProperties.MakeRelative(root.PathInternal, this.PathInternal));
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected override PathString CloneInternal ()
		{
			return this.Clone();
		}

		#endregion




		#region Interface: ICloneable<FilePath>

		/// <inheritdoc />
		public FilePath Clone ()
		{
			return new FilePath(this.GetPathProperties());
		}

		#endregion
	}
}
