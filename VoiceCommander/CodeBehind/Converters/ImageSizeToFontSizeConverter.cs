using System;
using System.Globalization;
using System.Windows.Data;

namespace VoiceCommander.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    class ImageSizeToFontSizeConverter : IValueConverter
    {
        public static ImageSizeToFontSizeConverter Instance = new ImageSizeToFontSizeConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var size = (double)value * 0.60d;

            if (size > 13d)
                return size;

            return 13d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
