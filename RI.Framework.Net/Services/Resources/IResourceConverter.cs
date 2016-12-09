using System;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Resources
{
	/// <summary>
	///     Defines the interface for a resource converter used by a resource service.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A resource converter is used by a <see cref="IResourceService" /> to convert between the raw data provided by its used underlying sources and the requested resource types.
	///     </para>
	/// </remarks>
	[Export]
	public interface IResourceConverter
	{
		/// <summary>
		///     Determines whether this converter supports conversion between two resource types.
		/// </summary>
		/// <param name="sourceType"> The source type to convert from. </param>
		/// <param name="targetType"> The target type to convert to. </param>
		/// <returns>
		///     true if this converter can convert from <paramref name="sourceType" /> to <paramref name="targetType" />, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="sourceType" /> or <paramref name="targetType" /> is null. </exception>
		bool CanConvert (Type sourceType, Type targetType);

		/// <summary>
		///     Converts a resource value to a specified type.
		/// </summary>
		/// <param name="type"> The target type. </param>
		/// <param name="value"> The resource value. </param>
		/// <returns>
		///     The resource value of <paramref name="type" /> as converted by this converter or null if <paramref name="value" /> is null.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The specified <paramref name="type" /> or the type of <paramref name="value" /> is not supported by this converter or <paramref name="type" /> and <paramref name="value" /> do not match. </exception>
		object Convert (Type type, object value);
	}
}
