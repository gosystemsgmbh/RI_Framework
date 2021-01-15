﻿using System;
using System.Collections;
using System.Collections.Generic;

using RI.Framework.Composition.Model;
using RI.Framework.Services.Settings.Converters;
using RI.Framework.Services.Settings.Storages;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;




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
    ///         Values are persisted with all available non-read-only setting storages and therefore are saved multiple times if multiple non-read-only setting storages are available, except when values are filtered based on their names using <see cref="ISettingStorage.WriteOnlyKnown" /> and <see cref="ISettingStorage.WritePrefixAffinities" />.
    ///     </note>
    ///     <note type="note">
    ///         When retrieving values, values from read-only setting storages have higher priority if they have the same name.
    ///         Otherwise, the order is undefined for values with the same name.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    /// TODO: Make thread-safe
    /// TODO: Move to RI.Framework.Common and implement PlayerPrefsStorage in RI.Framework.Net.Unity
    /// TODO: Implement JsonSettingStorage
    /// TODO: Implement XmlSettingStorage
    [Export]
    public interface ISettingService : ISynchronizable
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
        ///     Deletes all setting values of a specified name.
        /// </summary>
        /// <param name="name"> The name of the setting to delete. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        void DeleteValues (string name);

        /// <summary>
        ///     Deletes all setting values whose name matches a predicate.
        /// </summary>
        /// <param name="predicate"> The predicate used to test the names. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="predicate" /> is null. </exception>
        void DeleteValues (Predicate<string> predicate);

        /// <summary>
        ///     Gets the first setting value in its string representation.
        /// </summary>
        /// <param name="name"> The name of the setting. </param>
        /// <returns>
        ///     The setting value in its string representation or null if the setting is not available.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        string GetRawValue (string name);

        /// <summary>
        ///     Gets all setting values in their string representation.
        /// </summary>
        /// <param name="name"> The name of the setting. </param>
        /// <returns>
        ///     The setting values in their string representation or an empty list if the setting is not available.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        List<string> GetRawValues (string name);

        /// <summary>
        ///     Gets all setting values in their string representation based on a predicate which checks the names.
        /// </summary>
        /// <param name="predicate"> The predicate used to test the names. </param>
        /// <returns>
        ///     The values in their string representation, arranged as a dictionary where the key is the name and the value is a list of actual setting values belonging to the name.
        ///     An empty dictionary is returned if no values are found.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="predicate" /> is null. </exception>
        Dictionary<string, List<string>> GetRawValues (Predicate<string> predicate);

        /// <summary>
        ///     Gets the first setting value as a value of a certain type.
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
        ///     Gets the first setting value as a value of a certain type.
        /// </summary>
        /// <param name="name"> The name of the setting. </param>
        /// <param name="type"> The setting type. </param>
        /// <returns>
        ///     The setting value or null if the setting is not available.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> or <paramref name="type" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        /// <exception cref="InvalidTypeArgumentException"> The specified <paramref name="type" /> is not supported by any setting converter. </exception>
        object GetValue (string name, Type type);

        /// <summary>
        ///     Gets all setting values as values of a certain type.
        /// </summary>
        /// <typeparam name="T"> The setting type. </typeparam>
        /// <param name="name"> The name of the setting. </param>
        /// <returns>
        ///     The setting values or an empty list if the setting is not available.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        /// <exception cref="InvalidTypeArgumentException"> The specified <typeparamref name="T" /> is not supported by any setting converter. </exception>
        List<T> GetValues <T> (string name);

        /// <summary>
        ///     Gets all setting values as values of a certain type.
        /// </summary>
        /// <param name="name"> The name of the setting. </param>
        /// <param name="type"> The setting type. </param>
        /// <returns>
        ///     The setting values or an empty list if the setting is not available.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> or <paramref name="type" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        /// <exception cref="InvalidTypeArgumentException"> The specified <paramref name="type" /> is not supported by any setting converter. </exception>
        List<object> GetValues (string name, Type type);

        /// <summary>
        ///     Gets all settingh values based on a predicate which checks the names.
        /// </summary>
        /// <typeparam name="T"> The setting type. </typeparam>
        /// <param name="predicate"> The predicate used to test the names. </param>
        /// <returns>
        ///     The values, arranged as a dictionary where the key is the name and the value is a list of actual setting values belonging to the name.
        ///     An empty dictionary is returned if no values are found.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="predicate" /> is null. </exception>
        /// <exception cref="InvalidTypeArgumentException"> The specified <typeparamref name="T" /> is not supported by any setting converter. </exception>
        Dictionary<string, List<T>> GetValues <T> (Predicate<string> predicate);

        /// <summary>
        ///     Gets all settingh values based on a predicate which checks the names.
        /// </summary>
        /// <param name="predicate"> The predicate used to test the names. </param>
        /// <param name="type"> The setting type. </param>
        /// <returns>
        ///     The values, arranged as a dictionary where the key is the name and the value is a list of actual setting values belonging to the name.
        ///     An empty dictionary is returned if no values are found.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="predicate" /> or <paramref name="type" /> is null. </exception>
        /// <exception cref="InvalidTypeArgumentException"> The specified <paramref name="type" /> is not supported by any setting converter. </exception>
        Dictionary<string, List<object>> GetValues (Predicate<string> predicate, Type type);

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
        ///     Determines whether a setting is available whose name matches a predicate.
        /// </summary>
        /// <param name="predicate"> The predicate used to test the names. </param>
        /// <returns>
        ///     true if the setting is available, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="predicate" /> is null. </exception>
        bool HasValue (Predicate<string> predicate);

        /// <summary>
        ///     Initializes a setting in its string representation with a single value.
        /// </summary>
        /// <param name="name"> The name of the setting. </param>
        /// <param name="defaultValue"> The default setting value in its string representation. </param>
        /// <returns>
        ///     true if the default value was used, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         Initialization means that the value is only used if the setting does not already has values and <paramref name="defaultValue" /> is not null.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        bool InitializeRawValue (string name, string defaultValue);

        /// <summary>
        ///     Initializes a setting in its string representation with multiple values.
        /// </summary>
        /// <param name="name"> The name of the setting. </param>
        /// <param name="defaultValues"> The default setting values in their string representation. </param>
        /// <returns>
        ///     true if the default values were used, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         Initialization means that the values are only used if the setting does not already has values and <paramref name="defaultValues" /> is not null.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        bool InitializeRawValues (string name, IEnumerable<string> defaultValues);

        /// <summary>
        ///     Initializes a setting as a single value of a certain type.
        /// </summary>
        /// <typeparam name="T"> The setting type. </typeparam>
        /// <param name="name"> The name of the setting. </param>
        /// <param name="defaultValue"> The default setting value. </param>
        /// <returns>
        ///     true if the default value was used, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         Initialization means that the value is only used if the setting does not already has values and <paramref name="defaultValue" /> is not null.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        /// <exception cref="InvalidTypeArgumentException"> The specified <typeparamref name="T" /> for <paramref name="defaultValue" /> is not supported by any setting converter. </exception>
        bool InitializeValue <T> (string name, T defaultValue);

        /// <summary>
        ///     Initializes a setting as a single value of a certain type.
        /// </summary>
        /// <param name="name"> The name of the setting. </param>
        /// <param name="type"> The setting type. </param>
        /// <param name="defaultValue"> The default setting value. </param>
        /// <returns>
        ///     true if the default value was used, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         Initialization means that the value is only used if the setting does not already has values and <paramref name="defaultValue" /> is not null.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> or <paramref name="type" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        /// <exception cref="InvalidTypeArgumentException"> The specified <paramref name="type" /> for <paramref name="defaultValue" /> is not supported by any setting converter. </exception>
        bool InitializeValue (string name, object defaultValue, Type type);

        /// <summary>
        ///     Initializes a setting as multiple values of a certain type.
        /// </summary>
        /// <typeparam name="T"> The setting type. </typeparam>
        /// <param name="name"> The name of the setting. </param>
        /// <param name="defaultValues"> The default setting values. </param>
        /// <returns>
        ///     true if the default values were used, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         Initialization means that the values are only used if the setting does not already has values and <paramref name="defaultValues" /> is not null.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        /// <exception cref="InvalidTypeArgumentException"> The specified <typeparamref name="T" /> for <paramref name="defaultValues" /> is not supported by any setting converter. </exception>
        bool InitializeValues <T> (string name, IEnumerable<T> defaultValues);

        /// <summary>
        ///     Initializes a setting as multiple values of a certain type.
        /// </summary>
        /// <param name="name"> The name of the setting. </param>
        /// <param name="type"> The setting type. </param>
        /// <param name="defaultValues"> The default setting values. </param>
        /// <returns>
        ///     true if the default values were used, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         Initialization means that the values are only used if the setting does not already has values and <paramref name="defaultValues" /> is not null.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> or <paramref name="type" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        /// <exception cref="InvalidTypeArgumentException"> The specified <paramref name="type" /> for <paramref name="defaultValues" /> is not supported by any setting converter. </exception>
        bool InitializeValues (string name, IEnumerable defaultValues, Type type);

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
        ///     Sets a single setting value in its string representation.
        /// </summary>
        /// <param name="name"> The name of the setting. </param>
        /// <param name="value"> The setting value in its string representation or null to delete the setting. </param>
        /// <remarks>
        ///     <para>
        ///         All existing values of the same name will be overwritten.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        void SetRawValue (string name, string value);

        /// <summary>
        ///     Sets multiple setting values in their string representation.
        /// </summary>
        /// <param name="name"> The name of the setting. </param>
        /// <param name="values"> The setting values in their string representation or null or an empty sequence to delete the setting. </param>
        /// <remarks>
        ///     <para>
        ///         All existing values of the same name will be overwritten.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        void SetRawValues (string name, IEnumerable<string> values);

        /// <summary>
        ///     Sets a single setting as a value of a certain type.
        /// </summary>
        /// <typeparam name="T"> The setting type. </typeparam>
        /// <param name="name"> The name of the setting. </param>
        /// <param name="value"> The setting value. Specifying null deletes the setting. </param>
        /// <remarks>
        ///     <para>
        ///         All existing values of the same name will be overwritten.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        /// <exception cref="InvalidTypeArgumentException"> The specified <typeparamref name="T" /> for <paramref name="value" /> is not supported by any setting converter. </exception>
        void SetValue <T> (string name, T value);

        /// <summary>
        ///     Sets a single setting as a value of a certain type.
        /// </summary>
        /// <param name="name"> The name of the setting. </param>
        /// <param name="type"> The setting type. </param>
        /// <param name="value"> The setting value. Specifying null deletes the setting. </param>
        /// <remarks>
        ///     <para>
        ///         All existing values of the same name will be overwritten.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> or <paramref name="type" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        /// <exception cref="InvalidTypeArgumentException"> The specified <paramref name="type" /> for <paramref name="value" /> is not supported by any setting converter. </exception>
        void SetValue (string name, object value, Type type);

        /// <summary>
        ///     Sets multiple setting values as values of a certain type.
        /// </summary>
        /// <typeparam name="T"> The setting type. </typeparam>
        /// <param name="name"> The name of the setting. </param>
        /// <param name="values"> The setting values. Specifying null or an empty sequence deletes the setting. </param>
        /// <remarks>
        ///     <para>
        ///         All existing values of the same name will be overwritten.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        /// <exception cref="InvalidTypeArgumentException"> The specified <typeparamref name="T" /> for <paramref name="values" /> is not supported by any setting converter. </exception>
        void SetValues <T> (string name, IEnumerable<T> values);

        /// <summary>
        ///     Sets multiple setting values as values of a certain type.
        /// </summary>
        /// <param name="name"> The name of the setting. </param>
        /// <param name="type"> The setting type. </param>
        /// <param name="values"> The setting values. Specifying null or an empty sequence deletes the setting. </param>
        /// <remarks>
        ///     <para>
        ///         All existing values of the same name will be overwritten.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> or <paramref name="type" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        /// <exception cref="InvalidTypeArgumentException"> The specified <paramref name="type" /> for <paramref name="values" /> is not supported by any setting converter. </exception>
        void SetValues (string name, IEnumerable values, Type type);
    }
}
