using System;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Settings
{
	/// <summary>
	///     Defines the interface for a setting storage used by a setting service.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A setting storage is used by a <see cref="ISettingService" /> to actually persistently store the values as their string representations.
	///         Values are stored as name/value pairs.
	///     </para>
	/// </remarks>
	[Export]
	public interface ISettingStorage
	{
		/// <summary>
		///     Gets whether the setting storage only supports loading and retriving of values but not setting and saving.
		/// </summary>
		/// <value>
		///     true if the setting storage only supports loading and retrieving.
		/// </value>
		bool IsReadOnly { get; }

		/// <summary>
		///     Reads a value.
		/// </summary>
		/// <param name="name"> The name of the value. </param>
		/// <returns>
		///     The value or null if the value is not available.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		string GetValue (string name);

		/// <summary>
		///     Checks whether a value is available.
		/// </summary>
		/// <param name="name"> The name of the value. </param>
		/// <returns>
		///     true if the value is available, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		bool HasValue (string name);

		/// <summary>
		///     Reads, if necessary and applicable, all values from the storage.
		/// </summary>
		void Load ();

		/// <summary>
		///     Writes, if necessary and applicable, all values to the storage, making them persistent.
		/// </summary>
		/// <exception cref="InvalidOperationException"> The setting stoarge is read-only. </exception>
		void Save ();

		/// <summary>
		///     Creates, deletes, or updates a value.
		/// </summary>
		/// <param name="name"> The name of the value. </param>
		/// <param name="value"> The actual value or null to delete the value. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		/// <exception cref="InvalidOperationException"> The setting stoarge is read-only. </exception>
		void SetValue (string name, string value);
	}
}
