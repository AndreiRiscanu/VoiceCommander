using System;
using System.Globalization;
using System.Windows.Data;

namespace VoiceCommander.Converters
{
    /// <summary>
    /// Because of the font size and margin, the Grid width overflows so we need to recalibrate it
    /// </summary>
    [ValueConversion(typeof(double), typeof(double))]
    class GridWidthToListViewWidthConverter : IValueConverter
    {
        public static GridWidthToListViewWidthConverter Instance = new GridWidthToListViewWidthConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value * 0.92d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
