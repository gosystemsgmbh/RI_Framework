using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Linq;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition.Model;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Xml;




namespace RI.Framework.Services.Settings.Converters
{
	/// <summary>
	///     Implements a setting converter which can convert XML types.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The types supported by this setting converter are:
	///         <see cref="XmlDocument" />, <see cref="XDocument" />
	///     </para>
	///     <para>
	///         See <see cref="ISettingConverter" /> for more details.
	///     </para>
	/// </remarks>
	[Export]
	public sealed class XmlSettingConverter : ISettingConverter
	{
		#region Instance Fields

		private readonly Type[] _supportedTypes = {typeof(XmlDocument), typeof(XDocument)};

		#endregion




		#region Interface: ISettingConverter

		/// <inheritdoc />
		public SettingConversionMode ConversionMode => SettingConversionMode.StringConversion;

		/// <inheritdoc />
		public bool CanConvert (Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			return this._supportedTypes.Contains(type);
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

			XmlDocument doc = null;

			if (type == typeof(XmlDocument))
			{
				doc = (XmlDocument)value;
			}
			else if (type == typeof(XDocument))
			{
				doc = ((XDocument)value).ToXmlDocument();
			}

			if (doc != null)
			{
				using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
				{
					doc.Save(sw);
					sw.Flush();
					return sw.ToString();
				}
			}

			throw new InvalidTypeArgumentException(nameof(type));
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

			XmlDocument doc = new XmlDocument();

			using (StringReader sr = new StringReader(value))
			{
				try
				{
					doc.Load(sr);
				}
				catch (Exception exception)
				{
					throw new FormatException("The string representation is invalid and cannot be converted to the type " + type.Name + ".", exception);
				}
			}

			object finalValue;

			if (type == typeof(XmlDocument))
			{
				finalValue = doc;
			}
			else if (type == typeof(XDocument))
			{
				finalValue = doc.ToXDocument();
			}
			else
			{
				throw new InvalidTypeArgumentException(nameof(type));
			}

			if (finalValue == null)
			{
				throw new FormatException("The string representation is invalid and cannot be converted to the type " + type.Name + ".");
			}

			return finalValue;
		}

		#endregion
	}
}
