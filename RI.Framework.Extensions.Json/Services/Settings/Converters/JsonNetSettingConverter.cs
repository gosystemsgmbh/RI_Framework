using System;

using Newtonsoft.Json;

using RI.Framework.Composition.Model;




namespace RI.Framework.Services.Settings.Converters
{
	/// <summary>
	///     Implements a setting converter which can convert any JSON-serializable object.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The types supported by this setting converter are:
	///         Any JSON-serializable object.
	///     </para>
	///     <para>
	///         See <see cref="ISettingConverter" /> for more details.
	///     </para>
	/// </remarks>
	[Export]
	public sealed class JsonNetSettingConverter : ISettingConverter
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="JsonNetSettingConverter" />.
		/// </summary>
		/// <remarks>
		///     <para>
		///         Default JSON serialization settings are used.
		///     </para>
		/// </remarks>
		public JsonNetSettingConverter ()
			: this(null)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="JsonNetSettingConverter" />.
		/// </summary>
		/// <param name="settings"> The used JSON serialization settings or null to use default settings. </param>
		public JsonNetSettingConverter (JsonSerializerSettings settings)
		{
			this.Settings = settings;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used JSON serialization settings.
		/// </summary>
		/// <value>
		///     The used JSON serialization settings or null if default settings are used.
		/// </value>
		public JsonSerializerSettings Settings { get; }

		#endregion




		#region Interface: ISettingConverter

		/// <inheritdoc />
		public SettingConversionMode ConversionMode => SettingConversionMode.SerializationAsString;

		/// <inheritdoc />
		public bool CanConvert (Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			return true;
		}

		/// <inheritdoc />
		public string ConvertFrom (Type type, object value)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			string result = JsonConvert.SerializeObject(value, type, this.Settings);
			return result;
		}

		/// <inheritdoc />
		public object ConvertTo (Type type, string value)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			object result = JsonConvert.DeserializeObject(value, type, this.Settings);
			return result;
		}

		#endregion
	}
}
