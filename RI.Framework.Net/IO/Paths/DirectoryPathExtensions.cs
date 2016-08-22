using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

using RI.Framework.Utilities;




namespace RI.Framework.IO.Paths
{
	public static class DirectoryPathExtensions
	{
		[DllImport("kernel32.dll", SetLastError = false, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, int dwFlags);

		[Obsolete("Does only work when executed as administrator.")]
		public bool DeleteAsap()
		{
			return (this.DeleteAsap(false));
		}

		[Obsolete("Does only work when executed as administrator.")]
		public bool DeleteAsap(bool deleteFilesSeparately)
		{
			if (!this.Exists)
			{
				return true;
			}

			if (deleteFilesSeparately)
			{
				FilePath[] files = this.FindAllFiles(true);
				foreach (FilePath file in files)
				{
					file.DeleteAsap();
				}
			}

			if (this.TryDelete())
			{
				return true;
			}

			this.DeleteReboot(deleteFilesSeparately);
			return false;
		}

		[Obsolete("Does only work when executed as administrator.")]
		public bool DeleteReboot()
		{
			return (this.DeleteReboot(false));
		}

		[Obsolete("Does only work when executed as administrator.")]
		public bool DeleteReboot(bool markFilesSeparately)
		{
			if (!this.Exists)
			{
				return true;
			}

			if (markFilesSeparately)
			{
				FilePath[] files = this.FindAllFiles(true);
				foreach (FilePath file in files)
				{
					file.DeleteReboot();
				}
			}

			DirectoryPath.MoveFileEx(this.Path, null, DirectoryPath.MoveFileDelayUntilReboot);

			return false;
		}

		public DirectorySecurity GetAccessControl(AccessControlSections sections)
		{
			return (Directory.GetAccessControl(this.Path, sections));
		}

		public void SetAccessControl(DirectorySecurity accessControl)
		{
			Directory.SetAccessControl(this.Path, accessControl);
		}

		public static DirectoryPath GetCurrentDirectory()
		{
			return (new DirectoryPath(Environment.CurrentDirectory));
		}

		public static DirectoryPath GetTempDirectory()
		{
			return (new DirectoryPath(Path.GetTempPath()));
		}

		public DateTime CreationTime
		{
			get
			{
				return (Directory.GetCreationTime(this.Path));
			}
		}

		public DateTime CreationTimeUtc
		{
			get
			{
				return (this.CreationTime.ToUniversalTime());
			}
		}

		public bool Exists
		{
			get
			{
				return (Directory.Exists(this.Path));
			}
		}

		public bool IsEmpty
		{
			get
			{
				return (this.Path.IsEmpty());
			}
		}

		public DateTime LastAccessTime
		{
			get
			{
				return (Directory.GetLastAccessTime(this.Path));
			}
		}

		public DateTime LastAccessTimeUtc
		{
			get
			{
				return (this.LastAccessTime.ToUniversalTime());
			}
		}

		public DateTime LastWriteTime
		{
			get
			{
				return (Directory.GetLastWriteTime(this.Path));
			}
		}

		public DateTime LastWriteTimeUtc
		{
			get
			{
				return (this.LastWriteTime.ToUniversalTime());
			}
		}

		public long Size
		{
			get
			{
				FilePath[] files = this.FindAllFiles(true);
				long size = (from x in files select x.Size).Sum();
				return (size);
			}
		}

		public bool ContainsFile(FilePath file, bool recursive)
		{
			if (file == null)
			{
				throw (new ArgumentNullException(nameof(file)));
			}

			if (file.IsAbsolute)
			{
				throw (new PathNotRelativeArgumentException(nameof(file)));
			}

			FilePath fileToCheck = file.ToAbsolutePath(this);
			FilePath[] filesFound = fileToCheck.FindWildcardedFiles(recursive);
			return (filesFound.Length > 0);
		}

		public void Copy(DirectoryPath target)
		{
			this.Copy(target, false);
		}

		public void Copy(DirectoryPath target, bool overwrite)
		{
			if (target == null)
			{
				throw (new ArgumentNullException(nameof(target)));
			}

			target.Create();

			FilePath[] files = this.FindAllFiles(false);
			DirectoryPath[] directories = this.FindAllDirectories(false);

			foreach (FilePath file in files)
			{
				file.Copy(target, overwrite);
			}

			foreach (DirectoryPath directory in directories)
			{
				directory.Copy(target.Append(new DirectoryPath(directory.DirectoryName)), overwrite);
			}
		}

		public bool Create()
		{
			if (this.Exists)
			{
				return (false);
			}

			Directory.CreateDirectory(this.Path);

			return (true);
		}

		public void CreateNew()
		{
			this.Delete();
			this.Create();
		}

		public void Delete()
		{
			if (!this.Exists)
			{
				return;
			}

			Directory.Delete(this.Path, true);
		}

		public DirectoryPath[] FindAllDirectories(bool recursive)
		{
			return (this.FindDirectories(null, recursive));
		}

		public FilePath[] FindAllFiles(bool recursive)
		{
			return (this.FindFiles(null, recursive));
		}

		public DirectoryPath[] FindDirectories(string searchPattern, bool recursive)
		{
			searchPattern = searchPattern == null ? null : (searchPattern.IsEmpty() ? null : searchPattern);
			searchPattern = searchPattern ?? "*";

			List<DirectoryPath> subdirectories = new List<DirectoryPath>();

			if (this.Exists)
			{
				string[] foundDirectories = recursive ? this.FindDirectoriesRecursive(this, searchPattern) : Directory.GetDirectories(this.Path, searchPattern);
				foreach (string foundDirectory in foundDirectories)
				{
					subdirectories.Add(new DirectoryPath(foundDirectory));
				}
			}

			return (subdirectories.ToArray());
		}

		public FilePath[] FindFiles(string searchPattern, bool recursive)
		{
			searchPattern = searchPattern == null ? null : (searchPattern.IsEmpty() ? null : searchPattern);
			searchPattern = searchPattern ?? "*";

			List<FilePath> files = new List<FilePath>();

			if (this.Exists)
			{
				string[] foundFiles = recursive ? this.FindFilesRecursive(this, searchPattern) : Directory.GetFiles(this.Path, searchPattern);
				foreach (string foundFile in foundFiles)
				{
					files.Add(new FilePath(foundFile));
				}
			}

			return (files.ToArray());
		}

		public DirectoryPath[] FindWildcardedDirectories(bool recursive)
		{
			if (this.IsRoot)
			{
				return (new DirectoryPath[0]);
			}

			return (this.Parent.FindDirectories(this.DirectoryName, recursive));
		}

		public void Move(DirectoryPath target)
		{
			if (target == null)
			{
				throw (new ArgumentNullException(nameof(target)));
			}

			Directory.Move(this.Path, target.Path);
		}

		public bool TryDelete()
		{
			try
			{
				this.Delete();
			}
			catch
			{
			}

			return !this.Exists;
		}

		public bool TryDeleteAsMuchAsPossible()
		{
			this.FindAllDirectories(true).ForEach(x => x.TryDelete());
			this.FindAllFiles(true).ForEach(x => x.TryDelete());
			return this.TryDelete();
		}

		private string[] FindDirectoriesRecursive(DirectoryPath directory, string searchPattern)
		{
			List<string> directories = new List<string>();

			directories.AddRange(Directory.GetDirectories(directory.Path, searchPattern));

			foreach (string dir in Directory.GetDirectories(directory.Path, "*"))
			{
				directories.AddRange(this.FindDirectoriesRecursive(dir.ToDirectoryPath(), searchPattern));
			}

			return (directories.ToArray());
		}

		private string[] FindFilesRecursive(DirectoryPath directory, string searchPattern)
		{
			List<string> files = new List<string>();

			files.AddRange(Directory.GetFiles(directory.Path, searchPattern));

			foreach (string dir in Directory.GetDirectories(directory.Path, "*"))
			{
				files.AddRange(this.FindFilesRecursive(dir.ToDirectoryPath(), searchPattern));
			}

			return (files.ToArray());
		}
	}
}