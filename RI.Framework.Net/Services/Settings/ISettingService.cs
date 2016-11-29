using System;
using System.Collections.Generic;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Settings
{
	/// <summary>
	///     Defines the interface for a setting service.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A setting service is used to manage and persistently store settings between two sessions.
	///     </para>
	///     <para>
	///         A setting service stores the settings as strings in its underlying storages represented by <see cref="ISettingStorage" />s and uses <see cref="ISettingConverter" />s to convert between the strings and the values.
	///     </para>
	///     <para>
	///         Names of settings are considered case-insensitive.
	///     </para>
	///     <note type="note">
	///         A setting storage can be either read-only or not (see <see cref="ISettingStorage.IsReadOnly" />).
	///         Values are persisted with all available non-read-only setting storages and therefore are saved multiple times if multiple non-read-only setting storages are available.
	///     </note>
	///     <note type="note">
	///         When retrieving values, values from read-only setting storages have higher priority if they have the same name.
	///         Otherwise, the order is undefined for values with the same name.
	///     </note>
	/// </remarks>
	/// TODO: Non-generic GetValue, SetValue, InitializeValue method
	[Export]
	public interface ISettingService
	{
		/// <summary>
		///     Gets all currently available setting converters.
		/// </summary>
		/// <value>
		///     All currently available setting converters.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		IEnumerable<ISettingConverter> Converters { get; }

		/// <summary>
		///     Gets all currently available setting storages.
		/// </summary>
		/// <value>
		///     All currently available setting storages.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		IEnumerable<ISettingStorage> Storages { get; }

		/// <summary>
		///     Adds a setting converter and starts using it for all subsequent conversions.
		/// </summary>
		/// <param name="settingConverter"> The setting converter to add. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already added setting converter should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="settingConverter" /> is null. </exception>
		void AddConverter (ISettingConverter settingConverter);

		/// <summary>
		///     Adds a setting storage and starts using it for all subsequent operations.
		/// </summary>
		/// <param name="settingStorage"> The setting storage to add. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already added setting storage should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="settingStorage" /> is null. </exception>
		void AddStorage (ISettingStorage settingStorage);

		/// <summary>
		///     Deletes a setting with a specified name.
		/// </summary>
		/// <param name="name"> The name of the setting to delete. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		void DeleteValue (string name);

		/// <summary>
		///     Gets a setting in its string representation.
		/// </summary>
		/// <param name="name"> The name of the setting. </param>
		/// <returns>
		///     The setting value in its string representation or null if the setting is not available.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		string GetRawValue (string name);

		/// <summary>
		///     Gets a setting as a value of a certain type.
		/// </summary>
		/// <typeparam name="T"> The setting type. </typeparam>
		/// <param name="name"> The name of the setting. </param>
		/// <returns>
		///     The setting value or the default value of <typeparamref name="T" /> if the setting is not available.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The specified <typeparamref name="T" /> is not supported by any setting converter. </exception>
		T GetValue <T> (string name);

		/// <summary>
		///     Determines whether a setting with a specified name is available.
		/// </summary>
		/// <param name="name"> The name of the setting to check. </param>
		/// <returns>
		///     true if the setting is available, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		bool HasValue (string name);

		/// <summary>
		///     Initializes a setting in its string representation.
		/// </summary>
		/// <param name="name"> The name of the setting. </param>
		/// <param name="defaultValue"> The default setting value in its string representation. </param>
		/// <returns>
		///     true if the default value was used, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         Initialization means that the value is only used if the setting does not already exist and <paramref name="defaultValue" /> is not null.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		bool InitializeRawValue (string name, string defaultValue);

		/// <summary>
		///     Initializes a setting as a value of a certain type.
		/// </summary>
		/// <typeparam name="T"> The setting type. </typeparam>
		/// <param name="name"> The name of the setting. </param>
		/// <param name="defaultValue"> The default setting value. </param>
		/// <returns>
		///     true if the default value was used, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         Initialization means that the value is only used if the setting does not already exist and <paramref name="defaultValue" /> is not null.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The specified <typeparamref name="T" /> for <paramref name="defaultValue" /> is not supported by any setting converter. </exception>
		bool InitializeValue <T> (string name, T defaultValue);

		/// <summary>
		///     Reloads all settings from the underlying storage, replacing all previously loaded settings in this service.
		/// </summary>
		void Load ();

		/// <summary>
		///     Removes a setting converter and stops using it for all subsequent conversions.
		/// </summary>
		/// <param name="settingConverter"> The setting converter to remove. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already removed setting converter should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="settingConverter" /> is null. </exception>
		void RemoveConverter (ISettingConverter settingConverter);

		/// <summary>
		///     Removes a setting storage and stops using it for all subsequent operations.
		/// </summary>
		/// <param name="settingStorage"> The setting storage to remove. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already removed setting storage should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="settingStorage" /> is null. </exception>
		void RemoveStorage (ISettingStorage settingStorage);

		/// <summary>
		///     Saves all settings to the underlying storage, replacing all previously saved settings in the storage.
		/// </summary>
		void Save ();

		/// <summary>
		///     Sets a setting in its string representation.
		/// </summary>
		/// <param name="name"> The name of the setting. </param>
		/// <param name="value"> The setting value in its string representation or null to delete the setting. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		void SetRawValue (string name, string value);

		/// <summary>
		///     Sets a setting as a value of a certain type.
		/// </summary>
		/// <typeparam name="T"> The setting type. </typeparam>
		/// <param name="name"> The name of the setting. </param>
		/// <param name="value"> The setting value. For class types, specifying null deletes the setting. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The specified <typeparamref name="T" /> for <paramref name="value" /> is not supported by any setting converter. </exception>
		void SetValue <T> (string name, T value);
	}
}
