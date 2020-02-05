using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Diploma.UI.Converters
{

    public sealed class BoolToVisibilityConverter : IValueConverter
    {

        private readonly bool _visibleValue;

        public BoolToVisibilityConverter(bool visibleValue)
        {
            _visibleValue = visibleValue;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value == _visibleValue)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

}