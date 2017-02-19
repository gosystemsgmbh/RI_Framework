using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;




namespace RI.Framework.Utilities.Wpf.Converters
{
	[ValueConversion(typeof(bool), typeof(Visibility), ParameterType = typeof(Visibility))]
	public sealed class BoolVisibilityConverter : IValueConverter
	{
		/// <inheritdoc />
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((bool)value) ? Visibility.Visible : (Visibility)parameter;
		}

		/// <inheritdoc />
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new InvalidOperationException(this.GetType().Name + " cannot convert back a value to the source.");
		}
	}
}