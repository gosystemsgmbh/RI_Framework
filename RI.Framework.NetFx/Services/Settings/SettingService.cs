using System;
using System.Collections.Generic;

using RI.Framework.Collections;
using RI.Framework.Composition;
using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Settings
{
	/// <summary>
	///     Implements a default setting service which is suitable for most scenarios.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This setting service manages <see cref="ISettingStorage" />s  and <see cref="ISettingConverter" />s from two sources.
	///         One are the explicitly specified storages and converters added through <see cref="AddStorage" /> and <see cref="AddConverter" />.
	///         The second is a <see cref="CompositionContainer" /> if this <see cref="SettingService" /> is added as an export (the storages and converters are then imported through composition).
	///         <see cref="Storages" /> gives the sequence containing all setting storages from all sources and <see cref="Converters" /> gives the sequence containing all setting converters from all sources.
	///     </para>
	///     <para>
	///         See <see cref="ISettingService" /> for more details.
	///     </para>
	///     <note type="note">
	///         The first created instance of <see cref="SettingService" /> is set as the singleton instance for <see cref="Singleton{T}" />
	///     </note>
	/// </remarks>
	public sealed class SettingService : ISettingService
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="SettingService" />.
		/// </summary>
		public SettingService ()
		{
			this.StoragesManual = new List<ISettingStorage>();
			this.ConvertersManual = new List<ISettingConverter>();
			this.Cache = new Dictionary<string, string>(StringComparerEx.InvariantCultureIgnoreCase);

			Singleton<ISettingService>.Ensure(() => this);
		}

		#endregion




		#region Instance Properties/Indexer

		private Dictionary<string, string> Cache { get; set; }

		[Import(typeof(ISettingConverter), Recomposable = true)]
		private Import ConvertersImported { get; set; }

		private List<ISettingConverter> ConvertersManual { get; set; }

		[Import(typeof(ISettingStorage), Recomposable = true)]
		private Import StoragesImported { get; set; }

		private List<ISettingStorage> StoragesManual { get; set; }

		#endregion




		#region Instance Methods

		private ISettingConverter GetConverterForType (Type type)
		{
			foreach (ISettingConverter converter in this.Converters)
			{
				if (converter.CanConvert(type))
				{
					return converter;
				}
			}

			return null;
		}

		private void Log (string format, params object[] args)
		{
			LogLocator.LogDebug(this.GetType().Name, format, args);
		}

		#endregion




		#region Interface: ISettingService

		/// <inheritdoc />
		public IEnumerable<ISettingConverter> Converters
		{
			get
			{
				foreach (ISettingConverter converter in this.ConvertersManual)
				{
					yield return converter;
				}

				foreach (ISettingConverter converter in this.ConvertersImported.Values<ISettingConverter>())
				{
					yield return converter;
				}
			}
		}

		/// <inheritdoc />
		public IEnumerable<ISettingStorage> Storages
		{
			get
			{
				foreach (ISettingStorage storage in this.StoragesManual)
				{
					yield return storage;
				}

				foreach (ISettingStorage storage in this.StoragesImported.Values<ISettingStorage>())
				{
					yield return storage;
				}
			}
		}

		/// <inheritdoc />
		public void AddConverter (ISettingConverter settingConverter)
		{
			if (settingConverter == null)
			{
				throw new ArgumentNullException(nameof(settingConverter));
			}

			if (this.ConvertersManual.Contains(settingConverter))
			{
				return;
			}

			this.ConvertersManual.Add(settingConverter);
		}

		/// <inheritdoc />
		public void AddStorage (ISettingStorage settingStorage)
		{
			if (settingStorage == null)
			{
				throw new ArgumentNullException(nameof(settingStorage));
			}

			if (this.StoragesManual.Contains(settingStorage))
			{
				return;
			}

			this.StoragesManual.Add(settingStorage);
		}

		/// <inheritdoc />
		public void DeleteValue (string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			this.SetRawValue(name, null);
		}

		/// <inheritdoc />
		public string GetRawValue (string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			if (this.Cache.ContainsKey(name))
			{
				return this.Cache[name];
			}

			foreach (ISettingStorage store in this.Storages)
			{
				if (!store.IsReadOnly)
				{
					continue;
				}

				string value = store.GetValue(name);
				if (value != null)
				{
					return value;
				}
			}

			foreach (ISettingStorage store in this.Storages)
			{
				if (store.IsReadOnly)
				{
					continue;
				}

				string value = store.GetValue(name);
				if (value != null)
				{
					return value;
				}
			}

			return null;
		}

		/// <inheritdoc />
		public T GetValue <T> (string name)
		{
			return (T)this.GetValue(name, typeof(T));
		}

		/// <inheritdoc />
		public object GetValue (string name, Type type)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			ISettingConverter converter = this.GetConverterForType(type);
			if (converter == null)
			{
				throw new InvalidTypeArgumentException(nameof(type));
			}

			string stringValue = this.GetRawValue(name);
			if (stringValue == null)
			{
				return null;
			}

			object value = converter.ConvertTo(type, stringValue);
			return value;
		}

		/// <inheritdoc />
		public bool HasValue (string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			if (this.Cache.ContainsKey(name))
			{
				return true;
			}

			foreach (ISettingStorage store in this.Storages)
			{
				if (store.HasValue(name))
				{
					return true;
				}
			}

			return false;
		}

		/// <inheritdoc />
		public bool InitializeRawValue (string name, string defaultValue)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			if (defaultValue == null)
			{
				return false;
			}

			if (this.HasValue(name))
			{
				return false;
			}

			this.SetRawValue(name, defaultValue);

			return true;
		}

		/// <inheritdoc />
		public bool InitializeValue <T> (string name, T defaultValue)
		{
			return this.InitializeValue(name, defaultValue, typeof(T));
		}

		/// <inheritdoc />
		public bool InitializeValue (string name, object defaultValue, Type type)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			ISettingConverter converter = this.GetConverterForType(type);
			if (converter == null)
			{
				throw new InvalidTypeArgumentException(nameof(type));
			}

			string stringValue = defaultValue == null ? null : converter.ConvertFrom(type, defaultValue);
			return this.InitializeRawValue(name, stringValue);
		}

		/// <inheritdoc />
		public void Load ()
		{
			this.Log("Loading");

			this.Cache.Clear();

			foreach (ISettingStorage store in this.Storages)
			{
				store.Load();
			}
		}

		/// <inheritdoc />
		public void RemoveConverter (ISettingConverter settingConverter)
		{
			if (settingConverter == null)
			{
				throw new ArgumentNullException(nameof(settingConverter));
			}

			if (!this.ConvertersManual.Contains(settingConverter))
			{
				return;
			}

			this.ConvertersManual.RemoveAll(settingConverter);
		}

		/// <inheritdoc />
		public void RemoveStorage (ISettingStorage settingStorage)
		{
			if (settingStorage == null)
			{
				throw new ArgumentNullException(nameof(settingStorage));
			}

			if (!this.StoragesManual.Contains(settingStorage))
			{
				return;
			}

			this.StoragesManual.RemoveAll(settingStorage);
		}

		/// <inheritdoc />
		public void Save ()
		{
			this.Log("Saving");

			this.Cache.Clear();

			foreach (ISettingStorage store in this.Storages)
			{
				if (store.IsReadOnly)
				{
					continue;
				}

				store.Save();
			}
		}

		/// <inheritdoc />
		public void SetRawValue (string name, string value)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			this.Cache.Remove(name);
			if (value != null)
			{
				this.Cache.Add(name, value);
			}

			foreach (ISettingStorage store in this.Storages)
			{
				if (store.IsReadOnly)
				{
					continue;
				}

				store.SetValue(name, value);
			}
		}

		/// <inheritdoc />
		public void SetValue <T> (string name, T value)
		{
			this.SetValue(name, value, typeof(T));
		}

		/// <inheritdoc />
		public void SetValue (string name, object value, Type type)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			ISettingConverter converter = this.GetConverterForType(type);
			if (converter == null)
			{
				throw new InvalidTypeArgumentException(nameof(type));
			}

			string stringValue = value == null ? null : converter.ConvertFrom(type, value);
			this.SetRawValue(name, stringValue);
		}

		#endregion
	}
}
