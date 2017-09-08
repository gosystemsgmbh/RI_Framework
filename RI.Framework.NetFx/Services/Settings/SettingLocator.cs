using System;
using System.Collections;
using System.Collections.Generic;

using RI.Framework.Services.Settings.Converters;
using RI.Framework.Services.Settings.Storages;




namespace RI.Framework.Services.Settings
{
	/// <summary>
	///     Provides a centralized and global settings provider.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="SettingLocator" /> is merely a convenience utility as it uses <see cref="ServiceLocator" /> to retrieve and use a <see cref="ISettingService" />.
	///     </para>
	/// </remarks>
	public static class SettingLocator
	{
		/// <summary>
		///     Gets whether a setting service is available and can be used by <see cref="SettingLocator" />.
		/// </summary>
		/// <value>
		///     true if a setting service is available and can be used by <see cref="SettingLocator" />, false otherwise.
		/// </value>
		public static bool IsAvailable => SettingLocator.Service != null;

		/// <summary>
		///     Gets the available setting service.
		/// </summary>
		/// <value>
		///     The available setting service or null if no setting service is available.
		/// </value>
		public static ISettingService Service => ServiceLocator.GetInstance<ISettingService>();

		/// <inheritdoc cref="ISettingService.Converters" />
		public static IEnumerable<ISettingConverter> Converters => SettingLocator.Service?.Converters ?? new ISettingConverter[0];

		/// <inheritdoc cref="ISettingService.Storages" />
		public static IEnumerable<ISettingStorage> Storages => SettingLocator.Service?.Storages ?? new ISettingStorage[0];

		/// <inheritdoc cref="ISettingService.DeleteValues(string)" />
		public static void DeleteValues(string name) => SettingLocator.Service?.DeleteValues(name);

		/// <inheritdoc cref="ISettingService.DeleteValues(Predicate{string})" />
		public static void DeleteValues(Predicate<string> predicate) => SettingLocator.Service?.DeleteValues(predicate);

		/// <inheritdoc cref="ISettingService.GetRawValue" />
		public static string GetRawValue(string name) => SettingLocator.Service?.GetRawValue(name);

		/// <inheritdoc cref="ISettingService.GetValue{T}(string)" />
		public static T GetValue<T>(string name) => SettingLocator.IsAvailable ? SettingLocator.Service.GetValue<T>(name) : default(T);

		/// <inheritdoc cref="ISettingService.GetValue(string,Type)" />
		public static object GetValue(string name, Type type) => SettingLocator.Service?.GetValue(name, type);

		/// <inheritdoc cref="ISettingService.GetRawValues" />
		public static List<string> GetRawValues(string name) => SettingLocator.Service?.GetRawValues(name);

		/// <inheritdoc cref="ISettingService.GetValues{T}(string)" />
		public static List<T> GetValues<T>(string name) => SettingLocator.Service?.GetValues<T>(name);

		/// <inheritdoc cref="ISettingService.GetValues(string,Type)" />
		public static List<object> GetValues(string name, Type type) => SettingLocator.Service?.GetValues(name, type);

		/// <inheritdoc cref="ISettingService.HasValue(string)" />
		public static bool HasValue(string name) => SettingLocator.Service?.HasValue(name) ?? false;

		/// <inheritdoc cref="ISettingService.HasValue(Predicate{string})" />
		public static bool HasValue(Predicate<string> predicate) => SettingLocator.Service?.HasValue(predicate) ?? false;

		/// <inheritdoc cref="ISettingService.InitializeRawValue" />
		public static bool InitializeRawValue(string name, string defaultValue) => SettingLocator.Service?.InitializeRawValue(name, defaultValue) ?? false;

		/// <inheritdoc cref="ISettingService.InitializeValue{T}(string,T)" />
		public static bool InitializeValue<T>(string name, T defaultValue) => SettingLocator.Service?.InitializeValue(name, defaultValue) ?? false;

		/// <inheritdoc cref="ISettingService.InitializeValue(string,object,Type)" />
		public static bool InitializeValue(string name, object defaultValue, Type type) => SettingLocator.Service?.InitializeValue(name, defaultValue, type) ?? false;

		/// <inheritdoc cref="ISettingService.InitializeRawValues" />
		public static bool InitializeRawValues(string name, IEnumerable<string> defaultValues) => SettingLocator.Service?.InitializeRawValues(name, defaultValues) ?? false;

		/// <inheritdoc cref="ISettingService.InitializeValues{T}(string,IEnumerable{T})" />
		public static bool InitializeValues<T>(string name, IEnumerable<T> defaultValues) => SettingLocator.Service?.InitializeValues(name, defaultValues) ?? false;

		/// <inheritdoc cref="ISettingService.InitializeValues(string,IEnumerable,Type)" />
		public static bool InitializeValues(string name, IEnumerable defaultValues, Type type) => SettingLocator.Service?.InitializeValues(name, defaultValues, type) ?? false;

		/// <inheritdoc cref="ISettingService.Load" />
		public static void Load() => SettingLocator.Service?.Load();

		/// <inheritdoc cref="ISettingService.Save" />
		public static void Save() => SettingLocator.Service?.Save();

		/// <inheritdoc cref="ISettingService.SetRawValue" />
		public static void SetRawValue(string name, string value) => SettingLocator.Service?.SetRawValue(name, value);

		/// <inheritdoc cref="ISettingService.SetValue{T}(string,T)" />
		public static void SetValue<T>(string name, T value) => SettingLocator.Service?.SetValue(name, value);

		/// <inheritdoc cref="ISettingService.SetValue(string,object,Type)" />
		public static void SetValue(string name, object value, Type type) => SettingLocator.Service?.SetValue(name, value, type);

		/// <inheritdoc cref="ISettingService.SetRawValues" />
		public static void SetRawValues(string name, IEnumerable<string> value) => SettingLocator.Service?.SetRawValues(name, value);

		/// <inheritdoc cref="ISettingService.SetValues{T}(string,IEnumerable{T})" />
		public static void SetValues<T>(string name, IEnumerable<T> value) => SettingLocator.Service?.SetValues(name, value);

		/// <inheritdoc cref="ISettingService.SetValues(string,IEnumerable,Type)" />
		public static void SetValues(string name, IEnumerable value, Type type) => SettingLocator.Service?.SetValues(name, value, type);
	}
}
