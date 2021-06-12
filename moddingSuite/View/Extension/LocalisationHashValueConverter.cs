using System;
using System.Globalization;
using System.Windows.Data;
using moddingSuite.BL.Utils;
using moddingSuite.Util;

namespace moddingSuite.View.Extension
{
    public class LocalisationHashValueConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return StdUtils.ByteArrayToBigEndianHexByteString((byte[]) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Utils.StringToByteArrayFastest(value.ToString());
        }

        #endregion
    }
}