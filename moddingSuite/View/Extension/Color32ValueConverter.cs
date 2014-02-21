using moddingSuite.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace moddingSuite.View.Extension
{
    public class Color32ValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string val = value.ToString().Replace("#", string.Empty);

            var colArr = Utils.StringToByteArrayFastest(val);

            return Color.FromArgb(colArr[3], colArr[0], colArr[1], colArr[2]);
        }
    }
}
