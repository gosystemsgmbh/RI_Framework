using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Data;




namespace RI.Framework.Utilities.Wpf.Converters
{
	[ValueConversion(typeof(object), typeof(bool), ParameterType = typeof(object))]
	public sealed class ObjectBoolConverter : IValueConverter
	{
		#region Interface: IValueConverter

		/// <inheritdoc />
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value != null;
		}

		/// <inheritdoc />
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is bool))
			{
				return null;
			}

			return (bool)value ? (parameter is IEnumerable ? ((IEnumerable)parameter).Cast<object>().FirstOrDefault() : parameter) : null;
		}

		#endregion
	}
}