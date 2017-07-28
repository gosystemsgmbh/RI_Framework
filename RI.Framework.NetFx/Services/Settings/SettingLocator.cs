using System;
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

		/// <inheritdoc cref="ISettingService.DeleteValue" />
		public static void DeleteValue(string name) => SettingLocator.Service?.DeleteValue(name);

		/// <inheritdoc cref="ISettingService.GetRawValue" />
		public static string GetRawValue(string name) => SettingLocator.Service?.GetRawValue(name);

		/// <inheritdoc cref="ISettingService.GetValue{T}(string)" />
		public static T GetValue<T>(string name) => SettingLocator.IsAvailable ? SettingLocator.Service.GetValue<T>(name) : default(T);

		/// <inheritdoc cref="ISettingService.GetValue(string,Type)" />
		public static object GetValue(string name, Type type) => SettingLocator.Service?.GetValue(name, type);

		/// <inheritdoc cref="ISettingService.HasValue" />
		public static bool HasValue(string name) => SettingLocator.Service?.HasValue(name) ?? false;

		/// <inheritdoc cref="ISettingService.InitializeRawValue" />
		public static bool InitializeRawValue(string name, string defaultValue) => SettingLocator.Service?.InitializeRawValue(name, defaultValue) ?? false;

		/// <inheritdoc cref="ISettingService.InitializeValue{T}(string,T)" />
		public static bool InitializeValue<T>(string name, T defaultValue) => SettingLocator.Service?.InitializeValue(name, defaultValue) ?? false;

		/// <inheritdoc cref="ISettingService.InitializeValue(string,object,Type)" />
		public static bool InitializeValue(string name, object defaultValue, Type type) => SettingLocator.Service?.InitializeValue(name, defaultValue, type) ?? false;

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
	}
}
