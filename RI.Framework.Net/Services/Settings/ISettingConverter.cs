using System;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Settings
{
	/// <summary>
	///     Defines the interface for a setting converter used by a setting service.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A setting converter is used by a <see cref="ISettingService" /> to convert between setting values and their string representation for use in the underlying storage.
	///     </para>
	/// </remarks>
	[Export]
	public interface ISettingConverter
	{
		/// <summary>
		///     Determines whether this converter supports conversion to and from the specified setting type.
		/// </summary>
		/// <param name="type"> The setting type. </param>
		/// <returns>
		///     true if this converter can convert to and from the type specified by <paramref name="type" />, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		bool CanConvert (Type type);

		/// <summary>
		///     Converts a setting value from the specified type to its string representation.
		/// </summary>
		/// <param name="type"> The setting type. </param>
		/// <param name="value"> The setting value. </param>
		/// <returns>
		///     The string representation of the setting value as converted by this converter or null if <paramref name="value" /> is null.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The specified <paramref name="type" /> or the type of <paramref name="value" /> is not supported by this converter. </exception>
		string ConvertFrom (Type type, object value);

		/// <summary>
		///     Converts a setting value from its string representation to the specified type.
		/// </summary>
		/// <param name="type"> The setting type. </param>
		/// <param name="value"> The string representation of the setting value. </param>
		/// <returns>
		///     The setting value in the specified type as converted by this converter or null if <paramref name="value" /> is null..
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The specified <paramref name="type" /> is not supported by this converter. </exception>
		/// <exception cref="FormatException"> The string representation of <paramref name="value" /> is invalid and cannot be converted to <paramref name="type" />. </exception>
		object ConvertTo (Type type, string value);
	}
}
