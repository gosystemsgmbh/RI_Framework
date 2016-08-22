using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading;




namespace RI.Framework.IO.Paths
{
	public static class FilePathExtensions
	{
		[DllImport("kernel32.dll", SetLastError = false, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, int dwFlags);

		public static FilePath GetTempFile()
		{
			return (new FilePath(Path.GetTempFileName()));
		}

		public static FilePath GetTempFile(DirectoryPath tempDirectory)
		{
			if (tempDirectory == null)
			{
				throw new ArgumentNullException(nameof(tempDirectory));
			}

			FilePath file = tempDirectory.Append(new FilePath(Guid.NewGuid().ToString("N") + FilePath.TemporaryExtension));
			file.Create();
			return file;
		}

		public DateTime CreationTime
		{
			get
			{
				return (File.GetCreationTime(this.Path));
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
				return (File.Exists(this.Path));
			}
		}

		public TimeSpan FileAge
		{
			get
			{
				TimeSpan writeAge = TimeSpan.Zero;
				TimeSpan createAge = TimeSpan.Zero;

				do
				{
					DateTime now = DateTime.UtcNow;

					writeAge = now.Subtract(this.LastWriteTimeUtc);
					createAge = now.Subtract(this.CreationTimeUtc);
				}
				while (createAge.IsNegative() || writeAge.IsNegative());

				if (writeAge > createAge)
				{
					return (createAge);
				}

				return (writeAge);
			}
		}

		public DateTime LastAccessTime
		{
			get
			{
				return (File.GetLastAccessTime(this.Path));
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
				return (File.GetLastWriteTime(this.Path));
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
				FileInfo fileInfo = new FileInfo(this.Path);
				return (fileInfo.Length);
			}
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

			File.Copy(this.Path, Path.Combine(target.Path, this.FileName), overwrite);
		}

		public void Copy(FilePath target)
		{
			this.Copy(target, false);
		}

		public void Copy(FilePath target, bool overwrite)
		{
			if (target == null)
			{
				throw (new ArgumentNullException(nameof(target)));
			}

			target.Directory.Create();

			File.Copy(this.Path, target.Path, overwrite);
		}

		public bool Create()
		{
			if (this.Exists)
			{
				return (false);
			}

			this.Directory.Create();

			using (FileStream fs = this.OpenStream(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
			{
				fs.Close();
			}

			return (true);
		}

		public void CreateNew()
		{
			this.Directory.Create();

			using (FileStream fs = this.OpenStream(FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
			{
				fs.Close();
			}
		}

		public void Delete()
		{
			if (!this.Exists)
			{
				return;
			}

			File.Delete(this.Path);
		}

		[Obsolete("Does only work when executed as administrator.")]
		public bool DeleteAsap()
		{
			if (this.TryDelete())
			{
				return true;
			}

			this.DeleteReboot();
			return false;
		}

		public bool DeleteConcurrentImmediate()
		{
			return (this.DeleteConcurrentImmediate(null));
		}

		public bool DeleteConcurrentImmediate(ConcurrentFileAccessParameters concurrentFileAccessParameters)
		{
			concurrentFileAccessParameters = concurrentFileAccessParameters ?? new ConcurrentFileAccessParameters();

			if (!this.Exists)
			{
				return (true);
			}

			try
			{
				this.Delete();
			}
			catch
			{
				return (false);
			}

			return (true);
		}

		public bool DeleteConcurrentWait()
		{
			return (this.DeleteConcurrentWait(null));
		}

		public bool DeleteConcurrentWait(ConcurrentFileAccessParameters concurrentFileAccessParameters)
		{
			concurrentFileAccessParameters = concurrentFileAccessParameters ?? new ConcurrentFileAccessParameters();

			DateTime start = DateTime.UtcNow;

			while (DateTime.UtcNow.Subtract(start) < concurrentFileAccessParameters.AccessTimeout)
			{
				if (this.DeleteConcurrentImmediate(concurrentFileAccessParameters))
				{
					return (true);
				}

				Thread.Sleep(concurrentFileAccessParameters.AccessInterval);
			}

			return (false);
		}

		[Obsolete("Does only work when executed as administrator.")]
		public bool DeleteReboot()
		{
			if (!this.Exists)
			{
				return true;
			}

			FilePath.MoveFileEx(this.Path, null, FilePath.MoveFileDelayUntilReboot);

			return false;
		}

		public bool ExistConcurrentImmediate()
		{
			return (this.ExistConcurrentImmediate(null));
		}

		public bool ExistConcurrentImmediate(ConcurrentFileAccessParameters concurrentFileAccessParameters)
		{
			concurrentFileAccessParameters = concurrentFileAccessParameters ?? new ConcurrentFileAccessParameters();

			if (!this.Exists)
			{
				return (false);
			}

			if (concurrentFileAccessParameters.MinFileAge > this.FileAge)
			{
				return (false);
			}

			return (true);
		}

		public bool ExistConcurrentWait()
		{
			return (this.ExistConcurrentWait(null));
		}

		public bool ExistConcurrentWait(ConcurrentFileAccessParameters concurrentFileAccessParameters)
		{
			concurrentFileAccessParameters = concurrentFileAccessParameters ?? new ConcurrentFileAccessParameters();

			DateTime start = DateTime.UtcNow;

			while (DateTime.UtcNow.Subtract(start) < concurrentFileAccessParameters.AccessTimeout)
			{
				if (this.ExistConcurrentImmediate(concurrentFileAccessParameters))
				{
					return (true);
				}

				Thread.Sleep(concurrentFileAccessParameters.AccessInterval);
			}

			return (false);
		}

		public FilePath[] FindWildcardedFiles(bool recursive)
		{
			return (this.Directory.FindFiles(this.FileName, recursive));
		}

		public FileSecurity GetAccessControl(AccessControlSections sections)
		{
			return (File.GetAccessControl(this.Path, sections));
		}

		public void Move(DirectoryPath target)
		{
			if (target == null)
			{
				throw (new ArgumentNullException(nameof(target)));
			}

			target.Create();

			File.Move(this.Path, Path.Combine(target.Path, this.FileName));
		}

		public void Move(FilePath target)
		{
			if (target == null)
			{
				throw (new ArgumentNullException(nameof(target)));
			}

			target.Directory.Create();

			File.Move(this.Path, target.Path);
		}

		public StreamReader OpenReader(Encoding encoding, bool detectEncodingFromByteOrderMarks)
		{
			encoding = encoding ?? Encoding.UTF8;
			return (new StreamReader(this.Path, encoding, detectEncodingFromByteOrderMarks));
		}

		public FileStream OpenStream(FileMode mode, FileAccess access, FileShare share)
		{
			return (new FileStream(this.Path, mode, access, share));
		}

		public StreamWriter OpenWriter(bool append, Encoding encoding)
		{
			encoding = encoding ?? Encoding.UTF8;
			return (new StreamWriter(this.Path, append, encoding));
		}

		public byte[] ReadBinary()
		{
			if (!this.Exists)
			{
				return (null);
			}

			using (FileStream fs = this.OpenStream(FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				return (fs.Read());
			}
		}

		public int? ReadBinary(byte[] data, int offset, int count)
		{
			if (data == null)
			{
				throw (new ArgumentNullException(nameof(data)));
			}

			if (!this.Exists)
			{
				return (null);
			}

			using (FileStream fs = this.OpenStream(FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				return (fs.Read(data, offset, count));
			}
		}

		public int? ReadBinary(Stream data)
		{
			if (data == null)
			{
				throw (new ArgumentNullException(nameof(data)));
			}

			if (!data.CanWrite)
			{
				throw (new StreamNotWritableArgumentException(nameof(data)));
			}

			if (!this.Exists)
			{
				return (null);
			}

			using (FileStream fs = this.OpenStream(FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				return (fs.Read(data));
			}
		}

		public string ReadText()
		{
			return (this.ReadText(null));
		}

		public string ReadText(Encoding encoding)
		{
			encoding = encoding ?? Encoding.UTF8;

			if (!this.Exists)
			{
				return (null);
			}

			using (FileStream fs = this.OpenStream(FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (StreamReader sr = new StreamReader(fs, encoding, true))
				{
					return (sr.ReadToEnd());
				}
			}
		}

		public void SetAccessControl(FileSecurity accessControl)
		{
			File.SetAccessControl(this.Path, accessControl);
		}

		public bool TryDelete()
		{
			try
			{
				this.Delete();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public void WriteBinary(byte[] data, bool append)
		{
			this.WriteBinary(data, 0, data.Length, append);
		}

		public void WriteBinary(byte[] data, int offset, int count, bool append)
		{
			if (data == null)
			{
				throw (new ArgumentNullException(nameof(data)));
			}

			this.Directory.Create();

			using (FileStream fs = this.OpenStream(append ? FileMode.OpenOrCreate : FileMode.Create, FileAccess.Write, FileShare.Read))
			{
				fs.Seek(0, append ? SeekOrigin.End : SeekOrigin.Begin);
				fs.Write(data, offset, count);
			}
		}

		public int WriteBinary(Stream data, bool append)
		{
			if (data == null)
			{
				throw (new ArgumentNullException(nameof(data)));
			}

			if (!data.CanRead)
			{
				throw (new StreamNotReadableArgumentException(nameof(data)));
			}

			this.Directory.Create();

			using (FileStream fs = this.OpenStream(append ? FileMode.OpenOrCreate : FileMode.Create, FileAccess.Write, FileShare.Read))
			{
				fs.Seek(0, append ? SeekOrigin.End : SeekOrigin.Begin);
				return (data.Read(fs));
			}
		}

		public void WriteText(string data, bool append)
		{
			this.WriteText(data, append, null);
		}

		public void WriteText(string data, bool append, Encoding encoding)
		{
			if (data == null)
			{
				throw (new ArgumentNullException(nameof(data)));
			}

			this.Directory.Create();

			encoding = encoding ?? Encoding.UTF8;

			using (FileStream fs = this.OpenStream(append ? FileMode.OpenOrCreate : FileMode.Create, FileAccess.Write, FileShare.Read))
			{
				fs.Seek(0, append ? SeekOrigin.End : SeekOrigin.Begin);
				using (StreamWriter sw = new StreamWriter(fs, encoding))
				{
					sw.Write(data);
				}
			}
		}
	}
}