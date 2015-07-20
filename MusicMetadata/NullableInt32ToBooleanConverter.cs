using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MusicMetadata
{
    [ValueConversion(typeof(int?), typeof(bool))]
    class NullableInt32ToBooleanConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var nullableInt = value as int?;
            return nullableInt.HasValue && nullableInt.Value != 0;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var nullableBool = value as bool?;
            return nullableBool.HasValue && nullableBool.Value ? 1 : 0;
        }
    }
}
