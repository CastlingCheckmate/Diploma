using System;
using System.Globalization;
using System.Windows.Data;

namespace Diploma.UI.Converters
{

    public class NullToBoolConverter : IValueConverter
    {
        private readonly bool _nullValue;

        public NullToBoolConverter(bool nullValue)
        {
            _nullValue = nullValue;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return _nullValue;
            }
            return !_nullValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

}