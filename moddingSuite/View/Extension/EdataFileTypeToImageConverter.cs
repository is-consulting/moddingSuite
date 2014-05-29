using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using moddingSuite.Model.Edata;

namespace moddingSuite.View.Extension
{
    public class EdataFileTypeToImageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((EdataFileType)value)
            {
                case EdataFileType.Ndfbin:
                    return Application.Current.Resources["ScriptIcon"] as BitmapImage;
                case EdataFileType.Dictionary:
                    return Application.Current.Resources["OpenDictionayIcon"] as BitmapImage;
                case EdataFileType.Package:
                    return Application.Current.Resources["PackageFileIcon"] as BitmapImage;
                case EdataFileType.Image:
                    return Application.Current.Resources["TextureIcon"] as BitmapImage;
                case EdataFileType.Mesh:
                    return Application.Current.Resources["MeshFileIcon"] as BitmapImage;
                case EdataFileType.Scenario:
                    return Application.Current.Resources["ScenarioIcon"] as BitmapImage;

                default:
                    return Application.Current.Resources["UnknownFileIcon"] as BitmapImage;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}