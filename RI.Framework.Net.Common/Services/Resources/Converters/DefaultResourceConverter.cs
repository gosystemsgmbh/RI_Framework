using System;
using System.Xml;
using System.Xml.Linq;

using RI.Framework.Composition.Model;
using RI.Framework.IO.INI;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Resources.Converters
{
	/// <summary>
	///     Implements a default resource converter which can convert to and from the basic types used in .NET.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The types supported by this seting converter are:
	///         <see cref="string" /> to <see cref="string" />, <see cref="string" /> to <see cref="XDocument" />, <see cref="string" /> to <see cref="XmlDocument" />, <see cref="string" /> to <see cref="IniDocument" />, and arrays of <see cref="byte" /> to arrays of <see cref="byte" />.
	///     </para>
	///     <para>
	///         See <see cref="IResourceConverter" /> for more details.
	///     </para>
	/// </remarks>
	[Export]
	public sealed class DefaultResourceConverter : IResourceConverter
	{
		#region Interface: IResourceConverter

		/// <inheritdoc />
		public bool CanConvert (Type sourceType, Type targetType)
		{
			if (sourceType == null)
			{
				throw new ArgumentNullException(nameof(sourceType));
			}

			if (targetType == null)
			{
				throw new ArgumentNullException(nameof(targetType));
			}

			if ((sourceType == typeof(string)) && (targetType == typeof(string)))
			{
				return true;
			}

			if ((sourceType == typeof(byte[])) && (targetType == typeof(byte[])))
			{
				return true;
			}

			if ((sourceType == typeof(string)) && (targetType == typeof(XDocument)))
			{
				return true;
			}

			if ((sourceType == typeof(string)) && (targetType == typeof(XmlDocument)))
			{
				return true;
			}

			if ((sourceType == typeof(string)) && (targetType == typeof(IniDocument)))
			{
				return true;
			}

			return false;
		}

		/// <inheritdoc />
		public object Convert (Type type, object value)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			if ((value is string) && (type == typeof(string)))
			{
				return value;
			}

			if ((value is byte[]) && (type == typeof(byte[])))
			{
				return value;
			}

			if ((value is string) && (type == typeof(XDocument)))
			{
				XDocument document = XDocument.Parse((string)value, LoadOptions.None);
				return document;
			}

			if ((value is string) && (type == typeof(XmlDocument)))
			{
				XmlDocument document = new XmlDocument();
				document.LoadXml((string)value);
				return document;
			}

			if ((value is string) && (type == typeof(IniDocument)))
			{
				IniDocument document = new IniDocument();
				document.Load((string)value);
				return document;
			}

			throw new InvalidTypeArgumentException(nameof(value));
		}

		/// <inheritdoc />
		public ResourceLoadingInfo GetLoadingInfoFromFileExtension (string extension)
		{
			if (extension == null)
			{
				throw new ArgumentNullException(nameof(extension));
			}

			if (extension.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(extension));
			}

			extension = extension.ToUpperInvariant().Trim();

			if (extension == ".TXT")
			{
				return new ResourceLoadingInfo(ResourceLoadingType.Text, typeof(string));
			}

			return null;
		}

		#endregion
	}
}
