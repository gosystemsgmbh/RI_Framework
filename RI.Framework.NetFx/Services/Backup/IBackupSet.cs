using System;
using System.Collections.Generic;

using RI.Framework.IO.Paths;
using RI.Framework.Services.Backup.Storages;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Backup
{
	/// <summary>
	/// Defines the interface for a single backup set.
	/// </summary>
	/// <remarks>
	/// <para>
	/// See <see cref="IBackupService"/> for more details.
	/// </para>
	/// <note type="implement">
	/// <see cref="IBackupSet"/> is typically implemented internally by an <see cref="IBackupStorage"/> implementation.
	/// </note>
	/// </remarks>
	public interface IBackupSet : IEquatable<IBackupSet>
	{
		/// <summary>
		/// Gets whether the backup set can be copied to a directory.
		/// </summary>
		/// <value>
		/// true if the backup set can be copied to a directory, false otherwise.
		/// </value>
		bool CanCopyToDirectory { get; }

		/// <summary>
		/// Gets whether the backup set can be copied to a file.
		/// </summary>
		/// <value>
		/// true if the backup set can be copied to a file, false otherwise.
		/// </value>
		bool CanCopyToFile { get; }

		/// <summary>
		/// Gets the sequence of inclusions contained in the backup set.
		/// </summary>
		/// <value>
		/// The sequence of inclusions contained in the backup set.
		/// </value>
		IEnumerable<IBackupInclusion> Inclusions { get; }

		/// <summary>
		/// Gets the name of the backup.
		/// </summary>
		/// <value>
		/// The name of the backup or null if no name is defined.
		/// </value>
		string Name { get; }

		/// <summary>
		/// Gets the physical size of the backup in bytes.
		/// </summary>
		/// <value>
		/// The physical size of the backup in bytes.
		/// </value>
		long SizeInBytes { get; }

		/// <summary>
		/// Gets the timestamp when the backup was created.
		/// </summary>
		/// <value>
		/// The timestamp when the backup was created.
		/// </value>
		DateTime Timestamp { get; }

		/// <summary>
		/// Gets whether the backup set can be restored.
		/// </summary>
		/// <value>
		/// true if the backup set, depending on the backup storage it is associated with, and at least one inclusion can be restored, false otherwise.
		/// </value>
		bool CanRestore { get; }

		/// <summary>
		/// Copies the backup set to a directory.
		/// </summary>
		/// <param name="targetDirectory">The directory.</param>
		/// <remarks>
		/// <para>
		/// The behaviour and implementation is defined by the <see cref="IBackupStorage"/> the backup set is associated with.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="targetDirectory"/> is null.</exception>
		/// <exception cref="InvalidPathArgumentException"><paramref name="targetDirectory"/> is an invalid path.</exception>
		/// <exception cref="InvalidOperationException">The backup set cannot be copied to a directory.</exception>
		void CopyToDirectory (DirectoryPath targetDirectory);

		/// <summary>
		/// Copies the backup set to a file.
		/// </summary>
		/// <param name="targetFile">The file.</param>
		/// <remarks>
		/// <para>
		/// The behaviour and implementation is defined by the <see cref="IBackupStorage"/> the backup set is associated with.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="targetFile"/> is null.</exception>
		/// <exception cref="InvalidPathArgumentException"><paramref name="targetFile"/> is an invalid path.</exception>
		/// <exception cref="InvalidOperationException">The backup set cannot be copied to a file.</exception>
		bool CopyToFile (FilePath targetFile);
	}
}
