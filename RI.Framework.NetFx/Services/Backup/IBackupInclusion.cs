using System;
using System.Collections.Generic;




namespace RI.Framework.Services.Backup
{
	/// <summary>
	/// Defines the interface for a backup inclusion.
	/// </summary>
	public interface IBackupInclusion : IEquatable<IBackupInclusion>
	{
		/// <summary>
		/// Gets the ID of the inclusion.
		/// </summary>
		/// <value>
		/// The ID of the inclusion.
		/// </value>
		/// <remarks>
		/// <para>
		/// The ID is used to uniquely identify an inclusion, allowing the ID to be serialized.
		/// </para>
		/// </remarks>
		Guid Id { get; }

		/// <summary>
		/// Gets the resource key of the inclusion.
		/// </summary>
		/// <value>
		/// The resource key of the inclusion or null if no resource key is specified.
		/// </value>
		/// <remarks>
		/// <para>
		/// The resource key is used to associate a resource with the inclusion, e.g. a text resource for displaying the name of the inclusion to the user.
		/// </para>
		/// </remarks>
		string ResourceKey { get; }

		/// <summary>
		/// Gets whether the inclusion can be restored.
		/// </summary>
		/// <value>
		/// true if the inclusion can be restored, false otherwise.
		/// </value>
		bool SupportsRestore { get; }

		/// <summary>
		/// Gets the dictionary which can be used to store additional data about an inclusion.
		/// </summary>
		/// <value>
		/// The dictionary which can be used to store additional data about an inclusion.
		/// </value>
		/// <remarks>
		/// <note type="implement">
		/// This property must never be null.
		/// </note>
		/// </remarks>
		IDictionary<string, string> Tags { get; }

		/// <summary>
		/// Gets the set containing the stream IDs associated with this inclusion.
		/// </summary>
		/// <value>
		/// The set containing the stream IDs associated with this inclusion.
		/// </value>
		/// <remarks>
		/// <note type="implement">
		/// This property must never be null.
		/// </note>
		/// </remarks>
		ISet<Guid> Streams { get; }
	}
}
