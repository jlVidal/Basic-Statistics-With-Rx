using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RxStatistics.WPF
{
    public class ToStringFormatOneWayConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 1)
                return System.Convert.ChangeType(values[0], targetType, culture);
            if (values.Length >= 2 && values[0] is IFormattable)
                return (values[0] as IFormattable).ToString((string)values[1], culture);
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }
}
