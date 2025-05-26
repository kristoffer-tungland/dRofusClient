using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace dRofusClient.Windows.UI.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isNullOrEmpty = string.IsNullOrEmpty(value as string);
            if (Invert)
                isNullOrEmpty = !isNullOrEmpty;
            return isNullOrEmpty ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}