using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ToadZapret.Converters;

public class BoolToStatusColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isWorking)
        {
            return isWorking ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
        }
        
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 