using System;
using System.Collections.Generic;

using RI.Framework.IO.Paths;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Backup.Storages
{
	/// <summary>
	///     Defines the interface for a single backup set.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IBackupService" /> for more details.
	///     </para>
	///     <note type="implement">
	///         <see cref="IBackupSet" /> is typically implemented internally by an <see cref="IBackupStorage" /> implementation.
	///     </note>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public interface IBackupSet : ISynchronizable, IEquatable<IBackupSet>
	{
		/// <summary>
		///     Gets whether the backup set can be exported to a file.
		/// </summary>
		/// <value>
		///     true if the backup set can be exported to a file, false otherwise.
		/// </value>
		bool CanExportToFile { get; }

		/// <summary>
		///     Gets whether the backup set can be restored.
		/// </summary>
		/// <value>
		///     true if the backup set, depending on the backup storage it is associated with, and at least one inclusion can be restored, false otherwise.
		/// </value>
		bool CanRestore { get; }

		/// <summary>
		///     Gets the name of the backup.
		/// </summary>
		/// <value>
		///     The name of the backup or null if no name is defined.
		/// </value>
		string Name { get; }

		/// <summary>
		///     Gets the physical size of the backup in bytes.
		/// </summary>
		/// <value>
		///     The physical size of the backup in bytes or null if the information about the physical size is not provided/available.
		/// </value>
		long? SizeInBytes { get; }

		/// <summary>
		///     Gets the timestamp when the backup was created.
		/// </summary>
		/// <value>
		///     The timestamp when the backup was created.
		/// </value>
		DateTime Timestamp { get; }

		/// <summary>
		///     Exports the backup set to a file.
		/// </summary>
		/// <param name="targetFile"> The file. </param>
		/// <remarks>
		///     <para>
		///         The behaviour and implementation is defined by the <see cref="IBackupStorage" /> the backup set is associated with.
		///     </para>
		///     <note type="note">
		///         Do not call this method directly, it is intended to be called from an <see cref="IBackupService" /> implementation.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="targetFile" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="targetFile" /> is an invalid path. </exception>
		/// <exception cref="InvalidOperationException"> The backup set cannot be copied to a file. </exception>
		void ExportToFile (FilePath targetFile);

		/// <summary>
		///     Gets the list of inclusions contained in the backup set.
		/// </summary>
		/// <returns>
		///     The list of inclusions contained in the backup set.
		///     If no inclusions are contained, an empty list is returned.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         Do not call this method directly, it is intended to be called from an <see cref="IBackupService" /> implementation.
		///     </note>
		/// </remarks>
		List<IBackupInclusion> GetInclusions ();
	}
}
