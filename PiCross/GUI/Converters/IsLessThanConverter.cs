using System;
using System.Globalization;
using System.Windows.Data;

namespace GUI.Converters {

   public class IsLessThanConverter : IValueConverter {

        public static readonly IValueConverter Instance = new IsLessThanConverter();

        public object Convert( object value, Type targetType, object parameter, CultureInfo culture ) {
            var intValue = (int)value;
            var compareToValue = (int)parameter;

            return intValue < compareToValue;
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture ) {
            throw new NotImplementedException();
        }
    }
}
