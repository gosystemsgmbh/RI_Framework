using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;




namespace RI.Framework.Utilities.Wpf.Converters
{
	[ValueConversion(typeof(object), typeof(Visibility), ParameterType = typeof(Visibility))]
	public sealed class ObjectVisibilityConverter : IValueConverter
	{
		/// <inheritdoc />
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (value != null) ? Visibility.Visible : (Visibility)parameter;
		}

		/// <inheritdoc />
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new InvalidOperationException(this.GetType().Name + " cannot convert back a value to the source.");
		}
	}
}