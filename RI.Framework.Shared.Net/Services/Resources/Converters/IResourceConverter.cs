using System;

using RI.Framework.Composition.Model;
using RI.Framework.Services.Resources.Sources;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Resources.Converters
{
	/// <summary>
	///     Defines the interface for a resource converter used by a resource service.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A resource converter is used by a <see cref="IResourceService" /> to convert between the raw data provided by its used underlying resource sources / resource sets and the requested resource types.
	///     </para>
	///     <para>
	///         A resource converter is also used by <see cref="IResourceSet" />s and <see cref="IResourceSource" />s to determine how a eaw resource value is to be loaded so that it can be converted by a <see cref="IResourceConverter" />.
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
		/// <remarks>
		///     <note type="note">
		///         Do not call this method directly, it is intended to be called from an <see cref="IResourceService" /> implementation.
		///     </note>
		/// </remarks>
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
		/// <remarks>
		///     <note type="note">
		///         Do not call this method directly, it is intended to be called from an <see cref="IResourceService" /> implementation.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The specified <paramref name="type" /> or the type of <paramref name="value" /> is not supported by this converter or <paramref name="type" /> and <paramref name="value" /> do not match. </exception>
		object Convert (Type type, object value);

		/// <summary>
		///     Determines resource loading information based on the extension of a resource file.
		/// </summary>
		/// <param name="extension"> The extension of the resource file (including the leading dot). </param>
		/// <returns>
		///     The loading information for files with the specified extension or null if this resource converter does not support the specified file extension.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         Do not call this method directly, it is intended to be called from an <see cref="IResourceService" /> or <see cref="IResourceSource" /> or <see cref="IResourceSet" /> implementation.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="extension" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="extension" /> is an empty string. </exception>
		ResourceLoadingInfo GetLoadingInfoFromFileExtension (string extension);
	}
}
