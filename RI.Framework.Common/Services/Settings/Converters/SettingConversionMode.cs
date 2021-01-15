﻿using System;




namespace RI.Framework.Services.Settings.Converters
{
	/// <summary>
	///     Describes the setting conversion mode used by a <see cref="ISettingConverter" />.
	/// </summary>
	[Serializable]
	public enum SettingConversionMode
	{
		/// <summary>
		///     The converter uses explicit conversion to/from strings, e.g. using <c> ToString </c> and <c> TryParse </c> methods.
		///     Therefore, only some particular types can be converted by the converter.
		/// </summary>
		StringConversion = 0,

		/// <summary>
		///     The converter uses serialization to/from strings using some defined format (JSON, XML, etc.).
		///     Therefore, any serializable object can be converted by the converter (where &quot;serializability&quot; is defined by the converter).
		/// </summary>
		SerializationAsString = 1,
	}
}
