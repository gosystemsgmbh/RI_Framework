using System;
using System.Collections.Generic;
using System.IO;

using RI.Framework.IO.Paths;
using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.IO.Files
{
	/// <summary>
	///     Implements temporary file handling.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="TemporaryFile" /> is used for conveniently working with temporary files and a common directory where they are located.
	///     </para>
	///     <para>
	///         <see cref="TemporaryFile" /> implements <see cref="IDisposable" /> which attempts to delete the file when <see cref="IDisposable.Dispose" /> is called (but does not fail if not successful).
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class TemporaryFile : IDisposable, ISynchronizable
	{
		#region Static Constructor/Destructor

		static TemporaryFile ()
		{
			TemporaryFile.GlobalSyncRoot = new object();
		}

		#endregion




		#region Static Fields

		private static DirectoryPath _temporaryDirectory;

		#endregion




		#region Static Properties/Indexer

		/// <summary>
		///     Gets or sets the directory in which temporary files are to be created by default.
		/// </summary>
		/// <value>
		///     The directory in which temporary files are to be created by default or null if <see cref="DirectoryPath.GetTempDirectory" /> should be used.
		///     The default value is null.
		/// </value>
		public static DirectoryPath TemporaryDirectory
		{
			get
			{
				lock (TemporaryFile.GlobalSyncRoot)
				{
					return TemporaryFile._temporaryDirectory;
				}
			}
			set
			{
				lock (TemporaryFile.GlobalSyncRoot)
				{
					TemporaryFile._temporaryDirectory = value;
				}
			}
		}

		private static object GlobalSyncRoot { get; set; }

		#endregion




		#region Static Methods

		/// <summary>
		///     Deletes as much temporary files as possible in <see cref="TemporaryDirectory" />.
		/// </summary>
		/// <returns>
		///     The list of deleted files.
		///     If no files were deleted, an empty list is returned.
		/// </returns>
		/// <remarks>
		///     <note type="important">
		///         All files, regardles of name or extension, which are in <see cref="TemporaryDirectory" /> are deleted.
		///         Subdirectories are ignored.
		///     </note>
		///     <note type="important">
		///         If <see cref="TemporaryDirectory" /> is null, <see cref="DirectoryPath.GetTempDirectory" /> is used.
		///     </note>
		/// </remarks>
		public static List<FilePath> DeleteTemporaryFiles ()
		{
			lock (TemporaryFile.GlobalSyncRoot)
			{
				List<FilePath> files = TemporaryFile.GetTemporaryFiles();
				List<FilePath> deletedFiles = new List<FilePath>();
				foreach (FilePath file in files)
				{
					try
					{
						file.Delete();
						deletedFiles.Add(file);
					}
					catch
					{
						//This means that we have no access to the file or it is still in use
					}
				}
				return deletedFiles;
			}
		}

		/// <summary>
		///     Uses a specified directory to create a new temporary file in it.
		/// </summary>
		/// <param name="directory"> The directory in which a new temporary file is to be created. </param>
		/// <returns>
		///     The temporary file.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="directory" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The directory contains wildcards. </exception>
		/// <exception cref="DirectoryNotFoundException"> The directory does not exist. </exception>
		public static TemporaryFile FromExistingDirectory (DirectoryPath directory)
		{
			if (directory == null)
			{
				throw new ArgumentNullException(nameof(directory));
			}

			directory.VerifyRealDirectory();

			if (!directory.Exists)
			{
				throw new DirectoryNotFoundException("Directory not found: " + directory);
			}

			return new TemporaryFile(directory.AppendFile(TemporaryFile.CreateTemporaryFileName()));
		}

		/// <summary>
		///     Uses an existing file as a temporary file.
		/// </summary>
		/// <param name="file"> The file to be used as a temporary file. </param>
		/// <returns>
		///     The temporary file.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="file" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The file contains wildcards. </exception>
		/// <exception cref="FileNotFoundException"> The file does not exist. </exception>
		public static TemporaryFile FromExistingFile (FilePath file)
		{
			if (file == null)
			{
				throw new ArgumentNullException(nameof(file));
			}

			file.VerifyRealFile();

			if (!file.Exists)
			{
				throw new FileNotFoundException("File not found: " + file);
			}

			return new TemporaryFile(file);
		}

		/// <summary>
		///     Gets all temporary files currently in <see cref="TemporaryDirectory" />.
		/// </summary>
		/// <returns>
		///     The list of temporary files.
		///     If there are not temporary files in <see cref="TemporaryDirectory" />, an empty list is returned.
		/// </returns>
		/// <remarks>
		///     <note type="important">
		///         All files, regardles of name or extension, which are in <see cref="TemporaryDirectory" /> are returned.
		///         Subdirectories are ignored.
		///     </note>
		///     <note type="important">
		///         If <see cref="TemporaryDirectory" /> is null, <see cref="DirectoryPath.GetTempDirectory" /> is used.
		///     </note>
		/// </remarks>
		public static List<FilePath> GetTemporaryFiles ()
		{
			lock (TemporaryFile.GlobalSyncRoot)
			{
				return TemporaryFile.GetTempDirectory().GetFiles(false, false, "*");
			}
		}

		private static string CreateTemporaryFileName () => Guid.NewGuid().ToString("N") + DirectoryPath.TemporaryExtension;

		private static DirectoryPath GetTempDirectory () => TemporaryFile.TemporaryDirectory ?? DirectoryPath.GetTempDirectory();

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="TemporaryFile" />.
		/// </summary>
		/// <remarks>
		///     <para>
		///         A new temporary file is created in the directory specified by <see cref="TemporaryDirectory" />.
		///         If <see cref="TemporaryDirectory" /> is null, <see cref="DirectoryPath.GetTempDirectory" /> is used.
		///     </para>
		/// </remarks>
		public TemporaryFile ()
		{
			this.SyncRoot = new object();

			lock (TemporaryFile.GlobalSyncRoot)
			{
				this.File = TemporaryFile.GetTempDirectory().AppendFile(TemporaryFile.CreateTemporaryFileName());
			}

			this.File.CreateIfNotExist();
		}

		private TemporaryFile (FilePath file)
		{
			this.SyncRoot = new object();

			this.File = file;
			this.File.CreateIfNotExist();
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="TemporaryFile" />.
		/// </summary>
		~TemporaryFile ()
		{
			this.Delete();
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the temporary file.
		/// </summary>
		/// <value>
		///     The temporary file.
		/// </value>
		public FilePath File { get; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Attempts to delete the temporary file.
		/// </summary>
		/// <returns>
		///     true if the file was deleted, false otherwise.
		/// </returns>
		public bool Delete ()
		{
			try
			{
				lock (this.SyncRoot)
				{
					return this.File.Delete();
				}
			}
			catch
			{
				//This means that we have no access to the file or it is still in use
				return false;
			}
		}

		#endregion




		#region Interface: IDisposable

		/// <inheritdoc />
		void IDisposable.Dispose ()
		{
			this.Delete();
		}

		#endregion


		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; }
	}
}
